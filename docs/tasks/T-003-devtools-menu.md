# T-003 — Minimum controller DevTools menu

Status: **NEEDS-PLAYTEST**. T-002's config/logging probe passed its live test. Scope: one controller-accessible open/close action, category navigation, and a read-only debug item showing runtime/profile status. No weapon actions or tuning are included.

The first menu build uses the owner's chosen L3+R3 hold (700 ms) to open and close. It polls XInput controller indices 0–3 (using the first connected controller) and also checks ScriptHookDotNet's crouch/look-behind controls because the Steam Input community layout is unidentified. D-pad up/down or keyboard arrows select RUNTIME, CONFIG, and HELP. F10 toggles the menu as a keyboard fallback. The CONFIG view displays the active probe label; all views are read-only. An input or drawing exception logs `devtools_disabled` and stops the menu script without changing gameplay.

The C# build targeting x86 completed with zero errors and zero warnings. The exact installed ScriptHookDotNet assembly was used as the compiler reference. A standalone Windows XInput probe did not see a connected controller outside the game; that does not establish whether Steam Input exposes one inside GTA IV. The controller path and input conflicts remain unverified in-game. The DLL has not been installed because the game is running.

## Human test steps

1. Save and close GTA IV. From the repository root, run `./tools/build.ps1 -ScriptHookDotNetReference 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`, then `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`. The guarded installer preserves the previous DLL and config.
2. Launch GTA IV through Steam and load gameplay. Check `scripts\LibertyFramework\logs\LibertyFramework.log` for `devtools_started`. The game should still move and shoot normally with the menu closed.
3. Hold L3+R3 together for about one second. Expect a small `LIBERTY DEVTOOLS` panel. Release both sticks. If it does not open, press F10 on the keyboard and report whether that opens it.
4. With the menu open, press D-pad down and up once each. Expect the highlight to move among RUNTIME, CONFIG, and HELP. CONFIG should display the current probe label, normally `default`. If D-pad navigation fails, test the keyboard up/down arrows and report both results.
5. Hold L3+R3 again to close; F10 is the fallback. Check that normal movement and aiming still work. Briefly try opening/closing while aiming, in cover, in a vehicle, and from pause, noting any unwanted crouch, look-behind, weapon, phone, or pause action. Do not continue if a control conflict affects gameplay.
6. Report whether the panel appeared and navigated, whether gameplay continued, and any `devtools_disabled` or `devtools_controller_unavailable` log line. The agent will inspect the fresh log before changing code.
