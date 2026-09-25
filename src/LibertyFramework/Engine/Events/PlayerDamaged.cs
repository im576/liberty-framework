namespace LibertyFramework.Engine.Events
{
    // The player lost health or armour. Health on SHDN scale.
    public struct PlayerDamaged
    {
        public int HealthBefore;
        public int HealthAfter;
        public int ArmourBefore;
        public int ArmourAfter;
    }
}