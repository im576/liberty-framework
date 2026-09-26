using System;

namespace Liberty.Sdk
{
    // What a coroutine waits for: time, frames, or a condition with a timeout.
    public sealed class Wait
    {
        internal int UntilMs;
        internal int Frames;
        internal Func<bool> Condition;
        internal bool TimedOut;

        private Wait() { }

        public static Wait Milliseconds(int milliseconds) { Wait w = new Wait(); w.UntilMs = Environment.TickCount + Math.Max(0, milliseconds); return w; }
        public static Wait Seconds(float seconds) { return Milliseconds((int)(seconds * 1000)); }
        public static Wait NextFrame() { Wait w = new Wait(); w.Frames = 1; return w; }
        public static Wait FramesCount(int frames) { Wait w = new Wait(); w.Frames = Math.Max(1, frames); return w; }
        public static Wait Until(Func<bool> condition, int timeoutMs) { Wait w = new Wait(); w.Condition = condition; w.UntilMs = Environment.TickCount + timeoutMs; return w; }

        // Engine: advances this wait by one frame; true when the coroutine may continue.
        public bool Step(int nowMs)
        {
            if (Frames > 0) { Frames--; return Frames == 0; }
            if (Condition != null)
            {
                if (Condition()) { return true; }
                if (unchecked(nowMs - UntilMs) >= 0) { TimedOut = true; return true; }
                return false;
            }
            return unchecked(nowMs - UntilMs) >= 0;
        }

        public bool HasTimedOut { get { return TimedOut; } }
    }
}