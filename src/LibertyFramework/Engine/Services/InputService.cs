using LibertyFramework.Core.Input;
using Keys = System.Windows.Forms.Keys;

namespace LibertyFramework.Engine.Services
{
    // One controller poll per engine frame shared by every module (XInput, read directly so menus work while game
    // controls are locked), with edge detection. Keyboard keys are read through the game's key state.
    public sealed class InputService
    {
        private readonly ControllerInput pad = new ControllerInput();
        private ushort previous;

        internal void Poll()
        {
            previous = pad.Buttons;
            pad.Poll();
        }

        public bool PadConnected { get { return pad.Connected; } }
        // Button masks: ControllerInput.AButton, DPadUp, ...
        public bool Down(ushort button) { return (pad.Buttons & button) != 0; }
        public bool Pressed(ushort button) { return (pad.Buttons & button) != 0 && (previous & button) == 0; }
        public bool Released(ushort button) { return (pad.Buttons & button) == 0 && (previous & button) != 0; }
        public double RightX { get { return pad.RightX; } }
        public double RightY { get { return pad.RightY; } }
        public bool KeyDown(Keys key) { return GTA.Game.isKeyPressed(key); }
    }
}