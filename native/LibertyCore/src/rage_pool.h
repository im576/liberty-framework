// rage::fwPool view (MEMORY.md, T-022): objects +0, flags +4, size +8, item size +12. A slot is live when its flag
// byte has bit 7 clear; the handle is index << 8 | flag (the generation), the same rule the engine's GetAt checks.
// The same layout serves the ped, vehicle and object pools (each resolved from its DOES_*_EXIST worker).
#pragma once
#include <cstdint>

namespace lc
{
    class RagePool
    {
    public:
        RagePool(uint32_t pool_global, int32_t max_item_size) : global_(pool_global), max_item_(max_item_size) {}

        bool valid() const;
        int size() const;
        // Handle of slot 'index', or 0 when the slot is free.
        int32_t handle_at(int index) const;
        int used() const;
        // Address of slot 'index''s object (valid only while the slot is live).
        uint32_t object_at(int index) const;
        // Handle of the live entity whose object starts at 'address', or 0 (not in this pool, misaligned or free).
        int32_t handle_of(uint32_t address) const;

    private:
        struct Pool
        {
            uint32_t objects;
            uint8_t* flags;
            int32_t size;
            int32_t item_size;
        };
        const Pool* pool() const;
        uint32_t global_;
        int32_t max_item_;
    };
}
