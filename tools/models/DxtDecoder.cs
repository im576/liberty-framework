using System;

namespace LibertyFramework.Models
{
    // Reference decoder for DXT1/DXT3/DXT5 (BC1-3) to RGBA, any mip level. Used by the content compiler's self-test and
    // read-back to compare what a written texture dictionary holds with the source pixels. RGB565 endpoints expand by bit
    // replication, as Dxt1Encoder assumes; interpolation uses integer thirds (hardware rounding may differ by 1).
    internal static class DxtDecoder
    {
        internal static int BlockBytes(string format)
        {
            if (format == "DXT1") { return 8; }
            if (format == "DXT3" || format == "DXT5") { return 16; }
            throw new ArgumentException("not a DXT format: " + format);
        }

        internal static int LevelBytes(string format, int width, int height)
        {
            return Math.Max(1, (width + 3) / 4) * Math.Max(1, (height + 3) / 4) * BlockBytes(format);
        }

        // Returns width*height*4 bytes RGBA decoded from data[offset..].
        internal static byte[] Decode(byte[] data, int offset, string format, int width, int height)
        {
            int blockBytes = BlockBytes(format);
            int blocksWide = Math.Max(1, (width + 3) / 4), blocksHigh = Math.Max(1, (height + 3) / 4);
            if (offset < 0 || offset + blocksWide * blocksHigh * blockBytes > data.Length)
            {
                throw new ArgumentException(format + " " + width + "x" + height + " at " + offset + " runs past the data (" + data.Length + " bytes)");
            }
            byte[] rgba = new byte[width * height * 4];
            int[][] colours = new int[4][];
            int[] alphaPalette = new int[8];
            for (int by = 0; by < blocksHigh; by++)
            {
                for (int bx = 0; bx < blocksWide; bx++)
                {
                    int at = offset + (by * blocksWide + bx) * blockBytes;
                    int colourAt = format == "DXT1" ? at : at + 8;
                    ushort c0 = BitConverter.ToUInt16(data, colourAt), c1 = BitConverter.ToUInt16(data, colourAt + 2);
                    // DXT1 uses 3-colour + transparent mode when c0 <= c1; DXT3/DXT5 colour is always 4-colour.
                    bool fourColour = format != "DXT1" || c0 > c1;
                    FillPalette(c0, c1, fourColour, colours);
                    uint indices = BitConverter.ToUInt32(data, colourAt + 4);
                    ulong alphaBits = 0;
                    if (format == "DXT5")
                    {
                        int[] palette = Dxt5Encoder.Palette(data[at], data[at + 1]);
                        Array.Copy(palette, alphaPalette, 8);
                        for (int i = 0; i < 6; i++) { alphaBits |= (ulong)data[at + 2 + i] << (8 * i); }
                    }
                    for (int pixel = 0; pixel < 16; pixel++)
                    {
                        int x = bx * 4 + pixel % 4, y = by * 4 + pixel / 4;
                        if (x >= width || y >= height) { continue; }
                        int[] colour = colours[(indices >> (pixel * 2)) & 3];
                        int outAt = (y * width + x) * 4;
                        rgba[outAt] = (byte)colour[0]; rgba[outAt + 1] = (byte)colour[1]; rgba[outAt + 2] = (byte)colour[2];
                        if (format == "DXT1") { rgba[outAt + 3] = (byte)colour[3]; }
                        else if (format == "DXT3")
                        {
                            int nibble = (data[at + pixel / 2] >> ((pixel & 1) * 4)) & 0xF;
                            rgba[outAt + 3] = (byte)(nibble * 17);
                        }
                        else { rgba[outAt + 3] = (byte)alphaPalette[(int)((alphaBits >> (3 * pixel)) & 7)]; }
                    }
                }
            }
            return rgba;
        }

        private static void FillPalette(ushort c0, ushort c1, bool fourColour, int[][] palette)
        {
            int[] a = Expand(c0), b = Expand(c1);
            palette[0] = a; palette[1] = b;
            if (fourColour)
            {
                palette[2] = new[] { (2 * a[0] + b[0]) / 3, (2 * a[1] + b[1]) / 3, (2 * a[2] + b[2]) / 3, 255 };
                palette[3] = new[] { (a[0] + 2 * b[0]) / 3, (a[1] + 2 * b[1]) / 3, (a[2] + 2 * b[2]) / 3, 255 };
            }
            else
            {
                palette[2] = new[] { (a[0] + b[0]) / 2, (a[1] + b[1]) / 2, (a[2] + b[2]) / 2, 255 };
                palette[3] = new[] { 0, 0, 0, 0 };
            }
        }

        private static int[] Expand(ushort value)
        {
            int r = (value >> 11) & 31, g = (value >> 5) & 63, b = value & 31;
            return new[] { (r << 3) | (r >> 2), (g << 2) | (g >> 4), (b << 3) | (b >> 2), 255 };
        }
    }
}
