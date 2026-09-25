using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibertyFramework.GameApi
{
    // T-026 step 1: evidence for whether the game's main thread is parked while an SHDN script ticks.
    // - Frame counter (engine global behind GET_FRAME_COUNT) read at tick start and end: if it never advances
    //   during a tick, the game waits for our scripts, so direct engine reads from our thread are serialized with it.
    // - Frames between consecutive ticks: 0 = several ticks per frame, 1 = one per frame, >1 = our script skipped frames.
    // - Direct native probe: GET_CHAR_HEALTH's own handler called from our thread (read-only: pool lookup + ped
    //   vfunc) versus the SHDN call, timed and compared. Nothing here changes game state or gameplay.
    internal sealed class EngineThreadProbe
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeHandler(IntPtr context);

        private readonly IntPtr frameCounter;
        private readonly NativeHandler healthHandler;
        private readonly IntPtr block; // context +0 (result ptr, arg count, args ptr), args +16, result +48
        private int ticks, advancedDuringTick, sameFrame, nextFrame, skippedFrames, lastTickFrame = int.MinValue;

        internal EngineThreadProbe(uint frameCounterGlobal, uint getCharHealthHandler)
        {
            frameCounter = new IntPtr((int)frameCounterGlobal);
            if (getCharHealthHandler != 0)
            {
                healthHandler = (NativeHandler)Marshal.GetDelegateForFunctionPointer(new IntPtr((int)getCharHealthHandler), typeof(NativeHandler));
            }
            block = Marshal.AllocHGlobal(64);
        }

        internal int FrameCount { get { return Marshal.ReadInt32(frameCounter); } }

        internal void ObserveTick(int frameAtStart)
        {
            int frameAtEnd = FrameCount;
            ticks++;
            if (frameAtEnd != frameAtStart) { advancedDuringTick++; }
            if (lastTickFrame != int.MinValue)
            {
                int step = frameAtStart - lastTickFrame;
                if (step <= 0) { sameFrame++; } else if (step == 1) { nextFrame++; } else { skippedFrames++; }
            }
            lastTickFrame = frameAtEnd;
        }

        // GET_CHAR_HEALTH(ped, int* health) through its handler: args[0] = handle, args[1] = pointer to the result.
        internal int DirectHealth(int pedHandle)
        {
            int args = block.ToInt32() + 16, result = block.ToInt32() + 48;
            Marshal.WriteInt32(block, 0, result);
            Marshal.WriteInt32(block, 4, 2);
            Marshal.WriteInt32(block, 8, args);
            Marshal.WriteInt32(new IntPtr(args), pedHandle);
            Marshal.WriteInt32(new IntPtr(args + 4), result);
            Marshal.WriteInt32(new IntPtr(result), int.MinValue);
            healthHandler(block);
            return Marshal.ReadInt32(new IntPtr(result));
        }

        // Compares 20 direct calls with 20 SHDN calls on the same ped (the player). Logged by the caller.
        internal string ProbeHealth(int pedHandle, Func<int> shdnHealth)
        {
            if (healthHandler == null) { return "direct_native unavailable"; }
            const int Calls = 20;
            int direct = 0, viaShdn = 0;
            long start = Stopwatch.GetTimestamp();
            for (int i = 0; i < Calls; i++) { direct = DirectHealth(pedHandle); }
            long directTicks = Stopwatch.GetTimestamp() - start;
            start = Stopwatch.GetTimestamp();
            for (int i = 0; i < Calls; i++) { viaShdn = shdnHealth(); }
            long shdnTicks = Stopwatch.GetTimestamp() - start;
            return "direct_native get_char_health direct=" + direct + " shdn=" + viaShdn + " match=" + (direct == viaShdn) +
                " direct_us=" + (directTicks * 1000000.0 / Stopwatch.Frequency / Calls).ToString("0.00") +
                " shdn_us=" + (shdnTicks * 1000000.0 / Stopwatch.Frequency / Calls).ToString("0.0");
        }

        internal string ReportAndReset()
        {
            string report = "engine_thread_probe ticks=" + ticks + " frame_advanced_during_tick=" + advancedDuringTick +
                " ticks_same_frame=" + sameFrame + " ticks_next_frame=" + nextFrame + " ticks_after_skipped_frames=" + skippedFrames;
            ticks = advancedDuringTick = sameFrame = nextFrame = skippedFrames = 0;
            return report;
        }
    }
}
