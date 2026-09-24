# Project State

Update this file at the end of every task. Newest entries at the top of each section.
Keep it short: this is a dashboard, not a diary.

## Current phase

**Phase 1 — integrated gunplay build (T-010), awaiting one owner playtest.** Universal free aim, per-weapon recoil and real spread for test weapons 58/59/60, spread crosshair, gold pistol finish, and expanded DevTools are built and packaged in `staging/phase1`. Offline: 132/132 verification checks against the installed GTAIV.exe; install/rollback dry run byte-identical. No in-game evidence yet for any T-010 behaviour.

## Key decisions (see `docs/architecture/decisions/`)

- ADR-0001: CE + FusionFix + Tomasak ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 C# probe and reloads successfully. No downgrade.
- ADR-0002: FusionFix v5.0.1 ExtendedLimits assigned the custom pistol ID 58. It replaced the vanilla pistol in the handgun inventory; a deliberate switch is needed. Unused episodic slots are deferred.
- ADR-0004: Engine data (aim camera, CWeaponInfo accuracy, menu prefs, hud.dat reticle globals, bullet list) is located by native-hash/instruction-shape resolvers, validated at runtime, never code-patched, and restored on exit. See docs/game-api/MEMORY.md.
- ADR-0003: T-002 implements a JSON sample with live polling and last-valid retention; live log and gameplay checks passed.

## Verified in-game (by the human tester)

- 2026-09-24 (tester report and fresh logs): Steam Complete Edition exe 1.2.0.59 and FusionFix 5.0.1 reach gameplay. Tomasak ScriptHookDotNet 1.7.1.8 loads the probe; startup, heartbeats, domain unload, and restart after `ReloadScripts` were logged, and gameplay continued. Bluetooth DualSense through a Steam Input community layout; in-game auto aim on. With the runtime installed, vanilla pistol aim, fire, reload, cover and vehicle shooting, and save/load passed. See [T-000](tasks/T-000-baseline.md) and [T-001](tasks/T-001-runtime-spike.md).
- 2026-09-24 (tester report, screenshot, and fresh logs): The first DevTools panel opened, navigated, and closed with the controller; D-pad navigation also opened the phone. `LFWeaponGive` selected custom ID 58, and the vanilla handgun presence changed from true to false. Game heartbeats continued. Candidate firing and save/load have not been confirmed.
- 2026-09-24 (tester report): The custom ID 58 pistol passed aim, fire, reload, cover, vehicle, and save/load checks; GTA IV was then closed for the revised menu install.
- 2026-09-24 (tester report and fresh logs): Revised DevTools menu passed controller navigation without phone conflict; game control returned on close. XInput was connected in-game. Menu actions switched between custom pistol ID 58 and vanilla pistol ID 7. Total handgun ammo remained 226 across multiple switches, then 217 after gameplay and another switch. No DevTools error was logged.

## Open questions (blockers for later tasks)

| # | Question | Resolved by |
|---|---|---|
| Q1 | Resolved: ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 DLL on CE 1.2.0.59 with FusionFix; reload succeeds. The maximum usable C# language level is not established. | T-001 |
| Q2 | Resolved for the first pistol: custom ID 58 selects and passes core gameplay/save/load, and menu switching preserves total ammo. The same handgun slot cannot hold both variants simultaneously. Carbine/shotgun IDs 59/60 are staged but unverified. | T-007 |
| Q3 | Answered offline (universal, accepted by owner): PREF_AUTO_AIM + DISABLE_PLAYER_LOCKON + hud.dat health/armour globals. In-game confirmation pending. | T-010 |
| Q4 | Answered offline: CCamAimWeapon pitch/heading fields (+0x218/+0x21C) found via the SET_GAME_CAM_PITCH worker; runtime-validated before use. In-game confirmation pending. | T-010 |
| Q5 | Answered offline: shrink/zero the hud.dat reticle globals the HUD copies every frame. In-game confirmation pending. | T-010 |
| Q6 | Does Steam Input need a specific controller config (e.g. Xbox layout, no gyro) for consistent results? | T-000 |

## Changelog

- 2026-09-24 — T-010 built: free aim, recoil, real spread with shot-audit calibration, crosshair, gold pistol finish (lf_gold_pistol in update/LibertyFramework/LibertyFramework.img), DevTools pages (weapons, gunplay, live tuning, presets/config, teleport, test range, inspect). Build clean; verify 132/132; package + install/rollback dry run passed. Game files untouched; owner playtest pending.
- 2026-09-24 — With GTA IV closed, installed the T-007 three-weapon XML override and matching DLL. Backup, hash receipt, XML parse, and installed file checks passed; next step is one in-game carbine/shotgun test.
- 2026-09-24 — Staged T-007 carbine and shotgun XML entries using base M4 and pump shotgun models/stats, added guarded menu actions and per-pair ammo transfer, and built x86 offline. The running game files were not changed.
- 2026-09-24 — Owner confirmed the revised controller menu and weapon switching worked completely; T-003 is DONE. Logs independently confirm raw XInput input, menu actions, control lock/open-close, and handgun ammo continuity. T-007 advances to carbine/shotgun expansion.
- 2026-09-24 — Owner confirmed all candidate pistol behavior checks passed. Added total-ammo transfer to the revised menu's handgun switch; x86 build passed while GTA IV was closed. Clip state and menu switch behavior remain unverified.
- 2026-09-24 — After live feedback, added Weapon Status/Give Test Pistol/Give Vanilla Pistol to DevTools with confirmation for give actions. Revised menu temporarily disables player controls while open to prevent D-pad phone input; local x86 build passed, live test pending. No files under the running game were changed.
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
