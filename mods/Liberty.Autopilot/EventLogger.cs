using Liberty.Sdk;
using Liberty.Sdk.Events;

namespace Liberty.Autopilot
{
    // Logs every SDK game event as "event <Name> ..." while enabled; the autopilot's expect lines match these.
    internal sealed class EventLogger
    {
        private readonly LibertyModule owner;
        private readonly ILiberty liberty;
        internal bool Enabled;

        internal EventLogger(LibertyModule owner, ILiberty liberty) { this.owner = owner; this.liberty = liberty; }

        private void Log(string text) { if (Enabled) { liberty.Log.Info(owner, "event " + text); } }

        internal void Subscribe()
        {
            IEventBus bus = liberty.Events;
            bus.Subscribe<PedAppeared>(owner, e => Log("PedAppeared handle=" + e.Ped.Handle));
            bus.Subscribe<PedRemoved>(owner, e => Log("PedRemoved handle=" + e.Ped.Handle));
            bus.Subscribe<PedDamaged>(owner, e => Log("PedDamaged handle=" + e.Ped.Handle + " " + e.HealthBefore + "->" + e.HealthAfter +
                " bone=0x" + ((int)e.Bone).ToString("X") + " by_player=" + e.ByPlayer + " weapon=" + e.Weapon + " exact=" + e.Exact + " type=" + e.Type +
                " amount=" + e.Amount.ToString("0.0") + " health_lost=" + e.HealthLost.ToString("0.0") + " armour_lost=" + e.ArmourLost.ToString("0.0") +
                " attacker=" + e.Attacker.Handle + " vehicle=" + e.AttackerVehicle.Handle + " killed=" + e.Killed + " hit=" + e.HasHit + (e.HasHit ? " at=" + e.HitPosition + " dir=" + e.HitDirection : "")));
            bus.Subscribe<PedDied>(owner, e => Log("PedDied handle=" + e.Ped.Handle + " bone=0x" + ((int)e.Bone).ToString("X") + " by_player=" + e.ByPlayer +
                " exact=" + e.Exact + " type=" + e.Type + " killer=" + e.Killer.Handle + " weapon=" + e.Weapon));
            bus.Subscribe<PlayerShot>(owner, e => Log("PlayerShot weapon=" + e.Weapon + " clip " + e.ClipBefore + "->" + e.ClipAfter));
            bus.Subscribe<ReloadStarted>(owner, e => Log("ReloadStarted weapon=" + e.Weapon + " clip_before=" + e.ClipBefore));
            bus.Subscribe<ReloadFinished>(owner, e => Log("ReloadFinished weapon=" + e.Weapon + " clip_after=" + e.ClipAfter));
            bus.Subscribe<PlayerWeaponChanged>(owner, e => Log("PlayerWeaponChanged " + e.Previous + "->" + e.Current));
            bus.Subscribe<PlayerEnteredVehicle>(owner, e => Log("PlayerEnteredVehicle " + e.Vehicle.Handle));
            bus.Subscribe<PlayerExitedVehicle>(owner, e => Log("PlayerExitedVehicle " + e.Vehicle.Handle));
            bus.Subscribe<PlayerDamaged>(owner, e => Log("PlayerDamaged " + e.HealthBefore + "->" + e.HealthAfter));
            bus.Subscribe<PlayerDied>(owner, e => Log("PlayerDied"));
            bus.Subscribe<VehicleAppeared>(owner, e => Log("VehicleAppeared handle=" + e.Vehicle.Handle));
            bus.Subscribe<VehicleRemoved>(owner, e => Log("VehicleRemoved handle=" + e.Vehicle.Handle));
            bus.Subscribe<VehicleDamaged>(owner, e => Log("VehicleDamaged handle=" + e.Vehicle.Handle + " " + e.HealthBefore + "->" + e.HealthAfter +
                " engine " + e.EngineBefore + "->" + e.EngineAfter));
            bus.Subscribe<VehicleDestroyed>(owner, e => Log("VehicleDestroyed handle=" + e.Vehicle.Handle));
            bus.Subscribe<BulletFired>(owner, e => Log("BulletFired shooter=" + e.Shooter.Handle + " weapon=" + e.Weapon + " by_player=" + e.ByPlayer +
                " from=" + e.From + " to=" + e.To));
            bus.Subscribe<WeatherChanged>(owner, e => Log("WeatherChanged " + e.Previous + "->" + e.Current));
            bus.Subscribe<PauseChanged>(owner, e => Log("PauseChanged " + e.Paused));
            bus.Subscribe<FadeChanged>(owner, e => Log("FadeChanged " + e.FadedOut));
            bus.Subscribe<MissionChanged>(owner, e => Log("MissionChanged " + e.Active));
            bus.Subscribe<CutsceneChanged>(owner, e => Log("CutsceneChanged " + e.Playing));
            bus.Subscribe<ModuleFailed>(owner, e => liberty.Log.Error(owner, "autopilot_saw_module_failure " + e.ModuleId + " " + e.Error));
        }
    }
}
