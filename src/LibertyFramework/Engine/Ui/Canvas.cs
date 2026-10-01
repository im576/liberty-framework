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
        // Developer bisect only; normal rendering is always "all" after engine restart.
        internal string DiagnosticMode = "all";

        // Returns false when the screen size is not known yet.
        internal bool Begin(GTA.Graphics target, Size screen)
        {
            if (screen.Height <= 0 || screen.Width <= 0) { return false; }
            graphics = target;
            shdnTextCalls = 0;
            createdThisFrame = 0;
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
            if (DiagnosticMode == "none" || DiagnosticMode == "text") { return; }
            graphics.DrawRectangle(R(x, y, width, height), C(colour));
        }

        // "sprite" (default): text is rendered once with GDI+ into a cached texture and drawn as a sprite. "shdn": ScriptHookDotNet's
        // Graphics.DrawText. Measured in game (T-045, 2026-10-01): every frame that drew several strings through DrawText took
        // 0.4 to 0.9 s while rectangles and sprites cost nothing, and the cause inside DrawText is not established, so the
        // shared canvas does not use it by default. Switch back with engine.json uiTextRenderer or `ui-text-renderer shdn`.
        internal string TextRenderer = "sprite";
        // Diagnostic: with "shdn", only this many strings are drawn per frame.
        internal int ShdnTextLimit = int.MaxValue;
        private int shdnTextCalls;
        // Text sprite cache: least recently used first out, bounded in entries (engine.json uiTextCacheEntries) and in new
        // textures per frame (uiTextNewSpritesPerFrame): a frame that needs more than that draws the rest on the next frames
        // instead of hitching on one frame (each new string is a GDI+ render, a PNG encode and a texture).
        internal int TextSpriteCap = 600;
        internal int NewSpritesPerFrame = 8;
        private int createdThisFrame;
        private long createdTotal, evictedTotal, deferredTotal, spriteBytes;
        private sealed class TextSprite { internal TextureRef Texture; internal string Key; internal float Width, Height; internal LinkedListNode<string> Node; }
        private readonly Dictionary<string, TextSprite> textSprites = new Dictionary<string, TextSprite>();
        private readonly LinkedList<string> textSpriteUse = new LinkedList<string>();

        internal string TextStats()
        {
            return "ui_text_sprites renderer=" + TextRenderer + " count=" + textSprites.Count + "/" + TextSpriteCap + " estimated_bytes=" + spriteBytes +
                " created=" + createdTotal + " evicted=" + evictedTotal + " deferred_frames=" + deferredTotal + " new_per_frame=" + NewSpritesPerFrame;
        }

        private void SpriteText(string text, float x, float y, float width, TextStyle style, TextAlign align, Rgba colour)
        {
            int index = Math.Max(0, Math.Min(StyleSizes.Length - 1, (int)style));
            float pixelSize = StyleSizes[index] * scale, rectWidth = width * scale;
            string key = Logic.UiTextLogic.CacheKey(index, pixelSize, rectWidth, StyleBold[index], text);
            TextSprite sprite;
            if (textSprites.TryGetValue(key, out sprite))
            {
                textSpriteUse.Remove(sprite.Node);
                textSpriteUse.AddFirst(sprite.Node);
            }
            else
            {
                if (createdThisFrame >= NewSpritesPerFrame) { deferredTotal++; return; }
                createdThisFrame++;
                sprite = RenderText(text, pixelSize, rectWidth, StyleBold[index], key);
                sprite.Node = textSpriteUse.AddFirst(key);
                textSprites[key] = sprite;
                createdTotal++;
                spriteBytes += (long)(sprite.Width * sprite.Height * 4f);
                while (textSprites.Count > TextSpriteCap)
                {
                    LinkedListNode<string> oldest = textSpriteUse.Last;
                    textSpriteUse.RemoveLast();
                    TextSprite gone;
                    if (textSprites.TryGetValue(oldest.Value, out gone))
                    {
                        textSprites.Remove(oldest.Value);
                        textures.Release(gone.Texture, gone.Key);
                        spriteBytes -= (long)(gone.Width * gone.Height * 4f);
                        evictedTotal++;
                    }
                }
                if (createdTotal % 200 == 0) { LibertyFramework.Core.Logging.RuntimeLog.Info(TextStats()); }
            }
            if (sprite.Texture.IsNone) { return; }
            GTA.Texture t = textures.Get(sprite.Texture);
            if (t == null) { return; }
            float left = Logic.UiTextLogic.AlignedX(x * scale, rectWidth, sprite.Width, align);
            graphics.DrawSprite(t, new RectangleF(left, y * scale, sprite.Width, sprite.Height), C(colour));
        }

        // Diagnostic (T-045 root cause): draws `ProbeCount` strings per frame through ScriptHookDotNet's DrawText in one of four
        // arrangements. same = one string, one font, N times; different = N strings, one font; sizes = one string, the five
        // canvas fonts; noeffect = one string, a font with Effect set to none. `none` turns it off.
        internal string ProbeMode = "none";
        internal int ProbeCount = 8;
        private GTA.Font probeFont;
        private string probeFontMode;

        internal void DrawProbe()
        {
            if (ProbeMode == "none" || graphics == null) { return; }
            if (probeFont != null && probeFontMode != null && probeFontMode != ProbeMode && ProbeMode != "noeffect") { probeFont = null; probeFontMode = null; }
            if (ProbeMode == "noeffect" && probeFont == null)
            {
                probeFont = new GTA.Font(StyleSizes[1] * scale, FontScaling.Pixel, false, false);
                probeFont.Effect = FontEffect.None;
            }
            // The calls the DevTools menu makes (its text is not slow): a font from the 2-argument constructor, the 4-argument DrawText
            // (the font's own colour), unscaled 17 px; and the same with the bold 4-argument constructor; and the canvas font through the
            // 4-argument DrawText. They separate the constructor, the overload and the colour argument from the effect.
            if (ProbeMode == "devfont" || ProbeMode == "devfontbold" || ProbeMode == "canvasfont4")
            {
                if (probeFont == null)
                {
                    probeFont = ProbeMode == "devfontbold" ? new GTA.Font(17.0F, FontScaling.Pixel, true, false) : new GTA.Font(17.0F, FontScaling.Pixel);
                    probeFontMode = ProbeMode;
                }
                for (int i = 0; i < ProbeCount; i++)
                {
                    graphics.DrawText("PROBE 0123456789", R(900, 40 + i * 24, 360, 22), TextAlignment.Left, ProbeMode == "canvasfont4" ? Font(TextStyle.Body) : probeFont);
                }
                return;
            }
            for (int i = 0; i < ProbeCount; i++)
            {
                string text = ProbeMode == "different" ? "PROBE " + i + " STRING " + (i * 7919 % 1000) : "PROBE 0123456789";
                GTA.Font font = ProbeMode == "sizes" ? Font((TextStyle)(i % StyleSizes.Length)) : ProbeMode == "noeffect" ? probeFont : Font(TextStyle.Body);
                graphics.DrawText(text, R(900, 40 + i * 24, 360, 22), TextAlignment.Left, Color.White, font);
            }
        }
        // White text with a one-pixel dark shadow, tight to its extent; the draw call tints it. Ellipsised to the rectangle.
        private TextSprite RenderText(string text, float pixelSize, float rectWidth, bool bold, string key)
        {
            TextSprite sprite = new TextSprite();
            sprite.Key = "text:" + key;
            try
            {
                using (System.Drawing.Font font = new System.Drawing.Font("Arial", pixelSize, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel))
                using (StringFormat format = new StringFormat(StringFormat.GenericTypographic))
                {
                    format.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
                    SizeF size;
                    using (Bitmap probe = new Bitmap(1, 1))
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(probe))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        text = Logic.UiTextLogic.FitText(text, (float)Math.Floor(rectWidth) - 3f,
                            candidate => g.MeasureString(candidate, font, 4096, format).Width);
                        if (text.Length == 0) { sprite.Texture = TextureRef.None; return sprite; }
                        size = g.MeasureString(text, font, 4096, format);
                    }
                    int w = (int)Math.Ceiling(size.Width) + 3, h = (int)Math.Ceiling(size.Height) + 3;
                    using (Bitmap bitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        using (Brush shadow = new SolidBrush(Color.FromArgb(170, 0, 0, 0))) { g.DrawString(text, font, shadow, 1f, 1f, format); }
                        g.DrawString(text, font, Brushes.White, 0f, 0f, format);
                        using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                        {
                            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            sprite.Texture = textures.Add(stream.ToArray(), sprite.Key);
                        }
                    }
                    sprite.Width = w; sprite.Height = h;
                }
            }
            catch (Exception error)
            {
                sprite.Texture = TextureRef.None;
                LibertyFramework.Core.Logging.RuntimeLog.Error("ui_text_render_failed length=" + (text == null ? 0 : text.Length) + " error=" + error.Message);
            }
            return sprite;
        }

        public void Text(string text, float x, float y, float width, float height, TextStyle style, TextAlign align, Rgba colour)
        {
            if (DiagnosticMode == "none" || DiagnosticMode == "primitives" || string.IsNullOrEmpty(text)) { return; }
            if (TextRenderer == "sprite") { SpriteText(text, x, y, width, style, align, colour); return; }
            if (++shdnTextCalls > ShdnTextLimit) { return; }
            TextAlignment alignment = align == TextAlign.Center ? TextAlignment.Center : align == TextAlign.Right ? TextAlignment.Right : TextAlignment.Left;
            graphics.DrawText(text, R(x, y, width, height), alignment, C(colour), Font(style));
        }

        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint)
        {
            if (DiagnosticMode == "none" || DiagnosticMode == "text") { return; }
            GTA.Texture t = textures.Get(texture);
            if (t != null) { graphics.DrawSprite(t, R(x, y, width, height), C(tint)); }
        }

        // ScriptHookDotNet's rotated overload takes the sprite centre, size and rotation.
        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint, float rotationDegrees)
        {
            if (DiagnosticMode == "none" || DiagnosticMode == "text") { return; }
            if (rotationDegrees == 0f) { Sprite(texture, x, y, width, height, tint); return; }
            GTA.Texture t = textures.Get(texture);
            if (t == null) { return; }
            graphics.DrawSprite(t, (x + width / 2) * scale, (y + height / 2) * scale, width * scale, height * scale,
                (float)(rotationDegrees * Math.PI / 180.0), C(tint));
        }

        public void Line(float x1, float y1, float x2, float y2, float thickness, Rgba colour)
        {
            if (DiagnosticMode == "none" || DiagnosticMode == "text") { return; }
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
