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
    //
    // textureMode "native" (asset.json; NEEDS-PLAYTEST): the drawable is built exactly as above (same template texture
    // name), but the dictionary is written from scratch by TextureDictionaryWriter: the source texture at its own size
    // (nearest power of two, 4-2048), full mip chain, DXT5 when the material is not opaque and has alpha, else DXT1. The
    // bytes the writer does not compute are captured from the template's own dictionary.
    internal static class PropCompiler
    {
        // Size of the texture written when the material has no image (its base colour fills it): one DXT block.
        internal const int SolidColourTextureSizePixels = 4;

        internal sealed class Result
        {
            internal byte[] Drawable;
            internal byte[] Dictionary;
            internal Mesh Mesh;
            internal string TextureMode = AssetManifest.TextureModeTemplate;
            internal string TextureName;
            internal int TextureWidth, TextureHeight;
            internal string TextureFormat;
            internal int TextureLevels;
            // Native mode only: what was encoded (checked byte for byte on read-back) and the level 0 source pixels.
            internal NativeTexture EncodedTexture;
            internal RgbaImage SourceImage;
            // Set by Readback in native mode.
            internal TextureQuality TextureQuality;
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
            result.TextureMode = manifest.TextureMode ?? AssetManifest.TextureModeTemplate;
            result.Dictionary = result.TextureMode == AssetManifest.TextureModeNative
                ? BuildNativeTexture(archive, manifest, template, asset, result)
                : BuildTexture(archive, manifest, template, asset, result);
            return result;
        }

        private static byte[] BuildNativeTexture(ArchiveSource archive, AssetManifest manifest, DrawableFile template, ContentAsset asset, Result result)
        {
            string referenced = template.Shaders[0].Textures[0];
            string templatePath = manifest.Template.Model + ".wtd";
            RscResource templateDictionary = RscResource.Parse(archive.Extract(templatePath));
            TextureDictionaryPrototype prototype = TextureDictionaryPrototype.FromResource(templateDictionary, templatePath);
            List<string> differences = prototype.OpaqueDifferences(TextureDictionaryPrototype.Builtin());
            if (differences.Count > 0) { result.Notes.Add("template dictionary's copied bytes differ from the builtin prototype at " + string.Join(", ", differences.ToArray())); }
            // Keep the name the drawable references (spelled as the template dictionary spells it).
            TextureDictionary.Texture named = TextureDictionary.Parse(templateDictionary, false).Textures.FirstOrDefault(t => string.Equals(t.Name, referenced, StringComparison.OrdinalIgnoreCase));
            string textureName = named != null ? named.Name : referenced;

            string format;
            RgbaImage pixels = NativeSourcePixels(asset, result.Notes, out format);
            NativeTexture texture = TextureEncoder.Encode(textureName, pixels, format, 0);
            byte[] file = TextureDictionaryWriter.Write(new List<NativeTexture> { texture }, prototype).Resource.Serialize();
            TextureDictionary.Parse(RscResource.Parse(file));
            result.TextureName = texture.Name; result.TextureWidth = texture.Width; result.TextureHeight = texture.Height;
            result.TextureFormat = texture.Format; result.TextureLevels = texture.Levels.Count;
            result.EncodedTexture = texture; result.SourceImage = pixels;
            result.Notes.Add("native texture dictionary: " + texture.Width + "x" + texture.Height + " " + texture.Format + ", " + texture.Levels.Count + " mip levels, prototype " + prototype.Source);
            return file;
        }

        // Native mode's source: the LOD 0 material's texture at NativeSide x NativeSide, multiplied by its base colour (or a
        // SolidColourTextureSizePixels square of the base colour without a texture), and the format to encode it in: DXT5
        // when the material is not opaque and a pixel is translucent, else DXT1.
        internal static RgbaImage NativeSourcePixels(ContentAsset asset, List<string> notes, out string format)
        {
            ContentMaterial material = Lod0Material(asset);
            Bitmap image = material != null && material.Image >= 0 && material.Image < asset.Images.Count ? asset.Images[material.Image] : null;
            float[] tint = material != null ? material.BaseColour : new float[] { 1, 1, 1, 1 };
            RgbaImage pixels;
            if (image == null)
            {
                pixels = RgbaImage.Solid(SolidColourTextureSizePixels, SolidColourTextureSizePixels, Channel(tint[0]), Channel(tint[1]), Channel(tint[2]), Channel(tint.Length > 3 ? tint[3] : 1));
                notes.Add("no texture: " + SolidColourTextureSizePixels + "x" + SolidColourTextureSizePixels + " filled with the base colour");
            }
            else
            {
                int width = NativeSide(image.Width), height = NativeSide(image.Height);
                if (width != image.Width || height != image.Height) { notes.Add("texture resampled " + image.Width + "x" + image.Height + " -> " + width + "x" + height + " (power of two, " + TextureEncoder.MinSizePixels + "-" + TextureEncoder.MaxSizePixels + ")"); }
                pixels = RgbaImage.FromBitmap(image, width, height, tint);
            }
            bool alpha = material != null && material.AlphaMode != "OPAQUE" && pixels.HasTranslucency();
            format = alpha ? "DXT5" : "DXT1";
            if (material != null && material.AlphaMode != "OPAQUE" && !alpha) { notes.Add("alpha mode " + material.AlphaMode + " but every pixel is opaque: DXT1"); }
            return pixels;
        }

        // Nearest power of two on a log scale, clamped to the writer's range.
        internal static int NativeSide(int pixels)
        {
            int side = TextureEncoder.MinSizePixels;
            while (side < TextureEncoder.MaxSizePixels && Math.Abs(Math.Log(side * 2.0 / pixels)) <= Math.Abs(Math.Log((double)side / pixels))) { side *= 2; }
            return side;
        }

        private static ContentMaterial Lod0Material(ContentAsset asset)
        {
            return asset.Lod(0).Select(m => m.Material >= 0 && m.Material < asset.Materials.Count ? asset.Materials[m.Material] : null).FirstOrDefault(m => m != null);
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
            result.TextureFormat = texture.Format; result.TextureLevels = Math.Max(1, texture.Levels);
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
            ContentMaterial material = Lod0Material(asset);
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
