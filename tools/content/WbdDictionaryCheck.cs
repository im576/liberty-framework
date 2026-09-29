using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Read-only check of the measured CE WBD name-hash/target table across installed archives.
    internal static class WbdDictionaryCheck
    {
        internal static int Run(string[] args)
        {
            int gameAt = Array.IndexOf(args, "--game"), outAt = Array.IndexOf(args, "--out");
            if (gameAt < 0 || outAt < 0 || gameAt + 1 >= args.Length || outAt + 1 >= args.Length)
            { Console.WriteLine("usage: wbdcheck --game <dir> --out <json>"); return 2; }
            string game = args[gameAt + 1], output = args[outAt + 1];
            byte[] key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe"));
            List<string> skipped = new List<string>(), errors = new List<string>();
            List<object> samples = new List<object>();
            Dictionary<string, int> targetVtables = new Dictionary<string, int>();
            int wbd = 0, ce = 0, parsed = 0, tableRoundTrips = 0, entries = 0, sameImgWdr = 0;
            foreach (string relative in Probe.Archives(game, skipped))
            {
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    HashSet<uint> hashes = new HashSet<uint>(archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase))
                        .Select(e => TextureNameHash.Compute(Path.GetFileNameWithoutExtension(e.Name))));
                    foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wbd", StringComparison.OrdinalIgnoreCase)))
                    {
                        wbd++;
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                            if (resource.Type != 32 || BitConverter.ToUInt32(resource.Body, 0) != WbdDictionaryReader.CeVtable) { continue; }
                            ce++;
                            WbdDictionaryReader reader = WbdDictionaryReader.Parse(resource);
                            parsed++;
                            if (reader.TableRoundTrip()) { tableRoundTrips++; }
                            else { errors.Add(relative + "/" + entry.Name + ": table fields did not round-trip"); }
                            int local = 0;
                            foreach (WbdDictionaryReader.Entry item in reader.Entries)
                            {
                                entries++;
                                if (hashes.Contains(item.NameHash)) { local++; sameImgWdr++; }
                                string vtable = "0x" + item.TargetVtable.ToString("X8");
                                int n; targetVtables.TryGetValue(vtable, out n); targetVtables[vtable] = n + 1;
                            }
                            if (samples.Count < 30 && reader.Entries.Count > 0)
                            {
                                samples.Add(new Dictionary<string, object> {
                                    { "archive", relative }, { "bounds", entry.Name }, { "hashArray", Hex(reader.HashOffset) },
                                    { "targetArray", Hex(reader.TargetOffset) }, { "count", reader.Entries.Count },
                                    { "localWdrNames", local }, { "hashCapacity", reader.HashCapacity },
                                    { "targetCapacity", reader.TargetCapacity }
                                });
                            }
                        }
                        catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + "/" + entry.Name + ": " + error.Message); } }
                    }
                }
                catch (Exception error) { if (errors.Count < 30) { errors.Add(relative + ": " + error.Message); } }
            }
            Dictionary<string, object> report = new Dictionary<string, object> {
                { "question", "Does CE WBD class 0x00695360 consistently contain equal-length name-hash and target-pointer arrays that round-trip?" },
                { "rule", "structure only: counts, names, offsets and target vtable classes; no resource bytes" },
                { "wbdResources", wbd }, { "ceClass", ce }, { "parsed", parsed }, { "tableRoundTrips", tableRoundTrips },
                { "entries", entries }, { "sameImgWdrNameHashes", sameImgWdr }, { "targetVtables", targetVtables },
                { "samples", samples }, { "errors", errors }, { "skipped", skipped }
            };
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllText(output, new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.Serialize(report));
            Console.WriteLine("wbdcheck wbd=" + wbd + " ce=" + ce + " parsed=" + parsed + " tableRoundTrips=" + tableRoundTrips + " entries=" + entries + " sameImgWdr=" + sameImgWdr + " errors=" + errors.Count);
            return ce > 0 && parsed == ce && parsed == tableRoundTrips ? 0 : 1;
        }

        private static string Hex(int value) { return "0x" + value.ToString("X"); }
    }
}
