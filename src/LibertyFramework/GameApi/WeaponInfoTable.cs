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
            if (Validated) { RuntimeLog.Info("weaponinfo_validated " + string.Join(" ", detail.ToArray())); }
            else { RuntimeLog.Error("weaponinfo_validation_failed spread writes disabled " + string.Join(" ", detail.ToArray())); }
            return Validated;
        }

        internal void WriteAccuracy(int weaponId, float accuracy)
        {
            if (!Validated) { return; }
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
