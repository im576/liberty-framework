using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // A read-only hypothesis probe. It looks for Jenkins hashes of WDR names in bounds resources from the same IMG.
    // A match is a possible reference, not proof of a collision dictionary entry or a runtime pairing rule.
    // Only names, offsets and counts leave the owner's game; no resource bytes are written to the report.
    internal static class CollisionLinks
    {
        private const int MaxExamples = 100;
        private const int MaxErrors = 20;

        internal static Dictionary<string, object> Measure(string game, byte[] key)
        {
            List<string> skipped = new List<string>(), errors = new List<string>();
            List<object> examples = new List<object>();
            Dictionary<string, int> byExtension = new Dictionary<string, int>();
            HashSet<string> matchedModels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int archivesWithBoth = 0, resourcesScanned = 0, resourcesWithMatches = 0, matches = 0;

            foreach (string relative in Probe.Archives(game, skipped))
            {
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    Dictionary<uint, List<string>> localNames = new Dictionary<uint, List<string>>();
                    foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)))
                    {
                        string name = Path.GetFileNameWithoutExtension(entry.Name);
                        uint hash = TextureNameHash.Compute(name);
                        List<string> names;
                        if (!localNames.TryGetValue(hash, out names)) { names = new List<string>(); localNames[hash] = names; }
                        names.Add(name);
                    }
                    List<ImgArchive.Entry> bounds = archive.Entries.Where(e =>
                        e.Name.EndsWith(".wbd", StringComparison.OrdinalIgnoreCase) ||
                        e.Name.EndsWith(".wbn", StringComparison.OrdinalIgnoreCase)).ToList();
                    if (localNames.Count == 0 || bounds.Count == 0) { continue; }
                    archivesWithBoth++;

                    foreach (ImgArchive.Entry entry in bounds)
                    {
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                            resourcesScanned++;
                            bool found = false;
                            for (int at = 0; at + 4 <= resource.SystemSize; at += 4)
                            {
                                List<string> names;
                                if (!localNames.TryGetValue(BitConverter.ToUInt32(resource.Body, at), out names)) { continue; }
                                found = true;
                                matches += names.Count;
                                string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                                int count;
                                byExtension.TryGetValue(extension, out count);
                                byExtension[extension] = count + names.Count;
                                foreach (string name in names)
                                {
                                    matchedModels.Add(relative + "/" + name);
                                    if (examples.Count < MaxExamples)
                                    {
                                        examples.Add(new Dictionary<string, object> {
                                            { "archive", relative }, { "bounds", entry.Name }, { "model", name },
                                            { "systemOffset", "0x" + at.ToString("X") }
                                        });
                                    }
                                }
                            }
                            if (found) { resourcesWithMatches++; }
                        }
                        catch (Exception error)
                        {
                            if (errors.Count < MaxErrors) { errors.Add(relative + "/" + entry.Name + ": " + error.Message); }
                        }
                    }
                }
                catch (Exception error)
                {
                    if (errors.Count < MaxErrors) { errors.Add(relative + ": " + error.Message); }
                }
            }
            return new Dictionary<string, object>
            {
                { "question", "Do system words in a WBD or WBN match Jenkins hashes of WDR names in the same IMG, and at which offsets?" },
                { "hypothesis", "A matching word may be a model reference; matching does not establish a field meaning or runtime collision pairing." },
                { "archivesWithDrawablesAndBounds", archivesWithBoth }, { "boundsResourcesScanned", resourcesScanned },
                { "boundsResourcesWithMatches", resourcesWithMatches }, { "matchingWords", matches },
                { "distinctMatchedModels", matchedModels.Count }, { "matchesByExtension", byExtension },
                { "examples", examples }, { "errors", errors }, { "skipped", skipped }
            };
        }
    }
}
