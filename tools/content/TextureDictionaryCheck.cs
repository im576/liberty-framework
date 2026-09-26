using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // `wtdcheck`: measures TextureDictionaryWriter against the game's own dictionaries (read only). For every dictionary it
    // checks the rules the writer relies on (name hash, hash order, row stride, identical opaque record bytes, mip count)
    // and rebuilds it from its own textures with a prototype captured from itself, then compares field by field. Only
    // placement (pointers, page flags, data offsets) may differ. Dictionaries with uncompressed textures are listed, not
    // rebuilt (the writer writes DXT only).
    internal static class TextureDictionaryCheck
    {
        // Texture record bytes that hold pointers (their values depend on placement, not on the texture).
        private static readonly int[] PointerRecordOffsets = { 0x14, 0x15, 0x16, 0x17, 0x40, 0x41, 0x42, 0x43, 0x48, 0x49, 0x4A, 0x4B };

        internal sealed class Totals
        {
            internal int Files, Rebuilt, Identical, SkippedUncompressed, Failed, Textures, HashOk, OrderOk, StrideOk, PrototypeBuiltin, FullChain, SingleLevel, OtherLevels;
            internal readonly List<string> Problems = new List<string>();
        }

        internal static int Run(string[] args)
        {
            string game = null;
            List<string> inputs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--game" && i + 1 < args.Length) { game = args[++i]; } else { inputs.Add(args[i]); }
            }
            Totals totals = new Totals();
            TextureDictionaryPrototype builtin = TextureDictionaryPrototype.Builtin();
            foreach (string input in inputs)
            {
                if (Directory.Exists(input))
                {
                    foreach (string path in Directory.GetFiles(input, "*.wtd").OrderBy(p => p, StringComparer.OrdinalIgnoreCase)) { CheckOne(Path.GetFileName(path), File.ReadAllBytes(path), builtin, totals); }
                }
                else if (input.EndsWith(".img", StringComparison.OrdinalIgnoreCase) || input.EndsWith(".rpf", StringComparison.OrdinalIgnoreCase))
                {
                    if (game == null) { throw new ArgumentException("archives need --game <GTA IV folder> (for the archive key)"); }
                    ArchiveSource archive = ArchiveSource.Open(game, input);
                    foreach (string name in archive.Names.Where(n => n.EndsWith(".wtd", StringComparison.OrdinalIgnoreCase)).OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                    {
                        CheckOne(Path.GetFileName(input) + "/" + name, archive.Extract(name), builtin, totals);
                    }
                }
                else { CheckOne(Path.GetFileName(input), File.ReadAllBytes(input), builtin, totals); }
            }
            Console.WriteLine("wtdcheck files=" + totals.Files + " rebuilt=" + totals.Rebuilt + " identical=" + totals.Identical + " skipped(uncompressed)=" + totals.SkippedUncompressed + " failed=" + totals.Failed);
            Console.WriteLine("  textures=" + totals.Textures + " nameHash=" + totals.HashOk + " hashOrder(files)=" + totals.OrderOk + " rowStride=" + totals.StrideOk +
                " prototype==builtin(files)=" + totals.PrototypeBuiltin);
            Console.WriteLine("  mip levels: fullChain(to 4px)=" + totals.FullChain + " single=" + totals.SingleLevel + " other=" + totals.OtherLevels);
            foreach (string problem in totals.Problems) { Console.WriteLine("  " + problem); }
            return totals.Failed == 0 && totals.Rebuilt == totals.Identical ? 0 : 1;
        }

        private static void CheckOne(string label, byte[] file, TextureDictionaryPrototype builtin, Totals totals)
        {
            totals.Files++;
            try
            {
                RscResource original = RscResource.Parse(file, true);
                TextureDictionary parsed = TextureDictionary.Parse(original, false);
                byte[] body = original.Body;
                int hashes = SystemOffset(BitConverter.ToUInt32(body, 0x10));
                int array = SystemOffset(BitConverter.ToUInt32(body, 0x18));
                bool ordered = true;
                byte[] firstRecord = null;
                bool uncompressed = false;
                for (int i = 0; i < parsed.Textures.Count; i++)
                {
                    TextureDictionary.Texture texture = parsed.Textures[i];
                    totals.Textures++;
                    int record = SystemOffset(BitConverter.ToUInt32(body, array + i * 4));
                    uint stored = BitConverter.ToUInt32(body, hashes + i * 4);
                    if (stored == TextureNameHash.Compute(texture.Name)) { totals.HashOk++; } else { totals.Problems.Add(label + ": " + texture.Name + " hash 0x" + stored.ToString("X8") + " != 0x" + TextureNameHash.Compute(texture.Name).ToString("X8")); }
                    if (i > 0 && BitConverter.ToUInt32(body, hashes + (i - 1) * 4) >= stored) { ordered = false; }
                    if (!texture.Compressed) { uncompressed = true; continue; }
                    int stride = BitConverter.ToUInt16(body, record + 0x24);
                    if (stride == TextureDictionaryWriter.RowStrideBytes(texture.Format, texture.Width)) { totals.StrideOk++; }
                    else { totals.Problems.Add(label + ": " + texture.Name + " " + texture.Width + " " + texture.Format + " stride " + stride + " != " + TextureDictionaryWriter.RowStrideBytes(texture.Format, texture.Width)); }
                    int full = TextureEncoder.FullChainLevels(texture.Width, texture.Height);
                    if (texture.Levels == full && full > 1) { totals.FullChain++; } else if (texture.Levels == 1) { totals.SingleLevel++; }
                    else { totals.OtherLevels++; totals.Problems.Add(label + ": " + texture.Name + " " + texture.Width + "x" + texture.Height + " has " + texture.Levels + " levels (full chain " + full + ")"); }
                    byte[] bytes = Slice(body, record, TextureDictionaryPrototype.TextureRecordBytes);
                    if (firstRecord == null) { firstRecord = bytes; }
                    else if (TextureDictionaryWriter.OpaqueRecordOffsets.Any(at => bytes[at] != firstRecord[at])) { totals.Problems.Add(label + ": " + texture.Name + " opaque record bytes differ from the first texture's"); }
                }
                if (ordered) { totals.OrderOk++; } else { totals.Problems.Add(label + ": hash array not ascending"); }
                if (uncompressed) { totals.SkippedUncompressed++; return; }

                TextureDictionaryPrototype prototype = TextureDictionaryPrototype.FromResource(original, label);
                List<string> versusBuiltin = prototype.OpaqueDifferences(builtin);
                if (versusBuiltin.Count == 0) { totals.PrototypeBuiltin++; } else { totals.Problems.Add(label + ": prototype differs from builtin at " + string.Join(", ", versusBuiltin.ToArray())); }

                List<NativeTexture> sources = new List<NativeTexture>();
                foreach (TextureDictionary.Texture texture in parsed.Textures)
                {
                    NativeTexture native = new NativeTexture { Name = texture.Name, Format = texture.Format, Width = texture.Width, Height = texture.Height };
                    int at = texture.DataOffset;
                    for (int level = 0; level < Math.Max(1, texture.Levels); level++) { native.Levels.Add(Slice(body, at, texture.LevelBytes(level))); at += texture.LevelBytes(level); }
                    sources.Add(native);
                }
                RscResource rebuilt = RscResource.Parse(TextureDictionaryWriter.Write(sources, prototype).Resource.Serialize());
                totals.Rebuilt++;
                List<string> differences = Compare(original, rebuilt);
                if (differences.Count == 0) { totals.Identical++; }
                else { totals.Problems.Add(label + ": rebuilt differs: " + string.Join("; ", differences.Take(6).ToArray()) + (differences.Count > 6 ? " (+" + (differences.Count - 6) + ")" : "")); }
            }
            catch (Exception error)
            {
                totals.Failed++;
                totals.Problems.Add(label + ": " + error.GetType().Name + " " + error.Message);
            }
        }

        // Everything but placement: header (opaque bytes, counts), block map, hash array, texture order, each texture's
        // record without its pointers, its stored name and every mip level's bytes.
        internal static List<string> Compare(RscResource original, RscResource rebuilt)
        {
            List<string> differences = new List<string>();
            byte[] a = original.Body, b = rebuilt.Body;
            if (original.Type != rebuilt.Type) { differences.Add("resource type"); }
            if ((original.Flags & TextureDictionaryPrototype.UnknownFlagMask) != (rebuilt.Flags & TextureDictionaryPrototype.UnknownFlagMask)) { differences.Add("flag bits 30-31"); }
            foreach (int at in TextureDictionaryWriter.OpaqueHeaderOffsets.Concat(new[] { 0x04, 0x05, 0x06, 0x07, 0x14, 0x15, 0x16, 0x17, 0x1C, 0x1D, 0x1E, 0x1F }))
            {
                if (a[at] != b[at]) { differences.Add("header+0x" + at.ToString("X2")); }
            }
            for (int at = TextureDictionaryPrototype.BlockMapOffset; at < TextureDictionaryWriter.FirstRecordOffset; at++) { if (a[at] != b[at]) { differences.Add("block map+0x" + at.ToString("X3")); break; } }
            int count = BitConverter.ToUInt16(a, 0x1C);
            if (count != BitConverter.ToUInt16(b, 0x1C)) { return differences; }
            int hashesA = SystemOffset(BitConverter.ToUInt32(a, 0x10)), hashesB = SystemOffset(BitConverter.ToUInt32(b, 0x10));
            int arrayA = SystemOffset(BitConverter.ToUInt32(a, 0x18)), arrayB = SystemOffset(BitConverter.ToUInt32(b, 0x18));
            TextureDictionary parsedA = TextureDictionary.Parse(original), parsedB = TextureDictionary.Parse(rebuilt);
            for (int i = 0; i < count; i++)
            {
                if (BitConverter.ToUInt32(a, hashesA + i * 4) != BitConverter.ToUInt32(b, hashesB + i * 4)) { differences.Add("hash[" + i + "]"); }
                TextureDictionary.Texture ta = parsedA.Textures[i], tb = parsedB.Textures[i];
                if (ta.Name != tb.Name) { differences.Add("order[" + i + "] " + ta.Name + " vs " + tb.Name); continue; }
                int recordA = SystemOffset(BitConverter.ToUInt32(a, arrayA + i * 4)), recordB = SystemOffset(BitConverter.ToUInt32(b, arrayB + i * 4));
                for (int at = 0; at < TextureDictionaryPrototype.TextureRecordBytes; at++)
                {
                    if (Array.IndexOf(PointerRecordOffsets, at) >= 0) { continue; }
                    if (a[recordA + at] != b[recordB + at]) { differences.Add(ta.Name + " record+0x" + at.ToString("X2")); }
                }
                string nameA = CString(a, SystemOffset(BitConverter.ToUInt32(a, recordA + 0x14))), nameB = CString(b, SystemOffset(BitConverter.ToUInt32(b, recordB + 0x14)));
                if (nameA != nameB) { differences.Add(ta.Name + " stored name '" + nameA + "' vs '" + nameB + "'"); }
                int bytes = 0;
                for (int level = 0; level < Math.Max(1, ta.Levels); level++) { bytes += ta.LevelBytes(level); }
                for (int k = 0; k < bytes; k++) { if (a[ta.DataOffset + k] != b[tb.DataOffset + k]) { differences.Add(ta.Name + " pixel data"); break; } }
            }
            return differences;
        }

        private static string CString(byte[] body, int offset)
        {
            int end = Array.IndexOf(body, (byte)0, offset);
            return Encoding.ASCII.GetString(body, offset, end - offset);
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
    }
}
