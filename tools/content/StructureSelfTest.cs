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
