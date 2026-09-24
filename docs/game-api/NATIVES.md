# CE game API registry

No direct GTA native calls are implemented or verified. For each new call, agents must record the exact name/hash, signature, source link, ScriptHookDotNet wrapper alternative, CE 1.2.0.59 status, and the task/playtest that verified it.

| Native/wrapper | Purpose | CE status | Evidence | Task |
|---|---|---|---|---|
| None yet | — | untested | — | — |

T-003 uses these wrappers and one Windows API. Their signatures compile against the installed ScriptHookDotNet 1.7.1.8 assembly; behavior on this Steam Input layout still needs the T-003 playtest.

| API | Purpose | CE status | Evidence | Task |
|---|---|---|---|---|
| `GTA.Game.isKeyPressed(Keys)` | Keyboard F10 and arrow fallback | Compiled; in-game input pending | [Tomasak fork Game.h](https://github.com/Tomasak/gta4_scripthookdotnet/blob/master/ScriptHookDotNet/Game.h) | T-003 |
| `GTA.Game.isGameKeyPressed(GameKey)` | L3/R3 and D-pad fallback through game controls | Compiled; in-game mapping pending | [Tomasak fork Game.cpp](https://github.com/Tomasak/gta4_scripthookdotnet/blob/master/ScriptHookDotNet/Game.cpp) calls `IS_CONTROL_PRESSED`; no raw project native call | T-003 |
| `GTA.Script.PerFrameDrawing` and `GTA.Graphics` | Read-only menu rendering | Compiled; in-game drawing pending | [Tomasak fork drawing sample](https://github.com/Tomasak/gta4_scripthookdotnet/blob/master/TestScriptCS/Scripts/TestScripts.cs) | T-003 |
| Windows `XInputGetState(index, XINPUT_STATE*)` for indices 0–3 | Read Steam Input's virtual controller buttons | Compiled; no controller visible to standalone probe outside game; in-game pending | [Microsoft XInputGetState](https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate), [button masks](https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_gamepad) | T-003 |

Do not infer CE behavior from GTA V native databases or classic GTA IV offsets.
