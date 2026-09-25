# Project State

Update this file at the end of every task. Newest entries at the top of each section.
Keep it short: this is a dashboard, not a diary.

## Current phase

**Startup crash isolation (2026-09-25, Codex):** Owner's loading-screen freeze ended in Windows Error 1000, `scripthook.dll` / `0xc0000417`. The hook log stopped as it began loading Liberty Vehicle Services; installed configs match the saved baseline. With GTA IV closed, only the LVS `.CS` file was hash-checked and moved to an external quarantine for one controlled startup test. Causality and game launch result await the owner. [T-026](tasks/T-026-performance-visual-baseline.md).

**Performance baseline setup (2026-09-25, Codex):** Owner reports 15–20 FPS and world shadow/light/building flicker. Portable PresentMon, Ghidra + JDK, and FusionFix/DXVK reference sources are staged outside the game. Installed GTAIV.exe was imported into a read-only Ghidra project; an elevated capture script and configuration snapshot are prepared. Frame traces and in-game comparison remain pending. [T-026](tasks/T-026-performance-visual-baseline.md).

**Gore bleed/limb presentation installed (2026-09-24, Codex):** Owner reports Violent Liberty's larger stains/streaks work; little visible leaking and intermittent thrown limbs remain. Log confirms all attempted looping blood effects failed. The current build pulses confirmed one-shot effects with fade/slow timing, retains/retries limb clones and marks landing, and tunes the owner's Violent Liberty INI for stronger/longer head/neck and more frequent shotgun bleed. Build 112 sources, verify 339/339; copied install/rollback byte-identical, 7 real files hash-checked. Backup `violent-liberty-20260924-231558`. Owner playtest pending; stump cap geometry remains open. [T-022](tasks/T-022-combat-effects.md).

**Gore visual companion installed (2026-09-24, Codex):** Owner confirms pass 3 limb throwing and persistent removal in game, but says the cut/blood presentation is poor. A local Violent Liberty 1.2.2 archive provides dynamic wounds and surface blood; source is not included. Added a reversible external-blood mode so that mod can own wound rendering while Liberty Framework owns cuts and stump effects. Build 112 sources, verify 336/336, install/rollback fixture byte-identical; 7 installed files hash-checked, backup `violent-liberty-20260924-224005`. Vulkan startup/combined visuals await owner playtest. See [research](research/ViolentLiberty.md) and [T-022](tasks/T-022-combat-effects.md).

**Gore overhaul (2026-09-24, Claude):** visible blood on every firearm hit, 25 s bleeding, limb and head severing with burst/arterial spray and thrown limbs, fixed the ped-skeleton resolver, and added the ADR-0005 post-rebuild hook. Verify 312/312. Owner playtest pending ([section 7](testing/PHASE2_PLAYTEST.md)).

**Phase 2 open-items build installed (2026-09-24, Claude):** shoulder swap (T-015), arm/leg dismemberment with thrown limbs (T-022), LVS body-part labels (T-023). Verify 296/296; install/rollback dry run passed; owner playtest pending.

**Phase 2 follow-up installed (2026-09-24):** Enabled the owner-approved corpse head-removal prototype and expanded the safehouse gunsmith to carbine/shotgun tuning attachments. Build 104 sources and offline checks 270/270; install backup `phase2-20260924-194809`. In-game effects remain NEEDS-PLAYTEST. Shoulder swap, true limb mesh removal, and model-verified vehicle body kits still require CE runtime/asset evidence.

**Phase 2 integrated build installed (2026-09-24).** Owner confirmed Phase 1 weapon feel, back holsters including gold variants, and trunk storage worked in-game. Safehouse storage remains provisionally assumed. The integrated T-022/T-024/T-025 build passed 270/270 offline checks and an install/rollback dry run, then was installed with backup `phase2-20260924-184410`; all new behavior awaits the [combined owner playtest](testing/PHASE2_PLAYTEST.md). T-015 shoulder swap and T-023 real body variants remain blocked.

**Phase 1 + Liberty Arsenal baseline (2026-09-24).** Universal free aim, per-weapon recoil and real spread for test weapons 58/59/60, spread crosshair, gold pistol finish, expanded DevTools, Arsenal, and Liberty Vehicle Services CE were installed. The owner reported the tested weapons, tunes, holsters, gold variants, and trunks worked; individual T-010 edge cases are not all evidenced. Safehouse behavior is a provisional assumption.

## Key decisions (see `docs/architecture/decisions/`)

- ADR-0001: CE + FusionFix + Tomasak ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 C# probe and reloads successfully. No downgrade.
- ADR-0002: FusionFix v5.0.1 ExtendedLimits assigned the custom pistol ID 58. It replaced the vanilla pistol in the handgun inventory; a deliberate switch is needed. Unused episodic slots are deferred.
- ADR-0004: Engine data (aim camera, CWeaponInfo accuracy, menu prefs, hud.dat reticle globals, bullet list) is located by native-hash/instruction-shape resolvers, validated at runtime, never code-patched, and restored on exit. See docs/game-api/MEMORY.md.
- ADR-0005: Owner-approved exception to ADR-0004 for dismemberment: after-call hooks on the fragInst skeleton rebuilds (0x5F7D70/0x5F6FB0). Bytes validated, flag-gated, restored on unload/exit.
- ADR-0003: T-002 implements a JSON sample with live polling and last-valid retention; live log and gameplay checks passed.

## Verified in-game (by the human tester)

- 2026-09-24 (owner report): Phase 1 weapon gameplay and tunes worked, visible weapons including gold ones appeared on the player's back, and trunk storage worked perfectly. Safehouse storage is provisionally assumed working, without a direct report. Phase 2 has no in-game result yet.
- 2026-09-24 (owner report and logs): the T-007 carbine (ID 59) and shotgun (ID 60) identities selected and worked first try alongside the pistol (ID 58).
- 2026-09-24 (tester report and fresh logs): Steam Complete Edition exe 1.2.0.59 and FusionFix 5.0.1 reach gameplay. Tomasak ScriptHookDotNet 1.7.1.8 loads the probe; startup, heartbeats, domain unload, and restart after `ReloadScripts` were logged, and gameplay continued. Bluetooth DualSense through a Steam Input community layout; in-game auto aim on. With the runtime installed, vanilla pistol aim, fire, reload, cover and vehicle shooting, and save/load passed. See [T-000](tasks/T-000-baseline.md) and [T-001](tasks/T-001-runtime-spike.md).
- 2026-09-24 (tester report, screenshot, and fresh logs): The first DevTools panel opened, navigated, and closed with the controller; D-pad navigation also opened the phone. `LFWeaponGive` selected custom ID 58, and the vanilla handgun presence changed from true to false. Game heartbeats continued. Candidate firing and save/load have not been confirmed.
- 2026-09-24 (tester report): The custom ID 58 pistol passed aim, fire, reload, cover, vehicle, and save/load checks; GTA IV was then closed for the revised menu install.
- 2026-09-24 (tester report and fresh logs): Revised DevTools menu passed controller navigation without phone conflict; game control returned on close. XInput was connected in-game. Menu actions switched between custom pistol ID 58 and vanilla pistol ID 7. Total handgun ammo remained 226 across multiple switches, then 217 after gameplay and another switch. No DevTools error was logged.

## Open questions (blockers for later tasks)

| # | Question | Resolved by |
|---|---|---|
| Q1 | Resolved: ScriptHookDotNet 1.7.1.8 loads an x86 .NET Framework 4.0 DLL on CE 1.2.0.59 with FusionFix; reload succeeds. The maximum usable C# language level is not established. | T-001 |
| Q2 | Resolved: custom IDs 58/59/60 select in game (owner report, logs), and menu switching preserves total ammo. A custom weapon and its vanilla counterpart cannot be carried at the same time (same slot). | T-007 |
| Q3 | Answered offline (universal, accepted by owner): PREF_AUTO_AIM + DISABLE_PLAYER_LOCKON + hud.dat health/armour globals. In-game confirmation pending. | T-010 |
| Q4 | Answered offline: CCamAimWeapon pitch/heading fields (+0x218/+0x21C) found via the SET_GAME_CAM_PITCH worker; runtime-validated before use. In-game confirmation pending. | T-010 |
| Q5 | Answered offline: shrink/zero the hud.dat reticle globals the HUD copies every frame. In-game confirmation pending. | T-010 |
| Q6 | Does Steam Input need a specific controller config (e.g. Xbox layout, no gyro) for consistent results? | T-000 |

## Changelog

- 2026-09-24 — Phase 2 integration: merged both Sol task branches with T-024, reviewed ownership and installer behavior, added a persistent paid Match grip with a spread effect, built 104 sources with zero build warnings/errors, verified 270/270 CE checks, passed byte-identical install/rollback dry run, and installed 10 manifest files with SHA-256 confirmation. The Phase 1 gold assets, LVS scripts/INI, holster config, and player state were retained. Latest backup `scripts/LibertyFramework/backups/phase2-20260924-184410`; combined owner playtest pending.
- 2026-09-24 — T-024 Phase 2 foundations: lower early-burst bloom and faster short-burst recovery for registered test weapons; compact Square/X or E trunk/safehouse panel; lighter holster/debug scans and cached projection; 30-second frame/tick timing lines. Offline checks pass, combined owner playtest pending.
- 2026-09-24 — T-022 combat effects built on `codex/phase2-combat`: bounded player-attributed sampling, bone-region reactions via force native, stock blood PTFX attached to hit bone with cleanup, transient injury/wound state, corpse-only head-removal spike, and limb-loss candidate gating. CE native checks pass offline; appearance/behavior and actual arm/leg mesh removal remain open for owner playtest.
- 2026-09-24 — T-025 stream C staged on `codex/phase2-systems`: physical weapon IDs and metadata survive carry/trunk/safehouse/owned-car state; catalog covers a vanilla sidearm pair and existing carbine. A paid safehouse gunsmith choice switches a pistol between factory/gold variants. T-023 body variants remain blocked pending a model/extra-slot visual test; generic LVS Extras remain unchanged. Offline checks pass; integration and owner playtest pending.
- 2026-09-24 — Liberty Arsenal run (two Codex agents + orchestrator): T-020 Arsenal core (RDR2 loadout + melee, last-car/safehouse overflow, ownership, busted/wasted rules, trunk + safehouse stashes, LVS owned-vehicle link, safehouse discovery from map blips), T-021 visible holsters, T-011 gold carbine/shotgun, T-013 debug hit info, T-014 aim profiles (free/vanilla), T-016 switch while aiming, T-017 shake + aiming FOV; T-015 shoulder swap BLOCKED (no validated camera offset). Liberty Vehicle Services CE (MIT) bundled. Build clean, verify 240/240, install/rollback dry run byte-identical, **installed into the game** (backup phase1-20260924-163020). Owner playtest pending.
- 2026-09-24 — T-010 review pass: checked FusionFix patches against every resolver anchor (no conflict), added ScriptHook.dll name→CE hash checks to verify (166/166). Fixed runtime risks: no natives from drawing callbacks or at process exit, memory restores before the lock-on native, cheaper logging and controller polling, wrap-safe memory checks. Rebuilt and repackaged; game files untouched; owner playtest pending.
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
