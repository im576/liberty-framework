using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using LibertyFramework.Engine.Core;

namespace LibertyFramework.Verify
{
    // The C ABI of LibertyCore (native/LibertyCore/include/liberty_core.h) against its C# mirror (Engine/Core/CoreAbi.cs)
    // and the snapshot accessors in CoreBridge, without the game: a mismatch here is a core the engine would misread
    // (or refuse with engine_core_abi_mismatch) in game. The header's rule (4-byte fields only, no padding) makes the
    // layout computable from the text: every field is one word, arrays are N words, nested structs are their words.
    internal static unsafe class CoreAbiChecks
    {
        private sealed class CField { internal string Name; internal string Type; internal int Words; internal int Offset; }

        private static readonly Dictionary<string, string> Mirrors = new Dictionary<string, string>
        {
            { "lc_address_book", "LcAddressBook" }, { "lc_frame_input", "LcFrameInput" }, { "lc_world", "LcWorld" },
            { "lc_player", "LcPlayer" }, { "lc_pools", "LcPools" }, { "lc_ped", "LcPed" }, { "lc_vehicle", "LcVehicle" },
            { "lc_bullet", "LcBullet" }, { "lc_damage", "LcDamage" }, { "lc_event", "LcEvent" }, { "lc_fault", "LcFault" },
            { "lc_ray", "LcRay" }, { "lc_ray_hit", "LcRayHit" }, { "lc_ray_stats", "LcRayStats" },
        };

        internal static void Run(string repo, Checker check)
        {
            string header = File.ReadAllText(Path.Combine(repo, Path.Combine("native", Path.Combine("LibertyCore", Path.Combine("include", "liberty_core.h")))));
            header = Regex.Replace(header, "//[^\n]*", "");
            Dictionary<string, long> defines = Defines(header);
            Dictionary<string, List<CField>> structs = Structs(header, defines);

            check.Equal("ABI: CoreAbi.Version matches LC_ABI_VERSION", (int)defines["LC_ABI_VERSION"], (int)CoreAbi.Version);
            check.Equal("ABI: MaxPeds", (int)defines["LC_MAX_PEDS"], CoreAbi.MaxPeds);
            check.Equal("ABI: MaxVehicles", (int)defines["LC_MAX_VEHICLES"], CoreAbi.MaxVehicles);
            check.Equal("ABI: MaxEvents", (int)defines["LC_MAX_EVENTS"], CoreAbi.MaxEvents);
            check.Equal("ABI: MaxBullets", (int)defines["LC_MAX_BULLETS"], CoreAbi.MaxBullets);
            check.Equal("ABI: MaxDamages", (int)defines["LC_MAX_DAMAGES"], CoreAbi.MaxDamages);
            check.Equal("ABI: LcRay.MaxIgnore", (int)defines["LC_RAY_MAX_IGNORE"], LcRay.MaxIgnore);
            check.Equal("ABI: LcRayHit.RawWords", 24, LcRayHit.RawWords);

            FlagChecks(check, defines);
            EnumChecks(header, check);

            foreach (KeyValuePair<string, string> pair in Mirrors)
            {
                Type mirror = typeof(CoreAbi).Assembly.GetType("LibertyFramework.Engine.Core." + pair.Value, true);
                List<CField> fields;
                if (!structs.TryGetValue(pair.Key, out fields)) { check.True("ABI: " + pair.Key + " found in the header", false, ""); continue; }
                List<string> c = Flatten(pair.Key, structs), cs = FlattenManaged(mirror);
                check.Equal("ABI: " + pair.Value + " size matches " + pair.Key, c.Count * 4, Marshal.SizeOf(mirror));
                int firstDifference = FirstDifference(c, cs);
                check.True("ABI: " + pair.Value + " fields match " + pair.Key + " word for word (type per word)", firstDifference < 0,
                    firstDifference < 0 ? c.Count + " words" : "word " + firstDifference + ": header " + At(c, firstDifference) + " vs C# " + At(cs, firstDifference));
            }

            SnapshotChecks(structs, check);
        }

        // ---- header parsing ----

        private static Dictionary<string, long> Defines(string header)
        {
            Dictionary<string, long> values = new Dictionary<string, long>();
            foreach (Match m in Regex.Matches(header, @"#define\s+(LC_\w+)\s+\(?(-?(?:0x[0-9A-Fa-f]+|\d+))u?\)?"))
            {
                string text = m.Groups[2].Value;
                bool negative = text.StartsWith("-");
                if (negative) { text = text.Substring(1); }
                long value = text.StartsWith("0x") ? long.Parse(text.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) : long.Parse(text, CultureInfo.InvariantCulture);
                values[m.Groups[1].Value] = negative ? -value : value;
            }
            return values;
        }

        private static Dictionary<string, List<CField>> Structs(string header, Dictionary<string, long> defines)
        {
            Dictionary<string, List<CField>> structs = new Dictionary<string, List<CField>>();
            foreach (Match m in Regex.Matches(header, @"typedef\s+struct\s+(\w+)\s*\{([^}]*)\}\s*(\w+)\s*;"))
            {
                List<CField> fields = new List<CField>();
                int offset = 0;
                foreach (string raw in m.Groups[2].Value.Split(';'))
                {
                    string declaration = raw.Trim();
                    if (declaration.Length == 0) { continue; }
                    Match d = Regex.Match(declaration, @"^(\w+)\s+(.+)$", RegexOptions.Singleline);
                    string type = d.Groups[1].Value;
                    foreach (string part in d.Groups[2].Value.Split(','))
                    {
                        Match name = Regex.Match(part.Trim(), @"^(\w+)(?:\[(\w+)\])?$");
                        int count = 1;
                        if (name.Groups[2].Success)
                        {
                            string n = name.Groups[2].Value;
                            count = char.IsDigit(n[0]) ? int.Parse(n, CultureInfo.InvariantCulture) : (int)defines[n];
                        }
                        int words = (type == "int32_t" || type == "uint32_t" || type == "float") ? 1 : structs[type].Count > 0 ? Words(type, structs) : 0;
                        CField field = new CField { Name = name.Groups[1].Value, Type = type, Words = words * count, Offset = offset };
                        fields.Add(field);
                        offset += field.Words;
                    }
                }
                structs[m.Groups[3].Value] = fields;
            }
            return structs;
        }

        private static int Words(string type, Dictionary<string, List<CField>> structs)
        {
            int words = 0;
            foreach (CField f in structs[type]) { words += f.Words; }
            return words;
        }

        // One entry per 4-byte word: "int", "uint" or "float".
        private static List<string> Flatten(string type, Dictionary<string, List<CField>> structs)
        {
            List<string> words = new List<string>();
            foreach (CField f in structs[type])
            {
                if (f.Type == "int32_t" || f.Type == "uint32_t" || f.Type == "float")
                {
                    string kind = f.Type == "int32_t" ? "int" : f.Type == "uint32_t" ? "uint" : "float";
                    for (int i = 0; i < f.Words; i++) { words.Add(kind); }
                    continue;
                }
                int each = Words(f.Type, structs);
                for (int i = 0; i < f.Words / each; i++) { words.AddRange(Flatten(f.Type, structs)); }
            }
            return words;
        }

        private static List<string> FlattenManaged(Type type)
        {
            List<string> words = new List<string>();
            List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            fields.Sort((a, b) => Marshal.OffsetOf(type, a.Name).ToInt32().CompareTo(Marshal.OffsetOf(type, b.Name).ToInt32()));
            foreach (FieldInfo f in fields)
            {
                FixedBufferAttribute buffer = (FixedBufferAttribute)Attribute.GetCustomAttribute(f, typeof(FixedBufferAttribute));
                if (buffer != null)
                {
                    for (int i = 0; i < buffer.Length; i++) { words.Add(Kind(buffer.ElementType)); }
                    continue;
                }
                if (f.FieldType == typeof(int) || f.FieldType == typeof(uint) || f.FieldType == typeof(float)) { words.Add(Kind(f.FieldType)); continue; }
                words.AddRange(FlattenManaged(f.FieldType));
            }
            return words;
        }

        private static string Kind(Type t) { return t == typeof(int) ? "int" : t == typeof(uint) ? "uint" : t == typeof(float) ? "float" : t.Name; }

        private static int FirstDifference(List<string> a, List<string> b)
        {
            for (int i = 0; i < Math.Max(a.Count, b.Count); i++) { if (i >= a.Count || i >= b.Count || a[i] != b[i]) { return i; } }
            return -1;
        }

        private static string At(List<string> words, int i) { return i < words.Count ? words[i] : "(end)"; }

        // ---- constants and enums ----

        private static void FlagChecks(Checker check, Dictionary<string, long> d)
        {
            Pairs(check, d, "LC_VALID_", new Dictionary<string, uint> { { "WORLD", CoreAbi.ValidWorld }, { "PLAYER", CoreAbi.ValidPlayer }, { "PEDS", CoreAbi.ValidPeds },
                { "VEHICLES", CoreAbi.ValidVehicles }, { "BULLETS", CoreAbi.ValidBullets }, { "DAMAGE", CoreAbi.ValidDamage } });
            Pairs(check, d, "LC_PED_", new Dictionary<string, uint> { { "DEAD", CoreAbi.PedDead }, { "IN_VEHICLE", CoreAbi.PedInVehicle }, { "PLAYER", CoreAbi.PedPlayer }, { "NEW", CoreAbi.PedNew } });
            Pairs(check, d, "LC_VEH_", new Dictionary<string, uint> { { "NEW", CoreAbi.VehicleNew }, { "PLAYER", CoreAbi.VehiclePlayer } });
            Pairs(check, d, "LC_PLAYER_", new Dictionary<string, uint> { { "PLAYING", CoreAbi.PlayerPlaying }, { "CONTROL", CoreAbi.PlayerControl }, { "DEAD", CoreAbi.PlayerDead },
                { "IN_VEHICLE", CoreAbi.PlayerInVehicle }, { "RELOADING", CoreAbi.PlayerReloading } });
            Pairs(check, d, "LC_WORLD_", new Dictionary<string, uint> { { "PAUSED", CoreAbi.WorldPaused }, { "FADED_OUT", CoreAbi.WorldFadedOut } });
            Pairs(check, d, "LC_DAMAGE_", new Dictionary<string, uint> { { "KILLED", CoreAbi.DamageKilled } });
            Pairs(check, d, "LC_RAY_ACCEPT_", new Dictionary<string, uint> { { "WORLD", LcRay.AcceptWorld }, { "PEDS", LcRay.AcceptPeds }, { "VEHICLES", LcRay.AcceptVehicles },
                { "OBJECTS", LcRay.AcceptObjects }, { "ALL", LcRay.AcceptAll } });
            Pairs(check, d, "LC_RAY_", new Dictionary<string, uint> { { "INCLUDE_ALL", LcRay.IncludeAll }, { "RESEARCH", LcRay.FlagResearch },
                { "CLEAR", unchecked((uint)LcRay.Clear) }, { "HIT", unchecked((uint)LcRay.Hit) }, { "UNAVAILABLE", unchecked((uint)LcRay.Unavailable) }, { "INCONCLUSIVE", unchecked((uint)LcRay.Inconclusive) } });
            Pairs(check, d, "LC_ENTITY_", new Dictionary<string, uint> { { "NONE", (uint)LcRay.EntityNone }, { "PED", (uint)LcRay.EntityPed }, { "VEHICLE", (uint)LcRay.EntityVehicle }, { "OBJECT", (uint)LcRay.EntityObject } });
        }

        private static void Pairs(Checker check, Dictionary<string, long> defines, string prefix, Dictionary<string, uint> mirror)
        {
            List<string> wrong = new List<string>();
            foreach (KeyValuePair<string, uint> pair in mirror)
            {
                long value;
                if (!defines.TryGetValue(prefix + pair.Key, out value) || unchecked((uint)value) != pair.Value) { wrong.Add(prefix + pair.Key); }
            }
            check.True("ABI: " + prefix + "* constants match CoreAbi", wrong.Count == 0, wrong.Count == 0 ? mirror.Count + " values" : string.Join(",", wrong.ToArray()));
        }

        private static List<string> EnumNames(string header, string name)
        {
            Match m = Regex.Match(header, @"enum\s+" + name + @"\s*\{([^}]*)\}");
            List<string> names = new List<string>();
            foreach (string part in m.Groups[1].Value.Split(','))
            {
                string entry = part.Trim();
                if (entry.Length > 0) { names.Add(entry); }
            }
            return names;
        }

        private static void EnumChecks(string header, Checker check)
        {
            List<string> natives = EnumNames(header, "lc_native_id");
            natives.Remove("LC_N_COUNT");
            List<string> expected = new List<string>();
            foreach (string n in CoreAbi.NativeNames) { expected.Add("LC_N_" + n); }
            check.True("ABI: lc_native_id order matches CoreAbi.NativeNames", string.Join(",", natives.ToArray()) == string.Join(",", expected.ToArray()),
                natives.Count + " header vs " + expected.Count + " C#");
            check.Equal("ABI: NativeCount", natives.Count, CoreAbi.NativeCount);
            List<string> wrongIds = new List<string>();
            foreach (FieldInfo f in typeof(CoreAbi).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (!f.IsLiteral || f.FieldType != typeof(int) || f.Name.StartsWith("Ev") || f.Name.StartsWith("Max") || f.Name == "NativeCount") { continue; }
                int index = natives.IndexOf("LC_N_" + Upper(f.Name));
                if (index != (int)f.GetRawConstantValue()) { wrongIds.Add(f.Name + "=" + f.GetRawConstantValue() + " header=" + index); }
            }
            check.True("ABI: CoreAbi native ids match lc_native_id", wrongIds.Count == 0, string.Join(",", wrongIds.ToArray()));

            List<string> events = EnumNames(header, "lc_event_type");
            List<string> wrongEvents = new List<string>();
            int mirrored = 0;
            foreach (FieldInfo f in typeof(CoreAbi).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (!f.IsLiteral || !f.Name.StartsWith("Ev")) { continue; }
                mirrored++;
                int index = events.IndexOf("LC_EV_" + Upper(f.Name.Substring(2)));
                if (index != (int)f.GetRawConstantValue()) { wrongEvents.Add(f.Name + "=" + f.GetRawConstantValue() + " header=" + index); }
            }
            check.True("ABI: CoreAbi event types match lc_event_type", wrongEvents.Count == 0 && mirrored == events.Count - 1,
                wrongEvents.Count > 0 ? string.Join(",", wrongEvents.ToArray()) : mirrored + " mirrored of " + (events.Count - 1));
        }

        // GetCharHealth -> GET_CHAR_HEALTH; IsCharInAnyCar -> IS_CHAR_IN_ANY_CAR.
        private static string Upper(string camel) { return Regex.Replace(camel, "(?<=[a-z0-9])([A-Z])", "_$1").ToUpperInvariant(); }

        // ---- the snapshot's variable part, read through CoreBridge's own accessors ----

        private static void SnapshotChecks(Dictionary<string, List<CField>> structs, Checker check)
        {
            Dictionary<string, int> offsets = new Dictionary<string, int>();
            int total = 0;
            foreach (CField f in structs["lc_snapshot"]) { offsets[f.Name] = f.Offset * 4; total += f.Words * 4; }
            check.Equal("ABI: CoreBridge.ExpectedSnapshotBytes matches sizeof(lc_snapshot)", total, CoreBridge.ExpectedSnapshotBytes);
            check.Equal("ABI: LcSnapshotHead ends where peds begin", offsets["peds"], sizeof(LcSnapshotHead));

            byte[] buffer = new byte[total];
            fixed (byte* bytes = buffer)
            {
                LcSnapshotHead* head = (LcSnapshotHead*)bytes;
                *(int*)(bytes + offsets["vehicle_count"]) = 11;
                *(int*)(bytes + offsets["bullet_count"]) = 22;
                *(int*)(bytes + offsets["damage_count"]) = 33;
                *(int*)(bytes + offsets["event_count"]) = 44;
                *(int*)(bytes + offsets["events_dropped"]) = 55;
                check.True("ABI: snapshot counts read at the header's offsets",
                    CoreBridge.VehicleCount(head) == 11 && CoreBridge.BulletCount(head) == 22 && CoreBridge.DamageCount(head) == 33 &&
                    CoreBridge.EventCount(head) == 44 && CoreBridge.EventsDropped(head) == 55,
                    "vehicles=" + CoreBridge.VehicleCount(head) + " bullets=" + CoreBridge.BulletCount(head) + " damages=" + CoreBridge.DamageCount(head) +
                    " events=" + CoreBridge.EventCount(head) + " dropped=" + CoreBridge.EventsDropped(head));
                check.True("ABI: snapshot arrays start at the header's offsets",
                    (byte*)CoreBridge.Peds(head) - bytes == offsets["peds"] && (byte*)CoreBridge.Vehicles(head) - bytes == offsets["vehicles"] &&
                    (byte*)CoreBridge.Bullets(head) - bytes == offsets["bullets"] && (byte*)CoreBridge.Damages(head) - bytes == offsets["damages"] &&
                    (byte*)CoreBridge.Events(head) - bytes == offsets["events"], "");
            }
        }
    }
}
