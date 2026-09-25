using GTA;
using GTA.Native;

namespace LibertyFramework.CombatEffects
{
    internal static class CombatEffectsNatives
    {
        internal static bool IsMissionPed(Ped ped)
        {
            return Function.Call<bool>("IS_PED_A_MISSION_PED", ped);
        }

        internal static void React(Ped ped, float x, float y, float z)
        {
            Function.Call("APPLY_FORCE_TO_PED", ped, 3, x, y, z, 0.0f, 0.0f, 0.0f, 0, 1, 1, 1);
        }

        internal static bool Impact(string name, Ped ped, int bone)
        {
            return Function.Call<bool>("TRIGGER_PTFX_ON_PED_BONE", name, ped,
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, bone, 0);
        }

        internal static int StartWound(string name, Ped ped, int bone)
        {
            return Function.Call<int>("START_PTFX_ON_PED_BONE", name, ped,
                0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, bone, 0);
        }

        internal static void StopWound(int handle)
        {
            if (handle > 0) Function.Call("STOP_PTFX", handle);
        }

        internal static void RemoveHead(Ped ped)
        {
            Function.Call("EXPLODE_CHAR_HEAD", ped);
        }
    }
}
