using System;
using System.IO;
using System.Runtime.Serialization;

#pragma warning disable 0649
namespace LibertyFramework.CombatEffects
{
    [DataContract]
    internal sealed class CombatEffectsConfig
    {
        [DataMember(Name="schemaVersion", IsRequired=true)] internal int SchemaVersion;
        [DataMember(Name="enabled", IsRequired=true)] internal bool Enabled;
        [DataMember(Name="reactionsEnabled", IsRequired=true)] internal bool ReactionsEnabled;
        [DataMember(Name="injuriesEnabled", IsRequired=true)] internal bool InjuriesEnabled;
        [DataMember(Name="woundsEnabled", IsRequired=true)] internal bool WoundsEnabled;
        [DataMember(Name="limbLossPrototypeEnabled", IsRequired=true)] internal bool LimbLossPrototypeEnabled;
        [DataMember(Name="scanRadiusMeters", IsRequired=true)] internal float ScanRadiusMeters;
        [DataMember(Name="sampleIntervalMilliseconds", IsRequired=true)] internal int SampleIntervalMilliseconds;
        [DataMember(Name="maximumTrackedPeds", IsRequired=true)] internal int MaximumTrackedPeds;
        [DataMember(Name="maximumWoundsPerPed", IsRequired=true)] internal int MaximumWoundsPerPed;
        [DataMember(Name="woundLifetimeMilliseconds", IsRequired=true)] internal int WoundLifetimeMilliseconds;
        [DataMember(Name="reactionCooldownMilliseconds", IsRequired=true)] internal int ReactionCooldownMilliseconds;
        [DataMember(Name="minimumInjuryDamage", IsRequired=true)] internal int MinimumInjuryDamage;
        [DataMember(Name="minimumLimbLossDamage", IsRequired=true)] internal int MinimumLimbLossDamage;
        [DataMember(Name="minimumLimbLossHits", IsRequired=true)] internal int MinimumLimbLossHits;
        [DataMember(Name="allowedWeaponIds", IsRequired=true)] internal int[] AllowedWeaponIds;

        internal void Validate()
        {
            if (SchemaVersion != 1 || ScanRadiusMeters <= 0 || ScanRadiusMeters > 100 ||
                SampleIntervalMilliseconds < 25 || SampleIntervalMilliseconds > 1000 ||
                MaximumTrackedPeds < 1 || MaximumTrackedPeds > 64 || MaximumWoundsPerPed < 1 || MaximumWoundsPerPed > 16 ||
                WoundLifetimeMilliseconds < 100 || WoundLifetimeMilliseconds > 600000 ||
                ReactionCooldownMilliseconds < 0 || ReactionCooldownMilliseconds > 30000 ||
                MinimumInjuryDamage < 1 || MinimumLimbLossDamage < 1 || MinimumLimbLossHits < 1 ||
                AllowedWeaponIds == null || AllowedWeaponIds.Length == 0)
                throw new InvalidDataException("combat_effects.json invalid bounds");
            foreach (int id in AllowedWeaponIds)
                if (id < 58 || id > 255) throw new InvalidDataException("combat effects requires registered test weapon IDs");
        }
    }
}
