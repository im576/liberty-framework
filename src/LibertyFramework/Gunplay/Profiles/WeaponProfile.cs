using System.Runtime.Serialization;

// Fields are populated by DataContractJsonSerializer.
#pragma warning disable 0649

namespace LibertyFramework.Gunplay.Profiles
{
    [DataContract]
    internal sealed class WeaponProfile
    {
        [DataMember(Name = "weaponId", IsRequired = true, Order = 0)] internal int WeaponId;
        [DataMember(Name = "vanillaWeaponId", IsRequired = true, Order = 1)] internal int VanillaWeaponId;
        [DataMember(Name = "weaponInfoName", IsRequired = true, Order = 2)] internal string WeaponInfoName;
        [DataMember(Name = "label", IsRequired = true, Order = 3)] internal string Label;
        [DataMember(Name = "profileName", IsRequired = true, Order = 4)] internal string ProfileName;
        [DataMember(Name = "finish", IsRequired = true, Order = 5)] internal string Finish;
        [DataMember(Name = "initialAmmo", IsRequired = true, Order = 6)] internal int InitialAmmo;
        [DataMember(Name = "calibrationSource", IsRequired = true, Order = 7)] internal bool CalibrationSource;
        [DataMember(Name = "recoil", IsRequired = true, Order = 8)] internal RecoilProfile Recoil;
        [DataMember(Name = "spread", IsRequired = true, Order = 9)] internal SpreadProfile Spread;
    }
}
