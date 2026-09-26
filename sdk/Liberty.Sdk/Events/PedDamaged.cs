namespace Liberty.Sdk.Events
{
    // A ped lost health or armour. Exact is true when the engine observed the damage call itself (attacker, weapon and bone are then authoritative); otherwise they are the game's last-damage records.
    public struct PedDamaged
    {
        public PedRef Ped;
        public PedRef Attacker;
        public int Weapon;
        public Bone Bone;
        public int HealthBefore;
        public int HealthAfter;
        public bool ByPlayer;
        public bool Exact;
    }
}