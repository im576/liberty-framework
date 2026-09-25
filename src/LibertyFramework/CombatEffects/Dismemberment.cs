using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GTA;
using GTA.Native;
using LibertyFramework.CombatEffects.Logic;
using LibertyFramework.Core.Logging;
using LibertyFramework.GameApi;

namespace LibertyFramework.CombatEffects
{
    // T-022 dismemberment. A severed part = a bone and all its descendants collapsed into the cut joint (zero axes,
    // origin at the joint), so the mesh vanishes there and the surrounding skin closes over the stump.
    // The collapse is applied (1) right after the engine rebuilds the ped's skeleton each frame, through the
    // ADR-0005 skeleton hooks, and (2) on every script tick as a fallback. A thrown limb is a clone of the ped
    // with everything except that limb collapsed, ragdolled away from the shooter.
    internal sealed class Dismemberment
    {
        private sealed class Collapse
        {
            internal Ped Ped;
            internal bool Clone;
            internal string Name;
            internal int CutIndex;
            internal int[] Indices;
            internal uint FragInst, Skeleton, Matrices;
            internal long CreatedMilliseconds, LifetimeMilliseconds;
            internal int Ticks, HookHits;
            internal bool EvidenceLogged, Shown, LimbThrown;
            internal Vector3 Push;
            internal int StumpTag;
            internal int CutTag;
        }

        private readonly PedSkeleton skeleton;
        private readonly List<Collapse> records = new List<Collapse>();
        private volatile Collapse[] active = new Collapse[0];
        private readonly byte[] zeroAxes = new byte[48];

        internal Dismemberment(PedSkeleton skeleton)
        {
            this.skeleton = skeleton;
        }

        internal bool HooksActive { get; set; }

        internal bool IsTracked(Ped ped)
        {
            foreach (Collapse record in records) { if (record.Ped == ped) { return true; } }
            return false;
        }

        internal bool IsTracked(Ped ped, string name)
        {
            foreach (Collapse record in records) { if (record.Ped == ped && record.Name == name && !record.Clone) { return true; } }
            return false;
        }

        // Called on the engine thread right after a fragInst rebuilt its skeleton. Memory only, never throws.
        internal void OnSkeletonRebuilt(IntPtr fragInst)
        {
            try
            {
                Collapse[] snapshot = active;
                if (snapshot.Length == 0) { return; }
                uint frag = (uint)fragInst.ToInt32();
                foreach (Collapse record in snapshot)
                {
                    if (record.FragInst != frag || record.Matrices == 0) { continue; }
                    uint matrices = (uint)Marshal.ReadInt32(new IntPtr((int)(record.Skeleton + 0x14)));
                    if (matrices != record.Matrices) { continue; }
                    WriteCollapse(record);
                    record.HookHits++;
                }
            }
            catch
            {
                // Nothing may escape into the engine; the tick path logs and repairs state.
            }
        }

        private void WriteCollapse(Collapse record)
        {
            IntPtr cut = new IntPtr((int)(record.Matrices + (uint)(record.CutIndex * 64) + 48));
            byte[] origin = new byte[12];
            Marshal.Copy(cut, origin, 0, 12);
            byte[] rows = new byte[60];
            Buffer.BlockCopy(zeroAxes, 0, rows, 0, 48);
            Buffer.BlockCopy(origin, 0, rows, 48, 12);
            foreach (int index in record.Indices)
            {
                IntPtr matrix = new IntPtr((int)(record.Matrices + (uint)(index * 64)));
                Marshal.Copy(rows, 0, matrix, 12);                  // row 0 xyz
                Marshal.Copy(rows, 0, IntPtr.Add(matrix, 16), 12);  // row 1 xyz
                Marshal.Copy(rows, 0, IntPtr.Add(matrix, 32), 12);  // row 2 xyz
                Marshal.Copy(origin, 0, IntPtr.Add(matrix, 48), 12);
            }
        }

        // False when the skeleton cannot be read safely (logged).
        internal bool Sever(Ped target, LimbCutPlan plan, Vector3 push, long now, long lifetime)
        {
            if (IsTracked(target, plan.Name)) { return false; }
            Collapse record = Build(target, plan.CutTag, plan.Name, false, null);
            if (record == null) { return false; }
            record.Push = push;
            record.StumpTag = plan.StumpTag;
            record.CutTag = plan.CutTag;
            record.CreatedMilliseconds = now;
            record.LifetimeMilliseconds = lifetime;
            records.Add(record);
            Publish();
            RuntimeLog.Info("dismember part=" + plan.Name + " bones=" + record.Indices.Length + " hooked=" + (HooksActive && record.FragInst != 0));
            return true;
        }

        private Collapse Build(Ped ped, int cutTag, string name, bool clone, int[] keep)
        {
            uint pointer = skeleton.PedFromHandle(ped.GetHashCode());
            if (pointer == 0) { RuntimeLog.Error("dismember_skip no_ped_pointer part=" + name); return null; }
            int cut = skeleton.IndexOf(pointer, ped.Model.Hash, cutTag);
            if (cut <= 0) { RuntimeLog.Error("dismember_skip cut_bone_unresolved part=" + name + " tag=0x" + cutTag.ToString("X")); return null; }
            uint frag = skeleton.FragInst(pointer);
            uint skel = frag != 0 ? skeleton.Skeleton(frag) : 0;
            uint matrices = skel != 0 ? skeleton.MatricesOf(skel) : 0;
            uint expected = skeleton.MatrixBase(pointer);
            if (matrices == 0 || matrices != expected) { frag = 0; skel = 0; matrices = expected; }
            if (matrices == 0) { RuntimeLog.Error("dismember_skip no_matrices part=" + name); return null; }
            int count = skeleton.BoneCount(pointer);
            List<int> indices = skel != 0 && count > 0 ? skeleton.Subtree(skel, cut, count) : new List<int>();
            if (indices.Count == 0) { indices.Add(cut); }
            if (keep != null)
            {
                // Clone: collapse everything except the kept limb.
                List<int> rest = new List<int>();
                for (int index = 1; index < count; index++) { if (Array.IndexOf(keep, index) < 0) { rest.Add(index); } }
                indices = rest;
            }
            Collapse record = new Collapse();
            record.Ped = ped; record.Clone = clone; record.Name = name; record.CutIndex = cut;
            record.Indices = indices.ToArray(); record.FragInst = frag; record.Skeleton = skel; record.Matrices = matrices;
            return record;
        }

        // Raised with true while any collapse needs the post-rebuild hook, false when none do.
        internal Action<bool> ActiveChanged;

        private void Publish()
        {
            active = records.ToArray();
            if (ActiveChanged != null) { ActiveChanged(records.Count > 0); }
        }

        // Per-tick: refresh engine pointers (ragdoll on/off changes the fragInst), apply the fallback collapse,
        // throw limbs, expire records, and log hook evidence.
        internal void Update(CombatEffectsConfig config, long now, Action<object> onThrowReady)
        {
            bool changed = false;
            for (int i = records.Count - 1; i >= 0; i--)
            {
                Collapse record = records[i];
                if (record.Ped == null || !record.Ped.Exists() || now - record.CreatedMilliseconds > record.LifetimeMilliseconds)
                {
                    if (record.Clone && record.Ped != null && record.Ped.Exists()) { record.Ped.Delete(); }
                    records.RemoveAt(i);
                    changed = true;
                    continue;
                }
                uint pointer = skeleton.PedFromHandle(record.Ped.GetHashCode());
                if (pointer == 0) { continue; }
                uint frag = skeleton.FragInst(pointer);
                uint skel = frag != 0 ? skeleton.Skeleton(frag) : 0;
                uint matrices = skel != 0 ? skeleton.MatricesOf(skel) : skeleton.MatrixBase(pointer);
                if (frag != record.FragInst || skel != record.Skeleton || matrices != record.Matrices)
                {
                    record.FragInst = skel != 0 ? frag : 0; record.Skeleton = skel; record.Matrices = matrices;
                    changed = true;
                }
                if (record.Matrices != 0) { WriteCollapse(record); }
                record.Ticks++;
                if (record.Clone && !record.Shown && record.Ticks >= 2)
                {
                    record.Shown = true;
                    record.Ped.Visible = true;
                    Function.Call("APPLY_FORCE_TO_PED", record.Ped, 3, record.Push.X * config.SeveredLimbForce, record.Push.Y * config.SeveredLimbForce,
                        config.SeveredLimbForce * 0.6f, 0.0f, 0.0f, 0.0f, 0, 1, 1, 1);
                }
                if (!record.Clone && !record.LimbThrown && record.Ticks >= 2 && record.Name != "head" && onThrowReady != null)
                {
                    record.LimbThrown = true;
                    onThrowReady(record);
                }
                if (!record.EvidenceLogged && record.Ticks >= 40)
                {
                    record.EvidenceLogged = true;
                    RuntimeLog.Info("dismember_evidence part=" + record.Name + " clone=" + record.Clone + " hook_calls=" + record.HookHits +
                        " ticks=" + record.Ticks + " ragdoll_frag=" + (record.FragInst != 0));
                }
            }
            if (changed) { Publish(); }
        }

        // Spawn the thrown limb for a severed corpse: same model and clothes, every bone but the limb collapsed.
        internal void ThrowLimb(CombatEffectsConfig config, object severedRecord, long now)
        {
            Collapse source = (Collapse)severedRecord;
            if (CountClones() >= config.MaximumSeveredPeds) { return; }
            Ped clone = World.CreatePed(source.Ped.Model, source.Ped.Position + new Vector3(0, 0, 0.35f));
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
            clone.NoLongerNeeded();
            Collapse record = Build(clone, source.CutTag, source.Name + "_limb", true, source.Indices);
            if (record == null) { clone.Delete(); return; }
            record.CutIndex = source.CutIndex;
            record.Push = source.Push;
            record.CreatedMilliseconds = now;
            record.LifetimeMilliseconds = config.SeveredLimbLifetimeMilliseconds;
            records.Add(record);
            Publish();
            RuntimeLog.Info("dismember_limb_thrown part=" + source.Name + " collapsed=" + record.Indices.Length);
        }

        private int CountClones()
        {
            int count = 0;
            foreach (Collapse record in records) { if (record.Clone) { count++; } }
            return count;
        }

        internal int StumpTagOf(object severedRecord) { return ((Collapse)severedRecord).StumpTag; }
        internal Ped PedOf(object severedRecord) { return ((Collapse)severedRecord).Ped; }

        internal int SeveredCount
        {
            get { int count = 0; foreach (Collapse record in records) { if (!record.Clone) { count++; } } return count; }
        }

        internal void Clear()
        {
            active = new Collapse[0];
            foreach (Collapse record in records)
            {
                try { if (record.Clone && record.Ped != null && record.Ped.Exists()) { record.Ped.Delete(); } }
                catch (Exception error) { RuntimeLog.Error("dismember_clear_failed error=" + error.Message); }
            }
            records.Clear();
        }

        // Unload/exit: memory only. Stops the hook callback from touching any record.
        internal void Detach() { active = new Collapse[0]; }
    }
}
