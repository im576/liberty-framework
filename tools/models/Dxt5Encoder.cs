using System;

namespace LibertyFramework.Models
{
    // DXT5 (BC3) encoder: per 4x4 block, 8 bytes of interpolated alpha followed by an 8-byte colour block. The colour
    // block comes from Dxt1Encoder.EncodeBlock (4-colour mode, which is how DXT5 colour always decodes). Alpha tries both
    // DXT5 alpha modes and keeps the one with the smaller squared error:
    //   8-value mode (a0 > a1): a0/a1 are the block's max/min alpha, six values interpolated between them;
    //   6-value mode (a0 <= a1): endpoints span the alpha values other than 0 and 255, which get their own codes (6, 7).
    internal static class Dxt5Encoder
    {
        internal const int BlockBytes = 16;

        // pixels: width*height*4 bytes RGBA. Returns ceil(width/4) * ceil(height/4) * 16 bytes.
        internal static byte[] Encode(byte[] pixels, int width, int height)
        {
            if (pixels.Length < width * height * 4) { throw new ArgumentException("DXT5 input needs " + (width * height * 4) + " RGBA bytes, got " + pixels.Length); }
            int blocksWide = Math.Max(1, (width + 3) / 4), blocksHigh = Math.Max(1, (height + 3) / 4);
            byte[] output = new byte[blocksWide * blocksHigh * BlockBytes];
            double[][] colour = new double[16][];
            for (int i = 0; i < 16; i++) { colour[i] = new double[3]; }
            int[] alpha = new int[16];
            for (int by = 0; by < blocksHigh; by++)
            {
                for (int bx = 0; bx < blocksWide; bx++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int x = Math.Min(width - 1, bx * 4 + i % 4), y = Math.Min(height - 1, by * 4 + i / 4);
                        int at = (y * width + x) * 4;
                        colour[i][0] = pixels[at]; colour[i][1] = pixels[at + 1]; colour[i][2] = pixels[at + 2];
                        alpha[i] = pixels[at + 3];
                    }
                    int blockAt = (by * blocksWide + bx) * BlockBytes;
                    EncodeAlphaBlock(alpha, output, blockAt);
                    Dxt1Encoder.EncodeBlock(colour, output, blockAt + 8);
                }
            }
            return output;
        }

        internal static void EncodeAlphaBlock(int[] alpha, byte[] output, int at)
        {
            int min = 255, max = 0, innerMin = 255, innerMax = 0;
            foreach (int a in alpha)
            {
                min = Math.Min(min, a); max = Math.Max(max, a);
                if (a != 0 && a != 255) { innerMin = Math.Min(innerMin, a); innerMax = Math.Max(innerMax, a); }
            }
            ulong bestBits = 0;
            int bestA0 = max, bestA1 = min;
            long bestError = long.MaxValue;
            // 8-value mode needs a0 > a1; a uniform block uses a0 == a1 in 6-value mode, where index 0 is exact.
            if (max > min) { TryMode(alpha, max, min, ref bestA0, ref bestA1, ref bestBits, ref bestError); }
            if (innerMin > innerMax) { innerMin = innerMax = min == 255 ? 255 : 0; }
            TryMode(alpha, innerMin, innerMax, ref bestA0, ref bestA1, ref bestBits, ref bestError);
            output[at] = (byte)bestA0;
            output[at + 1] = (byte)bestA1;
            for (int i = 0; i < 6; i++) { output[at + 2 + i] = (byte)(bestBits >> (8 * i)); }
        }

        private static void TryMode(int[] alpha, int a0, int a1, ref int bestA0, ref int bestA1, ref ulong bestBits, ref long bestError)
        {
            int[] palette = Palette(a0, a1);
            ulong bits = 0;
            long error = 0;
            for (int i = 0; i < 16; i++)
            {
                int best = 0, bestDistance = int.MaxValue;
                for (int k = 0; k < 8; k++)
                {
                    int d = Math.Abs(alpha[i] - palette[k]);
                    if (d < bestDistance) { bestDistance = d; best = k; }
                }
                error += bestDistance * bestDistance;
                bits |= (ulong)best << (3 * i);
            }
            if (error < bestError) { bestError = error; bestBits = bits; bestA0 = a0; bestA1 = a1; }
        }

        // The DXT5 alpha palette as decoders build it (integer interpolation, both modes).
        internal static int[] Palette(int a0, int a1)
        {
            int[] palette = new int[8];
            palette[0] = a0; palette[1] = a1;
            if (a0 > a1)
            {
                for (int k = 1; k <= 6; k++) { palette[k + 1] = ((7 - k) * a0 + k * a1) / 7; }
            }
            else
            {
                for (int k = 1; k <= 4; k++) { palette[k + 1] = ((5 - k) * a0 + k * a1) / 5; }
                palette[6] = 0; palette[7] = 255;
            }
            return palette;
        }
    }
}
