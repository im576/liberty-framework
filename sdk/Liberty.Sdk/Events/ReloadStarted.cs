namespace Liberty.Sdk.Events
{
    // A reload began (the player's clip refilled or the game reports reloading).
    public struct ReloadStarted
    {
        public PedRef Ped;
        public int Weapon;
        public int ClipBefore;
    }
}