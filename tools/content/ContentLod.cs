using System.Collections.Generic;

namespace LibertyFramework.Content
{
    // One LOD level of an asset (0 = highest detail), split into material groups ordered by material index.
    internal sealed class ContentLod
    {
        internal int Level;
        internal readonly List<ContentMaterialGroup> Groups = new List<ContentMaterialGroup>();

        internal int TriangleCount
        {
            get { int total = 0; foreach (ContentMaterialGroup group in Groups) { total += group.TriangleCount; } return total; }
        }

        internal int VertexCount
        {
            get { int total = 0; foreach (ContentMaterialGroup group in Groups) { total += group.VertexCount; } return total; }
        }
    }
}
