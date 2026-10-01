# T-045 â€” Liberty weapon wheel

Status: **NEEDS-PLAYTEST** Â· Lane B Â· Depends on: T-044 Â· Design: STAGE1 section 7 Slice A "Weapon wheel", Slice C

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

- Open to first drawn frame â‰¤ 1 frame, open animation â‰¤ 200 ms, UI draw â‰¤ 0.5 ms.
- Scenario `stage1-weapon-wheel`: open, select each slot, confirm the equipped weapon each time (100%), close; with
  controller-style and keyboard input; screenshots for review.
- Smallest text â‰¥ 14 px at 1280x720 virtual.

## Human test steps

Owner instructions currently prohibit game testing. Do these steps only after the owner explicitly reauthorizes it; source/offline results do not establish gameplay acceptance.

1. In free play on foot, carry pistol, shotgun, AK and knife. Hold **Tab** about one second, press **Right**, release **Tab**: the wheel closes and equips the highlight. Tap **Tab**, release, press **Right**, then **Enter**: the tap keeps it open until confirm.
2. Repeat with controller **Back/View**, right stick or D-pad left/right, **A** to confirm and **B** to cancel. Check no unwanted vanilla phone/cycling action. Physical controller input remains unverified.
3. Repeat every filled slot; inspect next-frame `weapon_wheel_equipped ... match=True`. Confirm the empty thrown slot closes without changing weapon. Disable `weaponWheel.enabled` and check vanilla cycling.
4. Review labels, ammo and footer at 1280x720 and the owner's resolution. Once D is integrated, check top-right HUD and bottom-left radar coexistence. Run the bounded bisect and enforced budgets only after game testing is authorized; do not rerun/change the T-040 baseline.

## Codex continuation — September 30, 2026

Both original B tips are reconciled with main `f5679a5`, including `db08832`/`45561b4`, on `codex/lane-b-validation`.
See [Codex handoff](../handoffs/Codex-Lane-B-2026-09-30.md) for files, commits and exact restart instructions.

Actual elapsed press time replaces the 100 ms/frame cap. Confirm releases this menu's control lock before equip and reads
the actual weapon next frame. Bindings colliding with storage/navigation are rejected. Footer/ammo bounds are wider and
selection is amber. Shared diagnostics separate locked/no-draw, primitives, text and unlocked drawing, with `ui.input` and
`ui.snapshot` costs. UI gates enforce paired closed/open frame average/p95/p99, >=30 samples, no >=1 s stall and <=0.5 ms
combined `draw.ui` submission cost. Negative offline cases prove excessive draw/frame cost fail; thresholds remain proposals.

**Acceptance incomplete:** old list windows average 383–397 ms even with gameplay modules stopped; radial/wheel windows
average 825/840 ms. The old keyboard-highlight screenshot contains no wheel, so fails visual acceptance. Sidearm/empty-slot
captures render content with the old white highlight. Shared-path cause is unknown. The new diagnostic run produced no usable
measurements and was cleaned up/restored after the owner prohibited further game testing. No new fixes have game evidence.
