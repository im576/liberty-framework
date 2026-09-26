#include "natives.h"
#include "safe_call.h"

namespace lc
{
    namespace
    {
        // Order must match lc_native_id. Signatures from FusionFix natives.ixx (hashes and parameter lists are facts
        // about the game, not code); every entry is verified against ScriptHookDotNet by the host before use.
        const NativeInfo kNatives[LC_N_COUNT] = {
            { "DOES_CHAR_EXIST", 0x46531797, 1, 0 },
            { "IS_CHAR_DEAD", 0x6A6B4F18, 1, 0 },
            { "GET_CHAR_HEALTH", 0x4B6C2256, 1, 1 },
            { "GET_CHAR_ARMOUR", 0x3C756E54, 1, 1 },
            { "GET_CHAR_COORDINATES", 0x2B5C06E6, 1, 3 },
            { "GET_CHAR_HEADING", 0x057A3AC7, 1, 1 },
            { "GET_CHAR_MODEL", 0x0A3D60CE, 1, 1 },
            { "IS_CHAR_IN_ANY_CAR", 0x71184DA3, 1, 0 },
            { "GET_CAR_CHAR_IS_USING", 0x1B067237, 1, 1 },
            { "GET_CURRENT_CHAR_WEAPON", 0x5AB8289F, 1, 1 },
            { "GET_AMMO_IN_CLIP", 0x612C748F, 2, 1 },
            { "GET_CHAR_LAST_DAMAGE_BONE", 0x767E5013, 1, 1 },
            { "HAS_CHAR_BEEN_DAMAGED_BY_CHAR", 0x1DD624A0, 3, 0 },
            { "GET_PLAYER_ID", 0x62E319C6, 0, 0 },
            { "IS_PLAYER_PLAYING", 0x08274BA4, 1, 0 },
            { "IS_PLAYER_CONTROL_ON", 0x30CD2F1F, 1, 0 },
            { "IS_PAUSE_MENU_ACTIVE", 0x6C4568A7, 0, 0 },
            { "IS_SCREEN_FADED_OUT", 0x59EE3A11, 0, 0 },
            { "GET_GAME_TIMER", 0x022B2DA9, 0, 1 },
            { "GET_HOURS_OF_DAY", 0x0A9F7BA1, 0, 0 },
            { "GET_MINUTES_OF_DAY", 0x3DFE691D, 0, 0 },
            { "GET_CURRENT_WEATHER", 0x27E421EA, 0, 1 },
            { "DOES_VEHICLE_EXIST", 0x67A42263, 1, 0 },
            { "GET_CAR_COORDINATES", 0x2D432EAB, 1, 3 },
            { "GET_CAR_HEADING", 0x46803CFA, 1, 1 },
            { "GET_CAR_SPEED", 0x16DD2D00, 1, 1 },
            { "GET_CAR_HEALTH", 0x4D417CD3, 1, 1 },
            { "GET_ENGINE_HEALTH", 0x2B0A05E0, 1, 0 },
            { "GET_CAR_MODEL", 0x5FF84497, 1, 1 },
            { "GET_DRIVER_OF_CAR", 0x22457083, 1, 1 },
        };

        struct Context
        {
            void* result;
            uint32_t arg_count;
            void* args;
        };

    }

    const NativeInfo& native_info(int id) { return kNatives[id]; }

    int32_t Natives::call(int id, const int32_t* args, int32_t* outs)
    {
        const NativeInfo& info = kNatives[id];
        // Returns land in a 16-byte slot: some handlers write a vector-sized result.
        alignas(16) int32_t result[4] = {};
        alignas(16) int32_t out_slots[4] = {};
        uint32_t argv[8] = {};
        int count = 0;
        for (int i = 0; i < info.in_args; i++) { argv[count++] = static_cast<uint32_t>(args[i]); }
        for (int i = 0; i < info.out_args; i++) { argv[count++] = reinterpret_cast<uint32_t>(&out_slots[i]); }
        Context context{ result, static_cast<uint32_t>(count), argv };
        // Contained call: a native that faults on some entity is switched off for the session (safe_call.h).
        if (!safe_invoke(handlers_[id], &context))
        {
            verified_[id] = false;
            last_fault_native_ = id;
            fault_natives_[(last_fault().count - 1) % 16] = id;
            last_fault_argument_ = info.in_args > 0 ? args[0] : 0;
            for (int i = 0; i < info.out_args && outs != nullptr; i++) { outs[i] = 0; }
            return 0;
        }
        for (int i = 0; i < info.out_args && outs != nullptr; i++) { outs[i] = out_slots[i]; }
        return result[0];
    }
}
