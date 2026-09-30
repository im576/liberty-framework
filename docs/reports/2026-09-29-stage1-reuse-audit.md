# Stage 1 reuse audit (T-040)

Date 2026-09-29, build `stage1/T-040`. Method: the code and configs named below were read, not summarised from memory; every
file path is in the repository at this commit. Purpose (STAGE1 section 6): for each Slice A task T-041 to T-049, what already
exists to extend, what is missing, the risks, and where the code or config **contradicts the design**. Facts only.

## 0. Findings that change the plan

| # | Finding | Where | Affects |
|---|---|---|---|
| F1 | **Combat effects are a health-polling proximity scan, not exact events.** `SampleDamage` reads the health of up to 24 peds within 40 m every 50 ms (400 ms idle), attributes with `HAS_CHAR_BEEN_DAMAGED_BY_CHAR(target, player)` and `GET_CHAR_LAST_DAMAGE_BONE`, and only for the player as shooter. No built-in module subscribes to `PedDamaged`, `PedDied` or `BulletFired`. This is exactly what STAGE1 Pillar 2 ("never proximity scans") removes. | `CombatEffects/CombatEffectsController.cs:190-221`, `config/combat_effects.json` (`scanRadiusMeters`, `sampleIntervalMilliseconds`, `maximumTrackedPeds`) | T-047, T-048 |
| F2 | **The gunplay model only knows three weapons.** `GunplayConfig.FindWeapon` returns a profile only for `weapons[]` entries 58/59/60; spread and recoil are applied only when a profile exists (`GunplayController.cs:264,277`). Every catalog weapon (7 to 17) is vanilla-tuned today. | `Gunplay/Profiles/GunplayConfig.cs:30`, `config/gunplay.json` | T-041, T-042 |
| F3 | **The catalog contradicts the availability design.** `config/weapon-catalog.json` maps replacement models to vanilla ids 7 to 17, including two snipers (16 M40A1, 17 DSR-1), the P90 (13) and the MG36 (15). Stage 1 excludes snipers, LMGs and P90-type weapons from normal availability. The catalog has no class, tier or availability field, and no per-weapon combat data. | `config/weapon-catalog.json`, `Weapons/Logic/WeaponCatalogEntry.cs` | T-041 |
| F4 | **Loadout limits differ from the decided 2 long guns + 1 sidearm.** `arsenal.json` has `sidearmLimit: 2`, `longGunLimit: 2`, `meleeLimit: 1`, and category 4 (SMG) is in the **sidearm** group (body slot `SidearmSecondary`). Snipers and heavies (category 6, 7) are long guns sharing body slot `LongGun2` with rifles. Whether SMGs (Uzi, MP5) are sidearms or long guns is a design question; today two SMGs count against the sidearm limit. | `config/arsenal.json` `categories`, `Arsenal/Logic/ArsenalPolicy.cs` | T-044 |
| F5 | **The atmosphere density governor reduces population by default.** `atmosphere.json` `density.enabled: true`, floors 0.55 peds / 0.6 cars, full density at a smoothed frame time of 25 ms or less, minimum at 45 ms or more. STAGE1 Pillar 4 requires density never below 0.8 and Pillar 5 forbids using density reduction to meet a budget. It also skews the baseline: a slow scene lowers density, which lowers the measured cost. The baseline reports the lowest density seen (log line `density`, every 30 s). | `Atmosphere/AtmosphereController.cs:110-121`, `config/atmosphere.json` | T-040 baseline, Slice B |
| F6 | **The weather director fights scripted weather.** Atmosphere calls `FORCE_WEATHER` on its own timetable (2 to 5 in-game hours per block, decremented when the hour changes). Setting the clock with `time` counts as an hour change, so a scenario that sets time then weather can have the weather replaced within a second. The Stage 1 scenarios therefore set `time`, wait 1.5 s, then `weather`. | `AtmosphereController.cs:132-160` | every scenario that sets weather |
| F7 | **`CostMeter` has one reader.** Gunplay's 30 s `performance_scripts` log call and the `costs` command both call `ReportAndReset`, so either steals the other's window (a gunplay report inside an 8 s sample cuts it short). Fixed in T-040: two independent readers. | `Core/Performance/Logic/CostMeter.cs`, `GunplayController.cs:406` | all budgets |
| F8 | **UI and crosshair draw time is not measured.** `CostMeter` covers ticks only. Pillar 1 (UI draw <= 0.5 ms) and T-043 (reticle drawing <= 0.1 ms) need draw-pass timing. Added in T-040: `draw.ui` and `draw.crosshair`. | `Engine/LibertyEngine.cs:889-902`, `Gunplay/GunplayController.cs:918` | T-043, T-045, T-046, T-049 |
| F9 | **The HUD can only be hidden whole, or reticle parts individually.** `IUi.SetHudVisible` calls `DISPLAY_HUD(false)` and `DISPLAY_RADAR(false)` together. Per-component hiding exists only for the four reticle components (`HUD_WEAPON_CROSSHAIR`, `_HEALTH_TARGET`, `_ARMOUR_TARGET`, `_DOT`) through resolved hud.dat globals, and `HudResolved` requires exactly those four. `common/data/hud.dat` names the other components (`HUD_AMMO`, `HUD_WEAPON_ICON`, `HUD_WANTED_BACK/FRONT`, `HUD_CASH`, `HUD_RADAR`, `HUD_AREA_NAME`, `HUD_STREET_NAME`, `HUD_VEHICLE_NAME`, `HUD_HELP_MESSAGE`...), but nothing resolves their globals yet. Player health and armour are not a hud.dat component in that list: where the game draws them is unknown (open question). | `UiService.cs:115`, `GameApi/HudReticle.cs`, `Core/Memory/GameAddresses.cs:68-76,576-610` | T-049 |
| F10 | **Menus have no mouse input.** `MenuInput` reads the pad (through the Steam Input virtual pad, XInput) and keys (arrows, Enter, Space, G, Backspace/Escape, Page Up/Down). `InputService` exposes pad buttons, sticks and keys; no mouse position or buttons. A keyboard/mouse weapon wheel has to use keys or add mouse input to the SDK. | `Engine/Ui/MenuInput.cs`, `Engine/Services/InputService.cs` | T-045, T-046 |
| F11 | **Weapon data overrides are file-level, not runtime.** Fire rate, damage and the other WeaponInfo fields come from `WeaponInfo.xml` (the game loads FusionFix's `update\common\data` copy first). At runtime the mod writes one field only, the aiming accuracy, and only for registered weapons, validated against the loaded XML (`WeaponInfoTable`, `WeaponInfoXml`). "Extend the WeaponInfo override path" therefore means generating and installing an XML, not new memory writes. | `GameApi/WeaponInfoTable.cs`, `GameApi/WeaponInfoXml.cs`, `staging/t007` (T-007 base hash) | T-041 |
| F12 | **Measurement depends on modules that Stage 1 changes.** `stop <module>` is the mod-off switch used by the baseline (below); `engine.json` `disabledModules` (module never constructed) is the other. See the baseline report for the equivalence check. | `LibertyEngine.cs:227,336-345` | T-040 |

## 1. Per-feature audit

Legend: **Reuse** = exists and is extended; **Missing** = must be built.

### T-041 Arsenal and availability

| | |
|---|---|
| Reuse | `config/weapon-catalog.json` + `Weapons/Logic/WeaponCatalog.cs` (validated: unique `weaponId`, finishes, attachment options with `perShotBloomMultiplier`); `Weapons/WeaponFinishCatalog.cs`; `Weapons/TestWeaponActions.cs` (give/select for tests); `config/gunplay.json` `weapons[]` with `recoil`/`spread` blocks and `WeaponProfile`; `GameApi/WeaponInfoTable.cs` (accuracy write, restored on unload); `Arsenal` tracks owned vs mission weapons; `tools/install-weapon-pack.ps1` (Realistic Weapon Overhaul models, hash-checked); scenarios `bullet-events`, `exact-damage` (armed subjects, weapon 7). FusionFix ExtendedLimits ids 58/59/60 (ADR-0002). |
| Missing | Catalog fields for class, tier, availability rule and gunplay profile reference (F3). Gunplay gate extended from `weapons[]` to the catalog (F2). A generator or checker for `WeaponInfo.xml` fields (F11). Availability controls: no code today touches gun shop stock, pickups or NPC loadouts (nothing in `src/` or `docs/game-api/NATIVES.md` does); `docs/research/` has no note on them, so the availability rules stay open questions on the card. A list/give command for catalog weapons (`give <id>` exists in the autopilot; no `catalog` listing). Scenario `stage1-arsenal`. |
| Risks | Replacement ids 7 to 17 are vanilla ids that missions and NPCs also use: a Liberty profile on id 14 changes every AK in the game, including mission-given ones (rule 2 says weapons outside the Stage 1 catalog stay vanilla, which the catalog satisfies by construction, but NPC and mission weapons inside it change too). Adding more than three ExtendedLimits ids is unproven (ADR-0002 registered three). |
| Contradicts | F3. |

### T-042 Gunplay tuning and shoulder swap

| | |
|---|---|
| Reuse | `Gunplay/Recoil/` (`RecoilSolver`, `RecoilMultiplier`), `Gunplay/Spread/` (`SpreadModel`, `ShooterState`, `SpreadCalibrator`, `ShotAuditStats`, `ShotGeometry`), `Profiles/` (per-weapon `RecoilProfile`/`SpreadProfile`, `GunplayConfigValidator` 13 KB, live tuning list `tuning[]`), `GunplayController` (58 KB: shot detection, audit, calibration, camera kick), `Aim/ShoulderSwap.cs` (LB / Z, 180 ms slide, `requireAiming`, persisted side, `Restore`), `Aim/FreeAimMode.cs`, `GameApi/AimCamera*.cs` (camera settings table found by static analysis, T-015), `GameApi/BulletLog.cs`. The `gunplay_state` log line every 5 s carries `cone=`, `accuracy=`, `gain=`, `shots=`. Test range: `DevTools/TestRange/TestRangeService.cs`, `gunplay.json` `testRange` (targets at 5, 10, 25, 50 m). |
| Missing | Profiles for the Stage 1 catalog (F2). A measurement scenario that logs bullet events for a first shot at 25 m, a 3-round burst and a 30-round burst and computes cone, recovery and climb (`stage1-gunplay-range`): `BulletFired` events and the shot audit exist, the analysis and scenario do not. Shoulder swap "finishing": the card T-015 is NEEDS-PLAYTEST with a manual clipping report outstanding; nothing in the scenarios exercises swap at walls (no scenario mentions it). No stance-specific caliber model beyond the multipliers in the profile blocks. |
| Risks | Spread is applied by writing `accuracy` into the live WeaponInfo entry and calibrating a gain from measured bullet deviation (`spreadCalibration.autoCalibrate`): a tuning number in the JSON is not the delivered cone until the calibrator has settled, so the acceptance cones must be measured, not read from config. The camera kick is checked against the real camera (`recoilGlobal.cameraValidationSamples`/`cameraValidationToleranceDegrees`). Shoulder swap writes a game data table found by static analysis of GTAIV.exe 1.2.0.59 (T-015, `docs/game-api/MEMORY.md`); it restores on unload. |
| Contradicts | Nothing; F2 only. |

### T-043 Weapon-specific reticles

| | |
|---|---|
| Reuse | `Gunplay/Crosshair/CrosshairRenderer.cs` (four segments; gap = live cone via the game's own projection, measured each frame from `GET_VIEWPORT_POSITION_OF_COORD`; opens instantly, closes with exponential smoothing), `Profiles/CrosshairSettings.cs`, `config/gunplay.json` `crosshair` (`replaceVanillaReticle`, `showForVanillaWeapons`, sizes, colours, `outlinePixels`, `gapSmoothingPerSecond`), `GameApi/HudReticle.cs` (vanilla crosshair/dot/health/armour targets hidden by hud.dat globals, restored on disable), `IsCrosshairWeapon` (sniper scope untouched), drawing via ScriptHookDotNet `PerFrameDrawing` in `GunplayController.OnDraw`. Draw-thread rules already followed (state gathered on the tick). |
| Missing | Everything class-specific: styles (only one exists), a `reticles` config section and validator, per-weapon overrides, visibility rules (hip/aimed, vehicle, cutscene), pellet ring sizing. A per-frame log of drawn opening vs cone for the truthfulness check (the drawn gap is clamped to `minimumGapPixels` 4 and `maximumGapPixels` 180 and smoothed, so it can legitimately differ from the cone: the check must compare against the same clamp). Draw cost is not metered (F8). |
| Risks | It draws through ScriptHookDotNet's `Graphics`, not `Liberty.Ui.Canvas`: two draw paths for UI (Canvas text and sprites vs primitive rectangles). The cone shown is the calibrated accuracy read back, for weapons without a profile it is the vanilla accuracy, so classes for catalog weapons depend on F2. Sniper scope is the game's (`HUD_WEAPON_SCOPE`); restyling needs art or research. |
| Contradicts | Nothing. |

### T-044 Physical weapons (2 long guns + 1 sidearm)

| | |
|---|---|
| Reuse | `Arsenal/ArsenalCore.cs` (reconcile loop: observes the inventory, moves overflow to storage, tracks owned vs mission gains, busted/wasted loss via `ArsenalPolicy.ResolveLoss`), `Logic/ArsenalPolicy.cs` (`OverflowIndex` by group and limit), `Logic/CategoryRule.cs`, `config/arsenal.json` (limits, category to group and body slot), `Arsenal/Holsters/HolsterController.cs` (props attached to bones, hidden in vehicles/cutscenes/faded screens via `HolsterRules.Visible`, journal-based recovery of orphaned props after a crash, nudge tools), `config/holsters.json` (weapon list, placements per body slot, slings `lf_sling_a/b`), `config/models/sling.json`, `Arsenal/Contracts/*` (`CarriedWeapon`, `BodySlot`, `ICarriedWeaponsSource`), scenario `sling-review`. |
| Missing | The decided limits (F4: sidearm limit 1, and where SMGs go). Ammunition limits: no carried-ammo cap exists in `arsenal.json`. Placement per weapon size/class and per outfit (placements are per body slot only, one set for all outfits). Draw/holster transitions (none: the holster module only shows/hides props). Vehicle enter/exit stress scenario (`stage1-loadout-vehicles`). The holster weapon list ends at id 60 and only knows ids in `holsters.json`: catalog weapons not listed there are not shown. |
| Risks | `holsters` hides in vehicles by `IS_CHAR_IN_ANY_CAR`; cutscene detection is by `IsPlayerControlOn`/fade only. Holster ticks are 50 ms (`Interval = 50`), so a weapon change shows after up to 50 ms. Props are real game objects (pool `objects`), each with an ADR-0005-style journal; 100 enter/exit cycles is exactly what the journal exists to survive but has not been measured. |
| Contradicts | F4. |

### T-045 Weapon wheel

| | |
|---|---|
| Reuse | `Engine/Ui/RadialMenuView.cs` (8-or-fewer segments, right stick picks, D-pad steps, A/X/Y/B, 160 ms fade-in, segment labels/icons/badges evaluated on the tick into a snapshot for the draw pass), `RadialArt.cs` (ring, highlight and centre disc textures), SDK `IUi.OpenRadial`, `IUi.WeaponIcon(id)` (icons from `scripts\LibertyFramework\ui\icons\<id>.png`), `Arsenal/Ui/StorageWheel.cs` (category wheel used at trunks and safehouses), `Canvas` virtual 1280x720 with text styles 14, 16, 18, 22, 30 px (the smallest is 14 px, meeting the >= 14 px criterion), scenario `ui-review`. |
| Missing | A wheel that is opened **in play** by a held input: nothing opens a menu from a bare button today (DevTools uses F10 or L3+R3; the storage wheel opens from the interaction prompt). Segments for the physical loadout instead of the 8 inventory categories. Ammo clip/reserve, finish, category text. Bindings in config that avoid the game's weapon cycling, phone and cover inputs (`gunplay.json` `switchWhileAiming` already binds D-pad left/right to cycle weapons while aiming). Mouse selection (F10). Opening latency measure (open to first drawn frame) and draw cost (F8). |
| Risks | Menus lock player control while open (`RadialMenu.LockPlayerControl`); a hold-to-open wheel that locks control would freeze combat, and the game does not slow time. Control capture by another menu (DevTools, storage) needs an ownership rule. |
| Contradicts | Nothing. |

### T-046 Trunk UI

| | |
|---|---|
| Reuse | `Arsenal/Ui/TrunkSequence.cs` (engine choreography: turn, `amb@car_stash` open_boot, idle while browsing, `boot_withdraw` per move, `car_boot` close_boot; timings from `arsenal.json` `trunkTimings`), `StorageWheel`, `Logic/StorageBin.cs`, `Logic/LvsOwnedVehicleReader.cs` (Liberty Vehicle Services ownership), `Logic/TrunkTimings.cs`, `ArsenalCore` store/take actions (`Store`/`Take`, `OverflowDestination`), scenario `trunk-review` (opens, moves segments, stores, closes; passing). |
| Missing | The two-column carried/trunk layout, capacity per vehicle class (no capacity concept exists: a bin is an unbounded list), take/swap feedback, persistence proof after save/load, extension of `trunk-review` to take and swap. |
| Risks | The current wheel is category-based (one carried weapon per category); with a 2+1 loadout the mapping "category to slot" changes (F4). The `trunk-review` scenario holds each key for 1200 ms because a measured Windows run reached long frames (about 100 ms) with the wheel open and short presses were missed between input polls. |
| Contradicts | Nothing. |

### T-047 Harsh gore

| | |
|---|---|
| Reuse | `CombatEffects/` (`HitClassifier` bone to region, `BloodEffects` looped emitters and bounded pulses, `Dismemberment` + `Logic/LimbCutPlan` on the ADR-0005 skeleton hooks, `PedInjuryState`), `config/combat_effects.json` (about 100 fields: effect names, scales, durations, `maximumEmitters` 64, `maximumLoopedEffects` 32, `maximumSeveredPeds` 10, corpse limb lifetime 120 s), the SDK exact events `PedDamaged` (attacker, weapon, `DamageType`, bone, amounts, `Killed`, hit position and direction; ADR-0007) and `PedDied`, SDK `IFx`, `ITasks` (`Cower`, `Flee`, `HandsUp`), DevTools gore page and scenario `gore-review`, `exact-damage`, `docs/sdk/examples/BleedOutModule.cs` (a cower-before-death module on the SDK). |
| Missing | Replacing the polling attribution with `PedDamaged`/`PedDied` (F1); this also removes the player-only shooter limit (NPC-on-NPC violence gets effects) and the 40 m / 24-ped cap. Wounded-survivor behaviour: the only tasks the SDK has are cower, flee, hands up; there is no crawl/writhe task or animation in `docs/game-api` (open research question, rule 4). Whole-body persistence: bodies are game-managed; the config only limits severed peds (10) and emitters (64/32); the 3 to 5 minute body lifetime and a decal cap have no code. Shotgun/head trial scenarios with success counting (95%/90%/50 trials). Blood pools and trails wait for R3 (T-051). |
| Risks | `HAS_CHAR_BEEN_DAMAGED_BY_CHAR` and last-damage-bone are what the current code trusts; the exact hook (ADR-0007) is a code hook in the game's damage routine, validated at startup, and when it is unavailable `PedDamaged.Exact` is false (inferred). The gore module must handle `Exact == false` explicitly. Dismemberment writes skeleton state through two after-call hooks (ADR-0005); "0 floating or flashing limbs in 50 trials" is a new measurement. |
| Contradicts | F1. `allowedWeaponIds` (58/59/60) plus `allFirearms: true` means every firearm already qualifies, so the id list is dead configuration. |

### T-048 Contextual combat effects

| | |
|---|---|
| Reuse | `CombatEffectsController.OnDamage` already chooses entry effects by weapon slot (shotgun and sniper have their own entry/chunk effects), `BloodEffects`, `IFx`/`FxService` (particle triggers by name), the SDK `BulletFired` event (shooter, weapon, from, to), `IPerf` cost sections, `config/combat_effects.json`. Art queue: ART-005, ART-006 filed. |
| Missing | Muzzle flash, smoke, casing and spark sets per weapon class: nothing triggers muzzle effects today (only blood effects exist). Night muzzle light: no light native is listed in `docs/game-api/NATIVES.md` (research: FusionFix `natives.ixx`). Impact material: R2 (T-050). A `stage1-effects` scenario. Shared effect caps with T-047. |
| Risks | Particle names are resolved by the game at trigger time; the CE playtest found that looping effects are refused by `TRIGGER_PTFX` and `START` returned zero for tested blood loops (BloodEffects comment), so any looping muzzle/smoke effect is a spike. |
| Contradicts | Nothing. |

### T-049 Basic Liberty HUD

| | |
|---|---|
| Reuse | `IUi` (help box, notifications, subtitles, canvas per module, radial and list menus), `Canvas` (1280x720 virtual, text 14 to 30 px, sprites, rectangles, lines, opacity), `Engine/Ui/TextureStore`, `HudReticle` + `GameAddresses` hud.dat resolver (`HudComponentArray` 0x118E7F8 found by pattern, per-component alpha and size globals), `IPlayer`/`IPeds` health, armour, wanted, ammo readouts (`Weapons` ammo), `IUi.SetHudVisible`. |
| Missing | Finding out which vanilla elements can be hidden alone (F9): only reticle parts today. Everything Liberty-drawn: health/armour widgets, ammo, wanted, prompts (the Arsenal uses the help box for its prompt: `ShowHelp`), fade timings, palette/typography definition in config. Glyph selection for pad vs keyboard. UI draw cost (F8). |
| Risks | The radar cannot be hidden separately from the HUD by any existing call (F9; R5/T-054). Hiding an element the game still updates can leave dead space or break tutorial and mission text. The canvas is drawn from ScriptHookDotNet's `PerFrameDrawing` (Direct3D 9 hook) under DXVK; F12 screenshots show it, but a full-screen alpha layer cost is R4. |
| Contradicts | Nothing. |

## 2. Reuse map, condensed

| Stage 1 task | Extends | New files expected |
|---|---|---|
| T-041 | `weapon-catalog.json`, `WeaponCatalog*`, `gunplay.json weapons[]`, `WeaponInfoTable`, install scripts | catalog fields, WeaponInfo generator/check, `stage1-arsenal` |
| T-042 | `Recoil/`, `Spread/`, `Profiles/`, `Aim/ShoulderSwap`, test range | `stage1-gunplay-range`, swap scenario, profiles in config |
| T-043 | `CrosshairRenderer`, `CrosshairSettings`, `HudReticle` | reticle styles, `reticles` config, `stage1-reticles` |
| T-044 | `ArsenalCore`, `ArsenalPolicy`, `CategoryRule`, `HolsterController`, `holsters.json` | outfit placements, ammo caps, `stage1-loadout-vehicles` |
| T-045 | `RadialMenuView`, `StorageWheel`, `IUi` | loadout wheel module, bindings, `stage1-weapon-wheel` |
| T-046 | `TrunkSequence`, `StorageWheel`, `StorageBin` | two-group UI, capacity, extended `trunk-review` |
| T-047 | `CombatEffectsController`, `Dismemberment`, `BloodEffects`, exact events | exact-event attribution, persistence caps, wounded behaviour, gore scenarios |
| T-048 | `CombatEffectsController`, `FxService`, `BulletFired` | muzzle/impact sets, `stage1-effects` |
| T-049 | `UiService`, `Canvas`, `HudReticle`/`GameAddresses` | HUD module, resolver for more hud.dat components, `stage1-hud` |

## 3. Suggested order inside lanes (unchanged from the task queue)

The lane order in `docs/tasks/README.md` stands. Two changes to consider by the owner:

1. T-047's exact-event rewrite (F1) is independent of T-041 and can start right after this task; T-048 shares its
   effect caps and should follow it, as the queue already says.
2. F5 (density governor default on, floors below the Pillar 4 limit) needs an owner decision before Slice B: keep it as a
   protective governor with a raised floor, or switch it off by default.
