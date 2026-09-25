#include "ped_pool.h"

namespace lc
{
    const PedPool::Pool* PedPool::pool() const
    {
        if (global_ == 0) { return nullptr; }
        return *reinterpret_cast<Pool* const*>(global_);
    }

    // Sanity limits reject a pool pointer that does not look like the ped pool (fail closed; never iterate garbage).
    bool PedPool::valid() const
    {
        const Pool* p = pool();
        return p != nullptr && p->objects != 0 && p->flags != nullptr && p->size > 0 && p->size <= 4096 &&
            p->item_size >= 0x100 && p->item_size <= 0x4000;
    }

    int PedPool::size() const { return valid() ? pool()->size : 0; }

    int32_t PedPool::handle_at(int index) const
    {
        const Pool* p = pool();
        uint8_t flag = p->flags[index];
        if ((flag & 0x80) != 0) { return 0; }
        return (index << 8) | flag;
    }
}
