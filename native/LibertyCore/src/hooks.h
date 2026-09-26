// LibertyCore hook manager (ADR-0007). Entry detours with verified displaced bytes: the caller passes the exact bytes
// the function must start with (from its signature); only position-independent instructions may be displaced. A target
// that differs, or already starts with a jump (another mod's hook), is refused. Removal restores the original bytes
// only while the jump is still ours. Install and remove only while the game thread is parked (engine tick).
#pragma once
#include <cstdint>

namespace lc
{
    namespace hooks
    {
        // Returns the trampoline (call it to run the original function) or 0 when refused. Installing a name that is
        // already installed returns its existing trampoline (script reloads keep the core loaded).
        uint32_t install_entry(const char* name, uint32_t target, const uint8_t* expected, int length, uint32_t detour);
        bool remove(const char* name);
        void remove_all();
        bool installed(const char* name);
        // "name target=0x.. installed|removed" lines.
        int report(char* buffer, int size);
    }
}
