using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LibertyFramework.Content
{
    // glTF 2.0 -> ContentAsset. Mesh instances are baked with their node's world transform, then converted from glTF's
    // Y-up (Blender's exporter writes Blender (x, y, z) as glTF (x, z, -y)) to GTA IV's Z-up: (x, y, z) -> (x, -z, y).
    // That is a rotation, so handedness and winding are kept. UVs need no flip (glTF and Direct3D both put (0,0) top-left).
    // LOD: node extras "liberty_lod", or a node/mesh name ending in "_lod<N>", else 0.
    // Collision: node extras "liberty_collision" (a shape name; "" or "none" means not collision), or a node name ending in
    // "_col" (optionally followed by digits), which is a mesh shape. Optional "liberty_surface" names its surface.
    internal static class GltfImporter
    {
        private static readonly System.Text.RegularExpressions.Regex LodSuffix =
            new System.Text.RegularExpressions.Regex(@"_lod(\d+)(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Same rule as the Blender add-on's COLLISION_SUFFIX, including Blender's duplicate suffix (".001").
        private static readonly System.Text.RegularExpressions.Regex CollisionSuffix =
            new System.Text.RegularExpressions.Regex(@"_col\d*(\.\d+)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        internal const string CollisionNone = "none";

        internal static ContentAsset Import(string path)
        {
            GltfDocument doc = GltfDocument.Load(path);
            ContentAsset asset = new ContentAsset();
            asset.SourcePath = Path.GetFullPath(path);
            asset.Name = Path.GetFileNameWithoutExtension(path);
            ReadImages(doc, asset);
            ReadMaterials(doc, asset);
            Dictionary<string, object> sceneExtras = null;
            IList<Dictionary<string, object>> scenes = doc.Array("scenes");
            int sceneIndex = GltfDocument.Int(doc.Root, "scene", 0);
            List<int> roots = new List<int>();
            if (scenes.Count > 0)
            {
                Dictionary<string, object> scene = scenes[Math.Min(sceneIndex, scenes.Count - 1)];
                sceneExtras = GltfDocument.Obj(scene, "extras");
                double[] nodes = GltfDocument.Numbers(scene, "nodes");
                if (nodes != null) { foreach (double n in nodes) { roots.Add((int)n); } }
            }
            else { for (int i = 0; i < doc.Array("nodes").Count; i++) { roots.Add(i); } }
            if (sceneExtras != null)
            {
                // Only scalar liberty_* values: Blender also exports add-on property groups and other custom data as extras.
                foreach (KeyValuePair<string, object> pair in sceneExtras)
                {
                    if (!pair.Key.StartsWith("liberty_", StringComparison.Ordinal) || pair.Value is Dictionary<string, object> || pair.Value is System.Collections.IList) { continue; }
                    asset.Metadata[pair.Key] = Convert.ToString(pair.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            foreach (int root in roots) { Walk(doc, asset, root, Matrix4.Identity, -1); }
            return asset;
        }

        private static void Walk(GltfDocument doc, ContentAsset asset, int nodeIndex, Matrix4 parent, int inheritedLod)
        {
            Dictionary<string, object> node = doc.Array("nodes")[nodeIndex];
            Matrix4 world = parent * LocalMatrix(node);
            string name = GltfDocument.Str(node, "name") ?? ("node" + nodeIndex);
            Dictionary<string, object> extras = GltfDocument.Obj(node, "extras");
            int lod = LodOf(name, extras, inheritedLod);
            if (node.ContainsKey("skin")) { asset.HasSkin = true; }
            string shape = CollisionShapeOf(name, extras);
            if (node.ContainsKey("mesh"))
            {
                if (shape != null)
                {
                    bool lodTagged = (extras != null && extras.ContainsKey("liberty_lod")) || LodSuffix.IsMatch(name);
                    string surface = extras != null && extras.ContainsKey("liberty_surface") ? Convert.ToString(extras["liberty_surface"], System.Globalization.CultureInfo.InvariantCulture) : null;
                    ReadCollision(doc, asset, GltfDocument.Int(node, "mesh", 0), world, name, shape, surface, lodTagged);
                }
                else { ReadMesh(doc, asset, GltfDocument.Int(node, "mesh", 0), world, name, Math.Max(0, lod)); }
            }
            double[] children = GltfDocument.Numbers(node, "children");
            if (children != null) { foreach (double child in children) { Walk(doc, asset, (int)child, world, lod); } }
        }

        private static int LodOf(string name, Dictionary<string, object> extras, int inherited)
        {
            if (extras != null && extras.ContainsKey("liberty_lod")) { return Convert.ToInt32(extras["liberty_lod"]); }
            // "_lod<N>", optionally followed by Blender's duplicate suffix (".001"); same rule as the Blender add-on's lod_of.
            System.Text.RegularExpressions.Match match = LodSuffix.Match(name);
            return match.Success ? int.Parse(match.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture) : inherited;
        }

        // The collision shape a node declares, or null for a render node.
        internal static string CollisionShapeOf(string name, Dictionary<string, object> extras)
        {
            if (extras != null && extras.ContainsKey("liberty_collision"))
            {
                string shape = (Convert.ToString(extras["liberty_collision"], System.Globalization.CultureInfo.InvariantCulture) ?? "").Trim().ToLowerInvariant();
                return shape.Length == 0 || shape == CollisionNone ? null : shape;
            }
            return CollisionSuffix.IsMatch(name) ? ContentCollision.ShapeMesh : null;
        }

        private static Matrix4 LocalMatrix(Dictionary<string, object> node)
        {
            double[] m = GltfDocument.Numbers(node, "matrix");
            if (m != null && m.Length == 16) { return Matrix4.FromColumnMajor(m); }
            double[] t = GltfDocument.Numbers(node, "translation") ?? new double[] { 0, 0, 0 };
            double[] r = GltfDocument.Numbers(node, "rotation") ?? new double[] { 0, 0, 0, 1 };
            double[] s = GltfDocument.Numbers(node, "scale") ?? new double[] { 1, 1, 1 };
            return Matrix4.Compose(t, r, s);
        }

        private static void ReadMesh(GltfDocument doc, ContentAsset asset, int meshIndex, Matrix4 world, string nodeName, int lod)
        {
            Dictionary<string, object> mesh = doc.Array("meshes")[meshIndex];
            IList<Dictionary<string, object>> primitives = GltfDocument.Array(mesh, "primitives");
            Matrix4 normalMatrix = world.InverseTransposeLinear();
            for (int p = 0; p < primitives.Count; p++)
            {
                Dictionary<string, object> primitive = primitives[p];
                int mode = GltfDocument.Int(primitive, "mode", 4);
                Dictionary<string, object> attributes = GltfDocument.Obj(primitive, "attributes");
                if (attributes == null || !attributes.ContainsKey("POSITION")) { continue; }
                ContentMesh result = new ContentMesh();
                result.Name = (GltfDocument.Str(mesh, "name") ?? nodeName) + (primitives.Count > 1 ? "#" + p : "");
                result.Lod = lod;
                result.Material = GltfDocument.Int(primitive, "material", -1);
                int c;
                float[] positions = doc.ReadFloats(GltfDocument.Int(attributes, "POSITION", 0), out c);
                int count = positions.Length / 3;
                float[] normals = attributes.ContainsKey("NORMAL") ? doc.ReadFloats(GltfDocument.Int(attributes, "NORMAL", 0), out c) : null;
                float[] uvs = attributes.ContainsKey("TEXCOORD_0") ? doc.ReadFloats(GltfDocument.Int(attributes, "TEXCOORD_0", 0), out c) : null;
                int colourComponents = 0;
                float[] colours = attributes.ContainsKey("COLOR_0") ? doc.ReadFloats(GltfDocument.Int(attributes, "COLOR_0", 0), out colourComponents) : null;
                if (attributes.ContainsKey("JOINTS_0")) { asset.HasSkin = true; }
                result.HadNormals = normals != null;
                result.HadUvs = uvs != null;
                for (int i = 0; i < count; i++)
                {
                    double[] pos = world.TransformPoint(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
                    ContentVertex v = new ContentVertex();
                    v.X = (float)pos[0]; v.Y = (float)-pos[2]; v.Z = (float)pos[1];
                    if (normals != null)
                    {
                        double[] n = normalMatrix.TransformDirection(normals[i * 3], normals[i * 3 + 1], normals[i * 3 + 2]);
                        double length = Math.Sqrt(n[0] * n[0] + n[1] * n[1] + n[2] * n[2]);
                        if (length > 1e-9) { n[0] /= length; n[1] /= length; n[2] /= length; }
                        v.NX = (float)n[0]; v.NY = (float)-n[2]; v.NZ = (float)n[1];
                    }
                    if (uvs != null) { v.U = uvs[i * 2]; v.V = uvs[i * 2 + 1]; }
                    v.Colour = 0xFFFFFFFF;
                    if (colours != null)
                    {
                        float r = colours[i * colourComponents], g = colours[i * colourComponents + 1], b = colours[i * colourComponents + 2];
                        float a = colourComponents > 3 ? colours[i * colourComponents + 3] : 1f;
                        v.Colour = (uint)(Byte(a) << 24 | Byte(r) << 16 | Byte(g) << 8 | Byte(b));
                    }
                    result.Vertices.Add(v);
                }
                int[] indices = primitive.ContainsKey("indices") ? doc.ReadIndices(GltfDocument.Int(primitive, "indices", 0)) : Sequence(count);
                Triangulate(mode, indices, result.Indices);
                // A mirroring transform (negative determinant) flips the winding: restore it.
                if (world.Determinant3() < 0) { for (int i = 0; i + 2 < result.Indices.Count; i += 3) { int t = result.Indices[i + 1]; result.Indices[i + 1] = result.Indices[i + 2]; result.Indices[i + 2] = t; } }
                if (mode != 4 && mode != 5 && mode != 6) { asset.Metadata["unsupported_mode_" + result.Name] = mode.ToString(); }
                asset.Meshes.Add(result);
            }
        }

        // A collision node: its triangles (mesh shape), or a primitive fitted to the mesh's local bounds and the node's world
        // transform. Primitives are fitted in Blender's frame: glTF local (x, y, z) is Blender local (x, -z, y), so the
        // capsule axis (Blender local Z) is glTF local Y. Materials, normals and UVs are ignored.
        private static void ReadCollision(GltfDocument doc, ContentAsset asset, int meshIndex, Matrix4 world, string nodeName, string shape, string surface, bool lodTagged)
        {
            Dictionary<string, object> mesh = doc.Array("meshes")[meshIndex];
            ContentCollision collision = new ContentCollision { Name = nodeName, Shape = shape, Surface = surface, HadLodTag = lodTagged };
            asset.Collisions.Add(collision);
            if (Array.IndexOf(ContentCollision.Shapes, shape) < 0) { return; } // reported by the validator (LCC027)
            List<double[]> local = new List<double[]>();
            foreach (Dictionary<string, object> primitive in GltfDocument.Array(mesh, "primitives"))
            {
                Dictionary<string, object> attributes = GltfDocument.Obj(primitive, "attributes");
                if (attributes == null || !attributes.ContainsKey("POSITION")) { continue; }
                int c;
                float[] positions = doc.ReadFloats(GltfDocument.Int(attributes, "POSITION", 0), out c);
                int start = local.Count, count = positions.Length / 3;
                for (int i = 0; i < count; i++) { local.Add(new double[] { positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2] }); }
                if (shape != ContentCollision.ShapeMesh) { continue; }
                int mode = GltfDocument.Int(primitive, "mode", 4);
                int[] indices = primitive.ContainsKey("indices") ? doc.ReadIndices(GltfDocument.Int(primitive, "indices", 0)) : Sequence(count);
                List<int> triangles = new List<int>();
                Triangulate(mode, indices, triangles);
                if (world.Determinant3() < 0) { for (int i = 0; i + 2 < triangles.Count; i += 3) { int t = triangles[i + 1]; triangles[i + 1] = triangles[i + 2]; triangles[i + 2] = t; } }
                foreach (int index in triangles) { collision.Indices.Add(start + index); }
            }
            if (shape == ContentCollision.ShapeMesh)
            {
                foreach (double[] p in local)
                {
                    double[] w = world.TransformPoint(p[0], p[1], p[2]);
                    collision.Vertices.Add(new ContentVertex { X = (float)w[0], Y = (float)-w[2], Z = (float)w[1], NZ = 1, Colour = 0xFFFFFFFF });
                }
                return;
            }
            if (local.Count == 0) { collision.Problem = "the mesh has no vertices"; return; }
            FitPrimitive(collision, local, world);
        }

        // glTF (x, y, z) -> GTA IV / Blender (x, -z, y), and back.
        private static double[] ToGta(double[] p) { return new[] { p[0], -p[2], p[1] }; }
        private static double[] FromGta(double[] p) { return new[] { p[0], p[2], -p[1] }; }

        private static void FitPrimitive(ContentCollision collision, List<double[]> local, Matrix4 world)
        {
            // Local bounds in the object's Blender frame.
            double[] low = { double.MaxValue, double.MaxValue, double.MaxValue }, high = { double.MinValue, double.MinValue, double.MinValue };
            foreach (double[] p in local)
            {
                double[] b = ToGta(p);
                for (int i = 0; i < 3; i++) { low[i] = Math.Min(low[i], b[i]); high[i] = Math.Max(high[i], b[i]); }
            }
            double[] middle = FromGta(new[] { (low[0] + high[0]) / 2, (low[1] + high[1]) / 2, (low[2] + high[2]) / 2 });
            double[] centre = ToGta(world.TransformPoint(middle[0], middle[1], middle[2]));
            double[] extent = new double[3];
            double[][] axes = new double[3][];
            for (int axis = 0; axis < 3; axis++)
            {
                double[] unit = new double[3];
                unit[axis] = 1;
                double[] direction = FromGta(unit);
                double[] column = ToGta(world.TransformDirection(direction[0], direction[1], direction[2]));
                double scale = Math.Sqrt(column[0] * column[0] + column[1] * column[1] + column[2] * column[2]);
                if (scale < 1e-9) { collision.Problem = "the object's scale is zero on its local " + "XYZ"[axis] + " axis"; return; }
                axes[axis] = new[] { column[0] / scale, column[1] / scale, column[2] / scale };
                extent[axis] = (high[axis] - low[axis]) / 2 * scale;
            }
            for (int a = 0; a < 3; a++)
            {
                for (int b = a + 1; b < 3; b++)
                {
                    double dot = axes[a][0] * axes[b][0] + axes[a][1] * axes[b][1] + axes[a][2] * axes[b][2];
                    if (Math.Abs(dot) > ShearTolerance) { collision.Problem = "the object's transform is sheared (local axes not perpendicular: a non-uniformly scaled parent with a rotated child); apply the transform"; return; }
                }
            }
            // A mirrored transform leaves a left-handed frame; boxes, spheres and capsules are symmetric, so flip X to keep
            // the axes a rotation.
            double handedness = Dot(Cross(axes[0], axes[1]), axes[2]);
            if (handedness < 0) { axes[0] = new[] { -axes[0][0], -axes[0][1], -axes[0][2] }; }
            for (int i = 0; i < 3; i++)
            {
                collision.Centre[i] = (float)centre[i];
                for (int j = 0; j < 3; j++) { collision.Axes[i][j] = (float)axes[i][j]; }
            }
            switch (collision.Shape)
            {
                case ContentCollision.ShapeBox:
                    for (int i = 0; i < 3; i++) { collision.HalfExtents[i] = (float)extent[i]; }
                    break;
                case ContentCollision.ShapeSphere:
                    collision.Radius = (float)Math.Max(extent[0], Math.Max(extent[1], extent[2]));
                    collision.NonUniformRatio = Ratio(extent[0], extent[1], extent[2]);
                    break;
                case ContentCollision.ShapeCapsule:
                    collision.Radius = (float)Math.Max(extent[0], extent[1]);
                    collision.NonUniformRatio = Ratio(extent[0], extent[1], Math.Max(extent[0], extent[1]));
                    double length = 2 * extent[2] - 2 * collision.Radius;
                    if (length < -CapsuleLengthToleranceMeters)
                    {
                        collision.Problem = "the capsule is " + (2 * extent[2]).ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) + " m long on its local Z axis, shorter than its diameter " +
                            (2 * collision.Radius).ToString("0.###", System.Globalization.CultureInfo.InvariantCulture) + " m (a capsule runs along the object's local Z)";
                    }
                    collision.Length = (float)Math.Max(0, length);
                    break;
            }
        }

        // |cos| between two local axes above which a transform counts as sheared (about 0.06 degrees from perpendicular).
        private const double ShearTolerance = 1e-3;
        // Rounding allowed when a capsule's height equals its diameter (a sphere-like capsule).
        private const double CapsuleLengthToleranceMeters = 1e-4;

        private static float Ratio(double a, double b, double c)
        {
            double max = Math.Max(a, Math.Max(b, c)), min = Math.Min(a, Math.Min(b, c));
            return min > 1e-9 ? (float)(max / min) : float.PositiveInfinity;
        }

        private static double[] Cross(double[] a, double[] b) { return new[] { a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0] }; }
        private static double Dot(double[] a, double[] b) { return a[0] * b[0] + a[1] * b[1] + a[2] * b[2]; }

        private static int Byte(float value) { return (int)Math.Round(Math.Max(0, Math.Min(1, value)) * 255); }

        private static int[] Sequence(int count) { int[] s = new int[count]; for (int i = 0; i < count; i++) { s[i] = i; } return s; }

        private static void Triangulate(int mode, int[] indices, List<int> output)
        {
            if (mode == 4) { output.AddRange(indices); return; }
            if (mode == 5) // strip
            {
                for (int i = 2; i < indices.Length; i++)
                {
                    if (i % 2 == 0) { output.Add(indices[i - 2]); output.Add(indices[i - 1]); output.Add(indices[i]); }
                    else { output.Add(indices[i - 1]); output.Add(indices[i - 2]); output.Add(indices[i]); }
                }
                return;
            }
            if (mode == 6) // fan
            {
                for (int i = 2; i < indices.Length; i++) { output.Add(indices[0]); output.Add(indices[i - 1]); output.Add(indices[i]); }
            }
            // Points and lines produce no triangles; the validator reports them.
        }

        private static void ReadImages(GltfDocument doc, ContentAsset asset)
        {
            IList<Dictionary<string, object>> images = doc.Array("images");
            for (int i = 0; i < images.Count; i++)
            {
                Dictionary<string, object> image = images[i];
                byte[] bytes = image.ContainsKey("bufferView") ? doc.ReadBufferView(GltfDocument.Int(image, "bufferView", 0)) :
                    image.ContainsKey("uri") ? doc.ReadUri(GltfDocument.Str(image, "uri")) : null;
                Bitmap bitmap = null;
                if (bytes != null) { using (MemoryStream stream = new MemoryStream(bytes)) using (Image decoded = Image.FromStream(stream)) { bitmap = new Bitmap(decoded); } }
                asset.Images.Add(bitmap);
                asset.ImageNames.Add(GltfDocument.Str(image, "name") ?? Path.GetFileNameWithoutExtension(GltfDocument.Str(image, "uri") ?? ("image" + i)));
            }
        }

        private static void ReadMaterials(GltfDocument doc, ContentAsset asset)
        {
            IList<Dictionary<string, object>> textures = doc.Array("textures");
            foreach (Dictionary<string, object> material in doc.Array("materials"))
            {
                ContentMaterial m = new ContentMaterial();
                m.Name = GltfDocument.Str(material, "name") ?? ("material" + asset.Materials.Count);
                m.AlphaMode = GltfDocument.Str(material, "alphaMode") ?? "OPAQUE";
                m.DoubleSided = material.ContainsKey("doubleSided") && (bool)material["doubleSided"];
                Dictionary<string, object> pbr = GltfDocument.Obj(material, "pbrMetallicRoughness");
                double[] factor = GltfDocument.Numbers(pbr, "baseColorFactor");
                if (factor != null && factor.Length == 4) { m.BaseColour = new[] { (float)factor[0], (float)factor[1], (float)factor[2], (float)factor[3] }; }
                Dictionary<string, object> baseTexture = GltfDocument.Obj(pbr, "baseColorTexture");
                if (baseTexture != null)
                {
                    int texture = GltfDocument.Int(baseTexture, "index", -1);
                    if (texture >= 0 && texture < textures.Count) { m.Image = GltfDocument.Int(textures[texture], "source", -1); }
                }
                Dictionary<string, object> extras = GltfDocument.Obj(material, "extras");
                if (extras != null && extras.ContainsKey("liberty_shader")) { m.Shader = Convert.ToString(extras["liberty_shader"]); }
                asset.Materials.Add(m);
            }
        }
    }
}
