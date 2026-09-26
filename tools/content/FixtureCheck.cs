using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace LibertyFramework.Content
{
    // `fixtures <dir>`: authoring fixtures (tests/content/fixtures, made in Blender by tools/blender/examples/make_fixtures.py)
    // are imported and validated like `validate`, and their report compared with expect.json:
    //   status  "valid" or "invalid"
    //   errors  the exact set of error codes
    //   codes   codes that must appear (any severity)
    //   report  a subset of report.json: every value given must match (numbers within NumberTolerance, arrays by position)
    // Needs no game: this is how the IR, the validator and the capabilities are pinned against real Blender exports.
    internal static class FixtureCheck
    {
        // Fixture dimensions are in metres; Blender's float32 export and the compiler's arithmetic stay far inside this.
        internal const double NumberTolerance = 1e-3;

        internal static int Run(string directory)
        {
            if (!Directory.Exists(directory)) { Console.WriteLine("fixtures: folder not found: " + directory); return 1; }
            int passed = 0, failed = 0;
            foreach (string folder in Directory.GetDirectories(directory).OrderBy(f => f, StringComparer.Ordinal))
            {
                string manifestPath = Path.Combine(folder, "asset.json"), expectPath = Path.Combine(folder, "expect.json");
                if (!File.Exists(manifestPath) || !File.Exists(expectPath)) { continue; }
                List<string> problems;
                try { problems = Check(manifestPath, expectPath); }
                catch (Exception error) { problems = new List<string> { "threw " + error.GetType().Name + ": " + error.Message }; }
                string name = Path.GetFileName(folder);
                if (problems.Count == 0) { passed++; Console.WriteLine("  PASS " + name); continue; }
                failed++;
                foreach (string problem in problems) { Console.WriteLine("  FAIL " + name + ": " + problem); }
            }
            if (passed + failed == 0) { Console.WriteLine("fixtures: no fixture (asset.json + expect.json) under " + directory); return 1; }
            Console.WriteLine("fixtures: " + (failed == 0 ? "ok" : "FAILED") + " passed=" + passed + " failed=" + failed);
            return failed == 0 ? 0 : 1;
        }

        internal static List<string> Check(string manifestPath, string expectPath)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            Dictionary<string, object> expect = (Dictionary<string, object>)json.DeserializeObject(File.ReadAllText(expectPath));
            AssetManifest manifest = AssetManifest.Load(manifestPath);
            ContentAsset asset = GltfImporter.Import(manifest.SourcePath);
            manifest.ResolveWriter(asset);
            List<AssetValidator.Issue> issues = AssetValidator.Validate(asset, manifest, CompilerCapabilities.For(manifest));
            string status = AssetValidator.HasErrors(issues) ? Program.StatusInvalid : Program.StatusValid;
            Dictionary<string, object> report = (Dictionary<string, object>)json.DeserializeObject(Program.ReportJson(manifest, asset, issues, null, new List<string>(), status));

            List<string> problems = new List<string>();
            object value;
            if (expect.TryGetValue("status", out value) && (string)value != status) { problems.Add("status " + status + ", expected " + value); }
            if (expect.TryGetValue("errors", out value))
            {
                string[] want = ((IList)value).Cast<object>().Select(o => (string)o).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToArray();
                string[] got = issues.Where(i => i.Severity == "error").Select(i => i.Code).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToArray();
                if (!want.SequenceEqual(got)) { problems.Add("errors [" + string.Join(", ", got) + "], expected [" + string.Join(", ", want) + "]"); }
            }
            if (expect.TryGetValue("codes", out value))
            {
                foreach (string code in ((IList)value).Cast<object>().Select(o => (string)o))
                {
                    if (!issues.Any(i => i.Code == code)) { problems.Add("no " + code + " issue"); }
                }
            }
            if (expect.TryGetValue("report", out value)) { Match(value, report, "report", problems); }
            if (problems.Count > 0) { problems.AddRange(issues.Select(i => "  issue: " + i)); }
            return problems;
        }

        // Every value in expected must be in actual; problems name the path of each mismatch.
        internal static void Match(object expected, object actual, string path, List<string> problems)
        {
            Dictionary<string, object> expectedObject = expected as Dictionary<string, object>;
            if (expectedObject != null)
            {
                Dictionary<string, object> actualObject = actual as Dictionary<string, object>;
                if (actualObject == null) { problems.Add(path + ": expected an object, got " + Describe(actual)); return; }
                foreach (KeyValuePair<string, object> pair in expectedObject)
                {
                    object inner;
                    if (!actualObject.TryGetValue(pair.Key, out inner)) { problems.Add(path + "." + pair.Key + ": missing"); continue; }
                    Match(pair.Value, inner, path + "." + pair.Key, problems);
                }
                return;
            }
            IList expectedList = expected as IList;
            if (expectedList != null)
            {
                IList actualList = actual as IList;
                if (actualList == null || actualList.Count != expectedList.Count) { problems.Add(path + ": expected " + expectedList.Count + " items, got " + Describe(actual)); return; }
                for (int i = 0; i < expectedList.Count; i++) { Match(expectedList[i], actualList[i], path + "[" + i + "]", problems); }
                return;
            }
            if (IsNumber(expected))
            {
                if (!IsNumber(actual) || Math.Abs(Convert.ToDouble(expected, CultureInfo.InvariantCulture) - Convert.ToDouble(actual, CultureInfo.InvariantCulture)) > NumberTolerance)
                {
                    problems.Add(path + ": expected " + Describe(expected) + ", got " + Describe(actual));
                }
                return;
            }
            if (!Equals(expected, actual)) { problems.Add(path + ": expected " + Describe(expected) + ", got " + Describe(actual)); }
        }

        private static bool IsNumber(object value) { return value is int || value is long || value is decimal || value is double || value is float; }

        private static string Describe(object value)
        {
            if (value == null) { return "null"; }
            if (value is IList) { return ((IList)value).Count + " items"; }
            if (value is Dictionary<string, object>) { return "an object"; }
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
