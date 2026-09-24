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

Resolver anchors (not called, used to locate engine data): `IS_AUTO_AIMING_ON`, `IS_HUD_RETICULE_COMPLEX`, `GET_ROOT_CAM`, `IS_BULLET_IN_AREA`, `SET_GAME_CAM_PITCH`, `SET_GAME_CAM_HEADING`.

ScriptHookDotNet wrappers used: `Game.isKeyPressed`, `Game.isGameKeyPressed` (Aim, Crouch+LookBehind, Nav*), `Script.PerFrameDrawing`/`Graphics.DrawRectangle/DrawText`, `Player.CanControlCharacter`, `Ped.Weapons` (Select/Ammo/AmmoInClip/Slot), `World.CreatePed/CreateVehicle/GetGroundZ/GetNextPositionOnPavement`, `Game.CurrentCamera.Direction` (shot-audit cross-check). Windows `XInputGetState` (xinput1_4) reads the Steam Input virtual pad. T-003 evidence shows these work in game.

Do not infer CE behaviour from GTA V native databases or classic GTA IV offsets.
