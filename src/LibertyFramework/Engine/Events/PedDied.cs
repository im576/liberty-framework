namespace LibertyFramework.Engine.Events
{
    // A ped became dead since the last frame.
    public struct PedDied
    {
        public int Handle;
        public int Bone;
        public bool ByPlayer;
    }
}