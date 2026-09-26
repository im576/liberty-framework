using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LibertyFramework.Verify
{
    // SDK examples in the docs stay true: tools/verify.ps1 compiles docs/sdk/examples/*.cs against the current SDK
    // (SdkExamples.dll, warnings as errors), and every C# block in the docs that teach the SDK must be taken from those
    // files (each code line appears in an example), so a block cannot drift from code that compiles.
    internal static class SdkExampleChecks
    {
        private static readonly string[] Docs = { "docs/sdk/README.md", "docs/architecture/ENGINE.md" };

        internal static void Run(string repo, Checker check)
        {
            string folder = Path.Combine(repo, Path.Combine("docs", Path.Combine("sdk", "examples")));
            HashSet<string> lines = new HashSet<string>();
            foreach (string file in Directory.GetFiles(folder, "*.cs")) { foreach (string line in File.ReadAllLines(file)) { lines.Add(line.Trim()); } }

            string dll = Path.Combine(Path.GetDirectoryName(typeof(SdkExampleChecks).Assembly.Location), "SdkExamples.dll");
            int modules = 0;
            if (File.Exists(dll))
            {
                foreach (Type type in Assembly.LoadFrom(dll).GetTypes())
                {
                    foreach (object attribute in type.GetCustomAttributes(false)) { if (attribute.GetType().FullName == "Liberty.Sdk.ModuleAttribute") { modules++; } }
                }
            }
            check.True("SDK examples compiled against the current SDK (docs/sdk/examples)", modules >= 4, "modules=" + modules);

            foreach (string doc in Docs)
            {
                string text = File.ReadAllText(Path.Combine(repo, doc.Replace('/', Path.DirectorySeparatorChar)));
                int block = 0;
                foreach (Match m in Regex.Matches(text, "```csharp\\r?\\n(.*?)```", RegexOptions.Singleline))
                {
                    block++;
                    List<string> missing = new List<string>();
                    foreach (string raw in m.Groups[1].Value.Split('\n'))
                    {
                        string line = raw.Trim();
                        if (line.Length > 0 && !lines.Contains(line)) { missing.Add(line); }
                    }
                    check.True(doc + " C# block " + block + " comes from a compiled example", missing.Count == 0,
                        missing.Count == 0 ? "" : "not in docs/sdk/examples: " + missing[0]);
                }
            }
        }
    }
}
