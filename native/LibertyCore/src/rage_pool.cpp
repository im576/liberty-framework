#include "rage_pool.h"

namespace lc
{
    const RagePool::Pool* RagePool::pool() const
    {
        if (global_ == 0) { return nullptr; }
        return *reinterpret_cast<Pool* const*>(global_);
    }

    // Sanity limits reject a pointer that does not look like a pool (fail closed; never iterate garbage).
    bool RagePool::valid() const
    {
        const Pool* p = pool();
        return p != nullptr && p->objects != 0 && p->flags != nullptr && p->size > 0 && p->size <= 16384 &&
            p->item_size >= 0x40 && p->item_size <= max_item_;
    }

    int RagePool::size() const { return valid() ? pool()->size : 0; }

    int32_t RagePool::handle_at(int index) const
    {
        const Pool* p = pool();
        uint8_t flag = p->flags[index];
        if ((flag & 0x80) != 0) { return 0; }
        return (index << 8) | flag;
    }

    uint32_t RagePool::object_at(int index) const
    {
        const Pool* p = pool();
        return p->objects + static_cast<uint32_t>(index) * static_cast<uint32_t>(p->item_size);
    }

    int32_t RagePool::handle_of(uint32_t address) const
    {
        if (!valid() || address < pool()->objects) { return 0; }
        const Pool* p = pool();
        uint32_t delta = address - p->objects;
        uint32_t index = delta / static_cast<uint32_t>(p->item_size);
        if (delta % static_cast<uint32_t>(p->item_size) != 0 || index >= static_cast<uint32_t>(p->size)) { return 0; }
        return handle_at(static_cast<int>(index));
    }

    int RagePool::used() const
    {
        if (!valid()) { return 0; }
        const Pool* p = pool();
        int n = 0;
        for (int i = 0; i < p->size; i++) { if ((p->flags[i] & 0x80) == 0) { n++; } }
        return n;
    }
}
