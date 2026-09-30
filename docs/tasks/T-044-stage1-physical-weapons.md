# T-044 — Physical weapons: 2 long guns + 1 sidearm

Status: **READY** · Lane B · Depends on: T-040 (audit) · Design: STAGE1 section 7 Slice A "Physical weapons", 12.1,
10 Pillar 3

## Goal

Turn the Arsenal/holster/sling test features into the production physical loadout: **2 long guns + 1 sidearm**, one
long gun equipped at a time, the other slung and visible; everything else lives in the trunk or safehouse.

## Starting point (read first)

`src/LibertyFramework/Arsenal/` (`ArsenalCore.cs`, `Logic/ArsenalPolicy.cs`, `Logic/CategoryRule.cs`,
`Holsters/HolsterController.cs`, `Holsters/Logic/*`), `config/arsenal.json`, `config/holsters.json`,
`config/models/sling.json`, scenarios `sling-review` and `trunk-review`. The current policy carries more slots
(two sidearms, melee): change it through config and `CategoryRule`, not a rewrite.

## Scope

- Loadout rules from config: 1 sidearm, 2 long guns, limited carried ammunition. Melee and thrown weapons keep their
  current rules unless the owner decides otherwise (open question below). Overflow and ownership rules unchanged.
- Placement per weapon size and class (back/sling), both long guns visible without intersecting each other.
- **Vehicles and cutscenes:** carried props hidden or stowed on vehicle entry and in cutscenes, restored on exit; no
  orphaned or floating props.
- **Draw/holster transitions:** use the game's own animations where they exist (research the animation dictionaries,
  rule 4); otherwise a clean hide/show timed to the weapon switch.
- **Outfits:** placement offsets per Niko outfit class so straps and guns do not clip; review screenshots per outfit.
- Mission weapons, busted/wasted and save/load keep working (Arsenal already tracks mission flags).

## Acceptance (STAGE1 Pillar 3)

- 100% visible on foot; hidden in vehicles and cutscenes; 0 orphaned props after 100 autopilot vehicle enter/exit
  cycles (new scenario `stage1-loadout-vehicles`).
- Clipping: front/side/back screenshots for every Stage 1 outfit class and weapon class; owner pass.
- Inventory integrity: 0 lost owned weapons across 50 death cycles; identical state after save/load.
- Budget with T-042's: gunplay + arsenal + holsters ≤ 1.5 ms average.

## Open questions

1. Melee and thrown weapons: keep one melee slot and uncounted thrown weapons (current rule)? Ask the owner in the
   final report; do not block on it.

## Human test steps

Fill in when done.
