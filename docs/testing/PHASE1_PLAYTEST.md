# Phase 1 playtest (T-010) — one session

Controller names: Cross = A, Circle = B. DevTools: hold **L3+R3** (or **F10**). In the menu: D-pad up/down move, **D-pad left/right change values**, Cross select, Circle back (Circle on the top page closes). "Give" actions ask for a second Cross press.

## 0. Install (game closed)

1. Save and quit GTA IV completely.
2. Back up: copy `...\GTAIV\scripts` and `...\GTAIV\update` to a safe folder.
3. In PowerShell from the repo folder: `.\tools\install-phase1.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`. It refuses to run while the game is open, checks the files it was built from, backs up everything it replaces under `scripts\LibertyFramework\backups\phase1-<time>\`, and verifies hashes. Expect "Phase 1 installed and verified".

## 1. Startup (log: `scripts\LibertyFramework\logs\LibertyFramework.log`)

1. Launch through Steam, load your save. Expect normal loading, no crash. **If the game crashes at startup, go to Rollback.**
2. Walk for a few seconds. (Log proof: `gunplay_initialized prefs=True lockon=True hud=True aimcam=True weaponinfo=True bullets=True` and `weaponinfo_validated 58:2.6/2.6 ...`.)

## 2. DevTools and controller

1. Hold L3+R3: "LIBERTY DEVTOOLS" opens. D-pad up/down through all 8 pages — **the phone must not open**.
2. Enter a page with Cross, return with Circle; close with L3+R3, reopen, close with F10. Walking/aiming work normally after closing.

## 3. Test range

1. DevTools > TELEPORT > Gun Test Range (Broker). You land on a pavement near Hove Beach station. If the spot is bad, walk somewhere open with a wall in front and use TELEPORT > Set Gun Test Range here (Cross twice); use TEST RANGE > Teleport to Gun Test Range to confirm it returns you there.
2. Face a wall or open street, TEST RANGE > Spawn target lane: frozen peds at 5/10/25/50 m and a car. Clear wanted level / Restore health from the same page when needed.

## 4. Free aim, reticle, health ring (vanilla pistol first)

1. WEAPONS > Give vanilla pistol. Aim (L2) at a target ped: **no snap, no lock-on when pulling L2 fully, no health/armour ring**. The vanilla reticle is gone; a thin white four-line crosshair shows instead, and only while aiming.
2. Pause menu > Settings > Controls: Auto-Aim shows **Off**. Unpause.
3. GUNPLAY > Free aim: OFF. Aim again: lock-on/snap and the health ring return; Auto-Aim in Settings is back to what you had. Turn Free aim ON again.
4. GUNPLAY > Custom crosshair: OFF → vanilla reticle returns (and only it). Turn it ON again.

## 5. Gold pistol (ID 58)

1. WEAPONS > Give Gold Pistol. **The pistol in hand is gold; the HUD weapon icon is gold.** Give vanilla pistol: normal black pistol. Back to Gold Pistol.
2. At the 10 m target, fire single shots with pauses: camera kicks up ~1.6° and mostly returns; first shot lands at the crosshair centre.
3. Fire as fast as possible: the crosshair gap opens with each shot and impacts spread wider; stop and the gap closes within ~2 s.
4. Shoot 10+ rounds at a wall: bullet holes stay inside the crosshair gap. (Log proof: `shot_audit ... dev=... cone=...` and `shot_audit_summary ... inside_cone=NN%`.)

## 6. Gold carbine (ID 59) and gold shotgun (ID 60)

1. Give Gold Carbine. Short bursts (3–4 rounds) stay controllable; a long burst climbs and needs you to pull down; recovery is slower than the pistol. Crosshair grows during the burst.
2. Give Gold Shotgun. Strong kick per shot, slower recovery; after two shots the crosshair gap covers the pellet pattern.
3. Vanilla carbine/shotgun (WEAPONS page): no LF kick; spread unchanged from vanilla (A/B comparison).

## 7. Movement, cover, vehicle

1. Gold carbine: fire while standing, crouched, and running — running widens the crosshair; crouched tightens it.
2. Take cover (RB/R1), pop out and aim: kick and crosshair work. Blind-fire from cover (fire without aiming): no crosshair; shots scatter more.
3. Get in a car with the gold pistol, drive-by aim and fire. Report whether the camera kicks and whether a crosshair shows (vehicle kick depends on the aim camera being used there; spread still applies).

## 8. Live tuning, presets, config

1. LIVE TUNING: first row selects the weapon (left/right). Set Gold Pistol "R verticalKickDegrees" to 4.0 → next shots kick much harder. Set "S baseDegrees" to 2.0 → wider crosshair and wider impacts.
2. LIVE TUNING > Revert: reload gunplay.json (Cross twice) → original feel.
3. PRESETS & CONFIG: Load preset C (Mafia heavy) → heavier; then A. "Save live values as preset 'user_saved'" (Cross twice) → message confirms.
4. "Reload config from disk" → "Reloaded gunplay.json".

## 9. Inspect state

INSPECT STATE (or GUNPLAY > Debug overlay ON while shooting). Note: Camera `validated`, `weaponinfo=True`, Audit `inside_cone=` percentage for each gold weapon, FreeAim `auto_aim_pref=0 (was N)`.

## 10. Save/load

With the Gold Pistol in hand, save at a safehouse and reload: gold pistol still gold and still ID 58; free aim, crosshair and kick still work. Exit the game from the pause menu.

## Report

For each numbered section: pass / fail / notes. Attach `scripts\LibertyFramework\logs\LibertyFramework.log` (and `.1.log` if present).

## Rollback (game closed)

`.\tools\rollback-phase1.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'` restores every replaced file from the newest phase1 backup and removes added files. If the script cannot run, copy back the `scripts` and `update` folders you saved in step 0. Afterwards check Settings > Controls > Auto-Aim once (LF restores it automatically, but a crash during the session could have left it Off).
