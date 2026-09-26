using System.Collections.Generic;
using System.Linq;
using Liberty.Sdk;
using LibertyFramework.Engine.Core;
using LibertyFramework.Engine.Performance;

namespace LibertyFramework.Engine.Ui
{
    // M5 visual inspector (lf inspector on|off): the engine's live numbers and one row per module (state, cost, interval,
    // owned resources, reloads), on screen for developers and autopilot screenshots. The engine tick copies everything
    // into an immutable snapshot at most every RefreshMs; the draw pass only reads that copy (never engine or game state).
    internal sealed class InspectorOverlay
    {
        private sealed class Row
        {
            internal string Id, State;
            internal float AverageMs, MaxMs, BudgetMs;
            internal int IntervalMs, Owned, Reloads, Throttles;
            internal bool Running, Throttled;
        }

        private sealed class Snapshot
        {
            internal string[] Header;
            internal Row[] Rows;
        }

        // Developer display refresh; not a gameplay value.
        private const int RefreshMs = 500;
        private const float PanelWidth = 560, RowHeight = 17, TopMargin = 90, RightMargin = 24;
        private static readonly Rgba Background = new Rgba(8, 10, 14, 205);
        private static readonly Rgba Heading = new Rgba(120, 190, 255, 255);
        private static readonly Rgba Dim = new Rgba(170, 176, 186, 255);
        private static readonly Rgba Good = new Rgba(120, 220, 140, 255);
        private static readonly Rgba Warn = new Rgba(255, 196, 80, 255);
        private static readonly Rgba Bad = new Rgba(255, 110, 100, 255);

        private volatile Snapshot shown;
        private int lastRefreshMs;

        internal bool Visible { get; set; }

        internal void Refresh(LibertyEngine engine, int nowMs, int reloads)
        {
            if (!Visible) { shown = null; return; }
            if (shown != null && unchecked(nowMs - lastRefreshMs) < RefreshMs) { return; }
            lastRefreshMs = nowMs;
            MemoryProbe.Sample memory = engine.Watchdog.Memory;
            LcPools pools = engine.World.Pools;
            LcRayStats rays = engine.Core.RaycastStats();
            string[] header =
            {
                "Liberty engine " + LibertyEngine.Version + "  sdk " + SdkVersion.Text + "  frame " + engine.Frame + "  episode " + engine.Episode +
                    "  core " + (engine.UsingCore ? "on" : "off"),
                "frame " + engine.Perf.FrameMs.ToString("0.0") + " ms  p95 " + engine.Perf.FrameP95Ms.ToString("0.0") + " ms  pressure " +
                    engine.Perf.Pressure.ToString("0.00") + "  core " + engine.World.CoreMicroseconds.ToString("0") + " us  rays " +
                    (engine.Query.RaycastAvailable ? rays.Queries + " (" + rays.Tests + " tests)" : "off"),
                "address free " + (memory.AddressSpaceFreeBytes >> 20) + " MB (largest " + (memory.LargestFreeBlockBytes >> 20) + ")  managed " +
                    (engine.Perf.ManagedHeapBytes >> 20) + " MB  private " + (memory.PrivateBytes >> 20) + " MB",
                "peds " + engine.World.Peds.Count + "/" + pools.PedsUsed + "  vehicles " + engine.World.Vehicles.Count + "/" + pools.VehiclesUsed +
                    "  objects " + pools.ObjectsUsed + "  resources " + engine.Ledger.Count + "  coroutines " + engine.Scheduler.Running + "  reloads " + reloads,
            };
            List<Row> rows = new List<Row>();
            foreach (ModuleRuntime m in engine.Runtimes)
            {
                LibertyModule module = m.Module;
                rows.Add(new Row
                {
                    Id = m.Id,
                    Running = module.Running,
                    State = module.Running ? (m.Throttled ? "throttled" : "running") : (module.FailureReason ?? "stopped"),
                    AverageMs = m.AverageMs,
                    MaxMs = m.MaxMs,
                    BudgetMs = m.BudgetMs,
                    IntervalMs = module.Interval,
                    Owned = engine.Ledger.Summary(module).Values.Sum(),
                    Reloads = m.Reloads,
                    Throttles = m.Throttles,
                    Throttled = m.Throttled,
                });
            }
            shown = new Snapshot { Header = header, Rows = rows.ToArray() };
        }

        internal void Draw(ICanvas canvas)
        {
            Snapshot s = shown;
            if (!Visible || s == null) { return; }
            float x = canvas.Width - PanelWidth - RightMargin, y = TopMargin;
            float height = (s.Header.Length + s.Rows.Length + 2) * RowHeight + 16;
            canvas.Rect(x, y, PanelWidth, height, Background);
            float line = y + 8;
            foreach (string text in s.Header)
            {
                canvas.Text(text, x + 10, line, PanelWidth - 20, RowHeight, TextStyle.Small, TextAlign.Left, Dim);
                line += RowHeight;
            }
            line += RowHeight * 0.5f;
            Column(canvas, x, line, Heading, "module", "state", "avg ms", "max ms", "int", "owned", "reloads");
            line += RowHeight;
            foreach (Row row in s.Rows)
            {
                Rgba colour = !row.Running ? Bad : (row.Throttled || row.AverageMs > row.BudgetMs) ? Warn : Good;
                Column(canvas, x, line, colour, row.Id, row.State, row.AverageMs.ToString("0.000"), row.MaxMs.ToString("0.00"),
                    row.IntervalMs.ToString(), row.Owned.ToString(), row.Reloads.ToString());
                line += RowHeight;
            }
        }

        private static void Column(ICanvas canvas, float x, float y, Rgba colour, string id, string state, string average, string max,
            string interval, string owned, string reloads)
        {
            canvas.Text(id, x + 10, y, 130, RowHeight, TextStyle.Small, TextAlign.Left, colour);
            canvas.Text(state.Length > 20 ? state.Substring(0, 20) + "…" : state, x + 140, y, 150, RowHeight, TextStyle.Small, TextAlign.Left, colour);
            canvas.Text(average, x + 290, y, 55, RowHeight, TextStyle.Small, TextAlign.Right, colour);
            canvas.Text(max, x + 345, y, 55, RowHeight, TextStyle.Small, TextAlign.Right, colour);
            canvas.Text(interval, x + 400, y, 45, RowHeight, TextStyle.Small, TextAlign.Right, colour);
            canvas.Text(owned, x + 445, y, 50, RowHeight, TextStyle.Small, TextAlign.Right, colour);
            canvas.Text(reloads, x + 495, y, 55, RowHeight, TextStyle.Small, TextAlign.Right, colour);
        }
    }
}
