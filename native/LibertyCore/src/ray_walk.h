// The raycast pass-through walk (ADR-0008, docs/research/Raycast.md). Pure: no game, no Windows, so
// native/LibertyCore/tests/ray_walk_test.cpp runs it against a scripted line test.
//
// The game's line test reports only the first thing along a segment and ignores at most one entity. A query that
// accepts some kinds only (e.g. world and vehicles for line of sight) or ignores several entities walks the segment:
// each excluded hit is passed through by testing again from 'step' metres beyond it, with that hit's entity as the
// game's ignored entity. Every pass moves the start at least 'step' metres further along the segment, so the walk
// ends; 'max_passes' bounds its cost.
#pragma once
#include <cmath>
#include <cstdint>

namespace lc::ray
{
    struct Vec { float x, y, z; };

    inline Vec operator+(Vec a, Vec b) { return Vec{ a.x + b.x, a.y + b.y, a.z + b.z }; }
    inline Vec operator-(Vec a, Vec b) { return Vec{ a.x - b.x, a.y - b.y, a.z - b.z }; }
    inline Vec operator*(Vec a, float s) { return Vec{ a.x * s, a.y * s, a.z * s }; }
    inline float dot(Vec a, Vec b) { return a.x * b.x + a.y * b.y + a.z * b.z; }
    inline bool finite(Vec a) { return std::isfinite(a.x) && std::isfinite(a.y) && std::isfinite(a.z); }

    // One line test as the walk sees it.
    struct Probe
    {
        int32_t status = 0;      // kHit, kClear or kFailed
        Vec position{};
        int32_t kind = 0;        // LC_ENTITY_*
        int32_t handle = 0;      // script handle (0 for world geometry)
        uint32_t entity = 0;     // entity address, handed to the next test as its ignored entity (0 for world geometry)
    };

    constexpr int32_t kHit = 1, kClear = 0, kFailed = -1, kInconclusive = -2;
    constexpr int kMaxIgnore = 4;

    struct Query
    {
        Vec start{}, end{};
        uint32_t accept = 0;     // bit (1 << kind) per kind that stops the ray
        int32_t max_passes = 0;
        float step = 0.05f;
        int32_t ignore_count = 0;
        int32_t ignore_kind[kMaxIgnore] = {};
        int32_t ignore_handle[kMaxIgnore] = {};
        uint32_t first_ignore = 0;  // entity address of ignore[0] (0 = none), for the first test
    };

    struct Outcome
    {
        int32_t result = kClear;
        Probe last{};            // the accepted hit (kHit) or the last excluded hit (kInconclusive)
        float distance = 0;      // from the query start to last.position
        int32_t tests = 0;
        int32_t passes = 0;
    };

    inline bool ignored(const Query& q, const Probe& p)
    {
        if (p.handle == 0) { return false; }
        for (int i = 0; i < q.ignore_count && i < kMaxIgnore; i++)
        {
            if (q.ignore_kind[i] == p.kind && q.ignore_handle[i] == p.handle) { return true; }
        }
        return false;
    }

    // test(from, to, ignore_entity) -> Probe. Only called with from strictly before 'to' on the segment.
    template <typename LineTest>
    Outcome walk(const Query& q, LineTest&& test)
    {
        Outcome out;
        Vec delta = q.end - q.start;
        float length = std::sqrt(dot(delta, delta));
        if (!finite(q.start) || !finite(q.end) || !(length > 0) || !std::isfinite(length) || !(q.step > 0))
        {
            out.result = kFailed;
            return out;
        }
        Vec direction = delta * (1.0f / length);
        Vec from = q.start;
        float along_from = 0;
        uint32_t ignore = q.first_ignore;
        for (;;)
        {
            Probe p = test(from, q.end, ignore);
            out.tests++;
            if (p.status == kClear) { out.result = kClear; return out; }
            if (p.status != kHit || !finite(p.position)) { out.result = kFailed; return out; }
            out.last = p;
            Vec offset = p.position - q.start;
            out.distance = std::sqrt(dot(offset, offset));
            bool stops = !ignored(q, p) && p.kind >= 0 && p.kind < 32 && (q.accept & (1u << p.kind)) != 0;
            if (stops) { out.result = kHit; return out; }
            if (out.passes >= q.max_passes) { out.result = kInconclusive; return out; }
            out.passes++;
            // Beyond the hit, and never behind (or level with) the previous start: the walk always advances.
            float along = dot(offset, direction) + q.step;
            if (along < along_from + q.step) { along = along_from + q.step; }
            if (along >= length) { out.result = kClear; return out; }
            along_from = along;
            from = q.start + direction * along;
            ignore = p.entity;
        }
    }
}
