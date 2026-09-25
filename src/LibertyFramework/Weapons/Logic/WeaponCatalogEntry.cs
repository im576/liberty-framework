using System.Collections.Generic;
using System.Runtime.Serialization;

#pragma warning disable 0649
namespace LibertyFramework.Weapons.Logic
{
    [DataContract]
    internal sealed class WeaponCatalogEntry
    {
        [DataMember(Name = "id", IsRequired = true)] internal string Id;
        [DataMember(Name = "weaponId", IsRequired = true)] internal int WeaponId;
        [DataMember(Name = "family", IsRequired = true)] internal string Family;
        [DataMember(Name = "label", IsRequired = true)] internal string Label;
        [DataMember(Name = "role", IsRequired = true)] internal string Role;
        [DataMember(Name = "model", IsRequired = true)] internal string Model;
        [DataMember(Name = "finishes", IsRequired = true)] internal List<string> Finishes;
        [DataMember(Name = "attachments", IsRequired = true)] internal List<string> Attachments;
    }
}
