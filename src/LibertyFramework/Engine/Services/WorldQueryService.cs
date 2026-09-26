using System;
using System.Collections.Generic;
using System.Diagnostics;
using GTA.Native;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Performance.Logic;
using LibertyFramework.Engine.Core;

namespace LibertyFramework.Engine.Services
{
    // SDK IWorldQuery. Snapshot queries walk this frame's snapshot (no game calls, no allocation beyond the caller's list);
    // game queries are single natives (NATIVES.md, SDK 1.0 WorldQuery); raycasts are the game's line test called by the
    // core (ADR-0008, docs/research/Raycast.md).
    public sealed class WorldQueryService : IWorldQuery
    {
        private readonly LibertyEngine engine;
        private readonly List<KeyValuePair<float, int>> order = new List<KeyValuePair<float, int>>();

        internal WorldQueryService(LibertyEngine engine) { this.engine = engine; }

        // Line of sight to a ped: its head, chest and pelvis, in that order; the first clear line answers.
        private static readonly Bone[] SightBones = { Bone.Head, Bone.Spine2, Bone.Pelvis };
        private const RayMask SightBlockers = RayMask.World | RayMask.Vehicles | RayMask.Objects;
        private bool loggedOutsideTick;

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

        // ---- SDK 1.1 raycast (ADR-0008) ----

        public bool RaycastAvailable { get { return engine.Config.RaycastEnabled && engine.Core.RaycastReady; } }

        public RayHit Raycast(Vec3 from, Vec3 to, RayMask stopAt) { return Raycast(from, to, stopAt, RayIgnore.Nothing); }

        public RayHit Raycast(Vec3 from, Vec3 to, RayMask stopAt, RayIgnore ignore)
        {
            CheckSegment(from, to);
            return Cast(from, to, stopAt, ignore, engine.Config.RaycastMaxPasses, 0);
        }

        public bool HasLineOfSight(Vec3 from, Vec3 to, RayMask blockers, RayIgnore ignore)
        {
            return Raycast(from, to, blockers, ignore).Status == RayStatus.Clear;
        }

        public bool HasLineOfSight(PedRef viewer, PedRef target)
        {
            if (!RaycastAvailable || viewer.IsNone || target.IsNone || !engine.Peds.Exists(viewer) || !engine.Peds.Exists(target)) { return false; }
            // The viewer goes first: the first ignored entity is handed to the game, so its own capsule costs no pass.
            RayIgnore ignore = RayIgnore.Of(viewer).And(engine.Peds.GetVehicle(viewer)).And(engine.Peds.GetVehicle(target));
            Vec3 eye = engine.Peds.GetBonePosition(viewer, Bone.Head);
            for (int i = 0; i < SightBones.Length; i++)
            {
                Vec3 point = engine.Peds.GetBonePosition(target, SightBones[i]);
                float distance = eye.DistanceTo(point);
                if (!(distance <= engine.Config.RaycastMaxLengthMeters)) { return false; }
                if (distance < 0.01f) { return true; }
                RayStatus status = Cast(eye, point, SightBlockers, ignore, engine.Config.RaycastMaxPasses, 0).Status;
                if (status == RayStatus.Clear) { return true; }
                if (status == RayStatus.Unavailable) { return false; }
            }
            return false;
        }

        private void CheckSegment(Vec3 from, Vec3 to)
        {
            if (!Finite(from) || !Finite(to)) { throw new ArgumentException("raycast points must be finite: from " + from + " to " + to); }
            float length = from.DistanceTo(to);
            if (!(length >= 0.001f)) { throw new ArgumentException("raycast from and to are the same point " + from); }
            if (length > engine.Config.RaycastMaxLengthMeters)
            {
                throw new ArgumentException("raycast of " + length.ToString("0.0") + " m is longer than engine.json raycastMaxLengthMeters (" +
                    engine.Config.RaycastMaxLengthMeters.ToString("0") + " m)");
            }
        }

        private static bool Finite(Vec3 v)
        {
            return !float.IsNaN(v.X) && !float.IsInfinity(v.X) && !float.IsNaN(v.Y) && !float.IsInfinity(v.Y) && !float.IsNaN(v.Z) && !float.IsInfinity(v.Z);
        }

        // One query through the core. research = LcRay.FlagResearch keeps the raw result for the raydebug command.
        internal RayHit Cast(Vec3 from, Vec3 to, RayMask stopAt, RayIgnore ignore, int maxPasses, uint research)
        {
            LcRayHit raw;
            return Cast(from, to, stopAt, ignore, maxPasses, research, LcRay.IncludeAll, 1, out raw);
        }

        internal unsafe RayHit Cast(Vec3 from, Vec3 to, RayMask stopAt, RayIgnore ignore, int maxPasses, uint research, uint include, int mode, out LcRayHit raw)
        {
            raw = new LcRayHit();
            RayHit result = new RayHit();
            result.Status = RayStatus.Unavailable;
            if (!RaycastAvailable) { return result; }
            if (!engine.InGameContext)
            {
                // The game's physics may only be queried while the game thread is parked in the engine tick.
                if (!loggedOutsideTick)
                {
                    loggedOutsideTick = true;
                    RuntimeLog.Error("engine_raycast_outside_tick module=" + (engine.CurrentModule != null ? engine.CurrentModule.Id : "engine") +
                        " (raycasts answer Unavailable from OnDraw or other threads)");
                }
                return result;
            }
            LcRay ray = new LcRay();
            ray.StartX = from.X; ray.StartY = from.Y; ray.StartZ = from.Z;
            ray.EndX = to.X; ray.EndY = to.Y; ray.EndZ = to.Z;
            ray.IncludeFlags = include;
            ray.Mode = mode;
            ray.Accept = (uint)stopAt & LcRay.AcceptAll;
            ray.MaxPasses = maxPasses;
            ray.PassStep = engine.Config.RaycastPassStepMeters;
            ray.Flags = research;
            ray.IgnoreCount = ignore.Count;
            for (int i = 0; i < ignore.Count; i++)
            {
                ray.IgnoreKind[i] = (int)ignore.KindAt(i);
                ray.IgnoreHandle[i] = ignore.HandleAt(i);
            }
            long mark = Stopwatch.GetTimestamp();
            int status = engine.Core.Raycast(ref ray, ref raw);
            CostMeter.Add("engine.raycast", mark);
            result.Tests = raw.Tests;
            result.PassedThrough = raw.Passes;
            switch (status)
            {
                case LcRay.Clear: result.Status = RayStatus.Clear; return result;
                case LcRay.Hit: result.Status = RayStatus.Hit; break;
                case LcRay.Inconclusive: result.Status = RayStatus.Inconclusive; break;
                default: return result;
            }
            result.Position = new Vec3(raw.X, raw.Y, raw.Z);
            result.Normal = new Vec3(raw.NormalX, raw.NormalY, raw.NormalZ);
            result.Distance = raw.Distance;
            result.Kind = raw.EntityKind >= LcRay.EntityPed && raw.EntityKind <= LcRay.EntityObject ? (RayEntityKind)raw.EntityKind : RayEntityKind.World;
            result.EntityHandle = result.Kind == RayEntityKind.World ? 0 : raw.EntityHandle;
            return result;
        }
    }
}
