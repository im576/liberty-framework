using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Liberty.World
{
    // config\world\objects.json: where the static world objects this project ships stand, and how far from the player
    // they are streamed in and out. Positions and distances are data (AGENTS.md rule 3); WorldStreaming decides.
    [DataContract]
    public sealed class WorldObjectsConfig
    {
        [DataContract]
        public sealed class Proxy
        {
            // Vanilla model name (a prop the game gives solid collision).
            [DataMember(Name = "model", Order = 0)] public string Model;
            // Offset from the object's origin, metres along the world axes (the object's heading is not applied to it).
            [DataMember(Name = "offset", Order = 1, EmitDefaultValue = false)] public float[] Offset;
            // Added to the object's heading, degrees.
            [DataMember(Name = "headingOffsetDegrees", Order = 2, EmitDefaultValue = false)] public float HeadingOffsetDegrees;
        }
        [DataContract]
        public sealed class Placement
        {
            // Unique id for logs and the "world" command.
            [DataMember(Name = "name", Order = 0)] public string Name;
            // Model name as registered in lf_content.ide (a LibertyContent asset of type "object" or "prop").
            [DataMember(Name = "model", Order = 1)] public string Model;
            // World position of the model's origin (its spawn point), metres, GTA IV space.
            [DataMember(Name = "position", Order = 2)] public float[] Position;
            // Heading in degrees (0 = north, counter-clockwise), as IProps uses it.
            [DataMember(Name = "headingDegrees", Order = 3)] public float HeadingDegrees;
            // Put the origin on the ground under position (when the game reports a ground height there); position's z
            // is then only where the search starts. False places the origin exactly at position.
            [DataMember(Name = "snapToGround", Order = 4)] public bool SnapToGround;
            // Collision without authored bounds: vanilla props that already have solid collision are placed hidden at the
            // object's spot (each with its own offset and heading), so the object is solid. Several proxies tile a bigger
            // shape. Which vanilla models are solid is established by the proxy-probe scenario, not assumed.
            [DataMember(Name = "collisionProxies", Order = 5, EmitDefaultValue = false)] public List<Proxy> CollisionProxies;
            public Placement() { SetDefaults(); }

            // DataContractJsonSerializer runs no constructor or field initialiser: members absent from the JSON would be
            // 0/false/null. This runs before the JSON is read, so they keep these defaults instead.
            [OnDeserializing]
            private void OnDeserializing(StreamingContext context) { SetDefaults(); }

            private void SetDefaults() { SnapToGround = true; }
        }

        [DataMember(Name = "schemaVersion", Order = 0)] public int SchemaVersion;
        [DataMember(Name = "enabled", Order = 1)] public bool Enabled;
        // Spawn an object when the player comes this close (metres, horizontal).
        [DataMember(Name = "streamInMeters", Order = 2)] public float StreamInMeters;
        // Remove it again beyond this distance; larger than streamInMeters so an object at the edge does not flicker.
        [DataMember(Name = "streamOutMeters", Order = 3)] public float StreamOutMeters;
        // How often the distances are checked (milliseconds).
        [DataMember(Name = "checkIntervalMilliseconds", Order = 4)] public int CheckIntervalMilliseconds;
        // Objects get collision (from their model's bounds, when the game pairs them) unless this is false.
        [DataMember(Name = "collision", Order = 5)] public bool Collision;
        [DataMember(Name = "objects", Order = 6)] public List<Placement> Objects;

        public WorldObjectsConfig() { SetDefaults(); }

        // See Placement.OnDeserializing: absent members keep these defaults (config/world/objects.json normally sets them all).
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) { SetDefaults(); }

        private void SetDefaults()
        {
            SchemaVersion = 1;
            Enabled = true;
            StreamInMeters = 150;
            StreamOutMeters = 180;
            CheckIntervalMilliseconds = 500;
            Collision = true;
            Objects = new List<Placement>();
        }

        // Model names as LibertyContent writes them (asset.json "name": 1-23 characters).
        private static readonly Regex ModelName = new Regex("^[A-Za-z0-9_]{1,23}$");
        // Limits that keep the config sane, not tuning: the streaming radius stays inside the draw distances the
        // compiler accepts (drawDistanceMeters at most 1500).
        public const float MaxStreamMeters = 1500, MaxCoordinateMeters = 10000, MaxProxyOffsetMeters = 50;
        public const int MaxProxiesPerObject = 16;
        public const int MinCheckIntervalMilliseconds = 50, MaxCheckIntervalMilliseconds = 10000;

        public static WorldObjectsConfig Defaults() { return new WorldObjectsConfig(); }

        // Throws ArgumentException naming the first problem (IConfig.Load then keeps the defaults and logs it).
        public static void Validate(WorldObjectsConfig config)
        {
            if (config.SchemaVersion != 1) { throw new ArgumentException("schemaVersion must be 1"); }
            if (!(config.StreamInMeters > 0 && config.StreamInMeters <= MaxStreamMeters)) { throw new ArgumentException("streamInMeters must be above 0 and at most " + MaxStreamMeters); }
            if (!(config.StreamOutMeters > config.StreamInMeters && config.StreamOutMeters <= MaxStreamMeters)) { throw new ArgumentException("streamOutMeters must be above streamInMeters and at most " + MaxStreamMeters); }
            if (config.CheckIntervalMilliseconds < MinCheckIntervalMilliseconds || config.CheckIntervalMilliseconds > MaxCheckIntervalMilliseconds)
            {
                throw new ArgumentException("checkIntervalMilliseconds must be " + MinCheckIntervalMilliseconds + "-" + MaxCheckIntervalMilliseconds);
            }
            if (config.Objects == null) { throw new ArgumentException("objects is missing"); }
            HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Placement p in config.Objects)
            {
                string label = p == null ? "an object" : "object '" + p.Name + "'";
                if (p == null || string.IsNullOrEmpty(p.Name)) { throw new ArgumentException("every object needs a name"); }
                if (!names.Add(p.Name)) { throw new ArgumentException(label + " is listed twice"); }
                if (p.Model == null || !ModelName.IsMatch(p.Model)) { throw new ArgumentException(label + ": model must be 1-23 letters, digits or _"); }
                if (p.Position == null || p.Position.Length != 3 || p.Position.Any(v => float.IsNaN(v) || float.IsInfinity(v) || Math.Abs(v) > MaxCoordinateMeters))
                {
                    throw new ArgumentException(label + ": position must be three finite coordinates within " + MaxCoordinateMeters.ToString(CultureInfo.InvariantCulture) + " m");
                }
                if (float.IsNaN(p.HeadingDegrees) || float.IsInfinity(p.HeadingDegrees)) { throw new ArgumentException(label + ": headingDegrees must be finite"); }
                if (p.CollisionProxies != null)
                {
                    foreach (Proxy proxy in p.CollisionProxies)
                    {
                        if (proxy == null || proxy.Model == null || !ModelName.IsMatch(proxy.Model)) { throw new ArgumentException(label + ": every collision proxy needs a model of 1-23 letters, digits or _"); }
                        if (proxy.Offset != null && (proxy.Offset.Length != 3 || proxy.Offset.Any(v => float.IsNaN(v) || float.IsInfinity(v) || Math.Abs(v) > MaxProxyOffsetMeters)))
                        {
                            throw new ArgumentException(label + ": collision proxy offset must be three finite offsets within " + MaxProxyOffsetMeters.ToString(CultureInfo.InvariantCulture) + " m");
                        }
                        if (float.IsNaN(proxy.HeadingOffsetDegrees) || float.IsInfinity(proxy.HeadingOffsetDegrees)) { throw new ArgumentException(label + ": collision proxy heading must be finite"); }
                    }
                    if (p.CollisionProxies.Count > MaxProxiesPerObject) { throw new ArgumentException(label + ": at most " + MaxProxiesPerObject + " collision proxies"); }
                }
            }
        }
    }
}
