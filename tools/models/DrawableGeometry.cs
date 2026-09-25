namespace LibertyFramework.Models
{
    // One grmGeometry with the resource addresses of everything the builder patches.
    internal sealed class DrawableGeometry
    {
        // Field offsets inside the structures (docs/research/ModelFormat.md).
        internal const int GeometryVertexBuffer = 0x0C, GeometryIndexBuffer = 0x1C, GeometryIndexCount = 0x2C,
            GeometryFaceCount = 0x30, GeometryVertexCount = 0x34, GeometryPrimitive = 0x36, GeometryStride = 0x3C, GeometryBoneCount = 0x3E;
        internal const int VertexBufferCount = 0x04, VertexBufferData = 0x08, VertexBufferStride = 0x0C, VertexBufferDeclaration = 0x10, VertexBufferData2 = 0x18;
        internal const int IndexBufferCount = 0x04, IndexBufferData = 0x08;

        internal uint Address;
        internal uint VertexBuffer;
        internal uint IndexBuffer;
        internal uint VertexData;
        internal uint IndexData;
        internal int VertexCount;
        internal int IndexCount;
        internal int FaceCount;
        internal int PrimitiveType;
        internal int BoneCount;
        internal int ShaderIndex;
        internal VertexLayout Layout;
    }
}
