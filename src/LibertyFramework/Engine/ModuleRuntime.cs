using Liberty.Sdk;

namespace LibertyFramework.Engine
{
    // Engine bookkeeping for one loaded module: scheduling, crash phase, cost statistics and governor state.
    internal sealed class ModuleRuntime
    {
        internal ModuleRuntime(LibertyModule module, int phase, bool engineLevel)
        {
            Module = module;
            Phase = phase;
            EngineLevel = engineLevel;
            CostName = "module." + module.Id;
        }

        internal readonly LibertyModule Module;
        internal readonly int Phase;
        // Declares Capabilities.EngineInternal: measured and reported, never throttled (ours, tuned by hand).
        internal readonly bool EngineLevel;
        internal readonly string CostName;
        internal bool Started;
        internal int LastTickMs;
        internal float BudgetMs;

        // Exponential moving average of update cost (alpha 0.05, about the last 20 updates) and the session maximum.
        internal float AverageMs;
        internal float MaxMs;
        internal long Updates;
        internal int OverBudgetStreak;
        internal int UnderBudgetStreak;
        internal bool Throttled;
        internal int Throttles;
        internal int IntervalBeforeThrottle;

        internal ModuleManifest Manifest { get { return Module.Manifest; } }
        internal string Id { get { return Module.Id; } }
    }
}
