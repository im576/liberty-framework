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
            try { LibertyEngine.Boot(this); }
            catch (Exception error) { RuntimeLog.Error("engine_boot_failed error=" + error); return; }
            Tick += OnTick;
            PerFrameDrawing += OnDraw;
            BindConsoleCommand("lf", new ConsoleCommandDelegate(OnConsole), "- Liberty engine command, e.g. 'lf engine' or 'lf help'");
            AppDomain.CurrentDomain.DomainUnload += OnUnload;
        }

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