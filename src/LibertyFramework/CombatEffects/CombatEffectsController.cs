using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA;
using GTA.Native;
using LibertyFramework.CombatEffects.Logic;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;
using LibertyFramework.GameApi;

namespace LibertyFramework.CombatEffects
{
    // Gore, blood and dismemberment (T-022). Every shot the player lands on a nearby ped (any firearm when
    // allFirearms) produces a weapon-specific entry/exit spray, chunks on heavy hits, a bleeding wound that
    // drips for a while, and region reactions. A killing hit to a limb severs it at the joint (with an arterial
    // spurt and a thrown limb); a killing head hit decapitates. All effects are stock gta_core particles.
    public sealed class CombatEffectsController : Script
    {
        private sealed class PendingCut
        {
            internal Ped Ped;
            internal LimbCutPlan Plan;
            internal Vector3 Push;
            internal long Deadline;
            internal float Scale;
        }

        private readonly Dictionary<Ped, PedInjuryState> tracked = new Dictionary<Ped, PedInjuryState>();
        private readonly List<BloodEmitter> emitters = new List<BloodEmitter>();
        private readonly List<PendingCut> pending = new List<PendingCut>();
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private CombatEffectsConfig config;
        private DateTime lastConfigCheckUtc;
        private string configHash;
        private long lastSampleMilliseconds;
        private bool disabled;
        private Dismemberment dismember;
        private bool engineChecked;
        private SkeletonHook syncHook, poseHook;

        public CombatEffectsController()
        {
            Interval = 0;
            LoadConfig();
            Tick += OnTick;
            AppDomain.CurrentDomain.DomainUnload += OnUnload;
            AppDomain.CurrentDomain.ProcessExit += OnUnload;
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
                RuntimeLog.Info("combat_effects_config_loaded enabled=" + config.Enabled + " all_firearms=" + config.AllFirearms +
                    " dismemberment=" + config.DismembermentEnabled + " decapitation=" + config.DecapitationEnabled + " scale=" + config.EffectScale);
            }
            catch (Exception error) { RuntimeLog.Error("combat_effects_config_rejected error=" + error); }
        }

        private void OnTick(object sender, EventArgs args)
        {
            if (disabled) return;
            try
            {
                LoadConfig();
                if (config == null || !config.Enabled) { ClearAll(); return; }
                long now = clock.ElapsedMilliseconds;
                Player player = Player;
                Ped shooter = player == null ? null : player.Character;
                if (shooter == null || !shooter.Exists()) return;
                if (!engineChecked) { EnsureEngine(shooter); }
                if (dismember != null)
                {
                    try { dismember.Update(config, now, record => { if (config.SeveredLimbEnabled) dismember.ThrowLimb(config, record, now); }); }
                    catch (Exception error) { DisableDismemberment(error); }
                }
                ResolvePending(now);
                PulseEmitters(now);
                if (now - lastSampleMilliseconds < config.SampleIntervalMilliseconds) return;
                lastSampleMilliseconds = now;
                if (shooter.isDead || !Natives.IsPlayerPlaying(player) || Natives.IsScreenFadedOut()) { tracked.Clear(); return; }
                GTA.value.Weapon weapon = shooter.Weapons.Current;
                if (!EligibleWeapon(weapon)) return;
                SampleDamage(shooter, weapon, now);
            }
            catch (Exception error)
            {
                RuntimeLog.Error("feature_disabled combat_effects error=" + error);
                try { ClearAll(); } catch (Exception cleanupError) { RuntimeLog.Error("combat_effects_cleanup_failed error=" + cleanupError); }
                RemoveHooks();
                disabled = true;
            }
        }

        private bool EligibleWeapon(GTA.value.Weapon weapon)
        {
            if (weapon == null) return false;
            int id = (int)weapon.Type;
            if (config.AllFirearms)
            {
                switch (weapon.Slot)
                {
                    case WeaponSlot.Handgun: case WeaponSlot.Shotgun: case WeaponSlot.SMG:
                    case WeaponSlot.Rifle: case WeaponSlot.Sniper: case WeaponSlot.Heavy: return true;
                }
            }
            foreach (int allowed in config.AllowedWeaponIds) if (allowed == id) return true;
            return false;
        }

        private void SampleDamage(Ped shooter, GTA.value.Weapon weapon, long now)
        {
            HashSet<Ped> seen = new HashSet<Ped>();
            int count = 0;
            foreach (Ped target in World.GetPeds(shooter.Position, config.ScanRadiusMeters))
            {
                if (count >= config.MaximumTrackedPeds) break;
                if (target == null || target == shooter || !target.Exists() || target.isInVehicle()) continue;
                if (dismember != null && dismember.IsTracked(target) && !tracked.ContainsKey(target)) continue; // our own limb clones
                if (!config.IncludeMissionPeds && CombatEffectsNatives.IsMissionPed(target)) continue;
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
                if (damage > 0 && target.HasBeenDamagedBy(shooter)) OnDamage(shooter, weapon, target, state, damage, now);
            }
            foreach (Ped ped in new List<Ped>(tracked.Keys)) if (!seen.Contains(ped)) tracked.Remove(ped);
        }

        private void OnDamage(Ped shooter, GTA.value.Weapon weapon, Ped target, PedInjuryState state, int damage, long now)
        {
            int bone = DebugHitNatives.LastDamageBone(target);
            HitRegion region = HitClassifier.Classify(bone);
            if (region == HitRegion.Unknown) { bone = 0x36A0; region = HitRegion.Torso; } // unknown bone: bleed from the chest
            float scale = config.EffectScale * Clamp(damage / 40.0f, 0.8f, 2.2f);
            WeaponSlot slot = weapon.Slot;

            // Impact: weapon-specific entry spray, exit spray on strong hits, chunks on very strong hits.
            string entry = slot == WeaponSlot.Shotgun ? config.ShotgunEntryEffectName : slot == WeaponSlot.Sniper ? config.SniperEntryEffectName : config.ImpactEffectName;
            CombatEffectsNatives.Burst(entry, target, bone, scale);
            if (damage >= config.ExitDamage) CombatEffectsNatives.Burst(config.ExitEffectName, target, bone, scale);
            if (damage >= config.ChunkDamage || slot == WeaponSlot.Shotgun)
            {
                string chunks = slot == WeaponSlot.Shotgun ? config.ShotgunChunksEffectName : slot == WeaponSlot.Sniper ? config.SniperChunksEffectName : config.HeavyChunksEffectName;
                CombatEffectsNatives.Burst(chunks, target, bone, scale);
            }
            if (config.WoundsEnabled && state.Bleeds < config.MaximumWoundsPerPed)
            {
                state.Bleeds++;
                AddEmitter(target, bone, config.BleedEffectName, config.EffectScale, config.BleedIntervalMilliseconds, config.BleedDurationMilliseconds, now);
            }

            if (config.InjuriesEnabled && damage >= config.MinimumInjuryDamage)
            {
                int hits;
                state.RegionHits.TryGetValue(region, out hits);
                state.RegionHits[region] = hits + 1;
            }
            Vector3 away = target.Position - shooter.Position;
            float length = (float)Math.Sqrt(away.X * away.X + away.Y * away.Y);
            Vector3 push = length > 0.01f ? new Vector3(away.X / length, away.Y / length, 0) : new Vector3(0, 0, 0);
            if (config.ReactionsEnabled && !target.isDead && now - state.LastReactionMilliseconds >= config.ReactionCooldownMilliseconds)
            {
                state.LastReactionMilliseconds = now;
                float force = ReactionForce(region);
                if (force > 0) CombatEffectsNatives.React(target, push.X * force, push.Y * force,
                    region == HitRegion.LeftLeg || region == HitRegion.RightLeg ? 0.0f : force * config.ReactionVerticalFraction);
            }
            RuntimeLog.Info("combat_hit region=" + region + " bone=0x" + bone.ToString("X") + " damage=" + damage + " weapon=" + (int)weapon.Type + " dead=" + target.isDead);

            // Severing waits briefly for death: peds are flagged dead a few frames after the killing hit.
            if (LimbCutPlan.IsHeadBone(bone))
            {
                if (config.DecapitationEnabled && damage >= config.DecapitationMinimumDamage && !state.HeadRemoved)
                    Queue(target, LimbCutPlan.Head(), push, now, scale);
                else if (target.isDead) CombatEffectsNatives.Burst(config.MouthBloodEffectName, target, 0x4B5, config.EffectScale);
            }
            else if (config.DismembermentEnabled && damage >= config.MinimumLimbLossDamage)
            {
                LimbCutPlan plan = LimbCutPlan.ForHitBone(bone);
                if (plan != null) Queue(target, plan, push, now, scale);
            }
        }

        private void Queue(Ped target, LimbCutPlan plan, Vector3 push, long now, float scale)
        {
            foreach (PendingCut cut in pending) if (cut.Ped == target && cut.Plan.Name == plan.Name) return;
            PendingCut item = new PendingCut();
            item.Ped = target; item.Plan = plan; item.Push = push; item.Scale = scale;
            item.Deadline = now + config.PendingDeathWindowMilliseconds;
            pending.Add(item);
        }

        private void ResolvePending(long now)
        {
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                PendingCut cut = pending[i];
                if (cut.Ped == null || !cut.Ped.Exists() || now > cut.Deadline) { pending.RemoveAt(i); continue; }
                if (!cut.Ped.isDead && cut.Ped.Health > 0) continue;
                pending.RemoveAt(i);
                Sever(cut, now);
            }
        }

        private void Sever(PendingCut cut, long now)
        {
            bool head = cut.Plan.Name == "head";
            PedInjuryState state;
            tracked.TryGetValue(cut.Ped, out state);
            bool collapsed = dismember != null && dismember.SeveredCount < config.MaximumSeveredPeds &&
                dismember.Sever(cut.Ped, cut.Plan, cut.Push, now, config.SeveredCorpseLifetimeMilliseconds);
            if (head && !collapsed) { CombatEffectsNatives.RemoveHead(cut.Ped); } // stock fallback
            if (head && state != null) state.HeadRemoved = true;
            float burst = cut.Scale * 1.4f;
            CombatEffectsNatives.Burst(config.SeverBurstEffectName, cut.Ped, cut.Plan.StumpTag, burst);
            CombatEffectsNatives.Burst(config.SeverMistEffectName, cut.Ped, cut.Plan.StumpTag, burst);
            AddEmitter(cut.Ped, cut.Plan.StumpTag, config.ArterialEffectName, config.EffectScale * 1.2f, config.ArterialIntervalMilliseconds,
                config.ArterialDurationMilliseconds, now);
            AddEmitter(cut.Ped, cut.Plan.StumpTag, config.BleedEffectName, config.EffectScale * 1.3f, config.BleedIntervalMilliseconds,
                config.SeveredCorpseLifetimeMilliseconds, now);
            RuntimeLog.Info("combat_sever part=" + cut.Plan.Name + " collapsed=" + collapsed);
        }

        private void AddEmitter(Ped ped, int bone, string effect, float scale, int interval, int duration, long now)
        {
            if (string.IsNullOrEmpty(effect) || duration <= 0 || interval <= 0) return;
            if (emitters.Count >= config.MaximumEmitters) emitters.RemoveAt(0);
            BloodEmitter emitter = new BloodEmitter();
            emitter.Ped = ped; emitter.Bone = bone; emitter.Effect = effect; emitter.Scale = scale;
            emitter.IntervalMilliseconds = interval; emitter.NextMilliseconds = now; emitter.UntilMilliseconds = now + duration;
            emitters.Add(emitter);
        }

        private void PulseEmitters(long now)
        {
            for (int i = emitters.Count - 1; i >= 0; i--)
            {
                BloodEmitter emitter = emitters[i];
                if (emitter.Ped == null || !emitter.Ped.Exists() || now > emitter.UntilMilliseconds) { emitters.RemoveAt(i); continue; }
                if (now < emitter.NextMilliseconds) continue;
                emitter.NextMilliseconds = now + emitter.IntervalMilliseconds;
                CombatEffectsNatives.Burst(emitter.Effect, emitter.Ped, emitter.Bone, emitter.Scale);
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

        private static float Clamp(float value, float minimum, float maximum) { return value < minimum ? minimum : value > maximum ? maximum : value; }

        // Engine access (Gunplay's resolved addresses) and the ADR-0005 hooks; validated on the player's own ped first.
        private void EnsureEngine(Ped self)
        {
            LibertyFramework.Gunplay.GunplayController gunplay = LibertyFramework.Gunplay.GunplayController.Instance;
            LibertyFramework.Core.Memory.GameAddresses addresses = gunplay != null ? gunplay.Addresses : null;
            if (addresses == null) return; // Gunplay has not resolved yet; try next tick
            engineChecked = true;
            if (!config.DismembermentEnabled && !config.DecapitationEnabled) return;
            if (!addresses.PedSkeletonResolved) { RuntimeLog.Error("dismemberment_unavailable ped skeleton not resolved (see engine_resolve ped_skeleton)"); return; }
            PedSkeleton skeleton = new PedSkeleton(new LibertyFramework.Core.Memory.LiveMemory(), addresses);
            uint pointer = skeleton.PedFromHandle(self.GetHashCode());
            if (pointer == 0 || skeleton.MatrixBase(pointer) == 0 || skeleton.IndexOf(pointer, self.Model.Hash, 0x4B5) <= 0)
            {
                skeleton.Dispose();
                RuntimeLog.Error("dismemberment_validation_failed player_ped=0x" + pointer.ToString("X8"));
                return;
            }
            dismember = new Dismemberment(skeleton);
            if (addresses.SkeletonHooksResolved)
            {
                try
                {
                    SkeletonHook.AfterCallback callback = dismember.OnSkeletonRebuilt;
                    syncHook = new SkeletonHook("frag_skeleton_sync", addresses.FragSkeletonSyncFunction,
                        new byte?[] { 0x81, 0xEC, 0x64, 0x01, 0x00, 0x00, 0xA1, null, null, null, null }, callback);
                    poseHook = new SkeletonHook("frag_pose", addresses.FragPoseFunction,
                        new byte?[] { 0x55, 0x8B, 0xEC, 0x83, 0xE4, 0xF0, 0x81, 0xEC, 0x14, 0x01, 0x00, 0x00 }, callback);
                    syncHook.Install();
                    poseHook.Install();
                    dismember.HooksActive = true;
                    dismember.ActiveChanged = on => { syncHook.SetActive(on); poseHook.SetActive(on); };
                }
                catch (Exception error)
                {
                    RuntimeLog.Error("skeleton_hooks_failed tick fallback only error=" + error.Message);
                    RemoveHooks();
                }
            }
            RuntimeLog.Info("dismemberment_ready player_ped=0x" + pointer.ToString("X8") + " hooks=" + dismember.HooksActive);
        }

        private void DisableDismemberment(Exception error)
        {
            RuntimeLog.Error("feature_disabled dismemberment error=" + error);
            RemoveHooks();
            try { dismember.Clear(); } catch (Exception cleanup) { RuntimeLog.Error("dismemberment_cleanup_failed error=" + cleanup.Message); }
            dismember = null;
        }

        private void RemoveHooks()
        {
            if (dismember != null) dismember.Detach();
            try { if (syncHook != null) syncHook.Remove(); } catch (Exception error) { RuntimeLog.Error("skeleton_hook_remove_failed error=" + error.Message); }
            try { if (poseHook != null) poseHook.Remove(); } catch (Exception error) { RuntimeLog.Error("skeleton_hook_remove_failed error=" + error.Message); }
            if (dismember != null) dismember.HooksActive = false;
        }

        private void ClearAll()
        {
            tracked.Clear();
            emitters.Clear();
            pending.Clear();
            if (dismember != null) { try { dismember.Clear(); } catch (Exception error) { RuntimeLog.Error("dismemberment_cleanup_failed error=" + error.Message); } }
        }

        // Unload and process exit: memory only (no natives). Hooks come out before the domain goes away.
        private void OnUnload(object sender, EventArgs args)
        {
            RemoveHooks();
        }
    }
}
