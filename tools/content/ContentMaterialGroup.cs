using System.Collections.Generic;

namespace LibertyFramework.Content
{
    // The meshes of one LOD that share one material: what a writer turns into one geometry (a drawable geometry is drawn
    // with one shader, docs/research/ModelFormat.md: model +0x10 holds one shader index per geometry). Material -1 means
    // "no material" (the default white material).
    internal sealed class ContentMaterialGroup
    {
        internal int Material;
        internal readonly List<ContentMesh> Meshes = new List<ContentMesh>();

        internal int VertexCount
        {
            get { int total = 0; foreach (ContentMesh mesh in Meshes) { total += mesh.Vertices.Count; } return total; }
        }

        internal int TriangleCount
        {
            get { int total = 0; foreach (ContentMesh mesh in Meshes) { total += mesh.Indices.Count / 3; } return total; }
        }
    }
}
