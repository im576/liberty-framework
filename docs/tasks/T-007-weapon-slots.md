# T-007 — Separate gold weapon identifier spike

Status: **NEEDS-PLAYTEST**. T-001's runtime and post-install vanilla pistol checks passed. Scope: prove that FusionFix v5.0.1's custom weapon registration can create one separate pistol identifier without changing the vanilla pistol definition. Extend to three only if the first is stable. No finished gold art or custom gunplay.

The staged `LF_GOLD_PISTOL` entry copies this machine's base `PISTOL` data and uses the existing `w_glock` model; no external art is redistributed. The source data comes from the owner's installed game. `ExtendedLimits=1` is already set. The diagnostic script registers `LFWeaponStatus`, `LFWeaponGive`, and `LFWeaponVanilla` console commands. `LFWeaponGive` selects numeric ID 58, the first custom ID per FusionFix's [v5.0.1 source](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/v5.0.1/source/limits.ixx). That assignment is unverified until the game reports it. The guarded installer places the generated XML at `update/common/data/WeaponInfo.xml`, leaving the original file intact. A hash-checked removal script reverses the override.

The x86 build completed with zero errors/warnings and the staged XML parsed with one original PISTOL and one LF_GOLD_PISTOL. No game files have been changed while GTA IV runs.

## Human test steps (combine with T-003 after one shutdown)

1. Save and close GTA IV. Rebuild with `./tools/build.ps1 -ScriptHookDotNetReference 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`, deploy the DLL with `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`, then install the staged XML with `./tools/deploy-t007.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`. The agent performs this install after the owner reports the game closed.
2. Launch GTA IV through Steam. If it fails before gameplay, close it and have the agent run `remove-t007.ps1` while closed; report the startup log. Do not continue this spike after a startup failure.
3. In gameplay, first run T-003's menu checks. Then open the ScriptHookDotNet console with tilde and run `LFWeaponStatus`. The log should show a vanilla current ID and `candidate_present=False` before granting.
4. Run `LFWeaponGive`. Expect `current_id=58` and `candidate_present=True` in `scripts/LibertyFramework/logs/LibertyFramework.log`. The test pistol may look exactly like the vanilla pistol. If it does not select cleanly, stop the weapon checks and report the log.
5. Aim, fire, reload, use cover, and fire from a vehicle with the candidate. Run `LFWeaponVanilla` and `LFWeaponStatus` to see whether vanilla and custom entries coexist or replace each other in the handgun inventory. Save/load once and inspect the selected ID and presence flags again. Record any mission or death behavior only if naturally encountered; do not force a mission reset.
6. Report weapon status lines, gameplay behavior, and any crash or visual problem. The agent inspects logs and either marks the first identity proved or revises the route. The override stays a private diagnostic until the slot and asset path are validated.
