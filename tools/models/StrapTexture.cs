using System;

namespace LibertyFramework.Models
{
    // Procedural worn-leather strap texture: U runs across the strap, V along it. Oiled leather base with fine grain,
    // darker burnished edges, a row of saddle stitching inside each edge, and sparse lighter scuffs. Deterministic
    // (seeded), so rebuilding gives identical output.
    internal static class StrapTexture
    {
        internal sealed class Look
        {
            internal int[] BaseColour;
            internal int[] EdgeColour;
            internal int[] StitchColour;
            internal double GrainStrength;
            internal double ScuffStrength;
            internal double StitchInsetFraction;
            internal int StitchPeriodPixels;
        }

        // Returns width*height*3 RGB.
        internal static byte[] Generate(int width, int height, Look look, int seed)
        {
            byte[] rgb = new byte[width * height * 3];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double u = (x + 0.5) / width, v = (y + 0.5) / height;
                    double grain = Noise(x * 0.9, y * 0.35, seed) * 0.6 + Noise(x * 0.25, y * 0.08, seed + 7) * 0.4 - 0.5;
                    double scuff = Math.Max(0, Noise(x * 0.06, y * 0.02, seed + 13) - 0.62) * 2.6;
                    double edge = Math.Min(u, 1 - u);
                    double burnish = edge < 0.08 ? 1 - edge / 0.08 : 0;
                    double[] colour = new double[3];
                    for (int c = 0; c < 3; c++)
                    {
                        double baseValue = look.BaseColour[c] * (1 + grain * look.GrainStrength) + scuff * look.ScuffStrength * 255;
                        colour[c] = baseValue * (1 - burnish) + look.EdgeColour[c] * burnish;
                    }
                    double stitchU = Math.Abs(edge - look.StitchInsetFraction);
                    bool stitch = stitchU < 0.5 / width * 1.6 && (y % look.StitchPeriodPixels) < look.StitchPeriodPixels * 0.62;
                    if (stitch) { for (int c = 0; c < 3; c++) { colour[c] = look.StitchColour[c] * (0.85 + 0.15 * Noise(x, y, seed + 21)); } }
                    else if (Math.Abs(edge - look.StitchInsetFraction) < 1.6 / width) { for (int c = 0; c < 3; c++) { colour[c] *= 0.82; } }
                    int at = (y * width + x) * 3;
                    for (int c = 0; c < 3; c++) { rgb[at + c] = (byte)Math.Max(0, Math.Min(255, Math.Round(colour[c]))); }
                }
            }
            return rgb;
        }

        // 2x2 box downsample of an RGB image.
        internal static byte[] Half(byte[] rgb, int width, int height)
        {
            int w = Math.Max(1, width / 2), h = Math.Max(1, height / 2);
            byte[] output = new byte[w * h * 3];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        int sum = 0;
                        for (int dy = 0; dy < 2; dy++) { for (int dx = 0; dx < 2; dx++) { sum += rgb[((Math.Min(height - 1, y * 2 + dy)) * width + Math.Min(width - 1, x * 2 + dx)) * 3 + c]; } }
                        output[(y * w + x) * 3 + c] = (byte)(sum / 4);
                    }
                }
            }
            return output;
        }

        // Smooth value noise in [0,1].
        private static double Noise(double x, double y, int seed)
        {
            int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y);
            double fx = x - x0, fy = y - y0;
            fx = fx * fx * (3 - 2 * fx); fy = fy * fy * (3 - 2 * fy);
            double a = Hash(x0, y0, seed), b = Hash(x0 + 1, y0, seed), c = Hash(x0, y0 + 1, seed), d = Hash(x0 + 1, y0 + 1, seed);
            return (a * (1 - fx) + b * fx) * (1 - fy) + (c * (1 - fx) + d * fx) * fy;
        }

        private static double Hash(int x, int y, int seed)
        {
            unchecked
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                return (h & 0xFFFFFF) / (double)0xFFFFFF;
            }
        }
    }
}
