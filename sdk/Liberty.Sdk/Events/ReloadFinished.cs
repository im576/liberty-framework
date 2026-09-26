namespace Liberty.Sdk.Events
{
    // A reload completed.
    public struct ReloadFinished
    {
        public PedRef Ped;
        public int Weapon;
        public int ClipAfter;
    }
}