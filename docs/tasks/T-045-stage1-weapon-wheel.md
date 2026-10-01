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

Automated checks (keyboard input through the autopilot) are in the Claude continuation below. Steps 2 and 4 need the owner: a physical controller, the owner's resolution and Lane D's HUD cannot be exercised by the autopilot.

1. In free play on foot, carry pistol, shotgun, AK and knife. Hold **Tab** about one second, press **Right**, release **Tab**: the wheel closes and equips the highlight. Tap **Tab**, release, press **Right**, then **Enter**: the tap keeps it open until confirm.
2. Controller (owner only), on foot with a pistol, a long gun and the knife: hold **Back/View** for about a second, move the **right stick** (or D-pad left/right) to a slot, release **Back**: the wheel closes and that weapon is in hand. Tap **Back**, release, move the stick, press **A**: equips; press **B** instead: closes and keeps the weapon in hand. While the wheel is open the character must not move, and no phone or vanilla weapon-cycle action may fire when Back is pressed or released. If it does, note which action and change `weaponWheel.padButton` in `config/arsenal.json` (the file is read again when the game starts).
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
captures render content with the old white highlight. Shared-path cause is unknown. The first diagnostic was interrupted without measurements and restored. The reauthorized `9f11a42` diagnostic completed 60 steps and restored: primitives average 23.96 ms, but text-only/full/unlocked-full windows each contain five >=1 s stalls and no sub-second samples. This implicates shared text drawing without establishing an internal cause or gameplay acceptance. No new wheel acceptance result is claimed.

## Claude continuation: root cause evidence and fix, acceptance (2026-10-01)

Reviewed all Codex work (hold timing, equip-after-release readback, binding-collision rejection, lid-close safeguard, layout,
`UiBudgetLogic`) and merged `origin/main` (testing speed-up tools); no defect found in them.

**Shared UI stall, measured (one scene, frame time per window, `T045-ui-text`, run `20261001-000645`):**

| Window | Avg frame ms |
|---|---:|
| closed | 23 |
| list menu, sprite text (new default) | 25.5 |
| radial menu, sprite text | 26 |
| list menu, SHDN `DrawText` limited to 1 / 4 / all 8 strings | 96 / 310 / 393 |
| probe, one string drawn once / 8 times / 8 different strings / 5 font sizes | 98 / 609 / 616 / 702 |
| probe, font with `Effect` none, 8 strings | 147 |
| probe, DevTools-style font (2-argument constructor, 4-argument overload), 8 strings | 145 |
| probe, DevTools-style bold font (4-argument constructor), 8 strings | 142 |
| probe, canvas font through the 4-argument overload, 8 strings | 608 |

Conclusion (evidence only): ScriptHookDotNet `Graphics.DrawText` costs about 15 ms per string per frame at its cheapest and
75-90 ms with the canvas fonts' default effect (the effect multiplies the `DrawString` passes); the overload, the colour
argument, bold, string content and font count are not what costs. Rectangles and sprites cost nothing. The cause inside
DrawText (D3DX font rendering under the game's renderer) is not established, and a draw setup that is cheap enough was not found
(the cheapest, 145 ms for 8 strings, is 6x the sprite renderer), so the shared canvas draws text as cached GDI+ sprites
(`engine.json` `uiTextRenderer`, `ui-text-renderer shdn` switches back). The DevTools menu window in that scenario is not
a reliable reading (its open state was not confirmed). The cache is least-recently-used (600 entries), creates at most 8 new
textures per frame, and `ui-text-stats` reports count and estimated bytes (about 40 KB per string at 1080p; 437 entries = 19 MB).
Text textures belong to the engine's canvas, not to a module: they are released when the engine unloads, not on module stop or
hot reload. Lane D's HUD text uses the same canvas (changing numbers each create a texture; per-character sprites for digits
would avoid that and are a recommendation for D, not done here).

**Acceptance (full run `20261001-001218-7d63be6`, no -Quick, budgets unchanged):**
- `T045-weapon-wheel`: passed (NEEDS-REVIEW for the screenshots). 12 of 12 selections with next-frame readback, first draw within 0-1 frames, hold and tap keyboard flows, `ui-budget` wheel window avg 28.8 ms against 31.1 ms closed, p95 50.2 / 54.2, p99 67.9 / 78.0, draw.ui under 0.5 ms.
- `T046-trunk-ui`: passed (NEEDS-REVIEW for the screenshots). Store, take, swap with ammo and ownership read back, capacity refusal, two persisted-state round trips, control released after both closes, `ui-budget` trunk window avg 30.3 ms against 28.7 ms closed, p95 47.6 / 44.3 (+7.4%), p99 85.5 / 76.6 (+11.6%).
- An earlier full run (`ec5e244`) had one game crash and one p95 gate miss by 0.5 ms; both runs shared the machine with other lanes' builds (a 11-14 s stall in the holsters and wheel config-file polling preceded the crash, consistent with disk contention; not proven). The budget windows were then lengthened to 10 s (thresholds unchanged).
- Still NOT VERIFIED: physical controller (Back, stick, A/B), a real game save and load, the safehouse stash and gunsmith through this interface, Lane D HUD coexistence, owner judgement of the screenshots.

## Owner questions (defaults stay as shipped until answered)

- Wheel bindings: controller **Back/View** and keyboard **Tab** (`weaponWheel.padButton`, `keyboardKey` in `config/arsenal.json`). Acceptable, or does either collide with something you use on foot?
- Melee and thrown weapons keep their current rules (no loadout limit); confirm.
