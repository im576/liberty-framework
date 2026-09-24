# Task queue for agents

**Current assignment:** T-000, T-001, and T-002 have passed owner gameplay checks. T-003 (minimum DevTools menu) is the next implementation card; T-007 (gold weapon identity spike) is also ready. No gameplay changes have been started. The project owner performs game playtests and alone marks a tested card `DONE`.

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Track each task's scope and evidence separately, and update [PROJECT_STATE.md](../PROJECT_STATE.md).

Batch compatible offline work and combine human checks into one gameplay session where practical. Keep separate task statuses and record unverified assumptions rather than treating one task's success as evidence for another.

| ID | Task | Depends on | Status |
|---|---|---|---|
| T-000 | [Record game baseline](T-000-baseline.md) | none | DONE |
| T-001 | [Runtime hello world](T-001-runtime-spike.md) | T-000 | DONE |
| T-002 | [Config and logging skeleton](T-002-config-logging.md) | T-001 | DONE |
| T-003 | [DevTools minimum menu](T-003-devtools-menu.md) | T-002 | READY (controller layout details pending) |
| T-007 | [Separate gold weapon slot spike](T-007-weapon-slots.md) | T-001 | READY |
| T-008 | [Per-weapon aim and HUD spike](T-008-aim-hud.md) | T-007 | BLOCKED |
| T-009 | [Camera recoil API spike](T-009-camera-recoil.md) | T-001, T-007 | BLOCKED |

Later tasks (spread/crosshair, live tuning, gold assets, teleports, debug overlay, controller polish, shoulder swap, switching while aiming) should be written after these spikes establish feasible APIs. Avoid speculative implementation cards with invented controls or native names.
