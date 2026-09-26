// Unit test of the raycast pass-through walk (src/ray_walk.h) against a scripted line test: a segment along +Y with
// "slabs" (a wall, peds, a vehicle) that the fake line test reports in order, honouring its one ignored entity.
// Built and run by tools/build-core.ps1; exits non-zero on the first failure.
#include <cstdio>
#include <vector>
#include "../src/ray_walk.h"

using namespace lc::ray;

namespace
{
    struct Slab { float y; int32_t kind; int32_t handle; uint32_t entity; };

    struct FakeWorld
    {
        std::vector<Slab> slabs;
        int calls = 0;
        bool fail_on_call = false;
        int fail_call = -1;

        Probe test(const Vec& from, const Vec& to, uint32_t ignore)
        {
            calls++;
            Probe p;
            if (fail_on_call && calls == fail_call) { p.status = kFailed; return p; }
            float lo = from.y < to.y ? from.y : to.y, hi = from.y < to.y ? to.y : from.y;
            const Slab* best = nullptr;
            for (const Slab& s : slabs)
            {
                if (s.entity != 0 && s.entity == ignore) { continue; }
                if (s.y < lo || s.y > hi) { continue; }
                if (best == nullptr || (s.y - from.y) * (to.y - from.y) < (best->y - from.y) * (to.y - from.y)) { best = &s; }
            }
            if (best == nullptr) { p.status = kClear; return p; }
            p.status = kHit;
            p.position = Vec{ 0, best->y, 1 };
            p.kind = best->kind;
            p.handle = best->handle;
            p.entity = best->entity;
            return p;
        }
    };

    int failures = 0;

    void expect(bool ok, const char* what)
    {
        if (!ok) { failures++; std::printf("FAIL %s\n", what); }
        else { std::printf("ok   %s\n", what); }
    }

    Query along_y(float length, uint32_t accept, int max_passes)
    {
        Query q;
        q.start = Vec{ 0, 0, 1 };
        q.end = Vec{ 0, length, 1 };
        q.accept = accept;
        q.max_passes = max_passes;
        q.step = 0.05f;
        return q;
    }

    Outcome run(FakeWorld& w, const Query& q)
    {
        return walk(q, [&](const Vec& from, const Vec& to, uint32_t ignore) { return w.test(from, to, ignore); });
    }

    const uint32_t kWorld = 0x1, kPeds = 0x2, kVehicles = 0x4, kAll = 0xF;
}

int main()
{
    {
        FakeWorld w;
        Outcome o = run(w, along_y(20, kAll, 8));
        expect(o.result == kClear && o.tests == 1 && o.passes == 0, "empty segment is clear after one test");
    }
    {
        FakeWorld w;
        w.slabs = { { 10, 0, 0, 0 } };
        Outcome o = run(w, along_y(20, kAll, 8));
        expect(o.result == kHit && o.last.kind == 0 && o.distance > 9.99f && o.distance < 10.01f, "wall hit, distance 10");
    }
    {
        // Line of sight: peds do not block; the wall behind them does.
        FakeWorld w;
        w.slabs = { { 3, 1, 0x100, 0xA000 }, { 5, 1, 0x200, 0xB000 }, { 12, 0, 0, 0 } };
        Outcome o = run(w, along_y(20, kWorld | kVehicles, 8));
        expect(o.result == kHit && o.last.kind == 0 && o.passes == 2 && o.tests == 3, "passes two peds, stops at the wall");
    }
    {
        FakeWorld w;
        w.slabs = { { 3, 1, 0x100, 0xA000 }, { 5, 1, 0x200, 0xB000 } };
        Outcome o = run(w, along_y(20, kWorld, 8));
        expect(o.result == kClear && o.passes == 2, "excluded hits only: clear");
    }
    {
        FakeWorld w;
        w.slabs = { { 3, 1, 0x100, 0xA000 }, { 5, 1, 0x200, 0xB000 }, { 7, 1, 0x300, 0xC000 } };
        Outcome o = run(w, along_y(20, kWorld, 2));
        expect(o.result == kInconclusive && o.passes == 2 && o.last.handle == 0x300, "pass budget exhausted: inconclusive with the last excluded hit");
    }
    {
        FakeWorld w;
        w.slabs = { { 3, 1, 0x100, 0xA000 } };
        Outcome o = run(w, along_y(20, kAll, 0));
        expect(o.result == kHit && o.last.handle == 0x100, "max_passes 0 still reports an accepted first hit");
    }
    {
        // The caller ignores its own ped (first ignore goes to the game) and its vehicle (walked through).
        FakeWorld w;
        w.slabs = { { 0.5f, 1, 0x100, 0xA000 }, { 1.5f, 2, 0x400, 0xD000 }, { 9, 1, 0x200, 0xB000 } };
        Query q = along_y(20, kAll, 8);
        q.ignore_count = 2;
        q.ignore_kind[0] = 1; q.ignore_handle[0] = 0x100; q.first_ignore = 0xA000;
        q.ignore_kind[1] = 2; q.ignore_handle[1] = 0x400;
        Outcome o = run(w, q);
        expect(o.result == kHit && o.last.handle == 0x200 && o.passes == 1, "ignored self and own vehicle, hits the ped beyond");
    }
    {
        // An ignored entity hit again later (e.g. a second surface of the same vehicle) is still passed through.
        FakeWorld w;
        w.slabs = { { 2, 2, 0x400, 0xD000 }, { 2.2f, 2, 0x400, 0xD001 }, { 6, 0, 0, 0 } };
        Query q = along_y(20, kAll, 8);
        q.ignore_count = 1; q.ignore_kind[0] = 2; q.ignore_handle[0] = 0x400;
        Outcome o = run(w, q);
        expect(o.result == kHit && o.last.kind == 0 && o.last.position.y > 5.9f && o.passes == 2, "ignored vehicle passed twice, wall hit");
    }
    {
        // A hit behind the start (bad data) must not stall the walk: every pass advances by at least one step.
        FakeWorld w;
        int calls = 0;
        Query q = along_y(1, kWorld, 100);
        Outcome o = walk(q, [&](const Vec&, const Vec&, uint32_t) { calls++; Probe p; p.status = kHit; p.kind = 1; p.handle = 7; p.position = Vec{ 0, -5, 1 }; return p; });
        expect(o.result == kClear && calls <= 21, "walk always advances and ends");
    }
    {
        FakeWorld w;
        w.slabs = { { 3, 1, 0x100, 0xA000 }, { 5, 0, 0, 0 } };
        w.fail_on_call = true; w.fail_call = 2;
        Outcome o = run(w, along_y(20, kWorld, 8));
        expect(o.result == kFailed && o.tests == 2, "a failed test fails the query");
    }
    {
        FakeWorld w;
        Query q = along_y(20, kAll, 8);
        q.end = q.start;
        expect(run(w, q).result == kFailed && w.calls == 0, "zero-length ray refused without a test");
        q = along_y(20, kAll, 8);
        q.step = 0;
        expect(run(w, q).result == kFailed, "non-positive step refused");
    }
    {
        FakeWorld w;
        w.slabs = { { 4, 3, 0x900, 0xE000 } };
        Outcome o = run(w, along_y(20, kPeds, 8));
        expect(o.result == kClear && o.passes == 1, "object excluded when only peds stop the ray");
    }
    std::printf(failures == 0 ? "ray_walk_test passed\n" : "ray_walk_test FAILED (%d)\n", failures);
    return failures == 0 ? 0 : 1;
}
