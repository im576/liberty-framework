namespace Liberty.Sdk
{
    // Time, weather and population. Overrides made by a module are released when it stops.
    public interface IWorldControl
    {
        void SetTime(int hours, int minutes);
        void FreezeTime(LibertyModule owner, bool frozen);
        // Forces a weather id now and keeps it until ReleaseWeather (or the module stops).
        void ForceWeather(LibertyModule owner, int weather);
        void ReleaseWeather(LibertyModule owner);
        // Per-frame ped/vehicle density multipliers (0-1+) while the module runs; the lowest request wins.
        void SetDensity(LibertyModule owner, float peds, float vehicles);
        void ClearDensity(LibertyModule owner);
        float GroundZ(Vec3 position);
        void ClearArea(Vec3 center, float radius);
    }
}