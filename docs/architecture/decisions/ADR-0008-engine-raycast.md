# ADR-0008: Engine raycast through the game's own line test

Status: **proposed and implemented** (2026-09-26); in-game evidence pending for vehicles (T-027). Ground and ped hits
were verified by the research spike (commit 13ff3c2).

## Context

The SDK's `WorldQuery` layer (ROADMAP M3) had snapshot queries and the game's perception check, but no geometric line
of sight. GTA IV has no script native for it. The game tests lines against its physics world with one function
(`0xA536B0`, [Raycast.md](../../research/Raycast.md)). ADR-0004 covers reading and writing data found by patterns;
ADR-0007 covers detours. Neither covers **calling** an internal game function directly.

## Decision

The engine core calls the game's line test directly, under these rules:

1. **Resolved, never hardcoded.** `GameAddresses.ResolveLineTest` finds the function by its entity-type switch (one
   match), then checks the prologue and the physics-world call shape. The offline verifier pins the result against
   the disassembly (`0xA536B0`, world `0x12B9C78`). A resolver failure leaves raycasts unavailable; nothing else changes.
2. **Called exactly as the game calls it.** Aligned 16-byte vectors, the result zeroed with `+0x4C = 0xFFFF`, the
   trailing `4` every caller passes, the return read from `al` only.
3. **Only on the engine tick.** The game thread is parked while the engine runs (T-026 thread probe). The engine
   refuses raycasts from the draw pass and from other threads (`Unavailable`, logged once).
4. **Contained and switched off on the first fault.** Every call runs under the core's SEH guard (`safe_call.cpp`).
   A fault disables raycasts for the session and is logged (`engine_raycast_disabled`, fault `in=raycast line test`).
5. **Read-only.** The engine never writes game memory for a raycast. It reads the result block and one pointer
   (`[instance+0x0C]`), both through the guarded reader, and maps entities through the verified pools.
6. **Configurable off.** `engine.json` `raycastEnabled` (default true) skips installation entirely.

Filtering by kind is done by the engine, not by include bits (only three bits are mapped): the core walks the
segment and passes through hits the caller did not ask for (`raycastMaxPasses`, `raycastPassStepMeters`). The walk is
pure code with a native unit test.

## Consequences

- Mods get `Query.Raycast`, `Query.HasLineOfSight` (points and peds) in SDK 1.1 without natives or capabilities.
- A query costs one line test per excluded hit it passes through; module budgets account for it (the time is spent
  inside the calling module's update) and `lf costs` shows `engine.raycast`.
- Only collision the game has streamed in is tested. That is the game's own limitation and the same for its AI.
- Vehicle and object hits reuse the ped link until the `raycast` scenario proves or disproves it; a wrong link shows
  as `kind=World` and fails the self-test, never as a wrong entity.
- The same pattern (resolve, verify offline, call on the tick under SEH, disable on fault) is the template for
  later direct calls into the game.
