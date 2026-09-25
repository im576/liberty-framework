using System;
using System.Diagnostics;

namespace LibertyFramework.Core.Performance.Logic
{
    // One sample per gunplay tick. The interval is a frame pacing proxy; work is this script's CPU time.
    internal sealed class TickTimings
    {
        private readonly int[] frameMilliseconds = new int[201];
        private long lastStart;
        private int samples;
        private double workTotalMilliseconds;
        private double workMaxMilliseconds;
        private int framesOver33Milliseconds;
        private int framesOver50Milliseconds;

        internal void Observe(long start, long end)
        {
            if (lastStart != 0)
            {
                double frame = (start - lastStart) * 1000.0 / Stopwatch.Frequency;
                if (frame >= 0 && frame < 10000)
                {
                    int bucket = Math.Min(200, (int)Math.Round(frame));
                    frameMilliseconds[bucket]++;
                    samples++;
                    if (frame > 33.3) { framesOver33Milliseconds++; }
                    if (frame > 50) { framesOver50Milliseconds++; }
                    double work = Math.Max(0, (end - start) * 1000.0 / Stopwatch.Frequency);
                    workTotalMilliseconds += work;
                    workMaxMilliseconds = Math.Max(workMaxMilliseconds, work);
                }
            }
            lastStart = start;
        }

        internal string ReportAndReset()
        {
            string report = "samples=" + samples + " frame_p50_ms=" + Percentile(0.50) +
                " frame_p95_ms=" + Percentile(0.95) + " frame_p99_ms=" + Percentile(0.99) +
                " frames_over_33ms=" + framesOver33Milliseconds + " frames_over_50ms=" + framesOver50Milliseconds +
                " gunplay_avg_ms=" + (samples > 0 ? workTotalMilliseconds / samples : 0).ToString("0.000") +
                " gunplay_max_ms=" + workMaxMilliseconds.ToString("0.000");
            Array.Clear(frameMilliseconds, 0, frameMilliseconds.Length);
            samples = 0;
            workTotalMilliseconds = 0;
            workMaxMilliseconds = 0;
            framesOver33Milliseconds = 0;
            framesOver50Milliseconds = 0;
            return report;
        }

        private int Percentile(double fraction)
        {
            if (samples == 0) { return 0; }
            int target = (int)Math.Ceiling(samples * fraction);
            int total = 0;
            for (int bucket = 0; bucket < frameMilliseconds.Length; bucket++)
            {
                total += frameMilliseconds[bucket];
                if (total >= target) { return bucket; }
            }
            return 200;
        }
    }
}
