using LibertyFramework.Arsenal.Contracts;

namespace LibertyFramework.Verify
{
    // Offline tests for Arsenal Core (T-020). Owned by Agent A; add checks here.
    internal static class ArsenalCoreChecks
    {
        internal static void Run(string repoRoot, Checker check)
        {
            WeaponRecord record = new WeaponRecord();
            record.WeaponId = 58;
            record.Owned = true;
            check.True("arsenal contract: weapon record clones", record.Clone().WeaponId == 58 && record.Clone() != record, "");
        }
    }
}
