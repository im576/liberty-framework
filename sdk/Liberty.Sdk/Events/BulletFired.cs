namespace Liberty.Sdk.Events
{
    // A bullet entered the game's bullet list (any shooter).
    public struct BulletFired
    {
        public PedRef Shooter;
        public int Weapon;
        public Vec3 From;
        public Vec3 To;
        public bool ByPlayer;
    }
}