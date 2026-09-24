# Task queue for agents

**Current assignment:** T-002 passed and the owner confirmed gameplay. T-001's runtime load and reload passed; its post-install pistol check is being tested in the current game session. No gameplay changes have been started. The project owner performs game playtests and alone marks a tested card `DONE`.

Statuses: `READY` = agent can start; `NEEDS-PLAYTEST` = agent implementation awaits human test; `BLOCKED` = named dependency missing; `DONE` = human verified. Track each task's scope and evidence separately, and update [PROJECT_STATE.md](../PROJECT_STATE.md).

Batch compatible offline work and combine human checks into one gameplay session where practical. Keep separate task statuses and record unverified assumptions rather than treating one task's success as evidence for another.

| ID | Task | Depends on | Status |
|---|---|---|---|
| T-000 | [Record game baseline](T-000-baseline.md) | none | READY (human) |
| T-001 | [Runtime hello world](T-001-runtime-spike.md) | T-000 | NEEDS-PLAYTEST (vanilla pistol check) |
| T-002 | [Config and logging skeleton](T-002-config-logging.md) | T-001 | DONE |
| T-003 | [DevTools minimum menu](T-003-devtools-menu.md) | T-002 | READY (controller layout details pending) |
| T-007 | [Separate gold weapon slot spike](T-007-weapon-slots.md) | T-001 | BLOCKED (next weapon milestone) |
| T-008 | [Per-weapon aim and HUD spike](T-008-aim-hud.md) | T-007 | BLOCKED |
| T-009 | [Camera recoil API spike](T-009-camera-recoil.md) | T-001, T-007 | BLOCKED |

Later tasks (spread/crosshair, live tuning, gold assets, teleports, debug overlay, controller polish, shoulder swap, switching while aiming) should be written after these spikes establish feasible APIs. Avoid speculative implementation cards with invented controls or native names.
