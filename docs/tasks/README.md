# Task queue for agents

Development runs in cloud sessions; every check that needs the game is queued in `tests/local/checks.json` and run by `tools/verify-local.ps1` when the PC is available ([workflow](../workflow/CLOUD_LOCAL_LOOP.md), [plan](../testing/LOCAL_VERIFICATION_PLAN.md), [next sessions](../workflow/NEXT_SESSIONS.md)). Current state: [PROJECT_STATE.md](../PROJECT_STATE.md).

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Only the project owner marks a card `DONE`. Track each task's scope and evidence separately; batch compatible offline work into one game session.

## Active cards

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