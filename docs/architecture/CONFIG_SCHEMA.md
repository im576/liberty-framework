# Configuration contract

All files are JSON read with `DataContractJsonSerializer`; every field is required (a missing field rejects the file). Invalid edits are logged and the last valid config stays active. Runtime copies live in `scripts/LibertyFramework/config/`.

## probe.json (T-002)

`{"schemaVersion": 1, "probeLabel": "default"}` — label only; no gameplay effect.

## gunplay.json (T-010)

Polled every second; a valid change is applied live. DevTools "Save live values" rewrites it (previous copy kept as `gunplay.json.bak`).

| Section / field | Unit | Meaning |
|---|---|---|
| `freeAim.enabledOnStartup` | bool | start with universal free aim on |
| `freeAim.profile` | `vanilla` / `free` | config-selected aim mode; `slowdown` and `light` are rejected until a verified CE control exists |
| `freeAim.disableLockOn` / `forceAutoAimOff` / `hideTargetHealth` | bool | lock-on native, menu Auto-Aim pref, health/armour ring |
| `crosshair.replaceVanillaReticle` | bool | hide vanilla reticle and draw the LF crosshair |
| `crosshair.showForVanillaWeapons` | bool | LF crosshair also on non-test guns (static, from their own accuracy) |
| `crosshair.lineLengthPixels`, `lineThicknessPixels`, `outlinePixels` | px | segment look |
| `crosshair.minimumGapPixels` / `maximumGapPixels` | px | display clamp of the spread gap |
| `crosshair.colorArgb`, `outlineArgb` | 0–255 ×4 | colours |
| `crosshair.gapSmoothingPerSecond` | 1/s | closing smoothing (opening is immediate) |
| `crosshair.fovAxis` | `vertical`/`horizontal` | how camera FOV maps to screen pixels |
| `spreadCalibration.tangentPerAccuracyUnit` | tan/unit | muzzle deviation per CWeaponInfo accuracy unit (0.02 × 0.65 × 0.2 from disassembly) |
| `spreadCalibration.autoCalibrate`, `sampleWindow`, `gainAdjustRate`, `minimumGain`, `maximumGain` | — | closed-loop gain from measured bullet traces |
| `spreadCalibration.minimumSampleSpreadDegrees`, `maximumMeasurableDeviationDegrees` | deg | ignore samples outside this band |
| `spreadCalibration.minimumAccuracyValue` | accuracy | floor for the written value |
| `recoilGlobal.enableCameraKick`, `allowWithRealRecoil` | bool | kick master switch; refuse to double-kick with Real Recoil |
| `recoilGlobal.recoveryCancelStickThreshold` | 0–1 stick | right-stick deflection that hands recovery to the player |
| `recoilGlobal.cameraValidationToleranceDegrees`, `cameraValidationSamples` | deg, count | aim-field validation before the first write |
| `recoilGlobal.maximumDeltaSeconds` | s | frame-time clamp |
| `feel.enabled`, `shakePitchDegrees`, `shakeHeadingDegrees` | bool, deg | registered test-weapon per-shot jitter applied after recoil through the validated aim camera |
| `feel.aimFovReductionDegrees`, `fovSmoothingPerSecond` | deg, 1/s | test-weapon aiming field-of-view reduction and easing |
| `debugHit.scanRadiusMeters`, `scanIntervalMilliseconds`, `worldClassificationDelayMilliseconds` | m, ms, ms | nearby entity scan radius and cadence, and delay before a non-damaging shot is shown as world/unknown |
| `switchWhileAiming.enabled`, `previousButton`, `nextButton` | bool, `DPadLeft` / `DPadRight` | select only T-020 carried weapons via SHDN while aim is held; controller bindings must differ |
| `movement.movingSpeedThresholdMetersPerSecond`, `fullMovementPenaltySpeedMetersPerSecond` | m/s | movement penalty ramp |
| `weapons[]` | — | registered test weapons (IDs 58+ only); see below |
| `tuning[]` | key, step, min, max | parameters exposed in DevTools Live Tuning |
| `testRange.*` | m, rounds, hp | target lane distances, vehicle offset, ammo/health/armour refills |

`weapons[]`: `weaponId`, `vanillaWeaponId`, `weaponInfoName` (must match WeaponInfo.xml), `label`, `profileName`, `finish`, `initialAmmo`, `calibrationSource` (use this weapon's bullets to calibrate), `recoil`, `spread`, and (T-041) optional `catalogId` for a Stage 1 catalog weapon (see "Stage 1 arsenal" below).

`recoil` (degrees of aim camera, ms): `verticalKickDegrees`, `horizontalKickDegrees` (bias, + right), `horizontalRandomDegrees` (±), `firstShotMultiplier`, `sustainedFireGrowthPerShot`, `sustainedFireShotCap`, `chainResetMilliseconds`, `maxAccumulatedDegrees` (soft cap), `minimumKickFractionAtCap`, `kickDurationMilliseconds`, `recoveryDelayMilliseconds`, `recoveryDegreesPerSecond`, `recoveryFraction` (0–1 of each kick auto-recovered), and multipliers `moving`, `crouched`, `cover`, `vehicle`, `hipFire`.

`spread` (cone half-angle in degrees from the muzzle): `baseDegrees`, `perShotDegrees`, `burstShotCount` (early shots given a reduced bloom increment), `burstPerShotMultiplier` (0–1 early increment), `chainResetMilliseconds` (pause that starts a new burst), `maxDegrees`, `recoveryDelayMilliseconds`, `recoveryDegreesPerSecond` (long spray), `shortBurstRecoveryDegreesPerSecond` (taps and short bursts), `movingAddDegrees` (at full movement), multipliers `crouched`, `cover`, `vehicle`, `hipFire`, `blindFire`, `airborne`, and `pelletPatternDegrees` (display-only allowance for multi-pellet weapons until 16 bullets are measured). Both recovery rates start after the delay; the rate is chosen by burst length.

## presets/*.json

`{"schemaVersion":1,"name","description","weapons":[{"weaponId","profileName","recoil","spread"}]}` — replaces recoil/spread for listed weapons. Shipped: A GTA IV+, B Mafia Light, C Mafia Heavy, D Experimental (generated by `tools/generate-presets.ps1`). "Save as preset" writes `presets/user_saved.json`.

## devtools/locations.json

`{"schemaVersion":1,"locations":[{"id","name","x","y","z","heading","snap","note"}]}`. `snap`: `none` (exact), `pavement` (nearest pavement node), `ground` (ground below z). `gun_test_range` is used by Test Range and is overwritten by "Set Gun Test Range here".

**Stage 1 capture points (T-040).** The ids starting `s1_` are the 12 fixed capture points every Stage 1 visual and performance comparison uses (STAGE1 section 10): 8 in Broker/Dukes (`s1_hove_beach_a`, `s1_hove_beach_b`, `s1_firefly_island`, `s1_rotterdam_hill`, `s1_boabo`, `s1_east_hook`, `s1_dukes_meadow_hills`, `s1_east_island_city`) and 4 elsewhere (`s1_algonquin_star_junction`, `s1_algonquin_chinatown`, `s1_bohan_boulevard`, `s1_alderney_city`), all `snap: "none"`. Coordinates come from the game's own data (vehicle nodes in `common/data/maps/paths.ipl`, zones in `common/data/info.zon`), z is the road surface plus 1 m, heading looks down the road; each `note` names the zone and node. `tools/perf/New-Stage1Scenarios.ps1` builds the `stage1-*` scenarios from these entries in file order (first 8 = Broker/Dukes); moving or adding a point means rerunning it.

## Build-time: assets/finishes/finishes.json

## Arsenal: arsenal.json (T-020)

Required `schemaVersion:1`; `sidearmLimit:2`, `longGunLimit:2`, `meleeLimit:1`. `purchaseWindowMilliseconds` is the maximum elapsed time from money decrease to weapon gain for purchase ownership. `trunkDistanceMeters` is interaction distance from the boot; `trunkRearOffsetMeters` locates the boot behind the vehicle (SHDN vehicle local Y points forward). `ownedVehicleMatchMeters` matches LVS model hash and position; `fallbackVehicleMatchMeters` matches Arsenal's remembered vehicle marker. All distances are meters. `categories[]` maps each `WeaponCategory` enum number to `group` (`sidearm`, `longGun`, `melee`, `uncounted`) and `BodySlot` enum number. `safehouses[]` has `id`, `name`, `episode` (`iv`, `tlad`, `tbogt`), world `x/y/z`, `radius` meters, and `verified` boolean. The shipped list is empty because no sourced coordinates were provided; DevTools **Mark safehouse here** records the actual player coordinate, adds a verified entry, and writes `arsenal.json` with a backup.

**Stage 1 loadout (T-044).** Optional block `loadout` (absent in files that predate it; `package-phase2.ps1` adds it to an installed file without touching the owner's other values): `enabled` (false restores the general limits and category groups above), `sidearmLimit` (1-2) and `longGunLimit` (1-4) replace the general limits while enabled, `categoryOverrides[]` (same shape as `categories[]`, SMG may be `sidearm` or `longGun`; shipped: SMG → `longGun`, body slot 3, so an Uzi or MP5 is slung with the long guns and the one sidearm slot is the handgun), and `ammoCaps[]` (`category` 2-7, `maximumRounds` 1-9999): rounds above the cap are removed from the weapon in free play (not during missions or cutscenes; logged as `arsenal_ammo_capped`). Melee and thrown weapons are never capped and keep the general rules. Shipped caps (proposals): handgun 150, shotgun 60, SMG 240, rifle 240, sniper 40, heavy 12. The overflow rule is unchanged: the least recently used weapon of a group over its limit goes to the last car's trunk or the last safehouse stash.

**Weapon wheel (T-045).** Optional block `weaponWheel` (an install that predates it gets the block from packaging, and the defaults below apply without it): `enabled`, `padButton` (a `PadButton` name, default `Back`) and `keyboardKey` (a `VirtualKey` name, default `Tab`) are held to open the wheel on foot; `tapMilliseconds` (50-1000, default 250): a press shorter than this keeps the wheel open for stick/arrows and A/Enter, a longer hold equips the highlighted slot on release (elapsed time is measured with the monotonic tick clock; presses wholly inside an unsampled stall cannot be observed); `allowInVehicle` (default false). Navigation and storage actions (A/B/X/Y, shoulders, D-pad, Enter, Escape, Backspace, arrows, Space, G, E, Page Up/Down) are rejected as bindings. The wheel shows sidearm, two long guns, melee and thrown (`Arsenal/Logic/WeaponWheelLogic.cs`). The defaults are not checked against the game's own on-foot mapping: the owner confirms the Back button and Tab do nothing else.

**Trunk interface (T-046).** Optional block `trunkCapacity` (without it every trunk holds 8 and stashes are unlimited): `defaultSlots` (1-99), `safehouseSlots` (0 = unlimited) and `classes[]` (`id`, `slots` 1-99, `models[]` game model names, each in one class only; shipped: `sports` 4, `utility` 16). A full container refuses a store (`arsenal_store_refused`) but a take that swaps one weapon out for one in is allowed; unknown model names are logged once (`arsenal_trunk_class_unknown_model`). A take swaps out the carried weapon of the same category, else the least recently used carried weapon of the incoming weapon's group when that group is at its limit.

Phase 2 contextual storage uses the existing `trunkDistanceMeters` for both the vehicle rear and a capped safehouse prompt radius. Controller **X** or keyboard **E** opens the storage panel; **X/Space** stores a carried selection, **A/Enter** takes/swaps the highlighted stored weapon, **LB/RB or Page Up/Down** moves the storage cursor, and **B/Backspace** closes. This binding is fixed for the first playtest and should be exposed in config after control-conflict feedback. Nearby vehicle searches run every 250 ms.

The `performance` log line every 30 seconds records ScriptHook tick interval p50/p95/p99, counts over 33/50 ms, and gunplay-loop average/maximum CPU time. Tick intervals are a frame pacing proxy; use PresentMon for final presented-frame comparisons.

Per-episode `state/arsenal_<episode>.json` contains owned-carried IDs, persistent vehicle trunk bins, safehouse stash bins, last safehouse, and last vehicle marker. `JsonStore.Save` writes atomically and maintains `.bak`. A corrupt state file is renamed to `.corrupt_<UTC>` without changing the existing `.bak`, then state starts empty.

Finish colour ramps (`shadowRgb`, `midRgb`, `highlightRgb`, `contrast`, `lift`, `gamma`) per texture role (`diffuse`, `specular`, `icon`) and model variants (`variantModel`, `baseModel`, `sourceImg`, `finish`, `animGroup`, `drawDistance`, `audioMaterial`, `weaponInfoType`, `textureRoles`). Add a finish or a weapon variant here and rerun `tools/archive/package-phase1.ps1`.

## holsters.json (T-021)

`schemaVersion: 1`, `enabled`, `showOnBikes`, `nudgePositionMeters`, `nudgeRotationDegrees`, `weapons[]` (`weaponId`, `weaponInfoType`, `category` numeric `WeaponCategory`), and `placements[]` (`slot`, ScriptHookDotNet `bone`, `position` in meters XYZ, `rotation` in degrees XYZ). A placement can optionally specify `category` and/or `model` to override a slot default. `weaponInfoType` selects the active WeaponInfo.xml entry, whose `<assets model>` determines the prop. The starting offsets require visual calibration in game; DevTools saves changes with a `.bak`.

Optional `outfitClasses[]` and `loadoutPlacements[]` (T-044). An outfit class is `id` (unique; `default` is the implicit class for every outfit not listed), `component` (0-10, the ped component whose drawable decides) and `drawables[]` (the drawable numbers of that component, `GET_CHAR_DRAWABLE_VARIATION`, that belong to the class); the first listed class that matches wins and is re-read twice a second. `loadoutPlacements[]` has the same fields as `placements[]` plus an optional `outfit` (a class id); the most specific match wins: outfit, then `model`, then `category`, then the slot default. Because these are blocks of their own, an installed `holsters.json` that predates them still loads and packaging adds them.

Optional `slings[]` (W-5): `slot` (`LongGun1` or `LongGun2`, unique), `model` (a strap model from `LibertyModels.img`) and `bone` (ScriptHookDotNet bone). The strap is attached with zero offset and zero rotation because its mesh is authored in that bone's bind-pose space. Packaging now merges missing top-level fields into the installed file (`merge-defaults`), so saved nudges survive.

## engine.json (ADR-0006)

| Field | Meaning |
|---|---|
| `schemaVersion` | Always `1`. |
| `coreEnabled` | Load LibertyCore.dll. When off, the world snapshot comes from the SHDN fallback (player only). |
| `pedRadiusMeters` | Peds listed in the snapshot and reported in events (10–500). |
| `coreSpotCheckFrames` | How often the core's player position is compared with SHDN (at least 30). |
| `coreSpotCheckToleranceMeters` | Allowed drift before the core is switched off (0–5). |
| `disabledModules` | Module ids that are not constructed. |
| `loadModAssemblies` | Also load `[Module]` classes from `scripts\LibertyFramework\mods\*.dll` (Liberty SDK mods). |
| `vehicleRadiusMeters` | Optional (150). Vehicles listed in the snapshot and reported in events (10–500). |
| `bulletEvents` | Optional (true). Read the game's bullet trace list each frame and publish `BulletFired`. |
| `exactDamage` | Optional (true). Hook the game's ped damage routine (ADR-0007) so `PedDamaged`/`PedDied` are exact (attacker, weapon, type, bone, amounts, hit point) for every ped. Off = inferred from health changes. |
| `governorEnabled` | Optional (true). The performance governor throttles modules over budget and computes `Perf.Pressure`. |
| `moduleBudgetMs` | Optional (2.0). Average ms per update a module may use when its manifest gives no `BudgetMs` (0.1–50). |
| `throttledIntervalMs` | Optional (100). Update interval the governor gives a module that stays over budget (10–2000). |
| `targetFrameMs` | Optional (33.3). Frame time the governor treats as full pressure's starting point (8–100). |
| `lowAddressSpaceMegabytes` | Optional (600). Free 32-bit address space below which pressure rises (100–2000). |
| `watchdogStallMilliseconds` | Optional (5000). An engine frame running longer than this is logged with the running phase and a minidump (1000–60000). |
| `teleportBlockingWindowMilliseconds` | Optional (60000). Teleports call `LOAD_SCENE`, which blocks the game thread for seconds; the watchdog tolerates a stall this long while one runs (5000–120000). The time is logged as `player_teleport load_scene_ms`. |
| `uiTextRenderer` | Optional (`sprite`). How the shared UI canvas (menus, help box, HUD text of other modules) draws text: `sprite` renders each string once with GDI+ into a cached texture (white with a dark shadow, tinted per draw, ellipsised to its rectangle, at most 600 cached) and draws it as a sprite; `shdn` uses ScriptHookDotNet `Graphics.DrawText`, which made frames that drew several strings take 0.4-0.9 s in game (T-045 measurement; internal cause not established). `ui-text-renderer sprite\|shdn [limit]` switches it live. |
| `uiTextCacheEntries`, `uiTextNewSpritesPerFrame` | Optional (600, 8). Sprite text cache size (50-4000 strings, least recently used out) and how many new text textures one frame may create (1-64); a frame needing more draws the rest on the next frames. `ui-text-stats` reports the count and estimated bytes. |
| `adaptiveDensityFloor` | Optional (0 = off). When above 0, the governor lowers ped and vehicle density toward this fraction as `Perf.Pressure` rises (0–1). Off by default because it changes the vanilla population. |
| `hotReload` | Optional (false). Development: reload a mod assembly in `scripts\LibertyFramework\mods` when its file changes. The change must settle for one poll, and the content hash must differ from the loaded copy. Also switchable at runtime with `lf hotreload on/off`. |
| `hotReloadPollMs` | Optional (1000). How often the mods folder is checked while hot reload is on (250–10000). |
| `hotReloadMaxLeakMegabytes` | Optional (32). .NET Framework cannot unload a replaced assembly, so each reload keeps the old copy in the 32-bit address space. Past this total, reloads are refused until the game restarts (1–256). |
| `raycastEnabled` | Optional (true). ADR-0008: install the core's call into the game's line test for `Query.Raycast` / `HasLineOfSight`. Off = every raycast answers `Unavailable`. |
| `raycastMaxPasses` | Optional (8). Hits of kinds a query does not stop at (or ignored entities) that one query passes through, each costing one more line test. Beyond it the query is `Inconclusive` (0–32). |
| `raycastPassStepMeters` | Optional (0.05). How far beyond a passed-through hit the next line test starts (0.01–1). |
| `raycastMaxLengthMeters` | Optional (1000). Longest ray a query accepts; longer rays throw `ArgumentException` in the calling module (1–5000). |

Defaults apply when the file is absent or invalid (logged).

`arsenal.json` gains `inventoryRefreshMilliseconds` (500) and `stateRefreshMilliseconds` (200):
- **Inventory:** re-read on engine weapon, shot, reload and death events, and at least this often.
- **State flags:** arrest, death, mission, cutscene and fade are read at most this often.

`arsenal.json` `trunkTimings` (required) holds the S-3 trunk choreography timings, in milliseconds:

- **Steps:** `turnMilliseconds` (turn to the trunk), `lidOpenAtMilliseconds` and `lidCloseAtMilliseconds` (when the lid moves within the open and close clips), and `browseStepTimeoutMilliseconds`.
- **Clip windows:** `openMin/Max`, `idleMin`, `withdrawMin/Max` and `closeMin/Max`. Each step ends when its clip stops after min, or at max.
- **Ranges:** steps 0–5000; min <= max <= 10000. An invalid block disables Arsenal with a logged error.

## Build-time: models/sling.json (T-2 / W-5)

Read by `tools/models` (`LibertyModel sling`) at package time.

- **Body:** `bodyArchive`, `bodyPrefixes` (the outfit meshes the straps must clear), `skeletonModel` and `attachBone` (model bone name, e.g. `Char_Spine2`).
- **Template:** `templateArchive` and `templateModel` (a single-geometry prop whose drawable and texture dictionary are reused).
- **Strap shape:** `clearanceMeters` (gap over the outermost outfit), `hipMarginMeters` (where the cut stops past the hip anchor), `widthMeters`, `thicknessMeters`, `textureRepeatMeters` (strap length per texture repeat) and `segments` (points around the loop).
- **Registration:** `textureDictionary`, `drawDistanceMeters` and `audioMaterial` (IDE `weap` and `amat` entries).
- **`leather`:** `baseColour`, `edgeColour` and `stitchColour` (RGB), `grainStrength`, `scuffStrength`, `stitchInsetFraction` (of strap width from each edge) and `stitchPeriodPixels`.
- **`straps[]`:** `model`, plus the shoulder anchor and the hip anchor.
  - Shoulder anchor: `shoulderBone`, `shoulderTowards` and `shoulderFraction` (a point that far from the first bone toward the second).
  - Hip anchor: `hipBone`, `hipOffsetMeters` (model space, Z up).

## combat_effects.json (T-022)

`bloodVisualMode` is `stock` (default if absent) or `external`. In `external`, Liberty Framework keeps hit reactions and dismemberment, leaves impact decals/streaks to the companion, and adds bounded wound/stump pulses plus the game's bleeding flag. Use `external` only when another renderer supplies the broader wound visuals. The Violent Liberty companion and rollback steps are in [ViolentLiberty.md](../research/ViolentLiberty.md).

**Companion visual pass:** `externalBleedEffectName` is a confirmed one-shot effect, pulsed at a damaged bone because CE returned zero for every attempted looping blood effect. `externalBleedMinimumDamage`, `externalBleedDurationMilliseconds`, `externalFatalBleedDurationMilliseconds`, `externalBleedStartIntervalMilliseconds`, `externalBleedEndIntervalMilliseconds`, `externalBleedScaleMultiplier`, `externalBleedEndScaleFraction`, and `externalMaximumBleedEmitters` bound ordinary leaks (health, ms, scale ratios, count). `externalStumpBurstEffectName`, `externalStumpBleedScale`, `externalStumpBleedDurationMilliseconds`, and the two stump interval fields give cuts a stronger leak. `externalLimbLandingEffectName`/`Scale`, `limbLandingMinimumMilliseconds`, and `limbLandingMaximumHeightMeters` gate one ground impact effect. `limbThrowMaximumAttempts`/`RetryMilliseconds` bound failed clone retries; `severedLimbSpawnHeightMeters` and `severedLimbVerticalForceFraction` tune the throw. All are configured in `config/combat_effects.json`. The owner's external INI is tuned from `config/violent_liberty_tuning.json` during companion installation; that JSON selects head/neck pressure, bleed duration, and shotgun frequency/speed.

`schemaVersion:1`; `enabled` is the master toggle, on in the integrated Phase 2 playtest build. `reactionsEnabled`, `injuriesEnabled`, `woundsEnabled`, `limbLossPrototypeEnabled`, and `headLossPrototypeEnabled` independently gate force, injury counts, bone-attached stock blood PTFX, limb candidate logging, and corpse-only head removal. `impactEffectName` / `woundEffectName` are installed game PTFX names. `reactionForceHead/Torso/Arm/Leg` and `reactionVerticalFraction` tune the force vector. `scanRadiusMeters` (m), `sampleIntervalMilliseconds` (ms), `maximumTrackedPeds`, `maximumWoundsPerPed`, and `woundLifetimeMilliseconds` (ms) bound sampling and PTFX handles. `reactionCooldownMilliseconds` (ms), `minimumInjuryDamage` (health), `minimumLimbLossDamage` (health), and `minimumLimbLossHits` (count) gate events. `allowedWeaponIds` lists registered test weapon IDs only. The attached effect follows the bone; exact bullet impact coordinates remain unavailable.

**Gore overhaul fields (optional).** All `*EffectName` values are stock `gta_core.wpfl` names, checked offline.

- **Coverage:** `allFirearms` covers every firearm slot; `includeMissionPeds` covers mission peds.
- **Particle size:**
  - `effectScale` is the base particle scale (float, ≤5). A hit's scale is `effectScale × damage/40`, clamped to `[0.8 × effectScale, maximumHitScale]`, with `maximumHitScale` ≤8.
- **Damage thresholds:**
  - `minimumEffectDamage` (health): hits below it only drip. These are bleed-out ticks.
  - `exitDamage` / `chunkDamage` (health) gate the exit spray and the chunks.
- **Per-hit effects:**
  - Every hit plays the entry effect (`impactEffectName`, or the shotgun/sniper entry) plus `mistEffectName`.
  - `exitEffectName` plays at `exitDamage`.
  - Chunks play at `chunkDamage`, and always for shotguns and snipers: `heavyChunksEffectName`, `shotgunChunksEffectName` or `sniperChunksEffectName`.
  - The killing hit plays `deathEffectName` plus `mouthBloodEffectName`.
- **Bleeding:**
  - Each wound drips `bleedEffectName` every `bleedIntervalMilliseconds` for `bleedDurationMilliseconds`.
  - At `exitDamage` or more, the wound also spurts `woundSpurtEffectName` every `arterialIntervalMilliseconds` for `woundSpurtDurationMilliseconds`.
  - `maximumEmitters` caps active pulses.
- **Severing:**
  - `decapitationEnabled` / `decapitationMinimumDamage` (health) control decapitation.
  - `pendingDeathWindowMilliseconds` is how long after a hit the ped may die and still be severed.
  - At the stump: `severBurstEffectName` / `severMistEffectName`, then `arterialEffectName` every `arterialIntervalMilliseconds` for `arterialDurationMilliseconds`.
- **DevTools Gore Test:** `goreTestScale` (≤8) and `goreTestIntervalMilliseconds` (ms per gallery step).
- **Looping effects (streams, drips, mist, chunks):**
  - `maximumLoopedEffects` (1–64) caps how many run at once; the oldest is stopped first.
  - `burstLoopMilliseconds` (50–5000) is how long a one-off burst of a looping effect lasts.
- **Death leak:** `deathLeakEffectName` / `deathLeakDurationMilliseconds` (≤120000) set how long a killed body keeps leaking.
- **Severing timing:** `severDelayMilliseconds` (0–3000) waits after death before collapsing bones.
- **Collapse size:** `collapseScale` (0.0001–0.1) is the leftover scale of collapsed bones. It is never zero.

## T-011 finish variants

`lf_gold_carbine` uses `w_m4` diffuse `bm_m4a1`, specular `bm_m4a1_s`, and `icon`; `lf_gold_shotgun` uses `w_shotgun` diffuse `cj_shotgun_comp`, specular `cj_shotgun_comp_s`, and `icon`. Their normal maps are retained.
## Phase 2 weapon catalog and physical records (T-025)

The next Phase 2 playtest enables `combat_effects.json` `headLossPrototypeEnabled` for lethal head hits on ambient NPCs by gold weapons. Visible arm and leg removal remains a diagnostic candidate only. The gunsmith offers three persistent, tuning-only attachments: Match grip ($350, bloom increment multiplier 0.75) for gold pistol 58, Stability stock ($600, 0.82) for gold carbine 59, and Steady fore-end ($450, 0.85) for gold shotgun 60. No new attachment mesh is included.

`config/weapon-catalog.json` has `schemaVersion: 1` and unique `entries[]` keyed by `weaponId`.
Each entry defines `id` (stable catalog ID), `family`, `label`, `role` (`replacement`, `add-on`, or `test`), the registered model name, allowed `finishes[]`, and `attachments[]`. The current catalog points only to already registered GTA IV or Phase 1 models. `attachmentOptions[]` defines the `id`, label, price in dollars, and `perShotBloomMultiplier` (0–1) for each offered attachment. The gold pistol's `match-grip` costs $350 and multiplies each bloom increment by 0.75; its first-shot base cone and vanilla weapons are unchanged. Other attachment IDs remain metadata until a matching option and effect are implemented. The factory/gold finish choice switches the existing pistol models, while no new asset is created.

`state/arsenal_<episode>.json` remains schema version 1. Optional `carriedRecords[]` persists full `WeaponRecord` objects. Each record now has optional `instanceId` (UUID without separators), `catalogId`, `attachments[]`, and `progression` in addition to its prior ID, category, ammo, owned, finish, and acquisition time. Missing IDs are generated on load. `ownedCarried[]` remains for old saves. Arsenal snapshots carried ammo and metadata at most every five seconds when changed and immediately during storage/loss transfers. `JsonStore.Save` keeps a `.bak` of the previous state.

`config/arsenal.json` optionally sets `gunsmithGoldFinishPrice` in dollars (`500` in the template). At a safehouse Arsenal page or nearby safehouse panel, this charges once to unlock the existing gold pistol model for that physical service pistol; returning to the factory model and re-equipping gold are free. A missing or zero price disables new finish purchases while retaining legacy Arsenal operation and previously unlocked finishes. The Match grip price/effect live in `weapon-catalog.json`. The integrated installer merges the top-level Arsenal default into an existing `arsenal.json` without replacing marked safehouses.

## Stage 1 arsenal: weapon-catalog.json and gunplay.json weapons[] (T-041)

`config/weapon-catalog.json` (still `schemaVersion: 1`; every new field is optional, so an older catalog loads) now carries the Stage 1 arsenal:

- Top level: `tiers[]` (`id`, `label`, `normalAvailability`, `minimumStoryProgress` 0-1, default `sources[]`), `restrictedClasses[]` (classes that can never be Stage 1: `sniper`, `lmg`, `pdw`, `military`), `applyWeaponInfoStats` (true = the packager writes each Stage 1 entry's `stats` into WeaponInfo.xml; false = it writes `vanillaStats`, the game's own values, so the switch restores the game).
- Per entry: `weaponInfoType` (the game's WeaponInfo.xml name of that weapon id, for example `AK47`), `class` (`pistol`, `shotgun`, `smg`, `rifle`, `sniper`, `lmg`, `pdw`; T-042/T-043 select handling and reticle by class), `tier`, `stage1` (true = a Stage 1 arsenal weapon; false or absent = the game's own behaviour, AGENTS.md rule 2), `profile` (the `profileName` of its gunplay.json entry), `availability`, `stats`, `vanillaStats`.
- `availability`: `sources[]` (`gun-shop`, `street-dealer`, `contact`, `mission-reward`, `mission-only`), `price` in dollars, `contact` (a contact id that must be unlocked, for example `little-jacob`), `minimumStoryProgress` (overrides the tier's), `note`. Rules: no XP; a restricted tier or class is never offered; unknown story progress fails any rule that needs progress.
- `stats` / `vanillaStats`: `timeBetweenShotsMilliseconds`, `damageBase`, `clipSize`, `ammoMax` (all optional). They are the `timebetweenshots`, `damage base`, `clipsize` and `ammomax` fields of the weapon's `<data>` in `update\common\data\WeaponInfo.xml`, written by `tools/package-phase2.ps1` (`Merge-WeaponInfoStats` in `tools/PackageMerge.psm1`; only those fields, only Stage 1 entries, byte-identical when nothing changes). Accuracy is not in the file: the spread model writes it at run time.
- Validation (`WeaponCatalog.Validate`, the verifier and the game): a Stage 1 entry needs class, a known tier that has `normalAvailability: true`, a class outside `restrictedClasses`, `profile`, `weaponInfoType`, `availability.sources`, `stats` and `vanillaStats`.

`gunplay.json`: a `weapons[]` entry for a Stage 1 weapon keeps the **vanilla id** (`weaponId` = `vanillaWeaponId`, 1-57) and names its catalog entry in `catalogId` (new, optional; test weapons 58+ have none). The gate (`Gunplay/Logic/Stage1Gate`) applies the recoil/spread/reticle model to a vanilla-id weapon only when the catalog has a `stage1: true` entry with that `catalogId` and `weaponId`; a missing catalog or a weapon outside it stays vanilla. Optional `stage1Weapons: { "enabled": true }` (absent = enabled): `false` turns every catalog profile off (weapons behave as vanilla; the test weapons are unaffected). The accuracy the model writes for a catalog weapon is put back to the game's own value when the player switches away (a catalog weapon shares its WeaponInfo entry with every NPC that carries it). Stage 1 recoil/spread numbers are a starting point for T-042.

`classTargets[]` (T-042, optional; validated): per catalog class (`pistol`, `shotgun`, `smg`, `rifle`) what a Stage 1 weapon must deliver, all proposals until the owner confirms them (STAGE1 Pillar 3): `firstShotConeMaxDegrees` (standing aiming cone at rest), `burstShotCount` and `burstRecoveryMaxMilliseconds` with `recoveryToleranceFraction` (after that many shots at the weapon's own fire rate the cone is back within the tolerance of the first-shot cone within the time), `sustainedShots` (0 = no sustained target; automatic classes use 30) with `climbMinDegrees`/`climbMaxDegrees` (peak camera pitch during the burst) and, implied, a cone that grows with every sustained shot until `maxDegrees`. `GunplaySimulation` runs the game's own `SpreadModel` and `RecoilSolver` at 60 Hz with the weapon's catalog fire interval; the verifier and `catalog sim` both use it.

`reticles` (T-043, optional; absent or `enabled: false` = the single `crosshair` for every weapon, so an older gunplay.json keeps working). Sections: `notAiming` (`hidden`, the old behaviour, or `reduced`: the style is drawn at `notAimingOpacity` 0-1 when the weapon is held but not aimed), `hideInVehicle`, `debugLog` (log `reticle_frame` lines, the same as the `reticle debug` command), `classes[]` and `weapons[]`. A style (`ReticleStyleSettings`, every field optional) has `style` = `cross` (four arms), `bracket` (crop marks on the corners of the square the bullets stay inside, arms outward), `ring` (`ringDots` dots on a circle whose radius is the pellet spread), `dot` or `none`, plus `lineLengthPixels`, `lineThicknessPixels`, `outlinePixels`, `minimumGapPixels`, `maximumGapPixels`, `centerDotPixels` (0 = none), `colorArgb`, `outlineArgb`, `gapSmoothingPerSecond`. Sizes are pixels on the real screen height like the old crosshair. Resolution order (`ReticleResolver`): the old `crosshair` values, then the weapon's class entry (its catalog `class`; a weapon outside the catalog uses its inventory slot: handgun pistol, shotgun, smg, rifle, heavy, thrown, sniper), then the `weapons[]` entry (which may borrow another class with `class` and change single fields). The opening (cross gap, bracket corner distance, ring radius) is `tan(cone)` times the game's measured pixels per tangent, clamped only by the style's minimum and maximum; there is no cosmetic factor, it opens instantly on a shot and eases closed at `gapSmoothingPerSecond`. The `sniper` class is `none`: sniper-slot weapons keep the game's own scope (untouched). Commands: `reticle debug on|off`, `reticle check` (an `error:` reply when a steady frame is more than 5% off the cone or any frame is drawn more than 5% smaller than the cone asks for), `reticle reset|status`.

Commands (console and autopilot): `catalog [list]`, `catalog give <catalog id|weapon id> [ammo] [force]` (force is needed for a weapon that is not a Stage 1 weapon), `catalog offer <money> <story progress|unknown> [contacts,comma|-] [override]`, `catalog check`, `catalog sim [catalog id|all]` (T-042: the live config through the model against `classTargets`; an `error:` reply names every miss). T-042 also adds `range start ahead <m> [height]|start <x> <y> <z>`, `range fire <ms> [mode 0-4]`, `range stop|status` (the gunplay module logs each of the player's bullets as `range_shot`: deviation from the line to the aim point, the cone the model wrote, position in the chain; `range_summary` per weapon and group on stop; `tools/perf/Measure-GunplayRange.ps1` tabulates them), `aim on|off [crouched] [cover] [speed <m/s>]` (test hook: the model and the reticle treat the player as aiming, with a forced stance or speed; `range start` switches it on and `range stop` off), `swap left|right|toggle|status|probe` (shoulder swap without the aim button; `probe` fails unless the live camera table equals the originals times the side factor and the slide has finished) and `ray left|right` (the engine ray from the player's own sides, for shoulder clearance).
## Performance fields (T-026)

- **`gunplay.json` `performance`** (optional):
  - `gameCameraRefreshMilliseconds` (0-5000) sets how often `GET_GAME_CAM`/`DOES_CAM_EXIST` are re-read. In between, the handle is validated from the camera pool.
  - `fovRefreshMilliseconds` (0-5000) sets how often FOV is read while not aiming.
  - When the section is absent, every tick re-reads (the old behaviour).
- **`combat_effects.json`:**
  - `idleSampleIntervalMilliseconds` (0-2000) is the damage-scan interval when the player has not fired recently.
  - `activeSampleWindowMilliseconds` (0-30000) is how long after a detected shot the scan runs at `sampleIntervalMilliseconds`.
  - An idle interval of 0, or one not above `sampleIntervalMilliseconds`, keeps full-rate scanning.
- `combat_effects.json` `dismemberRefreshMilliseconds` (0-1000) is the dismemberment upkeep cadence once the engine collapse is installed and every record is older than 1 s. 0 means every tick.
- `combat_effects.json` `maximumCutsPerPed` (0-8, 0 = unlimited) is how many cuts one body can receive, counting pending and completed cuts.

## World objects: config/world/objects.json (T-033)

Read by the world mod (`mods/Liberty.World`, module id `world`) as `scripts\LibertyFramework\config\world\objects.json`.
It is hot-reloaded. An invalid file is rejected and logged (`config_rejected world/objects`), and the defaults are used.
The verifier checks the shipped file, and that every `model` it places is an asset under `content/`.

| Field | Default | Meaning |
|---|---|---|
| `schemaVersion` | 1 | must be 1 |
| `enabled` | true | false places nothing |
| `streamInMeters` | 150 | spawn an object when the player is this close (horizontal distance), above 0 and at most 1500 |
| `streamOutMeters` | 180 | delete it again beyond this; must be above `streamInMeters` (the gap stops flicker at the edge) |
| `checkIntervalMilliseconds` | 500 | how often distances are checked, 50–10000 |
| `collision` | true | give placed objects collision (from their model's bounds, when the game pairs them) |
| `objects[].name` | — | unique id, used in logs and by `world ray <name>` |
| `objects[].model` | — | model name of a LibertyContent asset (1–23 letters, digits or `_`) |
| `objects[].position` | — | `[x, y, z]` metres: where the model's origin goes |
| `objects[].headingDegrees` | 0 | 0 = north, counter-clockwise |
| `objects[].snapToGround` | true | put the origin on the ground under `position` when the game reports a ground height there; `z` is where the search starts |
| `objects[].collisionProxies` | none | list of hidden vanilla props with solid collision placed at the object so it is solid without authored bounds; each `{ model, offset [x,y,z] (world axes, metres), headingOffsetDegrees }`, at most 16. Solid models: see `proxy-probe` in [Collision.md](../research/Collision.md) |


## Liberty HUD: config/hud.json (T-049)

Read by the `hud` module (`src/LibertyFramework/Hud`) as `scripts\LibertyFramework\config\hud.json`; polled once per second and applied live. Every member is
optional: one absent from an older file takes the default below (a check keeps `config/hud.json` and the code defaults identical), so an install that has
the file keeps working when a field is added. An invalid file is rejected and logged (`hud_config_rejected`) and the last good configuration stays. The
installer merges new top-level sections into an existing file and keeps the owner's values. All sizes are virtual units (720 high, 720 x aspect wide); colours are
`[alpha, red, green, blue]`; times are seconds; the smallest text drawn is 14 px.

| Field | Default | Meaning |
|---|---|---|
| `schemaVersion` | 1 | must be 1 |
| `enabled` | true | **false restores the complete vanilla HUD** (every hidden component put back, nothing drawn, the help box drawn by the engine again) |
| `hideVanilla` | `components` | `components`: replace only components whose resolution AND visible hiding are verified (see [HudComponents.md](../research/HudComponents.md)); none currently qualify for T-049. `none`: leave vanilla alone |
| `layout.marginRight` / `marginTop` | 28 / 24 | the top-right group's distance from the screen edges |
| `layout.iconWidth` / `iconHeight` | 96 / 48 | weapon silhouette (the 2:1 icons of `ui\icons`) |
| `layout.ammoHeight` | 22 | height of the clip / reserve line, at least 14 |
| `layout.barWidth` / `barHeight` / `barGap` / `rowGap` | 112 / 5 / 4 / 6 | health and armour bars and the spacing of the rows (icon, ammo, health, armour, stars) |
| `layout.starSize` / `starGap` | 16 / 4 | wanted stars |
| `layout.promptX` / `promptY` / `promptWidth` / `promptHeight` | 34 / 30 / 360 / 44 | the help box (top left, where the IV one is) |
| `layout.shadowOffset` | 1 | dark offset copy behind icon and text, 0 = none |
| `palette.*` | see file | `text`, `textShadow`, `icon`, `barBack`, `health`, `healthLow`, `armour`, `wantedStar`, `wantedEmpty`, `promptBack`, `promptText`, `lowClip` |
| `weapon.enabled` | true | the icon + clip / reserve group |
| `weapon.fadeInSeconds` / `fadeOutSeconds` / `holdSeconds` | 0.15 / 0.6 / 4 | fade times and how long it stays after its latest trigger |
| `weapon.showOnChange` / `showOnShot` / `showWhileReloading` / `showWhileAiming` | all true | what brings it up; `showWhileAiming` follows the left trigger above `aimTriggerLevel` or the right mouse button |
| `weapon.alwaysWhenArmed` | false | keep it up whenever a weapon is held |
| `weapon.lowClipFraction` | 0.25 | the clip count turns `lowClip` at or below this fraction of the largest clip seen for the weapon (empty always) |
| `weapon.totalIncludesClip` | true | reserve = total - clip. September 30 pistol snapshot 150 total / 17 clip matches vanilla reserve 133; firing/reload supports this pistol only. Other weapons and consistent sample timing remain unverified |
| `weapon.aimTriggerLevel` | 0.3 | left-trigger level (0-1) that counts as aiming |
| `weapon.vanillaComponents` | `HUD_WEAPON_ICON`, `HUD_AMMO` | required counterparts for replacement; listing them is insufficient without verified visible hiding. Currently kept vanilla |
| `weapon.drawWithoutHidingVanilla` | false | draw even when a listed component cannot be hidden (a duplicate of the vanilla one) |
| `health.*`, `armour.*` | see file | `enabled`, fades, `holdSeconds` (after a change or damage), `showOnChange`, `combatHoldSeconds` (after a shot fired or damage taken; 0 = off), `lowFraction` (health stays up at or below it; 0 = never), `pulsePerSecond` / `pulseDepth` (brightness pulse while low), `maximum` (the value that fills the bar; health is on the gameplay scale, the game's raw value minus 100), `hideWhenZero` (armour only: not drawn at zero), `vanillaComponents`, `drawWithoutHidingVanilla`. Armour defaults: `lowFraction` 0, `pulseDepth` 0, `hideWhenZero` true |
| `health.vanillaComponents`, `armour.vanillaComponents` | empty | arcs belong to the radar; no separable component is known. Bars stay off until isolated hiding is verified (or explicit `drawWithoutHidingVanilla` accepts a duplicate) |
| `wanted.*` | see file | stars while wanted: fades (0.2 / 0.8), `holdSeconds` after the level drops to zero (1), `maximumStars` (6), `vanillaComponents` (`HUD_WANTED_BACK`, `HUD_WANTED_FRONT`), `drawWithoutHidingVanilla` |
| `prompt.enabled` | true | the help box is drawn by the HUD (restyled, faded) with button names |
| `prompt.fadeInSeconds` / `fadeOutSeconds` | 0.12 / 0.25 | |
| `prompt.device` | `auto` | `auto` follows the device used last (pad buttons, sticks and triggers against keyboard keys); `pad` / `keyboard` force one |
| `prompt.deviceSwitchMilliseconds` | 400 | minimum time between two automatic switches, so a stray key does not flip the names |
| `prompt.glyphs[]` | `interact` X/E, `accept` A/Enter, `cancel` B/Backspace, `reload` X/R, `aim` LT/Right Mouse, `fire` RT/Left Mouse, `wheel` RB/Tab | `{token}` in a help text becomes `pad` or `keyboard` text; lower-case unique tokens. A token with no entry stays visible in braces |
| `suppress.inCutscenes` / `whenPaused` / `whenFadedOut` / `whenDead` | all true | the Liberty elements are not drawn then (hidden vanilla components stay hidden) |
| `sampling.wantedPollMilliseconds` / `ammoPollMilliseconds` / `inputPollMilliseconds` | 250 / 100 / 100 | how often the slower game readings are taken (16-5000) |

Test commands (`lf hudctl ...`, console and autopilot): `status`, `table` (every hud.dat component with its live values, and calls the resolver could not parse), `check`,
`ammo`, `hide <NAME>` / `hide-matching <text>` / `restore [NAME]` (research), `force <element|all> on|off`, `hurt <n>`, `health <n>`, `armour <n>`, `prompt <text>`, `device auto|pad|keyboard`,
`layout-test on|off` (draw every element even where its vanilla counterpart stays). `probe-mode on|off` suspends Liberty replacements for the component probe;
manual hides are reapplied until restored. `config-test off|restore` saves the installed HUD config bytes, writes `enabled=false` for the normal poll,
and restores the exact original bytes on `restore`, module stop/unload or failure. These hooks are diagnostic only; they do not change shipped policy.
## Impact surfaces: config/impact-surfaces.json (T-050)

Generated by `tools/research/build_impact_surfaces.py "<GTAIV dir>"` from the installed `common/data/materials/materials.dat`; edit the script's class table, not the output, then regenerate. `materialsDatSha256` is the file the table was built for (a consumer refuses or warns on a different file). `rows[]` has `id` (the 0-based data row = `RayHit.SurfaceMaterialId`), `name`, `group` (materials.dat column 2), `impact` (`concrete`, `wood`, `metal`, `glass`, `default`, or `ped` for body parts, which gore handles; `armour` is chosen from the ped's armour state, never from a material) and `water`. `groupToImpact` maps groups to classes; any group not listed is `default`.


## UI acceptance diagnostics (T-045/T-046)

`ui-budget begin` resets measurement counters; `ui-budget baseline` captures a closed-menu window. `ui-budget check <label> <draw ms> <p95 ratio> <p99 ratio> <minimum frames>` reads/reset counters and returns an error when samples are missing/insufficient, average frame time or p95/p99 exceeds the paired closed window, a >=1 s stall occurs, or `draw.ui` average exceeds the budget. Scenarios propose 0.5 ms draw, +10% average/p95, +15% p99 and at least 30 samples. These are enforced script/frame pacing proxies, not presented-frame measurements or owner approval. `draw.ui` includes module canvases and menus together.

`ui-render-diagnostic all|none|text|primitives` temporarily filters the shared canvas for the bounded experiment; always restore `all`, or restart the engine. It changes no persistent config. `menu-test unlocked` opens the SDK sample with control locking disabled solely to isolate control capture. Neither diagnostic establishes gameplay acceptance.

### T-049 native display diagnostic (no new shipped config)

`hudctl native-display on <1..120000 milliseconds>|off` requires enabled HUD, `probe-mode on` and `layout-test off`.
The tick holds DISPLAY_HUD(false)/DISPLAY_RADAR(true); expiry, config-off, unsafe gameplay, public HUD-off and
stop/unload/failure cancel the lease. Restoring config never rearms it. `hudctl status` reports `native_requested`
and `native_applied`; neither is visual proof. `cash-test pulse|restore` supplies a temporary wallet-change baseline
in probe mode and restores on stop/unload/failure. Config-test logs saved/readback hashes. Do not transact, save,
change external native visibility or concurrently edit config during the diagnostic. Shipped vanilla guard and
HUD layout/tuning are unchanged. Required real text/unload review: T049-hud-native-story-text.
