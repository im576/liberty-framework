using System;

namespace LibertyFramework.Finishes
{
    // Recolours DXT1/3/5 textures in place by remapping each block's two RGB565 endpoint colours
    // through a luminance ramp (shadow -> mid -> highlight). Per-pixel indices and alpha are kept,
    // so detail, wear and transparency survive exactly; only the palette changes. Endpoint order is
    // preserved because it selects the DXT1 4-colour vs 3-colour+transparent mode.
    internal static class DxtRecolor
    {
        internal static int Apply(byte[] body, TextureDictionary.Texture texture, ColourRamp ramp)
        {
            int offset = texture.DataOffset;
            int blocks = 0;
            int colourOffsetInBlock = texture.Format == "DXT1" ? 0 : 8;
            for (int level = 0; level < Math.Max(1, texture.Levels); level++)
            {
                int bytes = texture.LevelBytes(level);
                for (int block = offset; block < offset + bytes; block += texture.BlockBytes)
                {
                    RecolourBlock(body, block + colourOffsetInBlock, ramp, texture.Format == "DXT1");
                    blocks++;
                }
                offset += bytes;
            }
            return blocks;
        }

        private static void RecolourBlock(byte[] body, int at, ColourRamp ramp, bool dxt1)
        {
            ushort c0 = BitConverter.ToUInt16(body, at);
            ushort c1 = BitConverter.ToUInt16(body, at + 2);
            bool fourColour = !dxt1 || c0 > c1;
            ushort n0 = Map(c0, ramp);
            ushort n1 = Map(c1, ramp);
            uint indices = BitConverter.ToUInt32(body, at + 4);
            if (fourColour && n0 <= n1)
            {
                if (n0 == n1)
                {
                    // Both endpoints collapsed to one colour: keep 4-colour mode by nudging, or use index 0 everywhere.
                    if (n0 < 0xFFFF) { n0++; } else { n1--; }
                }
                else
                {
                    Swap(ref n0, ref n1);
                    indices = SwapIndices(indices, true);
                }
            }
            else if (!fourColour && n0 > n1)
            {
                Swap(ref n0, ref n1);
                indices = SwapIndices(indices, false);
            }
            WriteUInt16(body, at, n0);
            WriteUInt16(body, at + 2, n1);
            WriteUInt32(body, at + 4, indices);
        }

        // 4-colour mode: 0<->1 and 2<->3. 3-colour mode: 0<->1, 2 (midpoint) and 3 (transparent) unchanged.
        private static uint SwapIndices(uint indices, bool fourColour)
        {
            uint result = 0;
            for (int pixel = 0; pixel < 16; pixel++)
            {
                uint value = (indices >> (pixel * 2)) & 3;
                if (value == 0) { value = 1; }
                else if (value == 1) { value = 0; }
                else if (fourColour) { value = value == 2 ? 3u : 2u; }
                result |= value << (pixel * 2);
            }
            return result;
        }

        internal static ushort Map(ushort rgb565, ColourRamp ramp)
        {
            double r = ((rgb565 >> 11) & 31) / 31.0;
            double g = ((rgb565 >> 5) & 63) / 63.0;
            double b = (rgb565 & 31) / 31.0;
            double luminance = 0.299 * r + 0.587 * g + 0.114 * b;
            luminance = Math.Max(0, Math.Min(1, (luminance - 0.5) * ramp.Contrast + 0.5));
            luminance = ramp.Lift + (1 - ramp.Lift) * Math.Pow(luminance, ramp.Gamma);
            double[] colour = luminance < 0.5 ?
                Lerp(ramp.Shadow, ramp.Mid, luminance * 2) :
                Lerp(ramp.Mid, ramp.Highlight, (luminance - 0.5) * 2);
            int r5 = (int)Math.Round(colour[0] / 255.0 * 31);
            int g6 = (int)Math.Round(colour[1] / 255.0 * 63);
            int b5 = (int)Math.Round(colour[2] / 255.0 * 31);
            return (ushort)((r5 << 11) | (g6 << 5) | b5);
        }

        private static double[] Lerp(int[] a, int[] b, double t)
        {
            return new[] { a[0] + (b[0] - a[0]) * t, a[1] + (b[1] - a[1]) * t, a[2] + (b[2] - a[2]) * t };
        }

        private static void Swap(ref ushort a, ref ushort b)
        {
            ushort temporary = a;
            a = b;
            b = temporary;
        }

        private static void WriteUInt16(byte[] data, int at, ushort value)
        {
            data[at] = (byte)value;
            data[at + 1] = (byte)(value >> 8);
        }

        private static void WriteUInt32(byte[] data, int at, uint value)
        {
            for (int index = 0; index < 4; index++) { data[at + index] = (byte)(value >> (index * 8)); }
        }
    }
}
