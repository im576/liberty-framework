using System;
using System.Reflection;
using GTA;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;
using GtaObject = GTA.Object;

namespace LibertyFramework.Engine
{
    // Converts SDK handles and vectors to ScriptHookDotNet objects, for the few calls only its wrappers make safely
    // (animation tasks, door objects). Objects come from ScriptHookDotNet's own handle cache (internal
    // GTA.ContentCache.GetPed/GetVehicle/GetObject), so lookups do not allocate and wrappers compare equal to SHDN's.
    // If the cache methods are missing the lookups return null (logged once) and the calling feature degrades.
    internal static class Handles
    {
        private static Func<int, Ped> getPed;
        private static Func<int, Vehicle> getVehicle;
        private static Func<int, GtaObject> getObject;
        private static bool resolved;

        private static void Resolve()
        {
            if (resolved) { return; }
            resolved = true;
            try
            {
                Type cache = typeof(Ped).Assembly.GetType("GTA.ContentCache");
                getPed = Bind<Ped>(cache, "GetPed");
                getVehicle = Bind<Vehicle>(cache, "GetVehicle");
                getObject = Bind<GtaObject>(cache, "GetObject");
            }
            catch (Exception error) { RuntimeLog.Error("engine_handles_unavailable error=" + error.Message); }
        }

        private static Func<int, T> Bind<T>(Type cache, string method) where T : class
        {
            MethodInfo info = cache != null ? cache.GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int) }, null) : null;
            if (info == null) { throw new MissingMethodException("GTA.ContentCache." + method); }
            return (Func<int, T>)Delegate.CreateDelegate(typeof(Func<int, T>), info);
        }

        internal static Ped Ped(PedRef ped) { Resolve(); return ped.IsNone || getPed == null ? null : getPed(ped.Handle); }
        internal static Vehicle Vehicle(VehicleRef vehicle) { Resolve(); return vehicle.IsNone || getVehicle == null ? null : getVehicle(vehicle.Handle); }
        internal static GtaObject Object(PropRef prop) { Resolve(); return prop.IsNone || getObject == null ? null : getObject(prop.Handle); }

        internal static PedRef Ref(Ped ped) { return ped == null ? PedRef.None : new PedRef(ped.GetHashCode()); }
        internal static VehicleRef Ref(Vehicle vehicle) { return vehicle == null ? VehicleRef.None : new VehicleRef(vehicle.GetHashCode()); }
        internal static PropRef Ref(GtaObject prop) { return prop == null ? PropRef.None : new PropRef(prop.GetHashCode()); }

        internal static Vector3 V(Vec3 v) { return new Vector3(v.X, v.Y, v.Z); }
        internal static Vec3 V(Vector3 v) { return new Vec3(v.X, v.Y, v.Z); }
    }
}
