using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA;
using GTA.Native;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.GameApi;

namespace LibertyFramework.CombatEffects
{
    // Conservative damage-event collector. It owns no ped, engine memory, or spawned
    // resource, so a reload/despawn clears all transient injury and wound state.
    public sealed class CombatEffectsController : Script
    {
        private readonly Dictionary<Ped, PedInjuryState> tracked = new Dictionary<Ped, PedInjuryState>();
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private CombatEffectsConfig config;
        private DateTime lastConfigCheckUtc;
        private string configHash;
        private long lastSampleMilliseconds;
        private bool disabled;

        public CombatEffectsController()
        {
            Interval = 0;
            LoadConfig();
            Tick += OnTick;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
        }

        private void LoadConfig()
        {
            if ((DateTime.UtcNow - lastConfigCheckUtc).TotalMilliseconds < 1000) return;
            lastConfigCheckUtc = DateTime.UtcNow;
            try
            {
                string path = Path.Combine(LibertyPaths.ConfigDirectory, "combat_effects.json");
                byte[] bytes = JsonStore.ReadBytes(path);
                string hash = JsonStore.Hash(bytes);
                if (hash == configHash) return;
                CombatEffectsConfig candidate = JsonStore.Parse<CombatEffectsConfig>(bytes);
                candidate.Validate();
                config = candidate;
                configHash = hash;
                Clear();
                RuntimeLog.Info("combat_effects_config_loaded enabled=" + config.Enabled);
            }
            catch (Exception error) { RuntimeLog.Error("combat_effects_config_rejected error=" + error); }
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled) return;
            try
            {
                LoadConfig();
                if (config == null || !config.Enabled) { Clear(); return; }
                long now = clock.ElapsedMilliseconds;
                if (now - lastSampleMilliseconds < config.SampleIntervalMilliseconds) return;
                lastSampleMilliseconds = now;
                Player player = Player;
                Ped shooter = player == null ? null : player.Character;
                if (shooter == null || !shooter.Exists() || shooter.isDead ||
                    !Natives.IsPlayerPlaying(player) || Natives.IsScreenFadedOut() ||
                    Function.Call<bool>("GET_MISSION_FLAG")) { Clear(); return; }
                int weaponId = (int)shooter.Weapons.CurrentType;
                bool eligibleWeapon = false;
                foreach (int id in config.AllowedWeaponIds) if (weaponId == id) { eligibleWeapon = true; break; }
                if (!eligibleWeapon) { Clear(); return; }

                HashSet<Ped> seen = new HashSet<Ped>();
                int count = 0;
                foreach (Ped target in World.GetPeds(shooter.Position, config.ScanRadiusMeters))
                {
                    if (count >= config.MaximumTrackedPeds) break;
                    if (target == null || target == shooter || !target.Exists() || target.isInVehicle() ||
                        CombatEffectsNatives.IsMissionPed(target)) continue;
                    ++count;
                    seen.Add(target);
                    PedInjuryState state;
                    if (!tracked.TryGetValue(target, out state))
                    {
                        state = new PedInjuryState();
                        state.LastHealth = target.Health;
                        tracked.Add(target, state);
                        continue;
                    }
                    int damage = state.LastHealth - target.Health;
                    state.LastHealth = target.Health;
                    if (damage > 0 && target.HasBeenDamagedBy(shooter)) OnDamage(target, state, damage, now);
                    ExpireWounds(state, now);
                }
                foreach (Ped ped in new List<Ped>(tracked.Keys))
                    if (!seen.Contains(ped)) { StopWounds(tracked[ped]); tracked.Remove(ped); }
            }
            catch (Exception error)
            {
                RuntimeLog.Error("feature_disabled combat_effects error=" + error);
                try { Clear(); } catch (Exception cleanupError) { RuntimeLog.Error("combat_effects_cleanup_failed error=" + cleanupError); }
                disabled = true;
            }
        }

        private void OnDamage(Ped target, PedInjuryState state, int damage, long now)
        {
            int bone = DebugHitNatives.LastDamageBone(target);
            HitRegion region = HitClassifier.Classify(bone);
            if (config.InjuriesEnabled && damage >= config.MinimumInjuryDamage && region != HitRegion.Unknown)
            {
                int hits;
                state.RegionHits.TryGetValue(region, out hits);
                state.RegionHits[region] = hits + 1;
            }
            if (config.ReactionsEnabled && now - state.LastReactionMilliseconds >= config.ReactionCooldownMilliseconds)
            {
                state.LastReactionMilliseconds = now;
                float force = ReactionForce(region);
                if (!target.isDead && force > 0)
                {
                    Ped shooter = Player.Character;
                    Vector3 delta = target.Position - shooter.Position;
                    float length = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    if (length > 0.01f) CombatEffectsNatives.React(target,
                        delta.X / length * force, delta.Y / length * force,
                        region == HitRegion.LeftLeg || region == HitRegion.RightLeg ? 0.0f : force * config.ReactionVerticalFraction);
                }
                RuntimeLog.Info("combat_reaction region=" + region + " bone=" + bone + " damage=" + damage);
            }
            if (config.WoundsEnabled && region != HitRegion.Unknown)
            {
                CombatEffectsNatives.Impact(config.ImpactEffectName, target, bone);
                if (state.Wounds.Count >= config.MaximumWoundsPerPed)
                {
                    CombatEffectsNatives.StopWound(state.Wounds[0].EffectHandle);
                    state.Wounds.RemoveAt(0);
                }
                state.Wounds.Add(new WoundRecord { Region = region, ApproximatePosition = target.Position,
                    CreatedMilliseconds = now, Damage = damage,
                    EffectHandle = CombatEffectsNatives.StartWound(config.WoundEffectName, target, bone) });
            }
            if (config.HeadLossPrototypeEnabled && region == HitRegion.Head &&
                damage >= config.MinimumLimbLossDamage && !state.HeadRemoved &&
                target.isDead)
            {
                state.HeadRemoved = true;
                CombatEffectsNatives.RemoveHead(target);
                RuntimeLog.Info("combat_head_loss damage=" + damage);
            }
            if (config.LimbLossPrototypeEnabled && damage >= config.MinimumLimbLossDamage &&
                (region == HitRegion.LeftArm || region == HitRegion.RightArm ||
                 region == HitRegion.LeftLeg || region == HitRegion.RightLeg))
            {
                int hits;
                if (state.RegionHits.TryGetValue(region, out hits) && hits >= config.MinimumLimbLossHits &&
                    state.LostLimbs.Add(region))
                    RuntimeLog.Info("combat_limb_loss_candidate region=" + region + " damage=" + damage);
            }
        }

        private float ReactionForce(HitRegion region)
        {
            switch (region)
            {
                case HitRegion.Head: return config.ReactionForceHead;
                case HitRegion.Torso: return config.ReactionForceTorso;
                case HitRegion.LeftArm: case HitRegion.RightArm: return config.ReactionForceArm;
                case HitRegion.LeftLeg: case HitRegion.RightLeg: return config.ReactionForceLeg;
                default: return 0;
            }
        }

        private void ExpireWounds(PedInjuryState state, long now)
        {
            for (int index = state.Wounds.Count - 1; index >= 0; --index)
                if (!config.WoundsEnabled || now - state.Wounds[index].CreatedMilliseconds > config.WoundLifetimeMilliseconds)
                {
                    CombatEffectsNatives.StopWound(state.Wounds[index].EffectHandle);
                    state.Wounds.RemoveAt(index);
                }
        }

        private static void StopWounds(PedInjuryState state)
        {
            foreach (WoundRecord wound in state.Wounds) CombatEffectsNatives.StopWound(wound.EffectHandle);
            state.Wounds.Clear();
        }

        private void Clear()
        {
            foreach (PedInjuryState state in tracked.Values) StopWounds(state);
            tracked.Clear();
        }

        private void OnDomainUnload(object sender, EventArgs args)
        {
            try { Clear(); } catch (Exception error) { RuntimeLog.Error("combat_effects_unload_cleanup_failed error=" + error); }
        }
    }
}
