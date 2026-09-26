using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // Liberty Content Compiler (LCC). Blender/glTF assets -> validated, compiled GTA IV resources, verified by reading them
    // back, with previews and a machine-readable report for agents. Commands:
    //   sample <dir> <name>                              write the original test crate as glTF (no Blender needed)
    //   validate <asset.json> [--report <report.json>]   import + validate, print issues (exit 2 on errors), optional report
    //   capabilities [--writer structure]                what this compiler writes, as JSON (the Blender add-on's limits)
    //   fixtures <dir>                                   validate every <dir>/<name>/asset.json against its expect.json
    //   roundtrip --game <game> [--out <json>] [img...]  rebuild the game's drawables with the structure writer and compare
    //   build <game> <asset.json> <out>                  validate, compile, read back, preview, report
    //   package <game> <out> <img> <ide> <asset.json...> build every asset, then one IMG and its IDE
    //   templates <game> <archive>                       list drawables usable as prop templates
    //   selftest [--out <dir>]                           offline tests (DXT codecs, WTD writer round trips, validator); no game
    //   wtdcheck [--game <dir>] <wtd|folder|img...>      rebuild game dictionaries with the WTD writer and compare (read only)
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length >= 3 && args[0] == "sample") { SampleAsset.Write(args[1], args[2]); Console.WriteLine("sample written: " + Path.Combine(args[1], args[2] + ".gltf")); return 0; }
                if (args.Length == 2 && args[0] == "validate") { return Validate(args[1], null); }
                if (args.Length == 4 && args[0] == "validate" && args[2] == "--report") { return Validate(args[1], args[3]); }
                if (args.Length == 1 && args[0] == "capabilities") { Console.WriteLine(CompilerCapabilities.Current.ToJson()); return 0; }
                if (args.Length == 3 && args[0] == "capabilities" && args[1] == "--writer" && args[2] == AssetManifest.WriterStructure) { Console.WriteLine(CompilerCapabilities.Structure.ToJson()); return 0; }
                if (args.Length == 2 && args[0] == "fixtures") { return FixtureCheck.Run(args[1]); }
                if (args.Length >= 4 && args[0] == "build") { string ignored; return Build(args[1], args[2], args[3], out ignored); }
                if (args.Length >= 6 && args[0] == "package") { return Package(args[1], args[2], args[3], args[4], args.Skip(5).ToArray()); }
                if (args.Length >= 3 && args[0] == "templates") { return Templates(args[1], args[2]); }
                if (args.Length >= 1 && args[0] == "selftest") { return SelfTest.Run(args.Skip(1).ToArray()); }
                if (args.Length >= 2 && args[0] == "wtdcheck") { return TextureDictionaryCheck.Run(args.Skip(1).ToArray()); }
                if (args.Length >= 1 && args[0] == "probe") { return Probe.Run(args.Skip(1).ToArray()); }
                if (args.Length >= 1 && args[0] == "roundtrip") { return DrawableRoundTrip.Run(args.Skip(1).ToArray()); }
                Console.WriteLine("usage: LibertyContent sample <dir> <name> | validate <asset.json> [--report <file>] | capabilities | fixtures <dir> |");
                Console.WriteLine("       build <game> <asset.json> <out> |");
                Console.WriteLine("       package <game> <out> <img> <ide> <asset.json...> | templates <game> <archive> |");
                Console.WriteLine("       selftest [--out <dir>] | wtdcheck [--game <dir>] <file.wtd|folder|archive.img...> |");
                Console.WriteLine("       probe drawables|collision --game <dir> --out <json> | roundtrip --game <dir> [--out <json>] [archive.img...]");
                return 1;
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR " + error.GetType().Name + ": " + error.Message);
                return 3;
            }
        }

        private static int Validate(string manifestPath, string reportPath)
        {
            AssetManifest manifest = AssetManifest.Load(manifestPath);
            ContentAsset asset = GltfImporter.Import(manifest.SourcePath);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, manifest, CompilerCapabilities.For(manifest));
            foreach (AssetValidator.Issue issue in issues) { Console.WriteLine(issue); }
            bool failed = AssetValidator.HasErrors(issues);
            if (reportPath != null)
            {
                string folder = Path.GetDirectoryName(Path.GetFullPath(reportPath));
                Directory.CreateDirectory(folder);
                File.WriteAllText(reportPath, ReportJson(manifest, asset, issues, null, new List<string>(), failed ? StatusInvalid : StatusValid));
            }
            Console.WriteLine("validate " + manifest.Name + ": " + (failed ? "FAILED" : "ok") + " (" + issues.Count + " issues)" + (reportPath != null ? " report=" + reportPath : ""));
            return failed ? 2 : 0;
        }

        // report.json "status" values.
        internal const string StatusValid = "valid", StatusInvalid = "invalid";

        private static int Build(string game, string manifestPath, string output, out string status)
        {
            AssetManifest manifest = AssetManifest.Load(manifestPath);
            string folder = Path.Combine(output, manifest.Name);
            Directory.CreateDirectory(folder);
            ContentAsset asset = GltfImporter.Import(manifest.SourcePath);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, manifest, CompilerCapabilities.For(manifest));
            foreach (AssetValidator.Issue issue in issues) { Console.WriteLine("  " + issue); }
            List<string> readback = new List<string>();
            PropCompiler.Result compiled = null;
            if (!AssetValidator.HasErrors(issues))
            {
                compiled = PropCompiler.Compile(game, manifest, asset);
                File.WriteAllBytes(Path.Combine(folder, manifest.Name + ".wdr"), compiled.Drawable);
                File.WriteAllBytes(Path.Combine(folder, manifest.TextureDictionary + ".wtd"), compiled.Dictionary);
                readback = Readback.Verify(compiled);
                WritePreviews(folder, manifest, compiled);
            }
            status = compiled == null ? StatusInvalid : readback.Count == 0 ? "ok" : "readback-failed";
            WriteReport(folder, manifest, asset, issues, compiled, readback, status);
            Console.WriteLine("build " + manifest.Name + ": " + status + (compiled != null ? " vertices=" + compiled.Mesh.Vertices.Count + " triangles=" +
                compiled.Mesh.Indices.Count / 3 + " texture=" + compiled.TextureName + " " + compiled.TextureWidth + "x" + compiled.TextureHeight + " " + compiled.TextureFormat + " levels=" + compiled.TextureLevels +
                (compiled.TextureQuality != null ? " psnr=" + Number(compiled.TextureQuality.PsnrRgbDb) + "dB" : "") : "") +
                " report=" + Path.Combine(folder, "report.json"));
            foreach (string problem in readback) { Console.WriteLine("  READBACK " + problem); }
            return status == "ok" ? 0 : 2;
        }

        private static void WritePreviews(string folder, AssetManifest manifest, PropCompiler.Result compiled)
        {
            // The preview is drawn from the geometry read back out of the compiled .wdr, i.e. what the game will load: the
            // highest LOD's geometries.
            DrawableFile drawable = new DrawableFile(RscResource.Parse(compiled.Drawable));
            List<Mesh> back = drawable.Models.Where(m => m.Lod == drawable.Models[0].Lod).SelectMany(m => m.Geometries).Select(drawable.ReadMesh).ToList();
            MeshPreview.Save(back, Path.Combine(folder, manifest.Name + "_preview.png"), manifest.Name + " (read back from .wdr)",
                back.Select(m => Color.FromArgb(170, 140, 100)).ToList());
            // The texture's top level decoded back out of the compiled .wtd (alpha kept for DXT5).
            RscResource dictionary = RscResource.Parse(compiled.Dictionary);
            TextureDictionary.Texture texture = TextureDictionary.Parse(dictionary).Textures.First(t => t.Name == compiled.TextureName);
            RgbaImage decoded = new RgbaImage(texture.Width, texture.Height, DxtDecoder.Decode(dictionary.Body, texture.DataOffset, texture.Format, texture.Width, texture.Height));
            using (Bitmap bitmap = decoded.ToBitmap()) { bitmap.Save(Path.Combine(folder, manifest.Name + "_texture.png"), System.Drawing.Imaging.ImageFormat.Png); }
        }

        private static void WriteReport(string folder, AssetManifest manifest, ContentAsset asset, List<AssetValidator.Issue> issues,
            PropCompiler.Result compiled, List<string> readback, string status)
        {
            File.WriteAllText(Path.Combine(folder, "report.json"), ReportJson(manifest, asset, issues, compiled, readback, status));
        }

        // report.json: the build (or validation) result for agents and the Blender add-on. "structure" is the IR as the
        // writers will see it: LOD levels with their geometries (one per material), LOD distances and collision shapes.
        internal static string ReportJson(AssetManifest manifest, ContentAsset asset, List<AssetValidator.Issue> issues,
            PropCompiler.Result compiled, List<string> readback, string status)
        {
            StringBuilder json = new StringBuilder("{\n");
            json.Append("  \"asset\": ").Append(Quote(manifest.Name)).Append(",\n");
            json.Append("  \"status\": ").Append(Quote(status)).Append(",\n");
            json.Append("  \"type\": ").Append(Quote(manifest.Type)).Append(",\n");
            json.Append("  \"source\": ").Append(Quote(asset.SourcePath)).Append(",\n");
            json.Append("  \"template\": ").Append(Quote(manifest.Template.Archive + "/" + manifest.Template.Model)).Append(",\n");
            json.Append("  \"capabilities\": ").Append(CompilerCapabilities.For(manifest).ToJson()).Append(",\n");
            json.Append("  \"metadata\": {").Append(string.Join(", ", asset.Metadata.OrderBy(p => p.Key).Select(p => Quote(p.Key) + ": " + Quote(p.Value)).ToArray())).Append("},\n");
            json.Append("  \"meshes\": ").Append(asset.Meshes.Count).Append(", \"materials\": ").Append(asset.Materials.Count).Append(", \"lods\": ").Append(asset.MaxLod + 1).Append(",\n");
            json.Append("  \"structure\": ").Append(StructureJson(manifest, asset)).Append(",\n");
            if (compiled != null)
            {
                json.Append("  \"compiled\": { \"vertices\": ").Append(compiled.Mesh.Vertices.Count).Append(", \"triangles\": ").Append(compiled.Mesh.Indices.Count / 3)
                    .Append(", \"texture\": ").Append(Quote(compiled.TextureName)).Append(", \"textureSize\": [").Append(compiled.TextureWidth).Append(", ").Append(compiled.TextureHeight)
                    .Append("], \"textureMode\": ").Append(Quote(compiled.TextureMode)).Append(", \"textureFormat\": ").Append(Quote(compiled.TextureFormat))
                    .Append(", \"textureLevels\": ").Append(compiled.TextureLevels).Append(TextureQualityJson(compiled.TextureQuality))
                    .Append(", \"flippedWinding\": ").Append(compiled.FlippedWinding ? "true" : "false").Append(", \"drawableBytes\": ").Append(compiled.Drawable.Length)
                    .Append(",\n    \"drawableWriter\": ").Append(Quote(compiled.DrawableWriter)).Append(", \"templateUsed\": ").Append(Quote(compiled.TemplateUsed))
                    .Append(StructureCompiledJson(compiled))
                    .Append(", \"dictionaryBytes\": ").Append(compiled.Dictionary.Length).Append(",\n    \"notes\": [").Append(string.Join(", ", compiled.Notes.Select(Quote).ToArray())).Append("] },\n");
            }
            json.Append("  \"issues\": [\n");
            json.Append(string.Join(",\n", issues.Select(i => "    { \"severity\": " + Quote(i.Severity) + ", \"code\": " + Quote(i.Code) + ", \"message\": " + Quote(i.Message) + " }").ToArray()));
            json.Append("\n  ],\n  \"readback\": [").Append(string.Join(", ", readback.Select(Quote).ToArray())).Append("]\n}\n");
            return json.ToString();
        }

        private static string StructureJson(AssetManifest manifest, ContentAsset asset)
        {
            List<string> lods = new List<string>();
            foreach (ContentLod lod in asset.BuildLods())
            {
                List<string> geometries = new List<string>();
                foreach (ContentMaterialGroup group in lod.Groups.Where(g => g.TriangleCount > 0))
                {
                    ContentMaterial material = group.Material >= 0 && group.Material < asset.Materials.Count ? asset.Materials[group.Material] : null;
                    geometries.Add("{ \"material\": " + Quote(material != null ? material.Name : null) + ", \"shader\": " + Quote(material != null ? material.Shader : "gta_default") +
                        ", \"textured\": " + (material != null && material.Image >= 0 ? "true" : "false") + ", \"alphaMode\": " + Quote(material != null ? material.AlphaMode : "OPAQUE") +
                        ", \"meshes\": " + group.Meshes.Count + ", \"triangles\": " + group.TriangleCount + ", \"vertices\": " + group.VertexCount + " }");
                }
                if (geometries.Count == 0) { continue; }
                lods.Add("{ \"level\": " + lod.Level + ", \"triangles\": " + lod.TriangleCount + ", \"vertices\": " + lod.VertexCount + ",\n        \"geometries\": [\n          " +
                    string.Join(",\n          ", geometries.ToArray()) + " ] }");
            }
            List<string> collision = new List<string>();
            foreach (ContentCollision shape in asset.Collisions)
            {
                string item = "{ \"name\": " + Quote(shape.Name) + ", \"shape\": " + Quote(shape.Shape) + ", \"surface\": " + Quote(shape.Surface);
                if (shape.Problem != null) { item += ", \"problem\": " + Quote(shape.Problem); }
                else if (shape.Shape == ContentCollision.ShapeMesh) { item += ", \"triangles\": " + shape.Indices.Count / 3 + ", \"vertices\": " + shape.Vertices.Count; }
                else if (Array.IndexOf(ContentCollision.Shapes, shape.Shape) >= 0)
                {
                    item += ", \"centre\": " + Vector(shape.Centre) + ", \"axes\": [" + string.Join(", ", shape.Axes.Select(Vector).ToArray()) + "]";
                    if (shape.Shape == ContentCollision.ShapeBox) { item += ", \"halfExtents\": " + Vector(shape.HalfExtents); }
                    else { item += ", \"radius\": " + Metres(shape.Radius); }
                    if (shape.Shape == ContentCollision.ShapeCapsule) { item += ", \"length\": " + Metres(shape.Length); }
                }
                collision.Add(item + " }");
            }
            string distances = manifest.LodDistancesMeters == null ? "null" : "[" + string.Join(", ", manifest.LodDistancesMeters.Select(Metres).ToArray()) + "]";
            return "{\n    \"lods\": [\n      " + string.Join(",\n      ", lods.ToArray()) + " ],\n    \"lodDistancesMeters\": " + distances +
                ",\n    \"collision\": [" + (collision.Count == 0 ? "" : "\n      " + string.Join(",\n      ", collision.ToArray()) + " ") + "]\n  }";
        }

        // Structure writer: ", lodCount, geometries [...], textures [...], lodDistancesWritten" (empty for v1 builds).
        private static string StructureCompiledJson(PropCompiler.Result compiled)
        {
            if (compiled.DrawableWriter != AssetManifest.WriterStructure) { return ""; }
            string geometries = string.Join(", ", compiled.Parts.Select(p => "{ \"lod\": " + p.Lod + ", \"material\": " + p.Material + ", \"texture\": " + Quote(p.TextureName) +
                ", \"vertices\": " + p.Mesh.Vertices.Count + ", \"triangles\": " + p.Mesh.Indices.Count / 3 + " }").ToArray());
            string textures = string.Join(", ", compiled.Textures.Select((t, k) => "{ \"name\": " + Quote(t.Name) + ", \"size\": [" + t.Width + ", " + t.Height + "], \"format\": " + Quote(t.Format) +
                ", \"levels\": " + t.Levels.Count + (k < compiled.TextureQualities.Count && compiled.TextureQualities[k] != null ? ", \"psnrRgbDb\": " + Number(compiled.TextureQualities[k].PsnrRgbDb) : "") + " }").ToArray());
            string distances = compiled.LodDistancesWritten == null ? "null" : "[" + string.Join(", ", compiled.LodDistancesWritten.Select(d => float.IsNaN(d) ? "null" : Metres(d)).ToArray()) + "]";
            return ", \"lodCount\": " + compiled.Parts.Select(p => p.Lod).Distinct().Count() + ",\n    \"geometries\": [" + geometries + "],\n    \"textures\": [" + textures +
                "],\n    \"lodDistancesWritten\": " + distances;
        }

        private static string Metres(float value) { return value.ToString("0.#####", CultureInfo.InvariantCulture); }
        private static string Vector(float[] values) { return "[" + string.Join(", ", values.Select(Metres).ToArray()) + "]"; }

        // Builds every asset, then packs the models and dictionaries into one IMG with its IDE ("weap" entries: the class
        // script-created props use in the proven sling path; collision comes with the bounds writer).
        private static int Package(string game, string output, string imgName, string ideName, string[] manifests)
        {
            Directory.CreateDirectory(output);
            List<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            StringBuilder ide = new StringBuilder("# Liberty Framework content (generated by tools/content, the Liberty Content Compiler)\nweap\n");
            StringBuilder amat = new StringBuilder("amat\n");
            int failed = 0;
            foreach (string manifestPath in manifests)
            {
                string status;
                if (Build(game, manifestPath, output, out status) != 0) { failed++; continue; }
                AssetManifest manifest = AssetManifest.Load(manifestPath);
                string folder = Path.Combine(output, manifest.Name);
                files.Add(new KeyValuePair<string, byte[]>(manifest.Name + ".wdr", File.ReadAllBytes(Path.Combine(folder, manifest.Name + ".wdr"))));
                files.Add(new KeyValuePair<string, byte[]>(manifest.TextureDictionary + ".wtd", File.ReadAllBytes(Path.Combine(folder, manifest.TextureDictionary + ".wtd"))));
                ide.Append(manifest.Name + ", " + manifest.TextureDictionary + ", null, 1, " + manifest.DrawDistanceMeters.ToString(CultureInfo.InvariantCulture) + ", 0\n");
                if (!string.IsNullOrEmpty(manifest.AudioMaterial)) { amat.Append(manifest.Name + ", 0, " + manifest.AudioMaterial + "\n"); }
            }
            if (failed > 0) { Console.WriteLine("package: " + failed + " asset(s) failed; nothing packed"); return 2; }
            ImgArchive.Write(Path.Combine(output, imgName), files);
            File.WriteAllText(Path.Combine(output, ideName), ide + "end\n" + amat + "end\n");
            Console.WriteLine("package " + imgName + " entries=" + files.Count + ", " + ideName + " written");
            return 0;
        }

        // Single-geometry drawables drawn with one textured gta_default shader and the position/normal/colour/uv layout,
        // with their texture's size (usable as prop templates).
        private static int Templates(string game, string archiveName)
        {
            ArchiveSource archive = ArchiveSource.Open(game, archiveName);
            int found = 0;
            foreach (string name in archive.Names.Where(n => n.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)).OrderBy(n => n))
            {
                try
                {
                    DrawableFile file = new DrawableFile(RscResource.Parse(archive.Extract(name), true));
                    if (file.Models.Count != 1 || file.Models[0].Geometries.Count != 1 || file.Shaders.Count != 1 || file.Shaders[0].Textures.Count != 1) { continue; }
                    if (file.Models[0].Geometries[0].Layout.Mask != 0x59) { continue; }
                    string model = Path.GetFileNameWithoutExtension(name);
                    string wtd = archive.Names.FirstOrDefault(n => string.Equals(n, model + ".wtd", StringComparison.OrdinalIgnoreCase));
                    if (wtd == null) { continue; }
                    TextureDictionary.Texture texture = TextureDictionary.Parse(RscResource.Parse(archive.Extract(wtd))).Textures
                        .FirstOrDefault(t => string.Equals(t.Name, file.Shaders[0].Textures[0], StringComparison.OrdinalIgnoreCase));
                    if (texture == null) { continue; }
                    found++;
                    Console.WriteLine(model + " shader=" + file.Shaders[0].Name + " texture=" + texture.Name + " " + texture.Width + "x" + texture.Height + " " + texture.Format + " levels=" + texture.Levels);
                }
                catch (Exception error) { Console.WriteLine("skip " + name + ": " + error.Message); }
            }
            Console.WriteLine("templates found=" + found);
            return 0;
        }

        // ", "textureQuality": {...}" for native-mode builds that were read back, else nothing.
        private static string TextureQualityJson(TextureQuality quality)
        {
            if (quality == null) { return ""; }
            string json = ", \"textureQuality\": { \"psnrRgbDb\": " + Number(quality.PsnrRgbDb) + ", \"maxErrorRgb\": " + quality.MaxErrorRgb;
            if (quality.HasAlpha) { json += ", \"psnrAlphaDb\": " + Number(quality.PsnrAlphaDb) + ", \"maxErrorAlpha\": " + quality.MaxErrorAlpha; }
            return json + " }";
        }

        private static string Number(double value) { return value.ToString("0.00", CultureInfo.InvariantCulture); }

        private static string Quote(string text)
        {
            if (text == null) { return "null"; }
            return "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
        }
    }
}
