using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Liberty.Sdk;

namespace Liberty.Autopilot
{
    // Test commands for the autopilot (tools/autopilot) and the console ("lf <command>"). Built only against
    // Liberty.Sdk, as a mod in scripts\LibertyFramework\mods\: it is the SDK's first real client, so every command here
    // exercises the public API and its ownership rules. Everything it spawns, cameras, HUD and weather overrides
    // are released by "clear"/"cam off" or automatically when the module stops.
    [Module("autopilot", Order = 200, Version = "1.0.0",
        Capabilities = new[] { Capabilities.Developer, Capabilities.InputCapture, Capabilities.PlayerControl },
        Description = "Autopilot test commands and the SDK self-test")]
    public sealed class AutopilotModule : LibertyModule
    {
        private readonly List<PedRef> subjects = new List<PedRef>();
        private readonly List<VehicleRef> cars = new List<VehicleRef>();
        private readonly List<IMenu> menus = new List<IMenu>();
        private CameraRef camera = CameraRef.None;
        private EventLogger events;

        protected override void OnStart()
        {
            events = new EventLogger(this, Liberty);
            events.Subscribe();
            Register("pos", "player position and heading", Position);
            Register("tp", "tp x y z [heading] - teleport the player", Teleport);
            Register("time", "time h m - set the clock", a => { Liberty.WorldControl.SetTime(Args.Int(a, 0), Args.Int(a, 1, 0)); return "time " + Args.Int(a, 0) + ":" + Args.Int(a, 1, 0).ToString("00"); });
            Register("weather", "weather <id> - force weather (0 sunny .. 7 lightning) until the module stops", a => { Liberty.WorldControl.ForceWeather(this, Args.Int(a, 0)); return "weather " + Args.Int(a, 0); });
            Register("god", "god on|off - player invincible", a => { Liberty.Player.SetInvincible(this, Args.On(a)); return "invincible " + Args.On(a); });
            Register("wanted", "wanted <level> - set the wanted level (0 clears)", a => { Liberty.Player.WantedLevel = Args.Int(a, 0, 0); return "wanted " + Args.Int(a, 0, 0); });
            Register("give", "give <weapon id> [ammo] - give and select a weapon", Give);
            Register("select", "select <weapon id>", a => { Liberty.Weapons.Select(Liberty.Player.Ped, Args.Int(a, 0)); return "selected " + Args.Int(a, 0); });
            Register("heading", "heading <degrees>", a => { Liberty.Peds.SetHeading(Liberty.Player.Ped, Args.Float(a, 0)); return "heading " + Args.F(Args.Float(a, 0)); });
            Register("spawn", "spawn <count> [distance m] [weapon id] - NPC subjects facing the player", Spawn);
            Register("spawncar", "spawncar <model> [distance m] - a vehicle in front of the player", SpawnCar);
            Register("clear", "delete spawned subjects and vehicles", Clear);
            Register("peds", "list peds in the engine snapshot", Peds);
            Register("vehicles", "list vehicles in the engine snapshot", Vehicles);
            Register("hurt", "hurt <subject|all> <amount> - reduce health", Hurt);
            Register("kill", "kill <subject|all>", a => Hurt(new[] { a.Length > 0 ? a[0] : "all", "1000" }));
            Register("cam", "cam <angle deg> <distance m> <height m> | cam ped <subject> <angle> <distance> <height> | cam off - review camera", Cam);
            Register("hud", "hud on|off - hide HUD and radar for clean screenshots", a => { Liberty.Ui.SetHudVisible(this, Args.On(a)); return "hud " + (Args.On(a) ? "on" : "off"); });
            Register("anim", "anim <dictionary> <clip> - play a clip on the player", a => Liberty.Animation.Play(Liberty.Player.Ped, new AnimClip(a[0], a[1]), AnimOptions.Default) ? "playing" : "failed");
            Register("events", "events on|off - log every engine event", a => { events.Enabled = Args.On(a); return "event log " + (events.Enabled ? "on" : "off"); });
            Register("status", "engine status", a => Liberty.Commands.Execute("engine", "autopilot"));
            Register("selftest", "run the SDK self-test (about 30 s); results in the log as 'selftest ...'", a => { Liberty.Scheduler.Start(this, "selftest", new SdkSelfTest(this, Liberty).Run()); return "selftest started"; });
            Register("stress", "stress <count> - spawn random peds around the player (performance)", Stress);
            Register("menu-test", "open a sample list menu (screenshots)", MenuTest);
            Register("radial-test", "open a sample radial menu (screenshots)", a => { OpenRadial(); return "radial open"; });
            Register("menu-close", "close the menus this module opened", a => { int n = menus.Count; foreach (IMenu m in menus) { m.Close(); } menus.Clear(); return "closed " + n; });
            Register("fire", "fire <subject|all> [ms] - armed subjects shoot at a point beside them (bullet events)", Fire);
            Liberty.Log.Info(this, "autopilot_ready sdk=" + SdkVersion.Text + " engine=" + Liberty.EngineVersion + " episode=" + Liberty.Episode);
        }

        protected override void OnStop()
        {
            menus.Clear();
            subjects.Clear();
            cars.Clear();
            camera = CameraRef.None;
        }

        private void Register(string name, string usage, Func<string[], string> handler) { Liberty.Commands.Register(this, name, usage, handler); }

        private string Position(string[] args)
        {
            PlayerState p = Liberty.World.Player;
            return Args.F(p.Position.X) + " " + Args.F(p.Position.Y) + " " + Args.F(p.Position.Z) + " heading " + Args.F(p.Heading) +
                " core_peds=" + Liberty.World.Peds.Count + " core_vehicles=" + Liberty.World.Vehicles.Count;
        }

        private string Teleport(string[] args)
        {
            Vec3 target = new Vec3(Args.Float(args, 0), Args.Float(args, 1), Args.Float(args, 2));
            Liberty.Player.Teleport(target, Args.Float(args, 3, Liberty.World.Player.Heading));
            return "teleported to " + target;
        }

        private string Give(string[] args)
        {
            int weapon = Args.Int(args, 0);
            PedRef ped = Liberty.Player.Ped;
            Liberty.Weapons.Give(ped, weapon, Args.Int(args, 1, 300));
            Liberty.Weapons.Select(ped, weapon);
            return "gave " + weapon;
        }

        // A point `distance` metres from the player, `spread` radians off their facing, on the ground.
        private Vec3 InFront(float distance, double spread)
        {
            PlayerState self = Liberty.World.Player;
            double heading = self.Heading * Math.PI / 180.0 + spread;
            // GTA headings: 0 = north (+Y), increasing counter-clockwise.
            Vec3 spot = self.Position + new Vec3((float)(-Math.Sin(heading) * distance), (float)(Math.Cos(heading) * distance), 0);
            float ground = Liberty.WorldControl.GroundZ(new Vec3(spot.X, spot.Y, self.Position.Z + 3f));
            spot.Z = ground != 0 ? ground + 1f : self.Position.Z;
            return spot;
        }

        // Subjects in an arc in front of the player, facing the player, ignoring ambient events so shots land.
        private string Spawn(string[] args)
        {
            int count = Math.Max(1, Math.Min(12, Args.Int(args, 0)));
            float distance = Args.Float(args, 1, 6f);
            int weapon = Args.Int(args, 2, 0);
            Liberty.Scheduler.Start(this, "spawn", SpawnRoutine(count, distance, weapon));
            return "spawning " + count + " at " + Args.F(distance) + " m";
        }

        private IEnumerator SpawnRoutine(int count, float distance, int weapon)
        {
            float heading = Liberty.World.Player.Heading + 180f;
            for (int i = 0; i < count; i++)
            {
                PedRef spawned = PedRef.None;
                bool done = false;
                Liberty.Peds.SpawnRandom(this, InFront(distance, (i - (count - 1) / 2.0) * 0.35), heading, ped => { spawned = ped; done = true; });
                yield return Wait.Until(() => done, 6000);
                if (spawned.IsNone) { Liberty.Log.Error(this, "autopilot_spawn_failed index=" + i); continue; }
                Liberty.Peds.SetBlockEvents(spawned, true);
                if (weapon > 0) { Liberty.Weapons.Give(spawned, weapon, 200); Liberty.Weapons.Select(spawned, weapon); }
                subjects.Add(spawned);
            }
            Liberty.Log.Info(this, "autopilot_spawned count=" + subjects.Count);
        }

        private string SpawnCar(string[] args)
        {
            ModelRef model = args.Length > 0 ? args[0] : "admiral";
            Liberty.Vehicles.Spawn(this, model, InFront(Args.Float(args, 1, 7f), 0), Liberty.World.Player.Heading + 90f, vehicle =>
            {
                if (vehicle.IsNone) { Liberty.Log.Error(this, "autopilot_spawncar_failed model=" + model); return; }
                cars.Add(vehicle);
                Liberty.Log.Info(this, "autopilot_spawncar handle=" + vehicle.Handle + " model=" + model);
            });
            return "spawning " + model;
        }

        private string Clear(string[] args)
        {
            foreach (PedRef ped in subjects) { Liberty.Peds.Delete(ped); }
            foreach (VehicleRef car in cars) { Liberty.Vehicles.Delete(car); }
            int n = subjects.Count + cars.Count;
            subjects.Clear();
            cars.Clear();
            return "cleared " + n;
        }

        private string Peds(string[] args)
        {
            IReadOnlyList<PedState> peds = Liberty.World.Peds;
            return "peds=" + peds.Count + " " + string.Join(" ", peds.OrderBy(p => p.Distance).Take(12)
                .Select(p => p.Ped.Handle + ":" + Args.F(p.Distance) + "m/" + p.Health + (p.IsDead ? "/dead" : "") + (p.IsPlayer ? "/player" : "")).ToArray());
        }

        private string Vehicles(string[] args)
        {
            IReadOnlyList<VehicleState> vehicles = Liberty.World.Vehicles;
            return "vehicles=" + vehicles.Count + " " + string.Join(" ", vehicles.OrderBy(v => v.Distance).Take(12)
                .Select(v => v.Vehicle.Handle + ":" + Args.F(v.Distance) + "m/" + v.Health + "/" + Args.F(v.EngineHealth) + "/" + Args.F(v.Speed) + "mps").ToArray());
        }

        private string Hurt(string[] args)
        {
            int amount = Args.Int(args, 1, 25);
            List<PedRef> targets = Targets(args.Length > 0 ? args[0] : "all");
            foreach (PedRef ped in targets) { if (Liberty.Peds.Exists(ped)) { Liberty.Peds.SetHealth(ped, Math.Max(-100, Liberty.Peds.GetHealth(ped) - amount)); } }
            return "hurt " + targets.Count + " by " + amount;
        }

        private List<PedRef> Targets(string which)
        {
            subjects.RemoveAll(p => !Liberty.Peds.Exists(p));
            if (which == "all") { return new List<PedRef>(subjects); }
            int index = Args.Int(new[] { which }, 0);
            return index >= 0 && index < subjects.Count ? new List<PedRef> { subjects[index] } : new List<PedRef>();
        }

        // Scripted camera orbiting a ped: angle 0 = in front of the ped looking back at them.
        private string Cam(string[] args)
        {
            if (args.Length > 0 && args[0] == "off") { CameraOff(); return "camera off"; }
            PedRef ped = Liberty.Player.Ped;
            if (args.Length > 0 && args[0] == "ped")
            {
                List<PedRef> subject = Targets(args[1]);
                if (subject.Count == 0) { return "no subject " + args[1]; }
                ped = subject[0];
                args = args.Skip(2).ToArray();
            }
            float pedHeading = Liberty.Peds.GetHeading(ped);
            double angle = (pedHeading + Args.Float(args, 0, 0f)) * Math.PI / 180.0;
            float distance = Args.Float(args, 1, 2.5f), height = Args.Float(args, 2, 0.6f);
            Vec3 target = Liberty.Peds.GetPosition(ped);
            Vec3 position = target + new Vec3((float)(-Math.Sin(angle) * distance), (float)(Math.Cos(angle) * distance), height);
            CameraOff();
            camera = Liberty.Cameras.Create(this);
            Liberty.Cameras.SetPosition(camera, position);
            Liberty.Cameras.PointAt(camera, target + new Vec3(0, 0, 0.3f));
            Liberty.Cameras.Activate(camera);
            return "camera at " + Args.F(Args.Float(args, 0, 0f)) + " deg, " + Args.F(distance) + " m";
        }

        private void CameraOff()
        {
            if (camera.IsNone) { return; }
            Liberty.Cameras.Destroy(camera);
            camera = CameraRef.None;
        }

        private string Stress(string[] args)
        {
            int count = Math.Max(1, Math.Min(40, Args.Int(args, 0, 20)));
            for (int i = 0; i < count; i++)
            {
                double angle = i * 2 * Math.PI / count;
                Liberty.Peds.SpawnRandom(this, InFront(8f + (i % 3) * 4f, angle), 0f, ped => { if (!ped.IsNone) { subjects.Add(ped); } });
            }
            return "stress spawning " + count;
        }

        private string MenuTest(string[] args)
        {
            ListMenu list = new ListMenu();
            list.Title = "LIBERTY SDK";
            int value = 5;
            list.Items = () => new List<ListItem>
            {
                ListItem.Info(() => "Frame " + Args.F(Liberty.Perf.FrameMs) + " ms, pressure " + Args.F(Liberty.Perf.Pressure)),
                ListItem.Adjust(() => "Value < " + value + " >", d => { value += d; return null; }),
                ListItem.Action("Notify", () => { Liberty.Ui.Notify("Hello from the SDK", 3000); return "sent"; }),
                ListItem.Action("Open radial", () => { OpenRadial(); return null; }),
            };
            menus.Add(Liberty.Ui.OpenList(this, list));
            return "menu open";
        }

        private void OpenRadial()
        {
            RadialMenu radial = new RadialMenu();
            radial.Title = "SDK WHEEL";
            for (int i = 0; i < 8; i++)
            {
                int index = i;
                RadialSegment segment = new RadialSegment();
                segment.Label = () => "Slot " + index;
                segment.Icon = () => Liberty.Ui.WeaponIcon(index == 0 ? 7 : 7 + index);
                segment.Badge = () => index % 2 == 0 ? "+" + index : null;
                radial.Segments.Add(segment);
            }
            radial.CenterLines = s => new[] { "SEGMENT", "Slot " + s, "Radial menus come from Liberty.Ui", "A Pick   B Close" };
            radial.OnAccept = s => "picked " + s;
            menus.Add(Liberty.Ui.OpenRadial(this, radial));
        }

        private string Fire(string[] args)
        {
            List<PedRef> targets = Targets(args.Length > 0 ? args[0] : "all");
            int ms = Args.Int(args, 1, 2000);
            foreach (PedRef ped in targets)
            {
                Vec3 at = Liberty.Peds.GetPosition(ped) + Vec3.FromHeading(Liberty.Peds.GetHeading(ped) + 90f) * 6f;
                Liberty.Tasks.ShootAt(ped, at, ms);
            }
            return "firing " + targets.Count;
        }
    }
}
