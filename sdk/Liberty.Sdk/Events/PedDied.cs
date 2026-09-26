namespace Liberty.Sdk.Events
{
    // A ped died. With exact damage (Exact true) killer, weapon, type and bone come from the killing blow.
    public struct PedDied
    {
        public PedRef Ped;
        public PedRef Killer;
        public int Weapon;
        public Bone Bone;
        public bool ByPlayer;
        // SDK 1.0 (exact damage):
        public bool Exact;
        public DamageType Type;
        public VehicleRef KillerVehicle;
    }
}
