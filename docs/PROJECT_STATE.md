# Project State

Update this file at the end of every task. Newest entries at the top of each section.
Keep it short: this is a dashboard, not a diary.

## Current phase

**Phase 0 — Research and environment.** T-001's minimal runtime probe is built, deployed, and verified in gameplay, including `ReloadScripts`; its task status stays `NEEDS-PLAYTEST` until the human closes it. No gameplay behavior is implemented. T-000 vehicle/save checks remain open. Weapon-slot choices are not yet verified in-game.

## Key decisions (see `docs/architecture/decisions/`)

- ADR-0001: CE + FusionFix + Tomasak ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 C# probe and reloads successfully. No downgrade.
- ADR-0002: Candidate gold weapon identities use spare/episodic slots via overrides; T-007 must prove this.
- ADR-0003: Plan for JSON profiles and hot reload; T-002 will establish the schema and behavior.

## Verified in-game (by the human tester)

- 2026-09-24 (tester report and fresh logs): Steam Complete Edition exe 1.2.0.59 and FusionFix 5.0.1 reach gameplay. Tomasak ScriptHookDotNet 1.7.1.8 loads the probe; startup, heartbeats, domain unload, and restart after `ReloadScripts` were logged, and gameplay continued. Bluetooth DualSense through a Steam Input community layout; in-game auto aim on; vanilla pistol aim, fire, reload, and cover shooting worked before deployment. See [T-000](tasks/T-000-baseline.md) and [T-001](tasks/T-001-runtime-spike.md). Vehicle shooting and save/load remain unverified.

## Open questions (blockers for later tasks)

| # | Question | Resolved by |
|---|---|---|
| Q1 | Resolved: ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 DLL on CE 1.2.0.59 with FusionFix; reload succeeds. The maximum usable C# language level is not established. | T-001 |
| Q2 | Can a spare weapon slot (UNUSED0 / an unused EPISODIC_n in IV) hold a working gold pistol/carbine/shotgun? | T-007 |
| Q3 | How can lock-on / target snapping / reticle health ring be disabled **per weapon**? | T-008 |
| Q4 | Which API actually moves the aim camera (pitch/yaw) for recoil kick on CE — a native, a SHDN wrapper, or memory? | T-009 |
| Q5 | How to hide the vanilla reticle so we can draw our own crosshair? | T-008 |
| Q6 | Does Steam Input need a specific controller config (e.g. Xbox layout, no gyro) for consistent results? | T-000 |

## Changelog

- 2026-09-24 — T-001 probe built with zero compiler errors/warnings, deployed while GTA IV was closed, and verified in gameplay. Fresh logs confirm startup, heartbeats, domain unload, and restart after `ReloadScripts`; tester reported continued gameplay.
- 2026-09-24 — Recorded tester's partial T-000 baseline; left T-000 open for gameplay/vehicle/runtime-install evidence.
- 2026-09-24 — Recovered the interrupted Claude scaffold; completed the research summary, task queue, setup/test templates, architecture notes, and original brief. No gameplay implementation or in-game verification.
- 2026-09-24 — Initial research notes and scaffold started. Task backlog written.
