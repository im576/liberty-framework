// rage::fwPool view (MEMORY.md, T-022): objects +0, flags +4, size +8, item size +12. A slot is live when its flag
// byte has bit 7 clear; the handle is index << 8 | flag (the generation), the same rule the engine's GetAt checks.
#pragma once
#include <cstdint>

namespace lc
{
    class PedPool
    {
    public:
        explicit PedPool(uint32_t pool_global) : global_(pool_global) {}

        bool valid() const;
        int size() const;
        // Handle of slot 'index', or 0 when the slot is free.
        int32_t handle_at(int index) const;

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
    };
}
