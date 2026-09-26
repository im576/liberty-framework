#include <windows.h>
#include "damage_hook.h"
#include "hooks.h"
#include "safe_call.h"

namespace
{
    // sub esp,0Ch; push ebx; mov ebx,[esp+18h]: 8 position-independent bytes (ADR-0007).
    const uint8_t kPrologue[8] = { 0x83, 0xEC, 0x0C, 0x53, 0x8B, 0x5C, 0x24, 0x18 };
    const char* const kName = "ped_damage_response";
    const uint32_t kRing = 64;

    typedef void(__thiscall* DamageFunction)(void* calculator, void* ped, void* response);
    typedef int32_t(__cdecl* ComponentToBone)(void* ped, int32_t component);

    DamageFunction g_original = nullptr;
    ComponentToBone g_to_bone = nullptr;
    lc::damage::Record g_ring[kRing];
    volatile uint32_t g_head = 0;   // written by the detour (game thread)
    volatile uint32_t g_tail = 0;   // written by the core frame (same OS thread, script fiber)
    volatile uint32_t g_dropped = 0;

    struct Capture
    {
        void* calculator;
        void* ped;
        void* response;
    };

    // Runs under safe_invoke: reads only what the game just used.
    void record_thunk(void* argument)
    {
        Capture* c = static_cast<Capture*>(argument);
        const uint8_t* calc = static_cast<const uint8_t*>(c->calculator);
        const uint8_t* response = static_cast<const uint8_t*>(c->response);
        if (calc == nullptr || response == nullptr || c->ped == nullptr) { return; }
        if (g_head - g_tail >= kRing) { g_dropped = g_dropped + 1; return; }
        lc::damage::Record& r = g_ring[g_head % kRing];
        r.victim = reinterpret_cast<uint32_t>(c->ped);
        r.damager = *reinterpret_cast<const uint32_t*>(calc + 0x0);
        r.amount = *reinterpret_cast<const float*>(calc + 0x4);
        r.component = *reinterpret_cast<const int32_t*>(calc + 0x8);
        r.weapon = *reinterpret_cast<const int32_t*>(calc + 0xC);
        r.flags = *(response + 0x4);
        r.health_lost = *reinterpret_cast<const float*>(response + 0x8);
        r.armour_lost = *reinterpret_cast<const float*>(response + 0xC);
        r.bone = r.component > 0 && g_to_bone != nullptr ? g_to_bone(c->ped, r.component) : -1;
        g_head = g_head + 1;
    }

    // Game callers use thiscall (ecx = calculator, two stack arguments, callee pops 8); fastcall has the same shape.
    void __fastcall detour(void* calculator, void* /*edx*/, void* ped, void* response)
    {
        g_original(calculator, ped, response);
        Capture capture{ calculator, ped, response };
        lc::safe_invoke(reinterpret_cast<uint32_t>(&record_thunk), &capture);
    }
}

namespace lc
{
    namespace damage
    {
        bool install(uint32_t function, uint32_t component_to_bone)
        {
            if (function == 0) { return false; }
            g_to_bone = reinterpret_cast<ComponentToBone>(component_to_bone);
            uint32_t trampoline = hooks::install_entry(kName, function, kPrologue, sizeof(kPrologue), reinterpret_cast<uint32_t>(&detour));
            if (trampoline == 0) { return false; }
            g_original = reinterpret_cast<DamageFunction>(trampoline);
            return true;
        }

        void remove() { hooks::remove(kName); }

        bool active() { return hooks::installed(kName); }

        int drain(Record* out, int capacity, uint32_t& dropped)
        {
            int n = 0;
            while (g_tail != g_head && n < capacity) { out[n++] = g_ring[g_tail % kRing]; g_tail = g_tail + 1; }
            dropped = g_dropped;
            return n;
        }

        uint32_t dropped() { return g_dropped; }
    }
}
