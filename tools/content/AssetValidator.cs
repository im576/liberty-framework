using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LibertyFramework.Content
{
    // Checks an imported asset before compilation. Errors stop the build; warnings are reported (and some are fixed by the
    // compiler, e.g. missing normals are generated). Codes are stable so reports and tests can refer to them.
    internal static class AssetValidator
    {
        internal sealed class Issue
        {
            internal string Severity; // error | warning | info
            internal string Code;
            internal string Message;
            public override string ToString() { return Severity.ToUpperInvariant() + " " + Code + ": " + Message; }
        }

        internal static readonly string[] SupportedShaders = { "gta_default" };
        internal const int MaxVerticesPerGeometry = CompilerCapabilities.MaxVerticesPerGeometry;
        internal const float MinExtentMeters = 0.02f, MaxExtentMeters = 200f;
        internal const int MaxTextureSize = TextureEncoder.MaxSizePixels;

        internal static List<Issue> Validate(ContentAsset asset, CompilerCapabilities capabilities)
        {
            List<Issue> issues = new List<Issue>();
            if (asset.Meshes.Count == 0) { Add(issues, "error", "LCC001", "no triangle meshes in " + asset.SourcePath); return issues; }
            foreach (KeyValuePair<string, string> meta in asset.Metadata)
            {
                if (meta.Key.StartsWith("unsupported_mode_", StringComparison.Ordinal)) { Add(issues, "error", "LCC002", "mesh " + meta.Key.Substring(17) + " uses primitive mode " + meta.Value + " (only triangles, strips and fans)"); }
            }
            if (asset.HasSkin) { Add(issues, "warning", "LCC015", "skin (joints/weights) present: this compiler version writes static props; the skin is ignored"); }

            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            foreach (ContentMesh mesh in asset.Meshes)
            {
                if (mesh.Indices.Count < 3) { Add(issues, "error", "LCC001", "mesh " + mesh.Name + " has no triangles"); continue; }
                if (!mesh.HadNormals) { Add(issues, "warning", "LCC003", "mesh " + mesh.Name + " has no normals; smooth normals will be generated"); }
                ContentMaterial material = mesh.Material >= 0 && mesh.Material < asset.Materials.Count ? asset.Materials[mesh.Material] : null;
                if (material != null && material.Image >= 0 && !mesh.HadUvs) { Add(issues, "error", "LCC005", "mesh " + mesh.Name + " is textured but has no UVs"); }
                if (material == null) { Add(issues, "warning", "LCC010", "mesh " + mesh.Name + " has no material; the default white material is used"); }
                int degenerate = 0, bad = 0;
                for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
                {
                    int a = mesh.Indices[i], b = mesh.Indices[i + 1], c = mesh.Indices[i + 2];
                    if (a < 0 || b < 0 || c < 0 || a >= mesh.Vertices.Count || b >= mesh.Vertices.Count || c >= mesh.Vertices.Count) { bad++; continue; }
                    if (Area2(mesh.Vertices[a], mesh.Vertices[b], mesh.Vertices[c]) < 1e-12) { degenerate++; }
                }
                if (bad > 0) { Add(issues, "error", "LCC006", "mesh " + mesh.Name + " has " + bad + " triangles with out-of-range indices"); }
                if (degenerate > 0) { Add(issues, "warning", "LCC007", "mesh " + mesh.Name + " has " + degenerate + " degenerate triangles (removed)"); }
                int nonFinite = 0, unnormalised = 0;
                foreach (ContentVertex v in mesh.Vertices)
                {
                    if (!Finite(v.X) || !Finite(v.Y) || !Finite(v.Z) || !Finite(v.U) || !Finite(v.V)) { nonFinite++; continue; }
                    float length = (float)Math.Sqrt(v.NX * v.NX + v.NY * v.NY + v.NZ * v.NZ);
                    if (mesh.HadNormals && Math.Abs(length - 1) > 0.01f) { unnormalised++; }
                    minX = Math.Min(minX, v.X); minY = Math.Min(minY, v.Y); minZ = Math.Min(minZ, v.Z);
                    maxX = Math.Max(maxX, v.X); maxY = Math.Max(maxY, v.Y); maxZ = Math.Max(maxZ, v.Z);
                }
                if (nonFinite > 0) { Add(issues, "error", "LCC008", "mesh " + mesh.Name + " has " + nonFinite + " vertices with NaN/infinite values"); }
                if (unnormalised > 0) { Add(issues, "warning", "LCC009", "mesh " + mesh.Name + " has " + unnormalised + " non-unit normals (renormalised)"); }
            }

            if (minX <= maxX)
            {
                float extent = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
                if (extent < MinExtentMeters || extent > MaxExtentMeters)
                {
                    Add(issues, "warning", "LCC011", "largest extent is " + F(extent) + " m (expected " + MinExtentMeters + "-" + MaxExtentMeters +
                        " m): check the Blender unit scale and that transforms are applied");
                }
                float cx = (minX + maxX) / 2, cy = (minY + maxY) / 2;
                float offset = (float)Math.Sqrt(cx * cx + cy * cy);
                if (offset > Math.Max(0.5f, extent)) { Add(issues, "warning", "LCC012", "the model's centre is " + F(offset) + " m from the origin in X/Y; the pivot (spawn point) is the origin"); }
                if (maxZ - minZ < 0.25f * Math.Max(maxX - minX, maxY - minY) && Math.Abs(minZ) > extent)
                {
                    Add(issues, "info", "LCC013", "the model sits " + F(minZ) + " m from Z=0; GTA IV is Z-up with props usually resting on Z=0");
                }
                Add(issues, "info", "LCC014", "bounds (" + F(minX) + ", " + F(minY) + ", " + F(minZ) + ") .. (" + F(maxX) + ", " + F(maxY) + ", " + F(maxZ) + ")");
            }

            ValidateLods(asset, capabilities, issues);

            foreach (ContentMaterial material in asset.Materials)
            {
                if (Array.IndexOf(SupportedShaders, material.Shader) < 0) { Add(issues, "error", "LCC019", "material " + material.Name + " asks for shader '" + material.Shader + "' (supported: " + string.Join(", ", SupportedShaders) + ")"); }
                if (material.AlphaMode != "OPAQUE") { Add(issues, "warning", "LCC020", "material " + material.Name + " uses alpha mode " + material.AlphaMode + "; textureMode template writes opaque DXT1, textureMode native writes DXT5 with alpha (whether gta_default draws it translucent is unverified in game)"); }
                if (material.Image >= 0 && material.Image < asset.Images.Count && asset.Images[material.Image] != null)
                {
                    System.Drawing.Bitmap image = asset.Images[material.Image];
                    if (!PowerOfTwo(image.Width) || !PowerOfTwo(image.Height)) { Add(issues, "warning", "LCC021", "texture " + asset.ImageNames[material.Image] + " is " + image.Width + "x" + image.Height + " (not a power of two; it will be resampled)"); }
                    if (image.Width > MaxTextureSize || image.Height > MaxTextureSize) { Add(issues, "warning", "LCC022", "texture " + asset.ImageNames[material.Image] + " is larger than " + MaxTextureSize + " (it will be resampled)"); }
                }
                else if (material.Image >= 0) { Add(issues, "error", "LCC023", "material " + material.Name + " references an image that could not be decoded"); }
            }
            return issues;
        }

        // LOD levels and their material groups (one group = one future geometry): LCC004, LCC016-018, LCC024, LCC025.
        private static void ValidateLods(ContentAsset asset, CompilerCapabilities capabilities, List<Issue> issues)
        {
            Dictionary<int, int> trianglesPerLod = new Dictionary<int, int>();
            foreach (ContentLod lod in asset.BuildLods())
            {
                // Meshes without triangles were reported as LCC001 and are left out of the counts.
                int triangles = lod.TriangleCount;
                if (triangles == 0) { continue; }
                trianglesPerLod[lod.Level] = triangles;
                if (lod.Level < 0 || lod.Level >= CompilerCapabilities.DrawableLodSlots)
                {
                    Add(issues, "error", "LCC024", "LOD " + lod.Level + " is outside 0-" + (CompilerCapabilities.DrawableLodSlots - 1) + " (a drawable has " + CompilerCapabilities.DrawableLodSlots + " LOD slots)");
                    continue;
                }
                int materials = lod.Groups.Count(g => g.TriangleCount > 0);
                if (materials > capabilities.MaxMaterialsPerLod)
                {
                    Add(issues, "error", "LCC016", "LOD " + lod.Level + " uses " + materials + " materials; compiler " + capabilities.Version + " writes " + capabilities.MaxMaterialsPerLod +
                        " material" + (capabilities.MaxMaterialsPerLod == 1 ? "" : "s") + " per LOD (merge materials or bake an atlas)");
                }
                foreach (ContentMaterialGroup group in lod.Groups)
                {
                    // v1 merges the whole LOD into one geometry; with one material per LOD that is exactly this group.
                    if (group.VertexCount > MaxVerticesPerGeometry)
                    {
                        Add(issues, "error", "LCC004", "LOD " + lod.Level + " material " + MaterialName(asset, group.Material) + " has " + group.VertexCount + " vertices in one geometry (16-bit indices allow " +
                            MaxVerticesPerGeometry + "); reduce or split it");
                    }
                }
                if (lod.Level >= capabilities.CompiledLodLevels)
                {
                    Add(issues, "info", "LCC025", "LOD " + lod.Level + " (" + triangles + " triangles, " + materials + " material" + (materials == 1 ? "" : "s") + ") is validated but not compiled by compiler " +
                        capabilities.Version + " (compiles LOD 0" + (capabilities.CompiledLodLevels > 1 ? "-" + (capabilities.CompiledLodLevels - 1) : "") + ")");
                }
            }
            for (int lod = 1; lod < CompilerCapabilities.DrawableLodSlots; lod++)
            {
                int current, previous;
                if (trianglesPerLod.TryGetValue(lod, out current) && trianglesPerLod.TryGetValue(lod - 1, out previous) && current >= previous)
                {
                    Add(issues, "warning", "LCC017", "LOD " + lod + " has " + current + " triangles, not fewer than LOD " + (lod - 1) + " (" + previous + ")");
                }
            }
            if (!trianglesPerLod.ContainsKey(0)) { Add(issues, "error", "LCC018", "no LOD 0 mesh"); }
        }

        private static string MaterialName(ContentAsset asset, int material)
        {
            return material >= 0 && material < asset.Materials.Count ? "'" + asset.Materials[material].Name + "'" : "(none)";
        }

        internal static bool HasErrors(List<Issue> issues) { return issues.Exists(i => i.Severity == "error"); }

        private static void Add(List<Issue> issues, string severity, string code, string message)
        {
            issues.Add(new Issue { Severity = severity, Code = code, Message = message });
        }

        private static float Area2(ContentVertex a, ContentVertex b, ContentVertex c)
        {
            double ux = b.X - a.X, uy = b.Y - a.Y, uz = b.Z - a.Z, vx = c.X - a.X, vy = c.Y - a.Y, vz = c.Z - a.Z;
            double x = uy * vz - uz * vy, y = uz * vx - ux * vz, z = ux * vy - uy * vx;
            return (float)(x * x + y * y + z * z);
        }

        private static bool Finite(float v) { return !float.IsNaN(v) && !float.IsInfinity(v); }
        private static bool PowerOfTwo(int v) { return v > 0 && (v & (v - 1)) == 0; }
        private static string F(float v) { return v.ToString("0.###", CultureInfo.InvariantCulture); }
    }
}
