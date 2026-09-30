# AGENTS.md — Rules for AI agents working in this repo

You are an AI coding agent. A human tester runs the game; **you cannot run GTA IV**.
Read this whole file before doing anything. It is short on purpose.

## 1. Read these first, in this order

1. `AGENTS.md` (this file)
2. `docs/PROJECT_STATE.md` — short dashboard: current phase, engine component status, decisions (history: `docs/archive/`)
   - In a cloud session (Claude Code on the web): also `docs/workflow/CLOUD_LOCAL_LOOP.md` — how work is built in the
     cloud, queued for the owner's PC and verified there. `tools/cloud/test-all.sh` runs every offline check.
   - Asked to "do the next session": `docs/workflow/NEXT_SESSIONS.md`.
3. The task card you were given, in `docs/tasks/` (e.g. `docs/tasks/T-033-world-objects.md`); finished cards are in `docs/archive/tasks/`
4. `docs/workflow/CLOUD_LOCAL_LOOP.md` (cloud sessions build and test offline; the owner's PC verifies through `tests/local/checks.json`) and `docs/workflow/NEXT_SESSIONS.md`, if your work queues game checks\n5. Only the docs that task card links to. Do not read the whole repo.

If you were not given a task card: open `docs/tasks/README.md`, pick the first active task whose
status is `READY` and whose dependencies are all `DONE`. Tell the human which one you picked.

## 2. What this project is (one paragraph)

The **Liberty engine** for **GTA IV: The Complete Edition, exe 1.2.0.59**, running on top of **FusionFix** and
**ScriptHookDotNet** (C#): a native core (`native/LibertyCore`), a public SDK (`sdk/`), a content compiler and Blender
add-on (`tools/content`, `tools/blender`), hot reload, and an autopilot that launches the game and runs scenarios. The
Phase 1/2 gameplay work (gold test weapons with free aim, recoil, crosshair, Arsenal, holsters/slings, trunk, gore, the
"Liberty DevTools" menu) is finished and kept as reference modules. Next: the first complex mod built on the engine;
then Phase 3, reverse engineering toward FiveM-level control. Current state: `docs/PROJECT_STATE.md`. Original spec
(historical): `docs/archive/HANDOFF.md`. Architecture: `docs/architecture/OVERVIEW.md`, `docs/architecture/ENGINE.md`.

## 3. Hard rules (never break these)

1. **Keep task scope explicit.** Track each change and its test evidence against a task card. The
   project owner prefers batching compatible offline work so one game launch can verify several
   checks. Prepare dependent work only when its assumptions are documented, and do not mark any
   task `DONE` until its own in-game evidence exists.
2. **Vanilla must stay vanilla.** Only the gold test weapons may behave differently from the base
   game. Every gameplay change must be gated by "is the current weapon a registered test weapon?".
   Owner-approved exception (T-010): free aim (no auto-aim/lock-on/health ring) and the LF crosshair
   apply to all weapons while their DevTools toggles are on, and restore the player's settings when off.
   Recoil and spread stay test-weapon-only.
3. **Tuning values live in `config/`, never in code.** Code defines algorithms. JSON defines numbers.
   If you type a gameplay number (recoil, spread, speed, FOV, distance) into a `.cs` file, you are
   doing it wrong — add a config field instead (see `docs/architecture/CONFIG_SCHEMA.md`).
4. **Do not guess engine behavior.** If you don't know how GTA IV does something, do not invent a
   native name, memory offset, or struct layout. Check `docs/game-api/NATIVES.md` and
   `docs/research/`. If it isn't there, write it down as an open question in the task card and
   stop, or build the smallest possible experiment (a "spike") that the human can test.
5. **Every native you call must be listed in `docs/game-api/NATIVES.md`** with its CE status.
   Prefer ScriptHookDotNet wrapper classes (`Player`, `Ped`, `Weapon`, `Camera`) over raw natives.
6. **No hardcoded memory addresses.** If memory access is ever needed, use a byte pattern scan and
   document it in `docs/game-api/MEMORY.md`. This requires an ADR (see `docs/architecture/decisions/`).
7. **Licensing.** Do NOT copy code from Liberty Tweaks (it has no license = all rights reserved).
   Do NOT copy code from FusionFix or IV-SDK .NET (GPL-3.0) unless an ADR approves it.
   Do NOT add ripped commercial-game assets. See `third_party/README.md`.
8. **Log everything important** through the project logger (see `docs/architecture/OVERVIEW.md` §Logging).
   Every catch block logs. Never swallow exceptions silently.
9. **Never break the game loop.** A script exception must be caught, logged, and the feature
   disabled — the game must keep running.
10. **Don't over-engineer.** Build abstractions only for things the current task needs.

## 4. Tech stack (verified in T-000/T-001)

| Thing | Value |
|---|---|
| Game | GTA IV: The Complete Edition; record the actual exe version in T-000 (1.2.0.59 is the proposed target) |
| Base mods | FusionFix (includes Ultimate ASI Loader as `dinput8.dll` + FusionOverloader) |
| Script runtime | Tomasak ScriptHookDotNet 1.7.1.8 + bundled CE hook; T-001 verified in-game load and reload |
| Language | C# / .NET Framework 4.0 / x86 verified by T-001 |
| C# version | C# 7.3 via Roslyn 4.11 (`tools/get-toolchains.ps1`), `/unsafe` for the LibertyCore ABI; verified in game 2026-09-25 |
| Native core | `native/LibertyCore` (C++20, clang/llvm-mingw, 32-bit) loaded by the engine; see ADR-0006 and `docs/architecture/ENGINE.md` |
| Structure | One SHDN script (`Engine.EngineHost`); every feature is a `[Module]` class. New features must be modules, not `GTA.Script` subclasses |
| In-game testing | `tools/autopilot` launches the game and runs scenarios; add a scenario for each new mechanic |
| Output | `LibertyFramework.net.dll` in `<GTA IV>\scripts\`; `LibertyCore.dll` in `scripts\LibertyFramework\bin\`; installed by `tools/install-phase2.ps1` or the local verifier |
| Config | JSON files in `config/`, deployed to `<GTA IV>\scripts\LibertyFramework\config\` |
| Logs | `<GTA IV>\scripts\LibertyFramework\logs\LibertyFramework.log` |

Build and deploy commands are in `tools/README.md`. The stack above was verified for the minimal T-001 probe; gameplay APIs need their own task-specific tests.

## 5. How to finish a task (Definition of Done)

A task is done only when ALL of these are true:

- [ ] Code builds with zero errors (paste the build output summary in your final message).
- [ ] `tools/cloud/test-all.sh` (cloud) has no FAIL; paste its summary table.
- [ ] Every behaviour that needs the game or its files has a check in `tests/local/checks.json`, added in the same
      PR as the code, and the plan is regenerated (`python3 tools/checks/checks.py plan`). See
      `docs/workflow/CLOUD_LOCAL_LOOP.md`.
- [ ] New tuning values are in `config/` and documented in `docs/architecture/CONFIG_SCHEMA.md`.
- [ ] New natives are added to `docs/game-api/NATIVES.md`.
- [ ] The task card's **"Human test steps"** section is filled in: numbered, exact button presses,
      and what the tester should see. The human cannot read your mind.
- [ ] The task card status is set to `NEEDS-PLAYTEST` (not `DONE` — only the human sets `DONE`).
- [ ] `docs/PROJECT_STATE.md` is updated (one or two lines).
- [ ] You committed with a clear message: `T-00X: <what changed>`.

## 5b. When you need art

Never stop to ask for image prompts. File a request in the art queue (`python tools/art/artq.py new ...`, see
`docs/art/README.md`), keep working, and review, approve, prep and integrate the result when it arrives in
`art/generated/`. The repository is the source of truth for every request, prompt and generated file.

## 6. When you are stuck

Write a `## Blocked` section in the task card: what you tried, what failed, the exact error, and
what information you need. Set status to `BLOCKED`. Stop. Do not keep trying random fixes.

## 7. Playtest reports

The human reports results using `docs/testing/PLAYTEST_REPORT_TEMPLATE.md`. When a report says
something broke, first read the log file excerpt in the report before changing code.

Results of `tools/verify-local.ps1` arrive on the `verification-results` branch. Process them as
`docs/workflow/CLOUD_LOCAL_LOOP.md` describes; the same rule applies: read the logs first.

## 8. Style

- One class per file. File name = class name.
- Namespaces mirror folders: `LibertyFramework.Gunplay.Recoil`, etc.
- Comment **why** when engine behavior is non-obvious. Don't comment obvious code.
- Names are descriptive: `verticalKickDegrees`, not `vk`.
- Units in names or doc comments: degrees, meters, milliseconds, per-second.
