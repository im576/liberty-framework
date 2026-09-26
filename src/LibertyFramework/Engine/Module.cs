using System;
using GTA;
using Liberty.Sdk;

namespace LibertyFramework.Engine
{
    // Engine-level module (Capabilities.EngineInternal): an SDK LibertyModule that may also use ScriptHookDotNet and
    // engine internals directly. Code moved over from GTA.Script keeps Tick / PerFrameDrawing / Player /
    // BindConsoleCommand; they behave as on GTA.Script but run on the engine thread in a defined order.
    // Gameplay mods should derive from Liberty.Sdk.LibertyModule instead and use only the SDK.
    public abstract class Module : LibertyModule
    {
        protected LibertyEngine Engine { get { return LibertyEngine.Current; } }
        protected Player Player { get { return Game.LocalPlayer; } }
        public event EventHandler Tick;
        public event GraphicsEventHandler PerFrameDrawing;

        protected void BindConsoleCommand(string command, ConsoleCommandDelegate handler, string help)
        {
            LibertyEngine.Current.Host.BindCommand(command, handler, help);
        }

        protected internal override void OnUpdate() { Tick?.Invoke(this, EventArgs.Empty); }

        // ScriptHookDotNet draw pass for migrated code (the engine calls it before the SDK canvas pass).
        internal void RenderLegacy(GraphicsEventArgs args) { PerFrameDrawing?.Invoke(this, args); }
    }
}