using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // `probe bounds` (check PROBE-bounds-layout, T-032): the layout of the game's collision resources, measured instead of
    // assumed. Files are grouped by extension, RSC type and serialized root class word, not a proven runtime address.
    // For each class, every 4-byte word of the root structure is classified across all its files as zero, a system or
    // graphics pointer (inside the file's segments), a small integer (with its values: type codes and counts), a plausible
    // float, or other. The same is done one level down for each root offset that holds a system pointer in most files.
    // The smallest pointer target bounds the root structure's size. That is the input for decoding one class
    // (docs/research/Collision.md, plan step 3). Structure only: word classes, small-integer field values, sizes and
    // names; never float values, geometry or raw bytes.
    internal static class BoundsLayout
    {
        internal const int RootBytes = 0x100, PointedBytes = 0x40, MaxClasses = 12, MaxSmallIntValues = 8, Samples = 5;
        // Pointed-to structures are measured for root offsets that hold a system pointer in at least this share of files.
        internal const double PointerShare = 0.5;

        internal enum WordClass { Zero, SystemPointer, GraphicsPointer, SmallInt, Float, Other }

        internal static WordClass Classify(uint word, int systemSize, int graphicsSize)
        {
            if (word == 0) { return WordClass.Zero; }
            if ((word & 0xF0000000) == 0x50000000 && word - 0x50000000 < (uint)systemSize) { return WordClass.SystemPointer; }
            if ((word & 0xF0000000) == 0x60000000 && word - 0x60000000 < (uint)graphicsSize) { return WordClass.GraphicsPointer; }
            if (word < 0x10000) { return WordClass.SmallInt; }
            float value = BitConverter.ToSingle(BitConverter.GetBytes(word), 0);
            if (!float.IsNaN(value) && !float.IsInfinity(value) && Math.Abs(value) >= 1e-6 && Math.Abs(value) <= 1e6) { return WordClass.Float; }
            return WordClass.Other;
        }

        internal sealed class OffsetStats
        {
            internal readonly int[] Counts = new int[6];
            internal readonly Dictionary<uint, int> SmallInts = new Dictionary<uint, int>();
            internal int Total;

            internal void Add(WordClass kind, uint word)
            {
                Counts[(int)kind]++;
                Total++;
                if (kind != WordClass.SmallInt) { return; }
                int count;
                SmallInts.TryGetValue(word, out count);
                SmallInts[word] = count + 1;
            }

            internal Dictionary<string, object> Report(int offset)
            {
                Dictionary<string, object> row = new Dictionary<string, object> { { "offset", "0x" + offset.ToString("X2") } };
                foreach (WordClass kind in Enum.GetValues(typeof(WordClass)))
                {
                    int percent = Total == 0 ? 0 : (int)Math.Round(100.0 * Counts[(int)kind] / Total);
                    if (percent > 0) { row[Name(kind)] = percent; }
                }
                if (SmallInts.Count > 0)
                {
                    row["smallIntValues"] = SmallInts.Count > MaxSmallIntValues ? (object)(SmallInts.Count + " distinct")
                        : SmallInts.OrderBy(p => p.Key).ToDictionary(p => p.Key.ToString(CultureInfo.InvariantCulture), p => (object)p.Value);
                }
                return row;
            }

            private static string Name(WordClass kind)
            {
                switch (kind)
                {
                    case WordClass.SystemPointer: return "systemPointer";
                    case WordClass.GraphicsPointer: return "graphicsPointer";
                    case WordClass.SmallInt: return "smallInt";
                    default: return kind.ToString().Substring(0, 1).ToLowerInvariant() + kind.ToString().Substring(1);
                }
            }
        }

        internal sealed class ClassStats
        {
            internal int Files;
            internal readonly OffsetStats[] Root = Enumerable.Range(0, RootBytes / 4).Select(i => new OffsetStats()).ToArray();
            internal readonly Dictionary<int, OffsetStats[]> Pointed = new Dictionary<int, OffsetStats[]>();
            internal readonly Dictionary<string, int> SystemSizes = new Dictionary<string, int>(), GraphicsSizes = new Dictionary<string, int>(), RootBounds = new Dictionary<string, int>();
            internal readonly List<string> Samples = new List<string>();

            // One resource of this class.
            internal void Add(RscResource resource, string name)
            {
                Files++;
                if (Samples.Count < BoundsLayout.Samples) { Samples.Add(name); }
                Count(SystemSizes, Bucket(resource.SystemSize));
                Count(GraphicsSizes, Bucket(resource.GraphicsSize));
                int limit = Math.Min(RootBytes, resource.SystemSize);
                int lowestTarget = int.MaxValue;
                for (int at = 0; at + 4 <= limit; at += 4)
                {
                    uint word = BitConverter.ToUInt32(resource.Body, at);
                    WordClass kind = Classify(word, resource.SystemSize, resource.GraphicsSize);
                    Root[at / 4].Add(kind, word);
                    if (kind != WordClass.SystemPointer) { continue; }
                    int target = (int)(word - 0x50000000);
                    if (target > 0) { lowestTarget = Math.Min(lowestTarget, target); }
                    OffsetStats[] pointed;
                    if (!Pointed.TryGetValue(at, out pointed)) { pointed = Enumerable.Range(0, PointedBytes / 4).Select(i => new OffsetStats()).ToArray(); Pointed[at] = pointed; }
                    for (int k = 0; k + 4 <= PointedBytes && target + k + 4 <= resource.SystemSize; k += 4)
                    {
                        uint inner = BitConverter.ToUInt32(resource.Body, target + k);
                        pointed[k / 4].Add(Classify(inner, resource.SystemSize, resource.GraphicsSize), inner);
                    }
                }
                Count(RootBounds, lowestTarget == int.MaxValue ? "no pointer" : "<= 0x" + (lowestTarget < 0x40 ? "3F" : lowestTarget < 0x80 ? "7F" : lowestTarget < 0x100 ? "FF" : "FFF+"));
            }

            internal Dictionary<string, object> Report()
            {
                int measured = Root.Count(o => o.Total > 0);
                return new Dictionary<string, object>
                {
                    { "files", Files }, { "samples", Samples }, { "systemSizes", SystemSizes }, { "graphicsSizes", GraphicsSizes },
                    { "rootSizeBound", RootBounds },
                    { "root", Enumerable.Range(0, measured).Select(i => (object)Root[i].Report(i * 4)).ToList() },
                    { "pointed", Pointed.Where(p => Root[p.Key / 4].Counts[(int)WordClass.SystemPointer] >= PointerShare * Files).OrderBy(p => p.Key)
                        .ToDictionary(p => "root+0x" + p.Key.ToString("X2"), p => (object)p.Value.Where(o => o.Total > 0).Select((o, i) => (object)o.Report(i * 4)).ToList()) },
                };
            }
        }

        internal static Dictionary<string, object> Measure(string game, byte[] key)
        {
            List<string> skipped = new List<string>(), errors = new List<string>();
            Dictionary<string, ClassStats> classes = new Dictionary<string, ClassStats>();
            int resources = 0;
            foreach (string relative in Probe.Archives(game, skipped))
            {
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    foreach (ImgArchive.Entry entry in archive.Entries.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                        if (!Probe.CollisionExtensions.Contains(extension)) { continue; }
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                            resources++;
                            uint vtable = resource.Body.Length >= 4 ? BitConverter.ToUInt32(resource.Body, 0) : 0;
                            string name = extension + " rsc type " + resource.Type + " root 0x" + vtable.ToString("X8");
                            ClassStats stats;
                            if (!classes.TryGetValue(name, out stats)) { stats = new ClassStats(); classes[name] = stats; }
                            stats.Add(resource, relative + "/" + entry.Name);
                        }
                        catch (Exception error) { if (errors.Count < 20) { errors.Add(relative + "/" + entry.Name + ": " + error.Message); } }
                    }
                }
                catch (Exception error) { errors.Add(relative + ": " + error.Message); }
            }
            return new Dictionary<string, object>
            {
                { "question", "What does each class of collision resource look like: which root words are pointers, counts or type codes, and how large is the root?" },
                { "extensions", Probe.CollisionExtensions }, { "resources", resources }, { "classes", classes.Count },
                { "layout", classes.OrderByDescending(p => p.Value.Files).ThenBy(p => p.Key, StringComparer.Ordinal).Take(MaxClasses).ToDictionary(p => p.Key, p => (object)p.Value.Report()) },
                { "errors", errors }, { "skipped", skipped },
            };
        }

        private static void Count(Dictionary<string, int> counts, string key)
        {
            int value;
            counts.TryGetValue(key, out value);
            counts[key] = value + 1;
        }

        private static string Bucket(int bytes)
        {
            int kb = bytes / 1024;
            return kb < 4 ? "<4 KB" : kb < 64 ? "4-63 KB" : kb < 1024 ? "64 KB-1 MB" : ">=1 MB";
        }
    }
}
