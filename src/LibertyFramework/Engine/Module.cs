using System;
using GTA;

namespace LibertyFramework.Engine
{
    // Base class of every mechanic (ADR-0006). The engine constructs modules, starts them in Order, ticks each at its
    // Interval, draws them, and stops them. Any exception from a module disables only that module.
    //
    // New modules override Started/Update/Render/Stopped and use Engine (world, events, scheduler, services).
    // Code moved over from ScriptHookDotNet scripts may keep its Tick / PerFrameDrawing handlers and Player: they
    // behave as they did on GTA.Script, but now run on the engine thread in a defined order.
    public abstract class Module
    {
        public string Id { get; internal set; }
        public int Order { get; internal set; }
        public bool Running { get; internal set; }
        public string FailureReason { get; internal set; }

        // Milliseconds between Update calls; 0 = every frame.
        public int Interval { get; set; }

        protected LibertyEngine Engine { get { return LibertyEngine.Current; } }

        // ScriptHookDotNet compatibility.
        protected Player Player { get { return Game.LocalPlayer; } }
        public event EventHandler Tick;
        public event GraphicsEventHandler PerFrameDrawing;
        protected void BindConsoleCommand(string command, ConsoleCommandDelegate handler, string help)
        {
            LibertyEngine.Current.Host.BindCommand(command, handler, help);
        }

        internal int LastTickMs { get; set; }
        internal bool Drawing { get { return PerFrameDrawing != null; } }

        protected internal virtual void Started() { }
        protected internal virtual void Update() { Tick?.Invoke(this, EventArgs.Empty); }
        protected internal virtual void Render(GraphicsEventArgs args) { PerFrameDrawing?.Invoke(this, args); }
        protected internal virtual void Stopped() { }
        // Called when the script domain unloads (ScriptHookDotNet reload or game exit); natives are unavailable here.
        protected internal virtual void Unloading() { }
    }
}