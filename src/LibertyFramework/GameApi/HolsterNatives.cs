using GTA;
using GTA.Native;

namespace LibertyFramework.GameApi
{
    // Called only on Script.Tick. Stream first, then let World.CreateObject use the loaded model.
    internal static class HolsterNatives
    {
        internal static void RequestModel(Model model)
        {
            Function.Call("REQUEST_MODEL", model.Hash);
        }

        internal static bool HasModelLoaded(Model model)
        {
            return Function.Call<bool>("HAS_MODEL_LOADED", model.Hash);
        }

        internal static bool ObjectExists(int handle)
        {
            return Function.Call<bool>("DOES_OBJECT_EXIST", handle);
        }

        internal static int ObjectModel(int handle)
        {
            Pointer model = typeof(int);
            Function.Call("GET_OBJECT_MODEL", handle, model);
            return (int)model;
        }

        internal static void DeleteObject(int handle)
        {
            Pointer pointer = handle;
            Function.Call("DELETE_OBJECT", pointer);
        }
    }
}
