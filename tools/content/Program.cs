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
    //   validate <asset.json>                            import + validate, print issues (exit 2 on errors)
    //   build <game> <asset.json> <out>                  validate, compile, read back, preview, report
    //   package <game> <out> <img> <ide> <asset.json...> build every asset, then one IMG and its IDE
    //   templates <game> <archive>                       list drawables usable as prop templates
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length >= 3 && args[0] == "sample") { SampleAsset.Write(args[1], args[2]); Console.WriteLine("sample written: " + Path.Combine(args[1], args[2] + ".gltf")); return 0; }
                if (args.Length >= 2 && args[0] == "validate") { return Validate(args[1]); }
                if (args.Length >= 4 && args[0] == "build") { string ignored; return Build(args[1], args[2], args[3], out ignored); }
                if (args.Length >= 6 && args[0] == "package") { return Package(args[1], args[2], args[3], args[4], args.Skip(5).ToArray()); }
                if (args.Length >= 3 && args[0] == "templates") { return Templates(args[1], args[2]); }
                Console.WriteLine("usage: LibertyContent sample <dir> <name> | validate <asset.json> | build <game> <asset.json> <out> |");
                Console.WriteLine("       package <game> <out> <img> <ide> <asset.json...> | templates <game> <archive>");
                return 1;
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR " + error.GetType().Name + ": " + error.Message);
                return 3;
            }
        }

        private static int Validate(string manifestPath)
        {
            AssetManifest manifest = AssetManifest.Load(manifestPath);
            ContentAsset asset = GltfImporter.Import(manifest.SourcePath);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, 1);
            foreach (AssetValidator.Issue issue in issues) { Console.WriteLine(issue); }
            Console.WriteLine("validate " + manifest.Name + ": " + (AssetValidator.HasErrors(issues) ? "FAILED" : "ok") + " (" + issues.Count + " issues)");
            return AssetValidator.HasErrors(issues) ? 2 : 0;
        }

        private static int Build(string game, string manifestPath, string output, out string status)
        {
            AssetManifest manifest = AssetManifest.Load(manifestPath);
            string folder = Path.Combine(output, manifest.Name);
            Directory.CreateDirectory(folder);
            ContentAsset asset = GltfImporter.Import(manifest.SourcePath);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, 1);
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
            status = compiled == null ? "invalid" : readback.Count == 0 ? "ok" : "readback-failed";
            WriteReport(folder, manifest, asset, issues, compiled, readback, status);
            Console.WriteLine("build " + manifest.Name + ": " + status + (compiled != null ? " vertices=" + compiled.Mesh.Vertices.Count + " triangles=" +
                compiled.Mesh.Indices.Count / 3 + " texture=" + compiled.TextureName + " " + compiled.TextureWidth + "x" + compiled.TextureHeight : "") +
                " report=" + Path.Combine(folder, "report.json"));
            foreach (string problem in readback) { Console.WriteLine("  READBACK " + problem); }
            return status == "ok" ? 0 : 2;
        }

        private static void WritePreviews(string folder, AssetManifest manifest, PropCompiler.Result compiled)
        {
            // The preview is drawn from the geometry read back out of the compiled .wdr, i.e. what the game will load.
            DrawableFile drawable = new DrawableFile(RscResource.Parse(compiled.Drawable));
            Mesh back = drawable.ReadMesh(drawable.Models[0].Geometries[0]);
            MeshPreview.Save(new List<Mesh> { back }, Path.Combine(folder, manifest.Name + "_preview.png"), manifest.Name + " (read back from .wdr)",
                new List<Color> { Color.FromArgb(170, 140, 100) });
            RscResource dictionary = RscResource.Parse(compiled.Dictionary);
            TextureDictionary.Texture texture = TextureDictionary.Parse(dictionary).Textures.First(t => t.Name == compiled.TextureName);
            DxtPreview.Save(dictionary.Body, texture, Path.Combine(folder, manifest.Name + "_texture.png"));
        }

        private static void WriteReport(string folder, AssetManifest manifest, ContentAsset asset, List<AssetValidator.Issue> issues,
            PropCompiler.Result compiled, List<string> readback, string status)
        {
            StringBuilder json = new StringBuilder("{\n");
            json.Append("  \"asset\": ").Append(Quote(manifest.Name)).Append(",\n");
            json.Append("  \"status\": ").Append(Quote(status)).Append(",\n");
            json.Append("  \"source\": ").Append(Quote(asset.SourcePath)).Append(",\n");
            json.Append("  \"template\": ").Append(Quote(manifest.Template.Archive + "/" + manifest.Template.Model)).Append(",\n");
            json.Append("  \"metadata\": {").Append(string.Join(", ", asset.Metadata.OrderBy(p => p.Key).Select(p => Quote(p.Key) + ": " + Quote(p.Value)).ToArray())).Append("},\n");
            json.Append("  \"meshes\": ").Append(asset.Meshes.Count).Append(", \"materials\": ").Append(asset.Materials.Count).Append(", \"lods\": ").Append(asset.MaxLod + 1).Append(",\n");
            if (compiled != null)
            {
                json.Append("  \"compiled\": { \"vertices\": ").Append(compiled.Mesh.Vertices.Count).Append(", \"triangles\": ").Append(compiled.Mesh.Indices.Count / 3)
                    .Append(", \"texture\": ").Append(Quote(compiled.TextureName)).Append(", \"textureSize\": [").Append(compiled.TextureWidth).Append(", ").Append(compiled.TextureHeight)
                    .Append("], \"flippedWinding\": ").Append(compiled.FlippedWinding ? "true" : "false").Append(", \"drawableBytes\": ").Append(compiled.Drawable.Length)
                    .Append(", \"dictionaryBytes\": ").Append(compiled.Dictionary.Length).Append(",\n    \"notes\": [").Append(string.Join(", ", compiled.Notes.Select(Quote).ToArray())).Append("] },\n");
            }
            json.Append("  \"issues\": [\n");
            json.Append(string.Join(",\n", issues.Select(i => "    { \"severity\": " + Quote(i.Severity) + ", \"code\": " + Quote(i.Code) + ", \"message\": " + Quote(i.Message) + " }").ToArray()));
            json.Append("\n  ],\n  \"readback\": [").Append(string.Join(", ", readback.Select(Quote).ToArray())).Append("]\n}\n");
            File.WriteAllText(Path.Combine(folder, "report.json"), json.ToString());
        }

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

        private static string Quote(string text)
        {
            if (text == null) { return "null"; }
            return "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
        }
    }
}
