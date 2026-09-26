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

`weapons[]`: `weaponId`, `vanillaWeaponId`, `weaponInfoName` (must match WeaponInfo.xml), `label`, `profileName`, `finish`, `initialAmmo`, `calibrationSource` (use this weapon's bullets to calibrate), `recoil`, `spread`.

`recoil` (degrees of aim camera, ms): `verticalKickDegrees`, `horizontalKickDegrees` (bias, + right), `horizontalRandomDegrees` (±), `firstShotMultiplier`, `sustainedFireGrowthPerShot`, `sustainedFireShotCap`, `chainResetMilliseconds`, `maxAccumulatedDegrees` (soft cap), `minimumKickFractionAtCap`, `kickDurationMilliseconds`, `recoveryDelayMilliseconds`, `recoveryDegreesPerSecond`, `recoveryFraction` (0–1 of each kick auto-recovered), and multipliers `moving`, `crouched`, `cover`, `vehicle`, `hipFire`.

`spread` (cone half-angle in degrees from the muzzle): `baseDegrees`, `perShotDegrees`, `burstShotCount` (early shots given a reduced bloom increment), `burstPerShotMultiplier` (0–1 early increment), `chainResetMilliseconds` (pause that starts a new burst), `maxDegrees`, `recoveryDelayMilliseconds`, `recoveryDegreesPerSecond` (long spray), `shortBurstRecoveryDegreesPerSecond` (taps and short bursts), `movingAddDegrees` (at full movement), multipliers `crouched`, `cover`, `vehicle`, `hipFire`, `blindFire`, `airborne`, and `pelletPatternDegrees` (display-only allowance for multi-pellet weapons until 16 bullets are measured). Both recovery rates start after the delay; the rate is chosen by burst length.

## presets/*.json

`{"schemaVersion":1,"name","description","weapons":[{"weaponId","profileName","recoil","spread"}]}` — replaces recoil/spread for listed weapons. Shipped: A GTA IV+, B Mafia Light, C Mafia Heavy, D Experimental (generated by `tools/generate-presets.ps1`). "Save as preset" writes `presets/user_saved.json`.

## devtools/locations.json

`{"schemaVersion":1,"locations":[{"id","name","x","y","z","heading","snap","note"}]}`. `snap`: `none` (exact), `pavement` (nearest pavement node), `ground` (ground below z). `gun_test_range` is used by Test Range and is overwritten by "Set Gun Test Range here".

## Build-time: assets/finishes/finishes.json

## Arsenal: arsenal.json (T-020)

Required `schemaVersion:1`; `sidearmLimit:2`, `longGunLimit:2`, `meleeLimit:1`. `purchaseWindowMilliseconds` is the maximum elapsed time from money decrease to weapon gain for purchase ownership. `trunkDistanceMeters` is interaction distance from the boot; `trunkRearOffsetMeters` locates the boot behind the vehicle (SHDN vehicle local Y points forward). `ownedVehicleMatchMeters` matches LVS model hash and position; `fallbackVehicleMatchMeters` matches Arsenal's remembered vehicle marker. All distances are meters. `categories[]` maps each `WeaponCategory` enum number to `group` (`sidearm`, `longGun`, `melee`, `uncounted`) and `BodySlot` enum number. `safehouses[]` has `id`, `name`, `episode` (`iv`, `tlad`, `tbogt`), world `x/y/z`, `radius` meters, and `verified` boolean. The shipped list is empty because no sourced coordinates were provided; DevTools **Mark safehouse here** records the actual player coordinate, adds a verified entry, and writes `arsenal.json` with a backup.

Phase 2 contextual storage uses the existing `trunkDistanceMeters` for both the vehicle rear and a capped safehouse prompt radius. Controller **Square/X** or keyboard **E** opens the compact panel; **A/Enter** transfers, **B/Backspace** closes. This binding is fixed for the first playtest and should be exposed in config after control-conflict feedback. Nearby vehicle searches run every 250 ms.

The `performance` log line every 30 seconds records ScriptHook tick interval p50/p95/p99, counts over 33/50 ms, and gunplay-loop average/maximum CPU time. Tick intervals are a frame pacing proxy; use PresentMon for final presented-frame comparisons.

Per-episode `state/arsenal_<episode>.json` contains owned-carried IDs, persistent vehicle trunk bins, safehouse stash bins, last safehouse, and last vehicle marker. `JsonStore.Save` writes atomically and maintains `.bak`. A corrupt state file is renamed to `.corrupt_<UTC>` without changing the existing `.bak`, then state starts empty.

Finish colour ramps (`shadowRgb`, `midRgb`, `highlightRgb`, `contrast`, `lift`, `gamma`) per texture role (`diffuse`, `specular`, `icon`) and model variants (`variantModel`, `baseModel`, `sourceImg`, `finish`, `animGroup`, `drawDistance`, `audioMaterial`, `weaponInfoType`, `textureRoles`). Add a finish or a weapon variant here and rerun `tools/package-phase1.ps1`.

## holsters.json (T-021)

`schemaVersion: 1`, `enabled`, `showOnBikes`, `nudgePositionMeters`, `nudgeRotationDegrees`, `weapons[]` (`weaponId`, `weaponInfoType`, `category` numeric `WeaponCategory`), and `placements[]` (`slot`, ScriptHookDotNet `bone`, `position` in meters XYZ, `rotation` in degrees XYZ). A placement can optionally specify `category` and/or `model` to override a slot default. `weaponInfoType` selects the active WeaponInfo.xml entry, whose `<assets model>` determines the prop. The starting offsets require visual calibration in game; DevTools saves changes with a `.bak`.

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
| `adaptiveDensityFloor` | Optional (0 = off). When above 0, the governor lowers ped and vehicle density toward this fraction as `Perf.Pressure` rises (0–1). Off by default because it changes the vanilla population. |
| `hotReload` | Optional (false). Development: reload a mod assembly in `scripts\LibertyFramework\mods` when its file changes. The change must settle for one poll, and the content hash must differ from the loaded copy. Also switchable at runtime with `lf hotreload on/off`. |
| `hotReloadPollMs` | Optional (1000). How often the mods folder is checked while hot reload is on (250–10000). |
| `hotReloadMaxLeakMegabytes` | Optional (32). .NET Framework cannot unload a replaced assembly, so each reload keeps the old copy in the 32-bit address space. Past this total, reloads are refused until the game restarts (1–256). |

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
