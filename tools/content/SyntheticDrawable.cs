using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // A drawable built from scratch for the offline tests, with the field offsets DrawableFile reads
    // (docs/research/ModelFormat.md). It is a fixture shaped the way the reader expects, not a claim about how the game lays
    // out its files: structure sizes are just large enough for the documented fields. Each model's sphere records sit
    // directly before its geometry pointer array, so reading one record too many meets pointers, not a sphere.
    internal static class SyntheticDrawable
    {
        internal sealed class Spec
        {
            // LOD -> models -> one mesh per geometry.
            internal readonly List<List<List<Mesh>>> Lods = new List<List<List<Mesh>>>();
            // Shader index of each geometry, in reader order (LOD, model, geometry); null: geometry i of a model uses shader i.
            internal List<int> ShaderIndices;
            // Texture name of each shader (the shader count).
            internal string[] Textures = { "synthetic" };
            internal string ShaderName = "gta_default";
            // Sphere records per model with several geometries: 1, one per geometry, or one per geometry plus the model.
            internal int BoundsRecordsMode = PerGeometryPlusModel;
            internal DrawableStructureBuilder.GraphicsOrder Order = DrawableStructureBuilder.GraphicsOrder.Interleaved;
            internal float[] LodDistances = { 30, 60, 120, 240 };
            // Test switches: geometry 1 reuses geometry 0's vertex buffer; the first model's record 0 gets radius 0.
            internal bool ShareFirstVertexBuffer;
            internal bool BreakFirstRecord;
        }

        internal const int OneRecord = 0, PerGeometry = 1, PerGeometryPlusModel = 2;
        private const byte Fill = 0xCD;

        private sealed class Writer
        {
            internal readonly List<byte> Bytes = new List<byte>();

            internal uint Alloc(int size)
            {
                while (Bytes.Count % 16 != 0) { Bytes.Add(Fill); }
                uint address = ResourceView.SystemBase + (uint)Bytes.Count;
                Bytes.AddRange(new byte[size]);
                return address;
            }

            internal uint String(string text)
            {
                uint address = Alloc(0);
                Bytes.AddRange(Encoding.ASCII.GetBytes(text + "\0"));
                while (Bytes.Count % 16 != 0) { Bytes.Add(Fill); }
                return address;
            }

            private int At(uint address) { return (int)(address - ResourceView.SystemBase); }
            internal void U32(uint address, uint value) { byte[] b = BitConverter.GetBytes(value); for (int i = 0; i < 4; i++) { Bytes[At(address) + i] = b[i]; } }
            internal void U16(uint address, int value) { Bytes[At(address)] = (byte)value; Bytes[At(address) + 1] = (byte)(value >> 8); }
            internal void U8(uint address, int value) { Bytes[At(address)] = (byte)value; }
            internal void U64(uint address, ulong value) { byte[] b = BitConverter.GetBytes(value); for (int i = 0; i < 8; i++) { Bytes[At(address) + i] = b[i]; } }
            internal void F32(uint address, float value) { U32(address, BitConverter.ToUInt32(BitConverter.GetBytes(value), 0)); }
        }

        internal static RscResource Build(Spec spec)
        {
            List<Mesh> geometries = spec.Lods.SelectMany(l => l.SelectMany(m => m)).ToList();
            const int stride = 36;
            // Graphics layout with Rockstar's page rules (DrawableBuilder.Place), so the round trip can reproduce it.
            List<KeyValuePair<int, bool>> buffers = new List<KeyValuePair<int, bool>>();
            if (spec.Order == DrawableStructureBuilder.GraphicsOrder.Interleaved) { for (int i = 0; i < geometries.Count; i++) { buffers.Add(new KeyValuePair<int, bool>(i, true)); buffers.Add(new KeyValuePair<int, bool>(i, false)); } }
            else { for (int i = 0; i < geometries.Count; i++) { buffers.Add(new KeyValuePair<int, bool>(i, true)); } for (int i = 0; i < geometries.Count; i++) { buffers.Add(new KeyValuePair<int, bool>(i, false)); } }
            Func<KeyValuePair<int, bool>, int> size = b => b.Value ? geometries[b.Key].Vertices.Count * stride : geometries[b.Key].Indices.Count * 2;
            int shift = DrawableBuilder.ShiftFor(buffers.Max(size));
            int[] vertexAt = new int[geometries.Count], indexAt = new int[geometries.Count];
            int cursor = 0;
            foreach (KeyValuePair<int, bool> b in buffers)
            {
                int start = DrawableBuilder.Place(cursor, size(b), shift);
                if (b.Value) { vertexAt[b.Key] = start; } else { indexAt[b.Key] = start; }
                cursor = start + size(b);
            }

            Writer w = new Writer();
            uint root = w.Alloc(0x80);
            uint group = w.Alloc(0x10);
            w.U32(root + DrawableFile.DrawableShaderGroup, group);
            uint shaderArray = w.Alloc(4 * spec.Textures.Length);
            w.U32(group + DrawableFile.ShaderGroupShaders, shaderArray);
            w.U16(group + DrawableFile.ShaderGroupShaders + 4, spec.Textures.Length);
            w.U16(group + DrawableFile.ShaderGroupShaders + 6, spec.Textures.Length);
            for (int s = 0; s < spec.Textures.Length; s++)
            {
                uint shader = w.Alloc(0x50);
                w.U32(shaderArray + (uint)(s * 4), shader);
                uint parameters = w.Alloc(4), types = w.Alloc(4), reference = w.Alloc(0x20);
                w.U32(shader + DrawableFile.ShaderParams, parameters);
                w.U32(shader + DrawableFile.ShaderParamCount, 1);
                w.U32(shader + DrawableFile.ShaderParamTypes, types);
                w.U8(types, 0); // 0 = texture
                w.U32(parameters, reference);
                w.U32(reference + DrawableFile.TextureRefName, w.String(spec.Textures[s]));
                w.U32(shader + DrawableFile.ShaderName, w.String(spec.ShaderName));
                w.U32(shader + DrawableFile.ShaderPreset, w.String(spec.ShaderName + ".sps"));
            }
            uint declaration = w.Alloc(0x10);
            w.U32(declaration, 0x59);                     // position, normal, colour, texcoord 0
            w.U8(declaration + 4, stride);
            w.U8(declaration + 7, 4);
            w.U64(declaration + 8, 6ul | 6ul << 12 | 9ul << 16 | 5ul << 24); // float3, float3, 4 bytes, float2

            int geometryIndex = 0;
            uint firstVertexBuffer = 0;
            bool firstRecord = true;
            for (int lod = 0; lod < spec.Lods.Count; lod++)
            {
                List<List<Mesh>> models = spec.Lods[lod];
                if (models.Count == 0) { continue; }
                uint collection = w.Alloc(8), modelArray = w.Alloc(4 * models.Count);
                w.U32(root + (uint)(DrawableFile.DrawableLods + lod * 4), collection);
                w.U32(collection, modelArray);
                w.U16(collection + 4, models.Count);
                w.U16(collection + 6, models.Count);
                for (int m = 0; m < models.Count; m++)
                {
                    List<Mesh> meshes = models[m];
                    int n = meshes.Count;
                    uint model = w.Alloc(0x20);
                    w.U32(modelArray + (uint)(m * 4), model);
                    int records = n == 1 || spec.BoundsRecordsMode == OneRecord ? 1 : spec.BoundsRecordsMode == PerGeometry ? n : n + 1;
                    uint bounds = w.Alloc(records * DrawableStructureBuilder.SphereRecordBytes);
                    uint geometryArray = w.Alloc(4 * n), mapping = w.Alloc(2 * n);
                    w.U32(model + DrawableFile.ModelGeometries, geometryArray);
                    w.U16(model + DrawableFile.ModelGeometries + 4, n);
                    w.U16(model + DrawableFile.ModelGeometries + 6, n);
                    w.U32(model + DrawableFile.ModelBounds, bounds);
                    w.U32(model + DrawableFile.ModelShaderMapping, mapping);
                    for (int r = 0; r < records; r++)
                    {
                        float[] sphere = DrawableStructureBuilder.Sphere(records > 1 && r < n ? new List<Mesh> { meshes[r] } : meshes);
                        uint at = bounds + (uint)(r * DrawableStructureBuilder.SphereRecordBytes);
                        w.F32(at, sphere[0]); w.F32(at + 4, sphere[1]); w.F32(at + 8, sphere[2]);
                        w.F32(at + 12, spec.BreakFirstRecord && firstRecord && r == 0 ? 0 : sphere[3]);
                    }
                    firstRecord = false;
                    for (int i = 0; i < n; i++, geometryIndex++)
                    {
                        Mesh mesh = meshes[i];
                        uint geometry = w.Alloc(0x40), indexBuffer = w.Alloc(0x10);
                        uint vertexBuffer = spec.ShareFirstVertexBuffer && geometryIndex == 1 ? firstVertexBuffer : w.Alloc(0x20);
                        if (geometryIndex == 0) { firstVertexBuffer = vertexBuffer; }
                        w.U32(geometryArray + (uint)(i * 4), geometry);
                        w.U16(mapping + (uint)(i * 2), spec.ShaderIndices != null ? spec.ShaderIndices[geometryIndex] : i);
                        w.U32(geometry + DrawableGeometry.GeometryVertexBuffer, vertexBuffer);
                        w.U32(geometry + DrawableGeometry.GeometryIndexBuffer, indexBuffer);
                        w.U32(geometry + DrawableGeometry.GeometryIndexCount, (uint)mesh.Indices.Count);
                        w.U32(geometry + DrawableGeometry.GeometryFaceCount, (uint)(mesh.Indices.Count / 3));
                        w.U16(geometry + DrawableGeometry.GeometryVertexCount, mesh.Vertices.Count);
                        w.U16(geometry + DrawableGeometry.GeometryPrimitive, 3);
                        w.U16(geometry + DrawableGeometry.GeometryStride, stride);
                        if (!(spec.ShareFirstVertexBuffer && geometryIndex == 1))
                        {
                            w.U16(vertexBuffer + DrawableGeometry.VertexBufferCount, mesh.Vertices.Count);
                            w.U32(vertexBuffer + DrawableGeometry.VertexBufferData, ResourceView.GraphicsBase + (uint)vertexAt[geometryIndex]);
                            w.U32(vertexBuffer + DrawableGeometry.VertexBufferStride, stride);
                            w.U32(vertexBuffer + DrawableGeometry.VertexBufferDeclaration, declaration);
                            w.U32(vertexBuffer + DrawableGeometry.VertexBufferData2, ResourceView.GraphicsBase + (uint)vertexAt[geometryIndex]);
                        }
                        w.U32(indexBuffer + DrawableGeometry.IndexBufferCount, (uint)mesh.Indices.Count);
                        w.U32(indexBuffer + DrawableGeometry.IndexBufferData, ResourceView.GraphicsBase + (uint)indexAt[geometryIndex]);
                    }
                }
            }
            for (int lod = 0; lod < 4 && lod < spec.LodDistances.Length; lod++) { w.F32(root + (uint)(DrawableStructureBuilder.DrawableLodDistances + lod * 4), spec.LodDistances[lod]); }
            float[] box = DrawableStructureBuilder.Box(geometries);
            float cx = (box[0] + box[3]) / 2, cy = (box[1] + box[4]) / 2, cz = (box[2] + box[5]) / 2;
            w.F32(root + DrawableFile.DrawableCenter, cx); w.F32(root + DrawableFile.DrawableCenter + 4, cy); w.F32(root + DrawableFile.DrawableCenter + 8, cz);
            for (int axis = 0; axis < 3; axis++)
            {
                w.F32(root + DrawableFile.DrawableMin + (uint)(axis * 4), box[axis]);
                w.F32(root + DrawableFile.DrawableMax + (uint)(axis * 4), box[axis + 3]);
            }
            w.F32(root + DrawableFile.DrawableRadius, geometries.Max(g => g.RadiusAround(cx, cy, cz)));

            while (w.Bytes.Count % 256 != 0) { w.Bytes.Add(Fill); }
            int systemSize = w.Bytes.Count;
            int graphicsSize;
            uint flags = DrawableBuilder.EncodeGraphics((uint)(systemSize / 256), cursor, shift, out graphicsSize);
            byte[] body = new byte[systemSize + graphicsSize];
            w.Bytes.CopyTo(body);
            for (int i = 0; i < geometries.Count; i++)
            {
                DrawableBuilder.WriteVertices(body, systemSize + vertexAt[i], geometries[i], new VertexLayout(0x59, stride, 4, 6ul | 6ul << 12 | 9ul << 16 | 5ul << 24));
                for (int k = 0; k < geometries[i].Indices.Count; k++)
                {
                    body[systemSize + indexAt[i] + k * 2] = (byte)geometries[i].Indices[k];
                    body[systemSize + indexAt[i] + k * 2 + 1] = (byte)(geometries[i].Indices[k] >> 8);
                }
            }
            return new RscResource { Type = DrawableFile.DrawableType, Flags = flags, Body = body };
        }

        // A closed prism: `sides` around, radius and height, standing on z0, with UVs; distinct per call (seed shifts it).
        internal static Mesh Prism(int sides, float radius, float height, float z0, float seed)
        {
            Mesh mesh = new Mesh();
            for (int ring = 0; ring < 2; ring++)
            {
                for (int i = 0; i < sides; i++)
                {
                    double angle = 2 * Math.PI * i / sides + seed;
                    float nx = (float)Math.Cos(angle), ny = (float)Math.Sin(angle);
                    mesh.Vertices.Add(new Mesh.Vertex { X = radius * nx, Y = radius * ny, Z = z0 + ring * height, NX = nx, NY = ny, U = (float)i / sides, V = ring, Colour = 0xFF808080u + (uint)i });
                }
            }
            for (int i = 0; i < sides; i++)
            {
                int j = (i + 1) % sides;
                mesh.Indices.AddRange(new[] { i, j, sides + j, i, sides + j, sides + i });
            }
            for (int i = 1; i + 1 < sides; i++) { mesh.Indices.AddRange(new[] { sides, sides + i, sides + i + 1 }); mesh.Indices.AddRange(new[] { 0, i + 1, i }); }
            return mesh;
        }
    }
}
