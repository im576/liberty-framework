# Phase 2 combined owner playtest

Status: **READY FOR OWNER TEST** after the integrated package is installed. Use a free-roam save, back up that save before testing deaths or purchases, and keep `scripts/LibertyFramework/logs/LibertyFramework.log` for the report. GTA IV was not run by Codex.

## 1. Startup and performance baseline

1. Start GTA IV Complete Edition and load a free-roam save. Confirm the game and existing Phase 1 gold models/holsters still work. Check the log for `arsenal_ready`, `combat_effects_config_loaded enabled=True`, and `performance` after 30 seconds.
2. Drive the same route for 2 minutes, then fight for 2 minutes with the same resolution, graphics settings, and other mods. Record `performance` p50/p95/p99 frame interval and gunplay tick average/maximum for each segment. Those values are script tick timing; capture the same route with PresentMon if available for authoritative presented frames.
3. To isolate combat sampling, set `enabled=false` in `scripts/LibertyFramework/config/combat_effects.json`, repeat the firefight, then restore `true`. Toggle the DevTools debug overlay only for the final pass. Keep timestamps for visible stutters.

## 2. Gun feel

1. Through DevTools, give/select gold pistol 58, carbine 59, and shotgun 60. Fire paced single shots, a three-round burst, and a long spray from the same position. The short burst should stay tighter than the spray; the first shot should start at base spread. Compare impact grouping to crosshair gap and report weapon, distance, and context when bullets repeatedly land outside it.
2. Repeat carbine shots while moving, crouched, in cover, and from a vehicle. Confirm vanilla weapons still use vanilla recoil/spread and universal free aim/crosshair follows its toggle.
3. Send the `shot_audit_summary` and any `shot_audit_owner_mismatch` lines. The initial tune is provisional until those bullet traces are reviewed in game.

## 3. Storage and gunsmith

1. Stand at the rear of a parked car. When `Square / X or E  Open trunk` appears, press **Square/X** or **E**. Use **D-pad up/down**, **A** to store/take one weapon, and **B** to close. Check ammo, visible holster, boot animation, and normal phone/vehicle/cover controls after closing. Try F10/L3+R3 while the panel is open; DevTools should stay closed until the storage panel closes.
2. Repeat at a marked safehouse. If no safehouse stash exists, use DevTools **ARSENAL → Mark safehouse here** at the intended spot once. The nearby button should open SAFEHOUSE STORAGE. Store and take a weapon. Restart the game and check the same `instanceId` and ammo in `scripts/LibertyFramework/state/arsenal_iv.json`.
3. Carry vanilla pistol 7 at that safehouse. In SAFEHOUSE STORAGE, select **Buy gold finish ($500)** and press **A** twice. Expect exactly $500 charged, gold ID 58 visible, unchanged ammo and `instanceId`, and progression 1 in state. Select **Equip factory finish**; expect ID 7 with no charge. Select **Equip gold finish** again; expect ID 58 with no charge. Test storing and taking it once. Other attachment slots are metadata only in this build.
4. Try opening storage while in a car, during a mission, and during a fade. The panel should be unavailable. Reload scripts while the panel is open, then confirm normal controls return.

## 4. Combat consequences

1. In free roam, use a gold weapon against an ambient, non-mission NPC and test torso, head, arm, and leg hits on fresh targets. Look for distinct force reactions and local blood effects. Report `combat_reaction region=... bone=... damage=...` and any missing/unknown region or visible effect. Test Range spawned peds may be excluded as mission peds.
2. To test the bounded prototype, set `limbLossPrototypeEnabled=true` in installed `combat_effects.json`. Two qualifying hits on one limb should produce one `combat_limb_loss_candidate` line. Arm/leg geometry does not detach in this build.
3. For the separate head effect test, set `headLossPrototypeEnabled=true`, then make a lethal head shot on an ambient NPC. Expect one `combat_head_loss` line and record whether the corpse changes visually. Turn that toggle off after the check. Vanilla weapons, missions, the player, and mission peds should not receive the new effects.
4. If the effect loop causes stutter or wrong reactions, set `enabled=false` in `combat_effects.json` and confirm the feature stops without a game restart. Attach the log excerpt.

## 5. Ownership and vehicle research

1. Buy, store, take, and relaunch with one owned weapon in an LVS owned vehicle. Compare `instanceId`, ammo, finish, and the `lvs:<id>` bin in Arsenal state. Take the same weapon type while already carrying it; expect the action to refuse and preserve both records.
2. Wasted: owned carried weapons should reach the last safehouse. Busted: carried weapons should be lost. Check `instanceId` changes only when obtaining a different physical weapon. Use separate saves for these destructive tests.
3. LVS's existing workshop **Extras** remains unchanged. To unblock real body mods, bring a Sultan RS to the workshop and note which extra slot, if any, changes bumpers, skirts, spoiler, or other body geometry during preview. Test purchase, cancel, damage, trunk access, and reload of that exact slot. Report model and slot with screenshots. If no verified body slot exists, T-023 stays blocked.

## Known gaps to report separately

- Arm/leg mesh detachment is not implemented; only bounded candidate detection and a separate corpse head native are present.
- Shoulder swap T-015 remains blocked pending a validated Complete Edition camera offset.
- New attachment models and mechanical effects, weapon replacements beyond the existing catalog/finish example, and a real vehicle body-kit asset pipeline are not in this installed build.
