using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Writes a texture dictionary (.wtd, RSC05 type 8) from scratch: any number of DXT textures, any size, full mip chains.
    // Layout (docs/research/ModelFormat.md, texture dictionary):
    //   system segment   +0x000 dictionary header (+0x04 block map, +0x10 hashes, +0x18 textures, u16 count/capacity each)
    //                    +0x020 block map (copied from the prototype)
    //                    +0x230 texture records, 0x50 bytes each, then the names ("pack:/<name>.dds"), the hash array and
    //                    the texture pointer array; the arrays are sorted by TextureNameHash; unused bytes are 0xCD
    //   graphics segment every texture's mip levels back to back, largest level first
    // The fields TextureDictionary.Parse reads are written from the textures; the hash array and the row stride (+0x24) are
    // derived from rules matched against the game's own dictionaries (checked by `LibertyContent wtdcheck`); every other
    // byte comes from a TextureDictionaryPrototype.
    //
    // Page placement: a graphics segment that fits one page (at most 8 MB) is written as ONE page, as generated drawables
    // are (a multi-page drawable rendered garbage in game, docs/research/ModelFormat.md). Larger dictionaries use 8 MB
    // pages and never let a texture straddle a page. Both are unverified in game for textures (NEEDS-PLAYTEST).
    internal static class TextureDictionaryWriter
    {
        internal const uint SystemBase = 0x50000000;
        internal const uint GraphicsBase = 0x60000000;
        internal const int FirstRecordOffset = TextureDictionaryPrototype.BlockMapOffset + TextureDictionaryPrototype.BlockMapBytes;
        internal const int StructureAlignmentBytes = 16;
        internal const int SystemSizeAlignmentBytes = 0x1000;
        // Texture data starts on a 256-byte boundary. The game's files place textures on 128-byte boundaries at least
        // (coronas.wtd puts imp_car at 0x5580); whether the game needs any alignment is open, 256 covers 128.
        internal const int TextureDataAlignmentBytes = 256;
        internal const int MaxPageShift = 15;
        internal const int MaxPageBytes = 256 << MaxPageShift;
        internal const int MaxNameLength = 63;
        internal const byte FillByte = 0xCD;

        // Header and texture-record bytes the writer copies from the prototype (every other byte it computes).
        internal static readonly int[] OpaqueHeaderOffsets = Enumerable.Range(0x00, 4).Concat(Enumerable.Range(0x08, 8)).ToArray();
        internal static readonly int[] OpaqueRecordOffsets = Enumerable.Range(0, TextureDictionaryPrototype.TextureRecordBytes)
            .Where(at => !(at >= 0x14 && at < 0x18) && !(at >= 0x1C && at < 0x26) && at != 0x27 && !(at >= 0x40 && at < 0x44) && !(at >= 0x48 && at < 0x4C)).ToArray();

        internal sealed class Placement
        {
            internal NativeTexture Texture;
            internal uint Hash;
            internal int RecordOffset;
            internal int NameOffset;
            internal int DataOffset; // within the graphics segment
        }

        internal sealed class Output
        {
            internal RscResource Resource;
            internal readonly List<Placement> Textures = new List<Placement>(); // in hash order
            internal int PageBytes;
            internal int PageCount;
        }

        internal static Output Write(IList<NativeTexture> textures, TextureDictionaryPrototype prototype)
        {
            if (prototype == null) { throw new ArgumentNullException("prototype"); }
            if (textures == null || textures.Count == 0) { throw new ArgumentException("a texture dictionary needs at least one texture"); }
            if (textures.Count > 0xFFFF) { throw new ArgumentException(textures.Count + " textures exceed the u16 count"); }
            Output output = new Output();
            foreach (NativeTexture texture in textures) { Check(texture); output.Textures.Add(new Placement { Texture = texture, Hash = TextureNameHash.Compute(texture.Name) }); }
            foreach (IGrouping<uint, Placement> same in output.Textures.GroupBy(p => p.Hash).Where(g => g.Count() > 1))
            {
                throw new ArgumentException("textures " + string.Join(", ", same.Select(p => p.Texture.Name).ToArray()) + " share the name hash 0x" + same.Key.ToString("X8") + " (duplicate names?)");
            }
            output.Textures.Sort((a, b) => a.Hash.CompareTo(b.Hash));

            // System segment layout.
            int cursor = FirstRecordOffset;
            foreach (Placement p in output.Textures) { p.RecordOffset = cursor; cursor += TextureDictionaryPrototype.TextureRecordBytes; }
            foreach (Placement p in output.Textures)
            {
                cursor = Align(cursor, StructureAlignmentBytes);
                p.NameOffset = cursor;
                cursor += StoredName(p.Texture.Name).Length + 1;
            }
            int hashesOffset = Align(cursor, StructureAlignmentBytes);
            int pointersOffset = Align(hashesOffset + output.Textures.Count * 4, StructureAlignmentBytes);
            int systemUsed = pointersOffset + output.Textures.Count * 4;
            int systemSize = Align(systemUsed, SystemSizeAlignmentBytes);
            if (systemSize / 256 > 0x7FF) { throw new InvalidDataException("system segment of " + systemSize + " bytes exceeds the 256-byte page count"); }

            // Graphics segment layout.
            int shift;
            int graphicsUsed = PlaceData(output, out shift);
            int graphicsSize;
            uint flags = DrawableBuilder.EncodeGraphics(prototype.FlagBits | (uint)(systemSize / 256), graphicsUsed, shift, out graphicsSize);
            output.PageBytes = 256 << shift;
            output.PageCount = graphicsSize / output.PageBytes;

            byte[] body = new byte[systemSize + graphicsSize];
            for (int i = 0; i < systemSize; i++) { body[i] = FillByte; }
            Buffer.BlockCopy(prototype.Header, 0, body, 0, TextureDictionaryPrototype.HeaderBytes);
            Buffer.BlockCopy(prototype.BlockMap, 0, body, TextureDictionaryPrototype.BlockMapOffset, TextureDictionaryPrototype.BlockMapBytes);
            PutU32(body, 0x04, SystemBase + TextureDictionaryPrototype.BlockMapOffset);
            PutU32(body, 0x10, SystemBase + (uint)hashesOffset);
            PutU16(body, 0x14, output.Textures.Count); PutU16(body, 0x16, output.Textures.Count);
            PutU32(body, 0x18, SystemBase + (uint)pointersOffset);
            PutU16(body, 0x1C, output.Textures.Count); PutU16(body, 0x1E, output.Textures.Count);
            for (int i = 0; i < output.Textures.Count; i++)
            {
                Placement p = output.Textures[i];
                NativeTexture t = p.Texture;
                int record = p.RecordOffset;
                Buffer.BlockCopy(prototype.TextureRecord, 0, body, record, TextureDictionaryPrototype.TextureRecordBytes);
                PutU32(body, record + 0x14, SystemBase + (uint)p.NameOffset);
                PutU16(body, record + 0x1C, t.Width);
                PutU16(body, record + 0x1E, t.Height);
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(t.Format), 0, body, record + 0x20, 4);
                PutU16(body, record + 0x24, RowStrideBytes(t.Format, t.Width));
                body[record + 0x27] = (byte)t.Levels.Count;
                PutU32(body, record + 0x40, 0);
                PutU32(body, record + 0x48, GraphicsBase + (uint)p.DataOffset);
                byte[] name = Encoding.ASCII.GetBytes(StoredName(t.Name) + "\0");
                Buffer.BlockCopy(name, 0, body, p.NameOffset, name.Length);
                PutU32(body, hashesOffset + i * 4, p.Hash);
                PutU32(body, pointersOffset + i * 4, SystemBase + (uint)record);
                int at = systemSize + p.DataOffset;
                foreach (byte[] level in t.Levels) { Buffer.BlockCopy(level, 0, body, at, level.Length); at += level.Length; }
            }
            output.Resource = new RscResource { Type = prototype.ResourceType, Flags = flags, Body = body };
            return output;
        }

        // Bytes per pixel row of the top level as the game's files store it at +0x24: 128 for a 256-wide DXT1, 256 for a
        // 256-wide DXT5 (a block row divided by its 4 pixel rows).
        internal static int RowStrideBytes(string format, int width)
        {
            return Math.Max(1, (width + 3) / 4) * DxtDecoder.BlockBytes(format) / 4;
        }

        internal static string StoredName(string name) { return "pack:/" + name + ".dds"; }

        // Largest texture first, each on a TextureDataAlignmentBytes boundary. Returns the bytes used and the page shift.
        private static int PlaceData(Output output, out int shift)
        {
            List<Placement> bySize = output.Textures.OrderByDescending(p => p.Texture.TotalBytes).ThenBy(p => p.Hash).ToList();
            int packed = 0;
            foreach (Placement p in bySize) { packed = Align(packed, TextureDataAlignmentBytes) + p.Texture.TotalBytes; }
            if (packed <= MaxPageBytes)
            {
                shift = 0;
                while ((256 << shift) < Math.Max(packed, SystemSizeAlignmentBytes)) { shift++; }
                int cursor = 0;
                foreach (Placement p in bySize) { cursor = Align(cursor, TextureDataAlignmentBytes); p.DataOffset = cursor; cursor += p.Texture.TotalBytes; }
                return cursor;
            }
            shift = MaxPageShift;
            int used = 0;
            foreach (Placement p in bySize)
            {
                int size = p.Texture.TotalBytes;
                if (size > MaxPageBytes) { throw new InvalidDataException("texture " + p.Texture.Name + " (" + size + " bytes) is larger than one " + MaxPageBytes + "-byte page"); }
                int start = Align(used, TextureDataAlignmentBytes);
                if (start / MaxPageBytes != (start + size - 1) / MaxPageBytes) { start = Align(start, MaxPageBytes); }
                p.DataOffset = start;
                used = start + size;
            }
            return used;
        }

        private static void Check(NativeTexture texture)
        {
            string name = texture.Name ?? "";
            if (name.Length == 0 || name.Length > MaxNameLength || name.Any(c => c < 0x21 || c > 0x7E || c == '/' || c == '\\' || c == ':'))
            {
                throw new ArgumentException("texture name '" + name + "' must be 1-" + MaxNameLength + " printable ASCII characters without / \\ :");
            }
            if (texture.Format != "DXT1" && texture.Format != "DXT3" && texture.Format != "DXT5") { throw new ArgumentException("texture " + name + ": format " + texture.Format + " is not written (DXT1, DXT3, DXT5)"); }
            if (texture.Width < 1 || texture.Height < 1 || texture.Width > 0xFFFF || texture.Height > 0xFFFF) { throw new ArgumentException("texture " + name + ": size " + texture.Width + "x" + texture.Height); }
            if (texture.Levels.Count < 1 || texture.Levels.Count > 0xFF) { throw new ArgumentException("texture " + name + ": " + texture.Levels.Count + " mip levels"); }
            for (int level = 0; level < texture.Levels.Count; level++)
            {
                int expected = DxtDecoder.LevelBytes(texture.Format, Math.Max(1, texture.Width >> level), Math.Max(1, texture.Height >> level));
                if (texture.Levels[level] == null || texture.Levels[level].Length != expected)
                {
                    throw new ArgumentException("texture " + name + " level " + level + " has " + (texture.Levels[level] == null ? 0 : texture.Levels[level].Length) + " bytes, expected " + expected);
                }
            }
        }

        private static int Align(int value, int alignment) { return (value + alignment - 1) / alignment * alignment; }
        private static void PutU32(byte[] b, int at, uint value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, at, 4); }
        private static void PutU16(byte[] b, int at, int value) { b[at] = (byte)value; b[at + 1] = (byte)(value >> 8); }
    }
}
