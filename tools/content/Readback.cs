using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Reads the compiled resources back with the same readers that parse the game's own files and compares them with
    // what was meant to be written: geometry field for field, bounds, and the texture dictionary's texture. In native
    // texture mode every mip level must read back byte for byte, and the top level is decoded and compared with the
    // source pixels (TextureQuality, stored on the result for report.json).
    internal static class Readback
    {
        // Sanity floor for the decoded top level against its source. DXT compression of ordinary textures scores well above
        // it; a swapped channel order, a wrong level or misplaced data scores far below.
        internal const double MinimumPsnrDb = 20;

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
            if (compiled.EncodedTexture != null)
            {
                TextureQuality quality;
                problems.AddRange(VerifyNativeTexture(compiled.Dictionary, compiled.EncodedTexture, compiled.SourceImage, out quality));
                compiled.TextureQuality = quality;
            }
            // The game finds textures by the hash of the lower-case name, so case does not matter.
            if (drawable.Shaders.Count != 1 || drawable.Shaders[0].Textures.Count != 1 || !string.Equals(drawable.Shaders[0].Textures[0], compiled.TextureName, StringComparison.OrdinalIgnoreCase))
            {
                problems.Add("drawable does not reference texture " + compiled.TextureName);
            }
            return problems;
        }
    
        // A texture written by TextureDictionaryWriter: format, size, level count, every level's bytes, and the decoded top
        // level against the source pixels (skipped when source is null).
        internal static List<string> VerifyNativeTexture(byte[] dictionaryFile, NativeTexture expected, RgbaImage source, out TextureQuality quality)
        {
            quality = null;
            List<string> problems = new List<string>();
            RscResource resource = RscResource.Parse(dictionaryFile);
            TextureDictionary.Texture texture = TextureDictionary.Parse(resource).Textures.FirstOrDefault(t => t.Name == expected.Name);
            if (texture == null) { problems.Add("texture " + expected.Name + " missing from the dictionary"); return problems; }
            if (texture.Format != expected.Format) { problems.Add("texture format " + texture.Format + " != " + expected.Format); }
            if (texture.Width != expected.Width || texture.Height != expected.Height) { problems.Add("texture size " + texture.Width + "x" + texture.Height + " != " + expected.Width + "x" + expected.Height); }
            if (texture.Levels != expected.Levels.Count) { problems.Add("mip levels " + texture.Levels + " != " + expected.Levels.Count); }
            if (problems.Count > 0) { return problems; }
            int at = texture.DataOffset;
            for (int level = 0; level < expected.Levels.Count; level++)
            {
                byte[] bytes = expected.Levels[level];
                if (at + bytes.Length > resource.Body.Length) { problems.Add("mip level " + level + " runs past the resource"); return problems; }
                for (int k = 0; k < bytes.Length; k++)
                {
                    if (resource.Body[at + k] != bytes[k]) { problems.Add("mip level " + level + " differs at byte " + k); break; }
                }
                at += bytes.Length;
            }
            if (source == null) { return problems; }
            if (source.Width != texture.Width || source.Height != texture.Height) { problems.Add("source image is " + source.Width + "x" + source.Height + ", texture " + texture.Width + "x" + texture.Height); return problems; }
            RgbaImage decoded = new RgbaImage(texture.Width, texture.Height, DxtDecoder.Decode(resource.Body, texture.DataOffset, texture.Format, texture.Width, texture.Height));
            quality = new TextureQuality { PsnrRgbDb = ImageMetrics.PsnrDb(source, decoded, false), MaxErrorRgb = ImageMetrics.MaxError(source, decoded, false) };
            if (texture.Format != "DXT1")
            {
                quality.HasAlpha = true;
                quality.PsnrAlphaDb = ImageMetrics.PsnrDb(source, decoded, true);
                quality.MaxErrorAlpha = ImageMetrics.MaxError(source, decoded, true);
            }
            if (quality.PsnrRgbDb < MinimumPsnrDb) { problems.Add("decoded texture colour PSNR " + quality.PsnrRgbDb.ToString("0.0", CultureInfo.InvariantCulture) + " dB is below " + MinimumPsnrDb + " dB"); }
            if (quality.HasAlpha && quality.PsnrAlphaDb < MinimumPsnrDb) { problems.Add("decoded texture alpha PSNR " + quality.PsnrAlphaDb.ToString("0.0", CultureInfo.InvariantCulture) + " dB is below " + MinimumPsnrDb + " dB"); }
            return problems;
        }
    }
}
