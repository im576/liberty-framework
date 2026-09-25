using System;
using System.Collections.Generic;
using System.IO;
using LibertyFramework.Arsenal.Contracts;
using LibertyFramework.Arsenal.Logic;
using LibertyFramework.Core.Config;
using LibertyFramework.Weapons.Logic;

namespace LibertyFramework.Verify
{
    internal static class Phase2SystemsChecks
    {
        internal static void Run(string root, Checker check)
        {
            WeaponCatalog catalog = JsonStore.Load<WeaponCatalog>(Path.Combine(root, "config/weapon-catalog.json"));
            catalog.Validate();
            check.True("catalog has service pistol replacement pair", catalog.Find(7).Family == catalog.Find(9).Family &&
                catalog.Find(7).Role == "replacement" && catalog.Find(9).Role == "replacement", "");
            check.True("catalog registers existing CE add-on", catalog.Find(59).Role == "add-on" && catalog.Find(59).Model == "lf_gold_carbine", "");

            ArsenalState state = new ArsenalState();
            WeaponRecord weapon = new WeaponRecord(); weapon.WeaponId = 7; weapon.Owned = true; weapon.Finish = "factory";
            weapon.Attachments = new List<string>(); weapon.Attachments.Add("prototype-optic"); weapon.Progression = 3;
            WeaponIdentity.Ensure(weapon);
            string originalId = weapon.InstanceId;
            state.CarriedRecords.Add(weapon);
            string folder = Path.Combine(Path.GetTempPath(), "lf_phase2_verify_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                string path = Path.Combine(folder, "arsenal.json");
                JsonStore.Save(path, state);
                ArsenalState loaded = ArsenalStateStore.LoadOrEmpty(path, error => { throw error; });
                WeaponRecord carried = loaded.CarriedRecords[0];
                check.True("physical weapon metadata survives state round trip", carried.InstanceId == originalId &&
                    carried.Finish == "factory" && carried.Progression == 3 && carried.Attachments[0] == "prototype-optic", "");
                WeaponRecord transferred = carried.Clone(); loaded.CarriedRecords.Clear();
                ArsenalPolicy.FindOrAdd(loaded.VehicleTrunks, "lvs:owned_42").Weapons.Add(transferred);
                JsonStore.Save(path, loaded);
                ArsenalState restored = ArsenalStateStore.LoadOrEmpty(path, error => { throw error; });
                check.True("trunk transfer retains physical identity", restored.CarriedRecords.Count == 0 &&
                    restored.VehicleTrunks[0].Weapons[0].InstanceId == originalId, "");
                WeaponRecord fromTrunk = restored.VehicleTrunks[0].Weapons[0];
                restored.VehicleTrunks[0].Weapons.Clear();
                ArsenalPolicy.FindOrAdd(restored.SafehouseStashes, "home").Weapons.Add(fromTrunk);
                JsonStore.Save(path, restored);
                ArsenalState atHome = ArsenalStateStore.LoadOrEmpty(path, error => { throw error; });
                check.True("safehouse transfer retains physical identity and metadata", atHome.SafehouseStashes[0].Weapons[0].InstanceId == originalId &&
                    atHome.SafehouseStashes[0].Weapons[0].Attachments[0] == "prototype-optic", "");
                restored.VehicleTrunks[0].Weapons.Add(transferred.Clone());
                WeaponIdentity.Normalize(restored);
                check.True("duplicate instance IDs repaired", restored.SafehouseStashes[0].Weapons[0].InstanceId !=
                    restored.VehicleTrunks[0].Weapons[0].InstanceId, "");
            }
            finally { Directory.Delete(folder, true); }
        }
    }
}
