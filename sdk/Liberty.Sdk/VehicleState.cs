namespace Liberty.Sdk
{
    // One vehicle in this frame's world snapshot. Health 0-1000; EngineHealth 0-1000 (below 0 = on fire).
    public struct VehicleState
    {
        public VehicleRef Vehicle;
        public ModelRef Model;
        public Vec3 Position;
        public float Heading;
        public float Speed;
        public int Health;
        public float EngineHealth;
        public PedRef Driver;
        public bool IsNew;
        public float Distance;
    }
}