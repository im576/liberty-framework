namespace Liberty.Sdk
{
    // What kind of damage a ped took (from the game's weapon type, per episode).
    public enum DamageType
    {
        Unknown = 0,
        Melee,
        Bullet,
        Explosion,
        Fire,
        Vehicle,
        Fall,
        Drowning,
        Other,
    }
}
