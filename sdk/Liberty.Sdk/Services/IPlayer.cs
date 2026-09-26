namespace Liberty.Sdk
{
    // The local player. Control locks and wanted changes made by a module are undone when it stops.
    public interface IPlayer
    {
        PedRef Ped { get; }
        int Index { get; }
        int Money { get; set; }
        int WantedLevel { get; set; }
        bool IsPlaying { get; }
        bool HasControl { get; }
        // Takes player control away until ReleaseControl (or the module stops). Needs Capabilities.PlayerControl.
        void LockControl(LibertyModule owner);
        void ReleaseControl(LibertyModule owner);
        // Loads the area first so the player does not fall through the map.
        void Teleport(Vec3 position, float heading);
        void SetInvincible(LibertyModule owner, bool invincible);
    }
}