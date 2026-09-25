namespace LibertyFramework.Engine.World
{
    // Global state in this frame's snapshot. Weather is the game's weather id (-1 when unknown).
    public struct WorldInfo
    {
        public uint GameTimerMs;
        public int Hours;
        public int Minutes;
        public int Weather;
        public bool Paused;
        public bool FadedOut;
    }
}