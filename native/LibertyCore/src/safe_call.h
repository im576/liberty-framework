// Exception-safe invocation (ADR-0006 safety net). safe_invoke(fn, arg) calls fn(arg) (cdecl) with a recovery point:
// if fn faults (access violation, illegal instruction, divide by zero, ...) and no handler of the game's own takes the
// exception first, an SEH frame registered around the call restores the stack, frame pointer, SEH chain and x87
// environment and resumes after it, returning false. Used for every direct native call and every raw game-memory read the core makes, so a bad entity costs one
// disabled native and a log line instead of the game. Re-entrant (each call carries its own frame).
#pragma once
#include <cstdint>

namespace lc
{
    struct FaultRecord
    {
        uint32_t count;        // faults contained this session
        uint32_t address;      // faulting instruction
        uint32_t data_address; // address that was read or written (access violations)
        uint32_t code;         // exception code
    };

    void install_safe_calls();
    bool safe_invoke(uint32_t function, void* argument);
    const FaultRecord& last_fault();
    // Fault number n (1-based) while it is among the last 16.
    const FaultRecord& fault_at(uint32_t number);
}
