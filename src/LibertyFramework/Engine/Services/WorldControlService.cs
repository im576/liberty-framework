using System;
using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IWorldControl. Overrides are owned: a module's frozen clock, forced weather and density request end when it
    // stops. Density requests combine by taking the lowest; the governor may scale them further (engine.json
    // adaptiveDensityFloor, off by default so the vanilla population is untouched).
    public sealed class WorldControlService : IWorldControl
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<LibertyModule, float[]> density = new Dictionary<LibertyModule, float[]>();
        private readonly HashSet<LibertyModule> clockFreezers = new HashSet<LibertyModule>();
        private LibertyModule weatherOwner;
        private float appliedPeds = 1f, appliedVehicles = 1f;
        private int lastApplyMs;
        private bool densityActive;

        internal WorldControlService(LibertyEngine engine) { this.engine = engine; }

        // Governor multiplier for population (1 = unchanged).
        internal float GovernorScale = 1f;

        public void SetTime(int hours, int minutes) { Function.Call("SET_TIME_OF_DAY", Clamp(hours, 0, 23), Clamp(minutes, 0, 59)); }

        public void FreezeTime(LibertyModule owner, bool frozen)
        {
            if (frozen)
            {
                if (!clockFreezers.Add(owner)) { return; }
                if (clockFreezers.Count == 1) { GTA.World.LockDayTime(); }
                engine.Ledger.Add(owner, "clock", 0, () => Unfreeze(owner));
            }
            else { engine.Ledger.Release(owner, "clock", 0); }
        }

        private void Unfreeze(LibertyModule owner)
        {
            if (clockFreezers.Remove(owner) && clockFreezers.Count == 0) { GTA.World.UnlockDayTime(); }
        }

        public void ForceWeather(LibertyModule owner, int weather)
        {
            if (weatherOwner != null && weatherOwner != owner) { engine.Ledger.Forget(weatherOwner, "weather", 0); }
            Function.Call("FORCE_WEATHER_NOW", weather);
            Function.Call("FORCE_WEATHER", weather);
            if (weatherOwner != owner)
            {
                weatherOwner = owner;
                engine.Ledger.Add(owner, "weather", 0, () => { if (weatherOwner == owner) { weatherOwner = null; Function.Call("RELEASE_WEATHER"); } });
            }
        }

        public void ReleaseWeather(LibertyModule owner) { engine.Ledger.Release(owner, "weather", 0); }

        public void SetDensity(LibertyModule owner, float peds, float vehicles)
        {
            if (!density.ContainsKey(owner)) { engine.Ledger.Add(owner, "density", 0, () => { density.Remove(owner); lastApplyMs = 0; }); }
            density[owner] = new[] { Math.Max(0f, peds), Math.Max(0f, vehicles) };
            lastApplyMs = 0;
        }

        public void ClearDensity(LibertyModule owner) { engine.Ledger.Release(owner, "density", 0); }

        public float GroundZ(Vec3 position) { return GTA.World.GetGroundZ(Handles.V(position)); }

        public void ClearArea(Vec3 center, float radius) { Function.Call("CLEAR_AREA", center.X, center.Y, center.Z, radius, true); }

        // Engine tick: density multipliers are re-applied when they change and every two seconds while any override
        // is active (the game's own scripts also set them).
        internal void Update()
        {
            float peds = 1f, vehicles = 1f;
            foreach (float[] request in density.Values) { peds = Math.Min(peds, request[0]); vehicles = Math.Min(vehicles, request[1]); }
            peds *= GovernorScale;
            vehicles *= GovernorScale;
            bool active = density.Count > 0 || GovernorScale < 1f;
            if (!active && !densityActive) { return; }
            int now = Environment.TickCount;
            bool changed = Math.Abs(peds - appliedPeds) > 0.01f || Math.Abs(vehicles - appliedVehicles) > 0.01f || active != densityActive;
            if (!changed && unchecked(now - lastApplyMs) < 2000) { return; }
            lastApplyMs = now;
            appliedPeds = active ? peds : 1f;
            appliedVehicles = active ? vehicles : 1f;
            densityActive = active;
            Function.Call("SET_PED_DENSITY_MULTIPLIER", appliedPeds);
            Function.Call("SET_CAR_DENSITY_MULTIPLIER", appliedVehicles);
        }

        private static int Clamp(int value, int low, int high) { return value < low ? low : value > high ? high : value; }
    }
}
