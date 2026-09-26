using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Self-test groups for the structure writer (NEXT_SESSIONS session 4): DrawableStructureBuilder on synthetic drawables
    // (SyntheticDrawable), the round-trip comparison the PC runs on the game's files, bounds-record measurement, trimming,
    // and refusals. Synthetic files prove the writer agrees with the reader and with itself; only the round trip on the
    // game's own drawables (T031-drawable-roundtrip) proves it agrees with Rockstar's files.
    internal static class StructureSelfTest
    {
        private static SyntheticDrawable.Spec TwoLods()
        {
            SyntheticDrawable.Spec spec = new SyntheticDrawable.Spec { Textures = new[] { "lod_a", "lod_b" } };
            spec.Lods.Add(new List<List<Mesh>> { new List<Mesh> { SyntheticDrawable.Prism(12, 0.4f, 1, 0, 0), SyntheticDrawable.Prism(8, 0.2f, 0.5f, 1, 0.3f) } });
            spec.Lods.Add(new List<List<Mesh>> { new List<Mesh> { SyntheticDrawable.Prism(6, 0.4f, 1.5f, 0, 0.1f) } });
            spec.ShaderIndices = new List<int> { 0, 1, 0 };
            return spec;
        }

        internal static void RoundTrip(SelfTest.Runner t, string output)
        {
            foreach (DrawableStructureBuilder.GraphicsOrder order in new[] { DrawableStructureBuilder.GraphicsOrder.Interleaved, DrawableStructureBuilder.GraphicsOrder.VerticesFirst })
            {
                SyntheticDrawable.Spec spec = TwoLods();
                spec.Order = order;
                RscResource original = RscResource.Parse(SyntheticDrawable.Build(spec).Serialize());
                DrawableFile file = new DrawableFile(original);
                t.Check(file.Models.Count == 2 && file.Models[0].Geometries.Count == 2 && file.Shaders.Count == 2 && file.Models[0].Geometries[1].ShaderIndex == 1,
                    order + ": synthetic drawable reads back (2 LODs, 3 geometries, 2 shaders)");
                DrawableRoundTrip.Outcome outcome = DrawableRoundTrip.Check(original);
                t.Check(outcome.Skip == null && outcome.Multi, order + ": eligible multi-geometry drawable", outcome.Skip);
                t.Check(outcome.Identical.SequenceEqual(new[] { order }), order + ": identical under its own order only",
                    string.Join(",", outcome.Identical.Select(o => o.ToString()).ToArray()) + " " + string.Join("; ", outcome.Differences.Select(p => p.Key + ": " + string.Join(" ", p.Value.Take(4).ToArray())).ToArray()));
                t.Check(outcome.BoundsRecords.SequenceEqual(new[] { "3 records for 2 geometries", "1 records for 1 geometry" }), order + ": bounds records measured", string.Join(" | ", outcome.BoundsRecords.ToArray()));
            }
            // The comparison catches a changed byte: a rebuild with one vertex moved.
            SyntheticDrawable.Spec moved = TwoLods();
            RscResource source = RscResource.Parse(SyntheticDrawable.Build(moved).Serialize());
            DrawableFile template = new DrawableFile(source);
            DrawableStructureBuilder.Plan plan = DrawableStructureBuilder.Identity(template);
            plan.SinglePage = false; plan.ExactBounds = true;
            Mesh.Vertex v = plan.ModelMeshes[1][0].Vertices[0];
            v.U += 0.25f;
            plan.ModelMeshes[1][0].Vertices[0] = v;
            List<string> diffs = DrawableRoundTrip.Compare(source, template, DrawableStructureBuilder.Build(template, plan));
            t.Check(diffs.SequenceEqual(new[] { "gfx vertices geometry 2" }), "a changed UV is found in its geometry", string.Join(" ", diffs.ToArray()));
            plan = DrawableStructureBuilder.Identity(template);
            plan.SinglePage = false; plan.ExactBounds = true; plan.LodDistances = new[] { 31f, float.NaN, float.NaN, float.NaN };
            diffs = DrawableRoundTrip.Compare(source, template, DrawableStructureBuilder.Build(template, plan));
            t.Check(diffs.SequenceEqual(new[] { "sys+0x50" }), "a changed LOD distance is found in the system segment", string.Join(" ", diffs.ToArray()));
        }

        internal static void BoundsRecords(SelfTest.Runner t, string output)
        {
            foreach (int mode in new[] { SyntheticDrawable.OneRecord, SyntheticDrawable.PerGeometry, SyntheticDrawable.PerGeometryPlusModel })
            {
                SyntheticDrawable.Spec spec = TwoLods();
                spec.BoundsRecordsMode = mode;
                DrawableFile file = new DrawableFile(SyntheticDrawable.Build(spec));
                int expected = mode == SyntheticDrawable.OneRecord ? 1 : mode == SyntheticDrawable.PerGeometry ? 2 : 3;
                int measured = DrawableStructureBuilder.BoundsRecords(file, file.Models[0]);
                t.Check(measured == expected, "records mode " + mode + ": measured " + expected + " for two geometries", measured.ToString());
                t.Check(DrawableStructureBuilder.BoundsRecords(file, file.Models[1]) == 1, "records mode " + mode + ": one record for one geometry");
                t.Check(DrawableRoundTrip.Check(RscResource.Parse(SyntheticDrawable.Build(spec).Serialize())).Identical.Count == 1, "records mode " + mode + ": round trip identical");
            }
            SyntheticDrawable.Spec broken = TwoLods();
            broken.BreakFirstRecord = true;
            DrawableFile brokenFile = new DrawableFile(SyntheticDrawable.Build(broken));
            t.Check(DrawableStructureBuilder.BoundsRecords(brokenFile, brokenFile.Models[0]) == -1, "a zero-radius record: no layout fits");
            string reason = DrawableStructureBuilder.Unsupported(brokenFile);
            t.Check(reason != null && reason.Contains("not spheres"), "a template whose records are not spheres is refused", reason);
            SyntheticDrawable.Spec shared = TwoLods();
            shared.ShareFirstVertexBuffer = true;
            string sharedReason;
            try { sharedReason = DrawableStructureBuilder.Unsupported(new DrawableFile(SyntheticDrawable.Build(shared))); }
            catch (InvalidDataException error) { sharedReason = "reader: " + error.Message; }
            t.Check(sharedReason != null, "geometries sharing a vertex buffer are refused (by the reader or the writer)", sharedReason);
        }

        private static SyntheticDrawable.Spec FourLods()
        {
            SyntheticDrawable.Spec spec = new SyntheticDrawable.Spec { Textures = new[] { "old_a", "old_b", "old_c" } };
            for (int lod = 0; lod < 4; lod++)
            {
                spec.Lods.Add(new List<List<Mesh>>
                {
                    new List<Mesh> { SyntheticDrawable.Prism(16 >> Math.Min(lod, 2), 0.5f, 2, 0, lod), SyntheticDrawable.Prism(5, 0.1f, 0.3f, 2, lod) },
                    new List<Mesh> { SyntheticDrawable.Prism(4, 0.1f, 0.1f, 3, lod) },
                });
            }
            spec.ShaderIndices = Enumerable.Range(0, 4).SelectMany(l => new[] { 0, 1, 2 }).ToList();
            return spec;
        }

        internal static void Trimming(SelfTest.Runner t, string output)
        {
            DrawableFile template = new DrawableFile(SyntheticDrawable.Build(FourLods()));
            t.Check(template.Models.Count == 8, "template: 4 LODs x 2 models");
            Mesh a = SyntheticDrawable.Prism(24, 0.3f, 3, 0, 0.5f), b = SyntheticDrawable.Prism(10, 0.3f, 0.2f, 3, 0.5f), c = SyntheticDrawable.Prism(6, 0.3f, 3, 0, 0.2f);
            DrawableStructureBuilder.Plan plan = new DrawableStructureBuilder.Plan();
            // LOD 0: model 0 keeps both geometries, model 1 dropped; LOD 1: model 0 keeps geometry 0; LODs 2-3 cleared.
            plan.ModelMeshes.Add(new List<Mesh> { a, b }); plan.ModelMeshes.Add(new List<Mesh>());
            plan.ModelMeshes.Add(new List<Mesh> { c }); plan.ModelMeshes.Add(new List<Mesh>());
            for (int i = 0; i < 4; i++) { plan.ModelMeshes.Add(new List<Mesh>()); }
            plan.ShaderCount = 2;
            plan.ShaderTextures = new[] { "lf_new_texture_zero", "lf_b" };
            plan.LodDistances = new[] { 15f, 40f, float.NaN, float.NaN };
            DrawableStructureBuilder.Output built = DrawableStructureBuilder.Build(template, plan);
            DrawableFile back = new DrawableFile(RscResource.Parse(built.Resource.Serialize()));
            t.Check(back.Models.Count == 2 && back.Models[0].Lod == 0 && back.Models[1].Lod == 1, "two models remain (LOD 0 and LOD 1)", string.Join(",", back.Models.Select(m => m.Lod.ToString()).ToArray()));
            t.Check(back.Models[0].Geometries.Count == 2 && back.Models[1].Geometries.Count == 1, "geometry counts 2 and 1");
            t.Check(back.View.U32(back.Root + DrawableFile.DrawableLods + 8) == 0 && back.View.U32(back.Root + DrawableFile.DrawableLods + 12) == 0, "LOD slots 2 and 3 cleared");
            t.Check(back.Shaders.Count == 2 && back.Shaders[0].Textures[0] == "lf_new_texture_zero" && back.Shaders[1].Textures[0] == "lf_b", "two shaders kept and renamed (a longer name goes to the tail)",
                string.Join(",", back.Shaders.Select(s => s.Textures[0]).ToArray()));
            List<Mesh> sent = new List<Mesh> { a, b, c };
            List<Mesh> read = back.Models.SelectMany(m => m.Geometries).Select(back.ReadMesh).ToList();
            bool same = read.Count == 3 && Enumerable.Range(0, 3).All(i => SameMesh(sent[i], read[i]));
            t.Check(same, "every kept geometry reads back as sent");
            t.Check(back.View.F32(back.Root + 0x50) == 15f && back.View.F32(back.Root + 0x54) == 40f && back.View.F32(back.Root + 0x58) == 120f, "LOD distances written, others kept");
            float[] model = DrawableStructureBuilder.Sphere(new List<Mesh> { a, b });
            bool records = Enumerable.Range(0, 3).All(r => Math.Abs(back.View.F32(back.Models[0].Bounds + (uint)(r * 16) + 12) - model[3]) < 1e-6f);
            t.Check(records, "generated drawables write the model's sphere into every record (conservative)");
            RscResource resource = back.Resource;
            t.Check(((resource.Flags >> 15) & 0x7FF) == 1, "graphics segment is one page", "pages " + ((resource.Flags >> 15) & 0x7FF));
            List<Tuple<int, int>> ranges = built.Placements.SelectMany(p => new[] { Tuple.Create(p.VertexOffset, p.Mesh.Vertices.Count * 36), Tuple.Create(p.IndexOffset, p.Mesh.Indices.Count * 2) }).OrderBy(r => r.Item1).ToList();
            t.Check(ranges.All(r => r.Item1 % 16 == 0) && ranges.Zip(ranges.Skip(1), (x, y) => x.Item1 + x.Item2 <= y.Item1).All(ok => ok), "buffers 16-byte aligned and not overlapping");
            float[] box = DrawableStructureBuilder.Box(sent);
            t.Check(back.View.F32(back.Root + DrawableFile.DrawableMin + 8) == box[2] && back.View.F32(back.Root + DrawableFile.DrawableMax + 8) == box[5], "drawable box encloses every kept geometry");

            Func<Action, string> refused = action => { try { action(); return null; } catch (InvalidDataException error) { return error.Message; } };
            DrawableStructureBuilder.Plan early = new DrawableStructureBuilder.Plan();
            early.ModelMeshes.Add(new List<Mesh>()); early.ModelMeshes.Add(new List<Mesh> { a });
            for (int i = 0; i < 6; i++) { early.ModelMeshes.Add(new List<Mesh>()); }
            string message = refused(() => DrawableStructureBuilder.Build(template, early));
            t.Check(message != null && (message.Contains("only trailing") || message.Contains("nothing in LOD 0")), "a model kept after a dropped one is refused", message);
            DrawableStructureBuilder.Plan empty = new DrawableStructureBuilder.Plan();
            for (int i = 0; i < 8; i++) { empty.ModelMeshes.Add(i == 2 ? new List<Mesh> { a } : new List<Mesh>()); }
            message = refused(() => DrawableStructureBuilder.Build(template, empty));
            t.Check(message != null && message.Contains("nothing in LOD 0"), "keeping nothing in LOD 0 is refused", message);
            DrawableStructureBuilder.Plan many = DrawableStructureBuilder.Identity(template);
            many.ModelMeshes[1].Add(a);
            message = refused(() => DrawableStructureBuilder.Build(template, many));
            t.Check(message != null && message.Contains("geometries, the plan"), "more meshes than geometries is refused", message);
            DrawableStructureBuilder.Plan shaders = DrawableStructureBuilder.Identity(template);
            shaders.ShaderCount = 1;
            message = refused(() => DrawableStructureBuilder.Build(template, shaders));
            t.Check(message != null && message.Contains("shader"), "dropping a shader a kept geometry uses is refused", message);
        }

        // A game folder holding synthetic drawables in one unencrypted IMG (no IMG key: the probes' test setup).
        private static string FakeGame(params KeyValuePair<string, RscResource>[] drawables)
        {
            string game = Path.Combine(Path.GetTempPath(), "liberty-structure-" + Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(Path.Combine(game, "pc", "models", "cdimages"));
            File.WriteAllBytes(Path.Combine(game, "GTAIV.exe"), new byte[64]);
            ImgArchive.Write(Path.Combine(game, "pc", "models", "cdimages", "test.img"), drawables.Select(d => new KeyValuePair<string, byte[]>(d.Key, d.Value.Serialize())).ToList());
            return game;
        }

        private static AssetManifest StructureManifest(string template, string textureMode, string lodDistances)
        {
            return SelfTest.ParseManifest("{\"schemaVersion\":1,\"name\":\"lf_struct\",\"type\":\"prop\",\"source\":\"x.gltf\",\"template\":{\"archive\":\"pc/models/cdimages/test.img\",\"model\":\"aaa_single\"}," +
                "\"textureDictionary\":\"lf_struct\",\"drawDistanceMeters\":100,\"textureMode\":\"" + textureMode + "\",\"drawableWriter\":\"structure\"," +
                "\"structureTemplate\":{\"archive\":\"pc/models/cdimages/test.img\",\"model\":\"" + template + "\"}" + (lodDistances != null ? ",\"lodDistancesMeters\":[" + lodDistances + "]" : "") + "}");
        }

        internal static void Compiler(SelfTest.Runner t, string output)
        {
            SyntheticDrawable.Spec single = new SyntheticDrawable.Spec();
            single.Lods.Add(new List<List<Mesh>> { new List<Mesh> { SyntheticDrawable.Prism(6, 0.3f, 1, 0, 0) } });
            string game = FakeGame(new KeyValuePair<string, RscResource>("aaa_single.wdr", SyntheticDrawable.Build(single)),
                new KeyValuePair<string, RscResource>("bbb_lods.wdr", SyntheticDrawable.Build(FourLods())));
            try
            {
                // LOD 0: two materials; LOD 1: the first material again.
                ContentAsset asset = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("b", 0, 1, 0.5f), SelfTest.Box("c", 1, 0, 0.6f));
                AssetManifest manifest = StructureManifest("auto", "native", "20,50");
                t.Check(CompilerCapabilities.For(manifest) == CompilerCapabilities.Structure && CompilerCapabilities.For(StructureManifest("auto", "native", null)).CompiledLodLevels == 4,
                    "structure manifests validate against the structure capabilities");
                t.Check(AssetValidator.Validate(asset, manifest, CompilerCapabilities.For(manifest)).All(i => i.Severity != "error" && i.Code != "LCC025" && i.Code != "LCC037"),
                    "two materials and two LODs are valid for the structure writer (no LCC016, LCC025, LCC037)");
                t.Check(AssetValidator.Validate(asset, StructureManifest("auto", "template", null), CompilerCapabilities.Structure).Any(i => i.Code == "LCC038"), "structure writer without native textures: LCC038");

                PropCompiler.Result result = PropCompiler.Compile(game, manifest, asset);
                t.Check(result.DrawableWriter == "structure" && result.TemplateUsed == "pc/models/cdimages/test.img/bbb_lods", "auto picks the first drawable that fits (not the single-LOD one)", result.TemplateUsed);
                t.Check(result.Parts.Select(p => p.Lod + ":" + p.Material + ":" + p.TextureName).SequenceEqual(new[] { "0:0:lf_struct", "0:1:lf_struct_1", "1:0:lf_struct" }), "parts: LOD, material, texture",
                    string.Join(" ", result.Parts.Select(p => p.Lod + ":" + p.Material + ":" + p.TextureName).ToArray()));
                t.Check(result.Textures.Count == 2 && result.Textures.All(x => x.Format == "DXT1"), "one native texture per material");
                List<string> problems = Readback.Verify(result);
                t.Check(problems.Count == 0, "the structure build reads back", string.Join("; ", problems.ToArray()));
                DrawableFile back = new DrawableFile(RscResource.Parse(result.Drawable));
                t.Check(back.Models.Select(m => m.Lod).SequenceEqual(new[] { 0, 1 }) && back.Shaders.Count == 2, "LODs 2-3 and the second models dropped; two shaders kept");
                t.Check(back.View.F32(back.Root + 0x50) == 20f && back.View.F32(back.Root + 0x54) == 50f, "LOD distances written");
                t.Check(result.Notes.Any(n => n.Contains("builtin dictionary prototype")), "no template dictionary: builtin prototype, noted");

                string report = Program.ReportJson(manifest, asset, AssetValidator.Validate(asset, manifest, CompilerCapabilities.For(manifest)), result, problems, "ok");
                Dictionary<string, object> parsed = (Dictionary<string, object>)new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(report);
                Dictionary<string, object> compiled = (Dictionary<string, object>)parsed["compiled"];
                t.Check((string)compiled["drawableWriter"] == "structure" && Convert.ToInt32(compiled["lodCount"]) == 2 && ((System.Collections.IList)compiled["geometries"]).Count == 3 &&
                    ((System.Collections.IList)compiled["textures"]).Count == 2 && (string)((Dictionary<string, object>)parsed["capabilities"])["version"] == "v2-structure", "report.json: writer, LOD count, geometries, textures, capabilities");
                if (output != null) { File.WriteAllText(Path.Combine(output, "structure_report.json"), report); File.WriteAllBytes(Path.Combine(output, "structure.wdr"), result.Drawable); }

                string refused;
                try { PropCompiler.Compile(game, StructureManifest("aaa_single", "native", null), asset); refused = null; }
                catch (InvalidDataException error) { refused = error.Message; }
                t.Check(refused != null && refused.Contains("does not fit") && refused.Contains("LOD 0 has 1 geometries, the asset needs 2"), "an explicit template that does not fit is an error naming why", refused);

                ContentAsset crowded = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("b", 0, 1, 0.5f));
                crowded.Materials.Add(new ContentMaterial { Name = "third" });
                crowded.Meshes.Add(SelfTest.Box("c", 0, 2, 0.4f));
                string fallback;
                t.Check(StructureCompiler.Compile(game, manifest, crowded, out fallback) == null && fallback != null && fallback.Contains("no drawable"), "auto with no fitting template: no result, the reason for a fallback", fallback);
                try { PropCompiler.Compile(game, manifest, crowded); refused = null; }
                catch (InvalidDataException error) { refused = error.Message; }
                t.Check(refused != null && refused.Contains("v1 cannot write") && refused.Contains("LCC016"), "no template and v1 cannot build it either: a clear error, not a silent LOD 0", refused);
            }
            finally { try { Directory.Delete(game, true); } catch (IOException) { } }
        }

        // The two PC commands end to end on a synthetic game: `roundtrip` (both buffer orders found, report written) and the
        // structure facts `probe drawables` adds (eligibility, sphere records, buffer order, template shapes).
        internal static void Commands(SelfTest.Runner t, string output)
        {
            SyntheticDrawable.Spec verticesFirst = TwoLods();
            verticesFirst.Order = DrawableStructureBuilder.GraphicsOrder.VerticesFirst;
            SyntheticDrawable.Spec single = new SyntheticDrawable.Spec();
            single.Lods.Add(new List<List<Mesh>> { new List<Mesh> { SyntheticDrawable.Prism(6, 0.3f, 1, 0, 0) } });
            string game = FakeGame(new KeyValuePair<string, RscResource>("bbb_lods.wdr", SyntheticDrawable.Build(FourLods())),
                new KeyValuePair<string, RscResource>("ccc_vf.wdr", SyntheticDrawable.Build(verticesFirst)),
                new KeyValuePair<string, RscResource>("ddd_single.wdr", SyntheticDrawable.Build(single)));
            try
            {
                string roundtrip = Path.Combine(game, "roundtrip.json"), probe = Path.Combine(game, "drawables.json");
                t.Check(DrawableRoundTrip.Run(new[] { "--game", game, "--out", roundtrip }) == 0, "roundtrip exits 0 when every eligible drawable is identical");
                System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
                Dictionary<string, object> r = json.Deserialize<Dictionary<string, object>>(File.ReadAllText(roundtrip));
                Dictionary<string, object> orders = (Dictionary<string, object>)r["identicalByOrder"];
                t.Check(Convert.ToInt32(r["eligible"]) == 3 && Convert.ToInt32(r["multiGeometry"]) == 2 && Convert.ToInt32(r["failed"]) == 0, "roundtrip report: 3 eligible, 2 multi-geometry, none failed");
                t.Check(Convert.ToInt32(orders["Interleaved"]) == 1 && Convert.ToInt32(orders["VerticesFirst"]) == 1 && Convert.ToInt32(orders["both (one buffer pair)"]) == 1,
                    "roundtrip report: each order found once, the single-geometry file under both", string.Join(",", orders.Select(p => p.Key + "=" + p.Value).ToArray()));
                t.Check(DrawableRoundTrip.Run(new[] { "--game", game, "pc/models/cdimages/missing.img" }) == 1, "an archive that cannot be read proves nothing: exit 1");

                t.Check(Probe.Run(new[] { "drawables", "--game", game, "--out", probe }) == 0, "probe drawables exits 0");
                Dictionary<string, object> d = json.Deserialize<Dictionary<string, object>>(File.ReadAllText(probe));
                Dictionary<string, object> templates = (Dictionary<string, object>)d["structureTemplates"];
                t.Check(templates.ContainsKey("slots 0123, geometries 2/2/2/2, shaders gta_default") &&
                    ((System.Collections.ArrayList)templates["slots 0123, geometries 2/2/2/2, shaders gta_default"]).Contains("pc/models/cdimages/test.img/bbb_lods"),
                    "probe lists the four-LOD drawable as a structure template by shape", string.Join(" | ", templates.Keys.ToArray()));
                Dictionary<string, object> order = (Dictionary<string, object>)d["graphicsOrderMultiGeometry"];
                t.Check(Convert.ToInt32(order["Interleaved"]) == 1 && Convert.ToInt32(order["VerticesFirst"]) == 1, "probe classifies each file's buffer order");
                Dictionary<string, object> records = (Dictionary<string, object>)d["boundsRecordsMultiGeometry"];
                t.Check(records.Keys.SequenceEqual(new[] { "geometries + 1" }), "probe measures the sphere records of multi-geometry models", string.Join(",", records.Keys.ToArray()));
                t.Check(Convert.ToInt32(((Dictionary<string, object>)d["structureWriterEligibility"])["eligible"]) == 3, "probe counts drawables the structure writer can use");
            }
            finally { try { Directory.Delete(game, true); } catch (IOException) { } }
        }

        internal static bool SameMesh(Mesh a, Mesh b)
        {
            if (a.Vertices.Count != b.Vertices.Count || !a.Indices.SequenceEqual(b.Indices)) { return false; }
            for (int i = 0; i < a.Vertices.Count; i++)
            {
                Mesh.Vertex x = a.Vertices[i], y = b.Vertices[i];
                if (x.X != y.X || x.Y != y.Y || x.Z != y.Z || x.U != y.U || x.V != y.V || x.NX != y.NX || x.NY != y.NY || x.NZ != y.NZ || x.Colour != y.Colour) { return false; }
            }
            return true;
        }
    }
}
