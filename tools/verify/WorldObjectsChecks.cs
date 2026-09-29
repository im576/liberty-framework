using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using Liberty.World;

namespace LibertyFramework.Verify
{
    // T-033: the world mod's configuration and streaming logic (mods/Liberty.World/Logic), offline. The shipped
    // config/world/objects.json must deserialise and validate as the module will load it, every placed model must be a
    // content asset this repository builds, bad configs must be refused, and the streaming decision must keep its
    // hysteresis.
    internal static class WorldObjectsChecks
    {
        internal static void Run(string repoRoot, Checker check)
        {
            string path = Path.Combine(repoRoot, "config", "world", "objects.json");
            WorldObjectsConfig shipped = Load(File.ReadAllText(path));
            string problem = null;
            try { WorldObjectsConfig.Validate(shipped); } catch (ArgumentException error) { problem = error.Message; }
            check.True("world objects: shipped config validates", problem == null, problem ?? shipped.Objects.Count + " objects");

            HashSet<string> assets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string manifest in Directory.GetFiles(Path.Combine(repoRoot, "content"), "asset.json", SearchOption.AllDirectories))
            {
                Match name = Regex.Match(File.ReadAllText(manifest), "\"name\"\\s*:\\s*\"([^\"]+)\"");
                if (name.Success) { assets.Add(name.Groups[1].Value); }
            }
            string[] missing = shipped.Objects.Where(o => !assets.Contains(o.Model)).Select(o => o.Model).ToArray();
            check.True("world objects: every placed model is a content asset", missing.Length == 0, missing.Length == 0 ? string.Join(",", shipped.Objects.Select(o => o.Model).ToArray()) : "missing " + string.Join(",", missing));

            check.True("world objects: defaults validate", Refused(WorldObjectsConfig.Defaults()) == null, "");
            check.True("world objects: streamOut not above streamIn is refused", Refused(Load("{\"streamInMeters\":100,\"streamOutMeters\":100,\"objects\":[]}")) != null, "");
            check.True("world objects: duplicate names are refused", Refused(Load("{\"objects\":[" + Object("a", "m1") + "," + Object("A", "m2") + "]}")) != null, "");
            check.True("world objects: a bad model name is refused", Refused(Load("{\"objects\":[" + Object("a", "bad name") + "]}")) != null, "");
            check.True("world objects: a position without three coordinates is refused", Refused(Load("{\"objects\":[{\"name\":\"a\",\"model\":\"m\",\"position\":[1,2]}]}")) != null, "");
            WorldObjectsConfig sparse = Load("{\"objects\":[" + Object("a", "m") + "]}");
            check.True("world objects: absent members keep their defaults when deserialised", sparse.Objects[0].SnapToGround && sparse.Enabled && sparse.StreamInMeters == 150 &&
                sparse.StreamOutMeters == 180 && sparse.CheckIntervalMilliseconds == 500 && sparse.Collision && sparse.SchemaVersion == 1, "");

            check.True("world streaming: absent and inside streamIn spawns", WorldStreaming.Decide(false, 149, 150, 180) == WorldStreaming.Step.Spawn, "");
            check.True("world streaming: absent between the radii stays absent", WorldStreaming.Decide(false, 160, 150, 180) == WorldStreaming.Step.Keep, "");
            check.True("world streaming: present between the radii stays (hysteresis)", WorldStreaming.Decide(true, 160, 150, 180) == WorldStreaming.Step.Keep, "");
            check.True("world streaming: present beyond streamOut is removed", WorldStreaming.Decide(true, 181, 150, 180) == WorldStreaming.Step.Remove, "");
            check.True("world streaming: distance is horizontal", Math.Abs(WorldStreaming.HorizontalDistance(0, 0, 3, 4) - 5) < 1e-6, "");
        }

        private static string Object(string name, string model) { return "{\"name\":\"" + name + "\",\"model\":\"" + model + "\",\"position\":[1,2,3]}"; }

        // As the engine's IConfig loads DataContract types (JsonStore): absent members keep the [OnDeserializing] defaults.
        private static WorldObjectsConfig Load(string json)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (WorldObjectsConfig)new DataContractJsonSerializer(typeof(WorldObjectsConfig)).ReadObject(stream);
            }
        }

        private static string Refused(WorldObjectsConfig config)
        {
            try { WorldObjectsConfig.Validate(config); return null; }
            catch (ArgumentException error) { return error.Message; }
        }
    }
}
