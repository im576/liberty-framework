using System.Runtime.Serialization;

namespace LibertyFramework.Core.Config
{
    [DataContract]
    internal sealed class ProbeConfig
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)]
        public int SchemaVersion { get; set; }

        [DataMember(Name = "probeLabel", IsRequired = true)]
        public string ProbeLabel { get; set; }
    }
}
