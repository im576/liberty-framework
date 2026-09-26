using System;
using System.Collections;
using GTA;
using GTA.Native;
using Liberty.Sdk;
using Bone = Liberty.Sdk.Bone;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IProps. Creation through ScriptHookDotNet's World.CreateObject after streaming (the holster path proven in
    // game); positioning, attachment and deletion are natives on the handle. ATTACH_OBJECT_TO_* take their rotation in
    // radians: the SDK self-test attached a prop with 90 and measured a 116.6 degree heading change (90 rad mod 360),
    // 2026-09-25. The SDK speaks degrees, so rotations are converted here.
    public sealed class PropService : IProps
    {
        private const int SpawnTimeoutMs = 5000;
        private const float DegreesToRadians = (float)(Math.PI / 180.0);
        private readonly LibertyEngine engine;

        internal PropService(LibertyEngine engine) { this.engine = engine; }

        public void Spawn(LibertyModule owner, ModelRef model, Vec3 position, Action<PropRef> onReady)
        {
            engine.RequireOwner(owner);
            engine.Scheduler.Start(owner, "spawn-prop", SpawnRoutine(owner, model, position, onReady));
        }

        private IEnumerator SpawnRoutine(LibertyModule owner, ModelRef model, Vec3 position, Action<PropRef> onReady)
        {
            int deadline = Environment.TickCount + SpawnTimeoutMs;
            PropRef prop = PropRef.None;
            while (prop.IsNone && unchecked(Environment.TickCount - deadline) < 0)
            {
                prop = TryCreate(owner, model, position);
                if (prop.IsNone) { yield return Wait.Milliseconds(50); }
            }
            engine.Streaming.ReleaseModel(owner, model);
            if (prop.IsNone) { RuntimeLog.Error("[" + owner.Id + "] prop_spawn_failed model=" + model); }
            if (onReady != null) { onReady(prop); }
        }

        public PropRef TryCreate(LibertyModule owner, ModelRef model, Vec3 position)
        {
            engine.RequireOwner(owner);
            if (!engine.Streaming.RequestModel(owner, model)) { return PropRef.None; }
            GTA.Object created = GTA.World.CreateObject(new Model(model.Hash), Handles.V(position));
            if (created == null || !created.Exists()) { return PropRef.None; }
            PropRef prop = Handles.Ref(created);
            int handle = prop.Handle;
            engine.Entities.Track(owner, EntityService.Kind.Prop, handle, model.Hash);
            engine.Ledger.Add(owner, "prop", handle, () => DeleteNow(handle));
            return prop;
        }

        public void Delete(PropRef prop)
        {
            if (prop.IsNone) { return; }
            if (engine.Ledger.Has("prop", prop.Handle)) { engine.Ledger.Release("prop", prop.Handle); }
            else { DeleteNow(prop.Handle); }
        }

        private void DeleteNow(int handle)
        {
            engine.Entities.Untrack(EntityService.Kind.Prop, handle);
            if (!Function.Call<bool>("DOES_OBJECT_EXIST", handle)) { return; }
            Pointer pointer = handle;
            Function.Call("DELETE_OBJECT", pointer);
        }

        public bool Exists(PropRef prop) { return !prop.IsNone && Function.Call<bool>("DOES_OBJECT_EXIST", prop.Handle); }

        public Vec3 GetPosition(PropRef prop) { return NativeCall.OutVector("GET_OBJECT_COORDINATES", prop.Handle); }

        public void SetPosition(PropRef prop, Vec3 position) { Function.Call("SET_OBJECT_COORDINATES", prop.Handle, position.X, position.Y, position.Z); }

        public float GetHeading(PropRef prop) { return NativeCall.OutFloat("GET_OBJECT_HEADING", prop.Handle); }

        public void SetRotation(PropRef prop, Vec3 degrees) { Function.Call("SET_OBJECT_ROTATION", prop.Handle, degrees.X, degrees.Y, degrees.Z); }

        public void SetCollision(PropRef prop, bool on) { Function.Call("SET_OBJECT_COLLISION", prop.Handle, on); }

        public void SetFrozen(PropRef prop, bool frozen) { Function.Call("FREEZE_OBJECT_POSITION", prop.Handle, frozen); }

        // ATTACH_OBJECT_TO_PED(object, ped, bone, offset xyz, rotation xyz, 0).
        public void AttachToPed(PropRef prop, PedRef ped, Bone bone, Vec3 offset, Vec3 rotation)
        {
            Vec3 r = rotation * DegreesToRadians;
            Function.Call("ATTACH_OBJECT_TO_PED", prop.Handle, ped.Handle, (int)bone, offset.X, offset.Y, offset.Z, r.X, r.Y, r.Z, 0);
        }

        // ATTACH_OBJECT_TO_CAR(object, vehicle, 0, offset xyz, rotation xyz).
        public void AttachToVehicle(PropRef prop, VehicleRef vehicle, Vec3 offset, Vec3 rotation)
        {
            Vec3 r = rotation * DegreesToRadians;
            Function.Call("ATTACH_OBJECT_TO_CAR", prop.Handle, vehicle.Handle, 0, offset.X, offset.Y, offset.Z, r.X, r.Y, r.Z);
        }

        public void Detach(PropRef prop) { Function.Call("DETACH_OBJECT", prop.Handle, true); }

        public Vec3 GetOffsetPosition(PropRef prop, Vec3 local) { return NativeCall.Offset("GET_OFFSET_FROM_OBJECT_IN_WORLD_COORDS", prop.Handle, local); }
    }
}
