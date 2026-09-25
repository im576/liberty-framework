using GTA;

namespace LibertyFramework.Engine.World
{
    // One ped in this frame's snapshot. Health is on ScriptHookDotNet's scale (Ped.Health; the raw native is 100 higher).
    public struct PedState
    {
        public int Handle;
        public int Model;
        public Vector3 Position;
        public float Heading;
        public int Health;
        public int Armour;
        public int Vehicle;
        public bool IsDead;
        public bool InVehicle;
        public bool IsPlayer;
        public bool IsNew;
        public float Distance;
    }
}