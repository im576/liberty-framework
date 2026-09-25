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
        [DataMember(Name="headLossPrototypeEnabled", IsRequired=true)] internal bool HeadLossPrototypeEnabled;
        [DataMember(Name="impactEffectName", IsRequired=true)] internal string ImpactEffectName;
        [DataMember(Name="woundEffectName", IsRequired=true)] internal string WoundEffectName;
        [DataMember(Name="reactionForceHead", IsRequired=true)] internal float ReactionForceHead;
        [DataMember(Name="reactionForceTorso", IsRequired=true)] internal float ReactionForceTorso;
        [DataMember(Name="reactionForceArm", IsRequired=true)] internal float ReactionForceArm;
        [DataMember(Name="reactionForceLeg", IsRequired=true)] internal float ReactionForceLeg;
        [DataMember(Name="reactionVerticalFraction", IsRequired=true)] internal float ReactionVerticalFraction;
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
        // T-022 dismemberment (optional fields; absent = off). A lethal limb hit collapses that limb's bones.
        [DataMember(Name="dismembermentEnabled", IsRequired=false)] internal bool DismembermentEnabled;
        [DataMember(Name="severedLimbEnabled", IsRequired=false)] internal bool SeveredLimbEnabled;
        [DataMember(Name="severedLimbForce", IsRequired=false)] internal float SeveredLimbForce;
        [DataMember(Name="severedLimbLifetimeMilliseconds", IsRequired=false)] internal int SeveredLimbLifetimeMilliseconds;
        [DataMember(Name="maximumSeveredPeds", IsRequired=false)] internal int MaximumSeveredPeds;
        [DataMember(Name="severedLimbLifetimeCorpseMilliseconds", IsRequired=false)] internal int SeveredCorpseLifetimeMilliseconds;
        [DataMember(Name="stumpEffectName", IsRequired=false)] internal string StumpEffectName;

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
            if (string.IsNullOrEmpty(ImpactEffectName) || string.IsNullOrEmpty(WoundEffectName) ||
                ReactionForceHead < 0 || ReactionForceHead > 10 || ReactionForceTorso < 0 || ReactionForceTorso > 10 ||
                ReactionForceArm < 0 || ReactionForceArm > 10 || ReactionForceLeg < 0 || ReactionForceLeg > 10 ||
                ReactionVerticalFraction < 0 || ReactionVerticalFraction > 1)
                throw new InvalidDataException("combat effects visual/reaction bounds invalid");
            if (DismembermentEnabled && (MaximumSeveredPeds < 1 || MaximumSeveredPeds > 16 || string.IsNullOrEmpty(StumpEffectName) ||
                SeveredCorpseLifetimeMilliseconds < 1000 || SeveredCorpseLifetimeMilliseconds > 600000 ||
                (SeveredLimbEnabled && (SeveredLimbForce < 0 || SeveredLimbForce > 30 || SeveredLimbLifetimeMilliseconds < 1000 || SeveredLimbLifetimeMilliseconds > 600000))))
                throw new InvalidDataException("combat effects dismemberment bounds invalid");
            foreach (int id in AllowedWeaponIds)
                if (id < 58 || id > 255) throw new InvalidDataException("combat effects requires registered test weapon IDs");
        }
    }
}
