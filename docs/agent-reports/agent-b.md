# Agent B — Feel & Presentation handoff (2026-09-24)

Branch/worktree: `arsenal/feel`. No game files were written; finish builder and verifier read local GTA IV files only. The owner has not playtested this branch.

| Card | Built and owned files | Natives / memory / config | In-game status |
|---|---|---|---|
| T-021 | `Arsenal/Holsters/**`, `GameApi/HolsterNatives.cs`, `config/holsters.json`, `FeelChecks.cs` | Registered streaming and orphan-cleanup natives; no engine-memory access. JSON maps weapon ID to active WeaponInfo.xml type, five body placements and nudge steps. | All slot placement, gold props, bike/cutscene/bust visibility, and same-process reload cleanup need playtest. The journal allows cleanup on the next tick after ReloadScripts because DomainUnload cannot call natives. |
| T-011 | `assets/finishes/finishes.json`, builder output under `staging/phase1` | No new runtime native or memory. Gold carbine/shotgun variants with retained normal maps. | In-hand/HUD/holster gold appearance needs playtest. |
| T-017 | `Gunplay/GunplayController.cs`, `Profiles/FeelSettings.cs`, `FeelFovRestoreState.cs`, `config/gunplay.json` | SHDN `Camera.FOV` wrapper and validated ADR-0004 aim-camera fields; no new offsets. Config shake/FOV values. FOV is journalled for recovery after same-process ReloadScripts. | Camera setter effect and feel need playtest. Recoil runs before shake. |
| T-013 | `Gunplay/DebugHitTracker.cs`, `GameApi/DebugHitNatives.cs`, gunplay config/overlay | Registered damage-bone and vehicle-damage natives; SHDN ped damage wrapper. Config debug scan radius and world classification delay. | Damage attribution and hit bone need playtest. `world/unknown` also covers targets outside the scan or without health loss. |
| T-014 | `Profiles/FreeAimSettings.cs`, config validator/controller, `config/gunplay.json` | No new native or memory. `freeAim.profile`: `free` and `vanilla` are supported; `slowdown`/`light` rejected until CE assist control is verified. | Restore and live profile switch need playtest. |
| T-015 | Task card only; status BLOCKED | No approved, validated gameplay-camera lateral offset. `SET_CAM_ATTACH_OFFSET` and `SET_CAM_POINT_OFFSET` signatures alone do not establish third-person aim behaviour. | Needs engine spike/resolver before implementation. |
| T-016 | Gunplay controller, `Profiles/AimingSwitchSettings.cs`, `Gunplay/Logic/AimingCycleRules.cs`, config | SHDN `Weapons.Select` wraps registered `SET_CURRENT_CHAR_WEAPON`; only IDs in `ArsenalRegistry.CarriedWeapons` are selected. Config D-pad left/right bindings. | Aim-camera continuity through native selection needs playtest. |

Documentation appended: `NATIVES.md`, `native-hashes.csv`, `CONFIG_SCHEMA.md`, and `PHASE1_PLAYTEST.md`. Human test steps are on each card. No new memory resolver or offset was added; `MEMORY.md` is unchanged.

The installed IV `WeaponInfo.xml` has no `<assets model>` for `FTHROWER` (ID 19), so that model-less entry cannot produce a prop. Episodic IDs 21–41 use `Game.CurrentEpisode` and the episode XML's `EPISODIC_N` entries; this mapping needs an in-game episode test. The merged finish package must update the three LF gold `<assets model>` values from the installed base models to `lf_gold_*` before the holster gold check.

## Offline evidence

- `tools/build-finishes.ps1 -GameDirectory <GTAIV>` succeeded; generated carbine and shotgun diffuse/specular/icon previews and read-back verified the six IMG entries. No game-folder write.
- `tools/build.ps1 -ScriptHookDotNetReference <runtime/ScriptHookDotNet.asi>`: Windows Framework C# 5 x86 build, zero errors and zero warnings.
- `tools/verify.ps1 -GameDirectory <GTAIV>`: `RESULT passed=202 failed=0` against `GTAIV.exe` and `ScriptHook.dll`, including native-map and new offline holster/aim/config checks.

## Requests for orchestrator

1. Merge with Agent A's T-020 implementation and check the `ArsenalRegistry.CarriedWeapons` slot assignments, `WeaponsRemoving` on bust/death, and loadout order used by T-016. Agent B could only compile against the shared contracts.
2. Update `docs/PROJECT_STATE.md` and `docs/tasks/README.md` after merge. Those files are outside Agent B ownership. Keep each card `NEEDS-PLAYTEST` until its own owner evidence; T-015 remains `BLOCKED`.
3. For T-015, supply a disassembly-backed lateral aim-camera offset resolver in `Core/Memory` with runtime validation and restore, or arrange a minimal CE native spike before enabling shoulder swap. That path is outside Agent B ownership.
4. Package/install the merged build and conduct the combined owner playtest. Check FOV restoration and whether T-016 selection preserves aim; both require GTA IV.
