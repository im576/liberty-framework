// Exact ped damage (ADR-0007): an entry detour on the game's ped damage-response routine records every damage it applies
// (calculator and response values) into a ring the core frame drains. Observe only: the result is never changed.
#pragma once
#include <cstdint>

namespace lc
{
    namespace damage
    {
        struct Record
        {
            uint32_t victim;       // CPed*
            uint32_t damager;      // CEntity* or 0
            int32_t weapon;
            int32_t component;
            int32_t bone;          // tag, or -1
            float amount;
            float health_lost;
            float armour_lost;
            uint32_t flags;
        };

        // function: the damage-response routine; component_to_bone: cdecl (CPed*, component) -> bone tag.
        bool install(uint32_t function, uint32_t component_to_bone);
        void remove();
        bool active();
        // Copies up to capacity pending records (oldest first); returns the count. dropped = records lost to a full ring.
        int drain(Record* out, int capacity, uint32_t& dropped);
    }
}
