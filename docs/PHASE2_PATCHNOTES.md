# Phase 2 integrated patch notes

## Performance pass (Claude), 2026-09-25

- **Less framework load (Vulkan and Violent Liberty stay):**
  - The aim script asks the engine for the camera far less often: it caches the handle and checks FOV only while aiming.
  - The gore script scans nearby NPCs at full speed only for 3 s after you fire; otherwise it scans every 0.4 s.
  - Each NPC now costs one engine call per scan instead of four or five.
- **Measurements:** every 30 s the log now shows what each Liberty Framework script costs (`performance_scripts`), plus a one-time `native_cost` line.
- **Startup crash finding:** the launch crashes happen inside Rockstar's launcher component (`MTLX.DLL`) before any mod loads. They happened on DirectX 9 and before mods too. Try opening the Rockstar Games Launcher first; if it still happens, turn off the Steam overlay for GTA IV.

## Gore pass 3 (Claude), 2026-09-24

- **Bodies leak:** wounds now pour continuous blood streams and drips (the game's looping blood effects, which the old one-shot calls could not play). Every wounded NPC also has the game's own bleeding switched on. Killed bodies keep leaking for 20 s. Strong hits spurt, and chunks fly on heavy hits and every cut.
- **Cut limbs and heads stay gone:** the cut is now applied inside the engine right after every skeleton pose update, so heads no longer flicker in and out.
- **Corpses no longer disappear when cut:** spawning the flying limb made the game clean up the corpse. Cut corpses are now kept.
- DevTools > Gore Test shows engine status and active blood loops.

## Gore fix after playtest (Claude), 2026-09-24

- **What the log showed:**
  - Severing did happen: the first arm came off and its limb was thrown.
  - Then a clothing lookup on the second thrown limb crashed. That switched dismemberment off and restored the corpses' limbs, so every later cut fell back to nothing.
  - The blood calls ran on every hit, but they were the same small spray the game already plays.
- **Fixes:**
  - A failed limb throw no longer turns dismemberment off (the clone just keeps default clothes).
  - Blood is much bigger (base scale 2.5, up to 4).
  - Every hit adds a mist; strong hits spurt for 3.5 s; the killing hit bursts.
  - Every cut adds chunks.
  - Bleed-out ticks from downed peds only drip, which stops the reaction/log spam.
- **New DevTools > Gore Test:** play every blood effect on the nearest NPC, with its name on screen. It can also kill the nearest NPC and cut the left arm, the right leg or the head.
- **Logging:** the log now records whether the engine accepted each effect (`ptfx ... spawned=True/False`).

## Gore overhaul (Claude), 2026-09-24

- **Blood you can see:** every firearm hit on any NPC, mission peds included, now sprays stock GTA IV blood sized by weapon.
  - Pistol/rifle: entry spray, plus exit spray on strong hits.
  - Shotgun and sniper: chunks.
  - Head and torso hits: blood from the mouth.
  - The old build passed an invalid particle size, so its blood was invisible.
- **Bleeding:** wounds keep dripping for 25 s.
- **Dismemberment:** killing shots to an arm or leg sever it at the elbow/knee or shoulder/hip. A killing headshot takes the head off.
  - Chunks and mist burst at the cut, the stump sprays for 9 s, and the limb flies off.
  - Works on any gun, and on peds who die a moment after the hit.
- **Engine fix:** the skeleton lookup that failed in your last session is fixed.
  - The limb removal is now re-applied every time the engine rebuilds a ragdoll (ADR-0005 hook), so a cut limb shouldn't pop back.
  - Hooks are removed on reload and exit.
- Config: `combat_effects.json` (thresholds, effect names, durations, `effectScale`). Set `dismembermentEnabled=false` to turn off all cutting.

## Open-items build (Claude), 2026-09-24

- **Shoulder swap (T-015):** press LB/L1 (or Z) while aiming to slide the camera to the other shoulder; press again to return. Found the aim camera's shoulder offset in the game's own camera settings table and flip it with an eased slide. Data-only write, validated and restored on exit/reload. Config `gunplay.json` → `shoulderSwap`.
- **Dismemberment (T-022):** lethal gold-weapon hits to an arm or leg sever it at the elbow/knee (lower hits) or shoulder/hip (upper hits): the limb is removed from the corpse, the stump bleeds and the limb is thrown off as a ragdoll with the victim's clothes. Self-checking; see the T-022 card for the log evidence to report. Corpse head removal continues as before.
- **Vehicle body-part labels (T-023):** LVS workshop Extras now name what each extra is (hood scoop, trunk spoiler, roof light/sign, bumper part…), read from your own vehicle model files: 80 cars, 295 extras. Unknown models keep "Extra N".
- Build: 110 sources, 0 errors/warnings; offline verification **296/296** (new: aim-camera table, ped-skeleton resolver, vehicle-extras catalog, limb cut plans). Dry-run install/rollback passed. Installed DLL SHA-256 `80E3193149A919FF083AAA8629A9D8B9245F4DEBFE4BBF6831237B1754B04648`; backup `scripts/LibertyFramework/backups/phase2-20260924-210021`; 12 installed files match the manifest.
- Not done: new body-kit **models** (needs asset work), wider weapon/attachment models, render-time hook for dismemberment if the engine rebuilds skeletons every frame (the log will tell).

## Follow-up build, 2026-09-24

- Extended the safehouse gunsmith from the pistol grip to catalog-defined tuning upgrades on all three registered gold weapons: carbine Stability stock ($600, per-shot bloom 0.82) and shotgun Steady fore-end ($450, 0.85). The same purchase validation, persistent physical record, and store/take behavior apply. No new visible model is included.
- Enabled the existing corpse-only head-removal native by default at the owner's request. It remains restricted to qualifying lethal head hits on nearby ambient NPCs with registered gold weapons. Arm/leg mesh removal is still unavailable.
- Build: 104 C# sources; offline verification: 270/270. Installed DLL SHA-256: `F055E2B7F45594C6F93A4866DE34CA9F10588619EE44D5732F40399A8CFE632C`. Installed 2026-09-24 with backup `scripts/LibertyFramework/backups/phase2-20260924-194809`. All follow-up gameplay requires an owner playtest.

Build installed 2026-09-24 for the owner's combined playtest. All new gameplay behavior is **NEEDS-PLAYTEST**; no agent ran GTA IV. The integrated DLL SHA-256 is `A0FB2B15B8814AC7A6CB7473AD7728B2B09F685CAA6001F04CD889C47FEC4F09`; 10 installed manifest files match. The latest backup is `scripts/LibertyFramework/backups/phase2-20260924-184410`.

## Gunplay

- Reduced early bloom for registered gold pistol 58, carbine 59, and shotgun 60. Added a configurable burst length, early-shot multiplier, reset pause, and faster short-burst recovery to all four presets. Long sprays still broaden toward each weapon's cap. Vanilla recoil/spread is unchanged.
- Retained the actual CWeaponInfo accuracy write and real-bullet shot audit. Reduced verbose per-bullet logging after the first 40 rounds; periodic summaries remain. In-game audit calibration and impact/crosshair alignment still need owner measurements.

## Arsenal and customization

- Added a nearby **Square/X or E** prompt and compact store/take panel at the rear of vehicles and at safehouse stashes. A selects, B closes; DevTools cannot cover the panel. Trunk doors and player control restore on close or error. The safehouse panel also exposes the pistol gunsmith.
- Each weapon record now has an instance ID, catalog ID, finish, attachment IDs, and progression value. Those values persist through carrying, trunks, safehouses, owned vehicles, and recovery after death. Legacy states are normalized on load; a same-type take is refused before changing either physical record.
- Added a catalog for the vanilla service pistol/combat pistol pair and existing custom gold weapons. At a safehouse, $500 buys the existing gold pistol finish once; factory/gold switching preserves instance ID and total ammo. A second gunsmith purchase fits a $350 Match grip to the gold pistol and multiplies its per-shot bloom by 0.75. Both choices appear in nearby safehouse storage. The grip persists with the physical instance through storage and reload. Other attachment slots and progression beyond these two examples have no game effect yet. Missing catalog or finish price leaves legacy Arsenal storage active.
- Existing Phase 1 gold weapon models, visible back holsters, trunk contents, and LVS ownership files are retained. No new weapon mesh or texture is shipped in this patch.

## Combat effects

- Added a separately configurable combat script for gold weapons against nearby ambient non-mission NPCs. It classifies damage bones into head, torso, arms, and legs; applies region-specific force; starts stock blood particles at the reported bone; tracks bounded wounds; and cleans up expired effects.
- Added optional, disabled-by-default limb-loss candidate logging and corpse-only head explosion. Actual arm/leg meshes are not removed. The stock particle name and force response are verified offline at the native-registration level and need visual playtest.

## Performance and reliability

- Added 30-second frame-interval p50/p95/p99 and gunplay tick average/maximum logging. Reduced holster scans to 50 ms, safehouse observation to 250 ms, temporary-trunk pruning to 1000 ms, and optional debug hit scans to the configured 100 ms with a 60 m radius. Cached crosshair projection while FOV and viewport stay steady.
- Combat sampling is capped by radius, ped count, wound handles, and interval; the integrated config uses 25 m, 16 peds, and 75 ms. Combat effects, holsters, and the debug overlay can be isolated during testing. No measured FPS gain is claimed before the owner benchmark.
- Added `tools/package-phase2.ps1`, `install-phase2.ps1`, and `rollback-phase2.ps1`. The package backs up changed files, verifies hashes and game version, merges new Arsenal defaults while preserving marked safehouses, and leaves save state, LVS INI, and Phase 1 gold assets intact.

## Open work

- T-015 shoulder swap: current reference uses camera obstruction by an invisible object; a safe CE offset/restore path has not been validated.
- T-023 real vehicle body variants: generic LVS Extras already previews/purchases/saves existing vehicle geometry; no specific model/slot has yet been visually verified as a body kit. No misleading relabel is installed.
- T-022 arm/leg dismemberment: validated detection and a head-only native spike exist; an original/permissioned limb mesh and safe hide/replace path still need research and an in-game visual spike.
- Wider attachment models/effects/camos/progression and replacement of every weapon need asset and game behavior passes after the first catalog/gunsmith examples.
