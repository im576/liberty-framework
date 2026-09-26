using System;
using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IWorldQuery. Snapshot queries walk this frame's snapshot (no game calls, no allocation beyond the caller's list);
    // game queries are single natives (NATIVES.md, SDK 1.0 WorldQuery).
    public sealed class WorldQueryService : IWorldQuery
    {
        private readonly LibertyEngine engine;
        private readonly List<KeyValuePair<float, int>> order = new List<KeyValuePair<float, int>>();

        internal WorldQueryService(LibertyEngine engine) { this.engine = engine; }

        public int PedsInRadius(Vec3 center, float radius, Func<PedState, bool> filter, IList<PedState> results)
        {
            results.Clear();
            IReadOnlyList<PedState> peds = engine.World.Peds;
            float r2 = radius * radius;
            order.Clear();
            for (int i = 0; i < peds.Count; i++)
            {
                float d = (peds[i].Position - center).LengthSquared;
                if (d <= r2 && (filter == null || filter(peds[i]))) { order.Add(new KeyValuePair<float, int>(d, i)); }
            }
            order.Sort((a, b) => a.Key.CompareTo(b.Key));
            foreach (KeyValuePair<float, int> pair in order) { results.Add(peds[pair.Value]); }
            return results.Count;
        }

        public int VehiclesInRadius(Vec3 center, float radius, IList<VehicleState> results)
        {
            results.Clear();
            IReadOnlyList<VehicleState> vehicles = engine.World.Vehicles;
            float r2 = radius * radius;
            order.Clear();
            for (int i = 0; i < vehicles.Count; i++)
            {
                float d = (vehicles[i].Position - center).LengthSquared;
                if (d <= r2) { order.Add(new KeyValuePair<float, int>(d, i)); }
            }
            order.Sort((a, b) => a.Key.CompareTo(b.Key));
            foreach (KeyValuePair<float, int> pair in order) { results.Add(vehicles[pair.Value]); }
            return results.Count;
        }

        public bool NearestPedInCone(Vec3 origin, Vec3 direction, float maxDistance, float halfAngleDegrees, Func<PedState, bool> filter, out PedState state)
        {
            state = default(PedState);
            Vec3 axis = direction.Normalized;
            float minCos = (float)Math.Cos(halfAngleDegrees * Math.PI / 180.0), best = maxDistance;
            bool found = false;
            IReadOnlyList<PedState> peds = engine.World.Peds;
            for (int i = 0; i < peds.Count; i++)
            {
                if (filter == null ? (peds[i].IsPlayer || peds[i].IsDead) : !filter(peds[i])) { continue; }
                if (!InCone(origin, axis, peds[i].Position, minCos, ref best)) { continue; }
                state = peds[i];
                found = true;
            }
            return found;
        }

        public bool NearestVehicleInCone(Vec3 origin, Vec3 direction, float maxDistance, float halfAngleDegrees, out VehicleState state)
        {
            state = default(VehicleState);
            Vec3 axis = direction.Normalized;
            float minCos = (float)Math.Cos(halfAngleDegrees * Math.PI / 180.0), best = maxDistance;
            bool found = false;
            IReadOnlyList<VehicleState> vehicles = engine.World.Vehicles;
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (!InCone(origin, axis, vehicles[i].Position, minCos, ref best)) { continue; }
                state = vehicles[i];
                found = true;
            }
            return found;
        }

        private static bool InCone(Vec3 origin, Vec3 axis, Vec3 point, float minCos, ref float best)
        {
            Vec3 to = point - origin;
            float distance = to.Length;
            if (distance < 0.001f || distance > best) { return false; }
            if (to.Dot(axis) / distance < minCos) { return false; }
            best = distance;
            return true;
        }

        public float GroundZ(Vec3 position) { return GTA.World.GetGroundZ(Handles.V(position)); }

        public bool WaterHeight(Vec3 position, out float height)
        {
            Pointer value = typeof(float);
            bool water = Function.Call<bool>("GET_WATER_HEIGHT", position.X, position.Y, position.Z, value);
            height = water ? (float)value : 0f;
            return water;
        }

        public bool HasSpotted(PedRef ped, PedRef other) { return Function.Call<bool>("HAS_CHAR_SPOTTED_CHAR", ped.Handle, other.Handle); }

        public bool IsOnScreen(PedRef ped) { return Function.Call<bool>("IS_CHAR_ON_SCREEN", ped.Handle); }

        // SHDN's Camera.isSphereVisible on the rendering camera: CAM_IS_SPHERE_VISIBLE on the GET_GAME_CAM handle answered false
        // for a ped that IS_CHAR_ON_SCREEN reported visible (self-test, 2026-09-25).
        public bool IsSphereVisible(Vec3 center, float radius)
        {
            GTA.Camera camera = GTA.Game.CurrentCamera;
            return camera != null && camera.isSphereVisible(Handles.V(center), radius);
        }
    }
}
