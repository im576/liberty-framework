using System;
using System.Diagnostics;
using System.Threading;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Performance
{
    // Background thread (never touches the game): notices an engine frame that runs longer than
    // watchdogStallMilliseconds, logs which phase (module) it is stuck in and writes one minidump per stall, so a
    // freeze leaves evidence the autopilot can read. Also samples process memory once a second for the governor.
    internal sealed class Watchdog
    {
        private readonly Func<string> phaseName;
        private readonly Func<string, bool> writeDump;
        private readonly int stallMs;
        private readonly Thread thread;
        private volatile bool stopping;
        private long frameStart;   // Stopwatch timestamp while a frame runs, 0 between frames
        private long reportedFrame;
        private long frameNumber;
        private MemoryProbe.Sample memory;
        private readonly object gate = new object();

        internal Watchdog(int stallMilliseconds, Func<string> phaseName, Func<string, bool> writeDump)
        {
            stallMs = stallMilliseconds;
            this.phaseName = phaseName;
            this.writeDump = writeDump;
            thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Name = "Liberty watchdog";
            thread.Priority = ThreadPriority.BelowNormal;
        }

        internal void Start() { thread.Start(); }

        internal void Stop() { stopping = true; }

        internal void FrameBegin(long frame)
        {
            Interlocked.Exchange(ref frameNumber, frame);
            Interlocked.Exchange(ref frameStart, Stopwatch.GetTimestamp());
        }

        internal void FrameEnd() { Interlocked.Exchange(ref frameStart, 0); }

        internal MemoryProbe.Sample Memory { get { lock (gate) { return memory; } } }

        private void Run()
        {
            int ticks = 0;
            while (!stopping)
            {
                Thread.Sleep(250);
                ticks++;
                try
                {
                    if (ticks % 4 == 0)
                    {
                        // The address-space walk runs every 10 s; the counters every second.
                        MemoryProbe.Sample sample = MemoryProbe.Take(ticks % 40 == 0 || memory.LargestFreeBlockBytes == 0, memory.LargestFreeBlockBytes);
                        lock (gate) { memory = sample; }
                    }
                    long start = Interlocked.Read(ref frameStart);
                    if (start == 0) { continue; }
                    double elapsedMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
                    long frame = Interlocked.Read(ref frameNumber);
                    if (elapsedMs < stallMs || reportedFrame == frame) { continue; }
                    reportedFrame = frame;
                    string phase = phaseName();
                    RuntimeLog.Error("engine_stall frame=" + frame + " elapsed_ms=" + elapsedMs.ToString("0") + " phase=" + phase);
                    bool dumped = writeDump("engine frame stalled " + elapsedMs.ToString("0") + " ms in " + phase);
                    RuntimeLog.Error("engine_stall_dump written=" + dumped);
                }
                catch (Exception error) { RuntimeLog.Error("watchdog_failed error=" + error.Message); }
            }
        }
    }
}
