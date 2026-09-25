using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;

namespace LibertyFramework.Engine.Services
{
    // Shared player-facing UI in GTA IV's style: the top-left help box and short notifications above the radar.
    // Modules post text on their tick; the engine draws it in its draw pass from cached state (no natives while drawing).
    public sealed class UiService
    {
        private sealed class Notice { internal string Text; internal int UntilMs; }

        private readonly GTA.Font font = new GTA.Font(17.0F, FontScaling.Pixel);
        private readonly object gate = new object();
        private string help;
        private Module helpOwner;
        private int helpUntilMs;
        private readonly List<Notice> notices = new List<Notice>();

        public UiService() { font.Color = Color.FromArgb(240, 236, 238, 240); }

        // Shows a help box until durationMs passes or the owner clears it (durationMs <= 0 = until cleared).
        public void ShowHelp(Module owner, string text, int durationMs)
        {
            lock (gate) { help = text; helpOwner = owner; helpUntilMs = durationMs > 0 ? Environment.TickCount + durationMs : int.MaxValue; }
        }

        public void ClearHelp(Module owner)
        {
            lock (gate) { if (helpOwner == owner) { help = null; helpOwner = null; } }
        }

        public void Notify(string text, int durationMs)
        {
            lock (gate)
            {
                Notice notice = new Notice();
                notice.Text = text; notice.UntilMs = Environment.TickCount + durationMs;
                notices.Add(notice);
                if (notices.Count > 4) { notices.RemoveAt(0); }
            }
        }

        internal void Draw(GraphicsEventArgs args)
        {
            Size screen = ScreenInfo.Size;
            if (screen.Height <= 0) { return; }
            float scale = screen.Height / 720f;
            int now = Environment.TickCount;
            GTA.Graphics graphics = args.Graphics;
            graphics.Scaling = FontScaling.Pixel;
            lock (gate)
            {
                if (help != null && (helpUntilMs == int.MaxValue || unchecked(now - helpUntilMs) < 0) && (helpOwner == null || helpOwner.Running))
                {
                    RectangleF box = new RectangleF(34 * scale, 30 * scale, 330 * scale, 40 * scale);
                    graphics.DrawRectangle(box, Color.FromArgb(200, 0, 0, 0));
                    graphics.DrawText(help, new RectangleF(box.X + 12 * scale, box.Y + 10 * scale, box.Width - 24 * scale, box.Height - 14 * scale), TextAlignment.Left, font);
                }
                notices.RemoveAll(n => unchecked(now - n.UntilMs) >= 0);
                float y = screen.Height - 220 * scale;
                for (int i = notices.Count - 1; i >= 0; i--)
                {
                    RectangleF box = new RectangleF(34 * scale, y, 330 * scale, 30 * scale);
                    graphics.DrawRectangle(box, Color.FromArgb(180, 0, 0, 0));
                    graphics.DrawText(notices[i].Text, new RectangleF(box.X + 10 * scale, box.Y + 6 * scale, box.Width - 20 * scale, box.Height - 8 * scale), TextAlignment.Left, font);
                    y -= 34 * scale;
                }
            }
        }
    }
}