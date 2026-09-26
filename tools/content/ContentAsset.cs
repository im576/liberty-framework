using System.Collections.Generic;
using System.Drawing;

namespace LibertyFramework.Content
{
    // The compiler's intermediate representation: format-neutral, in GTA IV space (metres, Z up, X right, Y forward).
    // Importers produce it, validators inspect it, writers turn it into game resources.
    internal sealed class ContentAsset
    {
        internal string Name;
        internal string SourcePath;
        internal readonly List<ContentMesh> Meshes = new List<ContentMesh>();
        internal readonly List<ContentMaterial> Materials = new List<ContentMaterial>();
        internal readonly List<Bitmap> Images = new List<Bitmap>();
        internal readonly List<string> ImageNames = new List<string>();
        // Source metadata (glTF extras "liberty_*"), for writers that need it.
        internal readonly Dictionary<string, string> Metadata = new Dictionary<string, string>();
        internal bool HasSkin;

        internal IEnumerable<ContentMesh> Lod(int lod)
        {
            foreach (ContentMesh mesh in Meshes) { if (mesh.Lod == lod) { yield return mesh; } }
        }

        internal int MaxLod
        {
            get { int max = 0; foreach (ContentMesh m in Meshes) { if (m.Lod > max) { max = m.Lod; } } return max; }
        }

        // The asset as LOD levels (ascending), each split into material groups (ascending material index; -1 first).
        // Meshes without triangles are included; the validator reports them.
        internal List<ContentLod> BuildLods()
        {
            SortedDictionary<int, SortedDictionary<int, ContentMaterialGroup>> levels = new SortedDictionary<int, SortedDictionary<int, ContentMaterialGroup>>();
            foreach (ContentMesh mesh in Meshes)
            {
                SortedDictionary<int, ContentMaterialGroup> groups;
                if (!levels.TryGetValue(mesh.Lod, out groups)) { groups = new SortedDictionary<int, ContentMaterialGroup>(); levels[mesh.Lod] = groups; }
                ContentMaterialGroup group;
                if (!groups.TryGetValue(mesh.Material, out group)) { group = new ContentMaterialGroup { Material = mesh.Material }; groups[mesh.Material] = group; }
                group.Meshes.Add(mesh);
            }
            List<ContentLod> lods = new List<ContentLod>();
            foreach (KeyValuePair<int, SortedDictionary<int, ContentMaterialGroup>> level in levels)
            {
                ContentLod lod = new ContentLod { Level = level.Key };
                lod.Groups.AddRange(level.Value.Values);
                lods.Add(lod);
            }
            return lods;
        }
    }

    internal sealed class ContentMesh
    {
        internal string Name;
        internal int Lod;
        internal int Material = -1;
        internal bool HadNormals;
        internal bool HadUvs;
        internal readonly List<ContentVertex> Vertices = new List<ContentVertex>();
        internal readonly List<int> Indices = new List<int>();
    }

    internal struct ContentVertex
    {
        internal float X, Y, Z, NX, NY, NZ, U, V;
        internal uint Colour; // ARGB
    }

    internal sealed class ContentMaterial
    {
        internal string Name;
        internal string Shader = "gta_default";
        internal int Image = -1;
        internal float[] BaseColour = { 1, 1, 1, 1 };
        internal string AlphaMode = "OPAQUE";
        internal bool DoubleSided;
    }
}
