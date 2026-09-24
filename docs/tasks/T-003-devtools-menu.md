# T-003 — Minimum controller DevTools menu

Status: **NEEDS-PLAYTEST**. The first live build opened, navigated, and closed with the owner's controller, but D-pad navigation also opened the in-game phone. A revised local build awaits installation after GTA IV closes. T-003 scope remains menu access/navigation and read-only runtime/profile status; T-007 owns the weapon actions now shown in the same menu.

The menu uses the owner's chosen L3+R3 hold (700 ms) to open and close. It polls XInput controller indices 0–3 (using the first connected controller) and also checks ScriptHookDotNet's crouch/look-behind controls because the Steam Input community layout is unidentified. D-pad up/down or keyboard arrows select items. F10 toggles the menu as a keyboard fallback. The CONFIG view displays the active probe label. The revised build temporarily disables player controls while the menu is open and restores the previous controllable state on close, error, or script-domain unload. Its open log records whether a raw XInput controller was connected. This suppression is unverified in-game.

The first installed x86 build loaded, drew the panel, and logged open/close without errors. The owner reported that the controller menu checks passed, and also observed the phone opening during D-pad navigation. The revised x86 build completes with zero errors/warnings using the exact installed ScriptHookDotNet assembly as a reference; it has not replaced the DLL in the running game.

## Human test steps

1. Save and close GTA IV. From the repository root, run `./tools/build.ps1 -ScriptHookDotNetReference 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`, then `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`. The guarded installer preserves the previous DLL and config.
2. Launch GTA IV through Steam and load gameplay. Check `scripts\LibertyFramework\logs\LibertyFramework.log` for `devtools_started`. The game should still move and shoot normally with the menu closed.
3. Hold L3+R3 together for about one second. Expect a small `LIBERTY DEVTOOLS` panel. Release both sticks. If it does not open, press F10 on the keyboard and report whether that opens it.
4. With the menu open, press D-pad down and up once each. Expect the highlight to move without the phone opening or Niko moving. CONFIG should display the current probe label, normally `default`. If controller navigation fails, F10 closes the menu; keyboard arrows and Enter remain fallback controls.
5. Close with L3+R3 or F10. Confirm movement, aiming, and the phone work normally afterward. If control remains disabled, try F10 once; report it immediately and stop testing. The agent can restore the prior DLL after closing the game.
6. Briefly try opening/closing while aiming, in cover, and in a vehicle. Report any unwanted crouch, look-behind, weapon, phone, or pause action. The agent will inspect `controller_connected`, `player_control_locked`, and any error log line.
