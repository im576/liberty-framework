using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // The engine's per-frame view of the game, read once per frame by the native core. Read facts here instead of
    // asking the game again; it costs nothing and every module sees the same frame.
    public interface IWorldState
    {
        int Frame { get; }
        bool FromCore { get; }
        bool HasPlayer { get; }
        bool HasPeds { get; }
        bool HasVehicles { get; }
        PlayerState Player { get; }
        WorldInfo Info { get; }
        IReadOnlyList<PedState> Peds { get; }
        IReadOnlyList<VehicleState> Vehicles { get; }
        bool TryGetPed(PedRef ped, out PedState state);
        bool TryGetVehicle(VehicleRef vehicle, out VehicleState state);
        // Nearest ped to a point within maxDistance matching the filter (null filter = any living non-player ped).
        bool TryGetNearestPed(Vec3 point, float maxDistance, Func<PedState, bool> filter, out PedState state);
        bool TryGetNearestVehicle(Vec3 point, float maxDistance, out VehicleState state);
    }
}