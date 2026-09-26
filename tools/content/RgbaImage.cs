using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LibertyFramework.Content
{
    // 8-bit RGBA pixels, row-major, top row first (the order glTF, GDI+ and DXT blocks share).
    internal sealed class RgbaImage
    {
        internal readonly int Width;
        internal readonly int Height;
        internal readonly byte[] Pixels;

        internal RgbaImage(int width, int height)
        {
            if (width < 1 || height < 1) { throw new ArgumentException("image size " + width + "x" + height); }
            Width = width; Height = height; Pixels = new byte[width * height * 4];
        }

        internal RgbaImage(int width, int height, byte[] pixels)
        {
            if (pixels.Length != width * height * 4) { throw new ArgumentException("RGBA buffer is " + pixels.Length + " bytes, expected " + (width * height * 4)); }
            Width = width; Height = height; Pixels = pixels;
        }

        internal static RgbaImage Solid(int width, int height, byte red, byte green, byte blue, byte alpha)
        {
            RgbaImage image = new RgbaImage(width, height);
            for (int i = 0; i < width * height; i++) { image.Pixels[i * 4] = red; image.Pixels[i * 4 + 1] = green; image.Pixels[i * 4 + 2] = blue; image.Pixels[i * 4 + 3] = alpha; }
            return image;
        }

        // Resamples a bitmap to width x height (high-quality bicubic, mirrored edges as PropCompiler's template path does)
        // and multiplies it by the material's base colour factor (RGBA, 0..1).
        internal static RgbaImage FromBitmap(Bitmap source, int width, int height, float[] tint)
        {
            RgbaImage image = new RgbaImage(width, height);
            using (Bitmap scaled = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(scaled))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    using (ImageAttributes wrap = new ImageAttributes())
                    {
                        wrap.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(source, new Rectangle(0, 0, width, height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, wrap);
                    }
                }
                BitmapData data = scaled.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    byte[] row = new byte[width * 4];
                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), row, 0, row.Length);
                        for (int x = 0; x < width; x++)
                        {
                            // GDI+ memory order is B, G, R, A.
                            int at = (y * width + x) * 4;
                            image.Pixels[at] = Scale(row[x * 4 + 2], tint[0]);
                            image.Pixels[at + 1] = Scale(row[x * 4 + 1], tint[1]);
                            image.Pixels[at + 2] = Scale(row[x * 4], tint[2]);
                            image.Pixels[at + 3] = Scale(row[x * 4 + 3], tint.Length > 3 ? tint[3] : 1);
                        }
                    }
                }
                finally { scaled.UnlockBits(data); }
            }
            return image;
        }

        // 2x2 box filter (the next mip level), clamped at 1 pixel.
        internal RgbaImage Half()
        {
            int w = Math.Max(1, Width / 2), h = Math.Max(1, Height / 2);
            RgbaImage output = new RgbaImage(w, h);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        int sum = 0;
                        for (int dy = 0; dy < 2; dy++)
                        {
                            for (int dx = 0; dx < 2; dx++) { sum += Pixels[(Math.Min(Height - 1, y * 2 + dy) * Width + Math.Min(Width - 1, x * 2 + dx)) * 4 + c]; }
                        }
                        output.Pixels[(y * w + x) * 4 + c] = (byte)((sum + 2) / 4);
                    }
                }
            }
            return output;
        }

        // RGB bytes (alpha dropped), the input Dxt1Encoder takes.
        internal byte[] ToRgb()
        {
            byte[] rgb = new byte[Width * Height * 3];
            for (int i = 0; i < Width * Height; i++) { rgb[i * 3] = Pixels[i * 4]; rgb[i * 3 + 1] = Pixels[i * 4 + 1]; rgb[i * 3 + 2] = Pixels[i * 4 + 2]; }
            return rgb;
        }

        internal bool HasTranslucency()
        {
            for (int i = 3; i < Pixels.Length; i += 4) { if (Pixels[i] != 255) { return true; } }
            return false;
        }

        private static byte Scale(byte value, float factor)
        {
            return (byte)Math.Max(0, Math.Min(255, Math.Round(value * Math.Max(0f, Math.Min(1f, factor)))));
        }
    }
}
