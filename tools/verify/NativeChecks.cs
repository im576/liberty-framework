using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.Verify
{
    // Every native name the DLL passes to Function.Call must be a known CE native (docs/game-api/
    // native-hashes.csv, taken from FusionFix's CE native list) whose hash GTAIV.exe 1.2.0.59 registers.
    // CE native hashes are not derivable from the name, so the snapshot is the reference.
    internal static class NativeChecks
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
            SortedSet<string> names = ReferencedNames(repoRoot);
            check.True("natives referenced by the DLL found", names.Count >= 15, "count=" + names.Count);
            foreach (string name in names)
            {
                uint hash;
                bool listed = known.TryGetValue(name, out hash);
                uint function = listed ? scanner.FindNative(hash) : 0;
                check.True("native registered in exe: " + name, listed && function != 0,
                    listed ? "hash=0x" + hash.ToString("X8") + " handler=0x" + function.ToString("X8") : "not in native-hashes.csv");
            }
            foreach (KeyValuePair<string, uint> pair in known)
            {
                if (names.Contains(pair.Key)) { continue; }
                check.True("resolver anchor native registered: " + pair.Key, scanner.FindNative(pair.Value) != 0, "hash=0x" + pair.Value.ToString("X8"));
            }

            // Function.Call("NAME") hashes the name (Jenkins one-at-a-time, lower case) and the installed
            // ScriptHook.dll translates that hash to the CE hash through a table of adjacent (name hash, CE hash)
            // pairs. A name missing from that table would fail only in game, so check every one here.
            string scriptHook = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(exePath)), "ScriptHook.dll");
            bool present = File.Exists(scriptHook);
            check.True("ScriptHook.dll present next to GTAIV.exe", present, scriptHook);
            if (!present) { return; }
            byte[] table = File.ReadAllBytes(scriptHook);
            foreach (string name in names)
            {
                uint ceHash;
                if (!known.TryGetValue(name, out ceHash)) { continue; }
                uint nameHash = NameHash(name);
                check.True("ScriptHook.dll maps " + name + " to its CE hash", HasPair(table, nameHash, ceHash),
                    "name_hash=0x" + nameHash.ToString("X8") + " ce_hash=0x" + ceHash.ToString("X8"));
            }
        }

        // Native names the DLL calls: literal first arguments of Function.Call / NativeCall.*, plus every name literal in a
        // "string native = ...;" declaration (a name chosen at run time, e.g. TASK_PLAY_ANIM or its _UPPER_BODY variant).
        internal static SortedSet<string> ReferencedNames(string repoRoot)
        {
            SortedSet<string> names = new SortedSet<string>();
            foreach (string file in Directory.GetFiles(Path.Combine(repoRoot, Path.Combine("src", "LibertyFramework")), "*.cs", SearchOption.AllDirectories))
            {
                string text = File.ReadAllText(file);
                foreach (Match match in Regex.Matches(text, "(?:Function\\.Call(?:<[^>]+>)?|NativeCall\\.\\w+)\\(\"([A-Z0-9_]+)\""))
                {
                    names.Add(match.Groups[1].Value);
                }
                foreach (Match declaration in Regex.Matches(text, "string native\\s*=([^;]+);"))
                {
                    foreach (Match literal in Regex.Matches(declaration.Groups[1].Value, "\"([A-Z0-9_]+)\"")) { names.Add(literal.Groups[1].Value); }
                }
            }
            return names;
        }

        // Repository only (runs in the cloud): every referenced name has a CE hash in native-hashes.csv. Whether GTAIV.exe
        // registers that hash is the game section's job.
        internal static void RunListed(string repoRoot, Checker check)
        {
            HashSet<string> known = new HashSet<string>();
            foreach (string line in File.ReadAllLines(Path.Combine(repoRoot, Path.Combine("docs", Path.Combine("game-api", "native-hashes.csv")))))
            {
                if (line.StartsWith("#") || line.Trim().Length == 0) { continue; }
                known.Add(line.Split(',')[0]);
            }
            SortedSet<string> names = ReferencedNames(repoRoot);
            check.True("native names found in the DLL sources", names.Count >= 15, "count=" + names.Count);
            List<string> missing = new List<string>();
            foreach (string name in names) { if (!known.Contains(name)) { missing.Add(name); } }
            check.True("every native the DLL calls is listed in native-hashes.csv", missing.Count == 0, missing.Count == 0 ? names.Count + " names" : string.Join(",", missing.ToArray()));
            check.True("the run-time-chosen animation natives are seen by the scan", names.Contains("TASK_PLAY_ANIM_UPPER_BODY") && names.Contains("TASK_PLAY_ANIM_SECONDARY"), "");
        }

        private static uint NameHash(string name)
        {
            uint hash = 0;
            foreach (char character in name.ToLowerInvariant())
            {
                hash = unchecked(hash + (byte)character);
                hash = unchecked(hash + (hash << 10));
                hash ^= hash >> 6;
            }
            hash = unchecked(hash + (hash << 3));
            hash ^= hash >> 11;
            return unchecked(hash + (hash << 15));
        }

        private static bool HasPair(byte[] data, uint first, uint second)
        {
            for (int offset = 0; offset + 8 <= data.Length; offset += 4)
            {
                if (BitConverter.ToUInt32(data, offset) == first && BitConverter.ToUInt32(data, offset + 4) == second) { return true; }
            }
            return false;
        }
    }
}
