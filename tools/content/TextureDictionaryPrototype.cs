using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // The bytes of a texture dictionary that TextureDictionaryWriter does not compute: fields whose meaning the repository's
    // parsers do not establish are copied verbatim from a game dictionary instead of being invented. The writer overwrites
    // only the fields listed in docs/research/ModelFormat.md (texture dictionary), everything else comes from here.
    //
    //   Header        0x20 bytes at +0x00. Opaque: +0x00 (vtable slot; the game's files store different values, e.g. 0x00695384
    //                 in Rockstar's and another in an OpenIV-made weapon, so it is fixed up at load), +0x08, +0x0C.
    //   BlockMap      0x210 bytes at +0x20, which the header's +0x04 points to (a zero word and 0xCD fill in every file read).
    //   TextureRecord 0x50 bytes, one per texture. Opaque: +0x00 (vtable slot), +0x04..+0x13, +0x18, +0x26, +0x28..+0x3F,
    //                 +0x44, +0x4C.
    //   FlagBits      RSC flag bits 30-31 (set in every file read; meaning unknown).
    internal sealed class TextureDictionaryPrototype
    {
        internal const int HeaderBytes = 0x20;
        internal const int BlockMapOffset = 0x20;
        internal const int BlockMapBytes = 0x210;
        internal const int TextureRecordBytes = 0x50;
        internal const uint UnknownFlagMask = 0xC0000000;
        internal const uint TextureDictionaryResourceType = 8;

        internal string Source;
        internal uint ResourceType;
        internal uint FlagBits;
        internal byte[] Header;
        internal byte[] BlockMap;
        internal byte[] TextureRecord;

        // The bytes of amb_nailgun.wtd and coronas.wtd (Rockstar-built dictionaries: identical in every copied byte). The game
        // loads both. Offline builds (selftest) use this; the prop compiler captures the template's own dictionary instead.
        internal static TextureDictionaryPrototype Builtin()
        {
            TextureDictionaryPrototype prototype = new TextureDictionaryPrototype();
            prototype.Source = "builtin (amb_nailgun.wtd / coronas.wtd)";
            prototype.ResourceType = TextureDictionaryResourceType;
            prototype.FlagBits = 0xC0000000;
            prototype.Header = new byte[HeaderBytes];
            PutU32(prototype.Header, 0x00, 0x00695384);
            PutU32(prototype.Header, 0x0C, 1);
            prototype.BlockMap = new byte[BlockMapBytes];
            for (int i = 4; i < BlockMapBytes; i++) { prototype.BlockMap[i] = 0xCD; }
            prototype.TextureRecord = new byte[TextureRecordBytes];
            PutU32(prototype.TextureRecord, 0x00, 0x006B1D94);
            PutU32(prototype.TextureRecord, 0x08, 0x00010000);
            for (int i = 0; i < 3; i++) { PutU32(prototype.TextureRecord, 0x28 + i * 4, 0x3F800000); }
            return prototype;
        }

        // Captures the opaque bytes of a game dictionary. Refuses one whose layout differs from what the writer reproduces
        // (block map not at +0x20, or anything else stored inside the block map's range).
        internal static TextureDictionaryPrototype FromResource(RscResource resource, string source)
        {
            byte[] body = resource.Body;
            if (resource.SystemSize < BlockMapOffset + BlockMapBytes) { throw new InvalidDataException(source + ": system segment too small for a texture dictionary prototype"); }
            if (BitConverter.ToUInt32(body, 0x04) != TextureDictionaryWriter.SystemBase + BlockMapOffset) { throw new InvalidDataException(source + ": block map pointer is not +0x20"); }
            TextureDictionary parsed = TextureDictionary.Parse(resource, false);
            if (parsed.Textures.Count == 0) { throw new InvalidDataException(source + ": dictionary has no textures"); }
            List<int> structures = new List<int> { SystemOffset(BitConverter.ToUInt32(body, 0x10)), SystemOffset(BitConverter.ToUInt32(body, 0x18)) };
            int array = structures[1];
            for (int i = 0; i < parsed.Textures.Count; i++)
            {
                int record = SystemOffset(BitConverter.ToUInt32(body, array + i * 4));
                structures.Add(record);
                structures.Add(SystemOffset(BitConverter.ToUInt32(body, record + 0x14)));
            }
            if (structures.Any(s => s < BlockMapOffset + BlockMapBytes)) { throw new InvalidDataException(source + ": dictionary data inside the block map range"); }
            TextureDictionaryPrototype prototype = new TextureDictionaryPrototype();
            prototype.Source = source;
            prototype.ResourceType = resource.Type;
            prototype.FlagBits = resource.Flags & UnknownFlagMask;
            prototype.Header = Slice(body, 0, HeaderBytes);
            prototype.BlockMap = Slice(body, BlockMapOffset, BlockMapBytes);
            prototype.TextureRecord = Slice(body, SystemOffset(BitConverter.ToUInt32(body, array)), TextureRecordBytes);
            return prototype;
        }

        // Differences in the opaque bytes (the ones the writer copies), as "<part>+0x<offset>" entries.
        internal List<string> OpaqueDifferences(TextureDictionaryPrototype other)
        {
            List<string> differences = new List<string>();
            if (ResourceType != other.ResourceType) { differences.Add("resource type"); }
            if (FlagBits != other.FlagBits) { differences.Add("flag bits 30-31"); }
            foreach (int at in TextureDictionaryWriter.OpaqueHeaderOffsets) { if (Header[at] != other.Header[at]) { differences.Add("header+0x" + at.ToString("X2")); } }
            for (int at = 0; at < BlockMapBytes; at++) { if (BlockMap[at] != other.BlockMap[at]) { differences.Add("blockmap+0x" + at.ToString("X3")); break; } }
            foreach (int at in TextureDictionaryWriter.OpaqueRecordOffsets) { if (TextureRecord[at] != other.TextureRecord[at]) { differences.Add("texture+0x" + at.ToString("X2")); } }
            return differences;
        }

        private static int SystemOffset(uint pointer)
        {
            if ((pointer & 0xF0000000) != TextureDictionaryWriter.SystemBase) { throw new InvalidDataException("expected a system pointer, got 0x" + pointer.ToString("X8")); }
            return (int)(pointer & 0x0FFFFFFF);
        }

        private static byte[] Slice(byte[] source, int offset, int length)
        {
            byte[] copy = new byte[length];
            Buffer.BlockCopy(source, offset, copy, 0, length);
            return copy;
        }

        private static void PutU32(byte[] target, int at, uint value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, target, at, 4); }
    }
}
