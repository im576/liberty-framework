using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#pragma warning disable 0649

namespace LibertyFramework.Engine
{
    // config/engine.json (CONFIG_SCHEMA.md).
    [DataContract]
    public sealed class EngineConfig
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] public int SchemaVersion;
        [DataMember(Name = "coreEnabled", IsRequired = true)] public bool CoreEnabled;
        [DataMember(Name = "pedRadiusMeters", IsRequired = true)] public float PedRadiusMeters;
        [DataMember(Name = "coreSpotCheckFrames", IsRequired = true)] public int CoreSpotCheckFrames;
        [DataMember(Name = "coreSpotCheckToleranceMeters", IsRequired = true)] public float CoreSpotCheckToleranceMeters;
        [DataMember(Name = "disabledModules", IsRequired = false)] public List<string> DisabledModules;
        [DataMember(Name = "loadModAssemblies", IsRequired = false)] public bool LoadModAssemblies;

        internal void Validate()
        {
            if (SchemaVersion != 1) { throw new InvalidDataException("engine schemaVersion must be 1"); }
            if (!(PedRadiusMeters >= 10 && PedRadiusMeters <= 500)) { throw new InvalidDataException("engine pedRadiusMeters must be 10-500"); }
            if (CoreSpotCheckFrames < 30) { throw new InvalidDataException("engine coreSpotCheckFrames must be >= 30"); }
            if (!(CoreSpotCheckToleranceMeters > 0 && CoreSpotCheckToleranceMeters < 5)) { throw new InvalidDataException("engine coreSpotCheckToleranceMeters must be 0-5"); }
            if (DisabledModules == null) { DisabledModules = new List<string>(); }
        }

        internal static EngineConfig Defaults()
        {
            EngineConfig config = new EngineConfig();
            config.SchemaVersion = 1; config.CoreEnabled = true; config.PedRadiusMeters = 120; config.CoreSpotCheckFrames = 300;
            config.CoreSpotCheckToleranceMeters = 0.05f; config.DisabledModules = new List<string>();
            return config;
        }
    }
}