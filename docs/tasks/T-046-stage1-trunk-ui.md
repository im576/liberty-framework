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
