# Task queue for agents

**Current assignment:** T-010 Phase 1 integrated gunplay build is packaged in `staging/phase1` and awaits one owner playtest ([checklist](../testing/PHASE1_PLAYTEST.md)). T-000–T-003 are DONE; T-007's carbine/shotgun IDs 59/60 were confirmed by the owner (logs show both selected). The project owner alone marks a tested card `DONE`.

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

Later tasks (spread/crosshair, live tuning, gold assets, teleports, debug overlay, controller polish, shoulder swap, switching while aiming) should be written after these spikes establish feasible APIs. Avoid speculative implementation cards with invented controls or native names.
