namespace Liberty.Sdk
{
    // Global state in this frame's snapshot. Weather: 0 extra sunny, 1 sunny, 2 sunny windy, 3 cloudy, 4 rain,
    // 5 drizzle, 6 foggy, 7 lightning (-1 unknown).
    public struct WorldInfo
    {
        public uint GameTimerMs;
        public int Hours;
        public int Minutes;
        public int Weather;
        public bool Paused;
        public bool FadedOut;
        public bool MissionActive;
        public bool CutscenePlaying;
    }
}