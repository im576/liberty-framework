# Task queue for agents

Current owner-requested repo/tooling review: [T-058](T-058-repository-review.md). Current scheduling and the installed
preview are in [ORCHESTRATOR.md](../workflow/ORCHESTRATOR.md); older cloud/lane instructions below are context.

Development runs in cloud sessions; every check that needs the game is queued in `tests/local/checks.json` and run by `tools/verify-local.ps1` when the PC is available ([workflow](../workflow/CLOUD_LOCAL_LOOP.md), [plan](../testing/LOCAL_VERIFICATION_PLAN.md), [next sessions](../workflow/NEXT_SESSIONS.md)). Current state: [PROJECT_STATE.md](../PROJECT_STATE.md).

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Only the project owner marks a card `DONE`. Track each task's scope and evidence separately; batch compatible offline work into one game session.

## Stage 1 — Liberty Vanilla+ (current work)

Design and acceptance criteria: [STAGE1.md](../design/STAGE1.md). Slice A (P0) plus the parallel research track. Slices
B and C get their cards after Slice A's audit (T-040) and the relevant spikes.

Owner priority update 2026-10-01: all existing/in-progress gameplay and functional UI modules come first, followed by
an integrated feature test build and owner playtest. The environment remaster and atmosphere/art polish are later;
art direction may continue in parallel. Slice C's functional HUD/menu work is not deferred with its final styling.
Builds/tests are resumed, centrally scheduled; see [feature playtest plan](../testing/FEATURE_PLAYTEST.md).

**Lanes.** Tasks in one lane touch the same files, so a lane runs in order in one session or worktree at a time.
Different lanes can run in parallel sessions; the game is shared, and `tools/verify-local.ps1` makes parallel runs
wait their turn. **Dependency rule for Stage 1:** a dependency is satisfied when it is `NEEDS-PLAYTEST` with its own
in-game checks passing (or `DONE`); owner sign-offs can come later.

| Lane | Order | Area |
|---|---|---|
| 0 | [T-040](T-040-stage1-foundation.md) | Reuse audit, capture points, measurement baseline (do first) |
| A | [T-041](T-041-stage1-weapons.md) → [T-042](T-042-stage1-gunplay.md) → [T-043](T-043-stage1-reticles.md) | Arsenal data, gunplay tuning and shoulder swap, weapon-specific reticles |
| B | [T-044](T-044-stage1-physical-weapons.md) → [T-045](T-045-stage1-weapon-wheel.md) → [T-046](T-046-stage1-trunk-ui.md) | 2+1 physical loadout, weapon wheel, trunk UI |
| C | [T-047](T-047-stage1-gore.md) → [T-048](T-048-stage1-combat-effects.md) | Harsh gore, contextual combat effects |
| D | [T-049](T-049-stage1-hud.md) | Basic Liberty HUD |
| R | [T-050](T-050-r2-hit-material.md), [T-051](T-051-r3-blood-decals.md), [T-052](T-052-r4-screen-effect-cost.md), [T-053](T-053-r1-weapon-audio.md), [T-054](T-054-r5-custom-radar.md), [T-055](T-055-r6-frontend-hooks.md) | Research spikes R2, R3, R4, R1, R5, R6 (in that priority) |

All Stage 1 cards start `READY`; T-040 is `NEEDS-PLAYTEST` (2026-09-30), so lanes A to D and R may start.

## Active cards (engine phase)

| ID | Task | Status |
|---|---|---|
| T-021 | [Visible weapons (holsters)](T-021-holsters.md) | NEEDS-PLAYTEST |
| T-024 | [Phase 2 foundations](T-024-phase2-foundations.md) | NEEDS-PLAYTEST |
| T-026 | [Performance and visual baseline](T-026-performance-visual-baseline.md) | NEEDS-PLAYTEST |
| T-027 | [Engine raycast and line of sight (SDK 1.1)](T-027-engine-raycast.md) | NEEDS-PLAYTEST |
| T-028 | [Content compiler v2: native texture dictionaries](T-028-lcc-native-textures.md) | NEEDS-PLAYTEST |
| T-029 | [Cloud development loop and local verification](T-029-cloud-local-loop.md) | NEEDS-PLAYTEST |
| T-030 | [Content compiler authoring side](T-030-lcc-authoring-side.md) | NEEDS-PLAYTEST |
| T-031 | [Structure writer: several geometries, shaders and LODs](T-031-structure-writer.md) | NEEDS-PLAYTEST |
| T-032 | [Collision: layout research and borrowed collision](T-032-collision.md) | NEEDS-PLAYTEST |
| T-033 | [Static world objects: build, register, place](T-033-world-objects.md) | NEEDS-PLAYTEST |

## Phase 1/2 gameplay cards kept in place

These stay here because `tests/local/checks.json` references their paths (the check queue validates them); they are
finished history, all NEEDS-PLAYTEST or owner-verified: [T-007](T-007-weapon-slots.md), [T-011](T-011-gold-finishes.md),
[T-013](T-013-debug-hit-info.md), [T-014](T-014-aim-profiles.md), [T-015](T-015-shoulder-swap.md),
[T-016](T-016-switch-while-aiming.md), [T-017](T-017-feel-shake-fov.md), [T-020](T-020-arsenal-core.md),
[T-022](T-022-combat-effects.md), [T-023](T-023-lvs-body-variants.md), [T-025](T-025-physical-weapons.md).

All other finished cards (T-000..T-003, T-008..T-010) are in [../archive/tasks/](../archive/tasks/).
