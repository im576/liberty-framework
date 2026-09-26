using System;
using System.Collections.Generic;
using System.Diagnostics;
using Liberty.Sdk;
using LibertyFramework.Core.Performance.Logic;
using LibertyFramework.Engine.Performance;

namespace LibertyFramework.Engine.Services
{
    // SDK IPerf: named cost samples (CostMeter; names are cached so End does not allocate), smoothed frame time and
    // its 95th percentile, the governor's pressure, and the memory figures the watchdog samples.
    public sealed class PerfService : IPerf
    {
        private const int Window = 240;
        private readonly LibertyEngine engine;
        private readonly Dictionary<LibertyModule, Dictionary<string, string>> names = new Dictionary<LibertyModule, Dictionary<string, string>>();
        private readonly float[] frames = new float[Window];
        private readonly float[] sorted = new float[Window];
        private int frameIndex, frameCount;
        private long lastFrame;
        private float frameMs, p95Ms;

        internal PerfService(LibertyEngine engine) { this.engine = engine; }

        public long Begin() { return Stopwatch.GetTimestamp(); }

        public void End(LibertyModule owner, string name, long token)
        {
            Dictionary<string, string> table;
            if (!names.TryGetValue(owner, out table)) { table = new Dictionary<string, string>(); names[owner] = table; }
            string key;
            if (!table.TryGetValue(name, out key)) { key = owner.Id + "." + name; table[name] = key; }
            CostMeter.Add(key, token);
        }

        public float FrameMs { get { return frameMs; } }
        public float FrameP95Ms { get { return p95Ms; } }
        public float Pressure { get { return engine.Governor.Pressure; } }
        public long ProcessPrivateBytes { get { return engine.Watchdog.Memory.PrivateBytes; } }
        public long AddressSpaceFreeBytes { get { return engine.Watchdog.Memory.AddressSpaceFreeBytes; } }
        public long ManagedHeapBytes { get { return GC.GetTotalMemory(false); } }
        internal long LargestFreeBlockBytes { get { return engine.Watchdog.Memory.LargestFreeBlockBytes; } }

        // Engine: once per frame, at the start of the frame (the interval between engine ticks is the frame time).
        internal void OnFrame()
        {
            long now = Stopwatch.GetTimestamp();
            if (lastFrame != 0)
            {
                float ms = (float)((now - lastFrame) * 1000.0 / Stopwatch.Frequency);
                // Pauses and loading screens stop the tick; do not let them poison the average.
                if (ms < 1000f)
                {
                    frameMs = frameMs == 0 ? ms : frameMs + (ms - frameMs) * 0.05f;
                    frames[frameIndex] = ms;
                    frameIndex = (frameIndex + 1) % Window;
                    if (frameCount < Window) { frameCount++; }
                    if (frameIndex % 60 == 0) { p95Ms = Percentile95(); }
                }
            }
            lastFrame = now;
        }

        private float Percentile95()
        {
            Array.Copy(frames, sorted, frameCount);
            Array.Sort(sorted, 0, frameCount);
            return sorted[Math.Min(frameCount - 1, (int)(frameCount * 0.95f))];
        }
    }
}
