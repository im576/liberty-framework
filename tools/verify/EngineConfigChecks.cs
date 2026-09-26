using System;
using System.IO;
using System.Text;
using LibertyFramework.Core.Config;
using LibertyFramework.Engine;

namespace LibertyFramework.Verify
{
    // config/engine.json (CONFIG_SCHEMA.md): the shipped file validates, optional fields default as documented, and the
    // ADR-0008 raycast fields reject out-of-range values.
    internal static class EngineConfigChecks
    {
        internal static void Run(string repoRoot, Checker check)
        {
            EngineConfig shipped = JsonStore.Load<EngineConfig>(Path.Combine(repoRoot, Path.Combine("config", "engine.json")));
            shipped.Validate();
            check.True("engine.json: validates", true, "");
            check.True("engine.json: raycast enabled", shipped.RaycastEnabled, "");
            check.Equal("engine.json: raycastMaxPasses", 8, shipped.RaycastMaxPasses);
            check.Near("engine.json: raycastPassStepMeters", 0.05, shipped.RaycastPassStepMeters, 1e-6);
            check.Near("engine.json: raycastMaxLengthMeters", 1000, shipped.RaycastMaxLengthMeters, 1e-3);

            // A file written before the raycast fields existed gets the documented defaults.
            EngineConfig old = Parse("{\"schemaVersion\":1,\"coreEnabled\":true,\"pedRadiusMeters\":120,\"coreSpotCheckFrames\":300,\"coreSpotCheckToleranceMeters\":0.05}");
            old.Validate();
            check.True("engine.json defaults: raycast on, 8 passes, 0.05 m step, 1000 m",
                old.RaycastEnabled && old.RaycastMaxPasses == 8 && Math.Abs(old.RaycastPassStepMeters - 0.05f) < 1e-6 && Math.Abs(old.RaycastMaxLengthMeters - 1000f) < 1e-3, "");
            check.True("engine.json defaults: raycastMaxPasses 0 is allowed", Valid("\"raycastMaxPasses\":0"), "");
            check.True("engine.json rejects raycastMaxPasses 33", !Valid("\"raycastMaxPasses\":33"), "");
            check.True("engine.json rejects raycastPassStepMeters 2", !Valid("\"raycastPassStepMeters\":2"), "");
            check.True("engine.json rejects raycastMaxLengthMeters 6000", !Valid("\"raycastMaxLengthMeters\":6000"), "");
            check.True("engine.json accepts raycastEnabled false", Valid("\"raycastEnabled\":false"), "");
        }

        private static EngineConfig Parse(string json) { return JsonStore.Parse<EngineConfig>(Encoding.UTF8.GetBytes(json)); }

        private static bool Valid(string extra)
        {
            try
            {
                Parse("{\"schemaVersion\":1,\"coreEnabled\":true,\"pedRadiusMeters\":120,\"coreSpotCheckFrames\":300,\"coreSpotCheckToleranceMeters\":0.05," + extra + "}").Validate();
                return true;
            }
            catch (InvalidDataException) { return false; }
        }
    }
}
