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
            SortedSet<string> names = new SortedSet<string>();
            foreach (string file in Directory.GetFiles(Path.Combine(repoRoot, Path.Combine("src", "LibertyFramework")), "*.cs", SearchOption.AllDirectories))
            {
                foreach (Match match in Regex.Matches(File.ReadAllText(file), "(?:Function\\.Call(?:<[^>]+>)?|NativeCall\\.\\w+)\\(\"([A-Z0-9_]+)\""))
                {
                    names.Add(match.Groups[1].Value);
                }
            }
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
