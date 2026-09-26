namespace Liberty.Sdk.Events
{
    // A vehicle lost body or engine health.
    public struct VehicleDamaged
    {
        public VehicleRef Vehicle;
        public int HealthBefore;
        public int HealthAfter;
        public float EngineBefore;
        public float EngineAfter;
    }
}