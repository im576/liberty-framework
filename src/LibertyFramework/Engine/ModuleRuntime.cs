using Liberty.Sdk;

namespace LibertyFramework.Engine
{
    // Engine bookkeeping for one loaded module: scheduling, crash phase, cost statistics and governor state.
    internal sealed class ModuleRuntime
    {
        internal ModuleRuntime(LibertyModule module, int phase, bool engineLevel, string sourcePath)
        {
            Module = module;
            Phase = phase;
            EngineLevel = engineLevel;
            SourcePath = sourcePath;
            CostName = "module." + module.Id;
        }

        // Replaced by hot reload and restart (Replace); the slot (phase, cost name) stays.
        internal LibertyModule Module { get; private set; }
        internal readonly int Phase;
        // Declares Capabilities.EngineInternal: measured and reported, never throttled (ours, tuned by hand).
        internal bool EngineLevel { get; private set; }
        // The mod assembly file this module came from; null for modules built into the engine assembly.
        internal readonly string SourcePath;
        internal readonly string CostName;
        internal bool Started;
        internal int LastTickMs;
        internal float BudgetMs;
        internal int Reloads;

        // Exponential moving average of update cost (alpha 0.05, about the last 20 updates) and the session maximum.
        internal float AverageMs;
        internal float MaxMs;
        internal long Updates;
        internal int OverBudgetStreak;
        internal int UnderBudgetStreak;
        internal bool Throttled;
        internal int Throttles;
        internal int IntervalBeforeThrottle;

        // A fresh instance takes over this slot with fresh statistics: the governor judges the new code on its own.
        internal void Replace(LibertyModule module)
        {
            Module = module;
            EngineLevel = module.Manifest.Has(Capabilities.EngineInternal);
            Started = false;
            AverageMs = 0; MaxMs = 0; Updates = 0; OverBudgetStreak = 0; UnderBudgetStreak = 0; Throttled = false; IntervalBeforeThrottle = 0;
            Reloads++;
        }

        internal ModuleManifest Manifest { get { return Module.Manifest; } }
        internal string Id { get { return Module.Id; } }
    }
}
