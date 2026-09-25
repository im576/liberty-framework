# Phase 2 — integrated gameplay overhaul

Owner direction (2026-09-24): build the full Phase 2 offline, then conduct one combined in-game playtest. Phase 1 gunplay, gold weapons, back holsters, and vehicle trunk storage were reported working; safehouse storage is provisionally assumed working. Weapon bloom rises too quickly and the game is laggy. Preserve the installed build until the game is closed and a staged Phase 2 package has passed offline checks.

## Workstreams and ownership

| Stream | Scope | Primary files | Integration boundary |
|---|---|---|---|
| A — foundations (this task) | Gun feel, performance measurement/refactor, contextual trunk/safehouse interaction, CE shoulder-swap spike | `Gunplay/Spread`, `GunplayController`, `Arsenal/ArsenalCore`, new interaction code, diagnostics | Publish hit events and weapon-instance contracts for B/C; no damage visuals or vehicle part implementation |
| B — combat consequences | Hit-location reactions, persistent injury, blood/wounds, bounded dismemberment | New `CombatEffects/` module, original/permissioned assets and config | Consume a narrow shot-hit event; no edits to core spread/recoil or LVS |
| C — ownership and customization | Physical weapon ownership, weapon catalog and attachment prototype; extend Liberty Vehicle Services CE with saved body parts | `Weapons/`, Arsenal weapon records/state, separately tracked MIT LVS extension and vehicle part assets | No edits to spread model, gore, or controller input |

Each stream uses its own `codex/phase2-*` branch and adds task cards, human test steps, build and verification evidence. Do not label features DONE before the owner's in-game test. Conflicting changes to `ArsenalCore` and packaging are integrated by stream A after the other streams land.

## Acceptance criteria

1. **Gun feel:** first shot stays accurate, short bursts remain useful, long sprays require correction; recoil, real bullet cone, and displayed crosshair agree. Record measured in-cone rate by weapon and investigate the current audit owner mismatch and calibration saturation before accepting a tune.
2. **Combat consequences:** shots to torso, head, arms, and legs trigger distinct reactions. Wounds and blood follow impact points. A bounded, non-player limb-loss prototype uses original or appropriately licensed assets and restores cleanly on despawn/reload. Story NPCs, missions, and player remain gated until verified.
3. **Weapons:** a physical weapon record persists identity, finish, ammo, ownership, and future attachment slots across body, trunk, safehouse, and owned vehicle. One non-gold replacement family and one add-on weapon demonstrate the catalog pipeline before scaling to all weapons.
4. **Vehicles:** the existing Liberty Vehicle Services CE workshop remains the ownership/persistence authority. One GTA IV-fitting car supports real body-part variants, preview, purchase, and saved restore. Test collision, damaged panels, trunk access, model streaming, and episodic load.
5. **Interaction:** a contextual controller prompt at the trunk rear and safehouse storage point opens a compact store/take/swap interface without opening DevTools. Input conflicts with cover, vehicle entry, phone, and missions are tested.
6. **Shoulder swap:** use only a CE 1.2.0.59 camera offset verified in a small spike. Save and restore the original offset across aim, cover, vehicle, mission camera changes, death, and script reload. If the control cannot be validated, keep the feature blocked and leave camera memory untouched.
7. **Performance:** capture repeatable frame times (median, 95th, 99th percentile) on the same drive and firefight with overlays and each module toggled separately. Instrument script tick costs and allocations; keep the full Phase 2 build within a measured budget before adding more assets or effects. No diagnosis from one FPS number.

## Reference boundaries

- Valve's *Second Shot* describes separate recovery behavior for tapping/bursting/spraying: https://blog.counter-strike.net/second-shot/
- Real Recoil Enhanced CE explains fire-rate-normalized kick: https://www.nexusmods.com/gta4/mods/1220 (author disallows redistribution/modification; study behavior only).
- Liberty Tweaks and Revamped Combat demonstrate GTA IV shoulder and injury behavior: https://github.com/catsmackaroo/LibertyTweaks and https://github.com/catsmackaroo/RevampedCombat (check licenses before reuse; no code copied).
- Violent Liberty demonstrates dynamic wounds/blood; its author says its dismemberment experiment is disabled: https://www.nexusmods.com/gta4/mods/1420 and https://www.reddit.com/r/GTAIV/comments/1wi7cak/violent_liberty_dynamic_blood_overhaul_launch/ (source and reuse permission unverified).
- Liberty Vehicle Services CE is MIT and provides our existing vehicle authority: https://github.com/ekzestean/Liberty-Vehicle-Services-CE
- PresentMon measures frame times and CPU/GPU busy: https://github.com/GameTechDev/PresentMon

## Packaging gate

Build and verify against the installed CE executable while the game is running, but do not deploy any DLL, asset, or config into the game until it is closed. Capture a rollback manifest and hashes; then install one integrated package for the owner's combined playtest. Keep each module independently disableable in config for performance isolation.
