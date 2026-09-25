using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.Verify
{
    // ADR-0006: the native core's natives (native/LibertyCore/src/natives.cpp) must be CE natives registered by
    // GTAIV.exe, agree with docs/game-api/native-hashes.csv, and match the C# ABI mirror's id list and count.
    internal static class EngineChecks
    {
        internal static void Run(string exePath, string repoRoot, Checker check)
        {
            CodeScanner scanner = new CodeScanner(new ImageFileMemory(exePath));
            Dictionary<string, uint> known = new Dictionary<string, uint>();
            foreach (string line in File.ReadAllLines(Path.Combine(repoRoot, Path.Combine("docs", Path.Combine("game-api", "native-hashes.csv")))))
            {
                if (line.StartsWith("#") || line.Trim().Length == 0) { continue; }
                string[] parts = line.Split(',');
                known[parts[0]] = uint.Parse(parts[1].Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            string natives = File.ReadAllText(Path.Combine(repoRoot, Path.Combine("native", Path.Combine("LibertyCore", Path.Combine("src", "natives.cpp")))));
            MatchCollection entries = Regex.Matches(natives, "\\{ \"([A-Z0-9_]+)\", 0x([0-9A-Fa-f]{8}), (\\d), (\\d) \\}");
            string header = File.ReadAllText(Path.Combine(repoRoot, Path.Combine("native", Path.Combine("LibertyCore", Path.Combine("include", "liberty_core.h")))));
            Match enumBody = Regex.Match(header, "enum lc_native_id\\s*\\{([^}]*)\\}");
            List<string> ids = new List<string>();
            foreach (Match id in Regex.Matches(enumBody.Groups[1].Value, "LC_N_([A-Z0-9_]+)")) { if (id.Groups[1].Value != "COUNT") { ids.Add(id.Groups[1].Value); } }
            check.True("core native table matches lc_native_id", entries.Count == ids.Count && entries.Count > 0, "table=" + entries.Count + " enum=" + ids.Count);
            string abi = File.ReadAllText(Path.Combine(repoRoot, Path.Combine("src", Path.Combine("LibertyFramework", Path.Combine("Engine", Path.Combine("Core", "CoreAbi.cs"))))));
            Match countMatch = Regex.Match(abi, "NativeCount = (\\d+)");
            check.True("C# CoreAbi.NativeCount matches the core", countMatch.Success && int.Parse(countMatch.Groups[1].Value) == entries.Count,
                "abi=" + countMatch.Groups[1].Value + " core=" + entries.Count);
            for (int i = 0; i < entries.Count; i++)
            {
                string name = entries[i].Groups[1].Value;
                uint hash = uint.Parse(entries[i].Groups[2].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                check.True("core native order " + i + ": " + name, i < ids.Count && ids[i] == name.Replace("IS_CHAR_", "IS_CHAR_"), i < ids.Count ? "enum=" + ids[i] : "missing");
                uint listed;
                if (known.TryGetValue(name, out listed)) { check.True("core native hash agrees with native-hashes.csv: " + name, listed == hash, "core=0x" + hash.ToString("X8") + " csv=0x" + listed.ToString("X8")); }
                check.True("core native registered in exe: " + name, scanner.FindNative(hash) != 0, "hash=0x" + hash.ToString("X8"));
            }
        }
    }
}
