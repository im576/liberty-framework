using System;
using GTA;
using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;
using Liberty.Sdk.Events;
using Bone = Liberty.Sdk.Bone;
using LibertyFramework.Core.Logging;
using LibertyFramework.Engine.Core;
using LibertyFramework.Engine.Events;

namespace LibertyFramework.Engine.World
{
    // Fills WorldState each frame from the native core, or from ScriptHookDotNet when the core is unavailable or was
    // switched off by a safety check (fallback: player and world flags only, no ped/vehicle lists or their events).
    // Publishes the SDK game events (Liberty.Sdk.Events) from the core's change list.
    internal sealed unsafe class WorldBuilder
    {
        private readonly CoreBridge core;
        private readonly WorldState state;
        private readonly EventBus events;
        private readonly EngineConfig config;
        private bool verified;
        private bool vehicleChecked;
        private bool vehicleNativesDone;
        private int vehicleSearchMs;
        private int spotCheckCountdown;
        private int flagsPolledMs;
        private bool missionActive, cutscenePlaying, flagsKnown;
        // Exact damage (ADR-0007): the frame each ped last got an exact record / an exact kill, so the inferred events for
        // the same damage are not published twice.
        private readonly Dictionary<int, int> exactDamageFrame = new Dictionary<int, int>();
        private readonly Dictionary<int, int> exactKillFrame = new Dictionary<int, int>();
        internal Episode Episode;

        internal WorldBuilder(CoreBridge core, WorldState state, EventBus events, EngineConfig config)
        {
            this.core = core; this.state = state; this.events = events; this.config = config;
            spotCheckCountdown = config.CoreSpotCheckFrames;
        }

        internal bool UsingCore { get { return core.Available && verified; } }

        internal void Build(int frame)
        {
            state.Frame = frame;
            GTA.Player player = Game.LocalPlayer;
            Ped ped = player != null ? player.Character : null;
            if (core.Available && !verified && ped != null && ped.Exists())
            {
                verified = CoreVerifier.VerifyAll(core, player, ped) > 0;
                if (!verified) { RuntimeLog.Error("engine_core_rejected every native failed verification; using fallback"); core.Shutdown(); }
            }
            PollFlags();
            if (UsingCore && ped != null && FromCore(ped)) { return; }
            Fallback(ped);
        }

        // Mission and cutscene state change rarely; two SHDN calls twice a second (NATIVES.md T-020).
        private void PollFlags()
        {
            int now = Environment.TickCount;
            if (flagsKnown && unchecked(now - flagsPolledMs) < 500) { return; }
            flagsPolledMs = now;
            bool mission, cutscene;
            try
            {
                mission = Function.Call<bool>("GET_MISSION_FLAG");
                cutscene = Function.Call<bool>("HAS_CUTSCENE_LOADED") && !Function.Call<bool>("HAS_CUTSCENE_FINISHED");
            }
            catch (Exception error) { RuntimeLog.Error("engine_flags_failed error=" + error.Message); return; }
            if (flagsKnown && mission != missionActive) { events.Publish(new MissionChanged { Active = mission }); }
            if (flagsKnown && cutscene != cutscenePlaying) { events.Publish(new CutsceneChanged { Playing = cutscene }); }
            missionActive = mission;
            cutscenePlaying = cutscene;
            flagsKnown = true;
        }

        private bool FromCore(Ped ped)
        {
            uint parts = CoreAbi.ValidWorld | CoreAbi.ValidPlayer | CoreAbi.ValidPeds | CoreAbi.ValidVehicles;
            if (config.BulletEvents) { parts |= CoreAbi.ValidBullets; }
            if (config.ExactDamage) { parts |= CoreAbi.ValidDamage; }
            LcSnapshotHead* head = core.Frame(ped.GetHashCode(), config.PedRadiusMeters, config.VehicleRadiusMeters, parts);
            core.ReportFaults();
            if (head == null) { return false; }
            if (head->ParkedViolations != 0)
            {
                // The engine frame counter moved while the core called into the game: our thread was not serialized with
                // the game thread. Direct calls are unsafe from here on.
                RuntimeLog.Error("engine_core_unsafe parked_violations=" + head->ParkedViolations + "; core disabled for this session");
                core.Shutdown();
                return false;
            }
            state.FromCore = true;
            state.CoreMicroseconds = head->CoreMicroseconds;
            state.HasWorld = (head->Valid & CoreAbi.ValidWorld) != 0;
            state.HasPlayer = (head->Valid & CoreAbi.ValidPlayer) != 0;
            state.HasPeds = (head->Valid & CoreAbi.ValidPeds) != 0;
            state.HasVehicles = (head->Valid & CoreAbi.ValidVehicles) != 0;
            state.Pools = head->Pools;
            if (state.HasWorld) { state.Info = World(head->World); }
            if (state.HasPlayer)
            {
                state.Player = Player(head->Player);
                if (state.Player.InVehicle && !vehicleChecked) { vehicleChecked = true; CoreVerifier.VerifyVehicle(core, ped); }
            }
            if (!vehicleNativesDone) { TryVerifyVehicleNatives(ped); }
            state.ClearPeds();
            LcPed* peds = CoreBridge.Peds(head);
            for (int i = 0; i < head->PedCount; i++) { state.AddPed(Ped(peds[i])); }
            state.ClearVehicles();
            int vehicleCount = CoreBridge.VehicleCount(head);
            LcVehicle* vehicles = CoreBridge.Vehicles(head);
            for (int i = 0; i < vehicleCount; i++) { state.AddVehicle(Vehicle(vehicles[i])); }
            if (state.HasPlayer && --spotCheckCountdown <= 0)
            {
                spotCheckCountdown = config.CoreSpotCheckFrames;
                Vector3 actual = ped.Position;
                float drift = (state.Player.Position - new Vec3(actual.X, actual.Y, actual.Z)).Length;
                if (drift > config.CoreSpotCheckToleranceMeters)
                {
                    RuntimeLog.Error("engine_core_drift player_position_error_m=" + drift.ToString("0.000") + "; core disabled for this session");
                    core.Shutdown();
                    return false;
                }
            }
            PublishBullets(head);
            PublishDamages(head);
            Publish(head);
            return true;
        }

        // Vehicle natives are compared with SHDN on a real vehicle once one is near (every 5 s until then).
        private void TryVerifyVehicleNatives(Ped ped)
        {
            int now = Environment.TickCount;
            if (vehicleSearchMs != 0 && unchecked(now - vehicleSearchMs) < 5000) { return; }
            vehicleSearchMs = now;
            try
            {
                GTA.Vehicle vehicle = ped.isInVehicle() ? ped.CurrentVehicle : GTA.World.GetClosestVehicle(ped.Position, 60f);
                if (vehicle == null || !vehicle.Exists()) { return; }
                vehicleNativesDone = true;
                if (!CoreVerifier.VerifyVehicleNatives(core, vehicle)) { RuntimeLog.Error("engine_core_vehicles_off verification failed; vehicle list stays empty"); }
            }
            catch (Exception error)
            {
                vehicleNativesDone = true;
                RuntimeLog.Error("engine_core_vehicle_verify_failed error=" + error.Message);
            }
        }

        private void PublishBullets(LcSnapshotHead* head)
        {
            int count = CoreBridge.BulletCount(head);
            if (count == 0 || !events.HasSubscribers<BulletFired>()) { return; }
            LcBullet* list = CoreBridge.Bullets(head);
            int player = state.HasPlayer ? state.Player.Ped.Handle : 0;
            for (int i = 0; i < count; i++)
            {
                LcBullet b = list[i];
                events.Publish(new BulletFired
                {
                    Shooter = new PedRef(b.Shooter), Weapon = b.Weapon, From = new Vec3(b.FromX, b.FromY, b.FromZ),
                    To = new Vec3(b.ToX, b.ToY, b.ToZ), ByPlayer = b.Shooter != 0 && b.Shooter == player
                });
            }
        }

        private void PublishDamages(LcSnapshotHead* head)
        {
            int frame = state.Frame;
            if (frame % 300 == 0) { Forget(exactDamageFrame, frame); Forget(exactKillFrame, frame); }
            int count = CoreBridge.DamageCount(head);
            if (count == 0) { return; }
            LcDamage* list = CoreBridge.Damages(head);
            LcBullet* bullets = CoreBridge.Bullets(head);
            int bulletCount = CoreBridge.BulletCount(head);
            int player = state.HasPlayer ? state.Player.Ped.Handle : 0;
            for (int i = 0; i < count; i++)
            {
                LcDamage d = list[i];
                if (d.Victim == 0) { continue; }
                exactDamageFrame[d.Victim] = frame;
                PedRef attacker = d.AttackerKind == 1 ? new PedRef(d.Attacker) : PedRef.None;
                VehicleRef attackerVehicle = d.AttackerKind == 2 ? new VehicleRef(d.Attacker) : VehicleRef.None;
                VehicleState vehicle;
                // A vehicle's damage belongs to its driver when there is one.
                if (!attackerVehicle.IsNone && state.TryGetVehicle(attackerVehicle, out vehicle) && !vehicle.Driver.IsNone) { attacker = vehicle.Driver; }
                DamageType type = DamageTypes.Classify(d.Weapon, d.AttackerKind, Episode);
                PedState victim;
                bool inSnapshot = state.TryGetPed(new PedRef(d.Victim), out victim);
                int after = inSnapshot ? victim.Health : (d.Victim == player && state.HasPlayer ? state.Player.Health : 0);
                PedDamaged e = new PedDamaged();
                e.Ped = new PedRef(d.Victim); e.Attacker = attacker; e.AttackerVehicle = attackerVehicle; e.Weapon = d.Weapon; e.Type = type;
                e.Bone = (Bone)d.Bone; e.Component = d.Component; e.Amount = d.Amount; e.HealthLost = d.HealthLost; e.ArmourLost = d.ArmourLost;
                e.HealthAfter = after; e.HealthBefore = after + (int)Math.Round(d.HealthLost);
                e.ByPlayer = player != 0 && attacker.Handle == player; e.Exact = true;
                // The game marks a ped dead a few frames after the lethal hit, so the blow that takes health to 0 or below (this frame's
                // snapshot, read after the damage) is the kill; later hits on the body are not.
                bool lethal = (d.Flags & CoreAbi.DamageKilled) != 0 || ((inSnapshot || d.Victim == player) && after <= 0);
                e.Killed = lethal && !Recent(exactKillFrame, d.Victim, 600);
                if (type == DamageType.Bullet && !attacker.IsNone && (inSnapshot || d.Victim == player))
                {
                    Vec3 at = inSnapshot ? victim.Position : state.Player.Position;
                    FindHit(bullets, bulletCount, attacker.Handle, at, ref e);
                }
                events.Publish(e);
                if (!e.Killed) { continue; }
                exactKillFrame[d.Victim] = frame;
                events.Publish(new PedDied
                {
                    Ped = e.Ped, Killer = attacker, KillerVehicle = attackerVehicle, Weapon = d.Weapon, Bone = e.Bone, ByPlayer = e.ByPlayer,
                    Exact = true, Type = type
                });
            }
        }

        // The attacker's bullet trace this frame that ended nearest the victim (within 2.5 m of the ped origin).
        private static void FindHit(LcBullet* bullets, int count, int shooter, Vec3 victim, ref PedDamaged e)
        {
            float best = 2.5f * 2.5f;
            for (int i = 0; i < count; i++)
            {
                if (bullets[i].Shooter != shooter) { continue; }
                Vec3 to = new Vec3(bullets[i].ToX, bullets[i].ToY, bullets[i].ToZ);
                float d = (to - victim).LengthSquared;
                if (d > best) { continue; }
                best = d;
                e.HasHit = true;
                e.HitPosition = to;
                e.HitDirection = (to - new Vec3(bullets[i].FromX, bullets[i].FromY, bullets[i].FromZ)).Normalized;
            }
        }

        private static void Forget(Dictionary<int, int> frames, int now)
        {
            List<int> old = null;
            foreach (KeyValuePair<int, int> pair in frames) { if (now - pair.Value > 300) { (old = old ?? new List<int>()).Add(pair.Key); } }
            if (old != null) { foreach (int key in old) { frames.Remove(key); } }
        }

        private bool Recent(Dictionary<int, int> frames, int ped, int window)
        {
            int at;
            return frames.TryGetValue(ped, out at) && state.Frame - at <= window;
        }

        private void Publish(LcSnapshotHead* head)
        {
            int count = CoreBridge.EventCount(head);
            if (CoreBridge.EventsDropped(head) > 0) { RuntimeLog.Error("engine_events_dropped count=" + CoreBridge.EventsDropped(head)); }
            LcEvent* list = CoreBridge.Events(head);
            PedRef player = state.HasPlayer ? state.Player.Ped : PedRef.None;
            int playerWeapon = state.HasPlayer ? state.Player.Weapon : -1;
            for (int i = 0; i < count; i++)
            {
                LcEvent e = list[i];
                switch (e.Type)
                {
                    case CoreAbi.EvPedAppeared: events.Publish(new PedAppeared { Ped = new PedRef(e.A) }); break;
                    case CoreAbi.EvPedRemoved: events.Publish(new PedRemoved { Ped = new PedRef(e.A) }); break;
                    case CoreAbi.EvPedDamaged:
                        if (Recent(exactDamageFrame, e.A, 2)) { break; }
                        events.Publish(new PedDamaged
                        {
                            Ped = new PedRef(e.A), HealthBefore = e.B - 100, HealthAfter = e.C - 100, Bone = (Bone)e.D, ByPlayer = e.E != 0,
                            Attacker = e.E != 0 ? player : PedRef.None, Weapon = e.E != 0 ? playerWeapon : -1, Exact = false
                        });
                        break;
                    case CoreAbi.EvPedDied:
                        if (Recent(exactKillFrame, e.A, 120)) { break; }
                        events.Publish(new PedDied
                        {
                            Ped = new PedRef(e.A), Bone = (Bone)e.D, ByPlayer = e.E != 0, Killer = e.E != 0 ? player : PedRef.None,
                            Weapon = e.E != 0 ? playerWeapon : -1
                        });
                        break;
                    case CoreAbi.EvPlayerShot: events.Publish(new PlayerShot { Weapon = e.A, ClipBefore = e.B, ClipAfter = e.C }); break;
                    case CoreAbi.EvPlayerReloadStarted: events.Publish(new ReloadStarted { Ped = player, Weapon = e.A, ClipBefore = e.B }); break;
                    case CoreAbi.EvPlayerReloaded: events.Publish(new ReloadFinished { Ped = player, Weapon = e.A, ClipAfter = e.C }); break;
                    case CoreAbi.EvPlayerWeapon: events.Publish(new PlayerWeaponChanged { Previous = e.A, Current = e.B }); break;
                    case CoreAbi.EvPlayerEnteredVehicle: events.Publish(new PlayerEnteredVehicle { Vehicle = new VehicleRef(e.A) }); break;
                    case CoreAbi.EvPlayerExitedVehicle: events.Publish(new PlayerExitedVehicle { Vehicle = new VehicleRef(e.A) }); break;
                    case CoreAbi.EvPlayerDied: events.Publish(new PlayerDied()); break;
                    case CoreAbi.EvPlayerDamaged: events.Publish(new PlayerDamaged { HealthBefore = e.B - 100, HealthAfter = e.C - 100, ArmourBefore = e.D, ArmourAfter = e.E }); break;
                    case CoreAbi.EvWeatherChanged: events.Publish(new WeatherChanged { Previous = e.A, Current = e.B }); break;
                    case CoreAbi.EvPauseChanged: events.Publish(new PauseChanged { Paused = e.A != 0 }); break;
                    case CoreAbi.EvFadeChanged: events.Publish(new FadeChanged { FadedOut = e.A != 0 }); break;
                    case CoreAbi.EvVehicleAppeared: events.Publish(new VehicleAppeared { Vehicle = new VehicleRef(e.A) }); break;
                    case CoreAbi.EvVehicleRemoved: events.Publish(new VehicleRemoved { Vehicle = new VehicleRef(e.A) }); break;
                    case CoreAbi.EvVehicleDamaged:
                        events.Publish(new VehicleDamaged { Vehicle = new VehicleRef(e.A), HealthBefore = e.B, HealthAfter = e.C, EngineBefore = e.D, EngineAfter = e.E });
                        break;
                    case CoreAbi.EvVehicleDestroyed: events.Publish(new VehicleDestroyed { Vehicle = new VehicleRef(e.A) }); break;
                }
            }
        }

        private WorldInfo World(LcWorld w)
        {
            WorldInfo info = new WorldInfo();
            info.GameTimerMs = w.GameTimerMs; info.Hours = w.Hours; info.Minutes = w.Minutes; info.Weather = w.Weather;
            info.Paused = (w.Flags & CoreAbi.WorldPaused) != 0; info.FadedOut = (w.Flags & CoreAbi.WorldFadedOut) != 0;
            info.MissionActive = missionActive; info.CutscenePlaying = cutscenePlaying;
            return info;
        }

        private static PlayerState Player(LcPlayer p)
        {
            PlayerState player = new PlayerState();
            player.Index = p.Index; player.Ped = new PedRef(p.Ped); player.Position = new Vec3(p.X, p.Y, p.Z); player.Heading = p.Heading;
            player.Health = p.Health - 100; player.Armour = p.Armour; player.Weapon = p.Weapon; player.AmmoInClip = p.AmmoInClip;
            player.Vehicle = new VehicleRef(p.Vehicle);
            player.IsPlaying = (p.Flags & CoreAbi.PlayerPlaying) != 0; player.HasControl = (p.Flags & CoreAbi.PlayerControl) != 0;
            player.IsDead = (p.Flags & CoreAbi.PlayerDead) != 0; player.InVehicle = (p.Flags & CoreAbi.PlayerInVehicle) != 0;
            player.IsReloading = (p.Flags & CoreAbi.PlayerReloading) != 0;
            return player;
        }

        private static PedState Ped(LcPed p)
        {
            PedState ped = new PedState();
            ped.Ped = new PedRef(p.Handle); ped.Model = ModelRef.FromHash((int)p.Model); ped.Position = new Vec3(p.X, p.Y, p.Z); ped.Heading = p.Heading;
            ped.Health = p.Health - 100; ped.Armour = p.Armour; ped.Vehicle = new VehicleRef(p.Vehicle); ped.Distance = p.Distance;
            ped.IsDead = (p.Flags & CoreAbi.PedDead) != 0; ped.InVehicle = (p.Flags & CoreAbi.PedInVehicle) != 0;
            ped.IsPlayer = (p.Flags & CoreAbi.PedPlayer) != 0; ped.IsNew = (p.Flags & CoreAbi.PedNew) != 0;
            return ped;
        }

        private static VehicleState Vehicle(LcVehicle v)
        {
            VehicleState vehicle = new VehicleState();
            vehicle.Vehicle = new VehicleRef(v.Handle); vehicle.Model = ModelRef.FromHash((int)v.Model); vehicle.Position = new Vec3(v.X, v.Y, v.Z);
            vehicle.Heading = v.Heading; vehicle.Speed = v.Speed; vehicle.Health = v.Health; vehicle.EngineHealth = v.EngineHealth;
            vehicle.Driver = new PedRef(v.Driver); vehicle.IsNew = (v.Flags & CoreAbi.VehicleNew) != 0; vehicle.Distance = v.Distance;
            return vehicle;
        }

        private void Fallback(Ped ped)
        {
            state.FromCore = false;
            state.HasPeds = false;
            state.HasVehicles = false;
            state.ClearPeds();
            state.ClearVehicles();
            state.HasWorld = false;
            WorldInfo info = state.Info;
            info.MissionActive = missionActive; info.CutscenePlaying = cutscenePlaying;
            state.Info = info;
            if (ped == null || !ped.Exists()) { state.HasPlayer = false; return; }
            try
            {
                PlayerState player = new PlayerState();
                player.Ped = new PedRef(ped.GetHashCode());
                Vector3 position = ped.Position;
                player.Position = new Vec3(position.X, position.Y, position.Z);
                player.Heading = ped.Heading;
                player.Health = ped.Health;
                player.Armour = ped.Armor;
                player.Weapon = (int)ped.Weapons.CurrentType;
                player.AmmoInClip = -1;
                player.IsDead = ped.isDead;
                player.InVehicle = ped.isInVehicle();
                player.IsPlaying = true;
                state.Player = player;
                state.HasPlayer = true;
            }
            catch (Exception error)
            {
                state.HasPlayer = false;
                RuntimeLog.Error("engine_fallback_failed error=" + error.Message);
            }
        }
    }
}
