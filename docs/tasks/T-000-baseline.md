# T-000 — Record the human game baseline

Status: **READY (human; partial baseline recorded)**. Owner: human tester. Dependency: none. Scope: installation and observation only.

## Steps and evidence

Follow [setup checklist](../setup/ENVIRONMENT.md). Record exact GTA IV exe version, install channel, FusionFix release/config, ScriptHookDotNet release/file list, controller and Steam Input mapping, aiming settings, launch success/failure, and relevant logs. Check that one vanilla pistol can aim/fire/reload, enter cover, and shoot from a vehicle before adding custom scripts.

## Tester report — 2026-09-24

| Item | Reported observation |
|---|---|
| Game | Steam Complete Edition, executable 1.2.0.59; Steam launches through Rockstar's launcher |
| FusionFix | 5.0.1; game reaches the menu with it enabled |
| Controller | Bluetooth PS5 DualSense with a Steam Input community layout; exact layout title unrecorded |
| Aim setting | In-game auto aim on |
| Vanilla pistol | Aim, fire, reload, and shooting from cover work |
| Follow-up | Tester confirmed GTA IV was running and working on 2026-09-24 before T-001 deployment; no ScriptHookDotNet runtime was then installed |

This is the tester's report, not an agent-run game test. ScriptHookDotNet load and probe behavior were later verified in [T-001](T-001-runtime-spike.md).

## Remaining baseline evidence

- Game reached **gameplay** with FusionFix enabled after T-001 deployment; tester confirmed continued gameplay after `ReloadScripts`.
- Confirm vanilla pistol shooting from a vehicle and a save/load cycle.
- Record the Steam Input community layout title, look/aim sensitivity, display mode, and relevant FusionFix settings for repeatability.
- ScriptHookDotNet was absent immediately before T-001 deployment. The official Tomasak 1.7.1.8 runtime is now installed; its load and log result are recorded in T-001.

The exact community layout title and optional sensitivity details can be added later, but the gameplay launch, vehicle check, and runtime installation state should be clear before marking this card `DONE`.

## Completion

Paste the observations and log excerpts in a [playtest report](../testing/PLAYTEST_REPORT_TEMPLATE.md), then set this card `DONE`. An agent may proceed to T-001 only after the baseline is known.
