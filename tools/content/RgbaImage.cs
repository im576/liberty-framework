using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        // A bitmap at width x height, multiplied by the material's base colour factor (RGBA, 0..1). Resampling is Resize, not
        // GDI+: GDI+'s scaler fades alpha at the image edges (seen with Mono's libgdiplus, whose wrap modes do not stop it),
        // which would make an opaque texture translucent at its border once alpha is kept (native texture mode).
        internal static RgbaImage FromBitmap(Bitmap source, int width, int height, float[] tint)
        {
            return FromBitmap(source, tint).Resize(width, height);
        }

        // A bitmap at its own size, multiplied by the base colour factor.
        internal static RgbaImage FromBitmap(Bitmap source, float[] tint)
        {
            RgbaImage image = new RgbaImage(source.Width, source.Height);
            // LockBits converts any source pixel format to 32-bit ARGB.
            BitmapData data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                byte[] row = new byte[source.Width * 4];
                for (int y = 0; y < source.Height; y++)
                {
                    Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), row, 0, row.Length);
                    for (int x = 0; x < source.Width; x++)
                    {
                        // GDI+ memory order is B, G, R, A.
                        int at = (y * source.Width + x) * 4;
                        image.Pixels[at] = Scale(row[x * 4 + 2], tint[0]);
                        image.Pixels[at + 1] = Scale(row[x * 4 + 1], tint[1]);
                        image.Pixels[at + 2] = Scale(row[x * 4], tint[2]);
                        image.Pixels[at + 3] = Scale(row[x * 4 + 3], tint.Length > 3 ? tint[3] : 1);
                    }
                }
            }
            finally { source.UnlockBits(data); }
            return image;
        }

        // Separable tent filter, clamped at the edges: bilinear when enlarging, area-weighted when reducing, an exact copy at
        // the same size. Each channel is filtered on its own, so a fully opaque image stays fully opaque.
        internal RgbaImage Resize(int width, int height)
        {
            if (width == Width && height == Height) { return new RgbaImage(width, height, (byte[])Pixels.Clone()); }
            List<KeyValuePair<int, double>>[] columns = Weights(Width, width), rows = Weights(Height, height);
            double[] horizontal = new double[Height * width * 4];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    foreach (KeyValuePair<int, double> w in columns[x])
                    {
                        for (int c = 0; c < 4; c++) { horizontal[(y * width + x) * 4 + c] += Pixels[(y * Width + w.Key) * 4 + c] * w.Value; }
                    }
                }
            }
            RgbaImage output = new RgbaImage(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        double sum = 0;
                        foreach (KeyValuePair<int, double> w in rows[y]) { sum += horizontal[(w.Key * width + x) * 4 + c] * w.Value; }
                        output.Pixels[(y * width + x) * 4 + c] = (byte)Math.Max(0, Math.Min(255, Math.Round(sum)));
                    }
                }
            }
            return output;
        }

        // Normalised source weights for each output sample along one axis (tent of half-width max(1, scale), source index
        // clamped to the image).
        private static List<KeyValuePair<int, double>>[] Weights(int sourceSize, int outputSize)
        {
            double scale = (double)sourceSize / outputSize, support = Math.Max(1.0, scale);
            List<KeyValuePair<int, double>>[] weights = new List<KeyValuePair<int, double>>[outputSize];
            for (int i = 0; i < outputSize; i++)
            {
                double centre = (i + 0.5) * scale - 0.5;
                Dictionary<int, double> sums = new Dictionary<int, double>();
                double total = 0;
                for (int j = (int)Math.Floor(centre - support); j <= (int)Math.Ceiling(centre + support); j++)
                {
                    double w = 1 - Math.Abs(j - centre) / support;
                    if (w <= 0) { continue; }
                    int clamped = Math.Max(0, Math.Min(sourceSize - 1, j));
                    double existing;
                    sums.TryGetValue(clamped, out existing);
                    sums[clamped] = existing + w;
                    total += w;
                }
                weights[i] = sums.OrderBy(p => p.Key).Select(p => new KeyValuePair<int, double>(p.Key, p.Value / total)).ToList();
            }
            return weights;
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

        // A 32-bit ARGB bitmap of these pixels (previews); the caller disposes it.
        internal Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                byte[] row = new byte[Width * 4];
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int at = (y * Width + x) * 4;
                        row[x * 4] = Pixels[at + 2]; row[x * 4 + 1] = Pixels[at + 1]; row[x * 4 + 2] = Pixels[at]; row[x * 4 + 3] = Pixels[at + 3];
                    }
                    Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, y * data.Stride), row.Length);
                }
            }
            finally { bitmap.UnlockBits(data); }
            return bitmap;
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
