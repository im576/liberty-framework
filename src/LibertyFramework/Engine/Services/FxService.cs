using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IFx. The last argument of every TRIGGER_/START_PTFX* native is a float scale (the handlers read it with movss;
    // an int made every effect invisible, NATIVES.md gore finding). TRIGGER refuses looping effects; START returns a
    // script id (0 = refused) that STOP_PTFX ends. Started effects are owned and stopped with the module.
    public sealed class FxService : IFx
    {
        private readonly LibertyEngine engine;

        internal FxService(LibertyEngine engine) { this.engine = engine; }

        public bool Burst(string effect, Vec3 position, Vec3 rotation, float scale)
        {
            if (string.IsNullOrEmpty(effect) || scale <= 0) { return false; }
            return Function.Call<bool>("TRIGGER_PTFX", effect, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, scale);
        }

        public bool BurstOnPed(string effect, PedRef ped, Bone bone, Vec3 offset, float scale)
        {
            if (string.IsNullOrEmpty(effect) || scale <= 0) { return false; }
            return Function.Call<bool>("TRIGGER_PTFX_ON_PED_BONE", effect, ped.Handle, offset.X, offset.Y, offset.Z, 0f, 0f, 0f, (int)bone, scale);
        }

        public FxRef Start(LibertyModule owner, string effect, Vec3 position, Vec3 rotation, float scale)
        {
            if (string.IsNullOrEmpty(effect) || scale <= 0) { return FxRef.None; }
            return Own(owner, Function.Call<int>("START_PTFX", effect, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, scale));
        }

        public FxRef StartOnPed(LibertyModule owner, string effect, PedRef ped, Bone bone, Vec3 offset, float scale)
        {
            if (string.IsNullOrEmpty(effect) || scale <= 0) { return FxRef.None; }
            return Own(owner, Function.Call<int>("START_PTFX_ON_PED_BONE", effect, ped.Handle, offset.X, offset.Y, offset.Z, 0f, 0f, 0f, (int)bone, scale));
        }

        public FxRef StartOnVehicle(LibertyModule owner, string effect, VehicleRef vehicle, Vec3 offset, float scale)
        {
            if (string.IsNullOrEmpty(effect) || scale <= 0) { return FxRef.None; }
            return Own(owner, Function.Call<int>("START_PTFX_ON_VEH", effect, vehicle.Handle, offset.X, offset.Y, offset.Z, 0f, 0f, 0f, scale));
        }

        private FxRef Own(LibertyModule owner, int id)
        {
            if (id == 0) { return FxRef.None; }
            engine.Ledger.Add(owner, "fx", id, () => Function.Call("STOP_PTFX", id));
            return new FxRef(id);
        }

        public void Stop(FxRef fx)
        {
            if (fx.IsNone) { return; }
            if (engine.Ledger.Has("fx", fx.Handle)) { engine.Ledger.Release("fx", fx.Handle); }
            else { Function.Call("STOP_PTFX", fx.Handle); }
        }
    }
}
