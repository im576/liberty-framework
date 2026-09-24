using System;
using System.Runtime.InteropServices;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Core.Input
{
    // Raw XInput state of the first connected pad (Steam Input exposes a virtual XInput pad).
    // Read directly so DevTools works while game controls are locked and D-pad never reaches the phone.
    internal sealed class ControllerInput
    {
        internal const ushort DPadUp = 0x0001;
        internal const ushort DPadDown = 0x0002;
        internal const ushort DPadLeft = 0x0004;
        internal const ushort DPadRight = 0x0008;
        internal const ushort LeftThumb = 0x0040;
        internal const ushort RightThumb = 0x0080;
        internal const ushort AButton = 0x1000;
        internal const ushort BButton = 0x2000;
        internal const ushort XButton = 0x4000;
        internal const ushort YButton = 0x8000;

        // XInputGetState on an empty slot is slow enough to cost frame time when called every frame,
        // so empty slots are rescanned at most once per interval; a connected pad is read every poll.
        private const int RescanIntervalMilliseconds = 1000;
        private bool available = true;
        private int connectedIndex = -1;
        private int lastScanTicks;

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputState
        {
            public uint PacketNumber;
            public ushort Buttons;
            public byte LeftTrigger;
            public byte RightTrigger;
            public short LeftX;
            public short LeftY;
            public short RightX;
            public short RightY;
        }

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        private static extern uint XInputGetState(uint userIndex, out XInputState state);

        internal bool Connected { get; private set; }
        internal ushort Buttons { get; private set; }
        internal ushort Pressed { get; private set; }
        internal double RightX { get; private set; }
        internal double RightY { get; private set; }

        internal void Poll()
        {
            ushort previous = Buttons;
            Buttons = 0;
            RightX = 0;
            RightY = 0;
            Connected = false;
            if (available)
            {
                try
                {
                    XInputState state;
                    if (connectedIndex >= 0 && XInputGetState((uint)connectedIndex, out state) == 0)
                    {
                        Read(state);
                    }
                    else
                    {
                        connectedIndex = -1;
                        int now = Environment.TickCount;
                        if (lastScanTicks == 0 || unchecked(now - lastScanTicks) >= RescanIntervalMilliseconds)
                        {
                            lastScanTicks = now;
                            for (int index = 0; index < 4; index++)
                            {
                                if (XInputGetState((uint)index, out state) != 0) { continue; }
                                connectedIndex = index;
                                Read(state);
                                break;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    // DllNotFound / EntryPointNotFound / BadImageFormat: keyboard still works.
                    available = false;
                    RuntimeLog.Error("controller_unavailable error=" + error.GetType().Name + ": " + error.Message);
                }
            }
            Pressed = (ushort)(Buttons & ~previous);
        }

        private void Read(XInputState state)
        {
            Connected = true;
            Buttons = state.Buttons;
            RightX = Normalize(state.RightX);
            RightY = Normalize(state.RightY);
        }

        internal bool IsDown(ushort mask)
        {
            return (Buttons & mask) == mask;
        }

        internal bool WasPressed(ushort mask)
        {
            return (Pressed & mask) != 0;
        }

        private static double Normalize(short value)
        {
            return Math.Max(-1.0, value / 32767.0);
        }
    }
}
