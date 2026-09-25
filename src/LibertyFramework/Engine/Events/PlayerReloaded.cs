namespace LibertyFramework.Engine.Events
{
    // The player's clip went up with the same weapon selected.
    public struct PlayerReloaded
    {
        public int Weapon;
        public int ClipBefore;
        public int ClipAfter;
    }
}