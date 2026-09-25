using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using LibertyFramework.Arsenal.Contracts;

#pragma warning disable 0649

namespace LibertyFramework.Arsenal.Holsters.Logic
{
    [DataContract]
    internal sealed class HolsterConfig
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "enabled", IsRequired = true)] internal bool Enabled;
        [DataMember(Name = "showOnBikes", IsRequired = true)] internal bool ShowOnBikes;
        [DataMember(Name = "nudgePositionMeters", IsRequired = true)] internal float NudgePositionMeters;
        [DataMember(Name = "nudgeRotationDegrees", IsRequired = true)] internal float NudgeRotationDegrees;
        [DataMember(Name = "weapons", IsRequired = true)] internal List<HolsterWeapon> Weapons;
        [DataMember(Name = "placements", IsRequired = true)] internal List<HolsterPlacement> Placements;
        [DataMember(Name = "slings", IsRequired = false)] internal List<HolsterSling> Slings;

        internal void Validate()
        {
            if (SchemaVersion != 1 || Weapons == null || Placements == null) { throw new InvalidDataException("holsters schemaVersion must be 1 with weapons and placements"); }
            if (!(NudgePositionMeters > 0) || float.IsInfinity(NudgePositionMeters) ||
                !(NudgeRotationDegrees > 0) || float.IsInfinity(NudgeRotationDegrees))
            { throw new InvalidDataException("holsters nudge steps must be finite and positive"); }
            HashSet<int> ids = new HashSet<int>();
            foreach (HolsterWeapon weapon in Weapons)
            {
                if (weapon == null || weapon.WeaponId <= 0 || string.IsNullOrEmpty(weapon.WeaponInfoType) ||
                    !Enum.IsDefined(typeof(WeaponCategory), weapon.Category) || !ids.Add(weapon.WeaponId))
                { throw new InvalidDataException("holsters weapons has invalid or duplicate weaponId"); }
            }
            HashSet<BodySlot> slots = new HashSet<BodySlot>();
            foreach (HolsterPlacement placement in Placements)
            {
                BodySlot slot;
                if (placement == null || !Enum.TryParse<BodySlot>(placement.Slot, out slot) || slot == BodySlot.None ||
                    string.IsNullOrEmpty(placement.Bone) ||
                    placement.Position == null || placement.Position.Length != 3 || placement.Rotation == null || placement.Rotation.Length != 3)
                { throw new InvalidDataException("holsters placement slot/bone/vector invalid"); }
                WeaponCategory category;
                if (!string.IsNullOrEmpty(placement.Category) &&
                    (!Enum.TryParse<WeaponCategory>(placement.Category, out category) || !Enum.IsDefined(typeof(WeaponCategory), category)))
                { throw new InvalidDataException("holsters placement category invalid"); }
                foreach (float value in placement.Position) { if (float.IsNaN(value) || float.IsInfinity(value)) { throw new InvalidDataException("holsters position not finite"); } }
                foreach (float value in placement.Rotation) { if (float.IsNaN(value) || float.IsInfinity(value)) { throw new InvalidDataException("holsters rotation not finite"); } }
                if (string.IsNullOrEmpty(placement.Category) && string.IsNullOrEmpty(placement.Model)) { slots.Add(slot); }
            }
            for (BodySlot slot = BodySlot.SidearmPrimary; slot <= BodySlot.Melee; slot++)
            { if (!slots.Contains(slot)) { throw new InvalidDataException("holsters missing placement for " + slot); } }
            if (Slings != null)
            {
                HashSet<BodySlot> slung = new HashSet<BodySlot>();
                foreach (HolsterSling sling in Slings)
                {
                    BodySlot slot;
                    if (sling == null || !Enum.TryParse<BodySlot>(sling.Slot, out slot) || (slot != BodySlot.LongGun1 && slot != BodySlot.LongGun2) ||
                        !slung.Add(slot) || string.IsNullOrEmpty(sling.Model) || string.IsNullOrEmpty(sling.Bone))
                    { throw new InvalidDataException("holsters sling needs a unique long-gun slot, model and bone"); }
                }
            }
        }

        internal HolsterSling FindSling(BodySlot slot)
        {
            if (Slings == null) { return null; }
            foreach (HolsterSling sling in Slings) { if (sling.Slot == slot.ToString()) { return sling; } }
            return null;
        }

        internal HolsterWeapon FindWeapon(int weaponId)
        {
            foreach (HolsterWeapon weapon in Weapons) { if (weapon.WeaponId == weaponId) { return weapon; } }
            return null;
        }

        internal HolsterPlacement FindPlacement(BodySlot slot, WeaponCategory category, string model)
        {
            HolsterPlacement selected = null;
            int best = -1;
            foreach (HolsterPlacement candidate in Placements)
            {
                if (candidate.Slot != slot.ToString()) { continue; }
                if (!string.IsNullOrEmpty(candidate.Model) && !string.Equals(candidate.Model, model, StringComparison.OrdinalIgnoreCase)) { continue; }
                if (!string.IsNullOrEmpty(candidate.Category) && candidate.Category != category.ToString()) { continue; }
                int score = (string.IsNullOrEmpty(candidate.Model) ? 0 : 2) + (string.IsNullOrEmpty(candidate.Category) ? 0 : 1);
                if (score > best) { selected = candidate; best = score; }
            }
            return selected;
        }
    }
}
