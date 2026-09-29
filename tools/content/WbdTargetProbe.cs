using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Read-only word-class inventory of the six CE WBD indexed target classes.
    // The 0x80-byte window is a prefix sample, not a claimed object size or decoded shape.
    internal static class WbdTargetProbe
    {
        private const int PrefixBytes = 0x200;
        private static readonly HashSet<uint> KnownVtables = new HashSet<uint> {
            0x0069C19C, 0x0069D56C, 0x0069AAF4, 0x0069D9E4, 0x0069BBEC, 0x0069D7F4,
            WbdDictionaryReader.CeVtable, 0x00695328
        };

        private sealed class WordStats
        {
            internal readonly Dictionary<string, int> Classes = new Dictionary<string, int>();
            internal readonly Dictionary<string, int> Pointees = new Dictionary<string, int>();
            internal readonly Dictionary<uint, int> Small = new Dictionary<uint, int>();
            internal int Seen;
            internal void Add(byte[] body, int systemSize, int graphicsSize, int at)
            {
                uint value = BitConverter.ToUInt32(body, at);
                BoundsLayout.WordClass kind = BoundsLayout.Classify(value, systemSize, graphicsSize);
                Count(Classes, kind.ToString()); Seen++;
                if (kind == BoundsLayout.WordClass.SmallInt) { int n; Small.TryGetValue(value, out n); Small[value] = n + 1; }
                if (kind == BoundsLayout.WordClass.SystemPointer)
                {
                    int target = (int)(value - 0x50000000);
                    uint first = target + 4 <= systemSize ? BitConverter.ToUInt32(body, target) : 0;
                    Count(Pointees, KnownVtables.Contains(first) ? "vtable 0x" + first.ToString("X8") : "other system target");
                }
            }
            internal Dictionary<string, object> Report(int offset)
            {
                return new Dictionary<string, object> {
                    { "offset", "0x" + offset.ToString("X2") }, { "seen", Seen }, { "classes", Classes },
                    { "pointees", Pointees },
                    { "smallValues", Small.Count <= 8 ? (object)Small.ToDictionary(p => p.Key.ToString(), p => p.Value) : Small.Count + " distinct" }
                };
            }
        }

        private sealed class ClassStats
        {
            internal int Entries;
            internal readonly HashSet<string> Resources = new HashSet<string>();
            internal readonly WordStats[] Words = Enumerable.Range(0, PrefixBytes / 4).Select(i => new WordStats()).ToArray();
            internal readonly List<object> Examples = new List<object>();
            internal readonly Dictionary<string, int> NextTargetGap = new Dictionary<string, int>();
            internal void Add(string archive, string file, string model, byte[] body, int systemSize, int graphicsSize, int target, int nextTarget)
            {
                Entries++; Resources.Add(archive + "/" + file);
                int gap = nextTarget < 0 ? -1 : nextTarget - target;
                Count(NextTargetGap, gap < 0 ? "none" : gap < 0x40 ? "<0x40" : gap < 0x80 ? "0x40-0x7F" : ">=0x80");
                if (Examples.Count < 8) { Examples.Add(new Dictionary<string, object> {
                    { "archive", archive }, { "bounds", file }, { "model", model },
                    { "target", "0x" + target.ToString("X") }, { "nextDictionaryTargetGap", gap < 0 ? "none" : "0x" + gap.ToString("X") }
                }); }
                for (int offset = 0; offset < PrefixBytes && target + offset + 4 <= systemSize && (gap < 0 || offset < gap); offset += 4)
                { Words[offset / 4].Add(body, systemSize, graphicsSize, target + offset); }
            }
            internal Dictionary<string, object> Report()
            {
                return new Dictionary<string, object> {
                    { "entries", Entries }, { "resources", Resources.Count }, { "examples", Examples },
                    { "nextDictionaryTargetGap", NextTargetGap },
                    { "prefixWords", Enumerable.Range(0, Words.Length).Where(i => Words[i].Seen > 0).Select(i => Words[i].Report(i * 4)).ToArray() }
                };
            }
        }

        internal static int Run(string[] args)
        {
            int gameAt = Array.IndexOf(args, "--game"), outAt = Array.IndexOf(args, "--out");
            if (gameAt < 0 || outAt < 0 || gameAt + 1 >= args.Length || outAt + 1 >= args.Length)
            { Console.WriteLine("usage: wbdtargets --game <dir> --out <json>"); return 2; }
            string game = args[gameAt + 1], output = args[outAt + 1];
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            List<string> skipped = new List<string>(), errors = new List<string>();
            Dictionary<string, ClassStats> classes = new Dictionary<string, ClassStats>();
            foreach (string relative in Probe.Archives(game, skipped))
            {
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    Dictionary<uint, string> names = new Dictionary<uint, string>();
                    foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)))
                    { string name = Path.GetFileNameWithoutExtension(entry.Name); names[TextureNameHash.Compute(name)] = name; }
                    foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wbd", StringComparison.OrdinalIgnoreCase)))
                    {
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                            if (resource.Type != 32 || BitConverter.ToUInt32(resource.Body, 0) != WbdDictionaryReader.CeVtable) { continue; }
                            WbdDictionaryReader reader = WbdDictionaryReader.Parse(resource);
                            List<int> targets = reader.Entries.Select(e => (int)(e.TargetPointer - 0x50000000)).Distinct().OrderBy(v => v).ToList();
                            foreach (WbdDictionaryReader.Entry item in reader.Entries)
                            {
                                int target = (int)(item.TargetPointer - 0x50000000), index = targets.BinarySearch(target);
                                int next = index + 1 < targets.Count ? targets[index + 1] : -1;
                                string vtable = "0x" + item.TargetVtable.ToString("X8"), model;
                                if (!names.TryGetValue(item.NameHash, out model)) { model = "<no same-IMG WDR match>"; }
                                ClassStats stats;
                                if (!classes.TryGetValue(vtable, out stats)) { stats = new ClassStats(); classes[vtable] = stats; }
                                stats.Add(relative, entry.Name, model, resource.Body, resource.SystemSize, resource.GraphicsSize, target, next);
                            }
                        }
                        catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + "/" + entry.Name + ": " + error.Message); } }
                    }
                }
                catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + ": " + error.Message); } }
            }
            Dictionary<string, object> report = new Dictionary<string, object> {
                { "question", "How do the six CE WBD indexed target classes differ in prefix word classes and nested pointers?" },
                { "rule", "structure only: word classes, valid pointer classes, small integers, names and offsets; no resource bytes or float values" },
                { "prefixBytes", PrefixBytes }, { "classes", classes.OrderByDescending(p => p.Value.Entries).ToDictionary(p => p.Key, p => (object)p.Value.Report()) },
                { "errors", errors }, { "skipped", skipped }
            };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllText(output, new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
            Console.WriteLine("wbdtargets classes=" + classes.Count + " entries=" + classes.Sum(p => p.Value.Entries) + " errors=" + errors.Count + " report=" + output);
            return classes.Count == 6 ? 0 : 1;
        }

        private static void Count(Dictionary<string, int> values, string key)
        { int n; values.TryGetValue(key, out n); values[key] = n + 1; }
    }
}
