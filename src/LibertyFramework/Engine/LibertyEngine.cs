using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GTA;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Performance.Logic;
using LibertyFramework.Engine.Core;
using LibertyFramework.Engine.Events;
using LibertyFramework.Engine.Scheduling;
using LibertyFramework.Engine.Services;
using LibertyFramework.Engine.World;

namespace LibertyFramework.Engine
{
    // The Liberty engine (ADR-0006): owns the world snapshot, the event bus, the scheduler, the services and every
    // module, and runs them in a fixed order once per game frame on the one ScriptHookDotNet script (EngineHost).
    public sealed class LibertyEngine
    {
        public static LibertyEngine Current { get; private set; }

        public WorldState World { get; private set; }
        public EventBus Events { get; private set; }
        public Scheduler Scheduler { get; private set; }
        public CommandRegistry Commands { get; private set; }
        public EntityService Entities { get; private set; }
        public EngineMemory Memory { get; private set; }
        public InputService Input { get; private set; }
        public AnimationService Animations { get; private set; }
        public UiService Ui { get; private set; }
        public StateService State { get; private set; }
        public EngineConfig Config { get; private set; }
        public IReadOnlyList<Module> Modules { get { return modules; } }
        public int Frame { get; private set; }
        public bool UsingCore { get { return builder != null && builder.UsingCore; } }

        internal EngineHost Host { get; private set; }

        private readonly List<Module> modules = new List<Module>();
        private readonly CoreBridge core = new CoreBridge();
        private WorldBuilder builder;
        private bool started;
        private int lastReportMs;

        private LibertyEngine(EngineHost host)
        {
            Host = host;
            World = new WorldState();
            Events = new EventBus(Fail);
            Scheduler = new Scheduler(Fail);
            Commands = new CommandRegistry();
            Entities = new EntityService();
            Memory = new EngineMemory();
            Input = new InputService();
            Animations = new AnimationService();
            Ui = new UiService();
            State = new StateService();
        }

        internal static void Boot(EngineHost host)
        {
            LibertyEngine engine = new LibertyEngine(host);
            Current = engine;
            engine.LoadConfig();
            engine.Discover();
            engine.RegisterCommands();
            RuntimeLog.Info("engine_booted modules=" + string.Join(",", engine.modules.Select(m => m.Id).ToArray()));
        }

        private void LoadConfig()
        {
            try
            {
                EngineConfig config = File.Exists(LibertyPaths.EngineConfig) ? JsonStore.Load<EngineConfig>(LibertyPaths.EngineConfig) : EngineConfig.Defaults();
                config.Validate();
                Config = config;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("engine_config_rejected using defaults error=" + error.Message);
                Config = EngineConfig.Defaults();
            }
        }

        // Every [Module] class in this assembly, plus mods\*.dll when enabled, constructed in Order.
        private void Discover()
        {
            List<Assembly> assemblies = new List<Assembly> { typeof(LibertyEngine).Assembly };
            if (Config.LoadModAssemblies && Directory.Exists(LibertyPaths.ModsDirectory))
            {
                foreach (string path in Directory.GetFiles(LibertyPaths.ModsDirectory, "*.dll"))
                {
                    try { assemblies.Add(Assembly.LoadFrom(path)); RuntimeLog.Info("engine_mod_assembly " + Path.GetFileName(path)); }
                    catch (Exception error) { RuntimeLog.Error("engine_mod_assembly_failed " + Path.GetFileName(path) + " error=" + error.Message); }
                }
            }
            List<KeyValuePair<ModuleAttribute, Type>> found = new List<KeyValuePair<ModuleAttribute, Type>>();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException error) { types = error.Types.Where(t => t != null).ToArray(); }
                foreach (Type type in types)
                {
                    if (type.IsAbstract || !typeof(Module).IsAssignableFrom(type)) { continue; }
                    ModuleAttribute attribute = (ModuleAttribute)Attribute.GetCustomAttribute(type, typeof(ModuleAttribute));
                    if (attribute == null) { RuntimeLog.Error("engine_module_unmarked " + type.FullName); continue; }
                    found.Add(new KeyValuePair<ModuleAttribute, Type>(attribute, type));
                }
            }
            foreach (KeyValuePair<ModuleAttribute, Type> pair in found.OrderBy(p => p.Key.Order).ThenBy(p => p.Key.Id, StringComparer.Ordinal))
            {
                if (Config.DisabledModules.Contains(pair.Key.Id)) { RuntimeLog.Info("engine_module_disabled_by_config " + pair.Key.Id); continue; }
                if (modules.Any(m => m.Id == pair.Key.Id)) { RuntimeLog.Error("engine_module_duplicate_id " + pair.Key.Id); continue; }
                try
                {
                    Module module = (Module)Activator.CreateInstance(pair.Value, true);
                    module.Id = pair.Key.Id;
                    module.Order = pair.Key.Order;
                    modules.Add(module);
                }
                catch (Exception error)
                {
                    RuntimeLog.Error("engine_module_construct_failed " + pair.Key.Id + " error=" + (error.InnerException ?? error));
                }
            }
        }

        private void RegisterCommands()
        {
            Commands.Register(null, "engine", "engine status", args => Status());
            Commands.Register(null, "modules", "list modules and their state", args => string.Join(" ", modules.Select(m => m.Id + (m.Running ? "" : "(off)")).ToArray()));
        }

        public string Status()
        {
            return "engine frame=" + Frame + " core=" + (UsingCore ? "on" : "off") + " peds=" + World.Peds.Count +
                " modules=" + modules.Count(m => m.Running) + "/" + modules.Count + " coroutines=" + Scheduler.Count;
        }

        public T Module<T>() where T : Module
        {
            foreach (Module module in modules) { T typed = module as T; if (typed != null) { return typed; } }
            return null;
        }

        internal void RunFrame()
        {
            long frameStart = Stopwatch.GetTimestamp();
            Frame++;
            if (!started) { Start(); }
            long mark = Stopwatch.GetTimestamp();
            try { builder.Build(Frame); }
            catch (Exception error) { RuntimeLog.Error("engine_world_failed error=" + error); }
            CostMeter.Add("engine.world", mark);

            try { Input.Poll(); }
            catch (Exception error) { RuntimeLog.Error("engine_input_failed error=" + error.Message); }
            mark = Stopwatch.GetTimestamp();
            Scheduler.Run();
            CostMeter.Add("engine.scheduler", mark);

            int now = Environment.TickCount;
            foreach (Module module in modules)
            {
                if (!module.Running) { continue; }
                if (module.Interval > 0 && unchecked(now - module.LastTickMs) < module.Interval) { continue; }
                module.LastTickMs = now;
                mark = Stopwatch.GetTimestamp();
                try { module.Update(); }
                catch (Exception error) { Fail(module, error); }
                CostMeter.Add("module." + module.Id, mark);
            }
            Commands.PumpFileChannel();
            CostMeter.Add("engine.frame", frameStart);
            if (unchecked(now - lastReportMs) >= 30000)
            {
                lastReportMs = now;
                RuntimeLog.Info("engine_status " + Status() + " core_us=" + World.CoreMicroseconds.ToString("0.0"));
            }
        }

        private void Start()
        {
            started = true;
            if (Memory.Resolve() && Config.CoreEnabled) { core.Initialize(Memory.Scanner, Memory.Addresses); }
            else if (!Config.CoreEnabled) { RuntimeLog.Info("engine_core_disabled_by_config"); }
            builder = new WorldBuilder(core, World, Events, Config);
            Entities.RecoverOrphans();
            foreach (Module module in modules)
            {
                module.Running = true;
                try { module.Started(); RuntimeLog.Info("engine_module_started " + module.Id); }
                catch (Exception error) { Fail(module, error); }
            }
        }

        internal void Draw(GraphicsEventArgs args)
        {
            foreach (Module module in modules)
            {
                if (!module.Running) { continue; }
                try { module.Render(args); }
                catch (Exception error) { Fail(module, error); }
            }
            try { Ui.Draw(args); }
            catch (Exception error) { RuntimeLog.Error("engine_ui_draw_failed error=" + error.Message); }
        }

        // Disables one module: stops its coroutines, drops its subscriptions and entities, and tells the others.
        internal void Fail(Module module, Exception error)
        {
            if (module == null) { RuntimeLog.Error("engine_error error=" + error); return; }
            if (!module.Running) { return; }
            module.Running = false;
            module.FailureReason = error.Message;
            RuntimeLog.Error("engine_module_failed " + module.Id + " error=" + error);
            try { module.Stopped(); } catch (Exception stopError) { RuntimeLog.Error("engine_module_stop_failed " + module.Id + " error=" + stopError.Message); }
            Scheduler.StopOwnedBy(module);
            Events.RemoveOwner(module);
            Commands.RemoveOwner(module);
            try { Entities.ReleaseAll(module); } catch (Exception cleanupError) { RuntimeLog.Error("engine_module_cleanup_failed " + module.Id + " error=" + cleanupError.Message); }
            Events.Publish(new ModuleFailed { ModuleId = module.Id, Error = error.Message });
        }

        internal void Unload()
        {
            foreach (Module module in modules)
            {
                try { module.Unloading(); } catch (Exception error) { RuntimeLog.Error("engine_module_unload_failed " + module.Id + " error=" + error.Message); }
            }
            Entities.SaveJournal();
            core.Shutdown();
            RuntimeLog.Info("engine_unloaded frame=" + Frame);
            Current = null;
        }
    }
}