using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Ui
{
    // Radial menu artwork for any segment count (generalised from the Arsenal wheel's WheelArt): the ring, one
    // highlight per segment and the centre disc are drawn once with GDI+ into PNGs and registered as textures. GTA IV
    // HUD colours: near-black glass, off-white strokes, no saturated accents.
    internal static class RadialArt
    {
        private const int Size = 512;
        private const float Outer = 250f, Inner = 122f, GapDegrees = 2.2f;

        // Ring radius fractions used by the menu layout.
        internal const float IconRadius = 186f / 256f;
        internal const float InnerRadius = Inner / 256f;

        internal static TextureRef Ring(TextureStore store, int segments) { return Get(store, "radial:ring:" + segments, () => DrawRing(segments, -1)); }

        internal static TextureRef Highlight(TextureStore store, int segments, int segment)
        {
            return Get(store, "radial:hl:" + segments + ":" + segment, () => DrawRing(segments, segment));
        }

        internal static TextureRef Centre(TextureStore store) { return Get(store, "radial:centre", DrawCentre); }

        private static TextureRef Get(TextureStore store, string key, System.Func<byte[]> draw)
        {
            TextureRef found = store.Find(key);
            return found.IsNone ? store.Add(draw(), key) : found;
        }

        private static byte[] DrawRing(int segments, int highlight)
        {
            using (Bitmap bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                float step = 360f / segments;
                for (int i = 0; i < segments; i++)
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
    }
}
