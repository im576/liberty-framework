# T-022 — Combat effects detection and safe prototype

> **Update 2026-09-24 (Claude): arm and leg dismemberment implemented and installed — NEEDS-PLAYTEST.**
> A **lethal** hit with a gold weapon on an ambient NPC's arm or leg severs it: upper-arm/thigh hits cut at the shoulder/hip, forearm/hand/calf/foot hits at the elbow/knee (`CombatEffects/Logic/LimbCutPlan.cs`). The corpse's limb bones are collapsed into the joint every tick through the engine's own bone-matrix functions (`GameApi/PedSkeleton.cs`, resolved and verified in MEMORY.md), a blood effect starts on the stump, and a severed limb is thrown: a clone of the same ped wearing the same clothes with every other bone collapsed into the limb, ragdolled with an outward push. The thrown limb only spawns after the engine has been shown to keep the collapsed bones on that corpse, so a full-body clone is never shown. Before arming, the feature validates the ped pool and skeleton on the player's own ped. Limits: 6 severed corpses, corpses kept severed for 120 s, limbs removed after 60 s (`combat_effects.json`: `dismembermentEnabled`, `severedLimbEnabled`, `severedLimbForce`, lifetimes, `maximumSeveredPeds`, `stumpEffectName`).
> **Evidence to collect:** `dismemberment_ready`, `dismember limb=...`, `dismember_evidence persisted_ticks=N overwritten_ticks=M`, `dismember_limb_thrown`. If `overwritten_ticks` dominates, the engine rebuilds the skeleton each frame, so the limb will not visibly disappear and no limb is thrown. Report it; a render-time hook would then be the next step. Stumps are the collapsed mesh around the joint (no new stump model yet).
> Human test: kill ambient NPCs with a gold weapon shot to a leg, then an arm (e.g. aim low with the carbine). Expect the limb gone at the knee/elbow/hip/shoulder, blood at the stump and the limb flying off. Also shoot a corpse's other limbs, switch to a vanilla weapon (the limb must not come back), and run `ReloadScripts` (clones are dropped; corpses may regain limbs).

Status: **NEEDS-PLAYTEST**. Offline build and native registration verification pass. Physical reaction force, bone-attached stock blood PTFX, and a corpse-only head removal spike are implemented but unverified in game. The owner requested that corpse head removal be enabled in the next installed playtest build. Actual arm/leg mesh removal is **not implemented**; limb-loss candidate logs are diagnostic. The effect uses the player's installed GTA IV resources, with no copied third-party asset.

## Implemented

- Separate `CombatEffectsController` script, live `combat_effects.json`, master and per-feature toggles.
- Polls at a configured interval, only within radius/cap, only for registered test weapons 58–60, outside missions, and only for non-player, non-mission peds. Attributes health loss to the player with the existing SHDN wrapper and uses the CE-registered last-damage-bone native.
- Maps GTA IV bone IDs to head, torso, left/right arm, left/right leg; unmapped bones stay unknown. Tracks per-ped region hits, bounded wound records, and a non-mutating limb-loss candidate flag. Applies distinct configured directional force per region, a one-shot impact PTFX at the damaged bone, and a bounded attached wound PTFX. Stops handles on expiry, despawn, range exit, disable, weapon switch, mission, or script reload.
- Optional `headLossPrototypeEnabled` calls the game's own `EXPLODE_CHAR_HEAD` only after an attributed lethal head hit to a non-mission NPC, once per corpse. There is no arm/leg mesh removal. No new memory access or redistributed art.

## Human test steps

1. With GTA IV **closed**, include `combat_effects.json` in the staged Phase 2 config and install the integrated DLL/config package. The integrated package enables bounded reactions/blood for gold weapons, with `headLossPrototypeEnabled=false`. Launch IV, use L3+R3 to give/select gold pistol 58, and confirm gameplay and stock hit reactions work. Look for `combat_effects_config_loaded enabled=True` in `scripts/LibertyFramework/logs/LibertyFramework.log`.
2. If the effects cause a problem, set `enabled` to `false` in the installed `scripts/LibertyFramework/config/combat_effects.json`; the script should stop its tracked PTFX within two seconds. Re-enable for the checks below.
3. In free roam, use gold pistol 58 to shoot an ambient non-mission NPC once in the torso, head, each arm and each leg (separate fresh targets if needed). Confirm distinct push directions/strengths and bone-local blood effects. Confirm `combat_reaction region=... bone=... damage=...` in the log; record any `Unknown` bone value or missing effect. Repeat with 59 and 60. Test Range spawned peds may be mission peds and are deliberately excluded.
4. Fire two qualifying hits to one limb with `limbLossPrototypeEnabled=true`. Look for exactly one `combat_limb_loss_candidate` line for that limb. Confirm there is no arm/leg model change; this is a detection safety test only.
5. With the installed default `headLossPrototypeEnabled=true`, score an attributed lethal head hit on an ambient NPC with enough damage. Confirm one `combat_head_loss` line and whether the corpse head changes. If the game hangs or the model looks wrong, set `headLossPrototypeEnabled=false` in the installed config and attach the log.
6. Switch to a vanilla weapon, enter a mission, and then disable `enabled`. Confirm no further `combat_reaction` entries from those contexts. Trigger ScriptHookDotNet `ReloadScripts`; check fresh config-load entry, wound PTFX stops, and gameplay continues. Attach log and visual observations.

## Engine questions before visual implementation

- Confirm `GET_CHAR_LAST_DAMAGE_BONE` returns the documented GTA IV PedBone IDs (head 1205, etc.) on this CE build.
- Verify that the CE-registered `APPLY_FORCE_TO_PED` produces the intended forces without disrupting AI, missions, or ragdolls. It has only offline signature and hash checks so far.
- Verify the stock `blood_stun_punch` PTFX can trigger/start/stop on damaged bones. If its START form is not persistent, replace it with an original decal/particle asset after obtaining a CE-safe path.
- A collision-safe arm/leg replacement and original stump meshes require an asset pipeline and game test. There is no validated native that hides individual limbs.

## Offline evidence

`tools/build.ps1`: 99 files, zero errors/warnings. `tools/verify.ps1`: 252/252, including CE handler registration and installed ScriptHook.dll name-to-hash mapping for the six newly called natives. In-game behavior awaits the owner's test.
