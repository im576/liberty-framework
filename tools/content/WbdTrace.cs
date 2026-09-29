using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Read-only structural trace from consecutive local WDR-name hashes in a WBD.
    // Reports names, offsets, pointer targets and small integers; never resource bytes.
    internal static class WbdTrace
    {
        internal static int Run(string[] args)
        {
            int gameAt = Array.IndexOf(args, "--game"), outAt = Array.IndexOf(args, "--out");
            if (gameAt < 0 || outAt < 0 || gameAt + 1 >= args.Length || outAt + 1 >= args.Length)
            { Console.WriteLine("usage: wbdtrace --game <dir> --out <json> <archive.img...>"); return 2; }
            string game = args[gameAt + 1], output = args[outAt + 1];
            List<string> archives = args.Where((value, i) => i != gameAt && i != gameAt + 1 && i != outAt && i != outAt + 1).ToList();
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            List<object> files = new List<object>();
            foreach (string relative in archives)
            {
                ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                Dictionary<uint, string> names = new Dictionary<uint, string>();
                foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)))
                {
                    string name = Path.GetFileNameWithoutExtension(entry.Name);
                    names[TextureNameHash.Compute(name)] = name;
                }
                foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wbd", StringComparison.OrdinalIgnoreCase)))
                {
                    RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                    files.Add(Trace(relative, entry.Name, resource, names));
                }
            }
            Dictionary<string, object> report = new Dictionary<string, object> {
                { "question", "Which system pointers lead to consecutive same-IMG WDR-name hashes in CE WBDs?" },
                { "rule", "structure only: names, offsets, pointer classes/targets and small counts; no game asset bytes" },
                { "files", files }
            };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllText(output, new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
            Console.WriteLine("wbdtrace files=" + files.Count + " report=" + output);
            return 0;
        }

        private static Dictionary<string, object> Trace(string archive, string name, RscResource resource, Dictionary<uint, string> names)
        {
            int size = resource.SystemSize;
            byte[] body = resource.Body;
            if (resource.Type != 32 || size < 0x20 || BitConverter.ToUInt32(body, 0) != WbdDictionaryReader.CeVtable)
            { return new Dictionary<string, object> { { "archive", archive }, { "bounds", name }, { "class", "not CE WBD 0x00695360" } }; }
            List<int> hits = new List<int>();
            for (int at = 0; at + 4 <= size; at += 4) { if (names.ContainsKey(BitConverter.ToUInt32(body, at))) { hits.Add(at); } }
            List<List<int>> runs = new List<List<int>>();
            foreach (int at in hits)
            {
                if (runs.Count == 0 || runs[runs.Count - 1].Last() + 4 != at) { runs.Add(new List<int>()); }
                runs[runs.Count - 1].Add(at);
            }
            List<object> selected = new List<object>();
            foreach (List<int> run in runs.Where(r => r.Count >= 3).OrderByDescending(r => r.Count).Take(4))
            {
                int begin = run[0], end = run.Last() + 4;
                List<object> incoming = new List<object>();
                for (int at = 0; at + 4 <= size; at += 4)
                {
                    uint word = BitConverter.ToUInt32(body, at);
                    if (BoundsLayout.Classify(word, size, resource.GraphicsSize) != BoundsLayout.WordClass.SystemPointer) { continue; }
                    int target = (int)(word - 0x50000000);
                    if (target >= begin - 0x100 && target <= end + 0x100)
                    { incoming.Add(new Dictionary<string, object> { { "at", Hex(at) }, { "target", Hex(target) }, { "deltaFromRun", target - begin } }); }
                }
                int from = Math.Max(0, (begin - 0x60) & ~3), to = Math.Min(size, (end + 0x60) & ~3);
                List<object> context = new List<object>();
                for (int at = from; at + 4 <= to; at += 4) { context.Add(Word(body, at, size, resource.GraphicsSize, names)); }
                selected.Add(new Dictionary<string, object> {
                    { "start", Hex(begin) }, { "length", run.Count },
                    { "models", run.Select(at => names[BitConverter.ToUInt32(body, at)]).ToArray() },
                    { "incomingNearRun", incoming }, { "context", context }
                });
            }
            List<object> root = new List<object>();
            for (int at = 0; at < Math.Min(size, 0x40); at += 4) { root.Add(Word(body, at, size, resource.GraphicsSize, names)); }
            List<object> candidateCounts = new List<object>();
            foreach (int at in new[] { 0x08, 0x14, 0x1C })
            {
                if (at + 4 > size) { continue; }
                int low = BitConverter.ToUInt16(body, at), high = BitConverter.ToUInt16(body, at + 2);
                if (low <= 4096 && high <= 4096)
                { candidateCounts.Add(new Dictionary<string, object> { { "at", Hex(at) }, { "low16", low }, { "high16", high } }); }
            }
            List<object> rootArrays = new List<object>();
            foreach (int slot in new[] { 0x10, 0x18 })
            {
                uint pointer = BitConverter.ToUInt32(body, slot);
                if (BoundsLayout.Classify(pointer, size, resource.GraphicsSize) != BoundsLayout.WordClass.SystemPointer) { continue; }
                int start = (int)(pointer - 0x50000000);
                List<object> head = new List<object>();
                for (int i = 0; i < 24 && start + i * 4 + 4 <= size; i++) { head.Add(Word(body, start + i * 4, size, resource.GraphicsSize, names)); }
                rootArrays.Add(new Dictionary<string, object> { { "rootSlot", Hex(slot) }, { "target", Hex(start) }, { "firstWords", head } });
            }
            Dictionary<string, object> parallel = ParallelArrays(body, size, resource.GraphicsSize, names);
            return new Dictionary<string, object> {
                { "archive", archive }, { "bounds", name }, { "type", resource.Type }, { "rootVtable", "0x00695360" }, { "systemSize", size },
                { "root", root }, { "candidateCounts", candidateCounts }, { "rootArrays", rootArrays },
                { "matchingWords", hits.Count }, { "runs", selected }, { "parallelArrays", parallel }
            };
        }

        private static Dictionary<string, object> ParallelArrays(byte[] body, int size, int graphicsSize, Dictionary<uint, string> names)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            int hashes = (int)(BitConverter.ToUInt32(body, 0x10) - 0x50000000);
            int pointers = (int)(BitConverter.ToUInt32(body, 0x18) - 0x50000000);
            int hashCount = BitConverter.ToUInt16(body, 0x14), hashCapacity = BitConverter.ToUInt16(body, 0x16);
            int pointerCount = BitConverter.ToUInt16(body, 0x1C), pointerCapacity = BitConverter.ToUInt16(body, 0x1E);
            result["hashCount"] = hashCount; result["hashCapacity"] = hashCapacity;
            result["pointerCount"] = pointerCount; result["pointerCapacity"] = pointerCapacity;
            bool safe = hashCount == pointerCount && hashCount > 0 && hashCount <= hashCapacity &&
                pointerCount <= pointerCapacity && hashes >= 0 && pointers >= 0 &&
                hashes + hashCount * 4 <= size && pointers + pointerCount * 4 <= size;
            result["arraysInRangeAndEqualCount"] = safe;
            if (!safe) { return result; }
            int valid = 0, local = 0;
            Dictionary<string, int> targetVtables = new Dictionary<string, int>();
            List<object> examples = new List<object>();
            for (int i = 0; i < hashCount; i++)
            {
                uint hash = BitConverter.ToUInt32(body, hashes + i * 4);
                uint pointer = BitConverter.ToUInt32(body, pointers + i * 4);
                if (BoundsLayout.Classify(pointer, size, graphicsSize) != BoundsLayout.WordClass.SystemPointer) { continue; }
                int target = (int)(pointer - 0x50000000);
                if (target + 4 > size) { continue; }
                valid++;
                string vtable = "0x" + BitConverter.ToUInt32(body, target).ToString("X8");
                int count; targetVtables.TryGetValue(vtable, out count); targetVtables[vtable] = count + 1;
                string model;
                if (names.TryGetValue(hash, out model))
                {
                    local++;
                    if (examples.Count < 30) { examples.Add(new Dictionary<string, object> {
                        { "index", i }, { "model", model }, { "hashOffset", Hex(hashes + i * 4) },
                        { "pointerOffset", Hex(pointers + i * 4) }, { "target", Hex(target) }, { "targetVtable", vtable }
                    }); }
                }
            }
            result["validSystemTargets"] = valid;
            result["localWdrMatchesAtSameIndex"] = local;
            result["targetVtables"] = targetVtables;
            result["examples"] = examples;
            return result;
        }

        private static Dictionary<string, object> Word(byte[] body, int at, int systemSize, int graphicsSize, Dictionary<uint, string> names)
        {
            uint word = BitConverter.ToUInt32(body, at);
            Dictionary<string, object> row = new Dictionary<string, object> { { "at", Hex(at) } };
            string model;
            if (names.TryGetValue(word, out model)) { row["class"] = "localWdrHash"; row["model"] = model; return row; }
            BoundsLayout.WordClass kind = BoundsLayout.Classify(word, systemSize, graphicsSize);
            row["class"] = kind.ToString();
            if (kind == BoundsLayout.WordClass.SystemPointer) { row["target"] = Hex((int)(word - 0x50000000)); }
            if (kind == BoundsLayout.WordClass.GraphicsPointer) { row["target"] = Hex((int)(word - 0x60000000)); }
            if (kind == BoundsLayout.WordClass.SmallInt) { row["value"] = word; }
            return row;
        }

        private static string Hex(int value) { return "0x" + value.ToString("X"); }
    }
}
