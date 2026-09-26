namespace Liberty.Sdk.Events
{
    // The player's clip went down with the same weapon selected (once per frame, possibly several rounds).
    public struct PlayerShot
    {
        public int Weapon;
        public int ClipBefore;
        public int ClipAfter;
    }
}