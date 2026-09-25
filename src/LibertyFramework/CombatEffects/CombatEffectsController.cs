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
                tracked.Clear();
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
                if (config == null || !config.Enabled) { tracked.Clear(); return; }
                long now = clock.ElapsedMilliseconds;
                if (now - lastSampleMilliseconds < config.SampleIntervalMilliseconds) return;
                lastSampleMilliseconds = now;
                Player player = Player;
                Ped shooter = player == null ? null : player.Character;
                if (shooter == null || !shooter.Exists() || shooter.isDead ||
                    !Natives.IsPlayerPlaying(player) || Natives.IsScreenFadedOut() ||
                    Function.Call<bool>("GET_MISSION_FLAG")) { tracked.Clear(); return; }
                int weaponId = (int)shooter.Weapons.CurrentType;
                bool eligibleWeapon = false;
                foreach (int id in config.AllowedWeaponIds) if (weaponId == id) { eligibleWeapon = true; break; }
                if (!eligibleWeapon) { tracked.Clear(); return; }

                HashSet<Ped> seen = new HashSet<Ped>();
                int count = 0;
                foreach (Ped target in World.GetPeds(shooter.Position, config.ScanRadiusMeters))
                {
                    if (count >= config.MaximumTrackedPeds) break;
                    if (target == null || target == shooter || !target.Exists() || target.isDead || target.isInVehicle()) continue;
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
                    if (config.WoundsEnabled)
                        state.Wounds.RemoveAll(wound => now - wound.CreatedMilliseconds > config.WoundLifetimeMilliseconds);
                }
                foreach (Ped ped in new List<Ped>(tracked.Keys)) if (!seen.Contains(ped)) tracked.Remove(ped);
            }
            catch (Exception error)
            {
                RuntimeLog.Error("feature_disabled combat_effects error=" + error);
                tracked.Clear();
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
                // The engine's normal impact response remains the physical reaction
                // until per-region animation/force APIs are validated in-game.
                state.LastReactionMilliseconds = now;
                RuntimeLog.Info("combat_reaction region=" + region + " bone=" + bone + " damage=" + damage);
            }
            if (config.WoundsEnabled)
            {
                if (state.Wounds.Count >= config.MaximumWoundsPerPed) state.Wounds.RemoveAt(0);
                state.Wounds.Add(new WoundRecord { Region = region, ApproximatePosition = target.Position,
                    CreatedMilliseconds = now, Damage = damage });
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

        private void OnDomainUnload(object sender, EventArgs args) { tracked.Clear(); }
    }
}
