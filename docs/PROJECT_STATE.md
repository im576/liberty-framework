# Project State

Update this file at the end of every task. Newest entries at the top of each section.
Keep it short: this is a dashboard, not a diary.

## Current phase

**Phase 0 — Research and environment.** The research and agent task scaffold is prepared. No gameplay code exists.
Next task: `T-000` (human records and tests the baseline), then `T-001` (hello-world script). Proposed runtime and weapon-slot choices remain unverified.

## Key decisions (see `docs/architecture/decisions/`)

- ADR-0001: Proposed runtime = CE + FusionFix + ScriptHookDotNet (C#); T-001 must verify before adoption. No downgrade by default.
- ADR-0002: Candidate gold weapon identities use spare/episodic slots via overrides; T-007 must prove this.
- ADR-0003: Plan for JSON profiles and hot reload; T-002 will establish the schema and behavior.

## Verified in-game (by the human tester)

_Nothing yet._

## Open questions (blockers for later tasks)

| # | Question | Resolved by |
|---|---|---|
| Q1 | Does ScriptHookDotNet 1.7.1.8 run on the tester's CE 1.2.0.59 + FusionFix install, and which .NET target/C# version loads? | T-001 |
| Q2 | Can a spare weapon slot (UNUSED0 / an unused EPISODIC_n in IV) hold a working gold pistol/carbine/shotgun? | T-007 |
| Q3 | How can lock-on / target snapping / reticle health ring be disabled **per weapon**? | T-008 |
| Q4 | Which API actually moves the aim camera (pitch/yaw) for recoil kick on CE — a native, a SHDN wrapper, or memory? | T-009 |
| Q5 | How to hide the vanilla reticle so we can draw our own crosshair? | T-008 |
| Q6 | Does Steam Input need a specific controller config (e.g. Xbox layout, no gyro) for consistent results? | T-000 |

## Changelog

- 2026-09-24 — Recovered the interrupted Claude scaffold; completed the research summary, task queue, setup/test templates, architecture notes, and original brief. No gameplay implementation or in-game verification.
- 2026-09-24 — Initial research notes and scaffold started. Task backlog written.
