using System;
using GTA;
using LibertyFramework.Core.Logging;
using LibertyFramework.Engine.Core;
using LibertyFramework.Engine.Events;

namespace LibertyFramework.Engine.World
{
    // Fills WorldState each frame from the native core, or from ScriptHookDotNet when the core is unavailable or was
    // switched off by a safety check (fallback: player and world flags only, no ped list or ped events).
    internal sealed unsafe class WorldBuilder
    {
        private readonly CoreBridge core;
        private readonly WorldState state;
        private readonly EventBus events;
        private readonly EngineConfig config;
        private bool verified;
        private bool vehicleChecked;
        private int spotCheckCountdown;

        internal WorldBuilder(CoreBridge core, WorldState state, EventBus events, EngineConfig config)
        {
            this.core = core; this.state = state; this.events = events; this.config = config;
            spotCheckCountdown = config.CoreSpotCheckFrames;
        }

        internal bool UsingCore { get { return core.Available && verified; } }

        internal void Build(int frame)
        {
            state.Frame = frame;
            Player player = Game.LocalPlayer;
            Ped ped = player != null ? player.Character : null;
            if (core.Available && !verified && ped != null && ped.Exists())
            {
                verified = CoreVerifier.VerifyAll(core, player, ped) > 0;
                if (!verified) { RuntimeLog.Error("engine_core_rejected every native failed verification; using fallback"); core.Shutdown(); }
            }
            if (UsingCore && ped != null && FromCore(ped)) { return; }
            Fallback(ped);
        }

        private bool FromCore(Ped ped)
        {
            LcSnapshotHead* head = core.Frame(ped.GetHashCode(), config.PedRadiusMeters, CoreAbi.ValidWorld | CoreAbi.ValidPlayer | CoreAbi.ValidPeds);
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
            if (state.HasWorld) { state.Info = World(head->World); }
            if (state.HasPlayer)
            {
                state.Player = Player(head->Player);
                if (state.Player.InVehicle && !vehicleChecked) { vehicleChecked = true; CoreVerifier.VerifyVehicle(core, ped); }
            }
            state.ClearPeds();
            LcPed* peds = CoreBridge.Peds(head);
            for (int i = 0; i < head->PedCount; i++) { state.AddPed(Ped(peds[i])); }
            if (state.HasPlayer && --spotCheckCountdown <= 0)
            {
                spotCheckCountdown = config.CoreSpotCheckFrames;
                float drift = (state.Player.Position - ped.Position).Length();
                if (drift > config.CoreSpotCheckToleranceMeters)
                {
                    RuntimeLog.Error("engine_core_drift player_position_error_m=" + drift.ToString("0.000") + "; core disabled for this session");
                    core.Shutdown();
                    return false;
                }
            }
            Publish(head);
            return true;
        }

        private void Publish(LcSnapshotHead* head)
        {
            int count = CoreBridge.EventCount(head);
            if (CoreBridge.EventsDropped(head) > 0) { RuntimeLog.Error("engine_events_dropped count=" + CoreBridge.EventsDropped(head)); }
            LcEvent* list = CoreBridge.Events(head);
            for (int i = 0; i < count; i++)
            {
                LcEvent e = list[i];
                switch (e.Type)
                {
                    case CoreAbi.EvPedAppeared: events.Publish(new PedAppeared { Handle = e.A }); break;
                    case CoreAbi.EvPedRemoved: events.Publish(new PedRemoved { Handle = e.A }); break;
                    case CoreAbi.EvPedDamaged: events.Publish(new PedDamaged { Handle = e.A, HealthBefore = e.B - 100, HealthAfter = e.C - 100, Bone = e.D, ByPlayer = e.E != 0 }); break;
                    case CoreAbi.EvPedDied: events.Publish(new PedDied { Handle = e.A, Bone = e.D, ByPlayer = e.E != 0 }); break;
                    case CoreAbi.EvPlayerShot: events.Publish(new PlayerShot { Weapon = e.A, ClipBefore = e.B, ClipAfter = e.C }); break;
                    case CoreAbi.EvPlayerReloaded: events.Publish(new PlayerReloaded { Weapon = e.A, ClipBefore = e.B, ClipAfter = e.C }); break;
                    case CoreAbi.EvPlayerWeapon: events.Publish(new PlayerWeaponChanged { Previous = e.A, Current = e.B }); break;
                    case CoreAbi.EvPlayerEnteredVehicle: events.Publish(new PlayerEnteredVehicle { Vehicle = e.A }); break;
                    case CoreAbi.EvPlayerExitedVehicle: events.Publish(new PlayerExitedVehicle { Vehicle = e.A }); break;
                    case CoreAbi.EvPlayerDied: events.Publish(new PlayerDied()); break;
                    case CoreAbi.EvPlayerDamaged: events.Publish(new PlayerDamaged { HealthBefore = e.B - 100, HealthAfter = e.C - 100, ArmourBefore = e.D, ArmourAfter = e.E }); break;
                    case CoreAbi.EvWeatherChanged: events.Publish(new WeatherChanged { Previous = e.A, Current = e.B }); break;
                    case CoreAbi.EvPauseChanged: events.Publish(new PauseChanged { Paused = e.A != 0 }); break;
                    case CoreAbi.EvFadeChanged: events.Publish(new FadeChanged { FadedOut = e.A != 0 }); break;
                }
            }
        }

        private static WorldInfo World(LcWorld w)
        {
            WorldInfo info = new WorldInfo();
            info.GameTimerMs = w.GameTimerMs; info.Hours = w.Hours; info.Minutes = w.Minutes; info.Weather = w.Weather;
            info.Paused = (w.Flags & CoreAbi.WorldPaused) != 0; info.FadedOut = (w.Flags & CoreAbi.WorldFadedOut) != 0;
            return info;
        }

        private static PlayerState Player(LcPlayer p)
        {
            PlayerState player = new PlayerState();
            player.Index = p.Index; player.Ped = p.Ped; player.Position = new Vector3(p.X, p.Y, p.Z); player.Heading = p.Heading;
            player.Health = p.Health - 100; player.Armour = p.Armour; player.Weapon = p.Weapon; player.AmmoInClip = p.AmmoInClip;
            player.Vehicle = p.Vehicle;
            player.IsPlaying = (p.Flags & CoreAbi.PlayerPlaying) != 0; player.HasControl = (p.Flags & CoreAbi.PlayerControl) != 0;
            player.IsDead = (p.Flags & CoreAbi.PlayerDead) != 0; player.InVehicle = (p.Flags & CoreAbi.PlayerInVehicle) != 0;
            return player;
        }

        private static PedState Ped(LcPed p)
        {
            PedState ped = new PedState();
            ped.Handle = p.Handle; ped.Model = (int)p.Model; ped.Position = new Vector3(p.X, p.Y, p.Z); ped.Heading = p.Heading;
            ped.Health = p.Health - 100; ped.Armour = p.Armour; ped.Vehicle = p.Vehicle; ped.Distance = p.Distance;
            ped.IsDead = (p.Flags & CoreAbi.PedDead) != 0; ped.InVehicle = (p.Flags & CoreAbi.PedInVehicle) != 0;
            ped.IsPlayer = (p.Flags & CoreAbi.PedPlayer) != 0; ped.IsNew = (p.Flags & CoreAbi.PedNew) != 0;
            return ped;
        }

        private void Fallback(Ped ped)
        {
            state.FromCore = false;
            state.HasPeds = false;
            state.ClearPeds();
            state.HasWorld = false;
            if (ped == null || !ped.Exists()) { state.HasPlayer = false; return; }
            try
            {
                PlayerState player = new PlayerState();
                player.Ped = ped.GetHashCode();
                player.Position = ped.Position;
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