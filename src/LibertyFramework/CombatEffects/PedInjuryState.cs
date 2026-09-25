using System.Collections.Generic;

namespace LibertyFramework.CombatEffects
{
    internal sealed class PedInjuryState
    {
        internal int LastHealth;
        internal long LastReactionMilliseconds;
        internal readonly List<WoundRecord> Wounds = new List<WoundRecord>();
        internal readonly Dictionary<HitRegion, int> RegionHits = new Dictionary<HitRegion, int>();
        internal readonly HashSet<HitRegion> LostLimbs = new HashSet<HitRegion>();
    }
}
