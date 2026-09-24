# T-020 — Liberty Arsenal core: loadout, ownership, storage, death/arrest

Status: **READY** (Codex Agent A, branch `arsenal/core`). Owner decisions 2026-09-24; see the rules below — they are requirements, not suggestions.

## Owner rules (requirements)

1. **Loadout (RDR2 + melee).** On the body at most: 2 sidearms, 2 long guns, 1 melee. Thrown weapons are carried but uncounted.
   - Sidearm categories: `Handgun`, `SMG` (GTA IV already allows one per category, so the pair is handgun + SMG).
   - Long-gun categories: `Shotgun`, `Rifle`, `Sniper`, `Heavy` — at most **two** of these four carried at once.
   - Melee: 1. Limits and category→body-slot mapping come from `config/arsenal.json`.
2. **Overflow ("the horse").** When the player gains a weapon beyond the limit, the least-recently-used weapon of that group moves to storage: the **last used vehicle's trunk** (RDR2 horse style, even if far away); if there is no last vehicle, to the **most recently used safehouse stash**. When the game itself replaces a same-category weapon (picking up a different pistol), an **owned** replaced weapon is also moved to that storage instead of vanishing. Log every move.
3. **Ownership.** Owned = bought (a purchase is a weapon gain in the same tick window as a money decrease; window in config) or taken from the player's trunk/stash. A picked-up weapon is unowned until the player stores it once (then it is owned). Mission-given weapons are unowned.
4. **Busted:** every carried weapon is permanently gone (owned or not). **Wasted:** carried *owned* weapons go to the most recently used safehouse stash; unowned are lost. Snapshot the carried set every tick so the result does not depend on when the game strips weapons.
5. **Missions and cutscenes:** never remove or move weapons while a mission is active or a cutscene/fade is running. When the mission ends, over-limit weapons go to overflow storage per rule 2.
6. **Trunk storage.** Standing at a vehicle's rear (distance in config) offers a trunk menu: store carried weapon / take stored weapon, with the boot opened (`OPEN_CAR_DOOR`, boot door index from natives/SHDN docs — verify) and closed after. Contents persist only for **owned vehicles** (below). Random vehicles keep contents while they exist and lose them when deleted, burnt out or sunk (log it).
7. **Safehouses.** A configured list (id, name, episode, x, y, z, radius). "Most recently used" = last one the player entered the radius of or saved at. Each has a stash spot with the same store/take menu. Coordinates must come from a documented source (game data file, script, or a value you log in game); mark unverified ones `"verified": false` in config. Add a DevTools action "Mark safehouse here" through the page hook.
8. **Owned vehicles.** Liberty Vehicle Services CE (MIT, ekzestean) is the vehicle base and is installed alongside us. Read its source (extracted at `...\work\research\lvs`; `OwnedVehicleRecord`, `LoadOwnedVehicles`, `[owned.<id>]` blocks) and implement a **read-only** reader of its owned-vehicle state so a trunk can be keyed by its owned-vehicle `Id`. If LVS is absent or its file is unreadable, fall back to an Arsenal-owned marker: the last vehicle the player stored weapons in and exited is remembered by model + position. You may adapt LVS code (MIT) with credit in `third_party/README.md`.
9. **Persistence.** `scripts/LibertyFramework/state/arsenal_<episode>.json` via `LibertyPaths.ArsenalState` and `JsonStore.Save` (atomic, `.bak`). Corrupt file → keep the `.bak`, log, start empty, never crash.

## Shared contracts (read-only for agents; request changes from the orchestrator)

- `src/LibertyFramework/Arsenal/Contracts/`: `BodySlot`, `WeaponCategory`, `WeaponRecord`, `CarriedWeapon`, `ICarriedWeaponsSource`, `ArsenalRegistry`.
- Agent A implements `ICarriedWeaponsSource` and assigns it to `ArsenalRegistry.CarriedWeapons` once running, bumping `Revision` on every change, and calls `ArsenalRegistry.RaiseWeaponsRemoving(reason)` before stripping weapons.
- DevTools: register at most one page with `DevToolsPages.Register(title, factory)`; do not edit `DevToolsMenu.cs`.
- Pure logic goes in folders named `Logic` (no `GTA` namespace), which `tools/verify.ps1` compiles automatically. Tests go in `tools/verify/ArsenalCoreChecks.cs`.
- Paths: `LibertyPaths.ArsenalConfig`, `LibertyPaths.ArsenalState(episode)`.

## Acceptance (offline)

- Build 0 errors / 0 warnings; verify all green.
- Tests: overflow picks least-recently-used; long-gun cap 2 across 4 categories; ownership transitions (purchase window, stash → owned, mission → unowned); bust clears all; death moves only owned to last safehouse; mission gate defers moves; storage JSON round-trip and corrupt-file recovery; LVS owned-file parse on a sample built from the LVS source format.
- Every native in `native-hashes.csv`, registered and ScriptHook-mapped (verify proves it).
- `config/arsenal.json` documented in `docs/architecture/CONFIG_SCHEMA.md` (Arsenal section).
- Human test steps below, status `NEEDS-PLAYTEST`, report in `docs/agent-reports/agent-a.md`.

## Human test steps

(Agent A fills in.)
