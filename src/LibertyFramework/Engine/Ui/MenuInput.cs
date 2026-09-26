using Liberty.Sdk;
using LibertyFramework.Engine.Services;

namespace LibertyFramework.Engine.Ui
{
    // One frame of menu input: controller buttons and their keyboard equivalents (the Arsenal wheel's mapping:
    // arrows, Enter = A, Space = X, G = Y, Backspace/Escape = B, Page Up/Down = LB/RB), as presses (edges).
    internal struct MenuInput
    {
        internal bool Up, Down, Left, Right, Accept, Back, X, Y, PreviousTab, NextTab;
        internal float StickX, StickY;

        internal static MenuInput Read(InputService input)
        {
            MenuInput m = new MenuInput();
            m.Up = input.Pressed(PadButton.DPadUp) || input.KeyPressed(VirtualKey.Up);
            m.Down = input.Pressed(PadButton.DPadDown) || input.KeyPressed(VirtualKey.Down);
            m.Left = input.Pressed(PadButton.DPadLeft) || input.KeyPressed(VirtualKey.Left);
            m.Right = input.Pressed(PadButton.DPadRight) || input.KeyPressed(VirtualKey.Right);
            m.Accept = input.Pressed(PadButton.A) || input.KeyPressed(VirtualKey.Enter);
            m.Back = input.Pressed(PadButton.B) || input.KeyPressed(VirtualKey.Back) || input.KeyPressed(VirtualKey.Escape);
            m.X = input.Pressed(PadButton.X) || input.KeyPressed(VirtualKey.Space);
            m.Y = input.Pressed(PadButton.Y) || input.KeyPressed(VirtualKey.G);
            m.PreviousTab = input.Pressed(PadButton.LeftShoulder) || input.KeyPressed((VirtualKey)0x21);
            m.NextTab = input.Pressed(PadButton.RightShoulder) || input.KeyPressed((VirtualKey)0x22);
            m.StickX = input.RightX;
            m.StickY = input.RightY;
            return m;
        }
    }
}
