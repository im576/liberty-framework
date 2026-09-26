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

W-5 sling straps reuse the same streaming, `World.CreateObject` and `AttachToPed` (zero offset/rotation). The one-time `holster_frame` calibration log reads the prop's axes with ScriptHookDotNet `GTA.Object.GetOffsetPosition` (its IL calls the `GET_OFFSET_FROM_OBJECT_IN_WORLD_COORDS` wrapper) and the bone's world matrix through the engine's `CPed::CopyBoneMatrix` (`PedSkeleton.WorldMatrix`, already resolved for T-022). `Game.Resolution` (IL: `GetScreenResolution` native wrapper) is read only on script ticks, never in `PerFrameDrawing`.

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

T-021 also uses ScriptHookDotNet `Game.CurrentEpisode` (registered `GET_CURRENT_EPISODE`, 0x7D7619D2) to select `TLAD/common/data/WeaponInfo.xml` or `TBoGT/common/data/WeaponInfo.xml` for episodic IDs 21â€“41. SHDN `Weapon.Slot` provides their category in the inventory fallback.

## T-022 combat effects

Reuses `GET_CHAR_LAST_DAMAGE_BONE` (registered 0x767E5013 via `DebugHitNatives`) and `GET_MISSION_FLAG` (registered 0x2BC64736) on Script.Tick. `Ped.HasBeenDamagedBy` is the existing SHDN wrapper; `World.GetPeds` enumerates only the configured nearby range. No new memory address is introduced. The GTA IV PedBone map is from [Sanny Builder's IV enum](https://github.com/sannybuilder/library/blob/master/gta_iv/enums.json); the correspondence to the CE last-damage-bone return remains an in-game test item.

Follow-up calls through `CombatEffectsNatives` on Script.Tick: `IS_PED_A_MISSION_PED` (0x05801768) excludes script/mission NPCs; `APPLY_FORCE_TO_PED` (0x7305301D) supplies directional reaction force; `TRIGGER_PTFX_ON_PED_BONE` (0x7D3C3C9D) and `START_PTFX_ON_PED_BONE` (0x2209116C) use installed game effect names on the damaged bone; `STOP_PTFX` (0x0EAA4429) cleans tracked wound handles; `EXPLODE_CHAR_HEAD` (0x4A802E89) is optional, corpse-only and once per ped. All six are registered in CE 1.2.0.59 and mapped by the installed ScriptHook.dll (offline verifier 252/252). Signature references: [GTA IV native header](https://github.com/ckeleshi/GTA4.CHS/blob/master/natives.h.txt), [GTA IV PTFX reference](https://pastebin.com/KfLYm5F1). Runtime behavior remains unverified.

## Phase 2 follow-up (Claude, 2026-09-24)

| Native | Purpose | CE status | Task |
|---|---|---|---|
| `GET_CHAR_DRAWABLE_VARIATION` / `GET_CHAR_TEXTURE_VARIATION` / `SET_CHAR_COMPONENT_VARIATION` | copy a corpse's clothing onto the thrown-limb clone | registered 0x1A1A6D83 / 0x3A7B78C5 / 0x71A52973 | T-022 |
| `GET_PED_BONE_POSITION` | resolver anchor for ped skeleton access (not called) | registered 0x43475BB3 | T-022 |

The thrown limb uses SHDN `World.CreatePed`, `Ped.Visible`, `Ped.Die`, `Ped.NoLongerNeeded`, `Ped.Delete`; `START_PTFX_ON_PED_BONE`/`STOP_PTFX`/`APPLY_FORCE_TO_PED` as already registered for T-022.

**Gore overhaul finding (2026-09-24):** the last argument of TRIGGER_PTFX_ON_PED_BONE / START_PTFX_ON_PED_BONE is a **float** scale (the CE handler reads it with movss at 0xBD678D). Passing int 0 made every effect invisible. Arguments: name, ped, offset x/y/z, rotation x/y/z (degrees), bone, float scale. Blood effect names are checked offline against the installed gta_core.wpfl.

**Gore pass 3 (2026-09-24):**

- **Looping blood effects.** `TRIGGER_PTFX_ON_PED_BONE` refuses looping effects: the engine rejects rules whose duration is negative (0xAA0D6E). The playtest 2 log showed these refused: blood_gun_mist, blood_gun_chunks, blood_shotgun_chunks, blood_sniper_chunks, blood_artery, blood_artery_directional, blood_artery_mist, blood_bang_chunks and blood_drips.
- **Starting and stopping loops.** `START_PTFX_ON_PED_BONE` (0x2209116C, same arguments) accepts every effect and returns a script ptfx id; `STOP_PTFX` (0x0EAA4429) ends it. `CombatEffects/BloodEffects.cs` learns which effects loop from the first refusal.
- **Persistent corpses.** `SET_CHAR_AS_MISSION_CHAR` (0x60EC0540) keeps a severed corpse from being cleaned up when the limb clone is spawned. It is released with `MARK_CHAR_AS_NO_LONGER_NEEDED` (SHDN `Ped.NoLongerNeeded`).

The companion visual pass also keeps the thrown-limb clone mission-owned until its timed `Ped.Delete`, instead of calling `Ped.NoLongerNeeded` before the clone becomes visible. Its optional landing effect uses the already-registered `GET_GROUND_Z_FOR_3D_COORD` and `TRIGGER_PTFX_ON_PED_BONE`; no new native hash or address is introduced.
- **Stock bleeding.** `SET_CHAR_BLEEDING` (0x38330B4A) turns on the game's own bleeding for hit peds.
- **Hash source.** The hashes come from FusionFix natives.ixx and are verified as registered and mapped by ScriptHook.dll.

**Direct natives (T-026 step 2):** `GameApi/DirectNatives.cs` calls 18 read-only natives through their CE handlers on the script thread: IS_PLAYER_PLAYING, GET_PLAYER_ID, IS_CHAR_DUCKING, IS_PED_IN_COVER, IS_CHAR_IN_ANY_CAR, IS_CHAR_IN_AIR, GET_CHAR_SPEED, IS_PAUSE_MENU_ACTIVE, IS_SCREEN_FADED_OUT, IS_PLAYER_CONTROL_ON, GET_CAM_FOV, GET_CHAR_HEALTH, IS_CHAR_DEAD, DOES_CHAR_EXIST, GET_CURRENT_CHAR_WEAPON, GET_AMMO_IN_CLIP, GET_MAX_AMMO_IN_CLIP and HAS_CHAR_BEEN_DAMAGED_BY_CHAR.

- **ABI:** `handler(ctx)` cdecl; `ctx+0` points to the result, `+4` is the argument count, `+8` points to the argument array.
- **Safety:** each handler was scanned for script-thread state; the offline verifier pins each handler address; each is runtime-verified against SHDN.
- **Excluded:** GET_GAME_CAM and GET_PLAYER_CHAR, whose callees are encrypted on disk.

## ADR-0006 engine core (native/LibertyCore)

The core calls these natives directly through their CE handlers, once per frame, while the game thread is parked. Each one is accepted only after it matches ScriptHookDotNet on the player at startup (`engine_verify`). Hashes and signatures come from FusionFix natives.ixx, and tools/verify `EngineChecks` confirms each is registered in GTAIV.exe and agrees with `native-hashes.csv`.

`DOES_CHAR_EXIST`, `IS_CHAR_DEAD`, `GET_CHAR_HEALTH`, `GET_CHAR_ARMOUR`, `GET_CHAR_COORDINATES`, `GET_CHAR_HEADING`, `GET_CHAR_MODEL`, `IS_CHAR_IN_ANY_CAR`, `GET_CAR_CHAR_IS_USING`, `GET_CURRENT_CHAR_WEAPON`, `GET_AMMO_IN_CLIP`, `GET_CHAR_LAST_DAMAGE_BONE`, `HAS_CHAR_BEEN_DAMAGED_BY_CHAR`, `GET_PLAYER_ID`, `IS_PLAYER_PLAYING`, `IS_PLAYER_CONTROL_ON`, `IS_PAUSE_MENU_ACTIVE`, `IS_SCREEN_FADED_OUT`, `GET_GAME_TIMER`, `GET_HOURS_OF_DAY`, `GET_MINUTES_OF_DAY`, `GET_CURRENT_WEATHER`.

In game (2026-09-25): 22/22 handlers resolved and 21/21 verified; `GET_CAR_CHAR_IS_USING` is checked the first time the player is in a vehicle. The core costs about 10–40 µs per frame.

The autopilot test module (Testing/AutopilotModule) also calls these through ScriptHookDotNet: `SET_TIME_OF_DAY`, `FORCE_WEATHER_NOW`, `DISPLAY_HUD`, `DISPLAY_RADAR`, `CLEAR_WANTED_LEVEL`, `REQUEST_COLLISION_AT_POSN` and `LOAD_SCENE`.

## SDK 1.0 services (Claude, 2026-09-25)

`Engine/Services/*` call the natives below through ScriptHookDotNet. All are CE-registered and mapped by ScriptHook.dll (`native-hashes.csv`, verifier 690/690), and the verifier now also scans `NativeCall.*("NAME")`. Argument meanings follow FusionFix natives.ixx signatures and Sanny Builder's GTA IV library. Calls whose extra arguments are undocumented go through SHDN wrappers instead: shooting, driving, ragdoll, speech, cameras, blips, bone positions.

Natives used: peds (SET_CHAR_HEALTH, ADD_ARMOUR_TO_CHAR, SET_CHAR_INVINCIBLE, FREEZE_CHAR_POSITION, SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, SET_CHAR_ACCURACY, DELETE_CHAR, MARK_CHAR_AS_NO_LONGER_NEEDED), vehicles (DOES_VEHICLE_EXIST, GET/SET_CAR_COORDINATES/HEADING/HEALTH, GET_CAR_SPEED, GET/SET_ENGINE_HEALTH, GET_CAR_MODEL, GET_DRIVER_OF_CAR, GET_OFFSET_FROM_CAR_IN_WORLD_COORDS, OPEN/SHUT_CAR_DOOR, IS_VEHICLE_EXTRA_TURNED_ON, TURN_OFF_VEHICLE_EXTRA, CHANGE_CAR_COLOUR, FIX_CAR, DELETE_CAR, MARK_CAR_AS_NO_LONGER_NEEDED), props (GET/SET_OBJECT_COORDINATES, GET_OBJECT_HEADING, SET_OBJECT_ROTATION, SET_OBJECT_COLLISION, FREEZE_OBJECT_POSITION, ATTACH_OBJECT_TO_PED/CAR, DETACH_OBJECT, GET_OFFSET_FROM_OBJECT_IN_WORLD_COORDS), weapons (GIVE_WEAPON_TO_CHAR, REMOVE_WEAPON_FROM_CHAR, REMOVE_ALL_CHAR_WEAPONS, GET_AMMO_IN_CHAR_WEAPON, SET_CHAR_AMMO, GET_CHAR_WEAPON_IN_SLOT, GET_WEAPONTYPE_MODEL, GET_WEAPONTYPE_SLOT), tasks (CLEAR_CHAR_TASKS[_IMMEDIATELY], TASK_STAND_STILL, TASK_GO_STRAIGHT_TO_COORD, TASK_WANDER_STANDARD, TASK_TURN_CHAR_TO_FACE_COORD, TASK_LOOK_AT_COORD, TASK_SMART_FLEE_CHAR, TASK_COMBAT, TASK_AIM_GUN_AT_COORD, TASK_HANDS_UP, TASK_COWER, TASK_ENTER_CAR_AS_DRIVER/PASSENGER, TASK_LEAVE_ANY_CAR, TASK_PLAY_ANIM/_UPPER_BODY/_SECONDARY), animation (REQUEST_ANIMS, HAVE_ANIMS_LOADED, REMOVE_ANIMS, IS_CHAR_PLAYING_ANIM, GET_CHAR_ANIM_CURRENT_TIME), streaming (IS_MODEL_IN_CDIMAGE, MARK_MODEL_AS_NO_LONGER_NEEDED), FX (TRIGGER_PTFX, START_PTFX, START_PTFX_ON_VEH, STOP_PTFX; the last argument is a float scale, confirmed by disassembly: movss at +0x1C), audio (PLAY_SOUND_FRONTEND, GET_SOUND_ID, PLAY_SOUND_FROM_POSITION/PED/VEHICLE, STOP_SOUND, RELEASE_SOUND_ID, HAS_SOUND_FINISHED), player (STORE_SCORE, ADD_SCORE, STORE_WANTED_LEVEL, ALTER_WANTED_LEVEL, APPLY_WANTED_LEVEL_CHANGE_NOW, SET_PLAYER_CONTROL, SET_PLAYER_INVINCIBLE), world (SET_TIME_OF_DAY, FORCE_WEATHER[_NOW], RELEASE_WEATHER, SET_PED/CAR_DENSITY_MULTIPLIER, CLEAR_AREA, DISPLAY_HUD, DISPLAY_RADAR), mission and cutscene flags (GET_MISSION_FLAG, HAS_CUTSCENE_LOADED, HAS_CUTSCENE_FINISHED).

Measured facts:
- ATTACH_OBJECT_TO_PED/CAR rotations are in **radians**.
- GET_CAR_COORDINATES dereferences the entity matrix (+0x20) without a null check.
- GET_DRIVER_OF_CAR can fault on pooled vehicles, and a contained fault still corrupted game state, so the core reads the driver pointer from `[vehicle+0xF50]` instead.
- Core v2 adds the vehicle natives above to the direct set (8/8 verified in game).
