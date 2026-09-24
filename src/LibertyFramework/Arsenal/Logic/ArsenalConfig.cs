using System.Collections.Generic;
using System.Runtime.Serialization;
using LibertyFramework.Arsenal.Contracts;

#pragma warning disable 0649
namespace LibertyFramework.Arsenal.Logic
{
    [DataContract]
    internal sealed class ArsenalConfig
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "sidearmLimit", IsRequired = true)] internal int SidearmLimit;
        [DataMember(Name = "longGunLimit", IsRequired = true)] internal int LongGunLimit;
        [DataMember(Name = "meleeLimit", IsRequired = true)] internal int MeleeLimit;
        [DataMember(Name = "purchaseWindowMilliseconds", IsRequired = true)] internal int PurchaseWindowMilliseconds;
        [DataMember(Name = "trunkDistanceMeters", IsRequired = true)] internal float TrunkDistanceMeters;
        [DataMember(Name = "trunkRearOffsetMeters", IsRequired = true)] internal float TrunkRearOffsetMeters;
        [DataMember(Name = "ownedVehicleMatchMeters", IsRequired = true)] internal float OwnedVehicleMatchMeters;
        [DataMember(Name = "fallbackVehicleMatchMeters", IsRequired = true)] internal float FallbackVehicleMatchMeters;
        [DataMember(Name = "categories", IsRequired = true)] internal List<CategoryRule> Categories;
        [DataMember(Name = "safehouses", IsRequired = true)] internal List<SafehouseRule> Safehouses;
    }

}
