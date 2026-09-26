using System;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Performance
{
    // Per-module budgets and the global pressure signal (ADR-0006 §performance).
    //  - A mod (no Capabilities.EngineInternal) whose average update cost stays over its budget for 120 updates is
    //    throttled to engine.json throttledIntervalMs; it is restored after 600 updates under half its budget. One that
    //    stays over ten times its budget while throttled is stopped (logged, ModuleFailed published).
    //  - Pressure (0..1) rises when frames are slower than targetFrameMs or free address space falls below
    //    lowAddressSpaceMegabytes; modules scale optional work by (1 - Pressure).
    internal sealed class Governor
    {
        private const float Alpha = 0.05f;
        private const int ThrottleAfter = 120, RestoreAfter = 600, StopAfter = 300;
        private readonly EngineConfig config;
        private float pressure;

        internal Governor(EngineConfig config) { this.config = config; }

        internal float Pressure { get { return pressure; } }

        // Returns a reason when the module should be stopped, else null.
        internal string Record(ModuleRuntime m, float ms)
        {
            m.Updates++;
            m.AverageMs = m.Updates == 1 ? ms : m.AverageMs + (ms - m.AverageMs) * Alpha;
            if (ms > m.MaxMs) { m.MaxMs = ms; }
            if (!config.GovernorEnabled || m.EngineLevel) { return null; }
            float budget = m.BudgetMs;
            if (m.AverageMs > budget) { m.OverBudgetStreak++; m.UnderBudgetStreak = 0; }
            else if (m.AverageMs < budget / 2) { m.UnderBudgetStreak++; m.OverBudgetStreak = 0; }
            if (!m.Throttled && m.OverBudgetStreak >= ThrottleAfter)
            {
                m.Throttled = true;
                m.Throttles++;
                m.IntervalBeforeThrottle = m.Module.Interval;
                m.Module.Interval = Math.Max(m.Module.Interval, config.ThrottledIntervalMs);
                m.OverBudgetStreak = 0;
                RuntimeLog.Error("governor_throttle module=" + m.Id + " avg_ms=" + m.AverageMs.ToString("0.00") + " budget_ms=" + budget.ToString("0.00") +
                    " interval_ms=" + m.Module.Interval);
            }
            else if (m.Throttled && m.UnderBudgetStreak >= RestoreAfter)
            {
                m.Throttled = false;
                m.Module.Interval = m.IntervalBeforeThrottle;
                m.UnderBudgetStreak = 0;
                RuntimeLog.Info("governor_restore module=" + m.Id + " avg_ms=" + m.AverageMs.ToString("0.00"));
            }
            else if (m.Throttled && m.AverageMs > budget * 10 && m.OverBudgetStreak >= StopAfter)
            {
                return "over budget: avg " + m.AverageMs.ToString("0.00") + " ms (budget " + budget.ToString("0.00") + " ms) while throttled";
            }
            return null;
        }

        // Once per frame with the smoothed frame time and the latest free address space.
        internal void Update(float frameMs, long addressSpaceFreeBytes)
        {
            float target = config.TargetFrameMs;
            float fromFrames = Clamp01((frameMs - target) / target);
            float fromMemory = 0f;
            long low = config.LowAddressSpaceMegabytes * 1024L * 1024L;
            if (addressSpaceFreeBytes > 0 && addressSpaceFreeBytes < low) { fromMemory = Clamp01(1f - (float)addressSpaceFreeBytes / low) * 2f; }
            float wanted = Math.Min(1f, Math.Max(fromFrames, fromMemory));
            // Rise quickly, fall slowly, so optional effects do not flicker on and off.
            pressure += (wanted - pressure) * (wanted > pressure ? 0.2f : 0.01f);
        }

        private static float Clamp01(float value) { return value < 0 ? 0 : value > 1 ? 1 : value; }
    }
}
