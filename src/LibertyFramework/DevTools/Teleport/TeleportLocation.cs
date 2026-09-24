using System.Runtime.Serialization;

// Fields are populated by DataContractJsonSerializer.
#pragma warning disable 0649

namespace LibertyFramework.DevTools.Teleport
{
    [DataContract]
    internal sealed class TeleportLocation
    {
        [DataMember(Name = "id", IsRequired = true, Order = 0)] internal string Id;
        [DataMember(Name = "name", IsRequired = true, Order = 1)] internal string Name;
        [DataMember(Name = "x", IsRequired = true, Order = 2)] internal float X;
        [DataMember(Name = "y", IsRequired = true, Order = 3)] internal float Y;
        [DataMember(Name = "z", IsRequired = true, Order = 4)] internal float Z;
        [DataMember(Name = "heading", IsRequired = true, Order = 5)] internal float Heading;
        // "none" = use x/y/z exactly; "pavement" = nearest pavement node; "ground" = ground below z.
        [DataMember(Name = "snap", IsRequired = true, Order = 6)] internal string Snap;
        [DataMember(Name = "note", IsRequired = false, Order = 7)] internal string Note;
    }
}
