using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace LibertyFramework.Content
{
    // Self-test groups for the authoring side (NEXT_SESSIONS session 3): collision nodes imported from glTF and fitted to
    // primitives, the structure and world-object validator rules, the capabilities that decide what is refused, the
    // report's structure section and the fixture matcher. Expected values are computed by hand from the transforms
    // written here (comments give the working), never read back from the code under test.
    internal static class AuthoringSelfTest
    {
        private const float Tolerance = 1e-4f;

        internal static void Capabilities(SelfTest.Runner t, string output)
        {
            CompilerCapabilities v1 = CompilerCapabilities.Current;
            t.Check(v1.Version == "v1" && v1.Shaders.SequenceEqual(new[] { "gta_default" }) && v1.AssetTypes.SequenceEqual(new[] { "prop", "object" }), "v1: gta_default; props and world objects (session 6)");
            t.Check(v1.CollisionShapes.Length == 0 && !v1.WritesLodDistances, "v1 writes no collision and no LOD distances");
            Dictionary<string, object> json = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(v1.ToJson());
            t.Check((string)json["version"] == "v1" && Convert.ToInt32(json["maxMaterialsPerLod"]) == 1 && Convert.ToInt32(json["compiledLodLevels"]) == 1 &&
                Convert.ToInt32(json["lodSlots"]) == 4 && Convert.ToInt32(json["maxVerticesPerGeometry"]) == 65535 && ((IList)json["collisionShapes"]).Count == 0 &&
                (bool)json["lodDistances"] == false && ((IList)json["assetTypes"]).Cast<object>().SequenceEqual(new object[] { "prop", "object" }) && (bool)json["collisionBorrow"],
                "capabilities JSON parses with every field", v1.ToJson());
        }

        // A unit cube (half size 0.5 in its local frame) shared by every node, positions and u16 indices in a data URI.
        private static string CubeGltf(string nodes, string sceneNodes)
        {
            float[] corners = { -0.5f, -0.5f, -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, -0.5f, -0.5f, 0.5f, -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f, 0.5f, 0.5f, -0.5f, 0.5f, 0.5f };
            ushort[] indices = { 0, 2, 1, 0, 3, 2, 4, 5, 6, 4, 6, 7, 0, 1, 5, 0, 5, 4, 1, 2, 6, 1, 6, 5, 2, 3, 7, 2, 7, 6, 3, 0, 4, 3, 4, 7 };
            byte[] buffer = new byte[corners.Length * 4 + indices.Length * 2];
            Buffer.BlockCopy(corners, 0, buffer, 0, corners.Length * 4);
            Buffer.BlockCopy(indices, 0, buffer, corners.Length * 4, indices.Length * 2);
            return "{ \"asset\": { \"version\": \"2.0\" }, \"scene\": 0, \"scenes\": [ { \"nodes\": [" + sceneNodes + "] } ],\n" +
                "\"nodes\": [\n" + nodes + "\n],\n" +
                "\"meshes\": [ { \"primitives\": [ { \"attributes\": { \"POSITION\": 0 }, \"indices\": 1, \"material\": 0 } ] } ],\n" +
                "\"materials\": [ { \"name\": \"body\" } ],\n" +
                "\"accessors\": [ { \"bufferView\": 0, \"componentType\": 5126, \"count\": 8, \"type\": \"VEC3\" }, { \"bufferView\": 1, \"componentType\": 5123, \"count\": 36, \"type\": \"SCALAR\" } ],\n" +
                "\"bufferViews\": [ { \"buffer\": 0, \"byteOffset\": 0, \"byteLength\": 96 }, { \"buffer\": 0, \"byteOffset\": 96, \"byteLength\": 72 } ],\n" +
                "\"buffers\": [ { \"byteLength\": " + buffer.Length + ", \"uri\": \"data:application/octet-stream;base64," + Convert.ToBase64String(buffer) + "\" } ] }\n";
        }

        private static string Node(string name, string extras, string transform, string children)
        {
            return "{ \"name\": \"" + name + "\", \"mesh\": 0" + (extras != null ? ", \"extras\": " + extras : "") + (transform != null ? ", " + transform : "") +
                (children != null ? ", \"children\": [" + children + "]" : "") + " }";
        }

        private static string Q(double degrees, string axis)
        {
            // Quaternion (x, y, z, w) for a rotation about one axis.
            double half = degrees * Math.PI / 360;
            string s = Math.Sin(half).ToString("R", CultureInfo.InvariantCulture), c = Math.Cos(half).ToString("R", CultureInfo.InvariantCulture);
            string v = axis == "x" ? s + ", 0, 0" : axis == "y" ? "0, " + s + ", 0" : "0, 0, " + s;
            return "\"rotation\": [" + v + ", " + c + "]";
        }

        private static ContentAsset ImportText(string gltf)
        {
            string path = Path.Combine(Path.GetTempPath(), "lcc_authoring_" + Guid.NewGuid().ToString("N") + ".gltf");
            try
            {
                File.WriteAllText(path, gltf);
                return GltfImporter.Import(path);
            }
            finally { File.Delete(path); }
        }

        private static bool Near(float a, float b) { return Math.Abs(a - b) <= Tolerance; }
        private static bool Near(float[] a, params float[] b) { return a.Length == b.Length && a.Zip(b, (x, y) => Math.Abs(x - y) <= Tolerance).All(x => x); }
        private static string V(float[] v) { return string.Join(", ", v.Select(x => x.ToString("0.####", CultureInfo.InvariantCulture)).ToArray()); }

        internal static void CollisionImport(SelfTest.Runner t, string output)
        {
            // Rotation about glTF +Y is rotation about GTA +Z (Y-up to Z-up), so "box" turns 30 degrees counter-clockwise seen
            // from above. glTF scale (sx, sy, sz) is Blender scale (sx, sz, sy): glTF local Y is Blender local Z.
            string[] nodes =
            {
                Node("body", null, null, null),                                                                                 // 0 render
                Node("box", "{ \"liberty_collision\": \"box\", \"liberty_surface\": \"metal\" }",
                    "\"translation\": [1, 0.5, -2], " + Q(30, "y") + ", \"scale\": [2, 3, 0.5]", null),                        // 1
                Node("ball", "{ \"liberty_collision\": \"sphere\" }", "\"translation\": [0, 0.5, 0], \"scale\": [0.4, 0.4, 0.4]", null), // 2
                Node("egg", "{ \"liberty_collision\": \"sphere\" }", "\"scale\": [0.4, 0.4, 0.2]", null),                        // 3
                Node("pill", "{ \"liberty_collision\": \"Capsule\" }", Q(90, "x") + ", \"scale\": [0.6, 2, 0.6]", null),         // 4
                Node("stub", "{ \"liberty_collision\": \"capsule\" }", "\"scale\": [1, 0.5, 1]", null),                          // 5
                Node("body_col", null, null, null),                                                                             // 6
                Node("body_col2.001", null, null, null),                                                                        // 7
                Node("cone", "{ \"liberty_collision\": \"cone\" }", null, null),                                                 // 8
                Node("thing_col", "{ \"liberty_collision\": \"none\" }", null, null),                                           // 9 render
                Node("lodcol", "{ \"liberty_collision\": \"box\", \"liberty_lod\": 1 }", null, null),                            // 10
                Node("matrixbox", "{ \"liberty_collision\": \"box\" }", "\"matrix\": [1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,-3,1]", null), // 11
                "{ \"name\": \"shearparent\", \"scale\": [2, 1, 1], \"children\": [13] }",                                      // 12
                Node("shearbox", "{ \"liberty_collision\": \"box\" }", Q(45, "y"), null),                                        // 13
                Node("badsurface", "{ \"liberty_collision\": \"box\", \"liberty_surface\": \"bad surface!\" }", null, null),     // 14
                Node("mirror", "{ \"liberty_collision\": \"box\" }", "\"scale\": [-1, 1, 1]", null),                             // 15
            };
            ContentAsset asset = ImportText(CubeGltf(string.Join(",\n", nodes), "0,1,2,3,4,5,6,7,8,9,10,11,12,14,15"));
            Func<string, ContentCollision> get = name => asset.Collisions.FirstOrDefault(c => c.Name == name);
            t.Check(asset.Collisions.Count == 13, "13 collision nodes", asset.Collisions.Count + ": " + string.Join(",", asset.Collisions.Select(c => c.Name).ToArray()));
            t.Check(asset.Meshes.Select(m => m.Name).OrderBy(n => n).SequenceEqual(new[] { "body", "thing_col" }), "render meshes: body, and thing_col (liberty_collision none beats the _col suffix)",
                string.Join(",", asset.Meshes.Select(m => m.Name).ToArray()));

            ContentCollision box = get("box");
            float c30 = (float)Math.Cos(Math.PI / 6), s30 = 0.5f;
            // Centre: glTF (1, 0.5, -2) -> GTA (1, 2, 0.5). Half extents: Blender scale (2, 0.5, 3) x 0.5.
            t.Check(box != null && box.Shape == "box" && box.Surface == "metal", "box: shape and surface");
            t.Check(box != null && Near(box.Centre, 1, 2, 0.5f), "box: centre in GTA space", box == null ? "" : V(box.Centre));
            t.Check(box != null && Near(box.HalfExtents, 1, 0.25f, 1.5f), "box: half extents from local bounds x scale (Blender axes)", box == null ? "" : V(box.HalfExtents));
            t.Check(box != null && Near(box.Axes[0], c30, s30, 0) && Near(box.Axes[1], -s30, c30, 0) && Near(box.Axes[2], 0, 0, 1), "box: axes turned 30 degrees about Z",
                box == null ? "" : V(box.Axes[0]) + " | " + V(box.Axes[1]) + " | " + V(box.Axes[2]));

            ContentCollision ball = get("ball"), egg = get("egg"), pill = get("pill"), stub = get("stub");
            t.Check(ball != null && Near(ball.Radius, 0.2f) && Near(ball.NonUniformRatio, 1) && Near(ball.Centre, 0, 0, 0.5f), "sphere: radius 0.2 at (0, 0, 0.5)", ball == null ? "" : ball.Radius + " " + V(ball.Centre));
            t.Check(egg != null && Near(egg.Radius, 0.2f) && Near(egg.NonUniformRatio, 2), "non-uniform sphere: largest radius, ratio 2", egg == null ? "" : egg.Radius + " " + egg.NonUniformRatio);
            // Capsule: shape names are case-insensitive. Blender scale (0.6, 0.6, 2): radius 0.3, height 2, length 2 - 0.6.
            // Turned 90 degrees about glTF X: its axis (glTF local Y) points along glTF +Z, which is GTA -Y.
            t.Check(pill != null && pill.Shape == "capsule" && Near(pill.Radius, 0.3f) && Near(pill.Length, 1.4f) && Near(pill.Axes[2], 0, -1, 0), "capsule: radius, length, axis along local Z",
                pill == null ? "" : pill.Shape + " " + pill.Radius + " " + pill.Length + " " + V(pill.Axes[2]));
            t.Check(stub != null && stub.Problem != null && stub.Problem.Contains("shorter than its diameter"), "capsule shorter than its diameter is a problem", stub == null ? "" : stub.Problem);

            ContentCollision mesh = get("body_col"), second = get("body_col2.001");
            t.Check(mesh != null && mesh.Shape == "mesh" && mesh.Indices.Count == 36 && mesh.Vertices.Count == 8, "_col suffix: mesh collision with 12 triangles");
            t.Check(second != null && second.Shape == "mesh", "_col2.001 suffix (digits, Blender duplicate) is mesh collision");
            t.Check(get("cone") != null && get("cone").Shape == "cone", "unknown shape kept for the validator");
            t.Check(get("lodcol") != null && get("lodcol").HadLodTag, "LOD tag on a collision node recorded");
            ContentCollision matrix = get("matrixbox");
            t.Check(matrix != null && Near(matrix.Centre, 0, 3, 0), "node \"matrix\" transform is applied (glTF (0, 0, -3) -> GTA (0, 3, 0))", matrix == null ? "" : V(matrix.Centre));
            t.Check(get("shearbox") != null && get("shearbox").Problem != null && get("shearbox").Problem.Contains("sheared"), "rotated child of a non-uniformly scaled parent is sheared");
            ContentCollision mirror = get("mirror");
            float[] cross = mirror == null ? null : new[] { mirror.Axes[0][1] * mirror.Axes[1][2] - mirror.Axes[0][2] * mirror.Axes[1][1],
                mirror.Axes[0][2] * mirror.Axes[1][0] - mirror.Axes[0][0] * mirror.Axes[1][2], mirror.Axes[0][0] * mirror.Axes[1][1] - mirror.Axes[0][1] * mirror.Axes[1][0] };
            t.Check(mirror != null && mirror.Problem == null && Near(mirror.HalfExtents, 0.5f, 0.5f, 0.5f) && Near(cross, mirror.Axes[2]), "mirrored box: positive extents, right-handed axes",
                mirror == null ? "" : V(mirror.HalfExtents) + " x*y=" + V(cross));

            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, CompilerCapabilities.Current);
            Func<string, string, bool> has = (code, name) => issues.Any(i => i.Code == code && i.Message.Contains("collision " + name + " "));
            t.Check(has("LCC027", "cone") && has("LCC027", "badsurface"), "LCC027: unknown shape, bad surface name");
            t.Check(has("LCC028", "stub") && has("LCC028", "shearbox") && !has("LCC028", "box"), "LCC028: capsule too short, sheared transform (not the good box)");
            t.Check(has("LCC029", "egg") && !has("LCC029", "ball"), "LCC029: non-uniform sphere only");
            t.Check(has("LCC030", "lodcol"), "LCC030: LOD tag on collision");
            t.Check(issues.Any(i => i.Code == "LCC032" && i.Severity == "error" && i.Message.Contains("writes no collision")), "LCC032: v1 refuses collision");
            CompilerCapabilities all = new CompilerCapabilities("test", 1, 1, new[] { "gta_default" }, new[] { "prop" }, ContentCollision.Shapes, false);
            t.Check(!AssetValidator.Validate(asset, all).Any(i => i.Code == "LCC032"), "LCC032 follows the capabilities (every shape written)");
        }

        private static ContentCollision BoxCollision(string name, float x, float y, float z, float half)
        {
            ContentCollision box = new ContentCollision { Name = name, Shape = ContentCollision.ShapeBox };
            box.Centre[0] = x; box.Centre[1] = y; box.Centre[2] = z;
            box.HalfExtents[0] = box.HalfExtents[1] = box.HalfExtents[2] = half;
            return box;
        }

        internal static void StructureRules(SelfTest.Runner t, string output)
        {
            CompilerCapabilities v1 = CompilerCapabilities.Current;
            ContentAsset two = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("b", 0, 1, 0.5f), SelfTest.Box("c", 1, 0, 0.5f));
            List<AssetValidator.Issue> issues = AssetValidator.Validate(two, v1);
            t.Check(issues.Any(i => i.Code == "LCC026" && i.Severity == "info" && i.Message.StartsWith("LOD 0: 2 geometries ('first' 12 triangles, 8 vertices, 'second' 12 triangles")),
                "LCC026 lists LOD 0's geometries", string.Join(" | ", issues.Where(i => i.Code == "LCC026").Select(i => i.Message).ToArray()));
            t.Check(issues.Count(i => i.Code == "LCC026") == 2, "LCC026 once per LOD");

            ContentAsset near = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            near.Collisions.Add(BoxCollision("inside", 0, 0, 0.5f, 0.5f));
            t.Check(!SelfTest.Codes(near, v1, null).Contains("LCC031"), "collision inside the model: no LCC031");
            ContentAsset far = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            far.Collisions.Add(BoxCollision("away", 50, 0, 0.5f, 0.5f));
            t.Check(SelfTest.Codes(far, v1, "warning").Contains("LCC031"), "collision 50 m away: LCC031");

            CompilerCapabilities boxes = new CompilerCapabilities("test", 1, 1, new[] { "gta_default" }, new[] { "prop" }, new[] { "box" }, false);
            t.Check(!SelfTest.Codes(near, boxes, null).Contains("LCC032"), "box collision with a box writer: accepted");
            ContentAsset meshCollision = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            ContentMesh source = SelfTest.Box("hull", 0, -1, 1);
            ContentCollision hull = new ContentCollision { Name = "hull", Shape = ContentCollision.ShapeMesh };
            hull.Vertices.AddRange(source.Vertices); hull.Indices.AddRange(source.Indices);
            meshCollision.Collisions.Add(hull);
            List<AssetValidator.Issue> refused = AssetValidator.Validate(meshCollision, boxes);
            t.Check(refused.Any(i => i.Code == "LCC032" && i.Message.Contains("(mesh)") && i.Message.Contains("writes only box")), "mesh collision with a box writer: LCC032 names the shape",
                string.Join(" | ", refused.Where(i => i.Code == "LCC032").Select(i => i.Message).ToArray()));

            ContentAsset empty = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            empty.Collisions.Add(new ContentCollision { Name = "nothing", Shape = ContentCollision.ShapeMesh });
            empty.Collisions.Add(new ContentCollision { Name = "dot", Shape = ContentCollision.ShapeSphere });
            List<string> emptyCodes = AssetValidator.Validate(empty, v1).Where(i => i.Code == "LCC028").Select(i => i.Message).ToList();
            t.Check(emptyCodes.Count == 2 && emptyCodes.Any(m => m.Contains("no triangles")) && emptyCodes.Any(m => m.Contains("no size")), "LCC028: mesh without triangles, sphere without radius",
                string.Join(" | ", emptyCodes.ToArray()));
            t.Check(!SelfTest.Codes(empty, v1, null).Contains("LCC031"), "shapes that failed LCC028 are not placed (no LCC031)");

            ContentAsset shader = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            shader.Materials[0].Shader = "gta_normal_spec";
            t.Check(SelfTest.Codes(shader, v1, "error").Contains("LCC019"), "LCC019: shader outside the capabilities");
            CompilerCapabilities specular = new CompilerCapabilities("test", 1, 1, new[] { "gta_default", "gta_normal_spec" }, new[] { "prop" }, new string[0], false);
            t.Check(!SelfTest.Codes(shader, specular, null).Contains("LCC019"), "LCC019 follows the capabilities");
        }

        private const string ManifestHead = "{\"schemaVersion\":1,\"name\":\"lf_test\",\"source\":\"lf_test.gltf\",\"template\":{\"archive\":\"pc/models/cdimages/weapons.img\",\"model\":\"amb_nailgun\"},\"textureDictionary\":\"lf_test\",\"drawDistanceMeters\":100";

        private static AssetManifest Manifest(string type, string lodDistances)
        {
            return SelfTest.ParseManifest(ManifestHead + ",\"type\":\"" + type + "\"" + (lodDistances != null ? ",\"lodDistancesMeters\":[" + lodDistances + "]" : "") + "}");
        }

        private static List<string> ManifestCodes(ContentAsset asset, AssetManifest manifest, CompilerCapabilities capabilities, string severity)
        {
            return AssetValidator.Validate(asset, manifest, capabilities).Where(i => severity == null || i.Severity == severity).Select(i => i.Code).ToList();
        }

        internal static void WorldObjectRules(SelfTest.Runner t, string output)
        {
            CompilerCapabilities v1 = CompilerCapabilities.Current;
            CompilerCapabilities objects = new CompilerCapabilities("test", 1, 1, new[] { "gta_default" }, new[] { "prop", "object" }, ContentCollision.Shapes, true);
            ContentAsset bare = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            ContentAsset solid = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            solid.Collisions.Add(BoxCollision("hull", 0, 0, 0.5f, 0.5f));

            CompilerCapabilities propsOnly = new CompilerCapabilities("props-only", 1, 1, new[] { "gta_default" }, new[] { "prop" }, new string[0], false);
            t.Check(ManifestCodes(bare, Manifest("object", null), propsOnly, "error").Contains("LCC033"), "type object with a compiler that builds props only: LCC033");
            t.Check(!ManifestCodes(bare, Manifest("object", null), v1, null).Contains("LCC033"), "v1 builds world objects (session 6): no LCC033");
            t.Check(!ManifestCodes(solid, Manifest("object", null), objects, null).Contains("LCC033"), "LCC033 follows the capabilities");
            t.Check(!ManifestCodes(bare, Manifest("prop", null), v1, null).Contains("LCC033"), "type prop with v1: no LCC033");
            t.Check(ManifestCodes(bare, Manifest("object", null), objects, "warning").Contains("LCC034"), "world object without collision: LCC034");
            t.Check(!ManifestCodes(solid, Manifest("object", null), objects, null).Contains("LCC034"), "world object with collision: no LCC034");
            t.Check(!ManifestCodes(bare, Manifest("prop", null), objects, null).Contains("LCC034"), "prop without collision: no LCC034");
            t.Check(ManifestCodes(solid, Manifest("object", null), objects, "error").Count == 0, "a complete world object passes when the capabilities allow it",
                string.Join(",", ManifestCodes(solid, Manifest("object", null), objects, "error").ToArray()));

            ContentAsset gap = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("c", 2, 0, 0.5f));
            t.Check(ManifestCodes(gap, Manifest("prop", null), v1, "warning").Contains("LCC036"), "LOD 2 without LOD 1: LCC036");
            ContentAsset lods = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("b", 1, 0, 0.5f));
            t.Check(!ManifestCodes(lods, Manifest("prop", null), v1, null).Contains("LCC036"), "LODs 0 and 1: no LCC036");
            t.Check(ManifestCodes(lods, Manifest("prop", "30"), v1, "error").Contains("LCC035"), "one distance for two LODs: LCC035");
            t.Check(ManifestCodes(lods, Manifest("prop", "40,30"), v1, "error").Contains("LCC035"), "descending distances: LCC035");
            t.Check(ManifestCodes(lods, Manifest("prop", "30,150"), v1, "error").Contains("LCC035"), "last distance beyond drawDistanceMeters: LCC035");
            List<string> good = ManifestCodes(lods, Manifest("prop", "30,100"), v1, null);
            t.Check(!good.Contains("LCC035") && good.Contains("LCC037"), "valid distances: no LCC035, LCC037 (not written by v1)", string.Join(",", good.ToArray()));
            t.Check(!ManifestCodes(lods, Manifest("prop", "30,100"), objects, null).Contains("LCC037"), "LCC037 follows the capabilities");
        }

        internal static void ReportStructure(SelfTest.Runner t, string output)
        {
            ContentAsset asset = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1), SelfTest.Box("b", 0, 1, 0.5f), SelfTest.Box("c", 1, 1, 0.5f));
            asset.Materials[1].Shader = "gta_normal_spec";
            ContentCollision box = BoxCollision("hull", 0, 0, 0.5f, 0.5f);
            box.Surface = "wood";
            asset.Collisions.Add(box);
            asset.Collisions.Add(new ContentCollision { Name = "stub", Shape = ContentCollision.ShapeCapsule, Problem = "too short" });
            AssetManifest manifest = Manifest("object", "30,60");
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, manifest, CompilerCapabilities.Current);
            string text = Program.ReportJson(manifest, asset, issues, null, new List<string>(), Program.StatusInvalid);
            Dictionary<string, object> report = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(text);
            Dictionary<string, object> structure = (Dictionary<string, object>)report["structure"];
            IList lods = (IList)structure["lods"];
            IList geometries = (IList)((Dictionary<string, object>)lods[0])["geometries"];
            t.Check((string)report["status"] == "invalid" && (string)report["type"] == "object" && ((Dictionary<string, object>)report["capabilities"])["version"].Equals("v1"), "status, type and capabilities");
            t.Check(lods.Count == 2 && geometries.Count == 2, "two LODs, two geometries in LOD 0", lods.Count + " " + geometries.Count);
            Dictionary<string, object> second = (Dictionary<string, object>)geometries[1];
            t.Check((string)second["material"] == "second" && (string)second["shader"] == "gta_normal_spec" && Convert.ToInt32(second["triangles"]) == 12, "geometry names its material, shader and triangles");
            t.Check(((IList)structure["lodDistancesMeters"]).Cast<object>().Select(Convert.ToDouble).SequenceEqual(new[] { 30.0, 60.0 }), "LOD distances");
            IList collision = (IList)structure["collision"];
            Dictionary<string, object> hull = (Dictionary<string, object>)collision[0];
            t.Check(collision.Count == 2 && (string)hull["shape"] == "box" && (string)hull["surface"] == "wood" && ((IList)hull["halfExtents"]).Count == 3 && ((IList)hull["axes"]).Count == 3,
                "box collision: shape, surface, half extents, axes");
            t.Check((string)((Dictionary<string, object>)collision[1])["problem"] == "too short", "a shape that failed fitting reports its problem");
            if (output != null) { File.WriteAllText(Path.Combine(output, "report_structure.json"), text, Encoding.UTF8); }
        }

        internal static void FixtureMatching(SelfTest.Runner t, string output)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            object actual = json.DeserializeObject("{ \"a\": 1.0004, \"b\": [1, \"x\", { \"c\": true }], \"d\": null }");
            Func<string, List<string>> match = expected => { List<string> problems = new List<string>(); FixtureCheck.Match(json.DeserializeObject(expected), actual, "report", problems); return problems; };
            t.Check(match("{ \"a\": 1 }").Count == 0, "numbers match within the tolerance");
            t.Check(match("{ \"a\": 1.01 }").Any(p => p.StartsWith("report.a")), "numbers outside the tolerance fail with their path");
            t.Check(match("{ \"b\": [1, \"x\", { \"c\": true }], \"d\": null }").Count == 0, "arrays, strings, booleans and null match");
            t.Check(match("{ \"b\": [1, \"x\"] }").Any(p => p.Contains("expected 2 items")), "array length is checked");
            t.Check(match("{ \"b\": [1, \"x\", { \"c\": false }] }").Any(p => p.StartsWith("report.b[2].c")), "nested mismatch names its path");
            t.Check(match("{ \"e\": 1 }").Any(p => p == "report.e: missing"), "a missing key fails");
        }
    }
}
