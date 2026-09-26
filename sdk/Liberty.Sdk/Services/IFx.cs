namespace Liberty.Sdk
{
    // Particle effects from the game's effect library (e.g. "blood_gun_entry", "ped_breath"). Owned; stopped with the module.
    public interface IFx
    {
        // One-shot effect at a point; rotation in degrees.
        bool Burst(string effect, Vec3 position, Vec3 rotation, float scale);
        bool BurstOnPed(string effect, PedRef ped, Bone bone, Vec3 offset, float scale);
        // Looping effect; returns None when the effect does not exist.
        FxRef Start(LibertyModule owner, string effect, Vec3 position, Vec3 rotation, float scale);
        FxRef StartOnPed(LibertyModule owner, string effect, PedRef ped, Bone bone, Vec3 offset, float scale);
        FxRef StartOnVehicle(LibertyModule owner, string effect, VehicleRef vehicle, Vec3 offset, float scale);
        void Stop(FxRef fx);
    }
}