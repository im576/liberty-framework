using System.Runtime.Serialization;

// Fields are populated by DataContractJsonSerializer.
#pragma warning disable 0649

namespace LibertyFramework.Arsenal.Contracts
{
    // One physical weapon the player owns or carries: what moves between body, trunk and stash.
    // Extend with new optional DataMembers (IsRequired = false) only; never rename existing ones.
    [DataContract]
    internal sealed class WeaponRecord
    {
        [DataMember(Name = "weaponId", IsRequired = true, Order = 0)] internal int WeaponId;
        [DataMember(Name = "category", IsRequired = true, Order = 1)] internal WeaponCategory Category;
        [DataMember(Name = "ammo", IsRequired = true, Order = 2)] internal int Ammo;
        [DataMember(Name = "owned", IsRequired = true, Order = 3)] internal bool Owned;
        [DataMember(Name = "finish", IsRequired = false, Order = 4)] internal string Finish;
        [DataMember(Name = "acquiredUtc", IsRequired = false, Order = 5)] internal string AcquiredUtc;

        internal WeaponRecord Clone()
        {
            return (WeaponRecord)MemberwiseClone();
        }
    }
}
