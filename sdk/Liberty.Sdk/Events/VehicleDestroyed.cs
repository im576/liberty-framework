namespace Liberty.Sdk.Events
{
    // A vehicle's engine health fell below zero (burning/destroyed).
    public struct VehicleDestroyed
    {
        public VehicleRef Vehicle;
    }
}