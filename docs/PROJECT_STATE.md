# Project State

Dashboard only. Update the status table when a component changes. The full diary (every run, changelog, playtest reports,
open questions) is [archive/PROJECT_STATE_history.md](archive/PROJECT_STATE_history.md). Newest implemented work
overrides older docs.

## Current phase

- **Phase 1 = Liberty Engine: complete (2026-09-30), except the items under "Known limits".** Native core, SDK, content
  pipeline and hot reload are verified in game. The full regression pass is queued as the first task of the mod phase.
- **Next: the first complex mod** built on the engine (uses the SDK and content compiler, gameplay code stays in `mods/`).
- **Then Phase 3: reverse engineering** toward FiveM-level control (authored collision, structure writing, deeper hooks).

## Cleanup and fixes in progress

**Done 2026-09-29/30 (Claude):**
- Repo cleanup: Phase 1/2 docs, task cards and one-shot scripts archived (`docs/archive`, `tools/archive`); side branches tagged `archive/*` and removed.
- Stalls: both were the blocking `LOAD_SCENE` inside teleport (4-18 s measured). The watchdog now tolerates it (`teleportBlockingWindowMilliseconds`), the time is logged (`player_teleport load_scene_ms`), and the autopilot allows 90 s for `goto`.
- Collision without authored bounds: **collision proxies** (`IProps.SetVisible`, `collisionProxies` in `config/world/objects.json`). In game, 14 of 24 tested vanilla props stop the engine ray (`proxy-probe`), and the test wall is solid to the ray through two hidden `corpbarrier1` props. Player and vehicle collision are inferred from the shared physics bounds, not measured.
- Structure writer bisect (in game, 3 runs): one geometry on a weapon template and on a map template spawn fine; two geometries crash at spawn whether they use two textures or one shared. Textures and dictionaries are cleared; the fault is in writing a second geometry (T-031).

## Known limits (carried into the mod phase)

- **One material per model.** Use a texture atlas. Multi-geometry structure drawables crash the game; the compiler warns. Repro: scenario `structure-bisect`, assets `lf_st_2tex`, `lf_fx_multimat`.
- **No authored collision, no LOD slots 1-3** (Phase 3). Use collision proxies; LOD via separate models or IDE draw distance.
- **Full regression pass not yet run on this build.** First task of the mod phase: `./tools/verify-local.ps1 -NoManual` (about 1 hour).
- Startup crash before the ASI loads (Rockstar `MTLX.DLL`, about 1 launch in 3): environmental; the autopilot relaunches.
- Episodes (TLAD/TBoGT) are not covered by design.

## Engine components

| Component | Status | Notes |
|---|---|---|
| Native core / hooks (`native/LibertyCore`) | Working | Hook manager, exact ped damage, pattern-resolved engine memory ([ADR-0006](architecture/decisions/ADR-0006-engine.md), [ADR-0007](architecture/decisions/ADR-0007-core-hooks-and-exact-damage.md)) |
| SDK 1.1 services (`sdk/`) | Working | Modules, events, raycast, services; [docs/sdk](sdk/README.md), roadmap in [sdk/ROADMAP.md](sdk/ROADMAP.md) |
| Hot reload | Working | Passed 24/24 in the last focused run |
| Autopilot (`tools/autopilot`) | Working | Claude-driven scenarios launch the game and capture evidence; SDK selftest 49/49 |
| Content compiler v1 | Working | Single-material drawables plus native textures ([docs/content](content/README.md)) |
| Structure writer | Single geometry works in game; multi-geometry crashes | Bisected 2026-09-29, see Known limits |
| Collision | Proxy props work; authored deferred | Authored collision moves to Phase 3. Research: WBD hash-to-target table decoded, shapes not decoded ([research/Collision.md](research/Collision.md)) |
| World objects (`mods/Liberty.World`) | Verified in game | Placement plus collision proxies (ray hit on the test wall) |
| LOD | Decided | All 22,812 sampled game drawables use only LOD slot 0, so multi-LOD-in-drawable is dropped |
| Gameplay reference modules | Owner-verified (Phase 1) | Gunplay, Arsenal, holsters/slings, trunk, gore; kept as reference mods, not the focus |
| Performance | Stalls fixed (declared), engine cost low | `engine.frame` about 2.5 ms; teleport blocks 4-18 s by design of `LOAD_SCENE` ([research/Performance.md](research/Performance.md)) |

## Key decisions (`docs/architecture/decisions/`)

- [ADR-0001](architecture/decisions/ADR-0001-runtime.md): CE 1.2.0.59 + FusionFix + Tomasak ScriptHookDotNet 1.7.1.8, x86 .NET Framework 4.0 C#. No downgrade.
- [ADR-0002](architecture/decisions/ADR-0002-weapon-slots.md): FusionFix ExtendedLimits gives the custom weapon IDs 58/59/60.
- [ADR-0003](architecture/decisions/ADR-0003-config.md): human-readable JSON config with live polling and last-valid retention.
- [ADR-0004](architecture/decisions/ADR-0004-engine-memory.md): engine data is found by native-hash/instruction-shape resolvers, validated at runtime, never hardcoded addresses.
- [ADR-0005](architecture/decisions/ADR-0005-skeleton-hook.md): owner-approved after-call hooks on the fragInst skeleton rebuilds for dismemberment; validated, gated, restored on unload.
- [ADR-0006](architecture/decisions/ADR-0006-engine.md): Liberty Engine, native core + one SHDN host + `[Module]` classes + events.
- [ADR-0007](architecture/decisions/ADR-0007-core-hooks-and-exact-damage.md): LibertyCore hook manager and exact ped damage.
- [ADR-0008](architecture/decisions/ADR-0008-engine-raycast.md): raycast through the game's own physics line test, pattern-resolved, engine tick only, read-only.

## Where things are

- Task cards: [tasks/README.md](tasks/README.md) (active); finished cards in [archive/tasks/](archive/tasks/).
- Architecture: [architecture/OVERVIEW.md](architecture/OVERVIEW.md), [architecture/ENGINE.md](architecture/ENGINE.md), [architecture/CONFIG_SCHEMA.md](architecture/CONFIG_SCHEMA.md).
- Cloud/local loop and the PC check queue: [workflow/CLOUD_LOCAL_LOOP.md](workflow/CLOUD_LOCAL_LOOP.md), `tests/local/checks.json`, [testing/LOCAL_VERIFICATION_PLAN.md](testing/LOCAL_VERIFICATION_PLAN.md).
- Research and game API: [research/](research/README.md), [game-api/NATIVES.md](game-api/NATIVES.md), [game-api/MEMORY.md](game-api/MEMORY.md).
- Reports: `docs/reports/`; tool index: [../tools/README.md](../tools/README.md).
- History and the original spec: [docs/archive/](archive/) (`HANDOFF.md`, `PHASE2_PLAN.md`, `PHASE2_PATCHNOTES.md`, agent reports).
