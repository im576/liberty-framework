# Liberty SDK roadmap and IV-SDK parity

Liberty rebuilds the capabilities people use IV-SDK / IV-SDK .NET for, independently and for the CE 1.2.0.59 game.
Those projects are GPL-3.0 and are not copied (AGENTS.md rule 7). This table tracks capability, not API shape.

## Capability parity

| Capability | Liberty status | Where |
|---|---|---|
| **Scripting and modules** | | |
| Script/plugin loading | **Done**: modules from the engine and `mods\*.dll` (manifest, dependencies, SDK version, capabilities) | `Engine/LibertyEngine.cs` |
| Hot reload | **Done** (dev): `lf reload <module>` swaps a mod assembly in a running game, `lf restart <module>`, and a file watcher (`hotreload`); old assemblies stay loaded, with a cap | `LibertyEngine.ReloadAssembly`, `ModuleReloader`, docs/sdk/README.md §7 |
| **Game access** | | |
| Native invocation | **Done**: SDK services; `INatives` (capability); 30 direct natives in the C++ core, each verified against SHDN at startup | `Engine/Services`, `native/LibertyCore` |
| Native interception | Planned on the hook manager (entry detours are in place) | M2+ |
| Entity pools | **Done** (read): ped, vehicle, object pools resolved by signature; per-frame snapshot, occupancy stats | core `rage_pool`, `GameAddresses.ResolveEntityPools` |
| Signatures / pattern scan | **Done**: `CodeScanner`, `IMemory.FindPattern/FindNative`; every address resolved by pattern and checked offline (verifier) | `Core/Memory`, `tools/verify` |
| Memory read/write | **Done**: `IMemory` (capability), patches recorded and restored on stop/unload | `MemoryService` |
| Hooks / detours | **Done** (core): ADR-0007 hook manager: entry detours with verified displaced bytes, owned, listed (lf hooks), restored on unload; ADR-0005 skeleton call-site hooks still separate | hooks.cpp |
| Exception-safe game calls | **Done**: SEH-contained native calls and raw reads in the core; faulting native disabled and logged | `safe_call.cpp` |
| Version / episode detection | **Done**: game version, episode (IV/TLAD/TBoGT) | `MemoryService.GameVersion`, `ILiberty.Episode` |
| **Tooling** | | |
| Debug UI | **Done**: DevTools (engine module), Liberty.Ui list/radial menus, canvas | DevTools, `Engine/Ui` |
| Inspector | **Done**: `lf inspector` on-screen overlay (engine numbers, one row per module), plus the `lf engine/modules/perf/pools/owned/costs/natives/hooks` commands | `Engine/Ui/InspectorOverlay.cs` |
| Tracing / profiling | **Done** (basic): per-module cost EMA/max, named cost samples, frame p95, memory/address space, governor | `PerfService`, `Governor` |
| Crash capture | **Done**: minidump + phase (which module) on crash; stall watchdog with minidump | `crash.cpp`, `Watchdog` |
| Remote control | **Done**: file command channel (autopilot inbox/outbox), console | `CommandRegistry` |
| Automated testing | **Done**: autopilot launches the game, runs scenarios, screenshots, SDK self-test | `tools/autopilot`, `mods/Liberty.Autopilot` |

## Milestones

| | Scope | Exit criteria |
|---|---|---|
| **M1 SDK 1.0 core** | SDK, services, core ABI, crash safety, Arsenal wheel/trunk on Liberty.Ui/choreography, docs | **Done 2026-09-25**: suite 9/11 → all targeted re-runs pass (trunk wheel + choreography in game, self-test) |
| **M2 Hooks + exact damage** | ADR-0007 core hook manager; damage hook: attacker, victim, weapon, damage, armour, bone, hit position/direction, type, kill | **Done 2026-09-25**: autopilot `exact-damage` passes (exact bullet hits with hit points, falls typed Fall, exact kills) |
| **M3 WorldQuery** | snapshot queries (radius/cone of peds and vehicles), ground/water, perception and visibility; later engine raycast/line of sight | layer 1 done (`Liberty.Query`, self-test); raycast open |
| **M4 Content pipeline 1** | glTF → IR → validators → WDR/WTD (single page) → IMG; read-back verification; preview renders; Blender add-on v0; autopilot asset scenario | **Done 2026-09-25** (`docs/content/README.md`): the glTF test crate compiles, reads back identical, spawns and renders correctly in game (`asset-review`) |
| **M5 Developer loop** | per-module hot reload (dev), visual inspector | **Done 2026-09-26**: `hot-reload` scenario (the autopilot reloads itself and its self-test passes 38/38), `inspector-review` |
| **M6 Content pipeline 2+** | skinned meshes, LODs, collision bounds, multi-page resources, WDD/WFT, animations | per-format scenarios |

## SDK 1.0 freeze criteria

1. Every service has at least one in-game test (self-test or scenario) that passes on the installed build.
2. The damage event is exact (M2). Fields added after the freeze are appended, never re-ordered or removed.
3. No SDK type references ScriptHookDotNet or engine internals. Checked by building `mods/*` against `Liberty.Sdk.dll` only.
4. Episode coverage: self-test passes in TLAD and TBoGT.
5. Docs: `docs/sdk/README.md` covers every service and event.
