using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LibertyFramework.Finishes
{
    // Decodes the top mip level to a PNG so a finish can be inspected without starting the game.
    internal static class DxtPreview
    {
        internal static void Save(byte[] body, TextureDictionary.Texture texture, string path)
        {
            using (Bitmap bitmap = new Bitmap(texture.Width, texture.Height, PixelFormat.Format32bppArgb))
            {
                int blocksWide = Math.Max(1, (texture.Width + 3) / 4);
                int offset = texture.DataOffset;
                int blockCount = texture.LevelBytes(0) / texture.BlockBytes;
                for (int block = 0; block < blockCount; block++)
                {
                    int at = offset + block * texture.BlockBytes;
                    int colourAt = texture.Format == "DXT1" ? at : at + 8;
                    Color[] palette = Palette(BitConverter.ToUInt16(body, colourAt), BitConverter.ToUInt16(body, colourAt + 2), texture.Format == "DXT1");
                    uint indices = BitConverter.ToUInt32(body, colourAt + 4);
                    int bx = (block % blocksWide) * 4;
                    int by = (block / blocksWide) * 4;
                    for (int pixel = 0; pixel < 16; pixel++)
                    {
                        int x = bx + pixel % 4;
                        int y = by + pixel / 4;
                        if (x >= texture.Width || y >= texture.Height) { continue; }
                        bitmap.SetPixel(x, y, palette[(indices >> (pixel * 2)) & 3]);
                    }
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }

        private static Color[] Palette(ushort c0, ushort c1, bool dxt1)
        {
            Color a = Expand(c0);
            Color b = Expand(c1);
            if (!dxt1 || c0 > c1)
            {
                return new[] { a, b, Mix(a, b, 2, 1), Mix(a, b, 1, 2) };
            }
            return new[] { a, b, Mix(a, b, 1, 1), Color.FromArgb(0, 0, 0, 0) };
        }

        private static Color Expand(ushort value)
        {
            int r = (value >> 11) & 31;
            int g = (value >> 5) & 63;
            int b = value & 31;
            return Color.FromArgb(255, r * 255 / 31, g * 255 / 63, b * 255 / 31);
        }

        private static Color Mix(Color a, Color b, int wa, int wb)
        {
            int total = wa + wb;
            return Color.FromArgb(255, (a.R * wa + b.R * wb) / total, (a.G * wa + b.G * wb) / total, (a.B * wa + b.B * wb) / total);
        }
    }
}
