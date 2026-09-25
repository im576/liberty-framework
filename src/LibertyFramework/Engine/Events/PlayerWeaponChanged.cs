namespace LibertyFramework.Engine.Events
{
    // The player selected another weapon.
    public struct PlayerWeaponChanged
    {
        public int Previous;
        public int Current;
    }
}