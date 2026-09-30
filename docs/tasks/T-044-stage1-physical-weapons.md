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

## Progress notes (Claude, lane B, 2026-09-30)

Built (branch stage1/T-044; evidence is added here as game runs finish):

- **Loadout rules** (config/arsenal.json block loadout, Arsenal/Logic/LoadoutRules.cs, ArsenalPolicy): 1 sidearm + 2 long guns, SMGs count as long guns (they are slung, the one sidearm slot is the handgun), ammunition caps per category. The block is optional and enabled: false restores the old 2 + 2 + 1 rules, so installs that keep their old rsenal.json still load (packaging adds the block). Overflow rule unchanged (least recently used goes to the car trunk or safehouse stash).
- **Holsters** (HolsterController): reacts in the frame of the engine events (PlayerWeaponChanged, vehicle enter/exit, CutsceneChanged) instead of the next 50 ms tick, reads the weapon in hand live, hides for cutscenes through the event, clears its props when the module stops (mod-off runs left props behind before), and has a holsters command (status, outfits, outfit <component> <drawable>, outfit restore).
- **Outfits**: Niko has 17 upper-body drawables (LibertyModel outfits on playerped.rpf): the torso stands 0.101 to 0.145 m behind Char_Spine2 at the gun's height. The ulky class (0.13 m or more: drawables 0, 1, 2, 3, 8, 9, 14) moves both slung guns 3 cm further back (outfitClasses, loadoutPlacements in config/holsters.json). Component 1 being the upper body is checked in game by the outfits scenario.
- **Tools and tests**: autopilot commands strip, uy, die, enter, leave, cycle-vehicle, cycle-weapons, cycle-deaths; engine command rsenal (status, oundtrip); 	ools/verify/LoadoutChecks.cs (limits, caps, overrides, 50 death cycles of the loss rule, save/load identity, placement specificity); generator 	ools/perf/New-LoadoutScenarios.ps1 (outfit and weapon-class reviews).
- **Scenarios and queue checks** (	ests/local/checks.json): T044-loadout-vehicles, -weapons, -deaths-a, -deaths-b, -outfits, -review.
## Human test steps

Fill in when done.
