using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Liberty.Sdk;
using LibertyFramework.Engine;
using LibertyFramework.Engine.Services;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Performance.Logic;
using LibertyFramework.Arsenal.Ui;

namespace LibertyFramework.Verify.UiBudgetObservation
{
    internal static class UiBudgetObservationChecks
    {
        private sealed class Owner : LibertyModule { }
        private sealed class Sequence : IChoreography
        {
            public string Name { get { return "trunk"; } }
            public bool IsRunning { get; set; }
            public bool Completed { get; set; }
            public int StepIndex { get; set; }
            public void Cancel() { IsRunning = false; }
        }
        private static int passed, failed;
        private static void Check(bool ok, string name)
        { Console.WriteLine((ok ? "PASS " : "FAIL ") + name); if (ok) { passed++; } else { failed++; } }
        private static void Frames(PerfService perf, float milliseconds, int count)
        {
            MethodInfo record = typeof(PerfService).GetMethod("RecordFrame", BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < count; i++) { record.Invoke(perf, new object[] { milliseconds }); }
        }
        private static void Costs(string name, int count)
        { for (int i = 0; i < count; i++) { CostMeter.Add(name, Stopwatch.GetTimestamp() - Stopwatch.Frequency / (name == "draw.ui" ? 10000 : 1000)); } }
        private static string[] ReadLogs()
        {
            RuntimeLog.Flush();
            using (FileStream stream = new FileStream(Path.Combine(LibertyPaths.Root, "logs/LibertyFramework.log"),
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (StreamReader reader = new StreamReader(stream))
            { return reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); }
        }
        private static string Last(string starts)
        {
            string found = null;
            foreach (string line in ReadLogs()) { if (line.Contains("[INFO] " + starts)) { found = line; } }
            return found ?? "";
        }
        private static void Set(object target, string field, object value)
        { target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value); }
        private static int Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            LibertyPaths.Root = Path.Combine(args[0], Guid.NewGuid().ToString("N"));
            LibertyEngine engine = new LibertyEngine { Frame = 100 }; LibertyEngine.Current = engine;
            PerfService perf = new PerfService(engine);
            Owner owner = new Owner { Running = true };
            TrunkSequence trunk = new TrunkSequence(owner);
            Check(trunk.Observation().Contains("trunk_active=False") && trunk.Observation().Contains("trunk_step=-1"), "actual trunk no sequence state has no game calls");
            Sequence sequence = new Sequence { IsRunning = true, StepIndex = 4 };
            Set(trunk, "sequence", sequence); Set(trunk, "browsing", true);
            Check(trunk.Observation().Contains("trunk_step=4 browsing=True close_requested=False"), "actual trunk managed browsing state without queries");
            int reads = 0;
            perf.ObserveUiBudgetState(owner, () => { reads++; return trunk.Observation(); });
            Frames(perf, 99, 3); Costs("discarded.prewindow", 3);
            perf.UiBudgetCommand(new[] { "begin" });
            Check(reads == 1 && Last("ui_budget_begin").Contains("ui_window=1 frame=100"), "begin records one boundary state and existing counter reset");
            Frames(perf, 20, 40); Costs("paired.closed", 7);
            int linesBefore = ReadLogs().Length;
            Check(reads == 1 && linesBefore == 1, "sample updates cause no observational reads or per-frame file log");
            engine.Frame = 140;
            perf.UiBudgetCommand(new[] { "baseline" });
            string baseline = Last("ui_budget_baseline");
            Check(baseline.Contains("frames=40 avg_ms=20.00") && !baseline.Contains("discarded.prewindow"), "baseline frame percentiles retain exact existing window");
            Check(Regex.IsMatch(baseline, @"paired.closed=[^ ]+/7@") && baseline.Contains("ui_window=1 baseline_window=1"), "exact closed command costs retained with window identity");
            Check(baseline.Contains("start_frame=100 end_frame=140") && baseline.Contains("state_start=[trunk_active=True") && reads == 2, "baseline includes start and end choreography states");
            Check(!CostMeter.ReportAndReset(CostReader.Command).Contains("paired.closed"), "baseline still resets command costs once");
            Check(CostMeter.ReportAndReset(CostReader.Log).Contains("paired.closed"), "baseline observation never resets separate logger reader");
            perf.UiBudgetCommand(new[] { "begin" });
            Frames(perf, 20, 40); Costs("paired.open", 9); Costs("draw.ui", 40);
            Set(trunk, "closeRequested", true); engine.Frame = 180;
            string check = perf.UiBudgetCommand(new[] { "check", "trunk", "0.5", "1.10", "1.15", "30" });
            Check(check.StartsWith("ui_budget label=trunk pass=True") && check.Contains("ui_window=2 baseline_window=1"), "unchanged gate result pairs open with exact baseline");
            Check(Regex.IsMatch(check, @"paired.open=[^ ]+/9@") && !check.Contains("paired.closed") && check.Contains("frames=40 avg_ms=20.00"), "check costs and frames only contain open window");
            Check(check.Contains("state_start=[trunk_active=True") && check.Contains("close_requested=True") && reads == 4, "check records managed state transition at end only");
            Check(Last("ui_budget label=trunk") .EndsWith(check), "actual asynchronous RuntimeLog persists exact command receipt");
            Check(CostMeter.ReportAndReset(CostReader.Log).Contains("paired.open"), "check leaves periodic costs independent");
            perf.UiBudgetCommand(new[] { "begin" });
            Frames(perf, 30, 40); Costs("draw.ui", 40);
            string fail = perf.UiBudgetCommand(new[] { "check", "trunk", "0.5", "1.10", "1.15", "30" });
            Check(fail.StartsWith("error ui_budget label=trunk pass=False reason=frame_average") && fail.Contains("baseline_window=1"), "observation cannot hide unchanged budget failure");
            Check(perf.FrameStatsReportAndReset().StartsWith("frames=0") && !CostMeter.ReportAndReset(CostReader.Command).Contains("draw.ui"), "check leaves same empty histogram and command counters");
            int beforeInvalid = reads;
            Frames(perf, 18, 2); Costs("invalid.retained", 2);
            try { perf.UiBudgetCommand(new[] { "check", "trunk", "0", "1.10", "1.15", "30" }); }
            catch (ArgumentException) { RuntimeLog.Info("test invalid command rejected"); }
            Check(reads == beforeInvalid && perf.FrameStatsReportAndReset().StartsWith("frames=2") && CostMeter.ReportAndReset(CostReader.Command).Contains("invalid.retained"), "invalid args retain pre-existing no-reset behavior");
            sequence.IsRunning = false; sequence.Completed = true; sequence.StepIndex = 6;
            Check(trunk.Observation().Contains("trunk_active=False trunk_completed=True trunk_step=6"), "actual completed choreography distinguishable from active browsing");
            engine.Ledger.ReleaseAll(owner);
            perf.UiBudgetCommand(new[] { "begin" });
            Check(Last("ui_budget_begin").Contains("state=[unavailable]") && reads == beforeInvalid && engine.Ledger.Count == 0, "owner ledger cleanup removes state observer");
            perf.ObserveUiBudgetState(owner, () => { throw new InvalidOperationException("observation fixture"); });
            perf.UiBudgetCommand(new[] { "begin" });
            Check(Last("ui_budget_begin").Contains("state=[unavailable]") && string.Join("\n", ReadLogs()).Contains("[ERROR] ui_budget_state_failed"), "observer failure logged without interrupting metric command");
            engine.Ledger.ReleaseAll(owner);
            perf.ObserveUiBudgetState(owner, () => "old");
            Owner replacement = new Owner { Running = true };
            perf.ObserveUiBudgetState(replacement, () => "replacement");
            engine.Ledger.ReleaseAll(owner); perf.UiBudgetCommand(new[] { "begin" });
            Check(Last("ui_budget_begin").Contains("state=[replacement]"), "old owner cleanup preserves replacement observer");
            replacement.Running = false; perf.UiBudgetCommand(new[] { "begin" });
            Check(Last("ui_budget_begin").Contains("state=[unavailable]"), "stopped owner cannot supply stale state");
            engine.Ledger.ReleaseAll(replacement);
            PerfService tail = new PerfService(engine);
            tail.UiBudgetCommand(new[] { "begin" });
            Frames(tail, 24, 370); Frames(tail, 40.6f, 20); Frames(tail, 67.9f, 4); Costs("tail.closed", 394);
            tail.UiBudgetCommand(new[] { "baseline" });
            string tailBaseline = Last("ui_budget_baseline");
            tail.UiBudgetCommand(new[] { "begin" });
            Frames(tail, 24, 356); Frames(tail, 46.3f, 19); Frames(tail, 99.6f, 4); Costs("draw.ui", 379);
            string tailFailure = tail.UiBudgetCommand(new[] { "check", "tail", "0.5", "1.10", "1.15", "30" });
            Check(tailBaseline.Contains("frames=394") && tailBaseline.Contains("tail.closed=") && tailFailure.Contains("frames=379") && tailFailure.Contains("draw.ui="), "paired unequal frame windows retain exact costs on failing tail");
            Check(tailFailure.StartsWith("error ui_budget label=tail pass=False reason=frame_p95") && tailFailure.Contains("baseline_window=1"), "original relative p95 gate fails even with compliant mean and draw average");
            Check(Last("ui_budget label=tail").EndsWith(tailFailure.Substring("error ".Length)) && !tailFailure.Contains("tail.closed="), "actual logger preserves strict failing result and separate paired baseline");
            Console.WriteLine("UI budget observation focused checks: " + passed + " passed / " + failed + " failed");
            Console.WriteLine("Actual logger evidence: " + LibertyPaths.Root);
            return failed == 0 ? 0 : 1;
        }
    }
}
