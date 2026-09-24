# Task queue for agents

**Current assignment:** repository preparation and research only. No gameplay code has been started. The project owner performs game playtests and alone marks a tested card `DONE`.

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Pick one task whose dependencies are done, keep changes inside its scope, and update [PROJECT_STATE.md](../PROJECT_STATE.md).

| ID | Task | Depends on | Status |
|---|---|---|---|
| T-000 | [Record game baseline](T-000-baseline.md) | none | READY (human) |
| T-001 | [Runtime hello world](T-001-runtime-spike.md) | T-000 | BLOCKED |
| T-002 | [Config and logging skeleton](T-002-config-logging.md) | T-001 | BLOCKED |
| T-003 | [DevTools minimum menu](T-003-devtools-menu.md) | T-002 | BLOCKED |
| T-007 | [Separate gold weapon slot spike](T-007-weapon-slots.md) | T-001 | BLOCKED |
| T-008 | [Per-weapon aim and HUD spike](T-008-aim-hud.md) | T-007 | BLOCKED |
| T-009 | [Camera recoil API spike](T-009-camera-recoil.md) | T-001, T-007 | BLOCKED |

Later tasks (spread/crosshair, live tuning, gold assets, teleports, debug overlay, controller polish, shoulder swap, switching while aiming) should be written after these spikes establish feasible APIs. Avoid speculative implementation cards with invented controls or native names.
