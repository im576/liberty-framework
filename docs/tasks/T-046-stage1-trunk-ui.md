# T-046 â€” Trunk UI: loadout â†” vehicle storage

Status: **NEEDS-PLAYTEST** Â· Lane B Â· Depends on: T-044; shares components with T-045 Â· Design: STAGE1 section 7 Slice A
"Trunk UI", Slice C

## Goal

A production trunk interface that makes the physical inventory legible: what Niko carries on one side, what the car
holds on the other, and moving a gun visibly changes where it physically is.

## Starting point

`Arsenal/Ui/TrunkSequence.cs` (choreography: turn, open boot, reach in, close), `Arsenal/Ui/StorageWheel.cs`,
`Logic/StorageBin.cs`, `config/arsenal.json` (`trunkTimings`), scenario `trunk-review` (passes; it faces north before
spawning the car and accepts whichever weapon the wheel starts on).

## Scope

- Two groups: **carried** (sidearm, long guns, ammunition, equipment) and **trunk** (stored weapons, ammunition,
  capacity used/available per vehicle class from config).
- Store, take and swap (when the carried slot is full) with clear feedback; the slung prop disappears or appears on
  Niko as it moves; the choreography keeps playing its reach-in step per move.
- Controller and keyboard/mouse navigation; the same visual components and language as the weapon wheel (T-045).
- Safehouse storage uses the same UI where it already uses the storage wheel.

## Acceptance

- `trunk-review` extended to store, take and swap: 100% round trips, state persisted after save/load.
- Open â‰¤ 1 frame, animation â‰¤ 200 ms, UI draw â‰¤ 0.5 ms; text â‰¥ 14 px at 720p virtual; screenshots for review.

## Human test steps

Owner reauthorized bounded game verification at 8:51 p.m. Pacific on September 30. Source/offline results alone do not establish gameplay acceptance; only checks that actually finish and restore before the 9:30 p.m. hard stop can supply new evidence.

1. Behind an Admiral on foot, press **E** (controller **X**); wait for lid animation and carried/trunk groups. Select the AK with arrows/right stick and press **Space** (**X**) to store; sling vanishes, trunk gains it with unchanged ammo, carried slot reads Empty.
2. Fill both long-gun slots, highlight the stored AK with **Page Up/Down** (**LB/RB**) and press **Enter** (**A**). Preview names the outgoing gun; check ammo/ownership/physical identity and sling changes. Repeat with a non-round ammo count.
3. Press **Backspace** (**B**), wait for lid close and movement to resume. Reopen/close repeatedly, including immediately after transfers. `storage state` must report open/closing/animation/locked false and control true with no other capturing menu.
4. Repeat on a Banshee: four stored guns fit; fifth store is refused and remains carried. Take/swap from full storage preserves inventory. Restart/load a save and confirm owned inventory/ammo/instances persist.
5. Repeat both devices at a previously verified/owner-marked safehouse, including gunsmith open/back. Review HUD/radar coexistence and enforced budgets. Actual game save/load, controller and safehouse gameplay remain open.

## Codex continuation — September 30, 2026

The trunk continuation includes the later wheel fixes and current main lock/cache tooling on `codex/lane-b-validation`.
See [Codex handoff](../handoffs/Codex-Lane-B-2026-09-30.md). Gameplay validation remains incomplete. The owner reauthorized bounded checks through 9:30 p.m. Pacific; any later session must follow its current instructions.

Storage uses the engine's owned player-control lock, releasing only Arsenal's claim on close. Completion explicitly shuts
the lid even if a missing animation skipped its timed close. Storage list starts below the top-right HUD band. Transfer logs
include ammunition, ownership and instance identity; the prepared scenario checks AK ammo 100 and displaced shotgun ammo 60
(existing T-044 cap), persisted equality, capacity refusal and control release after both closes. Offline serialization tests
include carried/trunk/safehouse ammo and ownership. Transfer/capacity policy is retained.
Successful swaps now request the reach-in choreography (the adapter previously handled only a `Taken` reply), and the
preview identifies a refused duplicate weapon type before confirm instead of promising a swap the inventory rejects.

Old `stage1-trunk-ui-20260930-174525`: 125 steps, one close failure; second close has no Back input logged. Five screenshots
were inspected: expected contents/capacity (0/8, 1/8, 4/4) and swap update are visible. Scene/vehicles obscure Niko and slings;
swap centre text clips, and the full-refused shot lacks the claimed transient refusal message. They do not establish prop,
close or new layout acceptance. There are no new screenshots after fixes; shared UI stalls remain open under T-045.

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
