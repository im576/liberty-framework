using GTA;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IAudio. Positional sounds take a script sound id (GET_SOUND_ID) that is stopped and released with the
    // owning module; frontend sounds and ambient speech need no cleanup.
    public sealed class AudioService : IAudio
    {
        private readonly LibertyEngine engine;

        internal AudioService(LibertyEngine engine) { this.engine = engine; }

        // PLAY_SOUND_FRONTEND(-1, name): -1 = fire and forget.
        public void PlayFrontend(string sound) { Function.Call("PLAY_SOUND_FRONTEND", -1, sound); }

        public SoundRef PlayAt(LibertyModule owner, string sound, Vec3 position)
        {
            int id = NewSound(owner);
            Function.Call("PLAY_SOUND_FROM_POSITION", id, sound, position.X, position.Y, position.Z);
            return new SoundRef(id + 1);
        }

        public SoundRef PlayOnPed(LibertyModule owner, string sound, PedRef ped)
        {
            int id = NewSound(owner);
            Function.Call("PLAY_SOUND_FROM_PED", id, sound, ped.Handle);
            return new SoundRef(id + 1);
        }

        public SoundRef PlayOnVehicle(LibertyModule owner, string sound, VehicleRef vehicle)
        {
            int id = NewSound(owner);
            Function.Call("PLAY_SOUND_FROM_VEHICLE", id, sound, vehicle.Handle);
            return new SoundRef(id + 1);
        }

        // Sound id 0 is valid in the game, so SoundRef stores id + 1.
        private int NewSound(LibertyModule owner)
        {
            int id = Function.Call<int>("GET_SOUND_ID");
            engine.Ledger.Add(owner, "sound", id + 1, () => { Function.Call("STOP_SOUND", id); Function.Call("RELEASE_SOUND_ID", id); });
            return id;
        }

        public void Stop(SoundRef sound)
        {
            if (sound.IsNone) { return; }
            engine.Ledger.Release("sound", sound.Handle);
        }

        public bool IsFinished(SoundRef sound) { return sound.IsNone || Function.Call<bool>("HAS_SOUND_FINISHED", sound.Handle - 1); }

        public void Say(PedRef ped, string context)
        {
            Ped wrapper = Handles.Ped(ped);
            if (wrapper != null) { wrapper.SayAmbientSpeech(context); }
        }
    }
}
