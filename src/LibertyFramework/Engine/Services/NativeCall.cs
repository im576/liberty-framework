using GTA;
using GTA.Native;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // Small helpers over ScriptHookDotNet's Function.Call for the SDK services: handles are plain ints, out
    // parameters come back as values. Every native named here is listed in docs/game-api/NATIVES.md.
    internal static class NativeCall
    {
        internal static int OutInt(string name, int handle)
        {
            Pointer value = typeof(int);
            Function.Call(name, handle, value);
            return (int)value;
        }

        internal static float OutFloat(string name, int handle)
        {
            Pointer value = typeof(float);
            Function.Call(name, handle, value);
            return (float)value;
        }

        internal static Vec3 OutVector(string name, int handle)
        {
            Pointer x = typeof(float), y = typeof(float), z = typeof(float);
            Function.Call(name, handle, x, y, z);
            return new Vec3((float)x, (float)y, (float)z);
        }

        // GET_OFFSET_FROM_*_IN_WORLD_COORDS(entity, local x, y, z, float* x, y, z).
        internal static Vec3 Offset(string name, int handle, Vec3 local)
        {
            Pointer x = typeof(float), y = typeof(float), z = typeof(float);
            Function.Call(name, handle, local.X, local.Y, local.Z, x, y, z);
            return new Vec3((float)x, (float)y, (float)z);
        }
    }
}
