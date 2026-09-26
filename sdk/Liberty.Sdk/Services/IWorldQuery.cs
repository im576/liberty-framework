using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // Spatial questions. The snapshot queries cost no game calls (they search this frame's snapshot, so they see peds and
    // vehicles within the engine's snapshot radius). The game queries ask the game. A geometric ray test is planned
    // (engine physics); until then, HasSpotted is the game's own perception check, not a line of sight.
    public interface IWorldQuery
    {
        // Clears results and fills it with the snapshot peds within radius (nearest first) matching filter (null = any).
        int PedsInRadius(Vec3 center, float radius, Func<PedState, bool> filter, IList<PedState> results);
        int VehiclesInRadius(Vec3 center, float radius, IList<VehicleState> results);
        // Nearest snapshot ped/vehicle inside a cone (halfAngleDegrees around direction) within maxDistance.
        bool NearestPedInCone(Vec3 origin, Vec3 direction, float maxDistance, float halfAngleDegrees, Func<PedState, bool> filter, out PedState state);
        bool NearestVehicleInCone(Vec3 origin, Vec3 direction, float maxDistance, float halfAngleDegrees, out VehicleState state);
        // Ground height below a point (0 when the area's collision is not loaded).
        float GroundZ(Vec3 position);
        // Water surface height at a point; false when there is no water there.
        bool WaterHeight(Vec3 position, out float height);
        // The game's perception: has ped noticed other (HAS_CHAR_SPOTTED_CHAR).
        bool HasSpotted(PedRef ped, PedRef other);
        bool IsOnScreen(PedRef ped);
        // Whether a sphere is inside the game camera's view.
        bool IsSphereVisible(Vec3 center, float radius);
    }
}
