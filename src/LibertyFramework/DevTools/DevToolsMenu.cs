using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GTA;
using LibertyFramework.Core.Logging;
using LibertyFramework.WeaponProbe;

namespace LibertyFramework.DevTools
{
    public sealed class DevToolsMenu : Script
    {
        private const ushort DPadUp = 0x0001;
        private const ushort DPadDown = 0x0002;
        private const ushort LeftThumb = 0x0040;
        private const ushort RightThumb = 0x0080;
        private const ushort AButton = 0x1000;
        private const int ChordHoldMilliseconds = 700;
        private static readonly string[] Categories = {
            "RUNTIME", "CONFIG", "WEAPON STATUS",
            "GIVE TEST PISTOL", "GIVE VANILLA PISTOL",
            "GIVE TEST CARBINE", "GIVE VANILLA CARBINE",
            "GIVE TEST SHOTGUN", "GIVE VANILLA SHOTGUN", "HELP"
        };
        private readonly GTA.Font font;
        private bool disabled;
        private bool open;
        private bool controlLocked;
        private bool controllerConnected;
        private bool previousToggle;
        private bool previousUp;
        private bool previousDown;
        private bool previousActivate;
        private bool chordTriggered;
        private bool xinputAvailable = true;
        private DateTime chordStartUtc;
        private int selectedCategory;
        private int pendingConfirmation = -1;
        private string lastActionMessage = "";

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
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            RuntimeLog.Info("devtools_started");
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled)
            {
                if (controlLocked)
                {
                    try { RestorePlayerControl(); }
                    catch (Exception restoreError)
                    {
                        RuntimeLog.Error("devtools_control_restore_failed error=" + restoreError);
                    }
                }
                return;
            }
            try
            {
                if (!open && controlLocked) { RestorePlayerControl(); }
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
                bool activate = Game.isKeyPressed(Keys.Enter) || (controllerButtons & AButton) != 0 ||
                    Game.isGameKeyPressed(GameKey.NavEnter);
                if (open)
                {
                    if (up && !previousUp)
                    {
                        selectedCategory = (selectedCategory + Categories.Length - 1) % Categories.Length;
                        pendingConfirmation = -1;
                        lastActionMessage = "";
                    }
                    if (down && !previousDown)
                    {
                        selectedCategory = (selectedCategory + 1) % Categories.Length;
                        pendingConfirmation = -1;
                        lastActionMessage = "";
                    }
                    if (activate && !previousActivate) { ActivateSelection(); }
                }
                previousUp = up;
                previousDown = down;
                previousActivate = activate;
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
                    if (XInputGetState(index, out state) == 0)
                    {
                        controllerConnected = true;
                        return state.Buttons;
                    }
                }
                controllerConnected = false;
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
            if (open)
            {
                open = false;
                pendingConfirmation = -1;
                RestorePlayerControl();
                RuntimeLog.Info("devtools_menu_closed");
                return;
            }

            if (Player != null && Player.CanControlCharacter)
            {
                controlLocked = true;
                Player.CanControlCharacter = false;
            }
            open = true;
            RuntimeLog.Info("devtools_menu_opened controller_connected=" + controllerConnected +
                " player_control_locked=" + controlLocked);
        }

        private void RestorePlayerControl()
        {
            if (!controlLocked) { return; }
            if (Player == null) { return; }
            Player.CanControlCharacter = true;
            controlLocked = false;
        }

        private void ActivateSelection()
        {
            if (selectedCategory < 2 || selectedCategory == Categories.Length - 1) { return; }
            if (selectedCategory >= 3 && pendingConfirmation != selectedCategory)
            {
                pendingConfirmation = selectedCategory;
                lastActionMessage = "Press Cross/A again to confirm";
                return;
            }

            pendingConfirmation = -1;
            try
            {
                string result;
                switch (selectedCategory)
                {
                    case 2: result = WeaponSlotProbe.ReportStatus(Player); break;
                    case 3: result = WeaponSlotProbe.SelectTestPistol(Player); break;
                    case 4: result = WeaponSlotProbe.SelectVanillaPistol(Player); break;
                    case 5: result = WeaponSlotProbe.SelectTestCarbine(Player); break;
                    case 6: result = WeaponSlotProbe.SelectVanillaCarbine(Player); break;
                    case 7: result = WeaponSlotProbe.SelectTestShotgun(Player); break;
                    case 8: result = WeaponSlotProbe.SelectVanillaShotgun(Player); break;
                    default: return;
                }
                lastActionMessage = result;
                RuntimeLog.Info("devtools_action=" + Categories[selectedCategory]);
            }
            catch (Exception error)
            {
                lastActionMessage = "Weapon action failed; see log";
                RuntimeLog.Error("devtools_weapon_action_failed error=" + error);
            }
        }

        private void OnDraw(object sender, GraphicsEventArgs args)
        {
            if (disabled || !open) { return; }
            try
            {
                args.Graphics.Scaling = FontScaling.Pixel;
                args.Graphics.DrawRectangle(new RectangleF(36, 80, 470, 500), Color.FromArgb(195, 8, 12, 18));
                args.Graphics.DrawText("LIBERTY DEVTOOLS", new RectangleF(52, 94, 430, 28),
                    TextAlignment.Left, font);
                for (int index = 0; index < Categories.Length; index++)
                {
                    float y = 132 + (index * 30);
                    if (index == selectedCategory)
                    {
                        args.Graphics.DrawRectangle(new RectangleF(48, y - 2, 442, 29),
                            Color.FromArgb(120, 190, 145, 35));
                    }
                    args.Graphics.DrawText(Categories[index], new RectangleF(58, y, 418, 25),
                        TextAlignment.Left, font);
                }

                string detail = selectedCategory == 0 ? "Runtime: active" :
                    selectedCategory == 1 ? "Probe: " + RuntimeProbe.ActiveProbeLabel :
                    selectedCategory == Categories.Length - 1 ?
                        "L3+R3 close; D-pad move; Cross/A select" :
                    lastActionMessage.Length > 0 ? lastActionMessage :
                    "Press Cross/A to inspect or select";
                args.Graphics.DrawText(detail, new RectangleF(52, 448, 430, 62),
                    TextAlignment.Left, font);
                string footer = controllerConnected ?
                    "Controller ready; F10 and keyboard also work" :
                    "Controller not detected; F10 and keyboard work";
                args.Graphics.DrawText(footer,
                    new RectangleF(52, 545, 430, 26), TextAlignment.Left, font);
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
            try { RestorePlayerControl(); }
            catch (Exception restoreError) { RuntimeLog.Error("devtools_control_restore_failed error=" + restoreError); }
            RuntimeLog.Error("devtools_disabled error=" + error);
        }

        private void OnDomainUnload(object sender, EventArgs args)
        {
            try { RestorePlayerControl(); }
            catch (Exception error) { RuntimeLog.Error("devtools_control_restore_failed error=" + error); }
        }
    }
}
