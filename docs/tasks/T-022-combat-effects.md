# T-022 — Combat effects detection and safe prototype

Status: **NEEDS-PLAYTEST**. Offline build and native registration verification pass. Physical reaction force, bone-attached stock blood PTFX, and a corpse-only head removal spike are implemented but unverified in game. Actual arm/leg mesh removal is **not implemented**; limb-loss candidate logs are diagnostic. The effect uses the player's installed GTA IV resources, with no copied third-party asset.

## Implemented

- Separate `CombatEffectsController` script, live `combat_effects.json`, master and per-feature toggles.
- Polls at a configured interval, only within radius/cap, only for registered test weapons 58–60, outside missions, and only for non-player, non-mission peds. Attributes health loss to the player with the existing SHDN wrapper and uses the CE-registered last-damage-bone native.
- Maps GTA IV bone IDs to head, torso, left/right arm, left/right leg; unmapped bones stay unknown. Tracks per-ped region hits, bounded wound records, and a non-mutating limb-loss candidate flag. Applies distinct configured directional force per region, a one-shot impact PTFX at the damaged bone, and a bounded attached wound PTFX. Stops handles on expiry, despawn, range exit, disable, weapon switch, mission, or script reload.
- Optional `headLossPrototypeEnabled` calls the game's own `EXPLODE_CHAR_HEAD` only after an attributed lethal head hit to a non-mission NPC, once per corpse. There is no arm/leg mesh removal. No new memory access or redistributed art.

## Human test steps

1. With GTA IV **closed**, include `combat_effects.json` in the staged Phase 2 config and install the integrated DLL/config package. Keep `enabled=false`, `headLossPrototypeEnabled=false` for the baseline launch. Launch IV, use L3+R3 to give/select gold pistol 58, and confirm gameplay and stock hit reactions work.
2. While in gameplay, set `enabled` to `true` in the installed `scripts/LibertyFramework/config/combat_effects.json`. Wait two seconds and inspect `scripts/LibertyFramework/logs/LibertyFramework.log` for `combat_effects_config_loaded enabled=True`.
3. In free roam, use gold pistol 58 to shoot an ambient non-mission NPC once in the torso, head, each arm and each leg (separate fresh targets if needed). Confirm distinct push directions/strengths and bone-local blood effects. Confirm `combat_reaction region=... bone=... damage=...` in the log; record any `Unknown` bone value or missing effect. Repeat with 59 and 60. Test Range spawned peds may be mission peds and are deliberately excluded.
4. Fire two qualifying hits to one limb with `limbLossPrototypeEnabled=true`. Look for exactly one `combat_limb_loss_candidate` line for that limb. Confirm there is no arm/leg model change; this is a detection safety test only.
5. Set `headLossPrototypeEnabled=true` and score an attributed lethal head hit on an ambient NPC with enough damage. Confirm one `combat_head_loss` line and whether the corpse head changes. Turn the toggle off after this test. If the game hangs or the model looks wrong, disable `enabled` and attach the log.
6. Switch to a vanilla weapon, enter a mission, and then disable `enabled`. Confirm no further `combat_reaction` entries from those contexts. Trigger ScriptHookDotNet `ReloadScripts`; check fresh config-load entry, wound PTFX stops, and gameplay continues. Attach log and visual observations.

## Engine questions before visual implementation

- Confirm `GET_CHAR_LAST_DAMAGE_BONE` returns the documented GTA IV PedBone IDs (head 1205, etc.) on this CE build.
- Verify that the CE-registered `APPLY_FORCE_TO_PED` produces the intended forces without disrupting AI, missions, or ragdolls. It has only offline signature and hash checks so far.
- Verify the stock `blood_stun_punch` PTFX can trigger/start/stop on damaged bones. If its START form is not persistent, replace it with an original decal/particle asset after obtaining a CE-safe path.
- A collision-safe arm/leg replacement and original stump meshes require an asset pipeline and game test. There is no validated native that hides individual limbs.

## Offline evidence

`tools/build.ps1`: 99 files, zero errors/warnings. `tools/verify.ps1`: 252/252, including CE handler registration and installed ScriptHook.dll name-to-hash mapping for the six newly called natives. In-game behavior awaits the owner's test.
