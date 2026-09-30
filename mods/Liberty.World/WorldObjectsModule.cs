using System.Collections.Generic;
using System.Linq;
using Liberty.Sdk;

namespace Liberty.World
{
    // Static world objects (T-033): places the objects listed in config\world\objects.json while the player is near them,
    // frozen and with collision, and removes them again further away (WorldStreaming). This is how LibertyContent's
    // "object" assets reach the map: the drawable is built and registered like a prop's (the IDE entry class the proven
    // spawn path uses), and this module places it. A map placement file is not written because its format is not
    // established here. Built only against Liberty.Sdk; the engine deletes everything it spawned when it stops.
    [Module("world", Order = 150, Version = "1.0.0", Description = "Places the project's static world objects near the player (config/world/objects.json)")]
    public sealed class WorldObjectsModule : LibertyModule
    {
        private sealed class Slot
        {
            internal WorldObjectsConfig.Placement Placement;
            internal PropRef Prop = PropRef.None;
            internal readonly List<PropRef> Proxies = new List<PropRef>(); // hidden vanilla props that supply the collision (Placement.CollisionProxies)
            internal bool Pending;
            internal bool Unavailable; // the model is not in the game's model index: reported once, never retried
        }

        private WorldObjectsConfig config;
        private readonly List<Slot> slots = new List<Slot>();
        // Bumped when the config reloads, so a spawn that completes afterwards knows it belongs to the old placements.
        private int generation;

        protected override void OnStart()
        {
            Apply(Liberty.Config.Load(this, "objects", WorldObjectsConfig.Defaults, WorldObjectsConfig.Validate));
            Liberty.Config.Watch(this, "objects", WorldObjectsConfig.Defaults, WorldObjectsConfig.Validate, changed =>
            {
                Liberty.Log.Info(this, "world_objects config reloaded");
                Apply(changed);
            });
            Liberty.Commands.Register(this, "world", "world [ray <name>] - the placed world objects; ray: raycast from the player to one (collision test)",
                a => a.Length >= 2 && a[0] == "ray" ? Ray(a[1]) : Describe());
        }

        private void Apply(WorldObjectsConfig next)
        {
            RemoveAll();
            generation++;
            config = next;
            Interval = config.CheckIntervalMilliseconds;
            slots.Clear();
            if (config.Enabled) { slots.AddRange(config.Objects.Select(p => new Slot { Placement = p })); }
            Liberty.Log.Info(this, "world_objects loaded objects=" + slots.Count + " enabled=" + config.Enabled + " in=" + config.StreamInMeters + "m out=" + config.StreamOutMeters + "m");
        }

        protected override void OnUpdate()
        {
            if (slots.Count == 0 || !Liberty.World.HasPlayer) { return; }
            Vec3 player = Liberty.World.Player.Position;
            foreach (Slot slot in slots)
            {
                if (slot.Unavailable || slot.Pending) { continue; }
                if (!slot.Prop.IsNone && !Liberty.Props.Exists(slot.Prop)) { Remove(slot, "removed by the game"); } // the proxy goes with it
                float[] at = slot.Placement.Position;
                float distance = WorldStreaming.HorizontalDistance(player.X, player.Y, at[0], at[1]);
                switch (WorldStreaming.Decide(!slot.Prop.IsNone, distance, config.StreamInMeters, config.StreamOutMeters))
                {
                    case WorldStreaming.Step.Spawn: Spawn(slot); break;
                    case WorldStreaming.Step.Remove: Remove(slot, "out of range"); break;
                }
            }
        }

        private void Spawn(Slot slot)
        {
            WorldObjectsConfig.Placement p = slot.Placement;
            ModelRef model = p.Model;
            if (!Liberty.Streaming.IsValidModel(model))
            {
                slot.Unavailable = true;
                Liberty.Log.Warn(this, "world_object_failed name=" + p.Name + " model=" + p.Model + " reason=not in the game's model index (is LibertyContent.img installed?)");
                return;
            }
            Vec3 position = new Vec3(p.Position[0], p.Position[1], p.Position[2]);
            if (p.SnapToGround)
            {
                // Searched from just above the configured point; 0 means the game reported no ground (area not loaded).
                float ground = Liberty.WorldControl.GroundZ(new Vec3(position.X, position.Y, position.Z + GroundSearchMeters));
                if (ground != 0) { position.Z = ground; }
            }
            int spawnedIn = generation;
            slot.Pending = true;
            Liberty.Props.Spawn(this, model, position, prop =>
            {
                slot.Pending = false;
                if (prop.IsNone) { Liberty.Log.Warn(this, "world_object_failed name=" + p.Name + " model=" + p.Model + " reason=spawn failed"); return; }
                if (spawnedIn != generation) { Liberty.Props.Delete(prop); return; } // the placements changed meanwhile
                Liberty.Props.SetFrozen(prop, true);
                Liberty.Props.SetPosition(prop, position);
                Liberty.Props.SetRotation(prop, new Vec3(0, 0, p.HeadingDegrees));
                Liberty.Props.SetCollision(prop, config.Collision);
                slot.Prop = prop;
                Liberty.Log.Info(this, "world_object spawned name=" + p.Name + " model=" + p.Model + " handle=" + prop.Handle + " at=" + position + " heading=" + p.HeadingDegrees);
                SpawnProxies(slot, position, spawnedIn);
            });
        }

        // The collision proxies: vanilla props with solid collision, frozen at the object's spot and hidden. A proxy's failure
        // never removes the visible object; it is logged and the object simply has less collision.
        private void SpawnProxies(Slot slot, Vec3 position, int spawnedIn)
        {
            WorldObjectsConfig.Placement p = slot.Placement;
            if (p.CollisionProxies == null) { return; }
            foreach (WorldObjectsConfig.Proxy proxy in p.CollisionProxies)
            {
                ModelRef model = proxy.Model;
                if (!Liberty.Streaming.IsValidModel(model))
                {
                    Liberty.Log.Warn(this, "world_proxy_failed name=" + p.Name + " model=" + proxy.Model + " reason=not in the game's model index");
                    continue;
                }
                float[] o = proxy.Offset ?? new float[3];
                Vec3 at = new Vec3(position.X + o[0], position.Y + o[1], position.Z + o[2]);
                float heading = p.HeadingDegrees + proxy.HeadingOffsetDegrees;
                string modelName = proxy.Model;
                Liberty.Props.Spawn(this, model, at, spawned =>
                {
                    if (spawned.IsNone) { Liberty.Log.Warn(this, "world_proxy_failed name=" + p.Name + " model=" + modelName + " reason=spawn failed"); return; }
                    if (spawnedIn != generation || slot.Prop.IsNone) { Liberty.Props.Delete(spawned); return; }
                    Liberty.Props.SetFrozen(spawned, true);
                    Liberty.Props.SetPosition(spawned, at);
                    Liberty.Props.SetRotation(spawned, new Vec3(0, 0, heading));
                    Liberty.Props.SetCollision(spawned, true);
                    Liberty.Props.SetVisible(spawned, false);
                    slot.Proxies.Add(spawned);
                    Liberty.Log.Info(this, "world_proxy spawned name=" + p.Name + " model=" + modelName + " handle=" + spawned.Handle + " at=" + at);
                });
            }
        }
        // How far above the configured point the ground search starts: a placement's z may sit slightly under the surface.
        private const float GroundSearchMeters = 2f;
        // The ray aims this far above the object's origin, inside a prop resting on the ground rather than at its base.
        private const float RayAimHeightMeters = 0.3f;

        // The engine's raycast from the player to the object's origin: whether its collision stops the ray (T032/T033).
        private string Ray(string name)
        {
            Slot slot = slots.FirstOrDefault(s => string.Equals(s.Placement.Name, name, System.StringComparison.OrdinalIgnoreCase));
            if (slot == null) { return "no world object " + name; }
            if (slot.Prop.IsNone || !Liberty.Props.Exists(slot.Prop)) { return "world object " + name + " is not spawned"; }
            PlayerState self = Liberty.World.Player;
            Vec3 target = Liberty.Props.GetPosition(slot.Prop) + new Vec3(0, 0, RayAimHeightMeters);
            RayHit hit = Liberty.Query.Raycast(self.Position, target, RayMask.All, RayIgnore.Of(self.Ped).And(self.Vehicle));
            bool match = hit.IsHit && hit.Kind == RayEntityKind.Object && (hit.EntityHandle == slot.Prop.Handle || slot.Proxies.Any(x => x.Handle == hit.EntityHandle));
            string line = "world_ray name=" + name + " handle=" + slot.Prop.Handle + " status=" + hit.Status + " kind=" + hit.Kind + " hit_handle=" + hit.EntityHandle +
                " distance=" + hit.Distance.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + " of=" +
                self.Position.DistanceTo(target).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + " match=" + match;
            Liberty.Log.Info(this, line);
            return line;
        }

        private void Remove(Slot slot, string reason)
        {
            foreach (PropRef proxy in slot.Proxies) { Liberty.Props.Delete(proxy); }
            slot.Proxies.Clear();
            if (slot.Prop.IsNone) { return; }
            Liberty.Props.Delete(slot.Prop);
            slot.Prop = PropRef.None;
            Liberty.Log.Info(this, "world_object removed name=" + slot.Placement.Name + " reason=" + reason);
        }

        private void RemoveAll() { foreach (Slot slot in slots) { Remove(slot, "reload"); } }

        private string Describe()
        {
            if (config == null || !config.Enabled) { return "world objects disabled"; }
            return "world objects=" + slots.Count + " spawned=" + slots.Count(s => !s.Prop.IsNone) + " " +
                string.Join(" ", slots.Select(s => s.Placement.Name + ":" + (s.Unavailable ? "unavailable" : s.Pending ? "spawning" : s.Prop.IsNone ? "away" : "handle " + s.Prop.Handle)).ToArray());
        }
    }
}
