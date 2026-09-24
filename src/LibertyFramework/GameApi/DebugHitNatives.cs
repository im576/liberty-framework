using GTA;
using GTA.Native;

namespace LibertyFramework.GameApi
{
    internal static class DebugHitNatives
    {
        internal static int LastDamageBone(Ped ped)
        {
            Pointer bone = typeof(int);
            Function.Call("GET_CHAR_LAST_DAMAGE_BONE", ped, bone);
            return (int)bone;
        }

        internal static bool VehicleDamagedBy(Vehicle vehicle, Ped shooter)
        {
            return Function.Call<bool>("HAS_CAR_BEEN_DAMAGED_BY_CHAR", vehicle, shooter);
        }
    }
}
