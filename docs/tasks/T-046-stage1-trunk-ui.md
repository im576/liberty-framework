# T-046 — Trunk UI: loadout ↔ vehicle storage

Status: **READY** · Lane B · Depends on: T-044; shares components with T-045 · Design: STAGE1 section 7 Slice A
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
- Open ≤ 1 frame, animation ≤ 200 ms, UI draw ≤ 0.5 ms; text ≥ 14 px at 720p virtual; screenshots for review.

## Human test steps

Fill in when done.
