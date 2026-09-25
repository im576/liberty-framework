using System;
using System.Collections.Generic;
using System.Drawing;
using Keys = System.Windows.Forms.Keys;
using GTA;
using LibertyFramework.Arsenal.Contracts;
using LibertyFramework.Core.Input;
using LibertyFramework.DevTools.Menu;

namespace LibertyFramework.Arsenal.Ui
{
    // S-2: the radial weapon wheel used for every weapon container (trunk, safehouse stash, gunsmith).
    // Eight segments = the inventory categories; each shows the weapon the player carries in that category.
    // The centre shows the selected category: what is carried, what the container holds (cycle with LB/RB), and the
    // available actions. Right stick / D-pad left-right choose a segment; A takes (swapping out what is carried),
    // X stores, Y opens the gunsmith (safehouse), B closes. Keyboard: arrows, Enter, Space, G, Backspace.
    internal sealed class WeaponWheel
    {
        internal interface IHost
        {
            string Title { get; }
            IList<WeaponRecord> Carried { get; }
            IList<WeaponRecord> Stored { get; }
            bool GunsmithAvailable { get; }
            string Name(int weaponId);
            string Store(WeaponRecord carried);
            string Take(WeaponRecord stored);
            List<MenuItem> GunsmithItems();
        }

        internal enum Result { None, Close }

        // Clockwise from the top.
        private static readonly WeaponCategory[] Order =
        {
            WeaponCategory.Handgun, WeaponCategory.SMG, WeaponCategory.Rifle, WeaponCategory.Sniper,
            WeaponCategory.Heavy, WeaponCategory.Thrown, WeaponCategory.Melee, WeaponCategory.Shotgun
        };
        private static readonly string[] CategoryNames = { "", "MELEE", "PISTOLS", "SHOTGUNS", "SUBMACHINE GUNS", "RIFLES", "SNIPER RIFLES", "HEAVY", "THROWN", "OTHER" };

        private readonly WheelArt art = new WheelArt();
        private readonly GTA.Font titleFont, bigFont, font, smallFont;
        private IHost host;
        private int segment, storedIndex, gunsmithIndex;
        private bool gunsmith;
        private MenuItem pendingConfirmation;
        private string message = "";
        private int messageUntil;
        private int openedAt;
        private bool prevLeft, prevRight, prevUp, prevDown, prevA, prevX, prevY, prevB;

        internal WeaponWheel()
        {
            titleFont = new GTA.Font(22f, FontScaling.Pixel, true, false); titleFont.Color = Color.FromArgb(235, 236, 238, 240);
            bigFont = new GTA.Font(20f, FontScaling.Pixel, true, false); bigFont.Color = Color.FromArgb(245, 245, 246, 248);
            font = new GTA.Font(16f, FontScaling.Pixel, false, false); font.Color = Color.FromArgb(225, 222, 224, 228);
            smallFont = new GTA.Font(14f, FontScaling.Pixel, false, false); smallFont.Color = Color.FromArgb(170, 205, 208, 214);
        }

        internal bool IsOpen { get { return host != null; } }

        internal void Open(IHost owner)
        {
            host = owner;
            gunsmith = false; storedIndex = 0; gunsmithIndex = 0; pendingConfirmation = null; message = "";
            openedAt = Environment.TickCount;
            // Start on the first category that holds something (carried or stored), else the top.
            segment = 0;
            for (int i = 0; i < Order.Length; i++) { if (Carried(Order[i]) != null || Stored(Order[i]).Count > 0) { segment = i; break; } }
            prevA = prevB = prevX = prevY = true; // the key that opened the wheel must be released first
        }

        internal void Close() { host = null; }

        private WeaponRecord Carried(WeaponCategory category)
        {
            foreach (WeaponRecord record in host.Carried) { if (record.Category == category) { return record; } }
            return null;
        }

        private List<WeaponRecord> Stored(WeaponCategory category)
        {
            List<WeaponRecord> result = new List<WeaponRecord>();
            foreach (WeaponRecord record in host.Stored) { if (record.Category == category) { result.Add(record); } }
            return result;
        }

        internal Result Update(ControllerInput pad)
        {
            if (host == null) { return Result.None; }
            bool left = Game.isKeyPressed(Keys.Left) || pad.IsDown(ControllerInput.DPadLeft);
            bool right = Game.isKeyPressed(Keys.Right) || pad.IsDown(ControllerInput.DPadRight);
            bool up = Game.isKeyPressed(Keys.Up) || pad.IsDown(ControllerInput.DPadUp) || pad.IsDown(ControllerInput.RightShoulder);
            bool down = Game.isKeyPressed(Keys.Down) || pad.IsDown(ControllerInput.DPadDown) || pad.IsDown(ControllerInput.LeftShoulder);
            bool a = Game.isKeyPressed(Keys.Enter) || pad.IsDown(ControllerInput.AButton);
            bool x = Game.isKeyPressed(Keys.Space) || pad.IsDown(ControllerInput.XButton);
            bool y = Game.isKeyPressed(Keys.G) || pad.IsDown(ControllerInput.YButton);
            bool b = Game.isKeyPressed(Keys.Back) || Game.isKeyPressed(Keys.Escape) || pad.IsDown(ControllerInput.BButton);
            Result result = Result.None;

            if (!gunsmith)
            {
                // Right stick picks a segment directly (like the game's own wheel), D-pad steps.
                double magnitude = Math.Sqrt(pad.RightX * pad.RightX + pad.RightY * pad.RightY);
                if (magnitude > 0.55)
                {
                    double angle = Math.Atan2(pad.RightX, pad.RightY) * 180.0 / Math.PI; // 0 = up, clockwise
                    int picked = (int)Math.Floor(((angle + 360.0 + 22.5) % 360.0) / 45.0);
                    if (picked != segment) { segment = picked; storedIndex = 0; pendingConfirmation = null; }
                }
                if (left && !prevLeft) { segment = (segment + Order.Length - 1) % Order.Length; storedIndex = 0; }
                if (right && !prevRight) { segment = (segment + 1) % Order.Length; storedIndex = 0; }
                List<WeaponRecord> stored = Stored(Order[segment]);
                if (stored.Count > 0)
                {
                    if (up && !prevUp) { storedIndex = (storedIndex + 1) % stored.Count; }
                    if (down && !prevDown) { storedIndex = (storedIndex + stored.Count - 1) % stored.Count; }
                    storedIndex = Math.Min(storedIndex, stored.Count - 1);
                }
                if (a && !prevA && stored.Count > 0) { Say(host.Take(stored[storedIndex])); storedIndex = 0; }
                if (x && !prevX)
                {
                    WeaponRecord carried = Carried(Order[segment]);
                    Say(carried != null ? host.Store(carried) : "Nothing carried in this slot");
                }
                if (y && !prevY && host.GunsmithAvailable) { gunsmith = true; gunsmithIndex = 0; pendingConfirmation = null; }
                if (b && !prevB) { result = Result.Close; }
            }
            else
            {
                List<MenuItem> items = Actionable(host.GunsmithItems());
                if (items.Count > 0)
                {
                    if (up && !prevUp) { gunsmithIndex = (gunsmithIndex + items.Count - 1) % items.Count; pendingConfirmation = null; }
                    if (down && !prevDown) { gunsmithIndex = (gunsmithIndex + 1) % items.Count; pendingConfirmation = null; }
                    gunsmithIndex = Math.Min(gunsmithIndex, items.Count - 1);
                    if (a && !prevA)
                    {
                        MenuItem item = items[gunsmithIndex];
                        if (item.RequiresConfirmation && pendingConfirmation != item) { pendingConfirmation = item; Say("Press A again to pay"); }
                        else { pendingConfirmation = null; Say(item.Activate()); }
                    }
                }
                if ((b && !prevB) || (y && !prevY)) { gunsmith = false; pendingConfirmation = null; }
            }
            prevLeft = left; prevRight = right; prevUp = up; prevDown = down; prevA = a; prevX = x; prevY = y; prevB = b;
            return result;
        }

        private static List<MenuItem> Actionable(List<MenuItem> items)
        {
            List<MenuItem> result = new List<MenuItem>();
            foreach (MenuItem item in items) { if (item.Activate != null) { result.Add(item); } }
            return result;
        }

        private void Say(string text) { message = text ?? ""; messageUntil = Environment.TickCount + 2600; }

        // screen comes from Engine.Services.ScreenInfo (Game.Resolution stalls or deadlocks the game).
        internal void Draw(GTA.Graphics graphics, Size screen)
        {
            if (host == null || screen.Height <= 0) { return; }
            graphics.Scaling = FontScaling.Pixel;
            float fade = Math.Min(1f, (Environment.TickCount - openedAt) / 160f);
            float diameter = screen.Height * 0.66f;
            float cx = screen.Width / 2f, cy = screen.Height / 2f;
            RectangleF wheel = new RectangleF(cx - diameter / 2, cy - diameter / 2, diameter, diameter);
            Color tint = Color.FromArgb((int)(255 * fade), 255, 255, 255);
            graphics.DrawSprite(art.Ring, wheel, tint);
            if (!gunsmith) { graphics.DrawSprite(art.Highlight(segment), wheel, tint); }
            graphics.DrawSprite(art.Centre, wheel, tint);

            float iconRadius = diameter / 2 * (186f / 256f);
            for (int i = 0; i < Order.Length; i++)
            {
                double angle = WheelArt.SegmentAngleDegrees(i) * Math.PI / 180.0;
                float ix = cx + (float)Math.Sin(angle) * iconRadius, iy = cy - (float)Math.Cos(angle) * iconRadius;
                WeaponRecord carried = Carried(Order[i]);
                int stored = Stored(Order[i]).Count;
                if (carried != null)
                {
                    GTA.Texture icon = art.Icon(carried.WeaponId);
                    float w = diameter * 0.19f, h = w / 2;
                    if (icon != null) { graphics.DrawSprite(icon, new RectangleF(ix - w / 2, iy - h / 2, w, h), Color.FromArgb((int)(255 * fade), 255, 255, 255)); }
                    else { graphics.DrawText(host.Name(carried.WeaponId), new RectangleF(ix - 70, iy - 10, 140, 22), TextAlignment.Center, smallFont); }
                }
                else
                {
                    graphics.DrawText("-", new RectangleF(ix - 20, iy - 12, 40, 24), TextAlignment.Center, smallFont);
                }
                if (stored > 0)
                {
                    graphics.DrawText("+" + stored, new RectangleF(ix - 30, iy + diameter * 0.055f, 60, 18), TextAlignment.Center, smallFont);
                }
            }

            float boxWidth = diameter * 0.44f;
            RectangleF text = new RectangleF(cx - boxWidth / 2, cy - diameter * 0.2f, boxWidth, 24);
            graphics.DrawText(host.Title, text, TextAlignment.Center, titleFont);
            text.Y += 30;
            if (gunsmith) { DrawGunsmith(graphics, text); }
            else { DrawCategory(graphics, text); }
            if (message.Length > 0 && Environment.TickCount < messageUntil)
            {
                graphics.DrawText(message, new RectangleF(cx - boxWidth / 2, cy + diameter * 0.155f, boxWidth, 20), TextAlignment.Center, smallFont);
            }
        }

        private void DrawCategory(GTA.Graphics graphics, RectangleF line)
        {
            WeaponCategory category = Order[segment];
            graphics.DrawText(CategoryNames[(int)category], line, TextAlignment.Center, smallFont);
            line.Y += 24;
            WeaponRecord carried = Carried(category);
            graphics.DrawText(carried != null ? host.Name(carried.WeaponId) : "Empty", line, TextAlignment.Center, bigFont);
            line.Y += 24;
            if (carried != null) { graphics.DrawText(carried.Ammo + " rounds" + (carried.Owned ? "  -  owned" : ""), line, TextAlignment.Center, font); }
            line.Y += 32;
            List<WeaponRecord> stored = Stored(category);
            if (stored.Count > 0)
            {
                WeaponRecord pick = stored[Math.Min(storedIndex, stored.Count - 1)];
                string cycle = stored.Count > 1 ? "  (" + (Math.Min(storedIndex, stored.Count - 1) + 1) + "/" + stored.Count + ")" : "";
                graphics.DrawText("In here: " + host.Name(pick.WeaponId) + "  " + pick.Ammo + cycle, line, TextAlignment.Center, font);
            }
            else { graphics.DrawText("Nothing stored in this slot", line, TextAlignment.Center, smallFont); }
            line.Y += 30;
            string hints = (stored.Count > 0 ? (carried != null ? "A Swap   " : "A Take   ") : "") + (carried != null ? "X Store   " : "") +
                (host.GunsmithAvailable ? "Y Gunsmith   " : "") + "B Close";
            graphics.DrawText(hints, line, TextAlignment.Center, smallFont);
        }

        private void DrawGunsmith(GTA.Graphics graphics, RectangleF line)
        {
            graphics.DrawText("GUNSMITH", line, TextAlignment.Center, smallFont);
            line.Y += 26;
            List<MenuItem> items = Actionable(host.GunsmithItems());
            if (items.Count == 0) { graphics.DrawText("Nothing to work on", line, TextAlignment.Center, font); }
            int first = Math.Max(0, Math.Min(gunsmithIndex - 2, items.Count - 5));
            for (int i = first; i < items.Count && i < first + 5; i++)
            {
                graphics.DrawText((i == gunsmithIndex ? "> " : "") + items[i].Label() + (i == gunsmithIndex ? " <" : ""), line, TextAlignment.Center,
                    i == gunsmithIndex ? bigFont : font);
                line.Y += 24;
            }
            line.Y += 8;
            graphics.DrawText("A Choose   Y/B Back", line, TextAlignment.Center, smallFont);
        }
    }
}
