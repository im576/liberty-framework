using System;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Ui
{
    // GTA IV-style radial menu (generalised from the Arsenal weapon wheel). Segment 0 is at the top, clockwise. The
    // right stick picks a segment directly, D-pad left/right steps, LB/RB or D-pad up/down cycle within a segment,
    // A/X/Y call the handlers, B closes. Values are evaluated on the engine tick into a snapshot for the draw pass.
    internal sealed class RadialMenuView : IMenu
    {
        private sealed class Snapshot
        {
            internal string Title;
            internal string[] Labels;
            internal TextureRef[] Icons;
            internal string[] Badges;
            internal string[] Centre;
            internal int Selected;
            internal string Message;
            internal int OpenedAt;
            internal TextureRef Ring, Highlight, Disc;
        }

        private readonly RadialMenu menu;
        private readonly TextureStore textures;
        private readonly Action<RadialMenuView> onClosed;
        private readonly int openedAt;
        private string message;
        private int messageUntil;
        private volatile Snapshot snapshot;

        internal RadialMenuView(LibertyModule owner, RadialMenu menu, TextureStore textures, Action<RadialMenuView> onClosed)
        {
            Owner = owner; this.menu = menu; this.textures = textures; this.onClosed = onClosed;
            IsOpen = true;
            openedAt = Environment.TickCount;
        }

        internal LibertyModule Owner { get; private set; }
        public bool IsOpen { get; private set; }
        public int Selected { get; set; }

        public void Message(string text) { message = text; messageUntil = Environment.TickCount + 2600; }

        public void Close()
        {
            if (!IsOpen) { return; }
            IsOpen = false;
            onClosed(this);
            if (menu.OnClosed != null) { menu.OnClosed(); }
        }

        internal void Update(MenuInput input)
        {
            int count = menu.Segments != null ? menu.Segments.Count : 0;
            if (count == 0) { Close(); return; }
            Selected = Math.Max(0, Math.Min(Selected, count - 1));
            double magnitude = Math.Sqrt(input.StickX * input.StickX + input.StickY * input.StickY);
            if (magnitude > 0.55)
            {
                double angle = Math.Atan2(input.StickX, input.StickY) * 180.0 / Math.PI; // 0 = up, clockwise
                double step = 360.0 / count;
                Selected = (int)Math.Floor(((angle + 360.0 + step / 2) % 360.0) / step) % count;
            }
            if (input.Left) { Selected = (Selected + count - 1) % count; }
            if (input.Right) { Selected = (Selected + 1) % count; }
            if (menu.OnCycle != null)
            {
                if (input.NextTab || input.Up) { menu.OnCycle(Selected, 1); }
                if (input.PreviousTab || input.Down) { menu.OnCycle(Selected, -1); }
            }
            if (input.Accept && menu.OnAccept != null) { Say(menu.OnAccept(Selected)); }
            if (input.X && menu.OnX != null) { Say(menu.OnX(Selected)); }
            if (input.Y && menu.OnY != null) { Say(menu.OnY(Selected)); }
            if (input.Back) { Close(); return; }
            if (!IsOpen) { return; }

            Snapshot s = new Snapshot();
            s.Title = menu.Title;
            s.Labels = new string[count];
            s.Icons = new TextureRef[count];
            s.Badges = new string[count];
            for (int i = 0; i < count; i++)
            {
                RadialSegment segment = menu.Segments[i];
                try
                {
                    s.Labels[i] = segment.Label != null ? segment.Label() : null;
                    s.Icons[i] = segment.Icon != null ? segment.Icon() : TextureRef.None;
                    s.Badges[i] = segment.Badge != null ? segment.Badge() : null;
                }
                catch (Exception error) { RuntimeLog.Error("[" + Owner.Id + "] ui_segment_failed index=" + i + " error=" + error.Message); }
            }
            s.Centre = menu.CenterLines != null ? menu.CenterLines(Selected) : null;
            s.Selected = Selected;
            s.Message = message != null && unchecked(Environment.TickCount - messageUntil) < 0 ? message : null;
            s.OpenedAt = openedAt;
            s.Ring = RadialArt.Ring(textures, count);
            s.Highlight = RadialArt.Highlight(textures, count, Selected);
            s.Disc = RadialArt.Centre(textures);
            snapshot = s;
        }

        private void Say(string text) { if (!string.IsNullOrEmpty(text)) { Message(text); } }

        internal void Draw(ICanvas canvas)
        {
            Snapshot s = snapshot;
            if (s == null) { return; }
            float fade = Math.Min(1f, (Environment.TickCount - s.OpenedAt) / 160f);
            float saved = canvas.Opacity;
            canvas.Opacity = saved * fade;
            float diameter = canvas.Height * menu.Size;
            float cx = canvas.Width / 2f, cy = canvas.Height / 2f;
            float left = cx - diameter / 2, top = cy - diameter / 2;
            Rgba tint = new Rgba(255, 255, 255, 255);
            canvas.Sprite(s.Ring, left, top, diameter, diameter, tint);
            canvas.Sprite(s.Highlight, left, top, diameter, diameter, tint);
            canvas.Sprite(s.Disc, left, top, diameter, diameter, tint);

            int count = s.Labels.Length;
            float iconRadius = diameter / 2 * RadialArt.IconRadius;
            for (int i = 0; i < count; i++)
            {
                double angle = i * 2 * Math.PI / count;
                float ix = cx + (float)Math.Sin(angle) * iconRadius, iy = cy - (float)Math.Cos(angle) * iconRadius;
                if (!s.Icons[i].IsNone)
                {
                    float w = diameter * 0.19f, h = w / 2;
                    canvas.Sprite(s.Icons[i], ix - w / 2, iy - h / 2, w, h, tint);
                }
                else
                {
                    canvas.Text(s.Labels[i] ?? "-", ix - 70, iy - 10, 140, 22, TextStyle.Small, TextAlign.Center, Rgba.Muted);
                }
                if (!string.IsNullOrEmpty(s.Badges[i]))
                {
                    canvas.Text(s.Badges[i], ix - 30, iy + diameter * 0.055f, 60, 18, TextStyle.Small, TextAlign.Center, Rgba.Muted);
                }
            }

            float boxWidth = diameter * 0.44f;
            float y = cy - diameter * 0.2f;
            canvas.Text(s.Title ?? "", cx - boxWidth / 2, y, boxWidth, 26, TextStyle.Title, TextAlign.Center, Rgba.White);
            y += 32;
            if (s.Centre != null)
            {
                for (int i = 0; i < s.Centre.Length; i++)
                {
                    TextStyle style = i == 1 ? TextStyle.Emphasis : i == s.Centre.Length - 1 ? TextStyle.Small : TextStyle.Body;
                    canvas.Text(s.Centre[i], cx - boxWidth / 2, y, boxWidth, 22, style, TextAlign.Center, i == s.Centre.Length - 1 ? Rgba.Muted : Rgba.White);
                    y += i == s.Centre.Length - 2 ? 30 : 24;
                }
            }
            if (s.Message != null)
            {
                canvas.Text(s.Message, cx - boxWidth / 2, cy + diameter * 0.155f, boxWidth, 20, TextStyle.Small, TextAlign.Center, Rgba.Muted);
            }
            canvas.Opacity = saved;
        }
    }
}
