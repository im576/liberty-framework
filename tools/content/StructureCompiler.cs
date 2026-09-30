using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // drawableWriter "structure" (NEEDS-PLAYTEST, T-031): compiles every LOD of the asset, one geometry per material, into a
    // structure template with DrawableStructureBuilder, and one native texture per material into a dictionary written from
    // scratch. The template's first model in LOD slot l holds the asset's LOD l: its geometry j gets the asset's material
    // group j (ContentLod.Groups order). The template decides how many LODs and materials fit; Match says why one does not.
    internal static class StructureCompiler
    {
        // What the asset needs from a template.
        internal sealed class Need
        {
            // LOD level -> the material index of each geometry (material groups with triangles, in order).
            internal readonly SortedDictionary<int, List<int>> Levels = new SortedDictionary<int, List<int>>();
            // Material index -> shader name (-1, no material: gta_default).
            internal readonly Dictionary<int, string> Shaders = new Dictionary<int, string>();

            internal string Describe()
            {
                return string.Join(", ", Levels.Select(l => "LOD " + l.Key + ": " + l.Value.Count + " geometr" + (l.Value.Count == 1 ? "y" : "ies")).ToArray());
            }
        }

        internal static Need NeedOf(ContentAsset asset)
        {
            Need need = new Need();
            foreach (ContentLod lod in asset.BuildLods())
            {
                List<int> materials = lod.Groups.Where(g => g.TriangleCount > 0).Select(g => g.Material).ToList();
                if (materials.Count == 0) { continue; }
                need.Levels[lod.Level] = materials;
                foreach (int material in materials)
                {
                    need.Shaders[material] = material >= 0 && material < asset.Materials.Count ? asset.Materials[material].Shader : "gta_default";
                }
            }
            return need;
        }

        // Why the drawable cannot hold what the asset needs, or null. On success shaderMaterial maps each shader index the
        // kept geometries use to the material it will show. Cheap structural checks come before the bounds measurement.
        internal static string Match(DrawableFile file, Need need, out Dictionary<int, int> shaderMaterial)
        {
            shaderMaterial = new Dictionary<int, int>();
            if (file.Skeleton != 0) { return "has a skeleton"; }
            if (file.EmbeddedTextures != 0) { return "has an embedded texture dictionary (its textures could shadow the asset's)"; }
            foreach (KeyValuePair<int, List<int>> level in need.Levels)
            {
                DrawableModel model = file.Models.FirstOrDefault(m => m.Lod == level.Key);
                if (model == null) { return "no model in LOD slot " + level.Key; }
                if (model.Geometries.Count < level.Value.Count) { return "LOD " + level.Key + " has " + model.Geometries.Count + " geometries, the asset needs " + level.Value.Count; }
                for (int j = 0; j < level.Value.Count; j++)
                {
                    DrawableGeometry g = model.Geometries[j];
                    if (g.Layout.Mask != 0x59) { return "LOD " + level.Key + " geometry " + j + " has vertex layout " + g.Layout.Describe(); }
                    if (g.BoneCount != 0) { return "LOD " + level.Key + " geometry " + j + " is skinned"; }
                    if (g.ShaderIndex >= file.Shaders.Count) { return "LOD " + level.Key + " geometry " + j + " has no shader"; }
                    DrawableShader shader = file.Shaders[g.ShaderIndex];
                    int material = level.Value[j];
                    if (shader.TextureNameSlots.Count != 1) { return "shader " + g.ShaderIndex + " has " + shader.TextureNameSlots.Count + " textures (one is written)"; }
                    if (!string.Equals(shader.Name, need.Shaders[material], StringComparison.OrdinalIgnoreCase)) { return "shader " + g.ShaderIndex + " is " + shader.Name + ", the material needs " + need.Shaders[material]; }
                    int other;
                    if (shaderMaterial.TryGetValue(g.ShaderIndex, out other) && other != material) { return "shader " + g.ShaderIndex + " would show two materials"; }
                    shaderMaterial[g.ShaderIndex] = material;
                }
            }
            return DrawableStructureBuilder.Unsupported(file);
        }

        private sealed class Template
        {
            internal string Name;
            internal DrawableFile File;
            internal Dictionary<int, int> ShaderMaterial;
        }

        // The compiled asset, or null with fallback set when an "auto" search found no template (PropCompiler then
        // decides whether v1 can build it). An explicit template that does not fit is an error, never a fallback.
        internal static PropCompiler.Result Compile(string game, AssetManifest manifest, ContentAsset asset, out string fallback)
        {
            fallback = null;
            Need need = NeedOf(asset);
            PropCompiler.Result result = new PropCompiler.Result { DrawableWriter = AssetManifest.WriterStructure, TextureMode = AssetManifest.TextureModeNative };
            AssetManifest.TemplateRef reference = manifest.StructureTemplateOrDefault;
            bool auto = string.Equals(reference.Model, AssetManifest.AutoTemplate, StringComparison.OrdinalIgnoreCase);
            // An auto search moves on when a drawable that matched still cannot be written (for example no room for a texture
            // name): one asset must not fail the whole package. An explicit template's failure is the error.
            foreach (Template template in Find(game, reference, need, result.Notes))
            {
                try { return Build(game, manifest, asset, need, template, result); }
                catch (InvalidDataException error)
                {
                    if (!auto) { throw; }
                    result.Notes.Add("template " + template.Name + " matched but could not be written (" + error.Message + "); trying the next");
                    result.Parts.Clear(); result.Textures.Clear(); result.TextureSources.Clear(); result.FlippedWinding = false;
                }
            }
            fallback = "structure writer: no drawable in " + reference.Archive + " fits (" + need.Describe() + ")";
            return null;
        }

        private static PropCompiler.Result Build(string game, AssetManifest manifest, ContentAsset asset, Need need, Template template, PropCompiler.Result result)
        {
            result.TemplateUsed = template.Name;

            // Meshes per LOD and material group, in the template's winding.
            Dictionary<int, List<Mesh>> meshes = new Dictionary<int, List<Mesh>>();
            foreach (ContentLod lod in asset.BuildLods())
            {
                if (!need.Levels.ContainsKey(lod.Level)) { continue; }
                meshes[lod.Level] = lod.Groups.Where(g => g.TriangleCount > 0).Select(g => PropCompiler.MergeMeshes(g.Meshes, result.Notes)).ToList();
            }
            DrawableModel referenceModel = template.File.Models.First(m => m.Lod == need.Levels.Keys.First());
            Mesh templateMesh = template.File.ReadMesh(referenceModel.Geometries[0]);
            double ours = meshes.Values.SelectMany(l => l).Sum(m => PropCompiler.WindingScore(m));
            if (Math.Sign(PropCompiler.WindingScore(templateMesh)) != Math.Sign(ours) && ours != 0)
            {
                foreach (Mesh mesh in meshes.Values.SelectMany(l => l))
                {
                    for (int i = 0; i + 2 < mesh.Indices.Count; i += 3) { int t = mesh.Indices[i + 1]; mesh.Indices[i + 1] = mesh.Indices[i + 2]; mesh.Indices[i + 2] = t; }
                }
                result.FlippedWinding = true;
                result.Notes.Add("triangle winding flipped to the template's convention");
            }

            // One texture per material, named after the asset.
            List<int> materials = need.Levels.Values.SelectMany(l => l).Distinct().ToList();
            Dictionary<int, string> textureNames = new Dictionary<int, string>();
            List<string> textureFormats = new List<string>();
            for (int k = 0; k < materials.Count; k++)
            {
                ContentMaterial material = materials[k] >= 0 && materials[k] < asset.Materials.Count ? asset.Materials[materials[k]] : null;
                string format;
                RgbaImage pixels = PropCompiler.MaterialPixels(asset, material, result.Notes, out format);
                // Materials that resolve to identical pixels share one texture: fewer dictionary entries, less memory.
                int shared = -1;
                for (int earlier = 0; earlier < result.TextureSources.Count && shared < 0; earlier++)
                {
                    RgbaImage other = result.TextureSources[earlier];
                    if (textureFormats[earlier] == format && other.Width == pixels.Width && other.Height == pixels.Height && other.Pixels.SequenceEqual(pixels.Pixels)) { shared = earlier; }
                }
                if (shared >= 0)
                {
                    textureNames[materials[k]] = result.Textures[shared].Name;
                    result.Notes.Add("material " + materials[k] + " shares texture " + result.Textures[shared].Name + " (identical pixels)");
                    continue;
                }
                textureNames[materials[k]] = TextureName(manifest.Name, result.Textures.Count);
                result.Textures.Add(TextureEncoder.Encode(textureNames[materials[k]], pixels, format, 0));
                result.TextureSources.Add(pixels);
                textureFormats.Add(format);
            }

            DrawableStructureBuilder.Plan plan = new DrawableStructureBuilder.Plan();
            HashSet<int> firstModels = new HashSet<int>();
            foreach (DrawableModel model in template.File.Models)
            {
                bool first = need.Levels.ContainsKey(model.Lod) && firstModels.Add(model.Lod);
                plan.ModelMeshes.Add(first ? meshes[model.Lod] : new List<Mesh>());
                if (!first) { continue; }
                for (int j = 0; j < meshes[model.Lod].Count; j++)
                {
                    result.Parts.Add(new PropCompiler.Part { Lod = model.Lod, Material = need.Levels[model.Lod][j], Mesh = meshes[model.Lod][j], TextureName = textureNames[need.Levels[model.Lod][j]] });
                }
            }
            plan.ShaderCount = template.ShaderMaterial.Keys.Max() + 1;
            // Every kept shader gets one of the asset's textures, so no template texture name is left to resolve.
            plan.ShaderTextures = Enumerable.Range(0, plan.ShaderCount)
                .Select(s => template.ShaderMaterial.ContainsKey(s) ? textureNames[template.ShaderMaterial[s]] : textureNames[materials[0]]).ToArray();
            if (manifest.LodDistancesMeters != null)
            {
                plan.LodDistances = Enumerable.Repeat(float.NaN, CompilerCapabilities.DrawableLodSlots).ToArray();
                for (int lod = 0; lod < manifest.LodDistancesMeters.Length && lod < plan.LodDistances.Length; lod++) { plan.LodDistances[lod] = manifest.LodDistancesMeters[lod]; }
                result.LodDistancesWritten = plan.LodDistances;
            }
            if (meshes.Values.Any(l => l.Count > 1))
            {
                result.Notes.Add("WARNING: more than one geometry per LOD. Every in-game spawn of such a drawable crashed the game (T-031: two materials, " +
                    "two textures or one shared, 2026-09-29); one geometry per LOD spawned fine. Use one material (a texture atlas) until this is fixed.");
            }
            result.Drawable = DrawableStructureBuilder.Build(template.File, plan).Resource.Serialize();

            TextureDictionaryPrototype prototype = Prototype(game, manifest, result.Notes);
            result.Dictionary = TextureDictionaryWriter.Write(result.Textures, prototype).Resource.Serialize();
            NativeTexture first0 = result.Textures[0];
            result.TextureName = first0.Name; result.TextureWidth = first0.Width; result.TextureHeight = first0.Height;
            result.TextureFormat = first0.Format; result.TextureLevels = first0.Levels.Count;
            result.EncodedTexture = first0; result.SourceImage = result.TextureSources[0];
            result.Mesh = PropCompiler.MergeMeshes(asset.Lod(need.Levels.Keys.First()), new List<string>());
            result.Notes.Add("structure writer: template " + template.Name + ", " + need.Describe() + ", " + result.Textures.Count + " texture" + (result.Textures.Count == 1 ? "" : "s"));
            return result;
        }

        // Texture k of the asset: its name, then _1, _2 ... (1-23 characters, the name limit the manifest uses).
        internal static string TextureName(string asset, int k)
        {
            if (k == 0) { return asset; }
            string suffix = "_" + k;
            return (asset.Length + suffix.Length > 23 ? asset.Substring(0, 23 - suffix.Length) : asset) + suffix;
        }

        // Drawables that fit, in search order (an explicit reference yields it or throws why not).
        private static IEnumerable<Template> Find(string game, AssetManifest.TemplateRef reference, Need need, List<string> notes)
        {
            byte[] key = null;
            try { key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe")); }
            catch (InvalidDataException error) { notes.Add("no IMG key (" + error.Message + "): encrypted archives cannot be searched"); }
            bool auto = string.Equals(reference.Model, AssetManifest.AutoTemplate, StringComparison.OrdinalIgnoreCase);
            List<string> archives = reference.Archive == AssetManifest.AnyArchive ? Probe.Archives(game, new List<string>()) : new List<string> { reference.Archive };
            int examined = 0;
            foreach (string archiveName in archives)
            {
                ImgArchive archive;
                try { archive = ImgArchive.Open(Path.Combine(game, archiveName), key); }
                catch (Exception error)
                {
                    if (!auto) { throw; }
                    notes.Add("template search skipped " + archiveName + ": " + error.Message);
                    continue;
                }
                IEnumerable<ImgArchive.Entry> entries = archive.Entries.Where(e => e.Name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase));
                entries = auto ? entries.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
                    : entries.Where(e => string.Equals(Path.GetFileNameWithoutExtension(e.Name), reference.Model, StringComparison.OrdinalIgnoreCase));
                foreach (ImgArchive.Entry entry in entries)
                {
                    examined++;
                    DrawableFile file;
                    string reason;
                    Dictionary<int, int> shaderMaterial = null;
                    try
                    {
                        file = new DrawableFile(RscResource.Parse(archive.Extract(entry.Name), true));
                        reason = Match(file, need, out shaderMaterial);
                    }
                    catch (Exception error) { file = null; reason = "does not parse: " + error.Message; }
                    string name = archiveName + "/" + Path.GetFileNameWithoutExtension(entry.Name);
                    if (reason == null)
                    {
                        if (auto) { notes.Add("template search: " + name + " fits (" + examined + " drawables examined)"); }
                        yield return new Template { Name = name, File = file, ShaderMaterial = shaderMaterial };
                        if (!auto) { yield break; }
                        continue;
                    }
                    if (!auto) { throw new InvalidDataException("structure template " + name + " does not fit the asset (" + need.Describe() + "): " + reason); }
                }
            }
            if (!auto && examined == 0) { throw new InvalidDataException("structure template " + reference.Model + " not found in " + reference.Archive); }
        }

        // Prototype bytes for the texture dictionary: captured from the v1 template's own dictionary when it has one, as
        // native mode does, otherwise the builtin prototype.
        private static TextureDictionaryPrototype Prototype(string game, AssetManifest manifest, List<string> notes)
        {
            string path = manifest.Template.Model + ".wtd";
            try
            {
                byte[] key = null;
                try { key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe")); }
                catch (InvalidDataException) { key = null; } // unencrypted archives still open; the search noted the missing key
                ImgArchive archive = ImgArchive.Open(Path.Combine(game, manifest.Template.Archive), key);
                if (archive.Entries.Any(e => string.Equals(e.Name, path, StringComparison.OrdinalIgnoreCase)))
                {
                    TextureDictionaryPrototype prototype = TextureDictionaryPrototype.FromResource(RscResource.Parse(archive.Extract(path)), path);
                    List<string> differences = prototype.OpaqueDifferences(TextureDictionaryPrototype.Builtin());
                    if (differences.Count > 0) { notes.Add("template dictionary's copied bytes differ from the builtin prototype at " + string.Join(", ", differences.ToArray())); }
                    return prototype;
                }
                notes.Add(path + " not in " + manifest.Template.Archive + ": builtin dictionary prototype");
            }
            catch (Exception error) { notes.Add("template dictionary unreadable (" + error.Message + "): builtin dictionary prototype"); }
            return TextureDictionaryPrototype.Builtin();
        }
    }
}
