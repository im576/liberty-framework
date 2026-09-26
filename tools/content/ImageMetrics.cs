using System;

namespace LibertyFramework.Content
{
    // Pixel comparisons between a source image and what a compiled texture decodes to.
    internal static class ImageMetrics
    {
        internal const double IdenticalPsnrDb = 99;

        // Peak signal-to-noise ratio over RGB (channels 0-2) or alpha (channel 3), in dB. Identical images give 99.
        internal static double PsnrDb(RgbaImage expected, RgbaImage actual, bool alpha)
        {
            CheckSameSize(expected, actual);
            double squared = 0;
            int samples = 0;
            for (int i = 0; i < expected.Pixels.Length; i += 4)
            {
                for (int c = alpha ? 3 : 0; c < (alpha ? 4 : 3); c++)
                {
                    double e = expected.Pixels[i + c] - actual.Pixels[i + c];
                    squared += e * e;
                    samples++;
                }
            }
            if (squared == 0) { return IdenticalPsnrDb; }
            return Math.Min(IdenticalPsnrDb, 10 * Math.Log10(255.0 * 255.0 / (squared / samples)));
        }

        // Largest absolute difference of one channel group (RGB or alpha), 0-255.
        internal static int MaxError(RgbaImage expected, RgbaImage actual, bool alpha)
        {
            CheckSameSize(expected, actual);
            int max = 0;
            for (int i = 0; i < expected.Pixels.Length; i += 4)
            {
                for (int c = alpha ? 3 : 0; c < (alpha ? 4 : 3); c++) { max = Math.Max(max, Math.Abs(expected.Pixels[i + c] - actual.Pixels[i + c])); }
            }
            return max;
        }

        private static void CheckSameSize(RgbaImage a, RgbaImage b)
        {
            if (a.Width != b.Width || a.Height != b.Height) { throw new ArgumentException("image sizes differ: " + a.Width + "x" + a.Height + " vs " + b.Width + "x" + b.Height); }
        }
    }
}
