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
3. Carry vanilla pistol 7 at that safehouse. In SAFEHOUSE STORAGE, select **Buy gold finish ($500)** and press **A** twice. Expect exactly $500 charged, gold ID 58 visible, unchanged ammo and `instanceId`, and progression 1 in state. Select **Equip factory finish**; expect ID 7 with no charge. Select **Equip gold finish** again; expect ID 58 with no charge.
4. While carrying gold pistol 58 at the same safehouse, select **Buy Match grip ($350)** and press **A** twice. Expect exactly $350 charged and `match-grip` in the carried record's `attachments`. Fire four quick shots before and after fitting it, at the same stance and distance. The `gunplay_state` log should change to `attachment_bloom=0.75` after firing with the grip; the later shots and crosshair should spread less. Store and take the pistol, then restart GTA IV; the attachment and its effect should remain, with no second purchase row.
4a. Repeat with gold carbine 59 and **Stability stock ($600)**, then gold shotgun 60 and **Steady fore-end ($450)**. Expect `stability-stock` and `steady-fore-end` on their respective physical records; their `attachment_bloom` values are 0.82 and 0.85. Both should survive store/take and restart. These are tuning upgrades; their meshes do not change.
5. Try opening storage while in a car, during a mission, and during a fade. The panel should be unavailable. Reload scripts while the panel is open, then confirm normal controls return.

## 4. Combat consequences

1. In free roam, use a gold weapon against an ambient, non-mission NPC and test torso, head, arm, and leg hits on fresh targets. Look for distinct force reactions and local blood effects. Report `combat_reaction region=... bone=... damage=...` and any missing/unknown region or visible effect. Test Range spawned peds may be excluded as mission peds.
2. To test the bounded prototype, set `limbLossPrototypeEnabled=true` in installed `combat_effects.json`. Two qualifying hits on one limb should produce one `combat_limb_loss_candidate` line. Arm/leg geometry does not detach in this build.
3. The installed default has `headLossPrototypeEnabled=true`. Make a lethal head shot on an ambient NPC. Expect one `combat_head_loss` line and record whether the corpse changes visually. Turn the toggle off if it causes a visual or gameplay problem. Vanilla weapons, missions, the player, and mission peds should not receive the new effects.
4. If the effect loop causes stutter or wrong reactions, set `enabled=false` in `combat_effects.json` and confirm the feature stops without a game restart. Attach the log excerpt.

## 5. Ownership and vehicle research

1. Buy, store, take, and relaunch with one owned weapon in an LVS owned vehicle. Compare `instanceId`, ammo, finish, and the `lvs:<id>` bin in Arsenal state. Take the same weapon type while already carrying it; expect the action to refuse and preserve both records.
2. Wasted: owned carried weapons should reach the last safehouse. Busted: carried weapons should be lost. Check `instanceId` changes only when obtaining a different physical weapon. Use separate saves for these destructive tests.
3. LVS's existing workshop **Extras** remains unchanged. To unblock real body mods, bring a Sultan RS to the workshop and note which extra slot, if any, changes bumpers, skirts, spoiler, or other body geometry during preview. Test purchase, cancel, damage, trunk access, and reload of that exact slot. Report model and slot with screenshots. If no verified body slot exists, T-023 stays blocked.

## Known gaps to report separately

- Arm/leg mesh detachment is not implemented; only bounded candidate detection and a separate corpse head native are present.
- Shoulder swap T-015 remains blocked pending a validated Complete Edition camera offset.
- New attachment models and mechanical effects, weapon replacements beyond the existing catalog/finish example, and a real vehicle body-kit asset pipeline are not in this installed build.

## 6. Open-items build (2026-09-24)

1. **Shoulder swap:** T-015 card, steps 1–4 (LB/L1 or Z while aiming).
2. **Dismemberment:** T-022 card update box. Leg and arm kills with gold weapons; report `dismember_evidence` lines.
3. **Vehicle body labels:** T-023 card steps 1–3 (Sultan RS, taxi/cabby, police, Infernus).

## 7. Gore overhaul (2026-09-24)

1. Any gun, any NPC: shoot the torso, then the head, then a limb. Expect a clear blood spray per hit (bigger for shotgun/sniper, chunks on strong hits), mouth blood on head/torso hits, and dripping from the wound for ~25 s.
2. Kill an NPC with a shot to the lower leg, then another with a shot to the upper arm. Expect the limb gone at the knee / shoulder, a chunk burst, a pumping stump, and the limb flying off. Walk around the body for ~10 s: the limb must stay gone.
3. Kill one with a strong headshot (shotgun/sniper/rifle). Expect decapitation.
4. Run `ReloadScripts` once, then repeat step 2.
5. Send the log lines: `skeleton_hook_installed` (two), `dismemberment_ready ... hooks=...`, a few `combat_hit` / `combat_sever`, and every `dismember_evidence`. If the game freezes on a first dismemberment, set `dismembermentEnabled` to `false` in the installed `combat_effects.json` and report it.

### 7b. Gore Test page (after playtest 1)

1. Stand next to any NPC. Open DevTools (L3+R3), go to **Gore Test**, choose **Play every blood effect on nearest NPC**, then close the menu. A red caption names each effect every ~2 s. Note which names show visible blood and which don't.
2. **Kill nearest NPC and cut left arm**, then **cut right leg**, then **cut head**, each on a fresh NPC. Walk around the body.
3. Then shoot NPCs normally with any gun.
4. Send the lines `gore_test`, `ptfx`, `dismember`, `combat_sever`, `dismember_evidence`, and any `ERROR`.

### 7c. Gore pass 3

1. **Gore Test > Play every blood effect.** Every effect should now show something. Streams and chunks run for about 2 s each.
2. **Gore Test > cut left arm, cut right leg, cut head,** each on a fresh NPC. The body must stay, the part must stay gone (no flicker), and the stump pours blood. Walk around the body for 10 s.
3. **Normal combat.** Shoot NPCs with a pistol, a shotgun and a sniper. Expect streams from wounds, dripping bodies and chunks.
4. **Log lines to send:** `skeleton_update`, `skeleton_collapse_engine`, `dismember`, `dismember_evidence`, `dismember_corpse_lost`, `ptfx`, `gore_test`, `ERROR`.

### 7d. Violent Liberty companion visual test

1. Launch with the companion installed and Vulkan selected. Reach free roam. If the game fails before the menu, close it and run `tools/rollback-violent-liberty.ps1` using the backup path printed by the installer; report the startup symptom and any new DXVK/ASI logs.
2. Shoot an ambient NPC once in the torso, then with a fresh NPC finish an arm or leg with a firearm. Confirm a wound appears at the shot, blood flows over clothing and stains nearby surfaces, then a limb flies off and stays gone. Look closely at the stump: note whether it is pinched skin, a hole, or covered by blood. Walk around the corpse for 10 seconds.
3. Press **F6** to toggle Violent Liberty's visuals. Repeat a comparable torso and limb shot. The dynamic wound/surface stains should disappear, while the limb cut and its focused stump stream remain. Press **F6** again to restore visuals. F5 toggles streams/spray; F7 toggles inside-vehicle blood.
4. Send a short video or close screenshot of the stump and any overlap between the two mods. Include `combat_effects_config_loaded ... blood_visual_mode=external`, `combat_sever`, `dismember_evidence`, any `ERROR`, and a rough FPS comparison with F6 on/off. A prior DXVK-only attempt crashed, so record whether this build starts normally.

### 7e. Bleed and limb presentation pass

1. Launch the updated companion build and leave **F5** and **F6** on. In DevTools > Gore Test, choose **Start wound leak on nearest NPC** and close the menu; this checks the pulse path without aiming. Then shoot a fresh NPC once in the torso with a pistol, without killing them. Watch the wound for about 5 seconds: expect several small pulses that slow and fade while Violent Liberty keeps the stain on clothing/surfaces. Repeat with a head/neck shot and a shotgun body shot; the latter should run more often, and a fatal head/neck flow should last longer.
2. Finish a fresh NPC with an arm shot, then a leg shot on another. Expect an immediate blood burst, a roughly 10-second fading stump leak, and a visible thrown limb. Watch for one blood hit when the limb reaches the ground. Repeat several times; note any cut that does not throw, any limb that vanishes before landing, and whether a thrown limb is replaced when many corpses accumulate.
3. Press **F6** to compare the same shots with Violent Liberty's decals disabled. Our small pulses and stump leak should still show; the large dynamic stains and streaks should not. Press F6 again. If the blood is excessive or performance drops, record which weapon/scene and the `performance` lines.
4. Send `ptfx ... leak`, `dismember_limb_visible`, `dismember_limb_landed`, `dismember_limb_gave_up`, `dismember_limb_replaced_oldest`, `combat_sever`, and any `ERROR` lines, plus a short video of one standing bleed and one stump. The game, renderer, and visual overlap cannot be verified offline.

### 8. Performance pass (T-026)

1. **Launch.** Open the Rockstar Games Launcher and wait until it has signed in, then launch GTA IV from Steam. If it still crashes before the menu, note the time; next, try with the Steam overlay disabled for GTA IV.
2. **Walk.** Load the usual save and walk the usual street for 90 s without shooting.
3. **Fight.** Have a 60 s firefight (pistol and shotgun, a few kills).
4. **Report.** Say whether it feels smoother than before, and where it drops. Send the log lines `native_cost`, every `performance` line, and every `performance_scripts` line.
5. **Optional frame capture.** In an Administrator PowerShell, run `./tools/capture-performance.ps1 -Label perf-pass -Seconds 120` during steps 2-3.

### 9. Async DXVK and engine probe (T-026)

1. **Launch.** Start the Rockstar Games Launcher first, then GTA IV. The first minute may still compile a few shaders; after that, turning corners and entering new areas should hitch less.
2. **Play.** Same route as before: 90 s walking, then a 60 s fight. Say whether hitching and FPS changed, and whether Violent Liberty's blood still shows.
3. **Send:**
   - these LibertyFramework log lines: `engine_thread_probe`, `direct_native`, `performance`, `performance_scripts`;
   - the `DXVK mesh path:` line from `plugins\ViolentLiberty.log`;
   - the first line of `GTAIV_d3d9.log` (it should say `v2.6.2-1-gplasync`).
4. **If something breaks** (black screen, crash at start, missing blood), close the game and tell me; the rollback restores the previous DXVK exactly.

### 10. Limb rework and direct reads

1. **Normal fights.** Kill NPCs with arm and leg shots, including a few shotgun kills. Watch each limb: it should appear beside the body and lie on the ground, with no floating and no full-NPC flash. The body keeps its stump.
2. **Gore Test.** Use DevTools > Gore Test > cut left arm / right leg on fresh NPCs.
3. **Feel:** say whether it feels smoother than the last session.
4. **Send these log lines:**
   - `direct_natives verified=` (and any `direct_native_rejected`)
   - `dismember_limb_visible`, `dismember_limb_rehidden`, `dismember_limb_floating_removed`, `dismember_limb_landed`, `combat_sever_fallback`
   - `performance_scripts`
