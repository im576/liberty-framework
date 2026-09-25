using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#pragma warning disable 0649
namespace LibertyFramework.Weapons.Logic
{
    [DataContract]
    internal sealed class WeaponCatalog
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "entries", IsRequired = true)] internal List<WeaponCatalogEntry> Entries;

        internal WeaponCatalogEntry Find(int weaponId)
        {
            foreach (WeaponCatalogEntry entry in Entries) { if (entry.WeaponId == weaponId) { return entry; } }
            return null;
        }

        internal void Validate()
        {
            if (SchemaVersion != 1 || Entries == null) { throw new InvalidOperationException("Invalid weapon catalog schema."); }
            Dictionary<int, bool> ids = new Dictionary<int, bool>();
            foreach (WeaponCatalogEntry entry in Entries)
            {
                if (entry.WeaponId <= 0 || string.IsNullOrEmpty(entry.Id) || string.IsNullOrEmpty(entry.Family) ||
                    entry.Finishes == null || entry.Finishes.Count == 0 || entry.Attachments == null || ids.ContainsKey(entry.WeaponId))
                    { throw new InvalidOperationException("Invalid or duplicate weapon catalog entry."); }
                ids.Add(entry.WeaponId, true);
            }
        }
    }

}
