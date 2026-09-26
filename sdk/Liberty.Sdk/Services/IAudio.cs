namespace Liberty.Sdk
{
    // Game sounds and speech. Owned sounds stop with the module.
    public interface IAudio
    {
        // Frontend (UI) sound by name, e.g. "PHONE_TEXT_ARRIVE".
        void PlayFrontend(string sound);
        SoundRef PlayAt(LibertyModule owner, string sound, Vec3 position);
        SoundRef PlayOnPed(LibertyModule owner, string sound, PedRef ped);
        SoundRef PlayOnVehicle(LibertyModule owner, string sound, VehicleRef vehicle);
        void Stop(SoundRef sound);
        bool IsFinished(SoundRef sound);
        // Ambient speech context, e.g. "GENERIC_CURSE" or "PAIN".
        void Say(PedRef ped, string context);
    }
}