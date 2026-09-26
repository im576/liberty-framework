using System;

namespace LibertyFramework.Models
{
    // Opaque DXT1 (BC1) encoder: per 4x4 block, endpoints are the extreme pixels along the block's principal colour
    // axis, quantised to RGB565 and ordered c0 > c1 so the block decodes in 4-colour mode; each pixel takes the
    // nearest of the four palette colours.
    internal static class Dxt1Encoder
    {
        // pixels: width*height*3 bytes RGB. Returns width/4 * height/4 * 8 bytes.
        internal static byte[] Encode(byte[] pixels, int width, int height)
        {
            int bw = Math.Max(1, (width + 3) / 4), bh = Math.Max(1, (height + 3) / 4);
            byte[] output = new byte[bw * bh * 8];
            double[][] block = new double[16][];
            for (int by = 0; by < bh; by++)
            {
                for (int bx = 0; bx < bw; bx++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int x = Math.Min(width - 1, bx * 4 + i % 4), y = Math.Min(height - 1, by * 4 + i / 4);
                        int at = (y * width + x) * 3;
                        block[i] = new double[] { pixels[at], pixels[at + 1], pixels[at + 2] };
                    }
                    EncodeBlock(block, output, (by * bw + bx) * 8);
                }
            }
            return output;
        }

        // Also the colour half of a DXT3/DXT5 block (Dxt5Encoder): those always decode in 4-colour mode, and this
        // encoder only ever writes 4-colour blocks (c0 > c1).
        internal static void EncodeBlock(double[][] block, byte[] output, int at)
        {
            double[] mean = new double[3];
            foreach (double[] p in block) { for (int c = 0; c < 3; c++) { mean[c] += p[c] / 16; } }
            // Principal axis by power iteration on the covariance matrix.
            double[,] cov = new double[3, 3];
            foreach (double[] p in block)
            {
                for (int r = 0; r < 3; r++) { for (int c = 0; c < 3; c++) { cov[r, c] += (p[r] - mean[r]) * (p[c] - mean[c]); } }
            }
            double[] axis = { 1, 1, 1 };
            for (int k = 0; k < 8; k++)
            {
                double[] next = new double[3];
                for (int r = 0; r < 3; r++) { for (int c = 0; c < 3; c++) { next[r] += cov[r, c] * axis[c]; } }
                double l = Math.Sqrt(next[0] * next[0] + next[1] * next[1] + next[2] * next[2]);
                if (l < 1e-9) { break; }
                axis = new[] { next[0] / l, next[1] / l, next[2] / l };
            }
            double min = double.MaxValue, max = double.MinValue;
            double[] low = mean, high = mean;
            foreach (double[] p in block)
            {
                double t = (p[0] - mean[0]) * axis[0] + (p[1] - mean[1]) * axis[1] + (p[2] - mean[2]) * axis[2];
                if (t < min) { min = t; low = p; }
                if (t > max) { max = t; high = p; }
            }
            ushort c0 = To565(high), c1 = To565(low);
            if (c0 < c1) { ushort swap = c0; c0 = c1; c1 = swap; }
            if (c0 == c1)
            {
                // Uniform block: 4-colour mode needs c0 > c1; every index 0 selects c0.
                if (c0 > 0) { c1 = (ushort)(c0 - 1); } else { c0 = 1; }
            }
            double[][] palette = new double[4][];
            palette[0] = From565(c0); palette[1] = From565(c1);
            palette[2] = new double[3]; palette[3] = new double[3];
            for (int c = 0; c < 3; c++)
            {
                palette[2][c] = (2 * palette[0][c] + palette[1][c]) / 3;
                palette[3][c] = (palette[0][c] + 2 * palette[1][c]) / 3;
            }
            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                int best = 0; double bestDistance = double.MaxValue;
                for (int k = 0; k < 4; k++)
                {
                    double d = 0;
                    for (int c = 0; c < 3; c++) { double e = block[i][c] - palette[k][c]; d += e * e; }
                    if (d < bestDistance) { bestDistance = d; best = k; }
                }
                indices |= (uint)best << (i * 2);
            }
            output[at] = (byte)c0; output[at + 1] = (byte)(c0 >> 8);
            output[at + 2] = (byte)c1; output[at + 3] = (byte)(c1 >> 8);
            Buffer.BlockCopy(BitConverter.GetBytes(indices), 0, output, at + 4, 4);
        }

        private static ushort To565(double[] rgb)
        {
            int r = Clamp((int)Math.Round(rgb[0] * 31 / 255)), g = Math.Min(63, Math.Max(0, (int)Math.Round(rgb[1] * 63 / 255))), b = Clamp((int)Math.Round(rgb[2] * 31 / 255));
            return (ushort)((r << 11) | (g << 5) | b);
        }

        private static int Clamp(int value) { return Math.Min(31, Math.Max(0, value)); }

        private static double[] From565(ushort c)
        {
            int r = (c >> 11) & 31, g = (c >> 5) & 63, b = c & 31;
            return new double[] { (r << 3) | (r >> 2), (g << 2) | (g >> 4), (b << 3) | (b >> 2) };
        }
    }
}
