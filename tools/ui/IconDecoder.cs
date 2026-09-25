using System;
using System.Drawing;
using System.Drawing.Imaging;
using LibertyFramework.Finishes;

namespace LibertyFramework.UiTools
{
    // Top-mip decoder with alpha (DXT1 punch-through, DXT3 explicit, DXT5 interpolated, A8R8G8B8/X8R8G8B8/L8), for
    // HUD icons that must keep their transparent background.
    internal static class IconDecoder
    {
        internal static void Save(byte[] body, TextureDictionary.Texture texture, string path)
        {
            using (Bitmap bitmap = Decode(body, texture)) { bitmap.Save(path, ImageFormat.Png); }
        }

        internal static Bitmap Decode(byte[] body, TextureDictionary.Texture texture)
        {
            Bitmap bitmap = new Bitmap(texture.Width, texture.Height, PixelFormat.Format32bppArgb);
            int at = texture.DataOffset;
            if (!texture.Compressed)
            {
                for (int y = 0; y < texture.Height; y++)
                    for (int x = 0; x < texture.Width; x++)
                    {
                        if (texture.Format == "L8") { int l = body[at++]; bitmap.SetPixel(x, y, Color.FromArgb(255, l, l, l)); continue; }
                        int b = body[at], g = body[at + 1], r = body[at + 2], a = texture.Format == "X8R8G8B8" ? 255 : body[at + 3];
                        at += 4;
                        bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    }
                return bitmap;
            }
            int blocksWide = Math.Max(1, (texture.Width + 3) / 4);
            int blockCount = texture.LevelBytes(0) / texture.BlockBytes;
            for (int block = 0; block < blockCount; block++, at += texture.BlockBytes)
            {
                int[] alpha = new int[16];
                for (int i = 0; i < 16; i++) { alpha[i] = 255; }
                int colourAt = at;
                if (texture.Format == "DXT3")
                {
                    ulong bits = BitConverter.ToUInt64(body, at);
                    for (int i = 0; i < 16; i++) { alpha[i] = (int)((bits >> (i * 4)) & 15) * 17; }
                    colourAt = at + 8;
                }
                else if (texture.Format == "DXT5")
                {
                    int a0 = body[at], a1 = body[at + 1];
                    ulong bits = 0;
                    for (int i = 0; i < 6; i++) { bits |= (ulong)body[at + 2 + i] << (8 * i); }
                    int[] table = new int[8];
                    table[0] = a0; table[1] = a1;
                    if (a0 > a1) { for (int i = 1; i <= 6; i++) { table[i + 1] = ((7 - i) * a0 + i * a1) / 7; } }
                    else { for (int i = 1; i <= 4; i++) { table[i + 1] = ((5 - i) * a0 + i * a1) / 5; } table[6] = 0; table[7] = 255; }
                    for (int i = 0; i < 16; i++) { alpha[i] = table[(int)((bits >> (i * 3)) & 7)]; }
                    colourAt = at + 8;
                }
                ushort c0 = BitConverter.ToUInt16(body, colourAt), c1 = BitConverter.ToUInt16(body, colourAt + 2);
                Color[] palette = Palette(c0, c1, texture.Format == "DXT1");
                uint indices = BitConverter.ToUInt32(body, colourAt + 4);
                int bx = (block % blocksWide) * 4, by = (block / blocksWide) * 4;
                for (int pixel = 0; pixel < 16; pixel++)
                {
                    int x = bx + pixel % 4, y = by + pixel / 4;
                    if (x >= texture.Width || y >= texture.Height) { continue; }
                    Color colour = palette[(indices >> (pixel * 2)) & 3];
                    int a = texture.Format == "DXT1" ? colour.A : alpha[pixel];
                    bitmap.SetPixel(x, y, Color.FromArgb(a, colour.R, colour.G, colour.B));
                }
            }
            return bitmap;
        }

        private static Color[] Palette(ushort c0, ushort c1, bool dxt1)
        {
            Color a = Expand(c0), b = Expand(c1);
            if (!dxt1 || c0 > c1) { return new[] { a, b, Mix(a, b, 2, 1), Mix(a, b, 1, 2) }; }
            return new[] { a, b, Mix(a, b, 1, 1), Color.FromArgb(0, 0, 0, 0) };
        }

        private static Color Expand(ushort value)
        {
            return Color.FromArgb(255, ((value >> 11) & 31) * 255 / 31, ((value >> 5) & 63) * 255 / 63, (value & 31) * 255 / 31);
        }

        private static Color Mix(Color a, Color b, int wa, int wb)
        {
            int total = wa + wb;
            return Color.FromArgb(255, (a.R * wa + b.R * wb) / total, (a.G * wa + b.G * wb) / total, (a.B * wa + b.B * wb) / total);
        }
    }
}
