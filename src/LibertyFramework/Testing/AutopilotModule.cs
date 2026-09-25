using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GTA;
using GTA.Native;
using LibertyFramework.CombatEffects;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.DevTools.Teleport;
using LibertyFramework.Engine;
using LibertyFramework.Engine.Events;
using LibertyFramework.Engine.Scheduling;
using LibertyFramework.Engine.World;

namespace LibertyFramework.Testing
{
    // Test commands for the autopilot (tools/autopilot) and the console ("lf <command>"): put the player somewhere,
    // set the world up, spawn subjects, trigger effects, frame a review camera, and log engine events. Every spawned
    // entity is owned by this module and removed by "clear" or when the module stops.
    [Module("autopilot", Order = 200)]
    internal sealed class AutopilotModule : Module
    {
        private readonly List<Ped> subjects = new List<Ped>();
        private Camera camera;
        private bool logEvents;

        protected internal override void Started()
        {
            Register("pos", "player position and heading", Position);
            Register("tp", "tp x y z [heading] - teleport the player", Teleport);
            Register("goto", "goto <location id> - teleport to a DevTools location", GoTo);
            Register("time", "time h m - set the clock", Time);
            Register("weather", "weather <id> - force weather now (0 sunny .. 7 lightning)", Weather);
            Register("god", "god on|off - player invincible", God);
            Register("wanted", "wanted 0 - clear the wanted level", args => { Function.Call("CLEAR_WANTED_LEVEL", Player); return "wanted cleared"; });
            Register("give", "give <weapon id> [ammo] - give and select a weapon", Give);
            Register("select", "select <weapon id>", args => { Player.Character.Weapons.Select((Weapon)Int(args, 0)); return "selected " + Int(args, 0); });
            Register("heading", "heading <degrees>", args => { Player.Character.Heading = Float(args, 0); return "heading " + Float(args, 0); });
            Register("spawn", "spawn <count> [distance m] [weapon id] - NPC subjects facing the player", Spawn);
            Register("clear", "delete spawned subjects", args => { Engine.Entities.ReleaseAll(this); subjects.Clear(); return "cleared"; });
            Register("peds", "list peds in the engine snapshot", Peds);
            Register("hurt", "hurt <subject|all> <amount> - reduce health", Hurt);
            Register("kill", "kill <subject|all>", args => Hurt(new[] { args.Length > 0 ? args[0] : "all", "1000" }));
            Register("gore", "gore gallery|arm|leg|head|leak - DevTools gore test on the nearest NPC", Gore);
            Register("cam", "cam <angle deg> <distance m> <height m> | cam ped <subject> <angle> <distance> <height> | cam off - review camera", Cam);
            Register("hud", "hud on|off - hide HUD and radar for clean screenshots", Hud);
            Register("anim", "anim <set> <clip> - play a clip on the player", args => Engine.Animations.Play(Player.Character, args[0], args[1], 4f) ? "playing" : "failed");
            Register("events", "events on|off - log every engine event", args => { logEvents = On(args); return "event log " + (logEvents ? "on" : "off"); });
            Register("status", "engine status", args => Engine.Status());

            Engine.Events.Subscribe<PedAppeared>(this, e => Log("PedAppeared handle=" + e.Handle));
            Engine.Events.Subscribe<PedRemoved>(this, e => Log("PedRemoved handle=" + e.Handle));
            Engine.Events.Subscribe<PedDamaged>(this, e => Log("PedDamaged handle=" + e.Handle + " " + e.HealthBefore + "->" + e.HealthAfter + " bone=0x" + e.Bone.ToString("X") + " by_player=" + e.ByPlayer));
            Engine.Events.Subscribe<PedDied>(this, e => Log("PedDied handle=" + e.Handle + " bone=0x" + e.Bone.ToString("X") + " by_player=" + e.ByPlayer));
            Engine.Events.Subscribe<PlayerShot>(this, e => Log("PlayerShot weapon=" + e.Weapon + " clip " + e.ClipBefore + "->" + e.ClipAfter));
            Engine.Events.Subscribe<PlayerReloaded>(this, e => Log("PlayerReloaded weapon=" + e.Weapon));
            Engine.Events.Subscribe<PlayerWeaponChanged>(this, e => Log("PlayerWeaponChanged " + e.Previous + "->" + e.Current));
            Engine.Events.Subscribe<PlayerEnteredVehicle>(this, e => Log("PlayerEnteredVehicle " + e.Vehicle));
            Engine.Events.Subscribe<PlayerExitedVehicle>(this, e => Log("PlayerExitedVehicle " + e.Vehicle));
            Engine.Events.Subscribe<PlayerDamaged>(this, e => Log("PlayerDamaged " + e.HealthBefore + "->" + e.HealthAfter));
            Engine.Events.Subscribe<WeatherChanged>(this, e => Log("WeatherChanged " + e.Previous + "->" + e.Current));
            Engine.Events.Subscribe<PauseChanged>(this, e => Log("PauseChanged " + e.Paused));
            Engine.Events.Subscribe<FadeChanged>(this, e => Log("FadeChanged " + e.FadedOut));
            Engine.Events.Subscribe<ModuleFailed>(this, e => RuntimeLog.Error("autopilot_saw_module_failure " + e.ModuleId + " " + e.Error));
        }

        protected internal override void Stopped()
        {
            CameraOff();
        }

        private void Register(string name, string usage, Func<string[], string> handler) { Engine.Commands.Register(this, name, usage, handler); }

        private void Log(string text) { if (logEvents) { RuntimeLog.Info("event " + text); } }

        private string Position(string[] args)
        {
            Ped ped = Player.Character;
            Vector3 p = ped.Position;
            return F(p.X) + " " + F(p.Y) + " " + F(p.Z) + " heading " + F(ped.Heading) + " core_peds=" + Engine.World.Peds.Count;
        }

        private string Teleport(string[] args)
        {
            Vector3 target = new Vector3(Float(args, 0), Float(args, 1), Float(args, 2));
            Ped ped = Player.Character;
            Function.Call("REQUEST_COLLISION_AT_POSN", target.X, target.Y, target.Z);
            Function.Call("LOAD_SCENE", target.X, target.Y, target.Z);
            ped.Position = target;
            if (args.Length > 3) { ped.Heading = Float(args, 3); }
            return "teleported to " + F(target.X) + " " + F(target.Y) + " " + F(target.Z);
        }

        private string GoTo(string[] args)
        {
            LocationFile file = JsonStore.Load<LocationFile>(LibertyPaths.Locations);
            TeleportLocation location = file.Locations.FirstOrDefault(l => string.Equals(l.Id, args[0], StringComparison.OrdinalIgnoreCase));
            if (location == null) { return "unknown location; known: " + string.Join(",", file.Locations.Select(l => l.Id).ToArray()); }
            return Teleport(new[] { F(location.X), F(location.Y), F(location.Z), F(location.Heading) }) + " (" + location.Name + ")";
        }

        private string Time(string[] args)
        {
            Function.Call("SET_TIME_OF_DAY", Int(args, 0), args.Length > 1 ? Int(args, 1) : 0);
            return "time " + Int(args, 0) + ":" + (args.Length > 1 ? Int(args, 1) : 0).ToString("00");
        }

        private string Weather(string[] args)
        {
            Function.Call("FORCE_WEATHER_NOW", Int(args, 0));
            return "weather " + Int(args, 0) + " (the atmosphere module may change it again at its next block)";
        }

        private string God(string[] args)
        {
            Player.Character.Invincible = On(args);
            return "invincible " + On(args);
        }

        private string Give(string[] args)
        {
            Ped ped = Player.Character;
            Weapon weapon = (Weapon)Int(args, 0);
            ped.Weapons.FromType(weapon).Ammo = args.Length > 1 ? Int(args, 1) : 300;
            ped.Weapons.Select(weapon);
            return "gave " + (int)weapon;
        }

        // Subjects in an arc in front of the player, facing the player, standing still (events blocked) so shots land.
        private string Spawn(string[] args)
        {
            int count = Math.Max(1, Math.Min(12, Int(args, 0)));
            float distance = args.Length > 1 ? Float(args, 1) : 6f;
            int weapon = args.Length > 2 ? Int(args, 2) : 0;
            Engine.Scheduler.Start(this, "spawn", SpawnRoutine(count, distance, weapon));
            return "spawning " + count + " at " + F(distance) + " m";
        }

        private IEnumerator SpawnRoutine(int count, float distance, int weapon)
        {
            Ped self = Player.Character;
            Vector3 origin = self.Position;
            double heading = self.Heading * Math.PI / 180.0;
            for (int i = 0; i < count; i++)
            {
                double spread = (i - (count - 1) / 2.0) * 0.35;
                // GTA headings: 0 = north (+Y), increasing counter-clockwise.
                Vector3 spot = origin + new Vector3((float)(-Math.Sin(heading + spread) * distance), (float)(Math.Cos(heading + spread) * distance), 0);
                spot.Z = GTA.World.GetGroundZ(new Vector3(spot.X, spot.Y, origin.Z + 3f)) + 1f;
                Ped ped = null;
                int deadline = Environment.TickCount + 3000;
                while (ped == null && Environment.TickCount < deadline)
                {
                    ped = GTA.World.CreatePed(spot);
                    if (ped == null) { yield return Wait.Milliseconds(100); }
                }
                if (ped == null || !ped.Exists()) { RuntimeLog.Error("autopilot_spawn_failed index=" + i); continue; }
                Engine.Entities.Track(this, ped, ped.Model.Hash, true);
                ped.Heading = self.Heading + 180f;
                ped.BlockPermanentEvents = true;
                if (weapon > 0) { ped.Weapons.FromType((Weapon)weapon).Ammo = 200; ped.Weapons.Select((Weapon)weapon); }
                subjects.Add(ped);
                yield return null;
            }
            RuntimeLog.Info("autopilot_spawned count=" + subjects.Count);
        }

        private string Peds(string[] args)
        {
            IReadOnlyList<PedState> peds = Engine.World.Peds;
            return "peds=" + peds.Count + " " + string.Join(" ", peds.OrderBy(p => p.Distance).Take(12)
                .Select(p => p.Handle + ":" + F(p.Distance) + "m/" + p.Health + (p.IsDead ? "/dead" : "") + (p.IsPlayer ? "/player" : "")).ToArray());
        }

        private string Hurt(string[] args)
        {
            int amount = args.Length > 1 ? Int(args, 1) : 25;
            List<Ped> targets = Targets(args.Length > 0 ? args[0] : "all");
            foreach (Ped ped in targets) { if (ped.Exists()) { ped.Health = Math.Max(-100, ped.Health - amount); } }
            return "hurt " + targets.Count + " by " + amount;
        }

        private List<Ped> Targets(string which)
        {
            subjects.RemoveAll(p => p == null || !p.Exists());
            if (which == "all") { return new List<Ped>(subjects); }
            int index = int.Parse(which, CultureInfo.InvariantCulture);
            return index >= 0 && index < subjects.Count ? new List<Ped> { subjects[index] } : new List<Ped>();
        }

        private string Gore(string[] args)
        {
            CombatEffectsController combat = Engine.Module<CombatEffectsController>();
            if (combat == null || !combat.Running) { return "combat module not running"; }
            string[] names = { "", "gallery", "arm", "leg", "head", "leak" };
            int request = Array.IndexOf(names, args.Length > 0 ? args[0] : "");
            if (request <= 0) { return "gore gallery|arm|leg|head|leak"; }
            combat.RequestGoreTest(request);
            return "gore " + args[0] + " requested on the nearest NPC";
        }

        // Scripted camera orbiting the player: angle 0 = in front of the player looking back at them.
        private string Cam(string[] args)
        {
            if (args.Length > 0 && args[0] == "off") { CameraOff(); return "camera off"; }
            Ped ped = Player.Character;
            if (args.Length > 0 && args[0] == "ped")
            {
                List<Ped> subject = Targets(args[1]);
                if (subject.Count == 0) { return "no subject " + args[1]; }
                ped = subject[0];
                args = args.Skip(2).ToArray();
            }
            double angle = (ped.Heading + Float(args, 0)) * Math.PI / 180.0;
            float distance = args.Length > 1 ? Float(args, 1) : 2.5f, height = args.Length > 2 ? Float(args, 2) : 0.6f;
            Vector3 target = ped.Position;
            Vector3 position = target + new Vector3((float)(-Math.Sin(angle) * distance), (float)(Math.Cos(angle) * distance), height);
            CameraOff();
            camera = new Camera();
            camera.Position = position;
            camera.LookAt(target + new Vector3(0, 0, 0.3f));
            camera.Activate();
            return "camera at " + F(Float(args, 0)) + " deg, " + F(distance) + " m";
        }

        private void CameraOff()
        {
            if (camera == null) { return; }
            // The game may already have destroyed a scripted camera (e.g. after a cutscene or death camera).
            try { camera.Deactivate(); camera.Delete(); }
            catch (Exception error) { RuntimeLog.Info("autopilot_camera_already_gone " + error.Message); }
            camera = null;
        }

        private string Hud(string[] args)
        {
            bool on = On(args);
            Function.Call("DISPLAY_HUD", on);
            Function.Call("DISPLAY_RADAR", on);
            return "hud " + (on ? "on" : "off");
        }

        private static bool On(string[] args) { return args.Length == 0 || args[0] == "on" || args[0] == "1" || args[0] == "true"; }
        private static int Int(string[] args, int index) { return int.Parse(args[index], CultureInfo.InvariantCulture); }
        private static float Float(string[] args, int index) { return float.Parse(args[index], CultureInfo.InvariantCulture); }
        private static string F(float value) { return value.ToString("0.###", CultureInfo.InvariantCulture); }
    }
}
