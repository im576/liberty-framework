using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

#pragma warning disable 0649

namespace LibertyFramework.Engine
{
    // config/engine.json (CONFIG_SCHEMA.md). Fields added after schema 1 shipped are optional; absent means the default.
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
        [DataMember(Name = "vehicleRadiusMeters", IsRequired = false)] public float VehicleRadiusMeters;
        [DataMember(Name = "bulletEvents", IsRequired = false)] private bool? bulletEvents;
        [DataMember(Name = "governorEnabled", IsRequired = false)] private bool? governorEnabled;
        [DataMember(Name = "moduleBudgetMs", IsRequired = false)] public float ModuleBudgetMs;
        [DataMember(Name = "throttledIntervalMs", IsRequired = false)] public int ThrottledIntervalMs;
        [DataMember(Name = "targetFrameMs", IsRequired = false)] public float TargetFrameMs;
        [DataMember(Name = "lowAddressSpaceMegabytes", IsRequired = false)] public int LowAddressSpaceMegabytes;
        [DataMember(Name = "watchdogStallMilliseconds", IsRequired = false)] public int WatchdogStallMilliseconds;
        [DataMember(Name = "adaptiveDensityFloor", IsRequired = false)] public float AdaptiveDensityFloor;

        public bool BulletEvents { get { return bulletEvents ?? true; } }
        public bool GovernorEnabled { get { return governorEnabled ?? true; } }

        internal void Validate()
        {
            if (SchemaVersion != 1) { throw new InvalidDataException("engine schemaVersion must be 1"); }
            if (!(PedRadiusMeters >= 10 && PedRadiusMeters <= 500)) { throw new InvalidDataException("engine pedRadiusMeters must be 10-500"); }
            if (CoreSpotCheckFrames < 30) { throw new InvalidDataException("engine coreSpotCheckFrames must be >= 30"); }
            if (!(CoreSpotCheckToleranceMeters > 0 && CoreSpotCheckToleranceMeters < 5)) { throw new InvalidDataException("engine coreSpotCheckToleranceMeters must be 0-5"); }
            if (DisabledModules == null) { DisabledModules = new List<string>(); }
            if (VehicleRadiusMeters == 0) { VehicleRadiusMeters = 150; }
            if (!(VehicleRadiusMeters >= 10 && VehicleRadiusMeters <= 500)) { throw new InvalidDataException("engine vehicleRadiusMeters must be 10-500"); }
            if (ModuleBudgetMs == 0) { ModuleBudgetMs = 2f; }
            if (!(ModuleBudgetMs >= 0.1f && ModuleBudgetMs <= 50)) { throw new InvalidDataException("engine moduleBudgetMs must be 0.1-50"); }
            if (ThrottledIntervalMs == 0) { ThrottledIntervalMs = 100; }
            if (ThrottledIntervalMs < 10 || ThrottledIntervalMs > 2000) { throw new InvalidDataException("engine throttledIntervalMs must be 10-2000"); }
            if (TargetFrameMs == 0) { TargetFrameMs = 33.3f; }
            if (!(TargetFrameMs >= 8 && TargetFrameMs <= 100)) { throw new InvalidDataException("engine targetFrameMs must be 8-100"); }
            if (LowAddressSpaceMegabytes == 0) { LowAddressSpaceMegabytes = 600; }
            if (LowAddressSpaceMegabytes < 100 || LowAddressSpaceMegabytes > 2000) { throw new InvalidDataException("engine lowAddressSpaceMegabytes must be 100-2000"); }
            if (WatchdogStallMilliseconds == 0) { WatchdogStallMilliseconds = 5000; }
            if (WatchdogStallMilliseconds < 1000 || WatchdogStallMilliseconds > 60000) { throw new InvalidDataException("engine watchdogStallMilliseconds must be 1000-60000"); }
            if (!(AdaptiveDensityFloor >= 0 && AdaptiveDensityFloor <= 1)) { throw new InvalidDataException("engine adaptiveDensityFloor must be 0-1"); }
        }

        internal static EngineConfig Defaults()
        {
            EngineConfig config = new EngineConfig();
            config.SchemaVersion = 1; config.CoreEnabled = true; config.PedRadiusMeters = 120; config.CoreSpotCheckFrames = 300;
            config.CoreSpotCheckToleranceMeters = 0.05f; config.DisabledModules = new List<string>();
            config.Validate();
            return config;
        }
    }
}
