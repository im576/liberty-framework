using System;
using System.Collections.Generic;
using System.IO;
using LibertyFramework.Arsenal.Contracts;

namespace LibertyFramework.Arsenal.Logic
{
    internal static class ArsenalConfigValidator
    {
        internal static void Validate(ArsenalConfig config)
        {
            if (config == null || config.SchemaVersion != 1 || config.SidearmLimit < 1 || config.SidearmLimit > 2 ||
                config.LongGunLimit < 1 || config.LongGunLimit > 4 || config.MeleeLimit != 1 ||
                config.PurchaseWindowMilliseconds < 0 || config.PurchaseWindowMilliseconds > 10000 ||
                config.TrunkDistanceMeters <= 0 || config.TrunkRearOffsetMeters <= 0 || config.OwnedVehicleMatchMeters <= 0 || config.FallbackVehicleMatchMeters <= 0 ||
                config.Categories == null || config.Safehouses == null) { throw new InvalidDataException("Invalid Arsenal limits or distances."); }
            HashSet<WeaponCategory> seen = new HashSet<WeaponCategory>();
            foreach (CategoryRule rule in config.Categories)
            {
                if (rule == null || !seen.Add(rule.Category) || (rule.Group != "sidearm" && rule.Group != "longGun" && rule.Group != "melee" && rule.Group != "uncounted"))
                    { throw new InvalidDataException("Invalid Arsenal category mapping."); }
                bool valid = (rule.Category == WeaponCategory.Melee && rule.Group == "melee" && rule.BodySlot == BodySlot.Melee) ||
                    ((rule.Category == WeaponCategory.Handgun || rule.Category == WeaponCategory.SMG) && rule.Group == "sidearm" &&
                        (rule.BodySlot == BodySlot.SidearmPrimary || rule.BodySlot == BodySlot.SidearmSecondary)) ||
                    ((rule.Category == WeaponCategory.Shotgun || rule.Category == WeaponCategory.Rifle || rule.Category == WeaponCategory.Sniper ||
                        rule.Category == WeaponCategory.Heavy) && rule.Group == "longGun" &&
                        (rule.BodySlot == BodySlot.LongGun1 || rule.BodySlot == BodySlot.LongGun2)) ||
                    (rule.Category == WeaponCategory.Thrown && rule.Group == "uncounted" && rule.BodySlot == BodySlot.None);
                if (!valid) { throw new InvalidDataException("Category mapped to wrong Arsenal group or body slot: " + rule.Category); }
            }
            foreach (WeaponCategory category in new WeaponCategory[] { WeaponCategory.Melee, WeaponCategory.Handgun, WeaponCategory.SMG,
                WeaponCategory.Shotgun, WeaponCategory.Rifle, WeaponCategory.Sniper, WeaponCategory.Heavy, WeaponCategory.Thrown })
                { if (!seen.Contains(category)) { throw new InvalidDataException("Missing Arsenal category " + category); } }
            HashSet<string> ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (SafehouseRule house in config.Safehouses)
            {
                if (house == null || string.IsNullOrWhiteSpace(house.Id) || !ids.Add(house.Id) || string.IsNullOrWhiteSpace(house.Name) ||
                    string.IsNullOrWhiteSpace(house.Episode) || house.Radius <= 0 || float.IsNaN(house.X) || float.IsNaN(house.Y) || float.IsNaN(house.Z))
                    { throw new InvalidDataException("Invalid Arsenal safehouse."); }
            }
        }
    }
}
