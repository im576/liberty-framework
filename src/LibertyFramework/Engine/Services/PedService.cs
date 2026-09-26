using System;
using System.Collections;
using GTA;
using GTA.Native;
using Liberty.Sdk;
using Bone = Liberty.Sdk.Bone;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IPeds. Creation goes through ScriptHookDotNet's World.CreatePed after the model is streamed (non-blocking);
    // everything else is a plain native on the handle (NATIVES.md, SDK services). Created peds are owned: deleted when
    // the owner stops unless Release'd.
    public sealed class PedService : IPeds
    {
        private const int SpawnTimeoutMs = 5000;
        private readonly LibertyEngine engine;

        internal PedService(LibertyEngine engine) { this.engine = engine; }

        public void Spawn(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<PedRef> onReady)
        {
            engine.Scheduler.Start(owner, "spawn-ped", SpawnRoutine(owner, model, position, heading, onReady));
        }

        private IEnumerator SpawnRoutine(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<PedRef> onReady)
        {
            int deadline = Environment.TickCount + SpawnTimeoutMs;
            PedRef ped = PedRef.None;
            while (ped.IsNone && unchecked(Environment.TickCount - deadline) < 0)
            {
                ped = TryCreate(owner, model, position, heading);
                if (ped.IsNone) { yield return Wait.Milliseconds(50); }
            }
            engine.Streaming.ReleaseModel(owner, model);
            if (ped.IsNone) { RuntimeLog.Error("[" + owner.Id + "] ped_spawn_failed model=" + model); }
            if (onReady != null) { onReady(ped); }
        }

        public PedRef TryCreate(LibertyModule owner, ModelRef model, Vec3 position, float heading)
        {
            if (!engine.Streaming.RequestModel(owner, model)) { return PedRef.None; }
            Ped ped = GTA.World.CreatePed(new Model(model.Hash), Handles.V(position));
            if (ped == null || !ped.Exists()) { return PedRef.None; }
            PedRef handle = Handles.Ref(ped);
            Function.Call("SET_CHAR_HEADING", handle.Handle, heading);
            Own(owner, handle, model.Hash);
            return handle;
        }

        public void SpawnRandom(LibertyModule owner, Vec3 position, float heading, Action<PedRef> onReady)
        {
            engine.Scheduler.Start(owner, "spawn-random-ped", RandomRoutine(owner, position, heading, onReady));
        }

        private IEnumerator RandomRoutine(LibertyModule owner, Vec3 position, float heading, Action<PedRef> onReady)
        {
            int deadline = Environment.TickCount + SpawnTimeoutMs;
            Ped ped = null;
            while (ped == null && unchecked(Environment.TickCount - deadline) < 0)
            {
                ped = GTA.World.CreatePed(Handles.V(position));
                if (ped == null) { yield return Wait.Milliseconds(100); }
            }
            PedRef handle = PedRef.None;
            if (ped != null && ped.Exists())
            {
                handle = Handles.Ref(ped);
                Function.Call("SET_CHAR_HEADING", handle.Handle, heading);
                Own(owner, handle, NativeCall.OutInt("GET_CHAR_MODEL", handle.Handle));
            }
            if (onReady != null) { onReady(handle); }
        }

        internal void Own(LibertyModule owner, PedRef ped, int model)
        {
            int handle = ped.Handle;
            engine.Entities.Track(owner, EntityService.Kind.Ped, handle, model);
            engine.Ledger.Add(owner, "ped", handle, () => DeleteNow(handle));
        }

        public void Delete(PedRef ped)
        {
            if (ped.IsNone || ped == engine.Player.Ped) { return; }
            if (engine.Ledger.Has("ped", ped.Handle)) { engine.Ledger.Release("ped", ped.Handle); }
            else { DeleteNow(ped.Handle); }
        }

        private void DeleteNow(int handle)
        {
            engine.Entities.Untrack(EntityService.Kind.Ped, handle);
            if (!Function.Call<bool>("DOES_CHAR_EXIST", handle)) { return; }
            Pointer pointer = handle;
            Function.Call("DELETE_CHAR", pointer);
        }

        public void Release(LibertyModule owner, PedRef ped)
        {
            if (!engine.Ledger.Has(owner, "ped", ped.Handle)) { return; }
            engine.Ledger.Forget(owner, "ped", ped.Handle);
            engine.Entities.Untrack(EntityService.Kind.Ped, ped.Handle);
            if (!Exists(ped)) { return; }
            Pointer pointer = ped.Handle;
            Function.Call("MARK_CHAR_AS_NO_LONGER_NEEDED", pointer);
        }

        public bool Exists(PedRef ped) { return !ped.IsNone && Function.Call<bool>("DOES_CHAR_EXIST", ped.Handle); }

        public Vec3 GetPosition(PedRef ped)
        {
            PedState state;
            if (engine.World.TryGetPed(ped, out state)) { return state.Position; }
            return NativeCall.OutVector("GET_CHAR_COORDINATES", ped.Handle);
        }

        public void SetPosition(PedRef ped, Vec3 position) { Function.Call("SET_CHAR_COORDINATES", ped.Handle, position.X, position.Y, position.Z); }

        public float GetHeading(PedRef ped) { return NativeCall.OutFloat("GET_CHAR_HEADING", ped.Handle); }

        public void SetHeading(PedRef ped, float heading) { Function.Call("SET_CHAR_HEADING", ped.Handle, heading); }

        // The natives use the raw scale (gameplay health + 100).
        public int GetHealth(PedRef ped) { return NativeCall.OutInt("GET_CHAR_HEALTH", ped.Handle) - 100; }

        public void SetHealth(PedRef ped, int health) { Function.Call("SET_CHAR_HEALTH", ped.Handle, Math.Max(0, health + 100)); }

        public int GetArmour(PedRef ped) { return NativeCall.OutInt("GET_CHAR_ARMOUR", ped.Handle); }

        // ADD_ARMOUR_TO_CHAR only adds; lowering goes through ScriptHookDotNet's Armor setter.
        public void SetArmour(PedRef ped, int armour)
        {
            int current = GetArmour(ped);
            if (armour > current) { Function.Call("ADD_ARMOUR_TO_CHAR", ped.Handle, armour - current); return; }
            if (armour == current) { return; }
            Ped wrapper = Handles.Ped(ped);
            if (wrapper != null) { wrapper.Armor = armour; }
        }

        public bool IsDead(PedRef ped) { return Function.Call<bool>("IS_CHAR_DEAD", ped.Handle); }

        public void Kill(PedRef ped) { Function.Call("SET_CHAR_HEALTH", ped.Handle, 0); }

        public ModelRef GetModel(PedRef ped) { return ModelRef.FromHash(NativeCall.OutInt("GET_CHAR_MODEL", ped.Handle)); }

        public VehicleRef GetVehicle(PedRef ped)
        {
            if (!Function.Call<bool>("IS_CHAR_IN_ANY_CAR", ped.Handle)) { return VehicleRef.None; }
            return new VehicleRef(NativeCall.OutInt("GET_CAR_CHAR_IS_USING", ped.Handle));
        }

        // ScriptHookDotNet's GTA.Bone uses the same tags as Liberty.Sdk.Bone; its wrapper handles the Vector3 out-parameter.
        public Vec3 GetBonePosition(PedRef ped, Bone bone)
        {
            Ped wrapper = Handles.Ped(ped);
            if (wrapper != null) { return Handles.V(wrapper.GetBonePosition((GTA.Bone)(int)bone)); }
            Pointer position = typeof(Vector3);
            Function.Call("GET_PED_BONE_POSITION", ped.Handle, (int)bone, 0f, 0f, 0f, position);
            return Handles.V((Vector3)position);
        }

        public void SetInvincible(PedRef ped, bool invincible) { Function.Call("SET_CHAR_INVINCIBLE", ped.Handle, invincible); }

        public void SetFrozen(PedRef ped, bool frozen) { Function.Call("FREEZE_CHAR_POSITION", ped.Handle, frozen); }

        public void SetBlockEvents(PedRef ped, bool block) { Function.Call("SET_BLOCKING_OF_NON_TEMPORARY_EVENTS", ped.Handle, block); }

        public void SetAccuracy(PedRef ped, int accuracy) { Function.Call("SET_CHAR_ACCURACY", ped.Handle, Math.Max(0, Math.Min(100, accuracy))); }

        // ScriptHookDotNet's ForceRagdoll picks SWITCH_PED_TO_RAGDOLL's flag arguments (their meaning is not documented).
        public void Ragdoll(PedRef ped, int milliseconds)
        {
            Ped wrapper = Handles.Ped(ped);
            if (wrapper != null) { wrapper.ForceRagdoll(milliseconds, false); }
        }
    }
}
