using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Read-only measurements of four pointers in the common CE WBD target class.
    // No labels for the pointed data or assumptions about shape semantics.
    internal static class WbdNestedProbe
    {
        private static readonly int[] Fields = { 0x8C, 0xB0, 0xD0, 0xE0 };
        private static readonly int[] Strides = { 4, 8, 12, 16, 24, 32, 48, 64 };

        private sealed class Stats
        {
            internal int Samples;
            internal readonly Dictionary<string, int> NextGap = new Dictionary<string, int>();
            internal readonly Dictionary<string, int> FirstWord = new Dictionary<string, int>();
            internal readonly Dictionary<string, int> AdjacentCounts = new Dictionary<string, int>();
            internal readonly Dictionary<string, int> PointerRuns = new Dictionary<string, int>();
            internal readonly List<object> Examples = new List<object>();
            internal void Add(byte[] body, int systemSize, int graphicsSize, int[] pointerTargets, int parent, int field,
                string archive, string file, string model)
            {
                uint raw = BitConverter.ToUInt32(body, parent + field);
                if ((raw & 0xF0000000) != 0x50000000) { throw new InvalidDataException("nested field is not system pointer"); }
                int start = (int)(raw - 0x50000000);
                if (start < 0 || start + 4 > systemSize) { throw new InvalidDataException("nested field out of range"); }
                Samples++;
                // Bound a candidate array by the next system pointer target found in the resource.
                // This is an upper bound only: unreferenced allocations may intervene.
                int index = Array.BinarySearch(pointerTargets, start);
                if (index < 0) { index = ~index; }
                while (index < pointerTargets.Length && pointerTargets[index] <= start) { index++; }
                int next = index < pointerTargets.Length ? pointerTargets[index] : systemSize;
                int gap = next - start;
                Count(NextGap, gap < 0x20 ? "<0x20" : gap < 0x80 ? "0x20-0x7F" :
                    gap < 0x200 ? "0x80-0x1FF" : ">=0x200");
                uint first = BitConverter.ToUInt32(body, start);
                Count(FirstWord, BoundsLayout.Classify(first, systemSize, graphicsSize).ToString());
                // Record nearby small integers as candidates only. These are not proven counts.
                foreach (int delta in new[] { -8, -4, 4, 8 })
                {
                    int at = parent + field + delta;
                    if (at < 0 || at + 4 > systemSize) { continue; }
                    uint value = BitConverter.ToUInt32(body, at);
                    if (value <= 4096) { Count(AdjacentCounts, delta + ":" + value); }
                }
                // Count initial system-pointer words for each possible stride, capped at 16 records.
                // This measures regularity; it does not identify an array or its record count.
                foreach (int stride in Strides)
                {
                    int run = 0;
                    for (int at = start; at + 4 <= next && run < 16; at += stride)
                    {
                        uint value = BitConverter.ToUInt32(body, at);
                        if ((value & 0xF0000000) != 0x50000000 || value - 0x50000000 >= systemSize) { break; }
                        run++;
                    }
                    Count(PointerRuns, stride + ":" + run);
                }
                if (Examples.Count < 12) { Examples.Add(new { archive, file, model,
                    parent = "0x" + parent.ToString("X"), field = "0x" + field.ToString("X"),
                    target = "0x" + start.ToString("X"), nextKnownPointerGap = "0x" + gap.ToString("X") }); }
            }
            internal object Report() { return new { samples = Samples, nextKnownPointerGap = NextGap,
                firstWordClasses = FirstWord, adjacentSmallIntegers = AdjacentCounts,
                initialSystemPointerRuns = PointerRuns, examples = Examples }; }
        }

        internal static int Run(string[] args)
        {
            int gameAt = Array.IndexOf(args, "--game"), outAt = Array.IndexOf(args, "--out");
            if (gameAt < 0 || outAt < 0 || gameAt + 1 >= args.Length || outAt + 1 >= args.Length)
            { Console.WriteLine("usage: wbdnested --game <dir> --out <json>"); return 2; }
            string game = args[gameAt + 1], output = args[outAt + 1];
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            Dictionary<int, Stats> stats = Fields.ToDictionary(f => f, f => new Stats());
            List<string> skipped = new List<string>(), errors = new List<string>();
            int resources = 0, entries = 0;
            foreach (string relative in Probe.Archives(game, skipped))
            {
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    Dictionary<uint, string> names = new Dictionary<uint, string>();
                    foreach (ImgArchive.Entry e in archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)))
                    { names[TextureNameHash.Compute(Path.GetFileNameWithoutExtension(e.Name))] = Path.GetFileNameWithoutExtension(e.Name); }
                    foreach (ImgArchive.Entry e in archive.Entries.Where(e => e.Name.EndsWith(".wbd", StringComparison.OrdinalIgnoreCase)))
                    {
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(e.Name), true);
                            if (resource.Type != 32 || BitConverter.ToUInt32(resource.Body, 0) != WbdDictionaryReader.CeVtable) { continue; }
                            WbdDictionaryReader reader = WbdDictionaryReader.Parse(resource);
                            resources++;
                            List<int> targets = new List<int>();
                            for (int at = 0; at + 4 <= resource.SystemSize; at += 4)
                            {
                                uint word = BitConverter.ToUInt32(resource.Body, at);
                                if ((word & 0xF0000000) == 0x50000000 && word - 0x50000000 < resource.SystemSize)
                                { targets.Add((int)(word - 0x50000000)); }
                            }
                            int[] pointerTargets = targets.Distinct().OrderBy(x => x).ToArray();
                            foreach (WbdDictionaryReader.Entry item in reader.Entries.Where(x => x.TargetVtable == 0x0069C19C))
                            {
                                int parent = (int)(item.TargetPointer - 0x50000000);
                                if (parent + Fields.Max() + 4 > resource.SystemSize) { throw new InvalidDataException("common target prefix truncated"); }
                                string model;
                                if (!names.TryGetValue(item.NameHash, out model)) { model = "<no same-IMG WDR match>"; }
                                foreach (int field in Fields) { stats[field].Add(resource.Body, resource.SystemSize,
                                    resource.GraphicsSize, pointerTargets, parent, field, relative, e.Name, model); }
                                entries++;
                            }
                        }
                        catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + "/" + e.Name + ": " + error.Message); } }
                    }
                }
                catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + ": " + error.Message); } }
            }
            object report = new { question = "What regularities occur behind four common CE WBD target pointers?",
                rule = "read-only structure measurements; no shape labels, resource bytes or float values",
                resources, entries, fields = stats.ToDictionary(p => "0x" + p.Key.ToString("X"), p => p.Value.Report()),
                errors, skipped };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllText(output, new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
            Console.WriteLine("wbdnested resources=" + resources + " entries=" + entries + " errors=" + errors.Count);
            return entries > 0 ? 0 : 1;
        }

        private static void Count(Dictionary<string, int> counts, string key)
        { int value; counts.TryGetValue(key, out value); counts[key] = value + 1; }
    }
}
