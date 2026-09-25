using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // LibertyModel: T-2 model pipeline for GTA IV drawables (.wdr).
    //   survey <game> <img...>                 parse every .wdr in the IMGs and report layouts, shaders and failures
    //   export <game> <img> <model> <out.obj>  write a drawable's high-LOD geometry as OBJ
    //   build ...                              see DrawableBuilder / docs/research/ModelFormat.md
    // Reads the player's own game files; nothing from the game is committed.
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length >= 3 && args[0] == "survey") { return Survey(args[1], args.Skip(2).ToArray()); }
                if (args.Length == 5 && args[0] == "export") { return Export(args[1], args[2], args[3], args[4]); }
                if (args.Length == 3 && args[0] == "selftest") { return SelfTest(args[1], args[2]); }
                if (args.Length == 4 && args[0] == "sling") { return Sling(args[1], args[2], args[3]); }
                Console.WriteLine("usage: LibertyModel survey <game> <img...> | export <game> <img> <model> <out.obj>");
                return 2;
            }
            catch (Exception error)
            {
                Console.WriteLine("ERROR " + error.Message);
                return 1;
            }
        }


        private static int Survey(string game, string[] imgs)
        {
            int ok = 0, failed = 0, singleGeometry = 0;
            Dictionary<string, int> failures = new Dictionary<string, int>();
            Dictionary<string, int> layouts = new Dictionary<string, int>();
            Dictionary<string, int> shaders = new Dictionary<string, int>();
            foreach (string img in imgs)
            {
                ArchiveSource archive = ArchiveSource.Open(game, img);
                foreach (string name in archive.Names)
                {
                    if (!name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)) { continue; }
                    try
                    {
                        DrawableFile file = new DrawableFile(RscResource.Parse(archive.Extract(name), true));
                        foreach (DrawableModel model in file.Models)
                        {
                            foreach (DrawableGeometry g in model.Geometries)
                            {
                                file.ReadMesh(g);
                                Count(layouts, g.Layout.Describe());
                            }
                        }
                        foreach (DrawableShader shader in file.Shaders) { Count(shaders, shader.Preset); }
                        if (file.Models.Count == 1 && file.Models[0].Geometries.Count == 1) { singleGeometry++; }
                        ok++;
                    }
                    catch (Exception error)
                    {
                        failed++;
                        string key = error.Message.Length > 90 ? error.Message.Substring(0, 90) : error.Message;
                        Count(failures, key);
                        if (failures[key] == 1) { Console.WriteLine("  fail " + name + ": " + error.Message); }
                    }
                }
            }
            Console.WriteLine("layouts:");
            foreach (KeyValuePair<string, int> pair in layouts.OrderByDescending(p => p.Value)) { Console.WriteLine("  " + pair.Value + "  " + pair.Key); }
            Console.WriteLine("shader presets:");
            foreach (KeyValuePair<string, int> pair in shaders.OrderByDescending(p => p.Value).Take(25)) { Console.WriteLine("  " + pair.Value + "  " + pair.Key); }
            Console.WriteLine("failures:");
            foreach (KeyValuePair<string, int> pair in failures.OrderByDescending(p => p.Value)) { Console.WriteLine("  " + pair.Value + "  " + pair.Key); }
            Console.WriteLine("survey drawables ok=" + ok + " failed=" + failed + " single_geometry=" + singleGeometry);
            return failed == 0 ? 0 : 3;
        }

        private static int Export(string game, string img, string model, string output)
        {
            DrawableFile file = new DrawableFile(RscResource.Parse(ArchiveSource.Open(game, img).Extract(model), true));
            List<KeyValuePair<string, Mesh>> meshes = new List<KeyValuePair<string, Mesh>>();
            foreach (DrawableModel m in file.Models)
            {
                if (m.Lod != 0) { continue; }
                for (int i = 0; i < m.Geometries.Count; i++)
                {
                    DrawableGeometry g = m.Geometries[i];
                    string shader = g.ShaderIndex < file.Shaders.Count ? file.Shaders[g.ShaderIndex].Preset : "none";
                    meshes.Add(new KeyValuePair<string, Mesh>("geometry" + meshes.Count + "_" + shader, file.ReadMesh(g)));
                }
            }
            ObjFile.Write(output, meshes);
            MeshPreview.Save(meshes.Select(p => p.Value).ToList(), Path.ChangeExtension(output, ".png"), model);
            foreach (DrawableShader shader in file.Shaders) { Console.WriteLine("shader " + shader.Preset + " textures=" + string.Join(",", shader.Textures.ToArray())); }
            Console.WriteLine("exported " + meshes.Count + " geometries to " + output);
            return 0;
        }

        // W-5: builds the sling straps described by config/models/sling.json into <out>\<model>.wdr, plus an OBJ and a
        // preview of the straps on the union of Niko's outfits (in the attach bone's space).
        private static int Sling(string game, string configPath, string output)
        {
            SlingConfig config = SlingConfig.Load(configPath);
            Directory.CreateDirectory(output);
            ArchiveSource bodyArchive = ArchiveSource.Open(game, config.BodyArchive);
            DrawableFile skeletonFile = new DrawableFile(RscResource.Parse(bodyArchive.Extract(config.SkeletonModel), true));
            Skeleton skeleton = new Skeleton(skeletonFile.View, skeletonFile.Skeleton);
            if (skeleton.WorstPositionError > 1e-4) { throw new InvalidDataException("skeleton rebuild error " + skeleton.WorstPositionError); }
            int attach = Bone(skeleton, config.AttachBone);

            List<Mesh> body = new List<Mesh>();
            foreach (string name in bodyArchive.Names.Where(n => n.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase) &&
                config.BodyPrefixes.Any(p => n.StartsWith(p, StringComparison.OrdinalIgnoreCase))).OrderBy(n => n))
            {
                DrawableFile file = new DrawableFile(RscResource.Parse(bodyArchive.Extract(name), true));
                foreach (DrawableModel model in file.Models.Where(m => m.Lod == 0))
                {
                    foreach (DrawableGeometry g in model.Geometries) { body.Add(file.ReadMesh(g)); }
                }
            }
            Console.WriteLine("body meshes=" + body.Count + " skeleton bones=" + skeleton.Bones.Count + " fk_error=" + skeleton.WorstPositionError.ToString("E1"));

            DrawableFile template = new DrawableFile(RscResource.Parse(ArchiveSource.Open(game, config.TemplateArchive).Extract(config.TemplateModel + ".wdr"), true));
            Skeleton.Bone bone = skeleton.Bones[attach];
            Quat toBone = bone.Rotation.Conjugate();
            List<Mesh> preview = new List<Mesh>(body);
            List<Color> tints = body.Select(m => Color.FromArgb(150, 150, 160)).ToList();
            foreach (SlingConfig.Strap strap in config.Straps)
            {
                Vec3 shoulderA = skeleton.Bones[Bone(skeleton, strap.ShoulderBone)].Position, shoulderB = skeleton.Bones[Bone(skeleton, strap.ShoulderTowards)].Position;
                StrapDesigner.Spec spec = new StrapDesigner.Spec();
                spec.ShoulderPoint = shoulderA + (shoulderB - shoulderA) * strap.ShoulderFraction;
                spec.HipPoint = skeleton.Bones[Bone(skeleton, strap.HipBone)].Position + new Vec3(strap.HipOffsetMeters[0], strap.HipOffsetMeters[1], strap.HipOffsetMeters[2]);
                spec.ClearanceMeters = config.ClearanceMeters; spec.HipMarginMeters = config.HipMarginMeters; spec.WidthMeters = config.WidthMeters; spec.ThicknessMeters = config.ThicknessMeters;
                spec.TextureRepeatMeters = config.TextureRepeatMeters; spec.Segments = config.Segments;
                List<Vec3> path;
                Mesh mesh = StrapDesigner.Design(body, skeleton, attach, spec, out path);
                RscResource built = DrawableBuilder.Build(template, mesh, null);
                File.WriteAllBytes(Path.Combine(output, strap.Model + ".wdr"), built.Serialize());
                // Prove the written file reads back to the same geometry.
                DrawableFile check = new DrawableFile(RscResource.Parse(built.Serialize()));
                Mesh back = check.ReadMesh(check.Models[0].Geometries[0]);
                if (back.Vertices.Count != mesh.Vertices.Count || back.Indices.Count != mesh.Indices.Count) { throw new InvalidDataException(strap.Model + " did not read back"); }
                ObjFile.Write(Path.Combine(output, strap.Model + ".obj"), new List<KeyValuePair<string, Mesh>> { new KeyValuePair<string, Mesh>(strap.Model, mesh) });
                preview.Add(Transform(mesh, v => bone.Rotation.Rotate(v) + bone.Position, v => bone.Rotation.Rotate(v)));
                tints.Add(Color.FromArgb(150, 95, 55));
                double length = 0;
                for (int i = 0; i < path.Count; i++) { length += (path[(i + 1) % path.Count] - path[i]).Length; }
                Console.WriteLine(strap.Model + ": vertices=" + mesh.Vertices.Count + " triangles=" + mesh.Indices.Count / 3 + " loop=" + length.ToString("0.000") + " m, " + (built.Body.Length / 1024) + " KB");
            }
            MeshPreview.Save(preview, Path.Combine(output, "sling_preview.png"), "Niko outfits + straps (model space)", tints);
            WriteStrapTexture(game, config, template, output);

            // Package: one IMG (FusionFix loads update\<folder>\*.img) and the IDE that registers the models.
            List<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            StringBuilder ide = new StringBuilder("# Liberty Framework models (generated by tools/models from config/models/sling.json)\nweap\n");
            StringBuilder amat = new StringBuilder("amat\n");
            foreach (SlingConfig.Strap strap in config.Straps)
            {
                files.Add(new KeyValuePair<string, byte[]>(strap.Model + ".wdr", File.ReadAllBytes(Path.Combine(output, strap.Model + ".wdr"))));
                ide.Append(strap.Model + ", " + config.TextureDictionary + ", null, 1, " + config.DrawDistanceMeters + ", 0\n");
                amat.Append(strap.Model + ", 0, " + config.AudioMaterial + "\n");
            }
            files.Add(new KeyValuePair<string, byte[]>(config.TextureDictionary + ".wtd", File.ReadAllBytes(Path.Combine(output, config.TextureDictionary + ".wtd"))));
            ImgArchive.Write(Path.Combine(output, "LibertyModels.img"), files);
            File.WriteAllText(Path.Combine(output, "lf_models.ide"), ide + "end\n" + amat + "end\n");
            Console.WriteLine("LibertyModels.img entries=" + files.Count + ", lf_models.ide written");
            return 0;
        }

        // The straps keep the template's texture reference; their own dictionary holds a texture of that name with the
        // same size and format as the template's, so the template dictionary is reused with new pixels.
        private static void WriteStrapTexture(string game, SlingConfig config, DrawableFile template, string output)
        {
            string textureName = template.Shaders[0].Textures[0];
            RscResource resource = RscResource.Parse(ArchiveSource.Open(game, config.TemplateArchive).Extract(config.TemplateModel + ".wtd"));
            TextureDictionary dictionary = TextureDictionary.Parse(resource);
            TextureDictionary.Texture texture = dictionary.Textures.FirstOrDefault(t => string.Equals(t.Name, textureName, StringComparison.OrdinalIgnoreCase));
            if (texture == null || texture.Format != "DXT1") { throw new InvalidDataException("template dictionary has no DXT1 texture " + textureName); }
            SlingConfig.Leather l = config.LeatherLook;
            StrapTexture.Look look = new StrapTexture.Look();
            look.BaseColour = l.BaseColour; look.EdgeColour = l.EdgeColour; look.StitchColour = l.StitchColour;
            look.GrainStrength = l.GrainStrength; look.ScuffStrength = l.ScuffStrength;
            look.StitchInsetFraction = l.StitchInsetFraction; look.StitchPeriodPixels = l.StitchPeriodPixels;
            byte[] rgb = StrapTexture.Generate(texture.Width, texture.Height, look, 1);
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
            File.WriteAllBytes(Path.Combine(output, config.TextureDictionary + ".wtd"), file);
            DxtPreview.Save(resource.Body, texture, Path.Combine(output, config.TextureDictionary + "_texture.png"));
            Console.WriteLine(config.TextureDictionary + ".wtd: " + texture.Name + " " + texture.Width + "x" + texture.Height + " DXT1 levels=" + texture.Levels);
        }

        private static int Bone(Skeleton skeleton, string name)
        {
            int index = skeleton.Find(name);
            if (index < 0) { throw new InvalidDataException("bone not found: " + name); }
            return index;
        }

        private static Mesh Transform(Mesh source, Func<Vec3, Vec3> point, Func<Vec3, Vec3> direction)
        {
            Mesh mesh = new Mesh();
            foreach (Mesh.Vertex v in source.Vertices)
            {
                Vec3 p = point(new Vec3(v.X, v.Y, v.Z)), n = direction(new Vec3(v.NX, v.NY, v.NZ));
                Mesh.Vertex t = v;
                t.X = (float)p.X; t.Y = (float)p.Y; t.Z = (float)p.Z; t.NX = (float)n.X; t.NY = (float)n.Y; t.NZ = (float)n.Z;
                mesh.Vertices.Add(t);
            }
            mesh.Indices.AddRange(source.Indices);
            return mesh;
        }

        // Rebuilds every single-geometry template drawable from its own mesh and compares it with the original.
        // Identical output proves the reader and writer agree with the game's files field for field.
        private static int SelfTest(string game, string img)
        {
            ArchiveSource archive = ArchiveSource.Open(game, img);
            int identical = 0, tested = 0;
            Dictionary<string, int> differences = new Dictionary<string, int>();
            foreach (string name in archive.Names)
            {
                if (!name.EndsWith(".wdr", StringComparison.OrdinalIgnoreCase)) { continue; }
                RscResource original = RscResource.Parse(archive.Extract(name), true);
                DrawableFile file = new DrawableFile(original);
                if (file.Models.Count != 1 || file.Models[0].Geometries.Count != 1 || file.Shaders.Count != 1 ||
                    file.Shaders[0].TextureNameSlots.Count != 1 || file.Models[0].Geometries[0].Layout.Mask != 0x59) { continue; }
                tested++;
                DrawableGeometry g = file.Models[0].Geometries[0];
                List<string> diffs = new List<string>();
                try
                {
                    RscResource rebuilt = DrawableBuilder.Build(file, file.ReadMesh(g), null);
                    // Bounds are compared with a tolerance: the game's exporter stored pre-quantisation bounds, while
                    // the builder derives them from the (quantised) vertices. Everything else must match exactly.
                    HashSet<int> boundsBytes = new HashSet<int>();
                    foreach (int start in new[] { DrawableFile.DrawableCenter, DrawableFile.DrawableMin, DrawableFile.DrawableMax })
                    {
                        for (int i = 0; i < 12; i++) { boundsBytes.Add(start + i); }
                    }
                    for (int i = 0; i < 4; i++) { boundsBytes.Add(DrawableFile.DrawableRadius + i); }
                    int sphere = (int)(file.Models[0].Bounds - ResourceView.SystemBase);
                    for (int i = 0; i < 16; i++) { boundsBytes.Add(sphere + i); }
                    CheckBounds(file.View, new ResourceView(rebuilt), file.Models[0].Bounds, diffs);
                    for (int i = 0; i < original.SystemSize; i++)
                    {
                        if (!boundsBytes.Contains(i) && original.Body[i] != rebuilt.Body[i]) { diffs.Add("sys+0x" + (i & ~3).ToString("X")); i = (i | 3); }
                    }
                    int vertexEnd = (int)(g.VertexData - ResourceView.GraphicsBase) + g.VertexCount * g.Layout.Stride;
                    int indexEnd = (int)(g.IndexData - ResourceView.GraphicsBase) + g.IndexCount * 2;
                    if (g.VertexData != ResourceView.GraphicsBase) { diffs.Add("vertex data not at graphics start"); }
                    for (int i = 0; i < Math.Max(vertexEnd, indexEnd) && i < rebuilt.GraphicsSize; i++)
                    {
                        bool inData = i < vertexEnd || (i >= (int)(g.IndexData - ResourceView.GraphicsBase) && i < indexEnd);
                        if (inData && original.Body[original.SystemSize + i] != rebuilt.Body[rebuilt.SystemSize + i]) { diffs.Add("gfx+0x" + (i & ~0xF).ToString("X")); i = (i | 0xF); }
                    }
                }
                catch (Exception error) { diffs.Add("error " + error.Message); }
                if (diffs.Count == 0) { identical++; continue; }
                string summary = string.Join(" ", diffs.Take(6).ToArray()) + (diffs.Count > 6 ? " (+" + (diffs.Count - 6) + ")" : "");
                Console.WriteLine("  " + name + ": " + summary);
                Count(differences, diffs.Count == 1 ? diffs[0] : "multiple");
            }
            Console.WriteLine("selftest rebuilt=" + tested + " identical=" + identical);
            return identical == tested ? 0 : 4;
        }

        // The rebuilt box must contain the stored one within 1 mm, and the rebuilt sphere must contain the stored box.
        private static void CheckBounds(ResourceView original, ResourceView rebuilt, uint sphere, List<string> diffs)
        {
            const float Tolerance = 0.001f;
            uint root = ResourceView.SystemBase;
            for (int axis = 0; axis < 3; axis++)
            {
                uint offset = (uint)(axis * 4);
                if (rebuilt.F32(root + DrawableFile.DrawableMin + offset) > original.F32(root + DrawableFile.DrawableMin + offset) + Tolerance ||
                    rebuilt.F32(root + DrawableFile.DrawableMax + offset) < original.F32(root + DrawableFile.DrawableMax + offset) - Tolerance)
                { diffs.Add("box axis " + axis); }
            }
            if (sphere == 0) { return; }
            float radius = rebuilt.F32(sphere + 12);
            for (int corner = 0; corner < 8; corner++)
            {
                double squared = 0;
                for (int axis = 0; axis < 3; axis++)
                {
                    uint pick = (corner & (1 << axis)) != 0 ? (uint)DrawableFile.DrawableMax : (uint)DrawableFile.DrawableMin;
                    double d = original.F32(root + pick + (uint)(axis * 4)) - rebuilt.F32(sphere + (uint)(axis * 4));
                    squared += d * d;
                }
                if (Math.Sqrt(squared) > radius + Tolerance) { diffs.Add("sphere misses box corner"); return; }
            }
        }

        private static void Count(Dictionary<string, int> table, string key)
        {
            int value;
            table.TryGetValue(key, out value);
            table[key] = value + 1;
        }
    }
}
