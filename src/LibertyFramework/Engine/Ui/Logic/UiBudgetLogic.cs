using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LibertyFramework.Engine.Ui.Logic
{
    // Acceptance uses measured frame intervals as well as draw submission cost: queued drawing can be cheap to submit
    // while blocking the game's later render pass. Missing, empty or invalid samples never establish acceptance.
    internal static class UiBudgetLogic
    {
        internal static string Evaluate(string frames, string costs, string baseline, double drawBudgetMs,
            double p95Ratio, double p99Ratio, int minimumFrames)
        {
            double count = Metric(frames, "frames"), baselineCount = Metric(baseline, "frames");
            if (!(count >= minimumFrames) || !(baselineCount >= minimumFrames)) { return "insufficient_frames"; }
            if (Metric(frames, "stalls_1s") != 0) { return "stall"; }
            if (!(Metric(frames, "avg_ms") <= Metric(baseline, "avg_ms") * p95Ratio)) { return "frame_average"; }
            if (!(Metric(frames, "p95_ms") <= Metric(baseline, "p95_ms") * p95Ratio)) { return "frame_p95"; }
            if (!(Metric(frames, "p99_ms") <= Metric(baseline, "p99_ms") * p99Ratio)) { return "frame_p99"; }
            Match draw = Regex.Match(costs ?? "", @"(?:^|\s)draw\.ui=([0-9.]+)/[0-9.]+/(\d+)@");
            double average;
            int drawCount;
            if (!draw.Success || !double.TryParse(draw.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out average) ||
                !int.TryParse(draw.Groups[2].Value, out drawCount) || drawCount < minimumFrames) { return "missing_draw_samples"; }
            if (!(average <= drawBudgetMs)) { return "draw_average"; }
            return null;
        }

        private static double Metric(string report, string name)
        {
            Match match = Regex.Match(report ?? "", @"(?:^|\s)" + Regex.Escape(name) + @"=([0-9.]+)(?:\s|$)");
            double value;
            return match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : double.NaN;
        }
    }
}
