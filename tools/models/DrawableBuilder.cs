using System;
using System.Text;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // Writes a new drawable by patching a game template that has one model, one geometry and one shader
    // (e.g. a hand-held prop drawn with gta_default). The system segment keeps the template's structures
    // (shader, skeleton, declaration) and only has counts, pointers, bounds and the texture name rewritten;
    // the graphics segment is rebuilt from the new mesh: vertices at offset 0, indices after them (16-byte aligned).
    internal static class DrawableBuilder
    {
        // Generated models keep their whole graphics segment in ONE page (verified in game 2026-09-25: a strap written as
        // 12 x 2 KB pages rendered vertices past the first block as garbage; the same mesh in one 32 KB page rendered
        // correctly). The multi-page placement below reproduces Rockstar's files and is used only by the round-trip selftest.
        internal static bool SinglePage = true;

        internal static RscResource Build(DrawableFile template, Mesh mesh, string textureName)
        {
            mesh.Validate();
            if (template.Models.Count != 1 || template.Models[0].Geometries.Count != 1) { throw ResourceView.Bad("template must have one model with one geometry"); }
            if (template.Shaders.Count != 1 || template.Shaders[0].TextureNameSlots.Count != 1) { throw ResourceView.Bad("template must have one shader with one texture"); }
            DrawableGeometry geometry = template.Models[0].Geometries[0];
            VertexLayout layout = geometry.Layout;
            if (layout.Mask != 0x59) { throw ResourceView.Bad("template vertex layout " + layout.Describe() + " is not position/normal/colour/uv"); }

            ResourceView source = template.View;
            int systemSize = source.SystemSize;
            int vertexBytes = mesh.Vertices.Count * layout.Stride;
            int indexBytes = mesh.Indices.Count * 2;
            int shift = ShiftFor(Math.Max(vertexBytes, indexBytes));
            if (SinglePage) { shift = 0; while ((256 << shift) < Align(vertexBytes, 16) + indexBytes) { shift++; } }
            int indexStart = SinglePage ? Align(vertexBytes, 16) : Place(vertexBytes, indexBytes, shift);
            int graphicsUsed = indexStart + indexBytes;
            int graphicsSize;
            uint flags = EncodeGraphics(template.Resource.Flags, graphicsUsed, shift, out graphicsSize);

            byte[] body = new byte[systemSize + graphicsSize];
            Buffer.BlockCopy(source.Body, 0, body, 0, systemSize);
            WriteVertices(body, systemSize, mesh, layout);
            for (int i = 0; i < mesh.Indices.Count; i++) { PutU16(body, systemSize + indexStart + i * 2, mesh.Indices[i]); }

            RscResource result = new RscResource();
            result.Type = template.Resource.Type;
            result.Flags = flags;
            result.Body = body;
            ResourceView view = new ResourceView(result);

            // Counts and buffer pointers.
            PutU32(body, view.Offset(geometry.Address + DrawableGeometry.GeometryIndexCount, 4), (uint)mesh.Indices.Count);
            PutU32(body, view.Offset(geometry.Address + DrawableGeometry.GeometryFaceCount, 4), (uint)(mesh.Indices.Count / 3));
            PutU16(body, view.Offset(geometry.Address + DrawableGeometry.GeometryVertexCount, 2), mesh.Vertices.Count);
            PutU16(body, view.Offset(geometry.VertexBuffer + DrawableGeometry.VertexBufferCount, 2), mesh.Vertices.Count);
            // Keep the template's convention for the first data pointer (some files leave it 0).
            if (source.U32(geometry.VertexBuffer + DrawableGeometry.VertexBufferData) != 0)
                { PutU32(body, view.Offset(geometry.VertexBuffer + DrawableGeometry.VertexBufferData, 4), ResourceView.GraphicsBase); }
            PutU32(body, view.Offset(geometry.VertexBuffer + DrawableGeometry.VertexBufferData2, 4), ResourceView.GraphicsBase);
            PutU32(body, view.Offset(geometry.IndexBuffer + DrawableGeometry.IndexBufferCount, 4), (uint)mesh.Indices.Count);
            PutU32(body, view.Offset(geometry.IndexBuffer + DrawableGeometry.IndexBufferData, 4), ResourceView.GraphicsBase + (uint)indexStart);

            // Bounds: drawable box, centre and vertex radius; model bounding sphere (centre + half diagonal).
            float minX, minY, minZ, maxX, maxY, maxZ;
            mesh.Bounds(out minX, out minY, out minZ, out maxX, out maxY, out maxZ);
            float cx = (minX + maxX) / 2, cy = (minY + maxY) / 2, cz = (minZ + maxZ) / 2;
            float halfDiagonal = (float)Math.Sqrt((maxX - cx) * (maxX - cx) + (maxY - cy) * (maxY - cy) + (maxZ - cz) * (maxZ - cz));
            uint root = template.Root;
            PutVector3(body, view.Offset(root + DrawableFile.DrawableCenter, 12), cx, cy, cz);
            PutVector3(body, view.Offset(root + DrawableFile.DrawableMin, 12), minX, minY, minZ);
            PutVector3(body, view.Offset(root + DrawableFile.DrawableMax, 12), maxX, maxY, maxZ);
            PutF32(body, view.Offset(root + DrawableFile.DrawableRadius, 4), mesh.RadiusAround(cx, cy, cz));
            uint sphere = template.Models[0].Bounds;
            if (sphere != 0)
            {
                PutVector3(body, view.Offset(sphere, 12), cx, cy, cz);
                PutF32(body, view.Offset(sphere + 12, 4), halfDiagonal);
            }

            if (textureName != null) { RenameTexture(body, view, template.Shaders[0].TextureNameSlots[0], textureName); }
            return result;
        }

        private static void WriteVertices(byte[] body, int graphicsStart, Mesh mesh, VertexLayout layout)
        {
            int position = layout.OffsetOf(0), normal = layout.OffsetOf(3), colour = layout.OffsetOf(4), uv = layout.OffsetOf(6);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                Mesh.Vertex v = mesh.Vertices[i];
                int at = graphicsStart + i * layout.Stride;
                PutVector3(body, at + position, v.X, v.Y, v.Z);
                PutVector3(body, at + normal, v.NX, v.NY, v.NZ);
                PutU32(body, at + colour, v.Colour);
                PutF32(body, at + uv, v.U);
                PutF32(body, at + uv + 4, v.V);
            }
        }

        // Rewrites the name in place when it fits the template's slot, otherwise in the system segment's unused tail.
        private static void RenameTexture(byte[] body, ResourceView view, uint nameSlot, string name)
        {
            byte[] text = Encoding.ASCII.GetBytes(name + "\0");
            uint current = BitConverter.ToUInt32(body, view.Offset(nameSlot, 4));
            if (text.Length <= view.StringCapacity(current))
            {
                int at = view.Offset(current, text.Length);
                int capacity = view.StringCapacity(current);
                for (int i = 0; i < capacity; i++) { body[at + i] = i < text.Length ? text[i] : (byte)0xCD; }
                return;
            }
            int free = view.SystemSize;
            while (free > 0 && body[free - 1] == 0xCD) { free--; }
            free = Align(free, 16);
            if (free + text.Length > view.SystemSize) { throw ResourceView.Bad("no room for texture name '" + name + "' in the template's system segment"); }
            Buffer.BlockCopy(text, 0, body, free, text.Length);
            PutU32(body, view.Offset(nameSlot, 4), ResourceView.SystemBase + (uint)free);
        }

        // Flags bits 15-25 hold the graphics page count and 26-29 the page shift (page = 256 << shift).
        // Size is rounded up to 4 KB and whole pages.
        internal static uint EncodeGraphics(uint flags, int used, int shift, out int size)
        {
            size = Align(Math.Max(used, 1), Math.Max(0x1000, 256 << shift));
            uint count = (uint)(size / (256 << shift));
            if (count == 0 || count > 0x7FF) { throw ResourceView.Bad("cannot encode graphics size 0x" + size.ToString("X")); }
            return (flags & ~((0x7FFu << 15) | (0xFu << 26))) | (count << 15) | ((uint)shift << 26);
        }

        // The game's files size pages from the largest buffer: page = next power of two >= it, divided by 16.
        internal static int ShiftFor(int largestBuffer)
        {
            int log = 0;
            while ((1 << log) < largestBuffer) { log++; }
            return Math.Min(15, Math.Max(0, log - 12));
        }

        // In the game's files a buffer never straddles an aligned block of max(4 KB, 8 pages); it moves to the next
        // block instead. Matched so rebuilt game drawables come out identical (docs/research/ModelFormat.md).
        internal static int Place(int offset, int size, int shift)
        {
            int start = Align(offset, 16);
            int block = Math.Max(0x1000, 0x800 << shift);
            if (start / block != (start + size - 1) / block) { start = Align(start, block); }
            return start;
        }

        private static int Align(int value, int alignment) { return (value + alignment - 1) / alignment * alignment; }
        private static void PutU32(byte[] b, int at, uint value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, at, 4); }
        private static void PutU16(byte[] b, int at, int value) { b[at] = (byte)value; b[at + 1] = (byte)(value >> 8); }
        private static void PutF32(byte[] b, int at, float value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, b, at, 4); }
        private static void PutVector3(byte[] b, int at, float x, float y, float z) { PutF32(b, at, x); PutF32(b, at + 4, y); PutF32(b, at + 8, z); }
    }
}
