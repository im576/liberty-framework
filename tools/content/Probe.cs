using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Read-only research probes over the owner's game files, run by tools/verify-local.ps1 (checks PROBE-*):
    //   probe drawables --game <game> --out <report.json>   how drawables use geometries, shaders and LOD slots
    //   probe collision --game <game> --out <report.json>   which collision resources exist, where, and their headers
    // Reports hold structure only: counts, sizes, resource types and flags, field values that describe layout, archive
    // and file names. Never geometry, pixels or raw asset bytes (third_party/README.md, AGENTS.md rule 7). A probe
    // answers a question for the next cloud session; it never passes or fails on the numbers themselves.
    internal static class Probe
    {
        private const int Samples = 25;
        // Extensions GTA IV uses for bounds (collision) resources, per public community documentation; the inventory
        // counts every extension, so a wrong guess here only changes which entries get a header summary.
        private static readonly string[] CollisionExtensions = { ".wbn", ".wbd", ".wbs" };

        internal static int Run(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "";
            string game = Option(args, "--game");
            string output = Option(args, "--out");
            if (game == null || output == null || (name != "drawables" && name != "collision"))
            {
                Console.WriteLine("usage: LibertyContent probe drawables|collision --game <game folder> --out <report.json>");
                return 2;
            }
            if (!File.Exists(Path.Combine(game, "GTAIV.exe"))) { Console.WriteLine("ERROR GTAIV.exe not found in " + game); return 1; }
            // The IMG key comes from the exe. Without it (a test folder) only unencrypted archives open; each failing archive
            // is reported in the result, never silently dropped.
            byte[] key = null;
            try { key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe")); }
            catch (InvalidDataException error) { Console.WriteLine("note: " + error.Message + "; encrypted archives will be reported as errors"); }
            Dictionary<string, object> report = name == "drawables" ? Drawables(game, key) : Collision(game, key);
            report["probe"] = name;
            report["generatedUtc"] = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            report["rule"] = "structure only: counts, sizes, types, layout fields and names; no geometry, pixels or asset bytes";
            JavaScriptSerializer json = new JavaScriptSerializer();
            json.MaxJsonLength = int.MaxValue;
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
            File.WriteAllText(output, json.Serialize(report));
            Console.WriteLine("probe " + name + " written: " + output);
            return 0;
        }

        private static string Option(string[] args, string name)
        {
            int index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }

        // Every IMG archive under the game folder (the IMG table carries each entry's resource type), except this
        // project's own archives. RPF archives are counted but not opened (IV keeps models and maps in IMGs).
        internal static List<string> Archives(string game, List<string> skipped)
        {
            List<string> found = new List<string>();
            foreach (string path in Directory.GetFiles(game, "*.*", SearchOption.AllDirectories))
            {
                string relative = path.Substring(game.Length).TrimStart('\\', '/').Replace('\\', '/');
                string extension = Path.GetExtension(path).ToLowerInvariant();
                if (extension != ".img" && extension != ".rpf") { continue; }
                if (relative.IndexOf("LibertyFramework", StringComparison.OrdinalIgnoreCase) >= 0) { skipped.Add(relative + " (this project's own archive)"); continue; }
                if (extension == ".rpf") { skipped.Add(relative + " (RPF, not opened)"); continue; }
                found.Add(relative);
            }
            found.Sort(StringComparer.OrdinalIgnoreCase);
            return found;
        }

        private static void Count(Dictionary<string, int> counts, string key)
        {
            int value;
            counts.TryGetValue(key, out value);
            counts[key] = value + 1;
        }

        private static string Bucket(int value)
        {
            if (value <= 4) { return value.ToString(CultureInfo.InvariantCulture); }
            if (value <= 8) { return "5-8"; }
            if (value <= 16) { return "9-16"; }
            if (value <= 64) { return "17-64"; }
            return "65+";
        }

        private static string SizeBucket(int bytes)
        {
            int kb = bytes / 1024;
            if (kb < 4) { return "<4 KB"; }
            if (kb < 64) { return "4-63 KB"; }
            if (kb < 256) { return "64-255 KB"; }
            if (kb < 1024) { return "256 KB-1 MB"; }
            return ">=1 MB";
        }

        private static Dictionary<string, int> Sorted(Dictionary<string, int> counts)
        {
            return counts.OrderByDescending(p => p.Value).ThenBy(p => p.Key, StringComparer.Ordinal).ToDictionary(p => p.Key, p => p.Value);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Drawables: the facts a multi-geometry / multi-LOD writer needs (docs/research/ModelFormat.md).

        private static Dictionary<string, object> Drawables(string game, byte[] key)
        {
            List<string> skipped = new List<string>();
            Dictionary<string, object> archivesReport = new Dictionary<string, object>();
            Dictionary<string, int> extensions = new Dictionary<string, int>(), failures = new Dictionary<string, int>();
            Dictionary<string, int> lodSlots = new Dictionary<string, int>(), modelsPerLod = new Dictionary<string, int>();
            Dictionary<string, int> geometriesPerModel = new Dictionary<string, int>(), shaders = new Dictionary<string, int>();
            Dictionary<string, int> layouts = new Dictionary<string, int>(), mapping = new Dictionary<string, int>();
            Dictionary<string, int> vertexOrder = new Dictionary<string, int>(), indexOrder = new Dictionary<string, int>();
            Dictionary<string, int> sharedBuffers = new Dictionary<string, int>(), lodDistances = new Dictionary<string, int>();
            Dictionary<string, int> systemSizes = new Dictionary<string, int>(), graphicsSizes = new Dictionary<string, int>();
            Dictionary<string, int> flagPages = new Dictionary<string, int>(), presets = new Dictionary<string, int>();
            List<object> samples = new List<object>();
            List<string> failureExamples = new List<string>();
            int total = 0, parsed = 0, withSkeleton = 0, withBones = 0, multiGeometry = 0, multiLod = 0;
            foreach (string relative in Archives(game, skipped))
            {
                int inArchive = 0, failedInArchive = 0;
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    foreach (ImgArchive.Entry entry in archive.Entries)
                    {
                        string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                        Count(extensions, extension);
                        if (extension != ".wdr") { continue; }
                        total++; inArchive++;
                        try
                        {
                            RscResource resource = RscResource.Parse(archive.Extract(entry.Name), true);
                            DrawableFile file = new DrawableFile(resource);
                            parsed++;
                            Count(systemSizes, SizeBucket(resource.SystemSize));
                            Count(graphicsSizes, SizeBucket(resource.GraphicsSize));
                            // RSC flags: bits 0-10 system size count, 11-14 its shift; 15-25 graphics count, 26-29 shift.
                            Count(flagPages, "system " + (resource.Flags & 0x7FF) + "<<" + ((resource.Flags >> 11) & 0xF) + ", graphics " + ((resource.Flags >> 15) & 0x7FF) + "<<" + ((resource.Flags >> 26) & 0xF));
                            if (file.Skeleton != 0) { withSkeleton++; }
                            int[] used = Enumerable.Range(0, 4).Where(l => file.Models.Any(m => m.Lod == l)).ToArray();
                            Count(lodSlots, string.Join(",", used.Select(l => l.ToString(CultureInfo.InvariantCulture)).ToArray()));
                            if (used.Length > 1) { multiLod++; }
                            for (int lod = 0; lod < 4; lod++)
                            {
                                int models = file.Models.Count(m => m.Lod == lod);
                                if (models > 0) { Count(modelsPerLod, "lod" + lod + ": " + Bucket(models)); }
                                float distance = file.View.F32(file.Root + (uint)(DrawableFile.DrawableLods + 0x10 + lod * 4));
                                Count(lodDistances, "lod" + lod + (models > 0 ? " used: " : " unused: ") + Math.Round(distance, 1).ToString(CultureInfo.InvariantCulture));
                            }
                            Count(shaders, Bucket(file.Shaders.Count));
                            foreach (DrawableShader shader in file.Shaders) { Count(presets, shader.Preset); }
                            bool anyMulti = false;
                            foreach (DrawableModel model in file.Models)
                            {
                                Count(geometriesPerModel, Bucket(model.Geometries.Count));
                                for (int i = 0; i < model.Geometries.Count; i++)
                                {
                                    DrawableGeometry g = model.Geometries[i];
                                    Count(layouts, "mask 0x" + g.Layout.Mask.ToString("X") + " stride " + g.Layout.Stride);
                                    if (g.BoneCount > 0) { withBones++; }
                                }
                                if (model.Geometries.Count < 2) { continue; }
                                anyMulti = true;
                                Count(mapping, model.Geometries.Select((g, i) => g.ShaderIndex == i).All(x => x) ? "geometry i uses shader i" :
                                    model.Geometries.Select(g => g.ShaderIndex).Distinct().Count() == model.Geometries.Count ? "distinct shaders, other order" : "shaders shared between geometries");
                                Count(vertexOrder, Order(model.Geometries.Select(g => g.VertexData).ToList(), model.Geometries.Select(g => g.VertexCount * g.Layout.Stride).ToList()));
                                Count(indexOrder, Order(model.Geometries.Select(g => g.IndexData).ToList(), model.Geometries.Select(g => g.IndexCount * 2).ToList()));
                                Count(sharedBuffers, model.Geometries.Select(g => g.VertexBuffer).Distinct().Count() == model.Geometries.Count ? "one vertex buffer per geometry" : "vertex buffers shared");
                            }
                            if (anyMulti) { multiGeometry++; }
                            if ((anyMulti || used.Length > 1) && samples.Count < Samples) { samples.Add(Sample(relative, entry.Name, resource, file)); }
                        }
                        catch (Exception error)
                        {
                            failedInArchive++;
                            string message = error.Message.Length > 90 ? error.Message.Substring(0, 90) : error.Message;
                            Count(failures, message);
                            if (failureExamples.Count < 20) { failureExamples.Add(relative + "/" + entry.Name + ": " + error.Message); }
                        }
                    }
                    archivesReport[relative] = new Dictionary<string, object> { { "drawables", inArchive }, { "failed", failedInArchive } };
                }
                catch (Exception error) { archivesReport[relative] = new Dictionary<string, object> { { "error", error.Message } }; }
            }
            return new Dictionary<string, object>
            {
                { "question", "How do the game's drawables use several geometries, several shaders and LOD slots 1-3, and does the reader parse them all?" },
                { "drawables", total }, { "parsed", parsed }, { "failed", total - parsed },
                { "multiGeometryDrawables", multiGeometry }, { "multiLodDrawables", multiLod },
                { "withSkeleton", withSkeleton }, { "geometriesWithBones", withBones },
                { "lodSlotsUsed", Sorted(lodSlots) }, { "modelsPerLod", Sorted(modelsPerLod) }, { "geometriesPerModel", Sorted(geometriesPerModel) },
                { "shadersPerDrawable", Sorted(shaders) }, { "shaderPresetsTop", Sorted(presets).Take(40).ToDictionary(p => p.Key, p => p.Value) },
                { "vertexLayouts", Sorted(layouts) },
                { "multiGeometryShaderMapping", Sorted(mapping) }, { "multiGeometryVertexData", Sorted(vertexOrder) },
                { "multiGeometryIndexData", Sorted(indexOrder) }, { "multiGeometryVertexBuffers", Sorted(sharedBuffers) },
                { "lodDistances", Sorted(lodDistances).Take(60).ToDictionary(p => p.Key, p => p.Value) },
                { "systemSizes", Sorted(systemSizes) }, { "graphicsSizes", Sorted(graphicsSizes) }, { "rscFlagSizes", Sorted(flagPages).Take(40).ToDictionary(p => p.Key, p => p.Value) },
                { "failures", Sorted(failures) }, { "failureExamples", failureExamples },
                { "entryExtensions", Sorted(extensions) }, { "archives", archivesReport }, { "skipped", skipped },
                { "samples", samples }
            };
        }

        // How a model's geometries place their data in the graphics segment, in geometry order.
        private static string Order(List<uint> starts, List<int> sizes)
        {
            bool ascending = true, packed = true;
            for (int i = 1; i < starts.Count; i++)
            {
                if (starts[i] < starts[i - 1] + sizes[i - 1]) { ascending = false; }
                if (starts[i] != starts[i - 1] + sizes[i - 1]) { packed = false; }
            }
            if (packed) { return "back to back in geometry order"; }
            return ascending ? "ascending with gaps" : "not in geometry order";
        }

        private static Dictionary<string, object> Sample(string archive, string name, RscResource resource, DrawableFile file)
        {
            List<object> lods = new List<object>();
            for (int lod = 0; lod < 4; lod++)
            {
                List<object> models = new List<object>();
                foreach (DrawableModel model in file.Models.Where(m => m.Lod == lod))
                {
                    models.Add(model.Geometries.Select(g => new Dictionary<string, object>
                    {
                        { "vertices", g.VertexCount }, { "indices", g.IndexCount }, { "shader", g.ShaderIndex },
                        { "layoutMask", "0x" + g.Layout.Mask.ToString("X") }, { "stride", g.Layout.Stride }, { "bones", g.BoneCount },
                        { "vertexDataOffset", g.VertexData - ResourceView.GraphicsBase }, { "indexDataOffset", g.IndexData - ResourceView.GraphicsBase }
                    }).ToList());
                }
                lods.Add(new Dictionary<string, object>
                {
                    { "lod", lod }, { "distance", file.View.F32(file.Root + (uint)(DrawableFile.DrawableLods + 0x10 + lod * 4)) }, { "models", models }
                });
            }
            return new Dictionary<string, object>
            {
                { "archive", archive }, { "name", name }, { "systemSize", resource.SystemSize }, { "graphicsSize", resource.GraphicsSize },
                { "flags", "0x" + resource.Flags.ToString("X8") }, { "skeleton", file.Skeleton != 0 },
                { "shaders", file.Shaders.Select(s => s.Preset).ToList() }, { "lods", lods }
            };
        }

        // ------------------------------------------------------------------------------------------------------------
        // Collision: an inventory for docs/research/Collision.md, before any structure is decoded.

        private static Dictionary<string, object> Collision(string game, byte[] key)
        {
            List<string> skipped = new List<string>();
            Dictionary<string, int> extensions = new Dictionary<string, int>();
            Dictionary<string, object> byArchive = new Dictionary<string, object>();
            Dictionary<string, int> headers = new Dictionary<string, int>(), firstWords = new Dictionary<string, int>();
            Dictionary<string, int> tableTypes = new Dictionary<string, int>();
            List<object> samples = new List<object>();
            List<string> errors = new List<string>();
            List<Dictionary<string, object>> candidates = new List<Dictionary<string, object>>();
            foreach (string relative in Archives(game, skipped))
            {
                Dictionary<string, int> local = new Dictionary<string, int>();
                // Base name -> extensions in this archive: a drawable (.wdr) shipped next to a bounds resource (.wbn) of the
                // same name is a model with its own collision file, the candidates for the RayMask.Objects scenario.
                Dictionary<string, HashSet<string>> byName = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    foreach (ImgArchive.Entry entry in archive.Entries)
                    {
                        string extension = Path.GetExtension(entry.Name).ToLowerInvariant();
                        Count(extensions, extension);
                        Count(local, extension);
                        string baseName = Path.GetFileNameWithoutExtension(entry.Name);
                        HashSet<string> kinds;
                        if (!byName.TryGetValue(baseName, out kinds)) { kinds = new HashSet<string>(); byName[baseName] = kinds; }
                        kinds.Add(extension);
                        if (!CollisionExtensions.Contains(extension)) { continue; }
                        Count(tableTypes, extension + " table type " + entry.ResourceType);
                        try
                        {
                            byte[] data = archive.Extract(entry.Name);
                            RscResource resource = RscResource.Parse(data, true);
                            Count(headers, extension + " rsc type " + resource.Type + ", system " + SizeBucket(resource.SystemSize) + ", graphics " + SizeBucket(resource.GraphicsSize));
                            // The first word of a RAGE resource's system segment is its root object's vtable address in
                            // the exe: the same value in different files means the same class. It is an address, not data.
                            uint vtable = resource.Body.Length >= 4 ? BitConverter.ToUInt32(resource.Body, 0) : 0;
                            Count(firstWords, extension + " root word 0x" + vtable.ToString("X8"));
                            if (samples.Count < Samples && samples.Cast<Dictionary<string, object>>().Count(s => (string)s["extension"] == extension) < 8)
                            {
                                List<string> words = new List<string>();
                                for (int i = 0; i < 8 && i * 4 + 4 <= resource.SystemSize && i * 4 + 4 <= resource.Body.Length; i++) { words.Add("0x" + BitConverter.ToUInt32(resource.Body, i * 4).ToString("X8")); }
                                samples.Add(new Dictionary<string, object>
                                {
                                    { "archive", relative }, { "name", entry.Name }, { "extension", extension }, { "rscType", resource.Type },
                                    { "flags", "0x" + resource.Flags.ToString("X8") }, { "systemSize", resource.SystemSize }, { "graphicsSize", resource.GraphicsSize },
                                    { "rootWords", words }
                                });
                            }
                        }
                        catch (Exception error) { if (errors.Count < 20) { errors.Add(relative + "/" + entry.Name + ": " + error.Message); } }
                    }
                    byArchive[relative] = Sorted(local);
                    foreach (KeyValuePair<string, HashSet<string>> pair in byName)
                    {
                        if (pair.Value.Contains(".wdr") && pair.Value.Contains(".wbn"))
                        {
                            candidates.Add(new Dictionary<string, object> { { "name", pair.Key.ToLowerInvariant() }, { "archive", relative }, { "fragment", pair.Value.Contains(".wft") } });
                        }
                    }
                }
                catch (Exception error) { byArchive[relative] = new Dictionary<string, object> { { "error", error.Message } }; }
            }
            // Sorted by name so every run on the same game files names the same first candidate.
            candidates = candidates.OrderBy(c => (string)c["name"], StringComparer.Ordinal).ToList();
            List<string> loose = new List<string>();
            foreach (string path in Directory.GetFiles(game, "*.*", SearchOption.AllDirectories))
            {
                if (CollisionExtensions.Contains(Path.GetExtension(path).ToLowerInvariant()) && loose.Count < 200) { loose.Add(path.Substring(game.Length).TrimStart('\\', '/').Replace('\\', '/')); }
            }
            return new Dictionary<string, object>
            {
                { "question", "Which collision (bounds) resources does the game ship, where, how many, and with which resource headers?" },
                { "collisionExtensions", CollisionExtensions }, { "entryExtensions", Sorted(extensions) },
                { "collisionTableTypes", Sorted(tableTypes) }, { "collisionHeaders", Sorted(headers) }, { "collisionRootWords", Sorted(firstWords).Take(60).ToDictionary(p => p.Key, p => p.Value) },
                { "looseCollisionFiles", loose }, { "errors", errors }, { "archives", byArchive }, { "skipped", skipped }, { "samples", samples },
                // Model names only (structure): what the raycast-objects scenario spawns ({probe:PROBE-collision:propCandidates}).
                { "propCandidates", candidates.Select(c => (string)c["name"]).Take(100).ToList() },
                { "propCandidateDetails", candidates.Take(100).ToList() }, { "propCandidateCount", candidates.Count }
            };
        }
    }
}
