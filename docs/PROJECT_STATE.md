# Project State

Update this file at the end of every task. Newest entries at the top of each section.
Keep it short: this is a dashboard, not a diary.

## Current phase

**Phase 0 — Research and environment.** T-000's baseline, T-001's runtime/reload and post-install pistol checks, and T-002's live config checks have passed owner playtests. T-003's read-only DevTools menu and T-007's custom pistol identity probe are built offline and await one guarded installation after GTA IV closes. No gold weapon behavior is implemented. The custom weapon identity is not yet verified in-game.

## Key decisions (see `docs/architecture/decisions/`)

- ADR-0001: CE + FusionFix + Tomasak ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 C# probe and reloads successfully. No downgrade.
- ADR-0002: FusionFix v5.0.1 ExtendedLimits provides candidate custom weapon IDs starting at 58; T-007 must prove selection and coexistence in this game. Unused episodic slots are deferred.
- ADR-0003: T-002 implements a JSON sample with live polling and last-valid retention; live log and gameplay checks passed.

## Verified in-game (by the human tester)

- 2026-09-24 (tester report and fresh logs): Steam Complete Edition exe 1.2.0.59 and FusionFix 5.0.1 reach gameplay. Tomasak ScriptHookDotNet 1.7.1.8 loads the probe; startup, heartbeats, domain unload, and restart after `ReloadScripts` were logged, and gameplay continued. Bluetooth DualSense through a Steam Input community layout; in-game auto aim on. With the runtime installed, vanilla pistol aim, fire, reload, cover and vehicle shooting, and save/load passed. See [T-000](tasks/T-000-baseline.md) and [T-001](tasks/T-001-runtime-spike.md).

## Open questions (blockers for later tasks)

| # | Question | Resolved by |
|---|---|---|
| Q1 | Resolved: ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 DLL on CE 1.2.0.59 with FusionFix; reload succeeds. The maximum usable C# language level is not established. | T-001 |
| Q2 | Can FusionFix's custom weapon registration hold a distinct pistol/carbine/shotgun, and do same-category weapons coexist in inventory? | T-007 |
| Q3 | How can lock-on / target snapping / reticle health ring be disabled **per weapon**? | T-008 |
| Q4 | Which API actually moves the aim camera (pitch/yaw) for recoil kick on CE — a native, a SHDN wrapper, or memory? | T-009 |
| Q5 | How to hide the vanilla reticle so we can draw our own crosshair? | T-008 |
| Q6 | Does Steam Input need a specific controller config (e.g. Xbox layout, no gyro) for consistent results? | T-000 |

## Changelog

- 2026-09-24 — Found FusionFix v5.0.1's custom weapon registration at ID 58+, with ExtendedLimits already enabled. Staged a cloned pistol definition under LF_GOLD_PISTOL and built diagnostic console commands. XML parsing and game-running deployment guard passed; no game files changed. Awaiting combined T-003/T-007 playtest.
- 2026-09-24 — Built T-003's read-only DevTools menu for x86 with zero errors/warnings. L3+R3 and D-pad input paths are prepared but unverified in-game; existing game files were not changed while GTA IV ran.
- 2026-09-24 — Owner confirmed all remaining pistol, vehicle, and save/load baseline checks passed with the runtime installed; T-000 and T-001 are DONE.
- 2026-09-24 — Owner confirmed gameplay stayed responsive after the T-002 live config test; T-002 is DONE and T-003 is ready for implementation.
- 2026-09-24 — T-002 live config test passed all three log checks in one session and restored the original config. GTA IV stayed running; awaiting tester confirmation of gameplay after the test.
- 2026-09-24 — Audited installed T-002 logs and hash; startup/default heartbeats are confirmed. Added one-session live config test script and guarded the original T-001 DLL backup against replacement. Live config edits are still untested in-game.
- 2026-09-24 — T-002 config sample, loader, bounded logger, and guarded deployment script built with zero errors/warnings. Offline harness passed valid edit, malformed-input retention, and recovery. Installed after GTA IV closed; installed DLL hash matches the build. In-game test pending.
- 2026-09-24 — T-001 probe built with zero compiler errors/warnings, deployed while GTA IV was closed, and verified in gameplay. Fresh logs confirm startup, heartbeats, domain unload, and restart after `ReloadScripts`; tester reported continued gameplay.
- 2026-09-24 — Recorded tester's partial T-000 baseline; left T-000 open for gameplay/vehicle/runtime-install evidence.
- 2026-09-24 — Recovered the interrupted Claude scaffold; completed the research summary, task queue, setup/test templates, architecture notes, and original brief. No gameplay implementation or in-game verification.
- 2026-09-24 — Initial research notes and scaffold started. Task backlog written.
