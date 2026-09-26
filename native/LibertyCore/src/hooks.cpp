#include <windows.h>
#include <cstdio>
#include <cstring>
#include "hooks.h"

namespace
{
    const int kMaxHooks = 16;
    const int kMaxStolen = 16;

    struct Hook
    {
        char name[32];
        uint32_t target;
        uint32_t detour;
        uint8_t original[kMaxStolen];
        int length;
        uint8_t* trampoline;
        bool active;
    };

    Hook g_hooks[kMaxHooks];
    int g_count = 0;

    Hook* find(const char* name)
    {
        for (int i = 0; i < g_count; i++) { if (std::strcmp(g_hooks[i].name, name) == 0) { return &g_hooks[i]; } }
        return nullptr;
    }

    bool write_code(uint32_t address, const uint8_t* bytes, int length)
    {
        DWORD old = 0;
        void* target = reinterpret_cast<void*>(address);
        if (!VirtualProtect(target, static_cast<SIZE_T>(length), PAGE_EXECUTE_READWRITE, &old)) { return false; }
        std::memcpy(target, bytes, static_cast<size_t>(length));
        DWORD ignored = 0;
        VirtualProtect(target, static_cast<SIZE_T>(length), old, &ignored);
        FlushInstructionCache(GetCurrentProcess(), target, static_cast<SIZE_T>(length));
        return true;
    }

    void rel32_jump(uint8_t* at, uint32_t from, uint32_t to)
    {
        at[0] = 0xE9;
        int32_t rel = static_cast<int32_t>(to - (from + 5));
        std::memcpy(at + 1, &rel, 4);
    }

    // Trampoline: the displaced bytes, then a jump back behind them. Written, then made read/execute.
    uint8_t* make_trampoline(uint32_t target, const uint8_t* stolen, int length)
    {
        uint8_t* memory = static_cast<uint8_t*>(VirtualAlloc(nullptr, 64, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE));
        if (memory == nullptr) { return nullptr; }
        std::memcpy(memory, stolen, static_cast<size_t>(length));
        rel32_jump(memory + length, reinterpret_cast<uint32_t>(memory + length), target + static_cast<uint32_t>(length));
        DWORD old = 0;
        if (!VirtualProtect(memory, 64, PAGE_EXECUTE_READ, &old)) { VirtualFree(memory, 0, MEM_RELEASE); return nullptr; }
        FlushInstructionCache(GetCurrentProcess(), memory, 64);
        return memory;
    }
}

namespace lc
{
    namespace hooks
    {
        uint32_t install_entry(const char* name, uint32_t target, const uint8_t* expected, int length, uint32_t detour)
        {
            if (name == nullptr || target == 0 || expected == nullptr || detour == 0 || length < 5 || length > kMaxStolen) { return 0; }
            Hook* existing = find(name);
            if (existing != nullptr && existing->active) { return existing->target == target ? reinterpret_cast<uint32_t>(existing->trampoline) : 0; }
            // The live bytes must be exactly what the signature promised (this also refuses a jump someone else placed).
            if (std::memcmp(reinterpret_cast<const void*>(target), expected, static_cast<size_t>(length)) != 0) { return 0; }
            Hook* hook = existing;
            if (hook == nullptr)
            {
                if (g_count >= kMaxHooks) { return 0; }
                hook = &g_hooks[g_count++];
                std::memset(hook, 0, sizeof(Hook));
                std::strncpy(hook->name, name, sizeof(hook->name) - 1);
            }
            hook->target = target;
            hook->detour = detour;
            hook->length = length;
            std::memcpy(hook->original, expected, static_cast<size_t>(length));
            if (hook->trampoline == nullptr) { hook->trampoline = make_trampoline(target, expected, length); }
            if (hook->trampoline == nullptr) { return 0; }
            uint8_t patch[kMaxStolen];
            std::memset(patch, 0x90, sizeof(patch));
            rel32_jump(patch, target, detour);
            if (!write_code(target, patch, length)) { return 0; }
            hook->active = true;
            return reinterpret_cast<uint32_t>(hook->trampoline);
        }

        bool remove(const char* name)
        {
            Hook* hook = find(name);
            if (hook == nullptr || !hook->active) { return false; }
            // Only undo our own jump; if someone patched over it, leave their bytes alone.
            uint8_t ours[5];
            rel32_jump(ours, hook->target, hook->detour);
            if (std::memcmp(reinterpret_cast<const void*>(hook->target), ours, 5) != 0) { hook->active = false; return false; }
            if (!write_code(hook->target, hook->original, hook->length)) { return false; }
            // The trampoline stays allocated: a thread could still be returning through it.
            hook->active = false;
            return true;
        }

        void remove_all()
        {
            for (int i = 0; i < g_count; i++) { if (g_hooks[i].active) { remove(g_hooks[i].name); } }
        }

        bool installed(const char* name)
        {
            Hook* hook = find(name);
            return hook != nullptr && hook->active;
        }

        int report(char* buffer, int size)
        {
            if (buffer == nullptr || size <= 0) { return 0; }
            int used = 0;
            buffer[0] = 0;
            for (int i = 0; i < g_count && used < size - 1; i++)
            {
                int n = std::snprintf(buffer + used, static_cast<size_t>(size - used), "%s target=0x%08X %s\n", g_hooks[i].name,
                    g_hooks[i].target, g_hooks[i].active ? "installed" : "removed");
                if (n < 0) { break; }
                used += n;
            }
            return g_count;
        }
    }
}
