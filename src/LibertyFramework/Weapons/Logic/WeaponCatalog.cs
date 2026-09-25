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
        [DataMember(Name = "attachmentOptions", IsRequired = false)] internal List<AttachmentOption> AttachmentOptions;

        internal WeaponCatalogEntry Find(int weaponId)
        {
            foreach (WeaponCatalogEntry entry in Entries) { if (entry.WeaponId == weaponId) { return entry; } }
            return null;
        }

        internal AttachmentOption FindAttachment(string id)
        {
            if (AttachmentOptions == null) { return null; }
            foreach (AttachmentOption option in AttachmentOptions) { if (option.Id == id) { return option; } }
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
            if (AttachmentOptions != null)
            {
                Dictionary<string, bool> optionIds = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                foreach (AttachmentOption option in AttachmentOptions)
                {
                    if (option == null || string.IsNullOrEmpty(option.Id) || string.IsNullOrEmpty(option.Label) ||
                        option.Price < 0 || option.PerShotBloomMultiplier <= 0 || option.PerShotBloomMultiplier > 1 ||
                        optionIds.ContainsKey(option.Id)) { throw new InvalidOperationException("Invalid attachment option."); }
                    optionIds.Add(option.Id, true);
                }
                foreach (WeaponCatalogEntry entry in Entries)
                    foreach (string id in entry.Attachments)
                        if (!optionIds.ContainsKey(id)) { throw new InvalidOperationException("Unknown attachment option " + id); }
            }
            else
            {
                foreach (WeaponCatalogEntry entry in Entries)
                    if (entry.Attachments.Count > 0) { throw new InvalidOperationException("Attachment definitions are missing."); }
            }
        }
    }

}
