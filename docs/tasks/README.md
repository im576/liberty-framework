# Task queue for agents

**Owner remote (from 2026-09-26):** development continues in cloud sessions; every check that needs the game is queued in `tests/local/checks.json` and run by `tools/verify-local.ps1` when the PC is available ([workflow](../workflow/CLOUD_LOCAL_LOOP.md), [plan](../testing/LOCAL_VERIFICATION_PLAN.md), [next sessions](../workflow/NEXT_SESSIONS.md)).

**Current assignment (before the owner went remote):** Phase 1 (T-010) plus the Arsenal run (T-011, T-013, T-014, T-016, T-017, T-020, T-021) is installed in the game and awaits one owner playtest ([checklist](../testing/PHASE1_PLAYTEST.md)). T-000–T-003 are DONE; T-007's carbine/shotgun IDs 59/60 were confirmed by the owner (logs show both selected). The project owner alone marks a tested card `DONE`.

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Track each task's scope and evidence separately, and update [PROJECT_STATE.md](../PROJECT_STATE.md).

Batch compatible offline work and combine human checks into one gameplay session where practical. Keep separate task statuses and record unverified assumptions rather than treating one task's success as evidence for another.

| ID | Task | Depends on | Status |
|---|---|---|---|
| T-000 | [Record game baseline](T-000-baseline.md) | none | DONE |
| T-001 | [Runtime hello world](T-001-runtime-spike.md) | T-000 | DONE |
| T-002 | [Config and logging skeleton](T-002-config-logging.md) | T-001 | DONE |
| T-003 | [DevTools minimum menu](T-003-devtools-menu.md) | T-002 | DONE |
| T-007 | [Separate gold weapon slot spike](T-007-weapon-slots.md) | T-001 | NEEDS-PLAYTEST (owner reported IDs 58/59/60 working; owner to mark DONE) |
| T-008 | [Per-weapon aim and HUD spike](T-008-aim-hud.md) | T-007 | SUPERSEDED by T-010 (owner accepted universal free aim) |
| T-009 | [Camera recoil API spike](T-009-camera-recoil.md) | T-001, T-007 | SUPERSEDED by T-010 |
| T-010 | [Phase 1 integrated gunplay build](T-010-phase1-gunplay.md) | T-003, T-007 | NEEDS-PLAYTEST |
| T-011 | [Gold carbine and shotgun](T-011-gold-finishes.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-013 | [Debug overlay hit info](T-013-debug-hit-info.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-014 | [Aim profiles](T-014-aim-profiles.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-015 | [Shoulder swap](T-015-shoulder-swap.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-016 | [Switch weapons while aiming](T-016-switch-while-aiming.md) | T-010, T-020 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-017 | [Feel: shake and aiming FOV](T-017-feel-shake-fov.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-020 | [Liberty Arsenal core](T-020-arsenal-core.md) | T-010 | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-021 | [Visible weapons (holsters)](T-021-holsters.md) | T-020 contracts | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-024 | [Phase 2 foundations](T-024-phase2-foundations.md) | T-010, T-020, T-021 | NEEDS-PLAYTEST |
| T-022 | [Combat effects + dismemberment](T-022-combat-effects.md) | T-010 | NEEDS-PLAYTEST (dismemberment installed 2026-09-24) |
| T-023 | [LVS body-part labels](T-023-lvs-body-variants.md) | LVS workshop | NEEDS-PLAYTEST (installed 2026-09-24) |
| T-025 | [Physical weapons and gunsmith](T-025-physical-weapons.md) | T-020 | NEEDS-PLAYTEST |
| T-026 | [Performance and visual baseline](T-026-performance-visual-baseline.md) | current installed stack | NEEDS-PLAYTEST (capture setup ready; owner frame traces pending) |
| T-027 | [Engine raycast and line of sight (SDK 1.1)](T-027-engine-raycast.md) | ADR-0006 core, raycast spike 13ff3c2 | NEEDS-PLAYTEST (autopilot `raycast` + `sdk-selftest` on the installed build) |
| T-028 | [Content compiler v2: native texture dictionaries](T-028-lcc-native-textures.md) | M4 content compiler v1 | NEEDS-PLAYTEST |
| T-029 | [Cloud development loop and local verification](T-029-cloud-local-loop.md) | none | NEEDS-PLAYTEST (first `verify-local.ps1 -Smoke` run) |

**Arsenal run (2026-09-24):** two Codex agents work in separate worktrees/branches (`arsenal/core` = Agent A, `arsenal/feel` = Agent B) against the shared contracts in `src/LibertyFramework/Arsenal/Contracts`; the orchestrator merges, reviews, packages and installs. T-012 (DevTools completeness) is deferred by the owner.

## Backlog rationale (review 2026-09-24)

Cards above supersede these rows; T-012 remains a proposal.

| ID | Proposal | Basis |
|---|---|---|
| T-011 | Gold finish for carbine and shotgun: two `finishes.json` variants (`w_m4`: `bm_m4a1`/`bm_m4a1_s`/`icon`, `gun@ak47`, `CM_WEAPONS_M4`; `w_shotgun`: `cj_shotgun_comp`/`cj_shotgun_comp_s`/`icon`, `gun@shotgun`, `CM_WEAPONS_SHOTGUN`), normal maps kept. No code change. Best after the pistol finish is proven in game. | HANDOFF §11 "if the gun is gold…"; textures listed from the installed `weapons.img` |
| T-012 | DevTools completeness: Remove weapons / reset weapon state, weapon hash display; test range moving NPC, 100 m marker, reset vehicle; empty PLAYER/VEHICLES/WORLD pages as extension points | HANDOFF §15, §16, §21 |
| T-013 | Debug overlay hit info: last hit bone, distance, damage, fire interval (RPM) | HANDOFF §22 |
| T-014 | Aim profiles beyond on/off (vanilla / slowdown-only / light assist) if an engine control for slowdown/magnetism is found | HANDOFF §9 |
| T-016 | Weapon switching while aiming (natives-only path) | HANDOFF §24, research/LibertyTweaks.md |
| T-017 | Feel layer: small per-shot camera shake and subtle aiming FOV, config-driven | HANDOFF §13, §25; research/RealRecoil.md takeaways |
