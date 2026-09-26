// Unit test of the direct native invoker (src/natives.cpp) against a fake safe_invoke: a native that faults is switched
// off for the session and is never invoked again, even by callers that checked ready() before the fault (the rest of
// that frame's ped or vehicle list). Built and run by tools/build-core.ps1; exits non-zero on the first failure.
#include <cstdio>
#include "../src/natives.cpp"

namespace
{
    const uint32_t kGoodHandler = 0x1000, kFaultyHandler = 0x2000;
    int g_good_calls = 0, g_faulty_calls = 0;
    lc::FaultRecord g_fault = {};
    int g_failures = 0;

    void expect(bool condition, const char* what)
    {
        std::printf("%s %s\n", condition ? "ok  " : "FAIL", what);
        if (!condition) { g_failures++; }
    }
}

namespace lc
{
    // The fake game: the good handler writes 42 to the result slot and 7 to its first out slot; the faulty one "faults".
    bool safe_invoke(uint32_t function, void* argument)
    {
        Context* context = static_cast<Context*>(argument);
        if (function == kFaultyHandler)
        {
            g_faulty_calls++;
            g_fault.count++;
            return false;
        }
        g_good_calls++;
        static_cast<int32_t*>(context->result)[0] = 42;
        uint32_t* args = static_cast<uint32_t*>(context->args);
        if (context->arg_count >= 2) { *reinterpret_cast<int32_t*>(args[1]) = 7; }
        return true;
    }

    void install_safe_calls() {}
    const FaultRecord& last_fault() { return g_fault; }
    const FaultRecord& fault_at(uint32_t) { return g_fault; }
}

int main()
{
    lc::Natives natives;
    // GET_CHAR_HEALTH: one value argument, one out slot.
    const int good = LC_N_GET_CHAR_HEALTH, faulty = LC_N_GET_CAR_HEALTH;
    natives.set_handler(good, kGoodHandler);
    natives.set_handler(faulty, kFaultyHandler);
    natives.set_verified(good, true);
    natives.set_verified(faulty, true);
    expect(natives.ready(good) && natives.ready(faulty), "both natives verified");

    int32_t out = -1;
    expect(natives.out1(good, 5, out) == 42 && out == 7 && g_good_calls == 1, "a working native returns its result and out slot");

    out = -1;
    expect(natives.out1(faulty, 5, out) == 0 && out == 0, "a faulting native returns 0 with its out slot zeroed");
    expect(!natives.ready(faulty) && natives.faulted(faulty), "the fault switches the native off");
    expect(natives.last_fault_native() == faulty && natives.last_fault_argument() == 5, "the fault names the native and its argument");

    // A caller that checked ready() earlier this frame calls again (the next vehicle in the list).
    out = -1;
    int before = g_faulty_calls;
    expect(natives.out1(faulty, 6, out) == 0 && out == 0, "a faulted native still answers 0");
    expect(g_faulty_calls == before, "a faulted native is not invoked again");

    natives.set_verified(faulty, true);
    expect(!natives.ready(faulty), "verification cannot switch a faulted native back on");

    natives.set_handler(faulty, kGoodHandler);
    natives.set_verified(faulty, true);
    expect(natives.ready(faulty) && natives.out1(faulty, 1, out) == 42, "a new handler (a new lc_init) starts clean");
    expect(natives.ready(good) && natives.out1(good, 1, out) == 42, "other natives are unaffected");

    std::printf("natives_test %s\n", g_failures == 0 ? "passed" : "failed");
    return g_failures == 0 ? 0 : 1;
}
