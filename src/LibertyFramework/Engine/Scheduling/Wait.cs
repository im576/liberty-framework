using System;

namespace LibertyFramework.Engine.Scheduling
{
    // What a coroutine waits for before it resumes: time, frames, or a condition (with an optional timeout).
    public sealed class Wait
    {
        internal int UntilMs;
        internal int Frames;
        internal Func<bool> Condition;
        internal bool TimedOut;

        private Wait() { }

        public static Wait Milliseconds(int milliseconds) { Wait wait = new Wait(); wait.UntilMs = Environment.TickCount + milliseconds; return wait; }
        public static Wait NextFrame() { Wait wait = new Wait(); wait.Frames = 1; return wait; }
        public static Wait FramesCount(int frames) { Wait wait = new Wait(); wait.Frames = Math.Max(1, frames); return wait; }

        // Resumes when condition() is true, or after timeoutMs (then Wait.LastTimedOut is true for that coroutine).
        public static Wait Until(Func<bool> condition, int timeoutMs)
        {
            Wait wait = new Wait();
            wait.Condition = condition;
            wait.UntilMs = Environment.TickCount + timeoutMs;
            return wait;
        }

        internal bool Ready(int now)
        {
            if (Frames > 0) { Frames--; return Frames == 0; }
            if (Condition != null)
            {
                if (Condition()) { return true; }
                if (unchecked(now - UntilMs) >= 0) { TimedOut = true; return true; }
                return false;
            }
            return unchecked(now - UntilMs) >= 0;
        }
    }
}