namespace Liberty.Sdk
{
    // The player in this frame's snapshot. Weapon and AmmoInClip are -1 when unknown.
    public struct PlayerState
    {
        public int Index;
        public PedRef Ped;
        public Vec3 Position;
        public float Heading;
        public int Health;
        public int Armour;
        public int Weapon;
        public int AmmoInClip;
        public VehicleRef Vehicle;
        public bool IsPlaying;
        public bool HasControl;
        public bool IsDead;
        public bool InVehicle;
        public bool IsReloading;
    }
}