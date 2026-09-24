using System.Runtime.Serialization;

#pragma warning disable 0649

namespace LibertyFramework.Arsenal.Holsters.Logic
{
    [DataContract]
    internal sealed class HolsterPlacement
    {
        [DataMember(Name = "slot", IsRequired = true)] internal string Slot;
        [DataMember(Name = "bone", IsRequired = true)] internal string Bone;
        [DataMember(Name = "position", IsRequired = true)] internal float[] Position;
        [DataMember(Name = "rotation", IsRequired = true)] internal float[] Rotation;
        [DataMember(Name = "category", IsRequired = false)] internal string Category;
        [DataMember(Name = "model", IsRequired = false)] internal string Model;
    }
}
