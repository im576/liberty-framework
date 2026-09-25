namespace LibertyFramework.Engine.Events
{
    // A ped lost health since the last frame. Bone is the game's last damage bone (-1 unknown); ByPlayer when the player damaged it. Health on SHDN scale.
    public struct PedDamaged
    {
        public int Handle;
        public int HealthBefore;
        public int HealthAfter;
        public int Bone;
        public bool ByPlayer;
    }
}