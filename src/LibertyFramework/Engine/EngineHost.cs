using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine
{
    // The only ScriptHookDotNet script in Liberty Framework (ADR-0006). ScriptHookDotNet instantiates it; it boots the
    // engine and forwards every frame, the draw pass, console commands and domain unload.
    public sealed class EngineHost : Script
    {
        public EngineHost()
        {
            Interval = 0;
            // Before anything touches Liberty.Sdk types (see SdkResolver).
            SdkResolver.Install();
            try { LibertyFramework.Engine.Services.DialogGuard.Start(); }
            catch (Exception error) { RuntimeLog.Error("dialog_guard_unavailable error=" + error.Message); }
            try { Boot(); }
            catch (Exception error) { RuntimeLog.Error("engine_boot_failed error=" + error); return; }
            Tick += OnTick;
            PerFrameDrawing += OnDraw;
            BindConsoleCommand("lf", new ConsoleCommandDelegate(OnConsole), "- Liberty engine command, e.g. 'lf engine' or 'lf help'");
            AppDomain.CurrentDomain.DomainUnload += OnUnload;
        }

        // Separate method so the SDK assembly is bound only after the resolver is installed.
        private void Boot() { LibertyEngine.Boot(this); }

        internal void BindCommand(string command, ConsoleCommandDelegate handler, string help) { BindConsoleCommand(command, handler, help); }

        private void OnTick(object sender, EventArgs args)
        {
            LibertyEngine engine = LibertyEngine.Current;
            if (engine == null) { return; }
            try { engine.RunFrame(); }
            catch (Exception error) { RuntimeLog.Error("engine_frame_failed error=" + error); }
        }

        private void OnDraw(object sender, GraphicsEventArgs args)
        {
            LibertyEngine engine = LibertyEngine.Current;
            if (engine != null) { engine.Draw(args); }
        }

        private void OnConsole(ParameterCollection parameters)
        {
            LibertyEngine engine = LibertyEngine.Current;
            if (engine == null) { return; }
            string[] words = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++) { words[i] = parameters[i]; }
            string reply = engine.Commands.Execute(string.Join(" ", words), "console");
            Game.Console.Print(reply);
        }

        private void OnUnload(object sender, EventArgs args)
        {
            LibertyEngine engine = LibertyEngine.Current;
            if (engine != null) { engine.Unload(); }
        }
    }
}
