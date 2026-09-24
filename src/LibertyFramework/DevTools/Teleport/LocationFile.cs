using System.Collections.Generic;
using System.Runtime.Serialization;

// Fields are populated by DataContractJsonSerializer.
#pragma warning disable 0649

namespace LibertyFramework.DevTools.Teleport
{
    // config/devtools/locations.json
    [DataContract]
    internal sealed class LocationFile
    {
        [DataMember(Name = "schemaVersion", IsRequired = true, Order = 0)] internal int SchemaVersion;
        [DataMember(Name = "locations", IsRequired = true, Order = 1)] internal List<TeleportLocation> Locations;
    }
}
