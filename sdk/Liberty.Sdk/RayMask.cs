using System;

namespace Liberty.Sdk
{
    // What stops a ray (IWorldQuery.Raycast / HasLineOfSight). Anything not in the mask is passed through: a line of
    // sight that should not be blocked by passers-by uses World | Vehicles | Objects.
    [Flags]
    public enum RayMask
    {
        None = 0,
        // Map geometry: ground, buildings, walls, and anything that is not a ped, vehicle or pooled object.
        World = 0x1,
        Peds = 0x2,
        Vehicles = 0x4,
        // Objects in the game's object pool (props, including the ones mods create).
        Objects = 0x8,
        All = World | Peds | Vehicles | Objects,
    }
}
