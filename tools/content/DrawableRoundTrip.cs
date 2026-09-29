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
    // `roundtrip --game <game> [--out <report.json>] [archive...]` (no archive: every IMG under the game, as the probes scan).
    // Rebuilds each drawable the structure writer can use (DrawableStructureBuilder.Unsupported) from its own meshes with
    // Rockstar's page rules, once per GraphicsOrder candidate, and compares the result with the original: the system segment
    // byte for byte except the bounds the builder derives from vertices (box, centre, radius and the measured sphere
    // records, checked with a 1 mm tolerance), and every geometry's vertex and index bytes. Identical output under one
    // candidate proves, for that file, the per-geometry patching, the trimming-free structure, the bounds records and the
    // buffer placement. This is the multi-geometry / multi-LOD extension of LibertyModel's single-geometry selftest.
    // Read only; the report holds names, counts and offsets, never asset data. Check T031-drawable-roundtrip.
    internal static class DrawableRoundTrip
    {
        private const int MaxListed = 100;

        internal sealed class Outcome
        {
            internal string Skip;                  // why the file is not eligible, else null
            internal bool Multi;                   // more than one geometry (any LOD)
            internal readonly List<DrawableStructureBuilder.GraphicsOrder> Identical = new List<DrawableStructureBuilder.GraphicsOrder>();
            internal readonly Dictionary<DrawableStructureBuilder.GraphicsOrder, List<string>> Differences = new Dictionary<DrawableStructureBuilder.GraphicsOrder, List<string>>();
            internal readonly List<string> BoundsRecords = new List<string>(); // per model: "records/geometries"
            internal bool FlagsEqual;
            internal string StaticSubsetExclusion;
            internal Dictionary<string, object> GraphicsLayout { get; set; }
        }

        internal static int Run(string[] args)
        {
            string game = null, output = null;
            List<string> archives = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--game" && i + 1 < args.Length) { game = args[++i]; }
                else if (args[i] == "--out" && i + 1 < args.Length) { output = args[++i]; }
                else { archives.Add(args[i]); }
            }
            if (game == null) { Console.WriteLine("usage: LibertyContent roundtrip --game <game folder> [--out <report.json>] [archive...]"); return 2; }
            if (!File.Exists(Path.Combine(game, "GTAIV.exe"))) { Console.WriteLine("ERROR GTAIV.exe not found in " + game); return 1; }
            byte[] key = null;
            try { key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe")); }
            catch (InvalidDataException error) { Console.WriteLine("note: " + error.Message + "; encrypted archives will be reported as errors"); }
            List<string> skippedArchives = new List<string>();
            if (archives.Count == 0) { archives = Probe.Archives(game, skippedArchives); }

            int drawables = 0, eligible = 0, multi = 0, identical = 0, failed = 0, flagsEqual = 0;
            Dictionary<string, int> byOrder = new Dictionary<string, int>(), skips = new Dictionary<string, int>(), bounds = new Dictionary<string, int>();
            List<string> failures = new List<string>(), archiveErrors = new List<string>();
            List<object> graphicsLayouts = new List<object>();
            int staticEligible = 0, staticIdentical = 0, staticFailed = 0;
            Dictionary<string, int> staticExcluded = new Dictionary<string, int>();
            List<string> staticFailures = new List<string>();
            Dictionary<string, object> perArchive = new Dictionary<string, object>();
            foreach (string relative in archives)
            {
                int inArchive = 0, identicalInArchive = 0;
                try
                {
                    ImgArchive archive = ImgArchive.Open(Path.Combine(game, relative), key);
                    foreach (ImgArchive.Entry entry in archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)).OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        drawables++;
                        Outcome outcome;
                        try { outcome = Check(RscResource.Parse(archive.Extract(entry.Name), true), failures.Count < MaxListed); }
                        catch (Exception error) { outcome = new Outcome { Skip = "does not parse: " + Short(error.Message) }; }
                        if (outcome.Skip != null) { Increment(skips, outcome.Skip); continue; }
                        eligible++; inArchive++;
                        if (outcome.StaticSubsetExclusion != null) { Increment(staticExcluded, outcome.StaticSubsetExclusion); }
                        else
                        {
                            staticEligible++;
                            if (outcome.Identical.Count > 0) { staticIdentical++; }
                            else
                            {
                                staticFailed++;
                                if (staticFailures.Count < MaxListed) { staticFailures.Add(relative + "/" + entry.Name); }
                            }
                        }
                        if (outcome.Multi) { multi++; }
                        if (outcome.FlagsEqual) { flagsEqual++; }
                        foreach (string record in outcome.BoundsRecords) { Increment(bounds, record); }
                        if (outcome.Identical.Count > 0)
                        {
                            identical++; identicalInArchive++;
                            Increment(byOrder, string.Join("+", outcome.Identical.Select(order => order.ToString()).ToArray()));
                            continue;
                        }
                        failed++;
                        if (failures.Count < MaxListed)
                        {
                            failures.Add(relative + "/" + entry.Name + ": " + string.Join("; ", outcome.Differences.Select(p => p.Key + ": " + string.Join(" ", p.Value.Take(6).ToArray())).ToArray()));
                            if (outcome.GraphicsLayout != null) { graphicsLayouts.Add(new Dictionary<string, object> {
                                { "file", relative + "/" + entry.Name }, { "templateOrderLayout", outcome.GraphicsLayout }
                            }); }
                        }
                    }
                    perArchive[relative] = new Dictionary<string, object> { { "eligible", inArchive }, { "identical", identicalInArchive } };
                }
                catch (Exception error) { archiveErrors.Add(relative + ": " + error.Message); perArchive[relative] = new Dictionary<string, object> { { "error", error.Message } }; }
            }

            foreach (string failure in failures) { Console.WriteLine("  " + failure); }
            foreach (string error in archiveErrors) { Console.WriteLine("  archive " + error); }
            // A matching subset cannot prove the requested archive set when another archive was unreadable.
            bool ok = eligible > 0 && failed == 0 && archiveErrors.Count == 0;
            string summary = "roundtrip: " + (ok ? "ok" : "FAILED") + " drawables=" + drawables + " eligible=" + eligible + " multi=" + multi + " identical=" + identical + " failed=" + failed +
                " orders[" + string.Join(", ", byOrder.OrderBy(p => p.Key).Select(p => p.Key + "=" + p.Value).ToArray()) + "] archiveErrors=" + archiveErrors.Count + (eligible == 0 ? " (no eligible drawable: nothing was proven)" : "");
            if (output != null)
            {
                JavaScriptSerializer json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
                File.WriteAllText(output, json.Serialize(new Dictionary<string, object>
                {
                    { "question", "Does the structure writer rebuild the game's multi-geometry and multi-LOD drawables byte for byte, and under which buffer order?" },
                    { "rule", "structure only: names, counts and offsets; no geometry or asset bytes" },
                    { "drawables", drawables }, { "eligible", eligible }, { "multiGeometry", multi }, { "identical", identical }, { "failed", failed },
                    { "identicalByOrder", byOrder }, { "boundsRecords", bounds }, { "pageFlagsEqual", flagsEqual },
                    { "notEligible", skips.OrderByDescending(p => p.Value).Take(40).ToDictionary(p => p.Key, p => p.Value) },
                    { "failures", failures }, { "archives", perArchive }, { "archiveErrors", archiveErrors }, { "skippedArchives", skippedArchives },
                    { "graphicsLayouts", graphicsLayouts }, { "graphicsLayoutsLimit", MaxListed },
                    { "externalTextureStaticSubset", new Dictionary<string, object> {
                        { "eligible", staticEligible }, { "identical", staticIdentical }, { "failed", staticFailed },
                        { "excluded", staticExcluded }, { "failures", staticFailures },
                        { "rule", "same broad builder eligibility, additionally no skeleton or embedded texture dictionary; not proof of material/template matching or runtime rendering" }
                    } },
                }));
            }
            Console.WriteLine(summary + (output != null ? " report=" + output : ""));
            return ok ? 0 : 1;
        }

        internal static Outcome Check(RscResource original, bool graphicsDetails = false)
        {
            Outcome outcome = new Outcome();
            DrawableFile file = new DrawableFile(original);
            outcome.Skip = DrawableStructureBuilder.Unsupported(file);
            if (outcome.Skip != null) { return outcome; }
            outcome.StaticSubsetExclusion = file.Skeleton != 0 && file.EmbeddedTextures != 0 ? "skeleton and embedded dictionary" :
                file.Skeleton != 0 ? "skeleton" : file.EmbeddedTextures != 0 ? "embedded dictionary" : null;
            outcome.Multi = DrawableStructureBuilder.Geometries(file).Count() > 1;
            foreach (DrawableModel model in file.Models)
            {
                outcome.BoundsRecords.Add(model.Bounds == 0 ? "no bounds" : DrawableStructureBuilder.BoundsRecords(file, model) + " records for " + model.Geometries.Count + " geometr" + (model.Geometries.Count == 1 ? "y" : "ies"));
            }
            foreach (DrawableStructureBuilder.GraphicsOrder order in new[] { DrawableStructureBuilder.GraphicsOrder.Interleaved, DrawableStructureBuilder.GraphicsOrder.VerticesFirst, DrawableStructureBuilder.GraphicsOrder.Template })
            {
                DrawableStructureBuilder.Plan plan = DrawableStructureBuilder.Identity(file);
                plan.SinglePage = false;
                plan.Order = order;
                plan.ExactBounds = true;
                List<string> differences;
                try
                {
                    DrawableStructureBuilder.Output rebuilt = DrawableStructureBuilder.Build(file, plan);
                    differences = Compare(original, file, rebuilt);
                    if (graphicsDetails && order == DrawableStructureBuilder.GraphicsOrder.Template && differences.Count > 0)
                    { outcome.GraphicsLayout = DrawableGraphicsLayout.Describe(original, file, rebuilt); }
                    if (rebuilt.Resource.Flags == original.Flags) { outcome.FlagsEqual = true; }
                }
                catch (Exception error) { differences = new List<string> { "error " + Short(error.Message) }; }
                if (differences.Count == 0) { outcome.Identical.Add(order); } else { outcome.Differences[order] = differences; }
            }
            return outcome;
        }

        // Differences between the original and a rebuild of it, as short labels ("sys+0x1A0", "gfx vertices geometry 2").
        internal static List<string> Compare(RscResource original, DrawableFile file, DrawableStructureBuilder.Output rebuilt)
        {
            List<string> diffs = new List<string>();
            RscResource copy = rebuilt.Resource;
            if (copy.SystemSize != original.SystemSize) { diffs.Add("system size " + copy.SystemSize + " != " + original.SystemSize); return diffs; }
            for (int i = 0; i < original.SystemSize; i++)
            {
                if (!rebuilt.BoundsBytes.Contains(i) && original.Body[i] != copy.Body[i]) { diffs.Add("sys+0x" + (i & ~3).ToString("X")); i |= 3; }
            }
            // The rebuilt box must contain the stored one within the tolerance (the game stores pre-quantisation bounds).
            ResourceView before = file.View, after = new ResourceView(copy);
            for (int axis = 0; axis < 3; axis++)
            {
                uint offset = (uint)(axis * 4);
                if (after.F32(file.Root + DrawableFile.DrawableMin + offset) > before.F32(file.Root + DrawableFile.DrawableMin + offset) + DrawableStructureBuilder.BoundsToleranceMeters ||
                    after.F32(file.Root + DrawableFile.DrawableMax + offset) < before.F32(file.Root + DrawableFile.DrawableMax + offset) - DrawableStructureBuilder.BoundsToleranceMeters)
                { diffs.Add("box axis " + axis); }
            }
            int index = 0;
            foreach (DrawableStructureBuilder.Placement p in rebuilt.Placements)
            {
                DrawableGeometry g = p.Geometry;
                int stride = g.Layout.Stride;
                if (!SameBytes(original, (int)(g.VertexData - ResourceView.GraphicsBase), copy, p.VertexOffset, g.VertexCount * stride)) { diffs.Add("gfx vertices geometry " + index); }
                if (!SameBytes(original, (int)(g.IndexData - ResourceView.GraphicsBase), copy, p.IndexOffset, g.IndexCount * 2)) { diffs.Add("gfx indices geometry " + index); }
                index++;
            }
            return diffs;
        }

        private static bool SameBytes(RscResource a, int aOffset, RscResource b, int bOffset, int length)
        {
            if (aOffset + length > a.GraphicsSize || bOffset + length > b.GraphicsSize) { return false; }
            for (int i = 0; i < length; i++) { if (a.Body[a.SystemSize + aOffset + i] != b.Body[b.SystemSize + bOffset + i]) { return false; } }
            return true;
        }

        private static void Increment(Dictionary<string, int> counts, string key)
        {
            int value;
            counts.TryGetValue(key, out value);
            counts[key] = value + 1;
        }

        private static string Short(string message) { return message.Length > 100 ? message.Substring(0, 100) : message; }
    }
}
