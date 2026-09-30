using System.Runtime.Serialization;

#pragma warning disable 0649
namespace LibertyFramework.Weapons.Logic
{
    // The WeaponInfo.xml fields a Stage 1 weapon's identity sets (T-041). Null = leave the game's own value.
    // Applied to the installed update\common\data\WeaponInfo.xml by tools/PackageMerge.psm1 (Merge-WeaponInfoStats);
    // the running game only reads them back to prove the file it loaded matches the catalog.
    [DataContract]
    internal sealed class WeaponStats
    {
        [DataMember(Name = "timeBetweenShotsMilliseconds", IsRequired = false)] internal int? TimeBetweenShotsMilliseconds;
        [DataMember(Name = "damageBase", IsRequired = false)] internal int? DamageBase;
        [DataMember(Name = "clipSize", IsRequired = false)] internal int? ClipSize;
        [DataMember(Name = "ammoMax", IsRequired = false)] internal int? AmmoMax;

        // The values that differ, as "field expected/actual" text; empty when they agree (fields the catalog leaves null are skipped).
        internal string Differences(WeaponStats actual)
        {
            System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
            Compare(parts, "timebetweenshots", TimeBetweenShotsMilliseconds, actual == null ? null : actual.TimeBetweenShotsMilliseconds);
            Compare(parts, "damage", DamageBase, actual == null ? null : actual.DamageBase);
            Compare(parts, "clipsize", ClipSize, actual == null ? null : actual.ClipSize);
            Compare(parts, "ammomax", AmmoMax, actual == null ? null : actual.AmmoMax);
            return string.Join(" ", parts.ToArray());
        }

        private static void Compare(System.Collections.Generic.List<string> parts, string field, int? expected, int? actual)
        {
            if (expected.HasValue && expected != actual) { parts.Add(field + " " + expected + "/" + (actual.HasValue ? actual.ToString() : "-")); }
        }
    }
}
