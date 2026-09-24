# Agent A — T-020 Arsenal Core

Branch: `arsenal/core`. Status: `NEEDS-PLAYTEST`; no GTA IV launch or game-folder write.

## What I built

- Script tick inventory snapshot and RDR2 body limits (2 sidearms, 2 long guns across shotgun/rifle/sniper/heavy, 1 melee; thrown uncounted). Least recently selected overflow goes to the last vehicle or last safehouse. Owned same-category replacements are retained. Mission/cutscene/fade movements are deferred.
- Purchase-window, storage and mission ownership decisions; busted clears carried weapons, wasted sends only owned carried weapons to the last safehouse. `ICarriedWeaponsSource` is registered, revisioned, and notifies Holsters before removals.
- One controller-accessible Arsenal DevTools page for trunk/safehouse store/take and **Mark safehouse here**. It uses the existing DevTools player-control lock. Vehicle boot opens through SHDN's `Vehicle.Door(Trunk)` wrapper and closes after the menu closes.
- Episode-specific JSON state, atomic saves with `.bak`, corrupt-file quarantine, temporary car trunks, LVS read-only owned INI parsing, and model/position fallback.

## Files

`src/LibertyFramework/Arsenal/ArsenalCore.cs`, `src/LibertyFramework/Arsenal/Logic/*.cs`, `config/arsenal.json`, `tools/verify/ArsenalCoreChecks.cs`, `docs/tasks/T-020-arsenal-core.md`; appended `docs/architecture/CONFIG_SCHEMA.md`, `docs/game-api/NATIVES.md`, `docs/game-api/native-hashes.csv`, `third_party/README.md`.

## Native and memory changes

Added CE hashes/registry rows for `GET_CURRENT_EPISODE`, `GET_MISSION_FLAG`, `HAS_CUTSCENE_LOADED`, `HAS_CUTSCENE_FINISHED`, `IS_SCREEN_FADING`, `IS_PLAYER_BEING_ARRESTED`, `IS_PLAYER_DEAD`, `IS_CAR_IN_WATER`. Existing `IS_SCREEN_FADED_OUT` is reused. No memory offsets or memory writes. `GET_CUTSCENE_SECTION_PLAYING` was appended to the CSV during research but is not called.

## Config and tests

`arsenal.json` defines limits, ownership window, trunk and LVS distances, category/body-slot mapping, and safehouses. It ships with an empty safehouse list because sourced coordinates were unavailable; the in-game Mark action records the exact position and writes it as verified.

`tools/build.ps1 -ScriptHookDotNetReference .../ScriptHookDotNet.asi`: built 77 sources, 0 errors, 0 warnings. `tools/verify.ps1 -GameDirectory .../GTAIV`: `RESULT passed=199 failed=0`. New tests cover limits, LRU, ownership, mission policy, bust/death, state round-trip/corrupt recovery, and LVS sample parsing. CE registration and ScriptHook name mappings all passed.

## Unverified in game

Every T-020 gameplay behavior. In particular, confirm the cutscene natives' semantics, boot rear location on several vehicle models, purchase timing, death/arrest detection before the game's strip, LVS position matching after driving, and storage menus after save/reload. The T-020 card has exact tester steps.

## Requests for orchestrator

1. Add `config/arsenal.json` to `tools/package-phase1.ps1` and the install manifest; that script currently copies a fixed file list.
2. Add a one or two line T-020 entry to `docs/PROJECT_STATE.md` after merge. That file is outside Agent A's ownership.
