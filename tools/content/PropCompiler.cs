using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Prop writer, version 1: patches a game template (one model, one geometry, gta_default with one texture) the way
    // the sling straps are built (proven in game). LOD 0 meshes of one material become the geometry; the material's base
    // colour texture is resampled to the template texture's size and written, with its mip chain, into a copy of the
    // template's texture dictionary. Multiple materials, LOD levels and collision need the structure writer (roadmap M6).
    internal static class PropCompiler
    {
        internal sealed class Result
        {
            internal byte[] Drawable;
            internal byte[] Dictionary;
            internal Mesh Mesh;
            internal string TextureName;
            internal int TextureWidth, TextureHeight;
            internal bool FlippedWinding;
            internal readonly List<string> Notes = new List<string>();
        }

        internal static Result Compile(string game, AssetManifest manifest, ContentAsset asset)
        {
            ArchiveSource archive = ArchiveSource.Open(game, manifest.Template.Archive);
            DrawableFile template = new DrawableFile(RscResource.Parse(archive.Extract(manifest.Template.Model + ".wdr"), true));
            Result result = new Result();
            result.Mesh = BuildMesh(asset, result);
            // Match the template's winding (front faces as the game draws them), measured on its own geometry.
            Mesh reference = template.ReadMesh(template.Models[0].Geometries[0]);
            if (Math.Sign(WindingScore(reference)) != Math.Sign(WindingScore(result.Mesh)) && WindingScore(result.Mesh) != 0)
            {
                for (int i = 0; i + 2 < result.Mesh.Indices.Count; i += 3) { int t = result.Mesh.Indices[i + 1]; result.Mesh.Indices[i + 1] = result.Mesh.Indices[i + 2]; result.Mesh.Indices[i + 2] = t; }
                result.FlippedWinding = true;
                result.Notes.Add("triangle winding flipped to the template's convention");
            }
            DrawableBuilder.SinglePage = true;
            result.Drawable = DrawableBuilder.Build(template, result.Mesh, null).Serialize();
            result.Dictionary = BuildTexture(archive, manifest, template, asset, result);
            return result;
        }

        // LOD 0 meshes merged into one; degenerate triangles dropped; normals generated when the source had none.
        private static Mesh BuildMesh(ContentAsset asset, Result result)
        {
            Mesh mesh = new Mesh();
            bool anyMissingNormals = false;
            foreach (ContentMesh part in asset.Lod(0))
            {
                int start = mesh.Vertices.Count;
                foreach (ContentVertex v in part.Vertices)
                {
                    Mesh.Vertex m = new Mesh.Vertex();
                    m.X = v.X; m.Y = v.Y; m.Z = v.Z; m.U = v.U; m.V = v.V; m.Colour = v.Colour;
                    float length = (float)Math.Sqrt(v.NX * v.NX + v.NY * v.NY + v.NZ * v.NZ);
                    if (length > 1e-6f) { m.NX = v.NX / length; m.NY = v.NY / length; m.NZ = v.NZ / length; } else { m.NZ = 1; }
                    mesh.Vertices.Add(m);
                }
                if (!part.HadNormals) { anyMissingNormals = true; }
                for (int i = 0; i + 2 < part.Indices.Count; i += 3)
                {
                    int a = part.Indices[i], b = part.Indices[i + 1], c = part.Indices[i + 2];
                    if (a == b || b == c || a == c) { continue; }
                    mesh.Indices.Add(start + a); mesh.Indices.Add(start + b); mesh.Indices.Add(start + c);
                }
            }
            if (anyMissingNormals) { MeshNormals.Recompute(mesh); result.Notes.Add("normals generated"); }
            mesh.Validate();
            return mesh;
        }

        // Sum over triangles of (face normal . vertex normals): positive when the winding agrees with the normals.
        internal static double WindingScore(Mesh mesh)
        {
            double score = 0;
            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                Mesh.Vertex a = mesh.Vertices[mesh.Indices[i]], b = mesh.Vertices[mesh.Indices[i + 1]], c = mesh.Vertices[mesh.Indices[i + 2]];
                double ux = b.X - a.X, uy = b.Y - a.Y, uz = b.Z - a.Z, wx = c.X - a.X, wy = c.Y - a.Y, wz = c.Z - a.Z;
                double nx = uy * wz - uz * wy, ny = uz * wx - ux * wz, nz = ux * wy - uy * wx;
                score += nx * (a.NX + b.NX + c.NX) + ny * (a.NY + b.NY + c.NY) + nz * (a.NZ + b.NZ + c.NZ);
            }
            return score;
        }

        private static byte[] BuildTexture(ArchiveSource archive, AssetManifest manifest, DrawableFile template, ContentAsset asset, Result result)
        {
            string textureName = template.Shaders[0].Textures[0];
            RscResource resource = RscResource.Parse(archive.Extract(manifest.Template.Model + ".wtd"));
            TextureDictionary dictionary = TextureDictionary.Parse(resource);
            TextureDictionary.Texture texture = dictionary.Textures.FirstOrDefault(t => string.Equals(t.Name, textureName, StringComparison.OrdinalIgnoreCase));
            if (texture == null || texture.Format != "DXT1") { throw new InvalidDataException("template dictionary has no DXT1 texture " + textureName); }
            result.TextureName = texture.Name; result.TextureWidth = texture.Width; result.TextureHeight = texture.Height;
            byte[] rgb = SourcePixels(asset, texture.Width, texture.Height, result);
            int offset = texture.DataOffset, width = texture.Width, height = texture.Height;
            for (int level = 0; level < Math.Max(1, texture.Levels); level++)
            {
                byte[] blocks = Dxt1Encoder.Encode(rgb, width, height);
                if (blocks.Length != texture.LevelBytes(level)) { throw new InvalidDataException("mip " + level + " size mismatch"); }
                Buffer.BlockCopy(blocks, 0, resource.Body, offset, blocks.Length);
                offset += blocks.Length;
                rgb = StrapTexture.Half(rgb, width, height);
                width = Math.Max(1, width / 2); height = Math.Max(1, height / 2);
            }
            byte[] file = resource.Serialize();
            TextureDictionary.Parse(RscResource.Parse(file));
            return file;
        }

        // The LOD 0 material's texture resampled to width x height (RGB), or its base colour when it has none.
        private static byte[] SourcePixels(ContentAsset asset, int width, int height, Result result)
        {
            ContentMaterial material = asset.Lod(0).Select(m => m.Material >= 0 && m.Material < asset.Materials.Count ? asset.Materials[m.Material] : null).FirstOrDefault(m => m != null);
            byte[] rgb = new byte[width * height * 3];
            Bitmap image = material != null && material.Image >= 0 && material.Image < asset.Images.Count ? asset.Images[material.Image] : null;
            float[] tint = material != null ? material.BaseColour : new float[] { 1, 1, 1, 1 };
            if (image == null)
            {
                for (int i = 0; i < width * height; i++) { rgb[i * 3] = Channel(tint[0]); rgb[i * 3 + 1] = Channel(tint[1]); rgb[i * 3 + 2] = Channel(tint[2]); }
                result.Notes.Add("no texture: filled with the base colour");
                return rgb;
            }
            if (image.Width != width || image.Height != height) { result.Notes.Add("texture resampled " + image.Width + "x" + image.Height + " -> " + width + "x" + height + " (template size)"); }
            using (Bitmap scaled = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(scaled))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    using (System.Drawing.Imaging.ImageAttributes wrap = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrap.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrap);
                    }
                }
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color c = scaled.GetPixel(x, y);
                        int at = (y * width + x) * 3;
                        rgb[at] = (byte)(c.R * tint[0]); rgb[at + 1] = (byte)(c.G * tint[1]); rgb[at + 2] = (byte)(c.B * tint[2]);
                    }
                }
            }
            return rgb;
        }

        private static byte Channel(float v) { return (byte)Math.Round(Math.Max(0, Math.Min(1, v)) * 255); }
    }
}
