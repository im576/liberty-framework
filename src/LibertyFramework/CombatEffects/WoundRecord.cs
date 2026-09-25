using GTA;

namespace LibertyFramework.CombatEffects
{
    internal sealed class WoundRecord
    {
        internal HitRegion Region;
        internal Vector3 ApproximatePosition;
        internal long CreatedMilliseconds;
        internal int Damage;
    }
}
