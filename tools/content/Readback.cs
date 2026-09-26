using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Reads the compiled resources back with the same readers that parse the game's own files and compares them with
    // what was meant to be written: geometry field for field, bounds, and the texture dictionary's texture.
    internal static class Readback
    {
        internal static List<string> Verify(PropCompiler.Result compiled)
        {
            List<string> problems = new List<string>();
            DrawableFile drawable = new DrawableFile(RscResource.Parse(compiled.Drawable));
            if (drawable.Models.Count != 1 || drawable.Models[0].Geometries.Count != 1) { problems.Add("drawable does not read back as one model with one geometry"); return problems; }
            Mesh back = drawable.ReadMesh(drawable.Models[0].Geometries[0]);
            Mesh sent = compiled.Mesh;
            if (back.Vertices.Count != sent.Vertices.Count) { problems.Add("vertex count " + back.Vertices.Count + " != " + sent.Vertices.Count); }
            if (back.Indices.Count != sent.Indices.Count) { problems.Add("index count " + back.Indices.Count + " != " + sent.Indices.Count); }
            if (problems.Count > 0) { return problems; }
            int positionErrors = 0, uvErrors = 0, normalErrors = 0;
            for (int i = 0; i < sent.Vertices.Count; i++)
            {
                Mesh.Vertex a = sent.Vertices[i], b = back.Vertices[i];
                if (a.X != b.X || a.Y != b.Y || a.Z != b.Z) { positionErrors++; }
                if (a.U != b.U || a.V != b.V) { uvErrors++; }
                if (Math.Abs(a.NX - b.NX) > 1e-3 || Math.Abs(a.NY - b.NY) > 1e-3 || Math.Abs(a.NZ - b.NZ) > 1e-3) { normalErrors++; }
            }
            int indexErrors = sent.Indices.Where((index, i) => back.Indices[i] != index).Count();
            if (positionErrors > 0) { problems.Add(positionErrors + " vertex positions differ"); }
            if (uvErrors > 0) { problems.Add(uvErrors + " UVs differ"); }
            if (normalErrors > 0) { problems.Add(normalErrors + " normals differ"); }
            if (indexErrors > 0) { problems.Add(indexErrors + " indices differ"); }
            TextureDictionary dictionary = TextureDictionary.Parse(RscResource.Parse(compiled.Dictionary));
            TextureDictionary.Texture texture = dictionary.Textures.FirstOrDefault(t => t.Name == compiled.TextureName);
            if (texture == null) { problems.Add("texture " + compiled.TextureName + " missing from the dictionary"); }
            else if (texture.Width != compiled.TextureWidth || texture.Height != compiled.TextureHeight) { problems.Add("texture size changed"); }
            // The game finds textures by the hash of the lower-case name, so case does not matter.
            if (drawable.Shaders.Count != 1 || drawable.Shaders[0].Textures.Count != 1 || !string.Equals(drawable.Shaders[0].Textures[0], compiled.TextureName, StringComparison.OrdinalIgnoreCase))
            {
                problems.Add("drawable does not reference texture " + compiled.TextureName);
            }
            return problems;
        }
    }
}
