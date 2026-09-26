using System;
using System.Collections.Generic;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // Writes a drawable with several geometries, shaders and LOD models by patching a game drawable whose structure is the
    // same or larger (a "structure template"). Every kept geometry gets new vertex and index data, counts and pointers.
    // Geometries, models and LOD slots the new drawable does not use are trimmed: their collection counts are shortened
    // (trailing entries only) or the LOD slot pointer is cleared, both fields whose meaning is documented. Structures are
    // never synthesised, because their full sizes are not established (docs/research/ModelFormat.md); the only system
    // bytes added are texture names that do not fit their slot, in the unused 0xCD tail as DrawableBuilder does.
    //
    // Choices the research has not established are named here and tested against the game's own drawables by
    // `LibertyContent roundtrip` (check T031-drawable-roundtrip), which rebuilds them byte for byte:
    //   GraphicsOrder     where the buffers of several geometries go. The round trip reports which candidate reproduces
    //                     the game's files; generated drawables use one page, where any order is addressable.
    //   bounds records    how many 16-byte spheres model +0x0C holds: 1, one per geometry, or one per geometry plus one for
    //                     the model. Measured per template by BoundsRecords: the records must be spheres enclosing their
    //                     geometry, never assumed from a count.
    //   drawable bounds   the box, centre and radius enclose every kept geometry of every LOD (conservative for culling).
    internal static class DrawableStructureBuilder
    {
        internal enum GraphicsOrder
        {
            Interleaved,    // geometry 0 vertices, geometry 0 indices, geometry 1 vertices, ...
            VerticesFirst,  // every geometry's vertices in order, then every geometry's indices
        }

        // Properties, not fields, for the settings only the content compiler assigns: LibertyModel compiles this file too,
        // with warnings as errors and CS0649 (field never assigned) on (tools/package-phase2.ps1).
        internal sealed class Plan
        {
            // One entry per template model, in DrawableFile.Models order (LOD 0 first): the meshes of the geometries to keep.
            // Fewer meshes than geometries drops the trailing geometries; none drops the model, which must then come after
            // every kept model of its LOD. A LOD whose models are all dropped has its slot cleared.
            internal readonly List<List<Mesh>> ModelMeshes = new List<List<Mesh>>();
            // Texture name to give each kept shader (index = shader index), or null to keep the template's name.
            internal string[] ShaderTextures { get; set; }
            // Shaders kept (the collection is shortened to this count); -1 keeps every shader.
            internal int ShaderCount = -1;
            // Drawable +0x50: one value per LOD slot; NaN (or a null array) keeps the template's value.
            internal float[] LodDistances { get; set; }
            // One graphics page (generated drawables; proven in game for one geometry) or Rockstar's page rules (round trip).
            internal bool SinglePage = true;
            internal GraphicsOrder Order = GraphicsOrder.Interleaved;
            // Round trip: each record gets its own geometry's sphere (and the model's). Generated drawables write the
            // whole model's sphere into every record, which encloses whatever each record stands for.
            internal bool ExactBounds { get; set; }
        }

        // A sphere record: centre, then radius.
        internal const int SphereRecordBytes = 16;
        // Containment slack when measuring a template's records: the game's exporter stores bounds before quantising
        // vertices (the single-geometry round trip needed the same 1 mm).
        internal const float BoundsToleranceMeters = 0.001f;
        // Largest radius accepted as a real record; anything above is not a prop's bounding sphere.
        private const float MaxRecordRadiusMeters = 100000f;

        // Every geometry of the drawable, in DrawableFile.Models order (LOD, model, geometry).
        internal static IEnumerable<DrawableGeometry> Geometries(DrawableFile file) { return file.Models.SelectMany(m => m.Geometries); }

        // The identity plan: every geometry keeps its own mesh (what the round trip rebuilds).
        internal static Plan Identity(DrawableFile file)
        {
            Plan plan = new Plan();
            foreach (DrawableModel model in file.Models) { plan.ModelMeshes.Add(model.Geometries.Select(file.ReadMesh).ToList()); }
            return plan;
        }

        // Why the builder cannot use this drawable as a template, or null.
        internal static string Unsupported(DrawableFile file)
        {
            foreach (DrawableModel model in file.Models)
            {
                foreach (DrawableGeometry g in model.Geometries)
                {
                    if (g.Layout.Mask != 0x59) { return "vertex layout " + g.Layout.Describe() + " is not position/normal/colour/uv"; }
                }
            }
            List<DrawableGeometry> all = Geometries(file).ToList();
            if (all.Select(g => g.VertexBuffer).Distinct().Count() != all.Count || all.Select(g => g.IndexBuffer).Distinct().Count() != all.Count)
            {
                return "geometries share vertex or index buffers";
            }
            foreach (DrawableModel model in file.Models)
            {
                if (model.Bounds != 0 && BoundsRecords(file, model) < 0) { return "model bounds are not spheres around its geometries"; }
            }
            return null;
        }

        // How many sphere records model +0x0C holds, measured against the model's own geometries: n + 1 (one per geometry
        // and one for the model) is tried first, then n, then 1. A record counts only when it is a finite sphere enclosing
        // what it would stand for, so bytes past the real array (pointers, other structures) do not pass. -1: none fits.
        internal static int BoundsRecords(DrawableFile file, DrawableModel model)
        {
            if (model.Bounds == 0) { return 0; }
            List<Mesh> meshes = model.Geometries.Select(file.ReadMesh).ToList();
            int n = meshes.Count;
            foreach (int records in n > 1 ? new[] { n + 1, n, 1 } : new[] { 1 })
            {
                bool fits = true;
                for (int i = 0; i < records && fits; i++)
                {
                    IEnumerable<Mesh> covered = records > 1 && i < n ? new[] { meshes[i] } : (IEnumerable<Mesh>)meshes;
                    fits = IsEnclosingSphere(file.View, model.Bounds + (uint)(i * SphereRecordBytes), covered);
                }
                if (fits) { return records; }
            }
            return -1;
        }

        private static bool IsEnclosingSphere(ResourceView view, uint address, IEnumerable<Mesh> meshes)
        {
            float x, y, z, radius;
            try { x = view.F32(address); y = view.F32(address + 4); z = view.F32(address + 8); radius = view.F32(address + 12); }
            catch (System.IO.InvalidDataException) { return false; } // the record would run past the system segment
            if (!Finite(x) || !Finite(y) || !Finite(z) || !(radius > 0) || radius > MaxRecordRadiusMeters) { return false; }
            float limit = radius + Math.Max(BoundsToleranceMeters, radius * 1e-4f);
            foreach (Mesh mesh in meshes)
            {
                foreach (Mesh.Vertex v in mesh.Vertices)
                {
                    double dx = v.X - x, dy = v.Y - y, dz = v.Z - z;
                    if (Math.Sqrt(dx * dx + dy * dy + dz * dz) > limit) { return false; }
                }
            }
            return true;
        }

        private static bool Finite(float value) { return !float.IsNaN(value) && !float.IsInfinity(value); }

        internal sealed class Output
        {
            internal RscResource Resource;
            // Bytes of the system segment the builder derived from vertices (drawable box, centre, radius, sphere records):
            // the round trip compares these with a tolerance instead of exactly.
            internal readonly HashSet<int> BoundsBytes = new HashSet<int>();
            // Kept geometries in write order with their graphics offsets.
            internal readonly List<Placement> Placements = new List<Placement>();
        }

        internal sealed class Placement
        {
            internal DrawableGeometry Geometry;
            internal Mesh Mesh;
            internal int VertexOffset, IndexOffset;
        }

        internal static Output Build(DrawableFile template, Plan plan)
        {
            string problem = Unsupported(template);
            if (problem != null) { throw ResourceView.Bad("template: " + problem); }
            if (plan.ModelMeshes.Count != template.Models.Count) { throw ResourceView.Bad("plan has " + plan.ModelMeshes.Count + " models, template " + template.Models.Count); }

            // Kept geometries, and which models and LODs survive.
            List<Placement> kept = new List<Placement>();
            int[] keptModelsPerLod = new int[4];
            bool[] droppedInLod = new bool[4];
            for (int m = 0; m < template.Models.Count; m++)
            {
                DrawableModel model = template.Models[m];
                List<Mesh> meshes = plan.ModelMeshes[m] ?? new List<Mesh>();
                if (meshes.Count > model.Geometries.Count) { throw ResourceView.Bad("model " + m + " has " + model.Geometries.Count + " geometries, the plan " + meshes.Count); }
                if (meshes.Count == 0) { droppedInLod[model.Lod] = true; continue; }
                if (droppedInLod[model.Lod]) { throw ResourceView.Bad("LOD " + model.Lod + ": a model is kept after a dropped one (only trailing models can be dropped)"); }
                keptModelsPerLod[model.Lod]++;
                for (int i = 0; i < meshes.Count; i++)
                {
                    meshes[i].Validate();
                    kept.Add(new Placement { Geometry = model.Geometries[i], Mesh = meshes[i] });
                }
            }
            if (keptModelsPerLod[0] == 0) { throw ResourceView.Bad("the plan keeps nothing in LOD 0"); }
            int shaderCount = plan.ShaderCount < 0 ? template.Shaders.Count : plan.ShaderCount;
            if (shaderCount > template.Shaders.Count) { throw ResourceView.Bad("plan keeps " + shaderCount + " shaders, template has " + template.Shaders.Count); }
            foreach (Placement p in kept)
            {
                if (template.Shaders.Count > 0 && p.Geometry.ShaderIndex >= shaderCount) { throw ResourceView.Bad("a kept geometry uses shader " + p.Geometry.ShaderIndex + " but only " + shaderCount + " are kept"); }
            }

            // Graphics segment.
            int stride = kept[0].Geometry.Layout.Stride;
            List<KeyValuePair<Placement, bool>> buffers = new List<KeyValuePair<Placement, bool>>(); // (geometry, isVertexBuffer)
            if (plan.Order == GraphicsOrder.Interleaved) { foreach (Placement p in kept) { buffers.Add(new KeyValuePair<Placement, bool>(p, true)); buffers.Add(new KeyValuePair<Placement, bool>(p, false)); } }
            else { foreach (Placement p in kept) { buffers.Add(new KeyValuePair<Placement, bool>(p, true)); } foreach (Placement p in kept) { buffers.Add(new KeyValuePair<Placement, bool>(p, false)); } }
            Func<KeyValuePair<Placement, bool>, int> size = b => b.Value ? b.Key.Mesh.Vertices.Count * stride : b.Key.Mesh.Indices.Count * 2;
            int shift = DrawableBuilder.ShiftFor(buffers.Max(size));
            if (plan.SinglePage)
            {
                int total = 0;
                foreach (KeyValuePair<Placement, bool> b in buffers) { total = Align(total, 16) + size(b); }
                shift = 0;
                while ((256 << shift) < total) { shift++; }
                if (shift > MaxPageShift) { throw ResourceView.Bad("graphics data (" + total + " bytes) does not fit one page"); }
            }
            int cursor = 0;
            foreach (KeyValuePair<Placement, bool> b in buffers)
            {
                int start = plan.SinglePage ? Align(cursor, 16) : DrawableBuilder.Place(cursor, size(b), shift);
                if (b.Value) { b.Key.VertexOffset = start; } else { b.Key.IndexOffset = start; }
                cursor = start + size(b);
            }
            int graphicsSize;
            uint flags = DrawableBuilder.EncodeGraphics(template.Resource.Flags, cursor, shift, out graphicsSize);

            ResourceView source = template.View;
            int systemSize = source.SystemSize;
            byte[] body = new byte[systemSize + graphicsSize];
            Buffer.BlockCopy(source.Body, 0, body, 0, systemSize);
            RscResource result = new RscResource { Type = template.Resource.Type, Flags = flags, Body = body };
            ResourceView view = new ResourceView(result);
            Output output = new Output { Resource = result };

            foreach (Placement p in kept)
            {
                DrawableGeometry g = p.Geometry;
                DrawableBuilder.WriteVertices(body, systemSize + p.VertexOffset, p.Mesh, g.Layout);
                for (int i = 0; i < p.Mesh.Indices.Count; i++) { PutU16(body, systemSize + p.IndexOffset + i * 2, p.Mesh.Indices[i]); }
                PutU32(body, view.Offset(g.Address + DrawableGeometry.GeometryIndexCount, 4), (uint)p.Mesh.Indices.Count);
                PutU32(body, view.Offset(g.Address + DrawableGeometry.GeometryFaceCount, 4), (uint)(p.Mesh.Indices.Count / 3));
                PutU16(body, view.Offset(g.Address + DrawableGeometry.GeometryVertexCount, 2), p.Mesh.Vertices.Count);
                PutU16(body, view.Offset(g.VertexBuffer + DrawableGeometry.VertexBufferCount, 2), p.Mesh.Vertices.Count);
                // Keep the template's convention for the first data pointer (some files leave it 0).
                uint vertexData = ResourceView.GraphicsBase + (uint)p.VertexOffset;
                if (source.U32(g.VertexBuffer + DrawableGeometry.VertexBufferData) != 0) { PutU32(body, view.Offset(g.VertexBuffer + DrawableGeometry.VertexBufferData, 4), vertexData); }
                PutU32(body, view.Offset(g.VertexBuffer + DrawableGeometry.VertexBufferData2, 4), vertexData);
                PutU32(body, view.Offset(g.IndexBuffer + DrawableGeometry.IndexBufferCount, 4), (uint)p.Mesh.Indices.Count);
                PutU32(body, view.Offset(g.IndexBuffer + DrawableGeometry.IndexBufferData, 4), ResourceView.GraphicsBase + (uint)p.IndexOffset);
                output.Placements.Add(p);
            }

            // Trimming: shorter geometry collections, shorter model collections, cleared LOD slots, fewer shaders.
            uint root = template.Root;
            for (int m = 0; m < template.Models.Count; m++)
            {
                int keep = (plan.ModelMeshes[m] ?? new List<Mesh>()).Count;
                DrawableModel model = template.Models[m];
                if (keep > 0 && keep < model.Geometries.Count) { PutU16(body, view.Offset(model.Address + DrawableFile.ModelGeometries + 4, 2), keep); }
            }
            for (int lod = 0; lod < 4; lod++)
            {
                uint slot = root + (uint)(DrawableFile.DrawableLods + lod * 4);
                uint collection = source.U32(slot);
                if (collection == 0) { continue; }
                if (keptModelsPerLod[lod] == 0) { PutU32(body, view.Offset(slot, 4), 0); continue; }
                if (keptModelsPerLod[lod] < source.U16(collection + 4)) { PutU16(body, view.Offset(collection + 4, 2), keptModelsPerLod[lod]); }
            }
            if (shaderCount < template.Shaders.Count)
            {
                uint group = source.U32(root + DrawableFile.DrawableShaderGroup);
                PutU16(body, view.Offset(group + DrawableFile.ShaderGroupShaders + 4, 2), shaderCount);
            }

            // Bounds.
            List<Mesh> all = kept.Select(p => p.Mesh).ToList();
            float[] box = Box(all);
            float cx = (box[0] + box[3]) / 2, cy = (box[1] + box[4]) / 2, cz = (box[2] + box[5]) / 2;
            PutVector3(body, view.Offset(root + DrawableFile.DrawableCenter, 12), cx, cy, cz);
            PutVector3(body, view.Offset(root + DrawableFile.DrawableMin, 12), box[0], box[1], box[2]);
            PutVector3(body, view.Offset(root + DrawableFile.DrawableMax, 12), box[3], box[4], box[5]);
            PutF32(body, view.Offset(root + DrawableFile.DrawableRadius, 4), all.Max(mesh => mesh.RadiusAround(cx, cy, cz)));
            foreach (int start in new[] { DrawableFile.DrawableCenter, DrawableFile.DrawableMin, DrawableFile.DrawableMax }) { for (int i = 0; i < 12; i++) { output.BoundsBytes.Add(start + i); } }
            for (int i = 0; i < 4; i++) { output.BoundsBytes.Add(DrawableFile.DrawableRadius + i); }
            for (int m = 0; m < template.Models.Count; m++)
            {
                DrawableModel model = template.Models[m];
                List<Mesh> meshes = plan.ModelMeshes[m] ?? new List<Mesh>();
                if (meshes.Count == 0 || model.Bounds == 0) { continue; }
                int records = BoundsRecords(template, model);
                for (int r = 0; r < records; r++)
                {
                    bool own = plan.ExactBounds && records > 1 && r < model.Geometries.Count;
                    if (own && r >= meshes.Count) { continue; } // a dropped geometry's record keeps the template's value
                    float[] sphere = Sphere(own ? new List<Mesh> { meshes[r] } : meshes);
                    int at = view.Offset(model.Bounds + (uint)(r * SphereRecordBytes), SphereRecordBytes);
                    PutVector3(body, at, sphere[0], sphere[1], sphere[2]);
                    PutF32(body, at + 12, sphere[3]);
                    for (int i = 0; i < SphereRecordBytes; i++) { output.BoundsBytes.Add(at + i); }
                }
            }

            if (plan.LodDistances != null)
            {
                for (int lod = 0; lod < 4 && lod < plan.LodDistances.Length; lod++)
                {
                    if (!float.IsNaN(plan.LodDistances[lod])) { PutF32(body, view.Offset(root + (uint)(DrawableLodDistances + lod * 4), 4), plan.LodDistances[lod]); }
                }
            }
            if (plan.ShaderTextures != null)
            {
                for (int s = 0; s < shaderCount && s < plan.ShaderTextures.Length; s++)
                {
                    if (plan.ShaderTextures[s] == null) { continue; }
                    if (template.Shaders[s].TextureNameSlots.Count != 1) { throw ResourceView.Bad("shader " + s + " has " + template.Shaders[s].TextureNameSlots.Count + " texture slots; one is renamed"); }
                    DrawableBuilder.RenameTexture(body, view, template.Shaders[s].TextureNameSlots[0], plan.ShaderTextures[s]);
                }
            }
            return output;
        }

        // Drawable +0x50: four floats, one per LOD slot (documented as the LOD distances; how the game uses them is what
        // the lod-review scenario shows).
        internal const int DrawableLodDistances = 0x50;
        // Page shift limit of the RSC flags field (4 bits): 256 << 15 = 8 MB.
        internal const int MaxPageShift = 15;

        // min xyz, max xyz over every vertex.
        internal static float[] Box(IEnumerable<Mesh> meshes)
        {
            float[] box = { float.MaxValue, float.MaxValue, float.MaxValue, float.MinValue, float.MinValue, float.MinValue };
            foreach (Mesh mesh in meshes)
            {
                float minX, minY, minZ, maxX, maxY, maxZ;
                mesh.Bounds(out minX, out minY, out minZ, out maxX, out maxY, out maxZ);
                box[0] = Math.Min(box[0], minX); box[1] = Math.Min(box[1], minY); box[2] = Math.Min(box[2], minZ);
                box[3] = Math.Max(box[3], maxX); box[4] = Math.Max(box[4], maxY); box[5] = Math.Max(box[5], maxZ);
            }
            return box;
        }

        // The box's centre and half diagonal, the sphere DrawableBuilder writes for one geometry.
        internal static float[] Sphere(IList<Mesh> meshes)
        {
            float[] b = Box(meshes);
            float cx = (b[0] + b[3]) / 2, cy = (b[1] + b[4]) / 2, cz = (b[2] + b[5]) / 2;
            float half = (float)Math.Sqrt((b[3] - cx) * (b[3] - cx) + (b[4] - cy) * (b[4] - cy) + (b[5] - cz) * (b[5] - cz));
            return new[] { cx, cy, cz, half };
        }

        private static int Align(int value, int alignment) { return (value + alignment - 1) / alignment * alignment; }
        private static void PutU32(byte[] b, int at, uint value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, at, 4); }
        private static void PutU16(byte[] b, int at, int value) { b[at] = (byte)value; b[at + 1] = (byte)(value >> 8); }
        private static void PutF32(byte[] b, int at, float value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, at, 4); }
        private static void PutVector3(byte[] b, int at, float x, float y, float z) { PutF32(b, at, x); PutF32(b, at + 4, y); PutF32(b, at + 8, z); }
    }
}
