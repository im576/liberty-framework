namespace Liberty.Sdk
{
    // What a ray hit.
    public enum RayEntityKind
    {
        // Map geometry, or anything that is not a ped, vehicle or pooled object.
        World = 0,
        Ped = 1,
        Vehicle = 2,
        Object = 3,
    }
}
