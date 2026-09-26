using System;
using System.Collections.Generic;
using Liberty.Sdk;

namespace LibertyFramework.Engine.World
{
    // SDK IWorldState: the per-frame view of the game, built by WorldBuilder from the native core (or the fallback).
    public sealed class WorldState : IWorldState
    {
        private readonly List<PedState> peds = new List<PedState>(128);
        private readonly Dictionary<int, int> pedIndex = new Dictionary<int, int>(128);
        private readonly List<VehicleState> vehicles = new List<VehicleState>(64);
        private readonly Dictionary<int, int> vehicleIndex = new Dictionary<int, int>(64);

        public int Frame { get; internal set; }
        public bool FromCore { get; internal set; }
        public bool HasPlayer { get; internal set; }
        public bool HasWorld { get; internal set; }
        public bool HasPeds { get; internal set; }
        public bool HasVehicles { get; internal set; }
        public PlayerState Player { get; internal set; }
        public WorldInfo Info { get; internal set; }
        public IReadOnlyList<PedState> Peds { get { return peds; } }
        public IReadOnlyList<VehicleState> Vehicles { get { return vehicles; } }
        public float CoreMicroseconds { get; internal set; }
        internal LibertyFramework.Engine.Core.LcPools Pools { get; set; }

        public bool TryGetPed(PedRef ped, out PedState state)
        {
            int index;
            if (pedIndex.TryGetValue(ped.Handle, out index)) { state = peds[index]; return true; }
            state = default(PedState);
            return false;
        }

        public bool TryGetVehicle(VehicleRef vehicle, out VehicleState state)
        {
            int index;
            if (vehicleIndex.TryGetValue(vehicle.Handle, out index)) { state = vehicles[index]; return true; }
            state = default(VehicleState);
            return false;
        }

        public bool TryGetNearestPed(Vec3 point, float maxDistance, Func<PedState, bool> filter, out PedState state)
        {
            state = default(PedState);
            float best = maxDistance * maxDistance;
            bool found = false;
            for (int i = 0; i < peds.Count; i++)
            {
                PedState p = peds[i];
                if (filter == null ? (p.IsPlayer || p.IsDead) : !filter(p)) { continue; }
                float d = (p.Position - point).LengthSquared;
                if (d <= best) { best = d; state = p; found = true; }
            }
            return found;
        }

        public bool TryGetNearestVehicle(Vec3 point, float maxDistance, out VehicleState state)
        {
            state = default(VehicleState);
            float best = maxDistance * maxDistance;
            bool found = false;
            for (int i = 0; i < vehicles.Count; i++)
            {
                float d = (vehicles[i].Position - point).LengthSquared;
                if (d <= best) { best = d; state = vehicles[i]; found = true; }
            }
            return found;
        }

        internal void ClearPeds() { peds.Clear(); pedIndex.Clear(); }
        internal void AddPed(PedState ped) { pedIndex[ped.Ped.Handle] = peds.Count; peds.Add(ped); }
        internal void ClearVehicles() { vehicles.Clear(); vehicleIndex.Clear(); }
        internal void AddVehicle(VehicleState vehicle) { vehicleIndex[vehicle.Vehicle.Handle] = vehicles.Count; vehicles.Add(vehicle); }
    }
}