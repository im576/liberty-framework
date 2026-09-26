// Direct script-native invoker (ADR-0006). CE native ABI: handler(ctx) cdecl; ctx+0 = pointer to the return slot,
// ctx+4 = argument count, ctx+8 = pointer to the argument array (4 bytes each; out-parameters are pointers).
#pragma once
#include <cstdint>
#include "liberty_core.h"

namespace lc
{
    struct NativeInfo
    {
        const char* name;
        uint32_t hash;       // CE hash (FusionFix natives.ixx)
        int in_args;         // leading value arguments
        int out_args;        // trailing out-pointer arguments (4-byte slots)
    };

    const NativeInfo& native_info(int id);

    class Natives
    {
    public:
        void set_handler(int id, uint32_t handler) { handlers_[id] = handler; verified_[id] = false; faulted_[id] = false; }
        void set_verified(int id, bool verified) { verified_[id] = verified && handlers_[id] != 0 && !faulted_[id]; }
        bool faulted(int id) const { return id >= 0 && id < LC_N_COUNT && faulted_[id]; }
        bool ready(int id) const { return id >= 0 && id < LC_N_COUNT && verified_[id]; }
        bool available(int id) const { return id >= 0 && id < LC_N_COUNT && handlers_[id] != 0; }

        // Calls native 'id' with its value arguments; out slots are written to outs[0..out_args).
        // Returns the 4-byte return slot. The caller guarantees ready(id) or is verifying (available(id)).
        // A native that faulted once is never called again this session (returns 0, outs zeroed): callers check ready()
        // once per frame, so without this the rest of that frame's ped or vehicle list would call it again on every
        // entity, and a contained fault can still corrupt game state (GET_DRIVER_OF_CAR, 2026-09-25).
        int32_t call(int id, const int32_t* args, int32_t* outs);

        int32_t call0(int id) { return call(id, nullptr, nullptr); }
        int32_t call1(int id, int32_t a) { int32_t args[1] = { a }; return call(id, args, nullptr); }
        int32_t out1(int id, int32_t a, int32_t& out) { int32_t args[1] = { a }; return call(id, args, &out); }

        // The native and first argument of the most recent contained fault (-1 when none).
        int last_fault_native() const { return last_fault_native_; }
        // Native of fault number n (1-based) among the last 16 (-1 for a raw read, -2 for the raycast line test).
        int fault_native(uint32_t number) const { return fault_natives_[(number - 1) % 16]; }
        void note_raw_fault(uint32_t number) { fault_natives_[(number - 1) % 16] = -1; }
        // The raycast line test (ABI 5) reports as native -2.
        void note_raycast_fault(uint32_t number) { fault_natives_[(number - 1) % 16] = -2; }
        int32_t last_fault_argument() const { return last_fault_argument_; }

    private:
        uint32_t handlers_[LC_N_COUNT] = {};
        bool verified_[LC_N_COUNT] = {};
        bool faulted_[LC_N_COUNT] = {};
        int last_fault_native_ = -1;
        int fault_natives_[16] = {};
        int32_t last_fault_argument_ = 0;
    };
}
