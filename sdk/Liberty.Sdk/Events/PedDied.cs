namespace Liberty.Sdk.Events
{
    // A ped died.
    public struct PedDied
    {
        public PedRef Ped;
        public PedRef Killer;
        public int Weapon;
        public Bone Bone;
        public bool ByPlayer;
    }
}