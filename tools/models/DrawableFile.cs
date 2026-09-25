using System;
using System.Collections.Generic;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // Reader for a GTA IV drawable (.wdr, RSC type 110): walks drawable -> LOD model collections -> models ->
    // geometries -> vertex/index buffers, and shader group -> shaders -> texture references. Only the fields the
    // tool needs are decoded; every structure is validated against the others (counts, strides, pointer ranges).
    internal sealed class DrawableFile
    {
        internal const uint DrawableType = 110;
        internal const int DrawableShaderGroup = 0x08, DrawableSkeleton = 0x0C, DrawableCenter = 0x10, DrawableMin = 0x20,
            DrawableMax = 0x30, DrawableLods = 0x40, DrawableRadius = 0x70;
        internal const int ModelGeometries = 0x04, ModelBounds = 0x0C, ModelShaderMapping = 0x10;
        internal const int ShaderGroupTextures = 0x04, ShaderGroupShaders = 0x08;
        internal const int ShaderParams = 0x14, ShaderParamCount = 0x1C, ShaderParamTypes = 0x24, ShaderName = 0x44, ShaderPreset = 0x48;
        internal const int TextureRefName = 0x14;

        internal readonly RscResource Resource;
        internal readonly ResourceView View;
        internal readonly uint Root = ResourceView.SystemBase;
        internal readonly List<DrawableModel> Models = new List<DrawableModel>();
        internal readonly List<DrawableShader> Shaders = new List<DrawableShader>();
        internal uint Skeleton;
        internal uint EmbeddedTextures;

        internal DrawableFile(RscResource resource)
        {
            if (resource.Type != DrawableType) { throw ResourceView.Bad("RSC type " + resource.Type + " is not a drawable"); }
            Resource = resource;
            View = new ResourceView(resource);
            ReadShaders();
            Skeleton = View.U32(Root + DrawableSkeleton);
            for (int lod = 0; lod < 4; lod++)
            {
                uint collection = View.U32(Root + (uint)(DrawableLods + lod * 4));
                if (collection == 0) { continue; }
                uint array = View.U32(collection);
                int count = View.U16(collection + 4);
                for (int i = 0; i < count; i++) { Models.Add(ReadModel(View.U32(array + (uint)(i * 4)), lod)); }
            }
            if (Models.Count == 0) { throw ResourceView.Bad("no models"); }
        }

        private void ReadShaders()
        {
            uint group = View.U32(Root + DrawableShaderGroup);
            if (group == 0) { return; }
            EmbeddedTextures = View.U32(group + ShaderGroupTextures);
            uint array = View.U32(group + ShaderGroupShaders);
            int count = View.U16(group + ShaderGroupShaders + 4);
            for (int i = 0; i < count; i++)
            {
                uint address = View.U32(array + (uint)(i * 4));
                DrawableShader shader = new DrawableShader();
                shader.Address = address;
                shader.Name = View.CString(View.U32(address + ShaderName));
                shader.Preset = View.CString(View.U32(address + ShaderPreset));
                uint parameters = View.U32(address + ShaderParams);
                uint types = View.U32(address + ShaderParamTypes);
                int parameterCount = (int)View.U32(address + ShaderParamCount);
                if (parameterCount < 0 || parameterCount > 64) { throw ResourceView.Bad("shader parameter count " + parameterCount); }
                for (int p = 0; p < parameterCount; p++)
                {
                    if (View.U8(types + (uint)p) != 0) { continue; }
                    uint reference = View.U32(parameters + (uint)(p * 4));
                    if (reference == 0) { continue; }
                    uint namePointer = View.U32(reference + TextureRefName);
                    shader.TextureNameSlots.Add(reference + TextureRefName);
                    shader.Textures.Add(View.CString(namePointer));
                }
                Shaders.Add(shader);
            }
        }

        private DrawableModel ReadModel(uint address, int lod)
        {
            DrawableModel model = new DrawableModel();
            model.Address = address;
            model.Lod = lod;
            uint geometries = View.U32(address + ModelGeometries);
            int count = View.U16(address + ModelGeometries + 4);
            uint mapping = View.U32(address + ModelShaderMapping);
            model.Bounds = View.U32(address + ModelBounds);
            for (int i = 0; i < count; i++)
            {
                DrawableGeometry geometry = ReadGeometry(View.U32(geometries + (uint)(i * 4)));
                geometry.ShaderIndex = mapping == 0 ? 0 : View.U16(mapping + (uint)(i * 2));
                if (geometry.ShaderIndex >= Shaders.Count && Shaders.Count > 0) { throw ResourceView.Bad("geometry uses shader " + geometry.ShaderIndex + " of " + Shaders.Count); }
                model.Geometries.Add(geometry);
            }
            return model;
        }

        private DrawableGeometry ReadGeometry(uint address)
        {
            DrawableGeometry g = new DrawableGeometry();
            g.Address = address;
            g.VertexBuffer = View.U32(address + DrawableGeometry.GeometryVertexBuffer);
            g.IndexBuffer = View.U32(address + DrawableGeometry.GeometryIndexBuffer);
            g.IndexCount = (int)View.U32(address + DrawableGeometry.GeometryIndexCount);
            g.FaceCount = (int)View.U32(address + DrawableGeometry.GeometryFaceCount);
            g.VertexCount = View.U16(address + DrawableGeometry.GeometryVertexCount);
            g.PrimitiveType = View.U16(address + DrawableGeometry.GeometryPrimitive);
            g.BoneCount = View.U16(address + DrawableGeometry.GeometryBoneCount);
            int stride = View.U16(address + DrawableGeometry.GeometryStride);

            uint declaration = View.U32(g.VertexBuffer + DrawableGeometry.VertexBufferDeclaration);
            g.Layout = new VertexLayout(View.U32(declaration), View.U8(declaration + 4), View.U8(declaration + 7), View.U64(declaration + 8));
            // Some files (e.g. the weapon models) leave the first data pointer 0 and set only the second.
            g.VertexData = View.U32(g.VertexBuffer + DrawableGeometry.VertexBufferData);
            if (g.VertexData == 0) { g.VertexData = View.U32(g.VertexBuffer + DrawableGeometry.VertexBufferData2); }
            g.IndexData = View.U32(g.IndexBuffer + DrawableGeometry.IndexBufferData);

            // The vertex buffer's count is authoritative: the installed weapon pack's w_m4 stores 0 in the geometry field
            // and still renders. The builder always writes both.
            int bufferCount = View.U16(g.VertexBuffer + DrawableGeometry.VertexBufferCount);
            if (g.VertexCount == 0) { g.VertexCount = bufferCount; }
            if (bufferCount != g.VertexCount) { throw ResourceView.Bad("vertex buffer count " + bufferCount + " differs from geometry " + g.VertexCount); }
            if (View.U32(g.VertexBuffer + DrawableGeometry.VertexBufferStride) != stride || g.Layout.Stride != stride) { throw ResourceView.Bad("stride mismatch"); }
            int indexBufferCount = (int)View.U32(g.IndexBuffer + DrawableGeometry.IndexBufferCount);
            if (g.IndexCount == 0 && g.FaceCount == 0) { g.IndexCount = indexBufferCount; g.FaceCount = indexBufferCount / 3; }
            if (indexBufferCount != g.IndexCount) { throw ResourceView.Bad("index buffer count " + indexBufferCount + " differs from geometry " + g.IndexCount); }
            if (g.PrimitiveType != 3 || g.FaceCount * 3 != g.IndexCount) { throw ResourceView.Bad("not a triangle list: primitive " + g.PrimitiveType + " faces " + g.FaceCount + " indices " + g.IndexCount); }
            if (!ResourceView.IsGraphics(g.VertexData) || !ResourceView.IsGraphics(g.IndexData)) { throw ResourceView.Bad("buffer data outside graphics segment: vb=0x" + g.VertexData.ToString("X8") + " ib=0x" + g.IndexData.ToString("X8") + " vbuf=0x" + g.VertexBuffer.ToString("X8")); }
            View.Offset(g.VertexData, g.VertexCount * stride);
            View.Offset(g.IndexData, g.IndexCount * 2);
            return g;
        }

        internal Mesh ReadMesh(DrawableGeometry g)
        {
            Mesh mesh = new Mesh();
            int stride = g.Layout.Stride;
            int vertexBase = View.Offset(g.VertexData, g.VertexCount * stride);
            int position = g.Layout.OffsetOf((int)VertexLayout.Semantic.Position);
            int normal = g.Layout.Has(VertexLayout.Semantic.Normal) ? g.Layout.OffsetOf((int)VertexLayout.Semantic.Normal) : -1;
            int colour = g.Layout.Has(VertexLayout.Semantic.Colour) ? g.Layout.OffsetOf((int)VertexLayout.Semantic.Colour) : -1;
            int uv = g.Layout.Has(VertexLayout.Semantic.TexCoord0) && g.Layout.SizeOf((int)VertexLayout.Semantic.TexCoord0) == 8
                ? g.Layout.OffsetOf((int)VertexLayout.Semantic.TexCoord0) : -1;
            if (position < 0 || g.Layout.SizeOf(0) != 12) { throw ResourceView.Bad("position is not float3"); }
            byte[] body = View.Body;
            for (int v = 0; v < g.VertexCount; v++)
            {
                int at = vertexBase + v * stride;
                Mesh.Vertex vertex = new Mesh.Vertex();
                vertex.X = BitConverter.ToSingle(body, at + position);
                vertex.Y = BitConverter.ToSingle(body, at + position + 4);
                vertex.Z = BitConverter.ToSingle(body, at + position + 8);
                if (normal >= 0 && g.Layout.SizeOf(3) == 12)
                {
                    vertex.NX = BitConverter.ToSingle(body, at + normal);
                    vertex.NY = BitConverter.ToSingle(body, at + normal + 4);
                    vertex.NZ = BitConverter.ToSingle(body, at + normal + 8);
                }
                vertex.Colour = colour >= 0 ? BitConverter.ToUInt32(body, at + colour) : 0xFFFFFFFF;
                if (uv >= 0)
                {
                    vertex.U = BitConverter.ToSingle(body, at + uv);
                    vertex.V = BitConverter.ToSingle(body, at + uv + 4);
                }
                mesh.Vertices.Add(vertex);
            }
            int indexBase = View.Offset(g.IndexData, g.IndexCount * 2);
            for (int i = 0; i < g.IndexCount; i++)
            {
                int index = BitConverter.ToUInt16(body, indexBase + i * 2);
                if (index >= g.VertexCount) { throw ResourceView.Bad("index " + index + " >= vertex count " + g.VertexCount); }
                mesh.Indices.Add(index);
            }
            return mesh;
        }
    }
}
