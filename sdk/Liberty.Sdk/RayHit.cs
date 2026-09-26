namespace Liberty.Sdk
{
    // Result of IWorldQuery.Raycast. Position, Normal, Distance and the entity are meaningful when Status is Hit (and,
    // for the thing last passed through, Inconclusive).
    public struct RayHit
    {
        public RayStatus Status;
        public Vec3 Position;
        // Surface normal at the hit, unit length (world geometry) as the game reports it.
        public Vec3 Normal;
        // Metres from the ray's start to Position.
        public float Distance;
        public RayEntityKind Kind;
        // Script handle of the hit ped, vehicle or object; 0 for world geometry.
        public int EntityHandle;
        // Line tests the query ran and hits outside the mask (or ignored) that it passed through.
        public int Tests;
        public int PassedThrough;

        public bool IsHit { get { return Status == RayStatus.Hit; } }
        public PedRef Ped { get { return Kind == RayEntityKind.Ped ? new PedRef(EntityHandle) : PedRef.None; } }
        public VehicleRef Vehicle { get { return Kind == RayEntityKind.Vehicle ? new VehicleRef(EntityHandle) : VehicleRef.None; } }
        public PropRef Prop { get { return Kind == RayEntityKind.Object ? new PropRef(EntityHandle) : PropRef.None; } }

        public override string ToString()
        {
            return Status == RayStatus.Hit || Status == RayStatus.Inconclusive
                ? Status + " " + Kind + (EntityHandle != 0 ? " " + EntityHandle : "") + " at " + Position + " distance " + Distance.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                : Status.ToString();
        }
    }
}
