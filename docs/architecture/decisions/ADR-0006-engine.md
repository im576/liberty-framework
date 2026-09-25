# ADR-0006: Liberty Engine (native core, one host, modules, events)

**Status:** accepted (2026-09-25, owner request: "make our engine first", "use native C++ as well")

## Context

Liberty Framework grew as eight ScriptHookDotNet (SHDN) `Script` classes. Each one runs on its own SHDN thread, polls the game on its own, and calls natives through SHDN's cross-thread transport, which costs 32–180 µs per call. Features duplicated reads, and they collided through shared game state:

- **Holster freeze:** the holster script called an engine function from its own thread, and the game froze after load on 2026-09-25.
- **Performance cost:** CombatEffects health-polls every ped, and Gunplay's camera reads cost 10 ms per tick.

The owner wants a base that makes large new mechanics, UIs, animations, NPCs, weapons, models, textures and audio easy to add. It must also be fast on a Ryzen 3 2300X and RX 570.

## Decision

Four layers. Each layer only calls the one below it.

1. **LibertyCore (native C++20, 32-bit DLL, `native/LibertyCore`):**
   - A direct native invoker: script-native handlers are called through the CE ABI (`handler(ctx)`) in about 0.02 µs, against 32–180 µs through SHDN.
   - Reads the rage entity pools.
   - Builds one **world snapshot** per frame: player, nearby peds with position, health, model and state, and world flags.
   - Turns frame-to-frame changes into a compact **event stream** (ped appeared or removed, damaged with bone, died; player shot, weapon change, vehicle enter and exit).
   - Guards its own safety: a frame counter proves the game thread is parked during every call.
   - Profiles itself.
   - C ABI only (`liberty_core.h`), statically linked, built with clang (llvm-mingw).
   - Later hooks (exact damage events, E-4; draw and animation hooks) live here too.
2. **Engine host (C#, `Engine/`):**
   - The **only** SHDN `Script`. It ticks every frame, in a fixed order:
     1. core frame
     2. snapshot verification and fallback
     3. event dispatch
     4. scheduled coroutines
     5. module ticks (each at its own interval)
   - It also has one draw pass. Draw handlers read cached state only and never call natives.
   - All gameplay code therefore runs on one thread, while the game is parked.
3. **Services (C#), which every module gets through `EngineContext`:**
   - `World` (the snapshot and entity queries)
   - `Natives` (direct when verified, SHDN otherwise)
   - `Events` (typed bus)
   - `Scheduler` (coroutines for sequences: choreography, scenarios)
   - `Entities` (spawned props and peds with ownership, cleanup, and orphan journals across script reloads)
   - `Commands` (console, plus the file channel the autopilot uses)
   - `Ui` (prompts, help boxes, layered drawing)
   - `State` (per-module save data)
   - `Profiler`
4. **Modules (C#):**
   - Each mechanic is a `Module` with an id, a config file, an enable switch, an interval, and optional `OnEvent`, `OnTick` and `OnDraw`.
   - An exception disables only that module (its `OnStop` runs), logs why, and leaves the game running.
   - Existing controllers become modules unchanged in behaviour first. Then they move to events and the snapshot.
   - Content (models, textures, audio, animations) is added through content packs built by `tools/` and registered in the asset catalog.

## Why these choices

- **Game thread.** Script systems that run gameplay code on (or synchronized with) the game thread stay free of races: ScriptHookV fibers, RAGE Plugin Hook `GameFiber`, and SHVDN v3 moving scripts onto the main thread. One host tick gives us the same property on SHDN. The engine-thread probe verified that the game is parked during a tick.
- **Why C++.** Native code is used where it pays: tight per-frame loops over pools and natives, zero garbage collection, and future engine hooks. Gameplay stays in C# for iteration speed and moddability.
- **Snapshot and events.** A snapshot plus diff events turns N features × M peds × K natives into one pass per frame. Mechanics subscribe instead of polling.
- **Fail closed.** Every direct native is verified against SHDN on the player at startup. The frame-counter guard and a pure-SHDN fallback snapshot keep the game running if the core cannot load or verify.

## Consequences

- SHDN instantiates only `Engine.EngineHost`. Feature classes no longer derive from `GTA.Script`.
- The build uses Roslyn (C# 7.3) and clang. `tools/get-toolchains.ps1` fetches both, outside the repository.
- `LibertyCore.dll` is deployed to `scripts/LibertyFramework/bin/`.
- The autopilot (scenario runner plus external launcher) is built on the same events and commands.
