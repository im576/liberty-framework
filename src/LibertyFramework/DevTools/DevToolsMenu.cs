using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.DevTools
{
    public sealed class DevToolsMenu : Script
    {
        private const ushort DPadUp = 0x0001;
        private const ushort DPadDown = 0x0002;
        private const ushort LeftThumb = 0x0040;
        private const ushort RightThumb = 0x0080;
        private const int ChordHoldMilliseconds = 700;
        private static readonly string[] Categories = { "RUNTIME", "CONFIG", "HELP" };
        private readonly GTA.Font font;
        private bool disabled;
        private bool open;
        private bool previousToggle;
        private bool previousUp;
        private bool previousDown;
        private bool chordTriggered;
        private bool xinputAvailable = true;
        private DateTime chordStartUtc;
        private int selectedCategory;

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

        public DevToolsMenu()
        {
            Interval = 50;
            font = new GTA.Font(18.0F, FontScaling.Pixel);
            font.Color = Color.White;
            Tick += OnTick;
            PerFrameDrawing += OnDraw;
            RuntimeLog.Info("devtools_started");
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled) { return; }
            try
            {
                ushort controllerButtons = ReadControllerButtons();
                bool toggle = Game.isKeyPressed(Keys.F10);
                if (toggle && !previousToggle)
                {
                    ToggleMenu();
                }
                previousToggle = toggle;

                bool thumbChord = (controllerButtons & (LeftThumb | RightThumb)) ==
                    (LeftThumb | RightThumb) ||
                    (Game.isGameKeyPressed(GameKey.Crouch) &&
                     Game.isGameKeyPressed(GameKey.LookBehind));
                if (thumbChord)
                {
                    if (chordStartUtc == DateTime.MinValue) { chordStartUtc = DateTime.UtcNow; }
                    if (!chordTriggered &&
                        (DateTime.UtcNow - chordStartUtc).TotalMilliseconds >= ChordHoldMilliseconds)
                    {
                        chordTriggered = true;
                        ToggleMenu();
                    }
                }
                else
                {
                    chordStartUtc = DateTime.MinValue;
                    chordTriggered = false;
                }

                bool up = Game.isKeyPressed(Keys.Up) || (controllerButtons & DPadUp) != 0 ||
                    Game.isGameKeyPressed(GameKey.NavUp);
                bool down = Game.isKeyPressed(Keys.Down) || (controllerButtons & DPadDown) != 0 ||
                    Game.isGameKeyPressed(GameKey.NavDown);
                if (open)
                {
                    if (up && !previousUp)
                    {
                        selectedCategory = (selectedCategory + Categories.Length - 1) % Categories.Length;
                    }
                    if (down && !previousDown)
                    {
                        selectedCategory = (selectedCategory + 1) % Categories.Length;
                    }
                }
                previousUp = up;
                previousDown = down;
            }
            catch (Exception error)
            {
                DisableAfterError(error);
            }
        }

        private ushort ReadControllerButtons()
        {
            if (!xinputAvailable) { return 0; }
            try
            {
                for (uint index = 0; index < 4; index++)
                {
                    XInputState state;
                    if (XInputGetState(index, out state) == 0) { return state.Buttons; }
                }
                return 0;
            }
            catch (DllNotFoundException error)
            {
                xinputAvailable = false;
                RuntimeLog.Error("devtools_controller_unavailable error=" + error);
                return 0;
            }
            catch (EntryPointNotFoundException error)
            {
                xinputAvailable = false;
                RuntimeLog.Error("devtools_controller_unavailable error=" + error);
                return 0;
            }
            catch (BadImageFormatException error)
            {
                xinputAvailable = false;
                RuntimeLog.Error("devtools_controller_unavailable error=" + error);
                return 0;
            }
        }

        private void ToggleMenu()
        {
            open = !open;
            RuntimeLog.Info("devtools_menu_" + (open ? "opened" : "closed"));
        }

        private void OnDraw(object sender, GraphicsEventArgs args)
        {
            if (disabled || !open) { return; }
            try
            {
                args.Graphics.Scaling = FontScaling.Pixel;
                args.Graphics.DrawRectangle(new RectangleF(36, 80, 340, 224), Color.FromArgb(195, 8, 12, 18));
                args.Graphics.DrawText("LIBERTY DEVTOOLS", new RectangleF(52, 94, 300, 28),
                    TextAlignment.Left, font);
                for (int index = 0; index < Categories.Length; index++)
                {
                    float y = 132 + (index * 32);
                    if (index == selectedCategory)
                    {
                        args.Graphics.DrawRectangle(new RectangleF(48, y - 2, 312, 29),
                            Color.FromArgb(120, 190, 145, 35));
                    }
                    args.Graphics.DrawText(Categories[index], new RectangleF(58, y, 292, 25),
                        TextAlignment.Left, font);
                }

                string detail = selectedCategory == 0 ? "Runtime: active" :
                    selectedCategory == 1 ? "Probe: " + RuntimeProbe.ActiveProbeLabel :
                    "L3+R3: toggle  D-pad: select";
                args.Graphics.DrawText(detail, new RectangleF(52, 248, 308, 30),
                    TextAlignment.Left, font);
            }
            catch (Exception error)
            {
                DisableAfterError(error);
            }
        }

        private void DisableAfterError(Exception error)
        {
            open = false;
            disabled = true;
            RuntimeLog.Error("devtools_disabled error=" + error);
        }
    }
}
