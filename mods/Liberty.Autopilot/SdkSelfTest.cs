using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Liberty.Sdk;
using Liberty.Sdk.Events;

namespace Liberty.Autopilot
{
    // Exercises every SDK service in the running game and logs one line per check:
    //   selftest <name> ok|FAIL|info <detail>      and finally      selftest_done passed=N failed=M
    // The autopilot scenario "sdk-selftest" expects selftest_done with failed=0. Everything created here is deleted
    // again (and would be released by the engine if the test stopped half-way).
    internal sealed class SdkSelfTest
    {
        [DataContract]
        internal sealed class Settings
        {
            [DataMember(Name = "schemaVersion")] public int SchemaVersion = 1;
            [DataMember(Name = "greeting")] public string Greeting = "hello";
        }

        private readonly LibertyModule owner;
        private readonly ILiberty liberty;
        private int passed, failed;
        private bool vehicleAppeared, vehicleRemoved;
        private VehicleRef watchedCar = VehicleRef.None;

        internal SdkSelfTest(LibertyModule owner, ILiberty liberty) { this.owner = owner; this.liberty = liberty; }

        private void Check(string name, bool ok, string detail)
        {
            if (ok) { passed++; } else { failed++; }
            liberty.Log.Info(owner, "selftest " + name + " " + (ok ? "ok" : "FAIL") + (string.IsNullOrEmpty(detail) ? "" : " " + detail));
        }

        private void Info(string name, string detail) { liberty.Log.Info(owner, "selftest " + name + " info " + detail); }

        internal IEnumerator Run()
        {
            liberty.Log.Info(owner, "selftest_begin engine=" + liberty.EngineVersion + " sdk=" + SdkVersion.Text);
            PedRef player = liberty.Player.Ped;
            Check("world", liberty.World.HasPlayer, "from_core=" + liberty.World.FromCore + " peds=" + liberty.World.Peds.Count + " vehicles=" + liberty.World.Vehicles.Count);
            Check("player", !player.IsNone && liberty.Peds.Exists(player), "ped=" + player.Handle + " index=" + liberty.Player.Index);
            Check("modules", liberty.Modules.IsRunning("autopilot") && liberty.Modules.List().Count > 1, "loaded=" + liberty.Modules.List().Count);
            Check("perf", liberty.Perf.FrameMs > 0 && liberty.Perf.AddressSpaceFreeBytes > 0,
                "frame_ms=" + liberty.Perf.FrameMs.ToString("0.0") + " free_mb=" + (liberty.Perf.AddressSpaceFreeBytes >> 20) + " pressure=" + liberty.Perf.Pressure.ToString("0.00"));
            Info("input", "pad=" + liberty.Input.PadConnected);
            Info("episode", liberty.Episode.ToString());
            float water;
            float ground = liberty.Query.GroundZ(liberty.World.Player.Position + new Vec3(0, 0, 2));
            Check("query-ground", ground != 0 && Math.Abs(ground - liberty.World.Player.Position.Z) < 3f, "ground=" + ground.ToString("0.00") + " water=" + liberty.Query.WaterHeight(liberty.World.Player.Position, out water) + " " + water.ToString("0.00"));

            // Capabilities: this module did not declare memory.patch or engine.internal, so both must be refused.
            Check("capability-memory", Refused(() => liberty.Memory.FindNative(0x62E319C6)), "IMemory refused without memory.patch");
            Check("capability-natives", Refused(() => liberty.Natives.CallInt(owner, "GET_PLAYER_ID")), "INatives refused without engine.internal");

            // Config and state round trips.
            Settings settings = liberty.Config.Load(owner, "selftest", () => new Settings(), s => { if (s.SchemaVersion != 1) { throw new InvalidOperationException("schema"); } });
            Check("config", settings != null && settings.Greeting != null, liberty.Config.PathOf(owner, "selftest"));
            Settings saved = new Settings();
            saved.Greeting = "state " + DateTime.Now.Ticks;
            liberty.State.Save(owner, "selftest", saved);
            Settings loaded = liberty.State.Load<Settings>(owner, "selftest");
            Check("state", loaded != null && loaded.Greeting == saved.Greeting, null);

            // Weapons.
            ModelRef pistolModel = liberty.Weapons.ModelOf(7);
            Check("weapon-model", !pistolModel.IsNone && liberty.Streaming.IsValidModel(pistolModel), "model=" + pistolModel + " slot=" + liberty.Weapons.SlotOf(7));
            liberty.Weapons.Give(player, 7, 60);
            Check("weapons", liberty.Weapons.Has(player, 7) && liberty.Weapons.GetAmmo(player, 7) > 0, "inventory=" + liberty.Weapons.Inventory(player).Count);

            // Props: streaming, creation, placement, heading, attachment rotation units, deletion.
            int deadline = Environment.TickCount + 5000;
            while (!liberty.Streaming.RequestModel(owner, pistolModel) && Environment.TickCount < deadline) { yield return null; }
            Vec3 front = liberty.World.Player.Position + Vec3.FromHeading(liberty.World.Player.Heading) * 1.5f;
            PropRef prop = liberty.Props.TryCreate(owner, pistolModel, front);
            Check("prop-create", !prop.IsNone && liberty.Props.Exists(prop), "handle=" + prop.Handle);
            if (!prop.IsNone)
            {
                liberty.Props.SetCollision(prop, false);
                liberty.Props.SetPosition(prop, front + new Vec3(0, 0, 1));
                Vec3 at = liberty.Props.GetPosition(prop);
                Check("prop-position", at.DistanceTo(front + new Vec3(0, 0, 1)) < 0.2f, "at=" + at);
                liberty.Props.AttachToPed(prop, player, Bone.Root, Vec3.Zero, Vec3.Zero);
                yield return Wait.FramesCount(2);
                float h0 = liberty.Props.GetHeading(prop);
                liberty.Props.AttachToPed(prop, player, Bone.Root, Vec3.Zero, new Vec3(0, 0, 90));
                yield return Wait.FramesCount(2);
                float h1 = liberty.Props.GetHeading(prop);
                float delta = ((h1 - h0) % 360 + 360) % 360;
                string units = Math.Abs(delta - 90) < 3 || Math.Abs(delta - 270) < 3 ? "degrees" : Math.Abs(delta - 116.6f) < 3 || Math.Abs(delta - 243.4f) < 3 ? "radians" : "unknown";
                // The SDK converts degrees for the game, so a 90 degree request must turn the prop by 90.
                Check("attach-rotation", units == "degrees", "heading_delta=" + delta.ToString("0.0") + " (90 requested; the game itself takes radians)");
                Info("attach-rotation-units", units + " heading_delta=" + delta.ToString("0.0") + " (90 requested)");
                liberty.Props.Detach(prop);
                liberty.Props.Delete(prop);
                Check("prop-delete", !liberty.Props.Exists(prop), null);
            }
            liberty.Streaming.ReleaseModel(owner, pistolModel);

            // Peds.
            PedRef ped = PedRef.None;
            bool pedDone = false;
            liberty.Peds.SpawnRandom(owner, front + Vec3.FromHeading(liberty.World.Player.Heading) * 2f, liberty.World.Player.Heading + 180, p => { ped = p; pedDone = true; });
            yield return Wait.Until(() => pedDone, 7000);
            Check("ped-spawn", !ped.IsNone && liberty.Peds.Exists(ped), "handle=" + ped.Handle);
            if (!ped.IsNone)
            {
                liberty.Peds.SetBlockEvents(ped, true);
                liberty.Peds.SetHealth(ped, 150);
                Check("ped-health", liberty.Peds.GetHealth(ped) == 150, "health=" + liberty.Peds.GetHealth(ped));
                // A new ped's skeleton is posed on its first update: give it a few frames.
                yield return Wait.FramesCount(5);
                Vec3 head = liberty.Peds.GetBonePosition(ped, Bone.Head), feet = liberty.Peds.GetPosition(ped);
                Check("ped-bone", head.Z > feet.Z && head.DistanceTo(feet) < 2.5f, "head=" + head + " origin=" + feet);
                liberty.Weapons.Give(ped, 7, 30);
                Check("ped-weapon", liberty.Weapons.Has(ped, 7), null);
                yield return Wait.FramesCount(3);
                PedState state;
                Check("ped-snapshot", liberty.World.TryGetPed(ped, out state), "in_snapshot distance=" + state.Distance.ToString("0.0"));
                List<PedState> near = new List<PedState>();
                liberty.Query.PedsInRadius(liberty.World.Player.Position, 10f, p => !p.IsPlayer, near);
                Check("query-radius", near.Exists(p => p.Ped == ped), "peds_within_10m=" + near.Count);
                PedState coned;
                bool inCone = liberty.Query.NearestPedInCone(liberty.World.Player.Position, liberty.Peds.GetPosition(ped) - liberty.World.Player.Position, 20f, 15f, null, out coned);
                Check("query-cone", inCone && coned.Ped == ped, "found=" + coned.Ped.Handle);
                Check("query-onscreen", liberty.Query.IsSphereVisible(liberty.Peds.GetPosition(ped), 1f), "on_screen=" + liberty.Query.IsOnScreen(ped));
                liberty.Tasks.HandsUp(ped, 2000);
                yield return Wait.Milliseconds(500);
                liberty.Peds.Delete(ped);
                Check("ped-delete", !liberty.Peds.Exists(ped), null);
            }

            // Vehicles, their snapshot entry and appear/remove events.
            liberty.Events.Subscribe<VehicleAppeared>(owner, OnVehicleAppeared);
            liberty.Events.Subscribe<VehicleRemoved>(owner, OnVehicleRemoved);
            VehicleRef car = VehicleRef.None;
            bool carDone = false;
            Vec3 carSpot = liberty.World.Player.Position + Vec3.FromHeading(liberty.World.Player.Heading + 90) * 6f;
            liberty.Vehicles.Spawn(owner, "admiral", carSpot, liberty.World.Player.Heading, v => { car = v; watchedCar = v; carDone = true; });
            yield return Wait.Until(() => carDone, 7000);
            Check("vehicle-spawn", !car.IsNone && liberty.Vehicles.Exists(car), "handle=" + car.Handle);
            if (!car.IsNone)
            {
                Check("vehicle-engine", liberty.Vehicles.GetEngineHealth(car) > 0, "engine=" + liberty.Vehicles.GetEngineHealth(car) + " body=" + liberty.Vehicles.GetHealth(car));
                Vec3 nose = liberty.Vehicles.GetOffsetPosition(car, new Vec3(0, 2, 0));
                Check("vehicle-offset", nose.DistanceTo(liberty.Vehicles.GetPosition(car)) > 1.5f, "nose=" + nose);
                liberty.Vehicles.OpenDoor(car, VehicleDoor.Trunk);
                yield return Wait.Milliseconds(600);
                liberty.Vehicles.CloseDoor(car, VehicleDoor.Trunk);
                yield return Wait.Until(() => vehicleAppeared || !liberty.World.HasVehicles, 3000);
                VehicleState vs;
                bool listed = liberty.World.TryGetVehicle(car, out vs);
                if (liberty.World.HasVehicles) { Check("vehicle-snapshot", listed && vehicleAppeared, "listed=" + listed + " appeared_event=" + vehicleAppeared); }
                else { Info("vehicle-snapshot", "core vehicle list not active (vehicle natives not verified yet)"); }
                liberty.Vehicles.Delete(car);
                Check("vehicle-delete", !liberty.Vehicles.Exists(car), null);
                if (liberty.World.HasVehicles)
                {
                    yield return Wait.Until(() => vehicleRemoved, 3000);
                    Check("vehicle-removed-event", vehicleRemoved, null);
                }
            }

            // Effects, sound, blips.
            Vec3 fxAt = liberty.World.Player.Position + Vec3.FromHeading(liberty.World.Player.Heading) * 3f;
            Info("fx-burst", "blood_gun_entry=" + liberty.Fx.Burst("blood_gun_entry", fxAt, Vec3.Zero, 1f));
            SoundRef sound = liberty.Audio.PlayAt(owner, "GENERIC_CURSE", fxAt);
            Check("audio-id", !sound.IsNone, "sound=" + sound.Handle);
            liberty.Audio.Stop(sound);
            BlipRef blip = liberty.Blips.AddForPosition(owner, fxAt);
            Check("blip", !blip.IsNone, "blip=" + blip.Handle);
            liberty.Blips.SetName(blip, "Liberty self-test");
            yield return Wait.Milliseconds(300);
            liberty.Blips.Remove(blip);

            // Camera.
            CameraRef camera = liberty.Cameras.Create(owner);
            Check("camera-create", !camera.IsNone, null);
            float fov = liberty.Cameras.GameCameraFov;
            Check("game-camera", fov > 10 && fov < 120, "fov=" + fov.ToString("0.0") + " position=" + liberty.Cameras.GameCameraPosition);
            if (!camera.IsNone)
            {
                Vec3 eye = liberty.World.Player.Position + Vec3.FromHeading(liberty.World.Player.Heading) * 3f + new Vec3(0, 0, 1);
                liberty.Cameras.SetPosition(camera, eye);
                liberty.Cameras.PointAt(camera, player);
                liberty.Cameras.Activate(camera);
                yield return Wait.Milliseconds(800);
                liberty.Cameras.Destroy(camera);
            }

            // UI: help, notification, subtitle, list and radial menus open and close.
            liberty.Ui.ShowHelp(owner, "Liberty SDK self-test", 3000);
            liberty.Ui.Notify("SDK self-test running", 3000);
            liberty.Ui.Subtitle("Subtitles come from Liberty.Ui", 2500);
            ListMenu list = new ListMenu();
            list.Title = "SELF-TEST";
            list.Items = () => new List<ListItem> { ListItem.Info(() => "frame " + liberty.Perf.FrameMs.ToString("0.0") + " ms"), ListItem.Action("Close", () => null) };
            IMenu menu = liberty.Ui.OpenList(owner, list);
            yield return Wait.Milliseconds(1200);
            Check("ui-list", menu.IsOpen && liberty.Ui.AnyMenuOpen && liberty.Input.CapturedBy == owner, null);
            menu.Close();
            Check("ui-list-close", !menu.IsOpen && !liberty.Ui.AnyMenuOpen, null);
            RadialMenu radial = new RadialMenu();
            radial.Title = "SELF-TEST";
            for (int i = 0; i < 6; i++) { int n = i; RadialSegment s = new RadialSegment(); s.Label = () => "Slot " + n; radial.Segments.Add(s); }
            radial.CenterLines = s => new[] { "SEGMENT", "Slot " + s, "B Close" };
            IMenu wheel = liberty.Ui.OpenRadial(owner, radial);
            yield return Wait.Milliseconds(1200);
            Check("ui-radial", wheel.IsOpen, null);
            wheel.Close();

            // Choreography on the player: turn, play a clip, a timed callback, completion.
            bool timedFired = false, completed = false;
            Vec3 lookAt = liberty.World.Player.Position + Vec3.FromHeading(liberty.World.Player.Heading + 90) * 5f;
            IChoreography sequence = liberty.Animation.Choreography(owner, "selftest")
                .TurnTo(player, lookAt, 600)
                .At(200, () => timedFired = true)
                .Play(player, new AnimClip("amb@car_stash", "idle"), AnimOptions.Default, 300, 1500)
                .OnComplete(() => completed = true)
                .Begin();
            yield return Wait.Until(() => !sequence.IsRunning, 6000);
            Check("choreography", completed && timedFired && sequence.Completed, "step=" + sequence.StepIndex);
            liberty.Animation.StopAll(player);

            liberty.Log.Info(owner, "selftest_done passed=" + passed + " failed=" + failed);
        }

        private void OnVehicleAppeared(VehicleAppeared e) { if (e.Vehicle == watchedCar) { vehicleAppeared = true; } }
        private void OnVehicleRemoved(VehicleRemoved e) { if (e.Vehicle == watchedCar) { vehicleRemoved = true; } }

        private bool Refused(Action action)
        {
            try { action(); return false; }
            catch (UnauthorizedAccessException) { return true; }
            catch (Exception error) { liberty.Log.Error(owner, "selftest unexpected " + error.GetType().Name + ": " + error.Message); return false; }
        }
    }
}
