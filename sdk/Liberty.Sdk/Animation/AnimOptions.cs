namespace Liberty.Sdk
{
    // How a clip plays. BlendIn is the blend speed (higher = faster; the game's scripts use 4-8).
    public struct AnimOptions
    {
        public float BlendIn;
        public bool Loop;
        // Upper body only, so the ped can keep moving.
        public bool UpperBody;
        // Keep the last frame after the clip ends.
        public bool HoldLastFrame;
        // Allow the player to move while it plays.
        public bool Secondary;

        public static AnimOptions Default { get { AnimOptions o = new AnimOptions(); o.BlendIn = 4f; return o; } }
        public static AnimOptions Looping { get { AnimOptions o = Default; o.Loop = true; return o; } }
    }
}