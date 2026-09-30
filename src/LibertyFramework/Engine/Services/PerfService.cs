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
                    RecordFrame(ms);
                    frameMs = frameMs == 0 ? ms : frameMs + (ms - frameMs) * 0.05f;
                    frames[frameIndex] = ms;
                    frameIndex = (frameIndex + 1) % Window;
                    if (frameCount < Window) { frameCount++; }
                    if (frameIndex % 60 == 0) { p95Ms = Percentile95(); }
                }
                else { histogramStalls++; }
            }
            lastFrame = now;
        }

        // ---- frame statistics for a whole measured run (T-040): a histogram of every frame since the last report, not
        // the 240-frame window above, so a 60-second sample yields real p50/p95/p99. Bins are 0.1 ms wide up to 250 ms.
        private const float BinMs = 0.1f;
        private const int Bins = 2500;
        private readonly int[] histogram = new int[Bins + 1];
        private int histogramFrames, histogramOver33, histogramOver100, histogramStalls;
        private double histogramTotalMs;
        private float histogramMaxMs;

        private void RecordFrame(float ms)
        {
            histogram[Math.Min(Bins, (int)(ms / BinMs))]++;
            histogramFrames++;
            histogramTotalMs += ms;
            if (ms > histogramMaxMs) { histogramMaxMs = ms; }
            if (ms > 33.4f) { histogramOver33++; }
            if (ms > 100f) { histogramOver100++; }
        }

        // A frame of 1 s or more is a stall or a load: excluded from the figures above, counted in stalls_1s.
        private float HistogramPercentile(double fraction)
        {
            int target = (int)Math.Ceiling(histogramFrames * fraction), seen = 0;
            for (int bin = 0; bin <= Bins; bin++)
            {
                seen += histogram[bin];
                if (seen >= target) { return (bin + 1) * BinMs; }
            }
            return histogramMaxMs;
        }

        // "frames=N avg_ms= p50_ms= p95_ms= p99_ms= max_ms= over33_ms= over100_ms= stalls_1s=" for the frames since the last
        // call, then starts a new interval (like "costs").
        internal string FrameStatsReportAndReset()
        {
            string text = histogramFrames == 0 ? "frames=0" :
                "frames=" + histogramFrames + " avg_ms=" + (histogramTotalMs / histogramFrames).ToString("0.00") +
                " p50_ms=" + HistogramPercentile(0.50).ToString("0.0") + " p95_ms=" + HistogramPercentile(0.95).ToString("0.0") +
                " p99_ms=" + HistogramPercentile(0.99).ToString("0.0") + " max_ms=" + histogramMaxMs.ToString("0.0") +
                " over33_ms=" + histogramOver33 + " over100_ms=" + histogramOver100;
            text += " stalls_1s=" + histogramStalls;
            Array.Clear(histogram, 0, histogram.Length);
            histogramFrames = histogramOver33 = histogramOver100 = histogramStalls = 0;
            histogramTotalMs = 0; histogramMaxMs = 0;
            return text;
        }

        private float Percentile95()
        {
            Array.Copy(frames, sorted, frameCount);
            Array.Sort(sorted, 0, frameCount);
            return sorted[Math.Min(frameCount - 1, (int)(frameCount * 0.95f))];
        }
    }
}
