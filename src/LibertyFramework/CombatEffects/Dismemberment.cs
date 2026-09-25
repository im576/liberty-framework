using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using LibertyFramework.CombatEffects.Logic;
using LibertyFramework.Core.Logging;
using LibertyFramework.GameApi;

namespace LibertyFramework.CombatEffects
{
    // T-022: a lethal limb hit severs the limb. The corpse's limb bones are collapsed into the cut joint
    // every tick (PedSkeleton), a blood effect runs on the stump, and a severed limb is thrown: a clone
    // of the same ped whose every other bone is collapsed into the limb, ragdolled with an outward force.
    // Self-checking: the log records whether the engine keeps the collapsed matrices between ticks
    // (dismember_evidence); the thrown limb is only spawned once that evidence exists for the corpse.
    internal sealed class Dismemberment
    {
        private sealed class Severed
        {
            internal Ped Ped;
            internal int ModelHash;
            internal LimbCutPlan Plan;
            internal List<int> Indices = new List<int>();
            internal int CutIndex;
            internal float[] LastCut;
            internal int Ticks, PersistedTicks, OverwrittenTicks;
            internal bool EvidenceLogged;
            internal long CreatedMilliseconds;
            internal int StumpEffect;
            internal bool LimbThrown;
            internal Vector3 Push;
        }

        private sealed class ThrownLimb
        {
            internal Ped Ped;
            internal List<int> Collapse = new List<int>();
            internal int RootIndex;
            internal float[] LastCut;
            internal int Ticks;
            internal bool Shown;
            internal long CreatedMilliseconds;
            internal Vector3 Push;
        }

        private readonly PedSkeleton skeleton;
        private readonly List<Severed> severed = new List<Severed>();
        private readonly List<ThrownLimb> limbs = new List<ThrownLimb>();

        internal Dismemberment(PedSkeleton skeleton)
        {
            this.skeleton = skeleton;
        }

        internal int Count { get { return severed.Count + limbs.Count; } }

        internal bool IsSevered(Ped ped)
        {
            foreach (Severed record in severed) { if (record.Ped == ped) { return true; } }
            foreach (ThrownLimb limb in limbs) { if (limb.Ped == ped) { return true; } }
            return false;
        }

        internal void Sever(CombatEffectsConfig config, Ped target, int hitBone, Vector3 push, long now)
        {
            LimbCutPlan plan = LimbCutPlan.ForHitBone(hitBone);
            if (plan == null || severed.Count >= config.MaximumSeveredPeds || IsSevered(target)) { return; }
            uint ped = skeleton.PedFromHandle(target.GetHashCode());
            if (ped == 0) { RuntimeLog.Error("dismember_skip no_ped_pointer"); return; }
            int model = target.Model.Hash;
            Severed record = new Severed();
            record.Ped = target; record.ModelHash = model; record.Plan = plan; record.CreatedMilliseconds = now; record.Push = push;
            record.CutIndex = skeleton.IndexOf(ped, model, plan.CutTag);
            if (record.CutIndex <= 0) { RuntimeLog.Error("dismember_skip cut_bone_unresolved limb=" + plan.Name + " tag=0x" + plan.CutTag.ToString("X")); return; }
            foreach (int tag in plan.RemovedTags)
            {
                int index = skeleton.IndexOf(ped, model, tag);
                if (index > 0 && !record.Indices.Contains(index)) { record.Indices.Add(index); }
            }
            record.StumpEffect = Function.Call<int>("START_PTFX_ON_PED_BONE", config.StumpEffectName, target,
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, plan.StumpTag, 0);
            severed.Add(record);
            RuntimeLog.Info("dismember limb=" + plan.Name + " bones=" + record.Indices.Count + " cut_index=" + record.CutIndex + " model=" + model);
        }

        internal void Update(CombatEffectsConfig config, long now)
        {
            for (int i = severed.Count - 1; i >= 0; i--)
            {
                Severed record = severed[i];
                if (record.Ped == null || !record.Ped.Exists() || now - record.CreatedMilliseconds > config.SeveredCorpseLifetimeMilliseconds)
                {
                    Release(record);
                    severed.RemoveAt(i);
                    continue;
                }
                uint ped = skeleton.PedFromHandle(record.Ped.GetHashCode());
                float[] cut = ped == 0 ? null : skeleton.Origin(ped, record.CutIndex);
                if (cut == null) { continue; }
                int persisted = skeleton.Collapse(ped, record.Indices, cut, record.LastCut);
                if (record.LastCut != null)
                {
                    if (persisted == record.Indices.Count) { record.PersistedTicks++; } else { record.OverwrittenTicks++; }
                }
                record.LastCut = cut;
                record.Ticks++;
                if (!record.EvidenceLogged && record.Ticks >= 40)
                {
                    record.EvidenceLogged = true;
                    RuntimeLog.Info("dismember_evidence limb=" + record.Plan.Name + " persisted_ticks=" + record.PersistedTicks + " overwritten_ticks=" + record.OverwrittenTicks +
                        " (persisted = the engine kept the collapsed bones between ticks)");
                }
                if (config.SeveredLimbEnabled && !record.LimbThrown && record.PersistedTicks >= 2 && limbs.Count < config.MaximumSeveredPeds)
                {
                    record.LimbThrown = true;
                    ThrowLimb(record, now);
                }
            }
            for (int i = limbs.Count - 1; i >= 0; i--)
            {
                ThrownLimb limb = limbs[i];
                if (limb.Ped == null || !limb.Ped.Exists() || now - limb.CreatedMilliseconds > config.SeveredLimbLifetimeMilliseconds)
                {
                    ReleaseLimb(limb);
                    limbs.RemoveAt(i);
                    continue;
                }
                UpdateLimb(config, limb);
            }
        }

        private void ThrowLimb(Severed source, long now)
        {
            Vector3 position = source.Ped.Position + new Vector3(0, 0, 0.3f);
            Ped clone = World.CreatePed(source.Ped.Model, position);
            if (clone == null || !clone.Exists()) { RuntimeLog.Error("dismember_limb_spawn_failed"); return; }
            clone.Visible = false;
            for (int component = 0; component < 11; component++)
            {
                int drawable = Function.Call<int>("GET_CHAR_DRAWABLE_VARIATION", source.Ped, component);
                int texture = Function.Call<int>("GET_CHAR_TEXTURE_VARIATION", source.Ped, component);
                Function.Call("SET_CHAR_COMPONENT_VARIATION", clone, component, drawable, texture);
            }
            clone.Heading = source.Ped.Heading;
            clone.Die();
            // Ambient ownership: if the script reloads before cleanup, the game despawns it like any corpse.
            clone.NoLongerNeeded();
            ThrownLimb limb = new ThrownLimb();
            limb.Ped = clone; limb.CreatedMilliseconds = now; limb.Push = source.Push;
            limb.RootIndex = source.CutIndex;
            uint pointer = skeleton.PedFromHandle(clone.GetHashCode());
            int bones = pointer == 0 ? 0 : skeleton.BoneCount(pointer);
            if (bones == 0) { RuntimeLog.Error("dismember_limb_no_skeleton"); clone.Delete(); return; }
            for (int index = 1; index < bones; index++) { if (!source.Indices.Contains(index)) { limb.Collapse.Add(index); } }
            limbs.Add(limb);
            RuntimeLog.Info("dismember_limb_thrown limb=" + source.Plan.Name + " collapsed=" + limb.Collapse.Count);
        }

        private void UpdateLimb(CombatEffectsConfig config, ThrownLimb limb)
        {
            uint pointer = skeleton.PedFromHandle(limb.Ped.GetHashCode());
            float[] cut = pointer == 0 ? null : skeleton.Origin(pointer, limb.RootIndex);
            if (cut == null) { return; }
            skeleton.Collapse(pointer, limb.Collapse, cut, limb.LastCut);
            limb.LastCut = cut;
            limb.Ticks++;
            // Shown only after two collapsed ticks, so the full-body clone is never seen.
            if (!limb.Shown && limb.Ticks >= 2)
            {
                limb.Shown = true;
                limb.Ped.Visible = true;
                Function.Call("APPLY_FORCE_TO_PED", limb.Ped, 3, limb.Push.X * config.SeveredLimbForce, limb.Push.Y * config.SeveredLimbForce,
                    config.SeveredLimbForce * 0.5f, 0.0f, 0.0f, 0.0f, 0, 1, 1, 1);
            }
        }

        private static void Release(Severed record)
        {
            if (record.StumpEffect > 0) { Function.Call("STOP_PTFX", record.StumpEffect); }
        }

        private static void ReleaseLimb(ThrownLimb limb)
        {
            if (limb.Ped != null && limb.Ped.Exists()) { limb.Ped.Delete(); }
        }

        internal void Clear()
        {
            foreach (Severed record in severed) { Release(record); }
            foreach (ThrownLimb limb in limbs) { ReleaseLimb(limb); }
            severed.Clear();
            limbs.Clear();
        }
    }
}
