using GTA;

namespace LibertyFramework.Engine.World
{
    // The player in this frame's snapshot. Health is on ScriptHookDotNet's scale. Weapon/AmmoInClip are -1 when unknown.
    public struct PlayerState
    {
        public int Index;
        public int Ped;
        public Vector3 Position;
        public float Heading;
        public int Health;
        public int Armour;
        public int Weapon;
        public int AmmoInClip;
        public int Vehicle;
        public bool IsPlaying;
        public bool HasControl;
        public bool IsDead;
        public bool InVehicle;
    }
}