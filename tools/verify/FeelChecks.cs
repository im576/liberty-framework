using LibertyFramework.Arsenal.Contracts;

namespace LibertyFramework.Verify
{
    // Offline tests for Feel & Presentation (T-011, T-013..T-017, T-021). Owned by Agent B; add checks here.
    internal static class FeelChecks
    {
        internal static void Run(string repoRoot, Checker check)
        {
            check.True("holster contract: five body slots", (int)BodySlot.Melee == 5, "");
        }
    }
}
