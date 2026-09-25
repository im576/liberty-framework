# CE game API registry

Hashes and handler addresses are checked by `tools/verify.ps1` against `GTAIV.exe` 1.2.0.59 (see `docs/game-api/native-hashes.csv`, taken from FusionFix's CE native list). "Registered" means the exe registers a handler for that hash; verify also checks that the installed `ScriptHook.dll` translates each called name (Jenkins hash of the lower-case name) to that CE hash, which is how `Function.Call("NAME")` reaches the game. In-game behaviour still needs the T-010 playtest. Natives are called only from script ticks and `DomainUnload`, never from `PerFrameDrawing` or `ProcessExit`. All calls go through `src/LibertyFramework/GameApi/Natives.cs`.

| Native | Purpose | CE status | Task |
|---|---|---|---|
| `GET_PLAYER_ID` | player index for memory lookups | registered 0x62E319C6 | T-010 |
| `DISABLE_PLAYER_LOCKON(player, bool)` | no hard lock-on (sets ped targeting flag bit 23, disassembled) | registered 0x711214F3 | T-010 |
| `GET_GAME_CAM`, `DOES_CAM_EXIST`, `GET_CAM_ROT`, `GET_CAM_POS`, `GET_CAM_FOV` | game camera handle, validation, crosshair FOV, shot audit | registered | T-010 |
| `GET_CHAR_SPEED`, `IS_CHAR_DUCKING`, `IS_PED_IN_COVER`, `IS_CHAR_IN_ANY_CAR`, `IS_CHAR_IN_AIR` | shooter state for spread/recoil multipliers | registered | T-010 |
| `IS_PAUSE_MENU_ACTIVE`, `IS_SCREEN_FADED_OUT`, `IS_PLAYER_PLAYING`, `IS_PLAYER_CONTROL_ON` | hide crosshair in menus/cutscenes; skip when not playing | registered | T-010 |
| `GET_GROUND_Z_FOR_3D_COORD`, `REQUEST_COLLISION_AT_POSN`, `LOAD_SCENE`, `SET_CHAR_COORDINATES`, `SET_CHAR_HEADING` | teleports | registered | T-010 |
| `CLEAR_WANTED_LEVEL` | test range helper | registered | T-010 |
| `GET_CURRENT_EPISODE()` | separate Arsenal state for IV, TLAD, TBOGT | registered 0x7D7619D2 | T-020 |
| `GET_MISSION_FLAG()` | defer Arsenal removals/storage during missions | registered 0x2BC64736 | T-020 |
| `HAS_CUTSCENE_LOADED()` / `HAS_CUTSCENE_FINISHED()` | defer Arsenal removals while a loaded cutscene has not finished | registered 0x5DE43980 / 0x4ECE1AD2 | T-020 |
| `IS_SCREEN_FADING()` | defer Arsenal removals during transitions | registered 0x73700561 | T-020 |
| `IS_PLAYER_BEING_ARRESTED()` | busted loss | registered 0x79A95BF9 | T-020 |
| `IS_PLAYER_DEAD(player)` | wasted loss | registered 0x12AE0E27 | T-020 |
| `IS_CAR_IN_WATER(vehicle)` | discard sunk temporary trunks | registered 0x0FF342B2 | T-020 |
| `GET_FIRST_BLIP_INFO_ID(sprite)` / `GET_NEXT_BLIP_INFO_ID(sprite)` / `DOES_BLIP_EXIST(blip)` / `GET_BLIP_COORDS(blip, Vector3*)` | discover the game's own safehouse radar blips (sprite 29 = SHDN `BlipIcon.Building_Safehouse`) as Arsenal safehouses; failure disables discovery only | registered 0x3BD729E9 / 0x154932F0 / 0x590A6FF4 / 0x4C1E75DB | T-020 |

Resolver anchors (not called, used to locate engine data): `IS_AUTO_AIMING_ON`, `IS_HUD_RETICULE_COMPLEX`, `GET_ROOT_CAM`, `IS_BULLET_IN_AREA`, `SET_GAME_CAM_PITCH`, `SET_GAME_CAM_HEADING`.

ScriptHookDotNet wrappers used: `Game.isKeyPressed`, `Game.isGameKeyPressed` (Aim, Crouch+LookBehind, Nav*), `Script.PerFrameDrawing`/`Graphics.DrawRectangle/DrawText`, `Player.CanControlCharacter`, `Ped.Weapons` (Select/Ammo/AmmoInClip/Slot), `World.CreatePed/CreateVehicle/GetGroundZ/GetNextPositionOnPavement`, `Game.CurrentCamera.Direction` (shot-audit cross-check). Windows `XInputGetState` (xinput1_4) reads the Steam Input virtual pad. T-003 evidence shows these work in game.

Do not infer CE behaviour from GTA V native databases or classic GTA IV offsets.

## T-021 holster streaming

| Native | Purpose | CE status | Task |
|---|---|---|---|
| `REQUEST_MODEL(uint32_t model)` | Begin asynchronous prop streaming on Script.Tick | registered 0x502B5185 | T-021 |
| `HAS_MODEL_LOADED(uint32_t model)` | Check the model before calling `World.CreateObject` | registered 0x4E61480A | T-021 |

Object creation, attachment and deletion use ScriptHookDotNet `World.CreateObject`, `GTA.Object.AttachToPed` and `GTA.Object.Delete`; placement uses `GTA.Bone` names from its source.

Same-process `ReloadScripts` cleanup checks journalled handles before creating props:

| Native | Purpose | CE status | Task |
|---|---|---|---|
| `DOES_OBJECT_EXIST(Object)` | Check prior prop handle | registered 0x6DAB78CD | T-021 |
| `GET_OBJECT_MODEL(Object, uint32_t*)` | Validate prior handle still names the same model | registered 0x5CC55619 | T-021 |
| `DELETE_OBJECT(Object*)` | Delete verified orphan from prior script domain | registered 0x62FE6290 | T-021 |

## T-013 debug hit overlay

| Native | Purpose | CE status | Task |
|---|---|---|---|
| `GET_CHAR_LAST_DAMAGE_BONE(Ped, uint32_t*)` | Report bone for the last damaged ped | registered 0x767E5013 | T-013 |
| `HAS_CAR_BEEN_DAMAGED_BY_CHAR(Vehicle, Ped)` | Attribute a vehicle health loss to the player | registered 0x61487DBF | T-013 |

`Ped.HasBeenDamagedBy(Ped)` wraps the registered `HAS_CHAR_BEEN_DAMAGED_BY_CHAR` (0x1DD624A0). `World.GetPeds` and `World.GetVehicles` enumerate nearby entities for debug display only.

## T-016 aiming cycle

ScriptHookDotNet `Ped.Weapons.FromType(...).isPresent` wraps `HAS_CHAR_GOT_WEAPON` (registered 0x11F759DE). `Ped.Weapons.Select(...)` wraps `SET_CURRENT_CHAR_WEAPON` (registered 0x6CF44DD6, third argument `true`) after checking ownership. The aiming cycle only selects IDs in `ArsenalRegistry.CarriedWeapons`.

## T-017 feel

ScriptHookDotNet `GTA.Camera.FOV` wraps registered `GET_CAM_FOV` and `SET_CAM_FOV` (0x55D470C2). T-017 applies its setter only on Script.Tick for a registered test weapon while aiming, and saves the original value for restore. Shake uses the already validated aim-camera fields under ADR-0004 after the recoil step.

T-021 raw calls are in `GameApi/HolsterNatives.cs`; T-013 raw calls are in `GameApi/DebugHitNatives.cs`. The earlier statement that all raw calls pass through `Natives.cs` predates this Agent B extension.

T-021 also uses ScriptHookDotNet `Game.CurrentEpisode` (registered `GET_CURRENT_EPISODE`, 0x7D7619D2) to select `TLAD/common/data/WeaponInfo.xml` or `TBoGT/common/data/WeaponInfo.xml` for episodic IDs 21–41. SHDN `Weapon.Slot` provides their category in the inventory fallback.

## T-022 combat effects

Reuses `GET_CHAR_LAST_DAMAGE_BONE` (registered 0x767E5013 via `DebugHitNatives`) and `GET_MISSION_FLAG` (registered 0x2BC64736) on Script.Tick. `Ped.HasBeenDamagedBy` is the existing SHDN wrapper; `World.GetPeds` enumerates only the configured nearby range. No new native or memory address is introduced. The GTA IV PedBone map is from [Sanny Builder's IV enum](https://github.com/sannybuilder/library/blob/master/gta_iv/enums.json); the correspondence to the CE last-damage-bone return remains an in-game test item.
