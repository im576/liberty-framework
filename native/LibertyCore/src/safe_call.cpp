#include <windows.h>
#include "safe_call.h"

// An SEH frame, not a vectored handler: exception dispatch offers a fault to every vectored handler and to every SEH
// frame registered deeper in the call (the game's own handlers, including code that relies on exceptions) before it
// reaches ours. So the core only catches faults nothing else would have handled. A vectored handler installed first
// swallowed such exceptions and left the game corrupted (2026-09-25).
extern "C"
{
    int32_t lc_safe_invoke(uint32_t function, void* argument);
    void lc_safe_resume();
    EXCEPTION_DISPOSITION __cdecl lc_safe_handler(EXCEPTION_RECORD* record, void* frame, CONTEXT* context, void* dispatcher);
}

// i686 cdecl int32_t lc_safe_invoke(function, argument). Frame layout (esp after registration = "frame"):
//   frame+0  next SEH record          frame+4  lc_safe_handler      frame+8  saved ebp
//   frame+12 x87 environment (28 bytes, fnstenv then fldenv at once: fnstenv masks FPU exceptions as a side effect)
// A fault sends the thread to lc_safe_resume with esp = frame; it unlinks the record (dropping any records the faulting
// code registered deeper, which live on the abandoned stack), restores the x87 environment and returns 0.
asm(R"(
    .intel_syntax noprefix
    .text
    .globl _lc_safe_invoke
_lc_safe_invoke:
    push ebp
    mov ebp, esp
    push ebx
    push esi
    push edi
    sub esp, 28
    fnstenv [esp]
    fldenv [esp]
    push ebp
    lea eax, [_lc_safe_handler]
    push eax
    push dword ptr fs:[0]
    mov dword ptr fs:[0], esp
    push dword ptr [ebp + 12]
    call dword ptr [ebp + 8]
    add esp, 4
    mov eax, dword ptr [esp]
    mov dword ptr fs:[0], eax
    add esp, 40
    mov eax, 1
    jmp 2f
    .globl _lc_safe_resume
_lc_safe_resume:
    mov eax, dword ptr [esp]
    mov dword ptr fs:[0], eax
    fldenv [esp + 12]
    add esp, 40
    xor eax, eax
2:
    pop edi
    pop esi
    pop ebx
    pop ebp
    ret
    .att_syntax
)");

namespace
{
    lc::FaultRecord g_fault{};
    lc::FaultRecord g_history[16] = {};

    bool Recoverable(DWORD code)
    {
        switch (code)
        {
        case EXCEPTION_ACCESS_VIOLATION:
        case EXCEPTION_ILLEGAL_INSTRUCTION:
        case EXCEPTION_PRIV_INSTRUCTION:
        case EXCEPTION_INT_DIVIDE_BY_ZERO:
        case EXCEPTION_INT_OVERFLOW:
        case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:
        case EXCEPTION_DATATYPE_MISALIGNMENT:
            return true;
        default:
            return false;
        }
    }
}

extern "C" EXCEPTION_DISPOSITION __cdecl lc_safe_handler(EXCEPTION_RECORD* record, void* frame, CONTEXT* context, void*)
{
    // Unwind passes (someone else is unwinding through us) and exceptions we do not recover from go on searching.
    if (record == nullptr || context == nullptr || (record->ExceptionFlags & (EXCEPTION_UNWINDING | EXCEPTION_EXIT_UNWIND)) != 0 ||
        !Recoverable(record->ExceptionCode))
    {
        return ExceptionContinueSearch;
    }
    g_fault.count++;
    g_fault.code = record->ExceptionCode;
    g_fault.address = reinterpret_cast<uint32_t>(record->ExceptionAddress);
    g_fault.data_address = record->NumberParameters >= 2 ? static_cast<uint32_t>(record->ExceptionInformation[1]) : 0;
    g_history[(g_fault.count - 1) % 16] = g_fault;
    uint32_t* registration = static_cast<uint32_t*>(frame);
    context->Esp = reinterpret_cast<DWORD>(registration);
    context->Ebp = registration[2];
    context->Eip = reinterpret_cast<DWORD>(&lc_safe_resume);
    return ExceptionContinueExecution;
}

namespace lc
{
    void install_safe_calls() { }

    bool safe_invoke(uint32_t function, void* argument) { return lc_safe_invoke(function, argument) != 0; }

    const FaultRecord& last_fault() { return g_fault; }

    const FaultRecord& fault_at(uint32_t number) { return g_history[(number - 1) % 16]; }
}
