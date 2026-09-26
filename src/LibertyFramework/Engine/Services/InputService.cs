using System.Collections.Generic;
using Liberty.Sdk;
using LibertyFramework.Core.Input;
using Keys = System.Windows.Forms.Keys;

namespace LibertyFramework.Engine.Services
{
    // SDK IInput: one controller poll per engine frame shared by every module (XInput, read directly so menus work while
    // game controls are locked), with edge detection. Keyboard keys are read through ScriptHookDotNet's key state;
    // edges are tracked for keys a module has asked about. Capture gives one module the input and locks the player's
    // controls (through the player service) until it releases or stops.
    public sealed class InputService : IInput
    {
        private readonly LibertyEngine engine;
        private readonly ControllerInput pad = new ControllerInput();
        private readonly Dictionary<int, bool[]> keys = new Dictionary<int, bool[]>();
        private ushort previous;
        private LibertyModule capturer;
        private readonly List<KeyValuePair<LibertyModule, object>> uiCaptures = new List<KeyValuePair<LibertyModule, object>>();

        internal InputService(LibertyEngine engine) { this.engine = engine; }

        internal void Poll()
        {
            previous = pad.Buttons;
            pad.Poll();
            foreach (KeyValuePair<int, bool[]> pair in keys)
            {
                pair.Value[0] = pair.Value[1];
                pair.Value[1] = GTA.Game.isKeyPressed((Keys)pair.Key);
            }
            if (capturer != null && !capturer.Running) { Release(capturer); }
        }

        public bool PadConnected { get { return pad.Connected; } }
        public bool Down(PadButton button) { return (pad.Buttons & (ushort)button) != 0; }
        public bool Pressed(PadButton button) { return (pad.Buttons & (ushort)button) != 0 && (previous & (ushort)button) == 0; }
        public bool Released(PadButton button) { return (pad.Buttons & (ushort)button) == 0 && (previous & (ushort)button) != 0; }
        public float LeftX { get { return (float)pad.LeftX; } }
        public float LeftY { get { return (float)pad.LeftY; } }
        public float RightX { get { return (float)pad.RightX; } }
        public float RightY { get { return (float)pad.RightY; } }
        public float LeftTrigger { get { return (float)pad.LeftTrigger; } }
        public float RightTrigger { get { return (float)pad.RightTrigger; } }

        public bool KeyDown(VirtualKey key) { return GTA.Game.isKeyPressed((Keys)(int)key); }

        public bool KeyPressed(VirtualKey key)
        {
            bool[] state;
            if (!keys.TryGetValue((int)key, out state))
            {
                // First question about this key: start tracking; an edge can be reported from the next frame on.
                bool down = KeyDown(key);
                keys[(int)key] = new[] { down, down };
                return false;
            }
            return state[1] && !state[0];
        }

        public bool Capture(LibertyModule owner)
        {
            engine.RequireCapability(owner, Capabilities.InputCapture);
            if (capturer == owner) { return true; }
            if (capturer != null) { return false; }
            capturer = owner;
            object key = new KeyValuePair<LibertyModule, string>(owner, "input");
            engine.Player.LockControlInternal(key);
            engine.Ledger.Add(owner, "input", 0, () => { if (capturer == owner) { capturer = null; } engine.Player.UnlockControlInternal(key); });
            return true;
        }

        public void Release(LibertyModule owner) { engine.Ledger.Release(owner, "input", 0); }

        public LibertyModule CapturedBy { get { return capturer ?? (uiCaptures.Count > 0 ? uiCaptures[uiCaptures.Count - 1].Key : null); } }

        // Open menus: the top menu's owner sees input, and player control stays locked while any menu is open.
        internal void CaptureForUi(LibertyModule owner, object menu, bool lockControl)
        {
            uiCaptures.Add(new KeyValuePair<LibertyModule, object>(owner, menu));
            if (lockControl) { engine.Player.LockControlInternal(menu); }
        }

        internal void ReleaseForUi(LibertyModule owner, object menu)
        {
            uiCaptures.RemoveAll(pair => pair.Value == menu);
            engine.Player.UnlockControlInternal(menu);
        }

        public bool PressedFor(LibertyModule module, PadButton button) { LibertyModule c = CapturedBy; return (c == null || c == module) && Pressed(button); }

        public bool KeyPressedFor(LibertyModule module, VirtualKey key) { LibertyModule c = CapturedBy; return (c == null || c == module) && KeyPressed(key); }

        // Engine-side access for migrated code that still uses ControllerInput masks.
        internal ControllerInput Pad { get { return pad; } }
    }
}
