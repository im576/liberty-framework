# Project State

Dashboard only. Update the status table when a component changes. The full diary (every run, changelog, playtest reports,
open questions) is [archive/PROJECT_STATE_history.md](archive/PROJECT_STATE_history.md). Newest implemented work
overrides older docs.

**Agent audio startup (T-057, 2026-10-01):** missing playback output is recovered through the already-installed Sonar
Gaming virtual endpoint before game launches. Healthy physical outputs are preserved. Tool tests 230/0; production x86
disable/recover and game boot verified. Full heartbeat smoke PASS in `20261001-100907-1f6f501`; no log errors.
[Task and evidence](tasks/T-057-test-audio-output.md). This is host tooling, not gameplay acceptance.

## Current phase

**Owner update (2026-10-02): begin the citywide environment overhaul, one integrated candidate across all weather/time conditions with one accountable owner and no new lane dispatch.** Approved Star Junction/station concepts define the target. [T-058](tasks/T-058-citywide-environment-overhaul.md) tracks implementation and evidence; this supersedes the prior art-only hold for that work. Gameplay compatibility and density OFF remain required.

**Owner's current priority (2026-10-01): gameplay feature build and playtest first; visual remaster later.** Finish and
combine weapons/gunplay/loadout, wheel/trunk/storage, gore/effects, functional HUD/menus and supporting feature modules.
Keep gameplay compatibility/performance acceptance; defer environment/art deployment and atmosphere polish from this
milestone. Art direction can proceed independently. The build hold is revoked; B/C/D/R resume on the shared schedule.
Current dispatch: [agent roster](handoffs/sol/AGENT-ROSTER.md). Feature test plan: [FEATURE_PLAYTEST.md](testing/FEATURE_PLAYTEST.md).

- **Phase 1 = Liberty Engine: complete (2026-09-30), except the items under "Known limits".** Native core, SDK, content
  pipeline and hot reload are verified in game. The full regression pass ran on 2026-09-30 and passes ([report](reports/2026-09-30-regression.md)).
- **Next: the first complex mod**, Liberty Vanilla+ Stage 1 ([design](design/STAGE1.md)), built on the engine. Task cards T-040 to T-055 (lanes in [tasks/README.md](tasks/README.md)); art requests ART-001 to ART-012 in the [art queue](art/README.md) (uses the SDK and content compiler, gameplay code stays in `mods/`).
- **Stage 1 lane 0 (T-040) is done, awaiting playtest (2026-09-30):** [reuse audit](reports/2026-09-29-stage1-reuse-audit.md), 12 capture points, measurement scenarios/tooling (	ools/perf), and the [mod-on/mod-off baseline](reports/2026-09-30-stage1-baseline.md). Headline: the current build already exceeds the gunplay/combat/script-cost budgets, the density governor thins the city to 0.55, and combat effects only react to the player's own violence.
- **Stage 1 lane A (T-041 arsenal, T-042 gunplay, T-043 reticles) is done, awaiting playtest (2026-09-30):** six catalog weapons (Glock 17, .44 AutoMag, Street Sweeper, Remington 1100, IMI Uzi, AK-47) on vanilla ids with Liberty profiles behind a catalog gate (`config/weapon-catalog.json` tiers, availability and stats; `stage1Weapons` switch; restricted ids 13/15/16/17 stay vanilla), class targets with a model simulation, range/swap/aim/reticle test commands, per-class reticles (cross, bracket, ring) over the old crosshair. In game: all four lane scenarios pass (run 20260930-134422-33e503f); reticle drawing costs about 0.10 ms per frame (proposal 0.1 ms); delivered first-shot spread sits inside the written cone for pistols, shotguns and the AK. Open: P90/MP5 slot, availability levers without script control (research/WeaponAvailability.md), feel and look sign-off.
- **Then Phase 3: reverse engineering** toward FiveM-level control (authored collision, structure writing, deeper hooks).

The T-040 density result above describes the historical baseline. The owner's current density policy is **OFF** in every
active checkout; that recorded baseline is preserved, not rerun. Current lane review and coordinator corrections are in
[the current orchestrator queue](workflow/ORCHESTRATOR.md). The earlier pause is historical: B/C/R resumed, and C/R are
continuing independent validation. D remains unmerged with runtime HUD issues. Stage 1 integration and acceptance come
before any Phase 3 work. Offline passes do not mark a lane DONE.

## Stage 1 progress

**Latest review (2026-10-01):** B's fresh wheel timing passes (16/16 openings at frame 0), but trunk p95/p99 fail;
the shared follow-up remains held. C stability/cleanup/performance, D HUD acceptance and R material/stall evidence
remain open. Main host startup deadline/evidence fix `9cdc097` passes tooling 256/0. D gets one corrected full
held-baseline batch; other workers continue offline. This update overrides older run/schedule statements below.
[Feature status and planning estimate](reports/2026-10-01-feature-status-and-estimate.md).

- **T-045 wheel / T-046 trunk UI (lane B), merged, NEEDS-PLAYTEST (2026-10-01):** main `f823e45` integrates reviewed
  Lane B `1744416` and cached sprite text, preserving newer main tooling. Full `20261001-092349-e470758` passed all
  wheel/trunk/text assertions and budgets; orchestrator reviewed all 12 wheel/trunk captures. Combined build PASS,
  verifier 424/0/7 not-run, tooling tests 230/0. Physical controller, real save/load, safehouse/gunsmith, B/D coexistence
  and owner judgement remain. The isolated follow-up's earlier integration wheel run crashed in scene
  setup before opening; trunk passed 133/133 steps. That failed evidence is preserved separately from the new run below.
  Assigned B follow-up: `192bd50` imports only the isolated watcher patch; `9dc7948` fixes missed shared-path callbacks
  with differing casing (focused actual-service checks 8/0). Full `20261001-122159-4330603` restored: wheel 92/0,
  trunk/text 133/0 each, all 12 captures reviewed; build PASS, verifier 441/0/5, tools 236/0. Wheel first-draw 2 frames/
  359 ms remains an acceptance gap; config rejection/recovery/restart and owner/combined checks remain unproven.
- **T-047/T-048 (lane C):** active, unmerged. Full `20261001-094219-2db0bf5` includes dismember CRASH and
  firefight/effects-night FAIL; latest full `20261001-103314-0b4f558` also restored with the same failed check categories.
  Budgets remain unchanged; lane work continues separately.
- **T-050 (lane R), merged by the parallel Claude coordinator in `e547d92`:** SDK 1.2's additive material API and mapping
  are on main. Quick coverage confirms wood; direct glass/water rows and object-hit correctness remain unproven.
  Full SDK 1.2/material acceptance remains pending. SDK 1.3 radar/probes and remaining research are still separate.
- **T-049 HUD (lane D):** unmerged; clean hiding and coexistence require further runtime evidence.

Latest main integration `f816a87` build PASS (zero errors); offline verifier 441/0/5 not-run; tooling tests 236/0.
Receipts: `results-local/offline/orchestrator-b-integration/`. B continues wheel timing/config-lifecycle coverage;
C/D/R prepared patches remain unmerged pending their scheduled runtime evidence. Host log reading now preserves history
through rotation gaps and empty unread tails, and lock self-tests use isolated holder notes. Details and failed/full
runtime receipts: [orchestrator integration review](reports/2026-10-01-orchestrator-integration.md).

- **T-044 physical loadout (lane B), NEEDS-PLAYTEST (2026-09-30):** 1 sidearm + 2 long guns (SMGs slung), ammo caps, holster props react in the event frame, outfit classes; in game: 100 vehicle enter/exit cycles with 0 orphaned props, 50 death cycles with 0 lost owned weapons (a death with no known safehouse now goes to an unassigned stash), save/load state identical. Card: [T-044](tasks/T-044-stage1-physical-weapons.md).

## Cleanup and fixes in progress

**Done 2026-09-29/30 (Claude):**
- Repo cleanup: Phase 1/2 docs, task cards and one-shot scripts archived (`docs/archive`, `tools/archive`); side branches tagged `archive/*` and removed.
- Stalls: both were the blocking `LOAD_SCENE` inside teleport (4-18 s measured). The watchdog now tolerates it (`teleportBlockingWindowMilliseconds`), the time is logged (`player_teleport load_scene_ms`), and the autopilot allows 90 s for `goto`.
- Collision without authored bounds: **collision proxies** (`IProps.SetVisible`, `collisionProxies` in `config/world/objects.json`). In game, 14 of 24 tested vanilla props stop the engine ray (`proxy-probe`), and the test wall is solid to the ray through two hidden `corpbarrier1` props. Player and vehicle collision are inferred from the shared physics bounds, not measured.
- Structure writer bisect (in game, 3 runs): one geometry on a weapon template and on a map template spawn fine; two geometries crash at spawn whether they use two textures or one shared. Textures and dictionaries are cleared; the fault is in writing a second geometry (T-031).

## Known limits (carried into the mod phase)

- **One material per model.** Use a texture atlas. Multi-geometry structure drawables crash the game; the compiler warns. Repro: scenario `structure-bisect`, assets `lf_st_2tex`, `lf_fx_multimat`.
- **No authored collision, no LOD slots 1-3** (Phase 3). Use collision proxies; LOD via separate models or IDE draw distance.
- **Manual gameplay checks (18) not run**: do them as one owner sitting when convenient; the automated regression passed ([report](reports/2026-09-30-regression.md)).
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
