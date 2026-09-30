using System;
using System.Collections.Generic;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.GameApi
{
    // Live access to CWeaponInfo accuracy, the value the game's CWeapon::DoAccuracy uses to
    // offset each bullet's end point. Writing it changes where real bullets go. Only the
    // registered test weapons are ever written, and originals are restored on unload.
    internal sealed class WeaponInfoTable
    {
        private const int MaximumWeaponId = 127;
        private readonly LiveMemory memory;
        private readonly GameAddresses addresses;
        private readonly Dictionary<int, float[]> originals = new Dictionary<int, float[]>();
        // Weapon ids whose entry was proven against WeaponInfo.xml; only these are ever written.
        private readonly HashSet<int> writable = new HashSet<int>();

        internal WeaponInfoTable(LiveMemory memory, GameAddresses addresses)
        {
            this.memory = memory;
            this.addresses = addresses;
        }

        internal bool Validated { get; private set; }

        private uint EntryAddress(int weaponId)
        {
            if (weaponId < 0 || weaponId > MaximumWeaponId) { return 0; }
            if (addresses.WeaponInfoCount > 0 && weaponId >= addresses.WeaponInfoCount) { return 0; }
            uint entry = addresses.WeaponInfoArray + (uint)(weaponId * addresses.WeaponInfoStride);
            return memory.IsReadable(entry, addresses.WeaponInfoStride) ? entry : 0;
        }

        internal bool TryReadAccuracy(int weaponId, out float accuracy)
        {
            accuracy = 0;
            uint entry = EntryAddress(weaponId);
            if (entry == 0) { return false; }
            accuracy = memory.ReadSingle(entry + (uint)addresses.AccuracyOffset);
            return !float.IsNaN(accuracy) && !float.IsInfinity(accuracy);
        }

        // Proves array base, stride and field offset: the values in memory must equal the
        // <aiming accuracy> values parsed from the WeaponInfo.xml the game actually loaded.
        internal bool Validate(IDictionary<int, float> expected)
        {
            int matches = 0;
            List<string> detail = new List<string>();
            foreach (KeyValuePair<int, float> pair in expected)
            {
                float actual;
                bool read = TryReadAccuracy(pair.Key, out actual);
                bool match = read && Math.Abs(actual - pair.Value) < 0.0005f;
                if (match) { matches++; }
                detail.Add(pair.Key + ":" + (read ? actual.ToString("0.####") : "unreadable") + "/" + pair.Value.ToString("0.####"));
            }
            Validated = expected.Count >= 3 && matches == expected.Count;
            if (Validated) { foreach (int id in expected.Keys) { writable.Add(id); } }
            if (Validated) { RuntimeLog.Info("weaponinfo_validated " + string.Join(" ", detail.ToArray())); }
            else { RuntimeLog.Error("weaponinfo_validation_failed spread writes disabled " + string.Join(" ", detail.ToArray())); }
            return Validated;
        }

        // Stage 1 catalog weapons (T-041): each id is proven on its own, so a wrong id can never disable the test weapons.
        // Needs the base validation first (array base, stride and offset). Returns the ids that were accepted.
        internal List<int> ValidateExtra(IDictionary<int, float> expected)
        {
            List<int> accepted = new List<int>();
            if (!Validated) { return accepted; }
            List<string> detail = new List<string>();
            foreach (KeyValuePair<int, float> pair in expected)
            {
                float actual;
                bool match = TryReadAccuracy(pair.Key, out actual) && Math.Abs(actual - pair.Value) < 0.0005f;
                if (match) { writable.Add(pair.Key); accepted.Add(pair.Key); }
                detail.Add(pair.Key + ":" + (match ? "ok" : "mismatch " + actual.ToString("0.####") + "/" + pair.Value.ToString("0.####")));
            }
            RuntimeLog.Info("weaponinfo_stage1_validated " + string.Join(" ", detail.ToArray()));
            return accepted;
        }

        internal bool CanWrite(int weaponId) { return Validated && writable.Contains(weaponId); }

        // Puts one weapon back to the game's own accuracy (a catalog weapon shares its id with every NPC that carries it).
        internal void Restore(int weaponId)
        {
            float[] saved;
            if (!originals.TryGetValue(weaponId, out saved)) { return; }
            uint entry = EntryAddress(weaponId);
            if (entry != 0)
            {
                memory.WriteSingle(entry + (uint)addresses.AccuracyOffset, saved[0]);
                memory.WriteSingle(entry + (uint)addresses.AccuracyAlternateOffset, saved[1]);
                RuntimeLog.Info("weaponinfo_restored id=" + weaponId + " accuracy=" + saved[0]);
            }
            originals.Remove(weaponId);
        }

        internal void WriteAccuracy(int weaponId, float accuracy)
        {
            if (!CanWrite(weaponId)) { return; }
            uint entry = EntryAddress(weaponId);
            if (entry == 0) { return; }
            uint primary = entry + (uint)addresses.AccuracyOffset;
            uint alternate = entry + (uint)addresses.AccuracyAlternateOffset;
            bool usesAlternate = (memory.ReadUInt32(entry + (uint)addresses.AccuracyFlagsOffset) & addresses.AccuracyAlternateFlag) != 0;
            if (!originals.ContainsKey(weaponId))
            {
                originals.Add(weaponId, new[] { memory.ReadSingle(primary), memory.ReadSingle(alternate) });
                RuntimeLog.Info("weaponinfo_original id=" + weaponId + " accuracy=" + originals[weaponId][0] + " alternate_used=" + usesAlternate);
            }
            memory.WriteSingle(primary, accuracy);
            if (usesAlternate) { memory.WriteSingle(alternate, accuracy); }
        }

        internal void RestoreAll()
        {
            foreach (KeyValuePair<int, float[]> pair in originals)
            {
                uint entry = EntryAddress(pair.Key);
                if (entry == 0) { continue; }
                memory.WriteSingle(entry + (uint)addresses.AccuracyOffset, pair.Value[0]);
                memory.WriteSingle(entry + (uint)addresses.AccuracyAlternateOffset, pair.Value[1]);
                RuntimeLog.Info("weaponinfo_restored id=" + pair.Key + " accuracy=" + pair.Value[0]);
            }
            originals.Clear();
        }
    }
}
