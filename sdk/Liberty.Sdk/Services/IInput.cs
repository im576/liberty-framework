namespace Liberty.Sdk
{
    // Controller (XInput) and keyboard, polled once per frame. Capture gives one module exclusive input (menus):
    // other modules see nothing pressed and the game's controls are locked until Release.
    public interface IInput
    {
        bool PadConnected { get; }
        bool Down(PadButton button);
        bool Pressed(PadButton button);
        bool Released(PadButton button);
        // Sticks -1..1 (Y up positive); triggers 0..1.
        float LeftX { get; }
        float LeftY { get; }
        float RightX { get; }
        float RightY { get; }
        float LeftTrigger { get; }
        float RightTrigger { get; }
        bool KeyDown(VirtualKey key);
        bool KeyPressed(VirtualKey key);
        // Needs Capabilities.InputCapture.
        bool Capture(LibertyModule owner);
        void Release(LibertyModule owner);
        LibertyModule CapturedBy { get; }
        // Input as seen by a module (false for everyone except the capturing module while captured).
        bool PressedFor(LibertyModule module, PadButton button);
        bool KeyPressedFor(LibertyModule module, VirtualKey key);
    }
}