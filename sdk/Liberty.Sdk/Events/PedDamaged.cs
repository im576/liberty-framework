namespace Liberty.Sdk.Events
{
    // A ped lost health or armour (the player included). Exact is true when the engine observed the game's own damage
    // routine: then attacker (a ped, or AttackerVehicle for vehicle damage), weapon, type, bone, amounts and Killed are
    // authoritative for every ped in the world, and HasHit/HitPosition/HitDirection give the bullet's impact when a
    // bullet from the attacker ended near the victim this frame. Without the observer (Exact false) the engine infers
    // damage from health changes of peds in the snapshot, and attacker/weapon/bone come from the game's last-damage records.
    public struct PedDamaged
    {
        public PedRef Ped;
        public PedRef Attacker;
        public int Weapon;
        public Bone Bone;
        public int HealthBefore;
        public int HealthAfter;
        public bool ByPlayer;
        public bool Exact;
        // SDK 1.0 (exact damage):
        public VehicleRef AttackerVehicle;
        public DamageType Type;
        public float Amount;
        public float HealthLost;
        public float ArmourLost;
        public bool Killed;
        public int Component;
        public bool HasHit;
        public Vec3 HitPosition;
        public Vec3 HitDirection;
    }
}
