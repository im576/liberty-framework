# T-022 — Combat effects detection and safe prototype

Status: **NEEDS-PLAYTEST**. Offline build and native registration verification pass. The implementation is a bounded hit/injury event prototype; visible wounds, physical per-region reactions, and rendered limb removal are **not implemented**. They require a CE-validated effect/animation path and original or permissioned assets. Do not treat a candidate log as dismemberment.

## Implemented

- Separate `CombatEffectsController` script, live `combat_effects.json`, master and per-feature toggles.
- Polls at a configured interval, only within radius/cap, only for registered test weapons 58–60, outside missions, and only for non-player peds. Attributes health loss to the player with the existing SHDN wrapper and uses the CE-registered last-damage-bone native.
- Maps GTA IV bone IDs to head, torso, left/right arm, left/right leg; unmapped bones stay unknown. Tracks per-ped region hits, bounded wound records, and a non-mutating limb-loss candidate flag. Removes state on despawn, range exit, disable, weapon switch, mission, or script reload.
- The engine's normal hit animation/blood remains active. No new engine animation, blood, model, memory, or asset API is called.

## Human test steps

1. With GTA IV **closed**, include `combat_effects.json` in the staged Phase 2 config and install the integrated DLL/config package. Keep its `enabled` field `false` for the baseline launch. Launch IV, use L3+R3 to give/select gold pistol 58, and confirm gameplay and stock hit reactions work.
2. While in gameplay, set `enabled` to `true` in the installed `scripts/LibertyFramework/config/combat_effects.json`. Wait two seconds and inspect `scripts/LibertyFramework/logs/LibertyFramework.log` for `combat_effects_config_loaded enabled=True`.
3. At the TEST RANGE, use gold pistol 58 to shoot a spawned non-player ped once in the torso, head, each arm and each leg (separate fresh targets if needed). Confirm `combat_reaction region=... bone=... damage=...` in the log. Record any `Unknown` bone value so the map can be corrected. Repeat with 59 and 60.
4. Fire two qualifying hits to one limb with `limbLossPrototypeEnabled=true`, while leaving other limits unchanged. Look for exactly one `combat_limb_loss_candidate` line for that limb. Confirm there is no model change; this is a detection safety test only.
5. Switch to a vanilla weapon, enter a mission, and then disable `enabled`. Confirm no further `combat_reaction` entries from those contexts. Trigger ScriptHookDotNet `ReloadScripts`; check fresh config-load entry and that gameplay continues. Attach log and any visual observations.

## Engine questions before visual implementation

- Confirm `GET_CHAR_LAST_DAMAGE_BONE` returns the documented GTA IV PedBone IDs (head 1205, etc.) on this CE build.
- Validate a CE-safe, bounded animation/force API for region-specific reactions without hijacking AI, missions or ragdolls.
- Validate wound position and particle/decal lifetime APIs, then create original wound materials and a collision-safe non-player limb replacement. No external gore code or commercial assets are included.

## Offline evidence

`tools/build.ps1`: 98 files, zero errors/warnings; SHA256 `099A12DED5698535FEEABC9DF8F8EB17FEC545956BFA6E4BD3A3C9764C24F06C` before documentation changes.
