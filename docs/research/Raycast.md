# Raycast / line of sight (GTAIV.exe 1.2.0.59)

GTA IV has no script native for a general ray test. `HAS_CHAR_SPOTTED_CHAR` is perception (it has memory and
hearing), `GET_GROUND_Z_FOR_3D_COORD` only answers "ground below". This note records how the game's own physics line
test was found, what is proven in game, and how the engine uses it ([ADR-0008](../architecture/decisions/ADR-0008-engine-raycast.md)).
Confidence tags as in [README.md](README.md).

## 1. The line test function

**[SOURCE: disassembly, pinned by tools/verify]** `0xA536B0` (preferred base), found from
`GET_GROUND_Z_FOR_3D_COORD` → `0xA523F0` → `0xA54510` → `0x738880` (the physics world query). Several game systems call
`0xA536B0` directly, so it is the game's general "test a line against the world" entry.

```
cdecl bool TestLine(Vec3* start, Vec3* end, CEntity* ignore, Result* out, uint includeFlags, int mode, int unused)
```

| Item | Finding |
|---|---|
| Vectors | 16-byte `Vec3` (x, y, z, pad); the prologue aligns its own stack (`55 8B EC 83 E4 F0`). |
| `ignore` | One entity whose physics instance is skipped. The function picks the instance by entity type, `([e+0x28] >> 6) & 0xF`: type 3 (ped) and type 2 (vehicle) have their own branches, every other type uses `[e+0x38]`. The ped branch (`F6 C2 40 74 0A 8B 81 B4 07 00 00 85 C0 75 1F 8B 81 B0 07 00 00`) reads `[ped+0x7B4]` when bit 0x40 of `dl` is set and that pointer is non-null, else `[ped+0x7B0]`. |
| `out` | 0x60-byte result. Every caller zeroes it and sets `+0x4C` to `0xFFFF` first. `+0x00` = hit physics instance, `+0x10` = hit position, `+0x20` = surface normal. |
| `includeFlags` | Archetype include bits handed to the world query (§3). |
| `mode` | Passed through to the world query. Callers pass `1`, `-1`, `0x40` and `8`. Meaning unknown. |
| Last argument | Every caller pushes `4`; the function does not read it. The engine pushes 4 as well. |
| Return | `bool` in `al` only. The upper bytes of `eax` are not defined, so the core reads one byte and treats any non-zero value as a hit. |
| World query | `mov ecx,[0x12B9C78]` (the physics world), then `thiscall 0x738880(&segment, [ebp+14h], eax, edx, -1, 7, [ebp+1Ch], 0)` at function+0x96. |

The resolver (`GameAddresses.ResolveLineTest`) finds the function by its entity-type switch (one match in the exe),
then checks the prologue and the world-query call shape before accepting it. The offline verifier pins the function
(`0xA536B0`) and the world global (`0x12B9C78`).

## 2. What is verified in game

**[VERIFIED 2026-09-26, raycast-spike, owner's machine]**

| Ray | Result |
|---|---|
| Down 10 m from the player, all include bits, mode 1 | Hit world geometry at ground height, normal (0, 0, 1). Include bit 0x2 alone also hits it. |
| Forward into a spawned ped | Hit, position and normal on the ped. The ped entity is at `[result+0x00] + 0x0C` (the physics instance's owner). Include bits 0x20 and 0x40 alone also hit it. |
| Forward at a spawned car, 1.5 m above the ground | Missed: the ray passed over the admiral's roof. Retest at door height (the spike scenario now casts 0.3 m below the player position, about 0.7 m above the ground). |
| Contained faults | None in the run. |

**[HYPOTHESIS]** 0x20 is the ped's animated capsule and 0x40 its ragdoll bounds, and `dl` in the ped branch holds the
include bits (so the ragdoll instance is the one ignored when 0x40 is requested). Not needed by the engine (it passes
all bits and ignores by entity).

## 3. Include bits

| Bit | What hit it | Status |
|---|---|---|
| 0x2 | ground (map collision) | [VERIFIED] |
| 0x20, 0x40 | a ped | [VERIFIED] |
| others | not yet seen | open: `raybits` runs against a vehicle at door height in `raycast-spike` and `raycast` |
| all (`0xFFFFFFFF`) | ground and peds | [VERIFIED]; what the engine passes |

The engine does **not** depend on this map. It passes every bit and decides what a hit is from the entity pools
(§4), so filtering stays correct for bits nobody has mapped yet. The map only matters for a later optimisation
(asking the game for fewer kinds up front).

## 4. From a hit to an entity

**[VERIFIED for peds]** `entity = [result+0x00 + 0x0C]`. The core looks that address up in the ped, vehicle and object
pools (`RagePool::handle_of`: exact slot start, live slot) and gets the script handle. Anything not in a pool is world
geometry (map collision, buildings). Vehicles and objects use the same link **[HYPOTHESIS until raycast/raycast-spike
report a vehicle hit]**; if the link differed for them, a vehicle hit would show as `kind=World` and the
`raycast-vehicle` self-test check fails, which is exactly what the test is for. The research flag (`lf raydebug`)
still scans the whole result and the first 0x100 bytes behind word 0 and reports where it found the entity (`link`).

## 5. Engine design (ABI 5)

- **Core:** `lc_raycast` (`native/LibertyCore/src/core.cpp`) calls the function under the core's SEH guard, on the engine
  tick only. Any fault switches raycasts off for the session (`engine_raycast_disabled`, fault reported as
  `in=raycast line test`).
- **Filtering by walking:** the game returns only the first hit and ignores one entity. The core's walk
  (`ray_walk.h`, unit-tested in `tests/ray_walk_test.cpp`) passes through hits the caller does not want: the next test
  starts `raycastPassStepMeters` beyond the hit with that entity ignored by the game. Up to `raycastMaxPasses`
  passes; beyond that the query is `Inconclusive`. Up to four caller-ignored entities (the first goes to the game).
- **SDK 1.1:** `Query.Raycast(from, to, RayMask, RayIgnore)` → `RayHit`, `Query.HasLineOfSight(from, to, blockers,
  ignore)`, `Query.HasLineOfSight(viewer, target)` (head to head/chest/pelvis; world, vehicles and objects block,
  peds do not; both peds' vehicles are ignored). See [docs/sdk/README.md](../sdk/README.md#raycast-and-line-of-sight).
- **Commands:** `lf ray`, `lf raystats`; research `lf raydebug` (raw first hit, whole result) and `lf raybits`
  (bits 0–31).

## 6. Open questions

| # | Question | How it gets answered |
|---|---|---|
| R1 | Is the vehicle entity at the same link (`[instance+0x0C]`)? | `raycast` scenario (`ray forward 12 -0.3`, `rayto car`) and self-test `raycast-vehicle` |
| R2 | Which include bits hit vehicles and objects? | `raybits forward 12 1 -0.3` in both scenarios; objects need a collidable prop |
| R3 | What does `mode` select (1, -1, 0x40, 8)? | `raydebug ... -1` rows in `raycast-spike`; not needed while mode 1 works |
| R4 | Do rays see collision that is not streamed in (far from the player)? | Expected no (the game only has nearby collision loaded); a long ray over the map would show it |
| R5 | Result words beyond +0x2C (material, component, fraction?) | `raydebug` logs all 24 words for comparison |
| R6 | Cost per line test | `lf costs` (`engine.raycast`) after the `raycast` scenario |
