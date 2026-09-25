using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Arsenal.Ui
{
    // S-2 weapon wheel artwork: the ring, its per-segment highlights and the centre disc are drawn once with GDI+ into
    // PNGs and handed to ScriptHookDotNet as textures; weapon icons are the game's own weapon HUD icons, extracted at
    // install time to scripts\LibertyFramework\ui\icons\<weaponId>.png. Colours follow GTA IV's HUD: near-black glass,
    // off-white strokes, no saturated accents.
    internal sealed class WheelArt
    {
        internal const int Segments = 8;
        private const int Size = 512;
        private const float Outer = 250f, Inner = 122f, GapDegrees = 2.2f;

        private GTA.Texture ring, centre;
        private readonly GTA.Texture[] highlights = new GTA.Texture[Segments];
        private readonly Dictionary<int, GTA.Texture> icons = new Dictionary<int, GTA.Texture>();
        private readonly HashSet<int> missingIcons = new HashSet<int>();

        internal GTA.Texture Ring { get { if (ring == null) { ring = Texture(DrawRing(-1)); } return ring; } }
        internal GTA.Texture Centre { get { if (centre == null) { centre = Texture(DrawCentre()); } return centre; } }

        internal GTA.Texture Highlight(int segment)
        {
            if (highlights[segment] == null) { highlights[segment] = Texture(DrawRing(segment)); }
            return highlights[segment];
        }

        internal GTA.Texture Icon(int weaponId)
        {
            GTA.Texture icon;
            if (icons.TryGetValue(weaponId, out icon)) { return icon; }
            if (missingIcons.Contains(weaponId)) { return null; }
            string path = Path.Combine(LibertyPaths.Root, Path.Combine("ui", Path.Combine("icons", weaponId + ".png")));
            if (!File.Exists(path)) { missingIcons.Add(weaponId); RuntimeLog.Error("wheel_icon_missing id=" + weaponId); return null; }
            icon = new GTA.Texture(File.ReadAllBytes(path));
            icons[weaponId] = icon;
            return icon;
        }

        // Segment 0 is centred at the top; segments run clockwise. highlight < 0 draws the whole ring.
        internal static float SegmentAngleDegrees(int segment) { return segment * 360f / Segments; }

        private static byte[] DrawRing(int highlight)
        {
            using (Bitmap bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float step = 360f / Segments;
                for (int i = 0; i < Segments; i++)
                {
                    if (highlight >= 0 && i != highlight) { continue; }
                    // GDI+ angles start at 3 o'clock; segment 0 is centred at 12 o'clock.
                    float start = -90f + i * step - step / 2 + GapDegrees / 2;
                    float sweep = step - GapDegrees;
                    using (GraphicsPath path = Wedge(start, sweep))
                    {
                        if (highlight < 0)
                        {
                            using (Brush fill = new SolidBrush(Color.FromArgb(178, 11, 13, 17))) { g.FillPath(fill, path); }
                            using (Pen rim = new Pen(Color.FromArgb(70, 225, 228, 232), 1.5f)) { g.DrawPath(rim, path); }
                        }
                        else
                        {
                            using (Brush fill = new SolidBrush(Color.FromArgb(62, 236, 238, 242))) { g.FillPath(fill, path); }
                            using (Pen edge = new Pen(Color.FromArgb(235, 240, 242, 246), 5f))
                            {
                                float c = Size / 2f;
                                g.DrawArc(edge, c - Outer + 3, c - Outer + 3, (Outer - 3) * 2, (Outer - 3) * 2, start, sweep);
                            }
                        }
                    }
                }
                return Png(bitmap);
            }
        }

        private static GraphicsPath Wedge(float start, float sweep)
        {
            float c = Size / 2f;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(c - Outer, c - Outer, Outer * 2, Outer * 2, start, sweep);
            path.AddArc(c - Inner, c - Inner, Inner * 2, Inner * 2, start + sweep, -sweep);
            path.CloseFigure();
            return path;
        }

        private static byte[] DrawCentre()
        {
            using (Bitmap bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float c = Size / 2f, r = Inner - 6;
                using (Brush fill = new SolidBrush(Color.FromArgb(222, 7, 8, 11))) { g.FillEllipse(fill, c - r, c - r, r * 2, r * 2); }
                using (Pen rim = new Pen(Color.FromArgb(55, 225, 228, 232), 1.5f)) { g.DrawEllipse(rim, c - r, c - r, r * 2, r * 2); }
                return Png(bitmap);
            }
        }

        private static byte[] Png(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream()) { bitmap.Save(stream, ImageFormat.Png); return stream.ToArray(); }
        }

        private static GTA.Texture Texture(byte[] png) { return new GTA.Texture(png); }
    }
}
