namespace Liberty.Sdk
{
    // One ped in this frame's world snapshot. Health is on the 0-100(+) gameplay scale (the game's raw value - 100).
    public struct PedState
    {
        public PedRef Ped;
        public ModelRef Model;
        public Vec3 Position;
        public float Heading;
        public int Health;
        public int Armour;
        public VehicleRef Vehicle;
        public bool IsDead;
        public bool InVehicle;
        public bool IsPlayer;
        public bool IsNew;
        public float Distance;
    }
}