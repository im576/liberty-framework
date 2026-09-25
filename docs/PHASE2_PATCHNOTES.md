# Phase 2 integrated patch notes

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
