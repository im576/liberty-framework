using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GTA;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Performance.Logic;
using LibertyFramework.Engine.Core;
using LibertyFramework.Engine.Events;
using LibertyFramework.Engine.Performance;
using LibertyFramework.Engine.Scheduling;
using LibertyFramework.Engine.Services;
using LibertyFramework.Engine.World;

namespace LibertyFramework.Engine
{
    // The Liberty engine (ADR-0006, SDK 1.0): implements Liberty.Sdk.ILiberty. Owns the world snapshot, the event bus,
    // the scheduler, every service and every module. Modules are discovered from [Liberty.Sdk.Module] manifests in this
    // assembly and scripts\LibertyFramework\mods\*.dll, checked (SDK version, dependencies, capabilities), started in
    // dependency order and run once per game frame on the one ScriptHookDotNet script (EngineHost). A module that
    // throws is stopped alone; everything it owned is released through the resource ledger.
    public sealed class LibertyEngine : ILiberty
    {
        public const string Version = "1.0.0";
        private const int PhaseWorld = 0, PhaseScheduler = 1, PhaseUi = 2, PhaseCommands = 3, PhaseDraw = 4, FirstModulePhase = 8, MaxPhases = 64;

        public static LibertyEngine Current { get; private set; }

        // Engine-typed access (engine modules and services).
        public WorldState World { get; private set; }
        public EventBus Events { get; private set; }
        public Scheduler Scheduler { get; private set; }
        public CommandRegistry Commands { get; private set; }
        public EntityService Entities { get; private set; }
        public EngineMemory Memory { get; private set; }
        public InputService Input { get; private set; }
        public AnimationService Animation { get; private set; }
        public UiService Ui { get; private set; }
        public StateService State { get; private set; }
        public ConfigService ModuleConfig { get; private set; }
        public EngineConfig Config { get; private set; }
        public PlayerService Player { get; private set; }
        public PedService Peds { get; private set; }
        public VehicleService Vehicles { get; private set; }
        public PropService Props { get; private set; }
        public WeaponService Weapons { get; private set; }
        public TaskService Tasks { get; private set; }
        public CameraService Cameras { get; private set; }
        public FxService Fx { get; private set; }
        public AudioService Audio { get; private set; }
        public StreamingService Streaming { get; private set; }
        public WorldControlService WorldControl { get; private set; }
        public BlipService Blips { get; private set; }
        public LogService Log { get; private set; }
        public PerfService Perf { get; private set; }
        public ModuleService ModuleList { get; private set; }
        public MemoryService MemoryAccess { get; private set; }
        public NativeService Natives { get; private set; }
        public ResourceLedger Ledger { get; private set; }
        public Episode Episode { get; private set; }
        public int Frame { get; private set; }
        public bool UsingCore { get { return builder != null && builder.UsingCore; } }

        internal Governor Governor { get; private set; }
        internal Watchdog Watchdog { get; private set; }
        internal EngineHost Host { get; private set; }
        internal IReadOnlyList<ModuleRuntime> Runtimes { get { return runtimes; } }
        // The module whose code is running right now (null for engine code); used for capability checks and ownership.
        internal LibertyModule CurrentModule { get; private set; }

        private readonly List<ModuleRuntime> runtimes = new List<ModuleRuntime>();
        private readonly CoreBridge core = new CoreBridge();
        private readonly string[] phaseNames = new string[MaxPhases];
        private volatile int phase = -1;
        private WorldBuilder builder;
        private bool started;
        private int lastReportMs;

        private LibertyEngine(EngineHost host)
        {
            Host = host;
            Ledger = new ResourceLedger();
            World = new WorldState();
            Events = new EventBus(Fail);
            Events.Enter = m => CurrentModule = m;
            Events.Current = () => CurrentModule;
            Scheduler = new Scheduler(Fail);
            Commands = new CommandRegistry();
            Commands.Enter = m => CurrentModule = m;
            Commands.Current = () => CurrentModule;
            Entities = new EntityService();
            Memory = new EngineMemory();
            State = new StateService();
            ModuleConfig = new ConfigService();
            Log = new LogService();
            Streaming = new StreamingService(this);
            Player = new PlayerService(this);
            Input = new InputService(this);
            Animation = new AnimationService(this);
            Ui = new UiService(this);
            Peds = new PedService(this);
            Vehicles = new VehicleService(this);
            Props = new PropService(this);
            Weapons = new WeaponService();
            Tasks = new TaskService();
            Cameras = new CameraService(this);
            Fx = new FxService(this);
            Audio = new AudioService(this);
            WorldControl = new WorldControlService(this);
            Blips = new BlipService(this);
            Perf = new PerfService(this);
            ModuleList = new ModuleService(this);
            MemoryAccess = new MemoryService(this);
            Natives = new NativeService(this);
        }

        // ILiberty (SDK view of the same services).
        string ILiberty.EngineVersion { get { return Version; } }
        Episode ILiberty.Episode { get { return Episode; } }
        IWorldState ILiberty.World { get { return World; } }
        IEventBus ILiberty.Events { get { return Events; } }
        IScheduler ILiberty.Scheduler { get { return Scheduler; } }
        IPlayer ILiberty.Player { get { return Player; } }
        IPeds ILiberty.Peds { get { return Peds; } }
        IVehicles ILiberty.Vehicles { get { return Vehicles; } }
        IProps ILiberty.Props { get { return Props; } }
        IWeapons ILiberty.Weapons { get { return Weapons; } }
        ITasks ILiberty.Tasks { get { return Tasks; } }
        ICameras ILiberty.Cameras { get { return Cameras; } }
        IAnimation ILiberty.Animation { get { return Animation; } }
        IFx ILiberty.Fx { get { return Fx; } }
        IAudio ILiberty.Audio { get { return Audio; } }
        IStreaming ILiberty.Streaming { get { return Streaming; } }
        IInput ILiberty.Input { get { return Input; } }
        IUi ILiberty.Ui { get { return Ui; } }
        IWorldControl ILiberty.WorldControl { get { return WorldControl; } }
        IBlips ILiberty.Blips { get { return Blips; } }
        IState ILiberty.State { get { return State; } }
        IConfig ILiberty.Config { get { return ModuleConfig; } }
        ICommands ILiberty.Commands { get { return Commands; } }
        ILog ILiberty.Log { get { return Log; } }
        IPerf ILiberty.Perf { get { return Perf; } }
        IModules ILiberty.Modules { get { return ModuleList; } }
        IMemory ILiberty.Memory { get { return MemoryAccess; } }
        INatives ILiberty.Natives { get { return Natives; } }

        internal static void Boot(EngineHost host)
        {
            LibertyEngine engine = new LibertyEngine(host);
            Current = engine;
            LibertyHost.Current = engine;
            engine.LoadConfig();
            // Crash capture is installed first, before anything else touches the game.
            engine.core.Load();
            engine.Governor = new Governor(engine.Config);
            engine.Watchdog = new Watchdog(engine.Config.WatchdogStallMilliseconds, engine.PhaseName, engine.core.WriteStallDump);
            engine.Watchdog.Start();
            engine.Discover();
            engine.RegisterCommands();
            RuntimeLog.Info("engine_booted version=" + Version + " sdk=" + SdkVersion.Text + " modules=" +
                string.Join(",", engine.runtimes.Select(m => m.Id).ToArray()));
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

        // ---- discovery ----

        private void Discover()
        {
            List<Assembly> assemblies = new List<Assembly> { typeof(LibertyEngine).Assembly };
            if (Config.LoadModAssemblies && Directory.Exists(LibertyPaths.ModsDirectory))
            {
                foreach (string path in Directory.GetFiles(LibertyPaths.ModsDirectory, "*.dll"))
                {
                    // From bytes, like ScriptHookDotNet: no file lock, so a mod can be replaced and picked up by ReloadScripts.
                    try { assemblies.Add(Assembly.Load(File.ReadAllBytes(path))); RuntimeLog.Info("engine_mod_assembly " + Path.GetFileName(path)); }
                    catch (Exception error) { RuntimeLog.Error("engine_mod_assembly_failed " + Path.GetFileName(path) + " error=" + error.Message); }
                }
            }
            List<KeyValuePair<ModuleManifest, Type>> found = new List<KeyValuePair<ModuleManifest, Type>>();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException error)
                {
                    RuntimeLog.Error("engine_assembly_partial " + assembly.GetName().Name + " error=" + error.LoaderExceptions.FirstOrDefault());
                    types = error.Types.Where(t => t != null).ToArray();
                }
                foreach (Type type in types)
                {
                    if (type.IsAbstract || !typeof(LibertyModule).IsAssignableFrom(type)) { continue; }
                    ModuleAttribute attribute = (ModuleAttribute)Attribute.GetCustomAttribute(type, typeof(ModuleAttribute));
                    if (attribute == null) { RuntimeLog.Error("engine_module_unmarked " + type.FullName); continue; }
                    ModuleManifest manifest = ModuleManifest.From(attribute, assembly.GetName().Name);
                    if (!SdkVersion.IsCompatible(manifest.SdkVersion))
                    {
                        RuntimeLog.Error("engine_module_sdk_incompatible " + manifest.Id + " wants=" + manifest.SdkVersion + " engine=" + SdkVersion.Text);
                        continue;
                    }
                    if (typeof(Module).IsAssignableFrom(type) && !manifest.Has(Capabilities.EngineInternal))
                    {
                        RuntimeLog.Error("engine_module_missing_capability " + manifest.Id + " engine modules must declare " + Capabilities.EngineInternal);
                    }
                    found.Add(new KeyValuePair<ModuleManifest, Type>(manifest, type));
                }
            }
            List<KeyValuePair<ModuleManifest, Type>> accepted = new List<KeyValuePair<ModuleManifest, Type>>();
            foreach (KeyValuePair<ModuleManifest, Type> pair in found.OrderBy(p => p.Key.Order).ThenBy(p => p.Key.Id, StringComparer.Ordinal))
            {
                if (Config.DisabledModules.Contains(pair.Key.Id)) { RuntimeLog.Info("engine_module_disabled_by_config " + pair.Key.Id); continue; }
                if (accepted.Any(p => p.Key.Id == pair.Key.Id)) { RuntimeLog.Error("engine_module_duplicate_id " + pair.Key.Id + " in " + pair.Key.Assembly); continue; }
                accepted.Add(pair);
            }
            foreach (KeyValuePair<ModuleManifest, Type> pair in DependencyOrder(accepted))
            {
                if (runtimes.Count + FirstModulePhase >= MaxPhases) { RuntimeLog.Error("engine_module_limit " + pair.Key.Id); continue; }
                try
                {
                    LibertyModule module = (LibertyModule)Activator.CreateInstance(pair.Value, true);
                    module.Manifest = pair.Key;
                    ModuleRuntime runtime = new ModuleRuntime(module, FirstModulePhase + runtimes.Count, pair.Key.Has(Capabilities.EngineInternal));
                    runtime.BudgetMs = pair.Key.BudgetMs > 0 ? pair.Key.BudgetMs : Config.ModuleBudgetMs;
                    runtimes.Add(runtime);
                    RuntimeLog.Info("engine_module_loaded " + pair.Key.Id + " version=" + pair.Key.Version + " assembly=" + pair.Key.Assembly +
                        (pair.Key.Requires.Length > 0 ? " requires=" + string.Join(",", pair.Key.Requires) : "") +
                        (pair.Key.Capabilities.Length > 0 ? " capabilities=" + string.Join(",", pair.Key.Capabilities) : ""));
                }
                catch (Exception error)
                {
                    RuntimeLog.Error("engine_module_construct_failed " + pair.Key.Id + " error=" + (error.InnerException ?? error));
                }
            }
        }

        // Stable topological order: a module comes after everything it requires; otherwise Order, then id. A module
        // whose requirement is missing or part of a cycle is dropped (logged).
        private static List<KeyValuePair<ModuleManifest, Type>> DependencyOrder(List<KeyValuePair<ModuleManifest, Type>> modules)
        {
            List<KeyValuePair<ModuleManifest, Type>> ordered = new List<KeyValuePair<ModuleManifest, Type>>();
            HashSet<string> placed = new HashSet<string>();
            HashSet<string> known = new HashSet<string>(modules.Select(m => m.Key.Id));
            List<KeyValuePair<ModuleManifest, Type>> pending = new List<KeyValuePair<ModuleManifest, Type>>();
            foreach (KeyValuePair<ModuleManifest, Type> m in modules)
            {
                string missing = m.Key.Requires.FirstOrDefault(r => !known.Contains(r));
                if (missing != null) { RuntimeLog.Error("engine_module_dependency_missing " + m.Key.Id + " requires=" + missing); continue; }
                pending.Add(m);
            }
            bool progress = true;
            while (pending.Count > 0 && progress)
            {
                progress = false;
                for (int i = 0; i < pending.Count; i++)
                {
                    if (!pending[i].Key.Requires.All(placed.Contains)) { continue; }
                    ordered.Add(pending[i]);
                    placed.Add(pending[i].Key.Id);
                    pending.RemoveAt(i);
                    progress = true;
                    break;
                }
            }
            foreach (KeyValuePair<ModuleManifest, Type> m in pending) { RuntimeLog.Error("engine_module_dependency_cycle " + m.Key.Id); }
            return ordered;
        }

        // ---- commands ----

        private void RegisterCommands()
        {
            Commands.Register(null, "engine", "engine status", args => Status());
            Commands.Register(null, "modules", "list modules: state, average/max ms, interval, throttles", args => ModulesReport());
            Commands.Register(null, "perf", "frame time, pressure and memory", args => PerfReport());
            Commands.Register(null, "costs", "named cost samples since the last call (resets them)", args => CostMeter.ReportAndReset());
            Commands.Register(null, "pools", "game pool occupancy (peds, vehicles, objects)", args => PoolsReport());
            Commands.Register(null, "natives", "raw native calls made through the SDK, per module", args => Natives.Report());
            Commands.Register(null, "owned", "owned <module> - resources a module holds", args => OwnedReport(args));
            Commands.Register(null, "stop", "stop <module> - stop a module and release everything it owns", args => StopCommand(args));
        }

        public string Status()
        {
            return "engine " + Version + " sdk " + SdkVersion.Text + " frame=" + Frame + " core=" + (UsingCore ? "on" : "off") +
                " peds=" + World.Peds.Count + " vehicles=" + World.Vehicles.Count + " modules=" + runtimes.Count(m => m.Module.Running) + "/" + runtimes.Count +
                " coroutines=" + Scheduler.Running + " resources=" + Ledger.Count + " episode=" + Episode;
        }

        private string ModulesReport()
        {
            return string.Join(" | ", runtimes.Select(m => m.Id + (m.Module.Running ? "" : "(off: " + (m.Module.FailureReason ?? "stopped") + ")") +
                " avg=" + m.AverageMs.ToString("0.000") + " max=" + m.MaxMs.ToString("0.00") + " int=" + m.Module.Interval +
                (m.Throttles > 0 ? " throttles=" + m.Throttles : "")).ToArray());
        }

        private string PerfReport()
        {
            MemoryProbe.Sample memory = Watchdog.Memory;
            return "frame_ms=" + Perf.FrameMs.ToString("0.00") + " p95_ms=" + Perf.FrameP95Ms.ToString("0.00") + " pressure=" + Perf.Pressure.ToString("0.00") +
                " private_mb=" + (memory.PrivateBytes >> 20) + " working_set_mb=" + (memory.WorkingSetBytes >> 20) +
                " address_free_mb=" + (memory.AddressSpaceFreeBytes >> 20) + " largest_free_block_mb=" + (memory.LargestFreeBlockBytes >> 20) +
                " managed_mb=" + (Perf.ManagedHeapBytes >> 20) + " physical_load=" + memory.PhysicalLoadPercent + "%" + " core_us=" + World.CoreMicroseconds.ToString("0.0");
        }

        private string PoolsReport()
        {
            LcPools p = World.Pools;
            return "peds=" + p.PedsUsed + "/" + p.PedsSize + " vehicles=" + p.VehiclesUsed + "/" + p.VehiclesSize + " objects=" + p.ObjectsUsed + "/" + p.ObjectsSize;
        }

        private string OwnedReport(string[] args)
        {
            ModuleRuntime m = Find(args.Length > 0 ? args[0] : "");
            if (m == null) { return "unknown module"; }
            Dictionary<string, int> summary = Ledger.Summary(m.Module);
            return m.Id + ": " + (summary.Count == 0 ? "nothing" : string.Join(" ", summary.Select(p => p.Key + "=" + p.Value).ToArray()));
        }

        private string StopCommand(string[] args)
        {
            ModuleRuntime m = Find(args.Length > 0 ? args[0] : "");
            if (m == null) { return "unknown module"; }
            StopModule(m, "stopped by command", false);
            return "stopped " + m.Id;
        }

        private ModuleRuntime Find(string id) { return runtimes.FirstOrDefault(r => string.Equals(r.Id, id, StringComparison.OrdinalIgnoreCase)); }

        public T Module<T>() where T : LibertyModule
        {
            foreach (ModuleRuntime m in runtimes) { T typed = m.Module as T; if (typed != null) { return typed; } }
            return null;
        }

        // ---- capability checks ----

        // Throws inside the calling module (which stops it) when it did not declare the capability. EngineInternal
        // implies every capability. Engine code (owner null) is always allowed.
        internal void RequireCapability(LibertyModule owner, string capability)
        {
            if (owner == null || owner.Manifest == null) { return; }
            if (owner.Manifest.Has(capability) || owner.Manifest.Has(Capabilities.EngineInternal)) { return; }
            throw new UnauthorizedAccessException("module " + owner.Id + " must declare capability '" + capability + "'");
        }

        // ---- frame ----

        private string PhaseName()
        {
            int p = phase;
            return p >= 0 && p < MaxPhases && phaseNames[p] != null ? phaseNames[p] : "idle";
        }

        private void SetPhase(int index)
        {
            phase = index;
            core.SetPhase(index);
        }

        internal void RunFrame()
        {
            long frameStart = Stopwatch.GetTimestamp();
            Frame++;
            Watchdog.FrameBegin(Frame);
            try
            {
                Perf.OnFrame();
                if (!started) { Start(); }
                SetPhase(PhaseWorld);
                long mark = Stopwatch.GetTimestamp();
                try { builder.Build(Frame); }
                catch (Exception error) { RuntimeLog.Error("engine_world_failed error=" + error); }
                CostMeter.Add("engine.world", mark);

                try { Input.Poll(); }
                catch (Exception error) { RuntimeLog.Error("engine_input_failed error=" + error.Message); }
                ModuleConfig.Poll(Fail);
                SetPhase(PhaseScheduler);
                mark = Stopwatch.GetTimestamp();
                Scheduler.Run(m => CurrentModule = m);
                CostMeter.Add("engine.scheduler", mark);
                UpdateGovernor();

                int now = Environment.TickCount;
                for (int i = 0; i < runtimes.Count; i++)
                {
                    ModuleRuntime m = runtimes[i];
                    LibertyModule module = m.Module;
                    if (!module.Running) { continue; }
                    if (module.Interval > 0 && unchecked(now - m.LastTickMs) < module.Interval) { continue; }
                    m.LastTickMs = now;
                    SetPhase(m.Phase);
                    CurrentModule = module;
                    mark = Stopwatch.GetTimestamp();
                    try { module.OnUpdate(); }
                    catch (Exception error) { Fail(module, error); }
                    CurrentModule = null;
                    CostMeter.Add(m.CostName, mark);
                    float ms = (float)((Stopwatch.GetTimestamp() - mark) * 1000.0 / Stopwatch.Frequency);
                    string reason = Governor.Record(m, ms);
                    if (reason != null) { StopModule(m, reason, true); }
                }

                SetPhase(PhaseUi);
                try { Ui.Update(); }
                catch (Exception error) { RuntimeLog.Error("engine_ui_update_failed error=" + error); }
                SetPhase(PhaseCommands);
                Commands.PumpFileChannel();
                Entities.Flush(false);
                CostMeter.Add("engine.frame", frameStart);
                if (unchecked(now - lastReportMs) >= 30000)
                {
                    lastReportMs = now;
                    RuntimeLog.Info("engine_status " + Status() + " " + PerfReport());
                }
            }
            finally
            {
                SetPhase(-1);
                CurrentModule = null;
                Watchdog.FrameEnd();
            }
        }

        private void UpdateGovernor()
        {
            try
            {
                WorldControl.GovernorScale = Config.AdaptiveDensityFloor > 0 ? 1f - (1f - Config.AdaptiveDensityFloor) * Governor.Pressure : 1f;
                Governor.Update(Perf.FrameMs, Watchdog.Memory.AddressSpaceFreeBytes);
                WorldControl.Update();
            }
            catch (Exception error) { RuntimeLog.Error("engine_governor_failed error=" + error.Message); }
        }

        private void Start()
        {
            started = true;
            try { Episode = (Episode)(int)Game.CurrentEpisode; }
            catch (Exception error) { RuntimeLog.Error("engine_episode_unknown error=" + error.Message); }
            if (Memory.Resolve() && Config.CoreEnabled) { core.Initialize(Memory.Scanner, Memory.Addresses); }
            else if (!Config.CoreEnabled) { RuntimeLog.Info("engine_core_disabled_by_config"); }
            RegisterPhases();
            builder = new WorldBuilder(core, World, Events, Config);
            Entities.RecoverOrphans();
            RuntimeLog.Info("engine_started episode=" + Episode + " game=" + SafeVersion());
            foreach (ModuleRuntime m in runtimes) { StartModule(m); }
        }

        private string SafeVersion()
        {
            try { return MemoryAccess.GameVersion; }
            catch (Exception error) { RuntimeLog.Error("engine_game_version_unknown error=" + error.Message); return "unknown"; }
        }

        private void RegisterPhases()
        {
            phaseNames[PhaseWorld] = "engine.world"; phaseNames[PhaseScheduler] = "engine.scheduler"; phaseNames[PhaseUi] = "engine.ui";
            phaseNames[PhaseCommands] = "engine.commands"; phaseNames[PhaseDraw] = "engine.draw";
            foreach (ModuleRuntime m in runtimes) { phaseNames[m.Phase] = "module." + m.Id; }
            for (int i = 0; i < MaxPhases; i++) { if (phaseNames[i] != null) { core.RegisterPhase(i, phaseNames[i]); } }
        }

        private void StartModule(ModuleRuntime m)
        {
            LibertyModule module = m.Module;
            foreach (string required in module.Manifest.Requires)
            {
                ModuleRuntime dependency = Find(required);
                if (dependency == null || !dependency.Module.Running)
                {
                    module.FailureReason = "requires " + required + " (not running)";
                    RuntimeLog.Error("engine_module_not_started " + module.Id + " reason=" + module.FailureReason);
                    return;
                }
            }
            module.Running = true;
            m.Started = true;
            SetPhase(m.Phase);
            CurrentModule = module;
            try { module.OnStart(); RuntimeLog.Info("engine_module_started " + module.Id); }
            catch (Exception error) { Fail(module, error); }
            finally { CurrentModule = null; }
        }

        internal void Draw(GraphicsEventArgs args)
        {
            try
            {
                foreach (ModuleRuntime m in runtimes)
                {
                    Module legacy = m.Module as Module;
                    if (legacy == null || !legacy.Running) { continue; }
                    try { legacy.RenderLegacy(args); }
                    catch (Exception error) { RuntimeLog.Error("engine_draw_failed " + m.Id + " error=" + error); }
                }
                Ui.Draw(args, DrawModules);
            }
            catch (Exception error) { RuntimeLog.Error("engine_ui_draw_failed error=" + error.Message); }
        }

        private void DrawModules(ICanvas canvas)
        {
            foreach (ModuleRuntime m in runtimes)
            {
                if (!m.Module.Running) { continue; }
                canvas.Opacity = 1f;
                try { m.Module.OnDraw(canvas); }
                catch (Exception error) { RuntimeLog.Error("engine_canvas_failed " + m.Id + " error=" + error); }
            }
        }

        // ---- stopping ----

        internal void Fail(LibertyModule module, Exception error)
        {
            if (module == null) { RuntimeLog.Error("engine_error error=" + error); return; }
            ModuleRuntime m = runtimes.FirstOrDefault(r => r.Module == module);
            if (m == null || !module.Running) { RuntimeLog.Error("engine_module_error_after_stop " + module.Id + " error=" + error.Message); return; }
            RuntimeLog.Error("engine_module_failed " + module.Id + " error=" + error);
            StopModule(m, error.Message, true);
        }

        // Stops one module: its OnStop, coroutines, subscriptions, commands, config watches and every owned resource;
        // then every module that requires it.
        private void StopModule(ModuleRuntime m, string reason, bool failed)
        {
            LibertyModule module = m.Module;
            if (!module.Running) { return; }
            module.Running = false;
            module.FailureReason = reason;
            LibertyModule previous = CurrentModule;
            CurrentModule = module;
            try { module.OnStop(); }
            catch (Exception stopError) { RuntimeLog.Error("engine_module_stop_failed " + module.Id + " error=" + stopError.Message); }
            finally { CurrentModule = previous; }
            Scheduler.StopOwnedBy(module);
            Events.RemoveOwner(module);
            Commands.RemoveOwner(module);
            ModuleConfig.RemoveOwner(module);
            int released = 0;
            try { released = Ledger.ReleaseAll(module); }
            catch (Exception cleanupError) { RuntimeLog.Error("engine_module_cleanup_failed " + module.Id + " error=" + cleanupError.Message); }
            Entities.Flush(true);
            RuntimeLog.Info("engine_module_stopped " + module.Id + " released=" + released + " reason=" + reason);
            if (failed) { Events.Publish(new Liberty.Sdk.Events.ModuleFailed { ModuleId = module.Id, Error = reason }); }
            foreach (ModuleRuntime dependent in runtimes.Where(r => r.Module.Running && r.Manifest.Requires.Contains(module.Id)).ToList())
            {
                StopModule(dependent, "requires " + module.Id + " (stopped)", failed);
            }
        }

        internal void Unload()
        {
            foreach (ModuleRuntime m in runtimes)
            {
                try { m.Module.OnUnload(); } catch (Exception error) { RuntimeLog.Error("engine_module_unload_failed " + m.Id + " error=" + error.Message); }
            }
            // Memory patches need no game functions: restore them now. Entities are deleted by the next load's journal pass.
            try { Ledger.ReleaseKind("patch"); } catch (Exception error) { RuntimeLog.Error("engine_patch_restore_failed error=" + error.Message); }
            Entities.SaveJournal();
            Watchdog.Stop();
            core.Shutdown();
            RuntimeLog.Info("engine_unloaded frame=" + Frame);
            LibertyHost.Current = null;
            Current = null;
        }
    }
}
