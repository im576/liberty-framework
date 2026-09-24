using System;
using System.Collections.Generic;
using LibertyFramework.Arsenal.Contracts;

namespace LibertyFramework.Arsenal.Logic
{
    internal static class ArsenalPolicy
    {
        internal static bool MayMoveWeapons(bool mission, bool cutsceneOrFade)
        {
            return !mission && !cutsceneOrFade;
        }
        internal static string Group(ArsenalConfig config, WeaponCategory category)
        {
            foreach (CategoryRule rule in config.Categories) { if (rule.Category == category) { return rule.Group; } }
            return "uncounted";
        }

        internal static BodySlot Slot(ArsenalConfig config, WeaponCategory category)
        {
            foreach (CategoryRule rule in config.Categories) { if (rule.Category == category) { return rule.BodySlot; } }
            return BodySlot.None;
        }

        internal static int OverflowIndex(ArsenalConfig config, IList<WeaponRecord> carried, IDictionary<int, long> lastUsed)
        {
            foreach (string group in new string[] { "sidearm", "longGun", "melee" })
            {
                int count = 0;
                int oldest = -1;
                long age = long.MaxValue;
                for (int index = 0; index < carried.Count; index++)
                {
                    if (Group(config, carried[index].Category) != group) { continue; }
                    count++;
                    long used;
                    if (!lastUsed.TryGetValue(carried[index].WeaponId, out used)) { used = 0; }
                    if (oldest < 0 || used < age) { oldest = index; age = used; }
                }
                int limit = group == "sidearm" ? config.SidearmLimit : group == "longGun" ? config.LongGunLimit : config.MeleeLimit;
                if (count > limit) { return oldest; }
            }
            return -1;
        }

        internal static bool IsOwnedGain(bool fromStorage, bool mission, long gainMilliseconds, long lastMoneyDecreaseMilliseconds, int windowMilliseconds)
        {
            return fromStorage || (!mission && lastMoneyDecreaseMilliseconds >= 0 && gainMilliseconds >= lastMoneyDecreaseMilliseconds &&
                gainMilliseconds - lastMoneyDecreaseMilliseconds <= windowMilliseconds);
        }

        internal static void ResolveLoss(IList<WeaponRecord> snapshot, bool busted, StorageBin lastSafehouse)
        {
            if (!busted && lastSafehouse != null)
            {
                foreach (WeaponRecord record in snapshot) { if (record.Owned) { lastSafehouse.Weapons.Add(record.Clone()); } }
            }
            snapshot.Clear();
        }

        internal static StorageBin FindOrAdd(List<StorageBin> bins, string id)
        {
            foreach (StorageBin bin in bins) { if (bin.Id == id) { return bin; } }
            StorageBin added = new StorageBin(); added.Id = id; bins.Add(added); return added;
        }
    }
}
