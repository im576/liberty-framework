using System;
using System.Collections.Generic;
using System.Drawing;
using GTA;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Ui
{
    // SDK ICanvas over ScriptHookDotNet's draw pass. Virtual coordinates: the screen is 720 units high and
    // 720 x aspect wide (1280 on 16:9), scaled to the real back buffer (ScreenInfo, never Game.Resolution).
    // Draw pass only: no game functions are called here.
    internal sealed class Canvas : ICanvas
    {
        private static readonly float[] StyleSizes = { 14f, 16f, 18f, 22f, 30f };
        private static readonly bool[] StyleBold = { false, false, true, true, true };

        private readonly TextureStore textures;
        private readonly Dictionary<int, GTA.Font> fonts = new Dictionary<int, GTA.Font>();
        private GTA.Graphics graphics;
        private float scale = 1f;
        private float fontScale = -1f;

        internal Canvas(TextureStore textures) { this.textures = textures; Opacity = 1f; }

        public float Width { get; private set; }
        public float Height { get { return 720f; } }
        public float Opacity { get; set; }

        // Returns false when the screen size is not known yet.
        internal bool Begin(GTA.Graphics target, Size screen)
        {
            if (screen.Height <= 0 || screen.Width <= 0) { return false; }
            graphics = target;
            graphics.Scaling = FontScaling.Pixel;
            scale = screen.Height / 720f;
            Width = screen.Width / scale;
            Opacity = 1f;
            if (Math.Abs(fontScale - scale) > 0.001f) { DisposeFonts(); fontScale = scale; }
            return true;
        }

        private Color C(Rgba c) { return Color.FromArgb((int)(c.A * Clamp01(Opacity)), c.R, c.G, c.B); }

        private RectangleF R(float x, float y, float w, float h) { return new RectangleF(x * scale, y * scale, w * scale, h * scale); }

        public void Rect(float x, float y, float width, float height, Rgba colour)
        {
            graphics.DrawRectangle(R(x, y, width, height), C(colour));
        }

        public void Text(string text, float x, float y, float width, float height, TextStyle style, TextAlign align, Rgba colour)
        {
            if (string.IsNullOrEmpty(text)) { return; }
            TextAlignment alignment = align == TextAlign.Center ? TextAlignment.Center : align == TextAlign.Right ? TextAlignment.Right : TextAlignment.Left;
            graphics.DrawText(text, R(x, y, width, height), alignment, C(colour), Font(style));
        }

        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint)
        {
            GTA.Texture t = textures.Get(texture);
            if (t != null) { graphics.DrawSprite(t, R(x, y, width, height), C(tint)); }
        }

        // ScriptHookDotNet's rotated overload takes the sprite centre, size and rotation.
        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint, float rotationDegrees)
        {
            if (rotationDegrees == 0f) { Sprite(texture, x, y, width, height, tint); return; }
            GTA.Texture t = textures.Get(texture);
            if (t == null) { return; }
            graphics.DrawSprite(t, (x + width / 2) * scale, (y + height / 2) * scale, width * scale, height * scale,
                (float)(rotationDegrees * Math.PI / 180.0), C(tint));
        }

        public void Line(float x1, float y1, float x2, float y2, float thickness, Rgba colour)
        {
            graphics.DrawLine(x1 * scale, y1 * scale, x2 * scale, y2 * scale, Math.Max(1f, thickness * scale), C(colour));
        }

        private GTA.Font Font(TextStyle style)
        {
            int index = Math.Max(0, Math.Min(StyleSizes.Length - 1, (int)style));
            GTA.Font font;
            if (!fonts.TryGetValue(index, out font))
            {
                font = new GTA.Font(StyleSizes[index] * scale, FontScaling.Pixel, StyleBold[index], false);
                fonts[index] = font;
            }
            return font;
        }

        private void DisposeFonts()
        {
            foreach (GTA.Font font in fonts.Values) { font.Dispose(); }
            fonts.Clear();
        }

        private static float Clamp01(float v) { return v < 0 ? 0 : v > 1 ? 1 : v; }
    }
}
