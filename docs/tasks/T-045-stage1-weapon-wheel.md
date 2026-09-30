# T-045 — Liberty weapon wheel

Status: **READY** · Lane B · Depends on: T-044 · Design: STAGE1 section 7 Slice A "Weapon wheel", Slice C

## Goal

A weapon wheel that shows the **physical** loadout (what Niko carries), not every owned gun, and switches weapons.
Functional and in the Liberty visual language now; restyled with the final icon family in Slice C.

## Starting point

Liberty.Ui radial menus (`src/LibertyFramework/Engine/Ui/RadialMenuView.cs`, `RadialArt.cs`, SDK `IUi`), the Arsenal
storage wheel (`Arsenal/Ui/StorageWheel.cs`), weapon HUD icons extracted at install, scenario `ui-review`.

## Scope

- Open with a held input on controller and keyboard/mouse (bindings in config; must not collide with the game's own
  weapon cycling, phone or cover inputs); releasing or confirming equips the highlighted slot.
- Segments: sidearm, long gun 1, long gun 2, and the melee/thrown slots the loadout keeps. Each shows icon, name,
  category, ammunition (clip / reserve), finish; empty slots read as empty.
- Time slows or the game pauses while open only if the owner wants it (config, default off: GTA IV has no wheel slowdown).
- Visual language (STAGE1 section 7 Slice C): dark translucent, restrained amber accent for the selection, gritty
  readable type; nothing that reads as GTA V's wheel.

## Acceptance (STAGE1 Pillar 1)

- Open to first drawn frame ≤ 1 frame, open animation ≤ 200 ms, UI draw ≤ 0.5 ms.
- Scenario `stage1-weapon-wheel`: open, select each slot, confirm the equipped weapon each time (100%), close; with
  controller-style and keyboard input; screenshots for review.
- Smallest text ≥ 14 px at 1280x720 virtual.

## Human test steps

Fill in when done.
