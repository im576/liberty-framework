using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Self-test groups for session 5 (T-032): borrowed collision (the candidate rule, explicit and missing models, the .wbn
    // written, reported and packaged, the validator's codes) and the bounds layout probe's word classification. Fake bounds
    // resources here are fixtures shaped by this test, not claims about the game's classes.
    internal static class CollisionSelfTest
    {
        // An RSC resource of type 32 whose root has a vtable-like word, a system pointer to +0x40, a small integer and a
        // float; at +0x40 a small integer. The type and words are the test's own.
        internal static byte[] FakeBounds(uint vtable, uint count)
        {
            byte[] body = new byte[256];
            BitConverter.GetBytes(vtable).CopyTo(body, 0);
            BitConverter.GetBytes(0x50000040u).CopyTo(body, 4);
            BitConverter.GetBytes(count).CopyTo(body, 8);
            BitConverter.GetBytes(1.5f).CopyTo(body, 12);
            BitConverter.GetBytes(7u).CopyTo(body, 0x40);
            return new RscResource { Type = 32, Flags = 1, Body = body }.Serialize();
        }

        private static string Manifest(string name, string extra)
        {
            return "{\"schemaVersion\":1,\"name\":\"" + name + "\",\"type\":\"prop\",\"source\":\"x.gltf\",\"template\":{\"archive\":\"pc/models/cdimages/test.img\",\"model\":\"none\"}," +
                "\"textureDictionary\":\"" + name + "\",\"drawDistanceMeters\":100" + extra + "}";
        }

        internal static void Borrow(SelfTest.Runner t, string output)
        {
            t.Throws<InvalidDataException>(() => SelfTest.ParseManifest(Manifest("lf_c", ",\"collision\":{}")), "collision without borrow is refused");
            AssetManifest borrowing = SelfTest.ParseManifest(Manifest("lf_c", ",\"collision\":{\"borrow\":{\"archive\":\"*\",\"model\":\"auto\"}}"));
            t.Check(borrowing.BorrowsCollision && borrowing.Collision.Borrow.Model == "auto", "collision.borrow parses");

            string game = StructureSelfTest.FakeGameFiles(
                new KeyValuePair<string, byte[]>("zz_prop.wdr", new byte[] { 1 }), new KeyValuePair<string, byte[]>("zz_prop.wbn", FakeBounds(0xAB, 2)),
                new KeyValuePair<string, byte[]>("Aa_Prop.wdr", new byte[] { 1 }), new KeyValuePair<string, byte[]>("aa_prop.wbn", FakeBounds(0xCD, 3)),
                new KeyValuePair<string, byte[]>("lonely.wbn", FakeBounds(0xEF, 4)),
                new KeyValuePair<string, byte[]>("bbb_lods.wdr", SyntheticDrawable.Build(StructureSelfTest.FourLods()).Serialize()));
            try
            {
                List<string> notes = new List<string>();
                List<KeyValuePair<string, string>> candidates = BorrowedCollision.Candidates(game, null, new[] { "pc/models/cdimages/test.img" }, notes);
                t.Check(candidates.Select(c => c.Key).SequenceEqual(new[] { "aa_prop", "zz_prop" }), "candidates: drawables with a same-named .wbn, lower-case, by name (a lone .wbn is not one)",
                    string.Join(",", candidates.Select(c => c.Key).ToArray()));
                BorrowedCollision.Borrowed auto = BorrowedCollision.Find(game, borrowing.Collision.Borrow, notes);
                t.Check(auto.From == "pc/models/cdimages/test.img/aa_prop" && auto.ResourceType == 32 && BitConverter.ToUInt32(RscResource.Parse(auto.Resource).Body, 0) == 0xCD,
                    "auto borrows the first candidate's bounds, bytes unchanged", auto.From);
                BorrowedCollision.Borrowed named = BorrowedCollision.Find(game, new AssetManifest.TemplateRef { Archive = "pc/models/cdimages/test.img", Model = "lonely" }, notes);
                t.Check(named.From.EndsWith("/lonely") && BitConverter.ToUInt32(RscResource.Parse(named.Resource).Body, 0) == 0xEF, "a named model's bounds are borrowed even without a drawable");
                t.Throws<InvalidDataException>(() => BorrowedCollision.Find(game, new AssetManifest.TemplateRef { Archive = "*", Model = "missing" }, new List<string>()), "a missing model is an error");

                // The probe names the same first candidate the build borrows (the raycast-objects scenario's model).
                string probe = Path.Combine(game, "collision.json");
                Probe.Run(new[] { "collision", "--game", game, "--out", probe });
                Dictionary<string, object> inventory = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(probe));
                t.Check(((ArrayList)inventory["propCandidates"]).Cast<object>().First().ToString() == "aa_prop", "PROBE-collision's first candidate is the one auto borrows");

                // Build and package: a structure-writer crate (the synthetic game has no v1 template) with borrowed collision.
                string folder = Path.Combine(game, "asset"), build = Path.Combine(game, "build");
                Directory.CreateDirectory(folder);
                SampleAsset.Write(folder, "lf_col_test");
                File.WriteAllText(Path.Combine(folder, "asset.json"), Manifest("lf_col_test", ",\"source\":\"lf_col_test.gltf\",\"drawableWriter\":\"structure\",\"collision\":{\"borrow\":{\"archive\":\"*\",\"model\":\"auto\"}}")
                    .Replace("\"source\":\"x.gltf\",", ""));
                string status;
                t.Check(Program.Build(game, Path.Combine(folder, "asset.json"), build, out status) == 0 && status == "ok", "a build with borrowed collision is ok", status);
                string wbn = Path.Combine(build, "lf_col_test", "lf_col_test.wbn");
                t.Check(File.Exists(wbn) && File.ReadAllBytes(wbn).SequenceEqual(auto.Resource), "the borrowed bounds are written as <name>.wbn, unchanged");
                Dictionary<string, object> report = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(Path.Combine(build, "lf_col_test", "report.json")));
                Dictionary<string, object> collision = (Dictionary<string, object>)((Dictionary<string, object>)report["compiled"])["collision"];
                t.Check((string)collision["mode"] == "borrowed" && ((string)collision["from"]).EndsWith("/aa_prop") && (string)collision["file"] == "lf_col_test.wbn", "report.json compiled.collision");
                t.Check(((ArrayList)report["issues"]).Cast<Dictionary<string, object>>().Any(i => (string)i["code"] == "LCC039"), "LCC039 says the collision is borrowed");
                t.Check(Program.Package(game, build, "LibertyContent.img", "lf_content.ide", new[] { Path.Combine(folder, "asset.json") }) == 0, "package with borrowed collision");
                ImgArchive packed = ImgArchive.Open(Path.Combine(build, "LibertyContent.img"), null);
                t.Check(packed.Entries.Select(e => e.Name).OrderBy(n => n).SequenceEqual(new[] { "lf_col_test.wbn", "lf_col_test.wdr", "lf_col_test.wtd" }) &&
                    packed.Entries.First(e => e.Name == "lf_col_test.wbn").ResourceType == 32, "the IMG holds the .wbn with its resource type", string.Join(",", packed.Entries.Select(e => e.Name + ":" + e.ResourceType).ToArray()));
                // Rebuilt without the borrow: the stale .wbn is removed so it is never packaged.
                File.WriteAllText(Path.Combine(folder, "asset.json"), Manifest("lf_col_test", ",\"source\":\"lf_col_test.gltf\",\"drawableWriter\":\"structure\"").Replace("\"source\":\"x.gltf\",", ""));
                Program.Build(game, Path.Combine(folder, "asset.json"), build, out status);
                t.Check(!File.Exists(wbn), "a build without the borrow deletes the old .wbn");
            }
            finally { try { Directory.Delete(game, true); } catch (IOException) { } }

            string empty = StructureSelfTest.FakeGameFiles(new KeyValuePair<string, byte[]>("lonely.wbn", FakeBounds(0xEF, 4)));
            try
            {
                List<string> emptyNotes = new List<string>();
                t.Check(BorrowedCollision.Find(empty, new AssetManifest.TemplateRef { Archive = "*", Model = "auto" }, emptyNotes) == null && emptyNotes.Any(n => n.Contains("built without collision")),
                    "auto with no candidate: no borrow and a note (the package goes on)");
            }
            finally { try { Directory.Delete(empty, true); } catch (IOException) { } }

            ContentAsset authored = SelfTest.Asset(SelfTest.Box("a", 0, 0, 1));
            ContentCollision box = new ContentCollision { Name = "hull", Shape = ContentCollision.ShapeBox };
            box.HalfExtents[0] = box.HalfExtents[1] = box.HalfExtents[2] = 0.5f; box.Centre[2] = 0.5f;
            authored.Collisions.Add(box);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(authored, borrowing, CompilerCapabilities.Current);
            t.Check(issues.Any(i => i.Code == "LCC040" && i.Severity == "warning") && !issues.Any(i => i.Code == "LCC032"), "authored shapes with a borrow: LCC040 warning instead of LCC032");
            AssetManifest world = SelfTest.ParseManifest(Manifest("lf_w", ",\"collision\":{\"borrow\":{\"archive\":\"*\",\"model\":\"auto\"}}").Replace("\"type\":\"prop\"", "\"type\":\"object\""));
            List<string> worldCodes = AssetValidator.Validate(SelfTest.Asset(SelfTest.Box("a", 0, 0, 1)), world, CompilerCapabilities.Current).Select(i => i.Code).ToList();
            t.Check(!worldCodes.Contains("LCC034") && !worldCodes.Contains("LCC033") && worldCodes.Contains("LCC039"), "a world object with borrowed collision: no LCC033, no LCC034", string.Join(",", worldCodes.ToArray()));
        }

        internal static void BoundsProbe(SelfTest.Runner t, string output)
        {
            string game = StructureSelfTest.FakeGameFiles(new KeyValuePair<string, byte[]>("a.wbn", FakeBounds(0x00ABCDEF, 3)), new KeyValuePair<string, byte[]>("b.wbn", FakeBounds(0x00ABCDEF, 5)),
                new KeyValuePair<string, byte[]>("c.wbd", FakeBounds(0x00123456, 1)), new KeyValuePair<string, byte[]>("d.wdr", new byte[] { 1, 2, 3 }));
            try
            {
                string path = Path.Combine(game, "bounds.json");
                t.Check(Probe.Run(new[] { "bounds", "--game", game, "--out", path }) == 0, "probe bounds exits 0");
                Dictionary<string, object> report = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(File.ReadAllText(path));
                Dictionary<string, object> layout = (Dictionary<string, object>)report["layout"];
                t.Check(Convert.ToInt32(report["resources"]) == 3 && layout.Keys.SequenceEqual(new[] { ".wbn rsc type 32 root 0x00ABCDEF", ".wbd rsc type 32 root 0x00123456" }),
                    "classes by extension, type and root word, most files first", string.Join(" | ", layout.Keys.ToArray()));
                Dictionary<string, object> wbn = (Dictionary<string, object>)layout[".wbn rsc type 32 root 0x00ABCDEF"];
                List<Dictionary<string, object>> root = ((ArrayList)wbn["root"]).Cast<Dictionary<string, object>>().ToList();
                t.Check(Convert.ToInt32(wbn["files"]) == 2 && Convert.ToInt32(root[1]["systemPointer"]) == 100 && Convert.ToInt32(root[2]["smallInt"]) == 100 && Convert.ToInt32(root[3]["float"]) == 100 &&
                    Convert.ToInt32(root[4]["zero"]) == 100, "root words classified: pointer, small integer, float, zero");
                Dictionary<string, object> values = (Dictionary<string, object>)root[2]["smallIntValues"];
                t.Check(values.Keys.OrderBy(k => k).SequenceEqual(new[] { "3", "5" }), "small-integer values listed (counts and type codes)");
                t.Check(!root[3].ContainsKey("smallIntValues") && !File.ReadAllText(path).Contains("1.5"), "float values are never reported (structure only)");
                Dictionary<string, object> pointed = (Dictionary<string, object>)wbn["pointed"];
                t.Check(pointed.ContainsKey("root+0x04") && Convert.ToInt32(((Dictionary<string, object>)((ArrayList)pointed["root+0x04"])[0])["smallInt"]) == 100, "the pointed structure is measured one level down");
                t.Check(((Dictionary<string, object>)wbn["rootSizeBound"]).ContainsKey("<= 0x7F"), "the smallest pointer target bounds the root size");
            }
            finally { try { Directory.Delete(game, true); } catch (IOException) { } }
        }
    }
}
