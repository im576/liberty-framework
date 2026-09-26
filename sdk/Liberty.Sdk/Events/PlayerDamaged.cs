namespace Liberty.Sdk.Events
{
    // The player lost health or armour.
    public struct PlayerDamaged
    {
        public int HealthBefore;
        public int HealthAfter;
        public int ArmourBefore;
        public int ArmourAfter;
    }
}