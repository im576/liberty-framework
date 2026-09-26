using System;
using System.Collections;
using GTA;
using GTA.Native;
using Liberty.Sdk;
using VehicleDoor = Liberty.Sdk.VehicleDoor;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IVehicles. Creation through ScriptHookDotNet's World.CreateVehicle after streaming; the rest are natives on
    // the handle. Door indices are the game's (0 front left .. 3 rear right, 4 hood, 5 trunk), the order SHDN uses.
    public sealed class VehicleService : IVehicles
    {
        private const int SpawnTimeoutMs = 5000;
        private readonly LibertyEngine engine;

        internal VehicleService(LibertyEngine engine) { this.engine = engine; }

        public void Spawn(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<VehicleRef> onReady)
        {
            engine.RequireOwner(owner);
            engine.Scheduler.Start(owner, "spawn-vehicle", SpawnRoutine(owner, model, position, heading, onReady));
        }

        private IEnumerator SpawnRoutine(LibertyModule owner, ModelRef model, Vec3 position, float heading, Action<VehicleRef> onReady)
        {
            int deadline = Environment.TickCount + SpawnTimeoutMs;
            VehicleRef vehicle = VehicleRef.None;
            while (vehicle.IsNone && unchecked(Environment.TickCount - deadline) < 0)
            {
                if (engine.Streaming.RequestModel(owner, model))
                {
                    Vehicle created = GTA.World.CreateVehicle(new Model(model.Hash), Handles.V(position));
                    if (created != null && created.Exists())
                    {
                        vehicle = Handles.Ref(created);
                        Function.Call("SET_CAR_HEADING", vehicle.Handle, heading);
                        Own(owner, vehicle, model.Hash);
                        break;
                    }
                }
                yield return Wait.Milliseconds(50);
            }
            engine.Streaming.ReleaseModel(owner, model);
            if (vehicle.IsNone) { RuntimeLog.Error("[" + owner.Id + "] vehicle_spawn_failed model=" + model); }
            if (onReady != null) { onReady(vehicle); }
        }

        internal void Own(LibertyModule owner, VehicleRef vehicle, int model)
        {
            int handle = vehicle.Handle;
            engine.Entities.Track(owner, EntityService.Kind.Vehicle, handle, model);
            engine.Ledger.Add(owner, "vehicle", handle, () => DeleteNow(handle));
        }

        public void Delete(VehicleRef vehicle)
        {
            if (vehicle.IsNone) { return; }
            if (engine.Ledger.Has("vehicle", vehicle.Handle)) { engine.Ledger.Release("vehicle", vehicle.Handle); }
            else { DeleteNow(vehicle.Handle); }
        }

        private void DeleteNow(int handle)
        {
            engine.Entities.Untrack(EntityService.Kind.Vehicle, handle);
            if (!Function.Call<bool>("DOES_VEHICLE_EXIST", handle)) { return; }
            Pointer pointer = handle;
            Function.Call("DELETE_CAR", pointer);
        }

        public void Release(LibertyModule owner, VehicleRef vehicle)
        {
            if (!engine.Ledger.Has(owner, "vehicle", vehicle.Handle)) { return; }
            engine.Ledger.Forget(owner, "vehicle", vehicle.Handle);
            engine.Entities.Untrack(EntityService.Kind.Vehicle, vehicle.Handle);
            if (!Exists(vehicle)) { return; }
            Pointer pointer = vehicle.Handle;
            Function.Call("MARK_CAR_AS_NO_LONGER_NEEDED", pointer);
        }

        public bool Exists(VehicleRef vehicle) { return !vehicle.IsNone && Function.Call<bool>("DOES_VEHICLE_EXIST", vehicle.Handle); }

        public Vec3 GetPosition(VehicleRef vehicle)
        {
            VehicleState state;
            if (engine.World.TryGetVehicle(vehicle, out state)) { return state.Position; }
            return NativeCall.OutVector("GET_CAR_COORDINATES", vehicle.Handle);
        }

        public void SetPosition(VehicleRef vehicle, Vec3 position) { Function.Call("SET_CAR_COORDINATES", vehicle.Handle, position.X, position.Y, position.Z); }

        public float GetHeading(VehicleRef vehicle) { return NativeCall.OutFloat("GET_CAR_HEADING", vehicle.Handle); }

        public void SetHeading(VehicleRef vehicle, float heading) { Function.Call("SET_CAR_HEADING", vehicle.Handle, heading); }

        public float GetSpeed(VehicleRef vehicle) { return NativeCall.OutFloat("GET_CAR_SPEED", vehicle.Handle); }

        public int GetHealth(VehicleRef vehicle) { return NativeCall.OutInt("GET_CAR_HEALTH", vehicle.Handle); }

        public void SetHealth(VehicleRef vehicle, int health) { Function.Call("SET_CAR_HEALTH", vehicle.Handle, Math.Max(0, health)); }

        public float GetEngineHealth(VehicleRef vehicle) { return Function.Call<float>("GET_ENGINE_HEALTH", vehicle.Handle); }

        public void SetEngineHealth(VehicleRef vehicle, float health) { Function.Call("SET_ENGINE_HEALTH", vehicle.Handle, health); }

        public ModelRef GetModel(VehicleRef vehicle) { return ModelRef.FromHash(NativeCall.OutInt("GET_CAR_MODEL", vehicle.Handle)); }

        // GET_DRIVER_OF_CAR faulted on some pooled vehicles and a contained fault still corrupted game state (NATIVES.md), so
        // the snapshot's driver (read by the core from the vehicle, no native) is used whenever the vehicle is in it. The
        // native remains only for vehicles outside the snapshot radius, or a session without the core.
        public PedRef GetDriver(VehicleRef vehicle)
        {
            VehicleState state;
            if (engine.World.HasVehicles && engine.World.TryGetVehicle(vehicle, out state)) { return state.Driver; }
            return new PedRef(NativeCall.OutInt("GET_DRIVER_OF_CAR", vehicle.Handle));
        }

        public Vec3 GetOffsetPosition(VehicleRef vehicle, Vec3 local) { return NativeCall.Offset("GET_OFFSET_FROM_CAR_IN_WORLD_COORDS", vehicle.Handle, local); }

        public void OpenDoor(VehicleRef vehicle, VehicleDoor door) { Function.Call("OPEN_CAR_DOOR", vehicle.Handle, (int)door); }

        public void CloseDoor(VehicleRef vehicle, VehicleDoor door) { Function.Call("SHUT_CAR_DOOR", vehicle.Handle, (int)door); }

        public bool IsExtraOn(VehicleRef vehicle, int extra) { return Function.Call<bool>("IS_VEHICLE_EXTRA_TURNED_ON", vehicle.Handle, extra); }

        // TURN_OFF_VEHICLE_EXTRA(vehicle, extra, turnOff).
        public void SetExtra(VehicleRef vehicle, int extra, bool on) { Function.Call("TURN_OFF_VEHICLE_EXTRA", vehicle.Handle, extra, !on); }

        public void SetColours(VehicleRef vehicle, int primary, int secondary) { Function.Call("CHANGE_CAR_COLOUR", vehicle.Handle, primary, secondary); }

        public void Repair(VehicleRef vehicle) { Function.Call("FIX_CAR", vehicle.Handle); }

        public VehicleRef GetClosest(Vec3 position, float radius)
        {
            VehicleState state;
            if (engine.World.HasVehicles) { return engine.World.TryGetNearestVehicle(position, radius, out state) ? state.Vehicle : VehicleRef.None; }
            return Handles.Ref(GTA.World.GetClosestVehicle(Handles.V(position), radius));
        }
    }
}
