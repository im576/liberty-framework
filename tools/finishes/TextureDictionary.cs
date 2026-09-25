using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibertyFramework.Finishes
{
    // GTA IV pgDictionary<grcTexturePC> inside a .wtd resource body. Offsets verified against
    // w_glock.wtd from this installation (see docs/game-api/MEMORY.md, finish pipeline).
    internal sealed class TextureDictionary
    {
        internal sealed class Texture
        {
            internal string Name;
            internal int Width;
            internal int Height;
            internal string Format;
            internal int Levels;
            internal int DataOffset;

            internal int BlockBytes { get { return Format == "DXT1" ? 8 : 16; } }
            internal bool Compressed { get { return Format == "DXT1" || Format == "DXT3" || Format == "DXT5"; } }
            internal int PixelBytes { get { return Format == "L8" ? 1 : 4; } }

            internal int LevelBytes(int level)
            {
                int width = Math.Max(1, Width >> level);
                int height = Math.Max(1, Height >> level);
                if (!Compressed) { return width * height * PixelBytes; }
                return Math.Max(1, (width + 3) / 4) * Math.Max(1, (height + 3) / 4) * BlockBytes;
            }
        }

        internal readonly List<Texture> Textures = new List<Texture>();

        internal static TextureDictionary Parse(RscResource resource) { return Parse(resource, true); }

        // lenient: uncompressed D3D formats (A8R8G8B8 = 21, X8R8G8B8 = 22, L8 = 50) are listed instead of rejected
        // (the finish pipeline stays strict; the UI icon extractor reads HD weapon packs that use them).
        internal static TextureDictionary Parse(RscResource resource, bool strict)
        {
            byte[] body = resource.Body;
            TextureDictionary dictionary = new TextureDictionary();
            uint texturesPointer = BitConverter.ToUInt32(body, 0x18);
            int count = BitConverter.ToUInt16(body, 0x1C);
            for (int index = 0; index < count; index++)
            {
                int entry = SystemOffset(BitConverter.ToUInt32(body, SystemOffset(texturesPointer) + index * 4));
                Texture texture = new Texture();
                texture.Name = ReadName(body, SystemOffset(BitConverter.ToUInt32(body, entry + 0x14)));
                texture.Width = BitConverter.ToUInt16(body, entry + 0x1C);
                texture.Height = BitConverter.ToUInt16(body, entry + 0x1E);
                texture.Format = Encoding.ASCII.GetString(body, entry + 0x20, 4);
                texture.Levels = body[entry + 0x27];
                uint data = BitConverter.ToUInt32(body, entry + 0x48);
                if ((data >> 28) != 6) { throw new InvalidDataException(texture.Name + " pixel pointer is not in the graphics segment"); }
                texture.DataOffset = resource.SystemSize + (int)(data & 0x0FFFFFFF);
                if (!strict && !texture.Compressed)
                {
                    uint code = BitConverter.ToUInt32(body, entry + 0x20);
                    texture.Format = code == 21 ? "A8R8G8B8" : code == 22 ? "X8R8G8B8" : code == 50 ? "L8" : "D3DFMT" + code;
                }
                else if (!texture.Compressed)
                {
                    throw new InvalidDataException(texture.Name + " format " + texture.Format + " unsupported");
                }
                dictionary.Textures.Add(texture);
            }
            return dictionary;
        }

        private static int SystemOffset(uint pointer)
        {
            if ((pointer >> 28) != 5) { throw new InvalidDataException("Expected system pointer, got 0x" + pointer.ToString("X8")); }
            return (int)(pointer & 0x0FFFFFFF);
        }

        // Names are stored as "pack:/cj_glock.dds".
        private static string ReadName(byte[] body, int offset)
        {
            int end = Array.IndexOf(body, (byte)0, offset);
            string name = Encoding.ASCII.GetString(body, offset, end - offset);
            if (name.StartsWith("pack:/")) { name = name.Substring(6); }
            if (name.EndsWith(".dds")) { name = name.Substring(0, name.Length - 4); }
            return name;
        }
    }
}
