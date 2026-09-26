using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // Spatial questions. The snapshot queries cost no game calls (they search this frame's snapshot, so they see peds and
    // vehicles within the engine's snapshot radius). The game queries ask the game. Raycast and HasLineOfSight (SDK 1.1)
    // are geometric: the game's own physics line test against its collision. HasSpotted is the game's perception check,
    // not a line of sight.
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

        // ---- SDK 1.1: raycast and line of sight (engine physics; docs/sdk/README.md "Raycast") ----
        // Whether raycasts can run now (engine core on, the game's line test found and not switched off after a fault).
        bool RaycastAvailable { get; }
        // First thing in stopAt between from and to; anything else, and everything in ignore, is passed through. Only
        // collision the game has loaded is tested (near the player). Call from OnUpdate, coroutines or commands, never
        // from OnDraw. Throws ArgumentException for non-finite points, from == to, or a ray longer than
        // engine.json raycastMaxLengthMeters (1000 m by default).
        RayHit Raycast(Vec3 from, Vec3 to, RayMask stopAt);
        RayHit Raycast(Vec3 from, Vec3 to, RayMask stopAt, RayIgnore ignore);
        // True when nothing in blockers lies between the points (Raycast status Clear). False when blocked,
        // inconclusive or unavailable; check RaycastAvailable, or use Raycast for the reason.
        bool HasLineOfSight(Vec3 from, Vec3 to, RayMask blockers, RayIgnore ignore);
        // Whether viewer's eyes have a clear line to target's head, chest or pelvis. Blockers are world, vehicles and
        // objects; other peds do not block. Both peds' own vehicles are ignored (you see people through their car).
        // Geometry only: no view cone, distance or lighting. False when either ped does not exist, raycasts are
        // unavailable, or the target is farther than engine.json raycastMaxLengthMeters.
        bool HasLineOfSight(PedRef viewer, PedRef target);
    }
}
