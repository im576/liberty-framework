# T-027 — Engine raycast and line of sight (SDK 1.1)

Status: **NEEDS-PLAYTEST**

Roadmap: M3 WorldQuery layer 2. Decision: [ADR-0008](../architecture/decisions/ADR-0008-engine-raycast.md).
Research: [Raycast.md](../research/Raycast.md). Continues the research spike in commit 13ff3c2.

## Goal

Mods and engine modules can ask "what is between these two points?" and "can this ped see that one?" using the
game's own collision, with filtering by kind (world, peds, vehicles, objects) and ignored entities, safely (resolved
address, SEH-contained, off after a fault, engine tick only).

## What was built (2026-09-26, Claude)

- **Core ABI 5** (`native/LibertyCore`): `lc_raycast` with an `accept` mask, up to 4 ignored entities, pass-through
  walk (`src/ray_walk.h`), the verified hit→entity link `[instance+0x0C]`, the game's `bool` return read from `al`
  only, counters (`lc_raycast_stats`), fault → raycasts off for the session. Unit test
  `tests/ray_walk_test.cpp` (13 cases), built and run by `tools/build-core.ps1`.
- **SDK 1.1:** `IWorldQuery.RaycastAvailable`, `Raycast(from, to, RayMask[, RayIgnore])` → `RayHit`,
  `HasLineOfSight(from, to, blockers, ignore)`, `HasLineOfSight(viewer, target)`; types `RayMask`, `RayHit`,
  `RayIgnore`, `RayStatus`, `RayEntityKind`.
- **Engine:** `WorldQueryService` implementation; raycasts refused outside the engine thread or during the draw pass;
  `engine.json` `raycastEnabled`, `raycastMaxPasses`, `raycastPassStepMeters`, `raycastMaxLengthMeters`; commands
  `lf ray`, `lf raystats`; research `lf raydebug` (raw hit, now with a height argument) and `lf raybits` (bits 0–31);
  inspector shows ray counts; `lf engine` shows `raycast=on|off`.
- **Tests:** SDK self-test gains `raycast-available`, `raycast-ground`, `raycast-clear`, `raycast-invalid`,
  `raycast-ped`, `raycast-pass-through`, `raycast-ignore`, `line-of-sight-ped`, `raycast-vehicle`,
  `line-of-sight-vehicle`. New autopilot scenario `raycast`; `raycast-spike` now casts at the car at door height.
  Autopilot commands `rayto` and `los`.

## Build evidence

- `native/LibertyCore` builds with llvm-mingw 20260922 (i686, C++20, `-Wall -Wextra -Werror`); exports
  `lc_raycast`, `lc_raycast_install`, `lc_raycast_stats`. `ray_walk_test` passes 13/13.
- `Liberty.Sdk`, `LibertyFramework.net` and `Liberty.Autopilot` compile with warnings as errors.
- These builds ran in a Linux container (mono `mcs` against the pinned ScriptHookDotNet 1.7.1.8 binary, the Linux
  build of the pinned llvm-mingw). The owner's `tools/build.ps1`, `tools/build-core.ps1` and `tools/verify.ps1` still
  need one run on Windows before packaging (the verifier needs `GTAIV.exe`).

## Human test steps

The autopilot runs everything; you only start it and read the reports.

1. Close GTA IV.
2. In PowerShell at the repository root, build: `./tools/build-core.ps1`, then
   `./tools/build.ps1 -ScriptHookDotNetReference <ScriptHookDotNet.asi>`. Expect `ray_walk_test passed` and no errors.
3. `./tools/verify.ps1 -GameDirectory '<GTAIV folder>'`. Expect all checks to pass, including `line test function`
   and `physics world global`.
4. Package and install: `./tools/package-phase2.ps1 ...` then `./tools/install-phase2.ps1 -GameDirectory '<GTAIV folder>'`
   (same arguments as usual; game closed).
5. Run `./tools/autopilot/Run-Scenario.ps1 -GameDirectory '<GTAIV folder>' -Scenario tools/autopilot/scenarios/raycast.txt -OutputDirectory <runs folder>`.
   Expect every step `OK` in `report.md`. The `vehicle.png` screenshot shows the admiral in front of Niko;
   `ped.png` shows one pedestrian facing him.
6. Run the same command with `-Scenario tools/autopilot/scenarios/sdk-selftest.txt`. Expect
   `selftest_done passed=N failed=0`, with the ten `raycast-*` / `line-of-sight-*` checks `ok`.
7. Run it once more with `-Scenario tools/autopilot/scenarios/raycast-spike.txt` (research: include bits for the
   vehicle; no pass/fail beyond the spawns).
8. Optional, by hand in game: open the console, stand facing a wall and type `lf ray forward 20`. Expect
   `status=Hit kind=World` and a distance matching the wall. Face a parked car at arm's length and type
   `lf ray forward 5 -0.3`; expect `kind=Vehicle`. Type `lf raystats`; expect `faults=0`.
   If the console answers `status=Unavailable` and the log has `engine_raycast_outside_tick`, ScriptHookDotNet runs
   console commands off the engine thread; that is the guard working, not a failure. Use the autopilot path above.

Report with `docs/testing/PLAYTEST_REPORT_TEMPLATE.md`, attaching the three `report.md` files (they include the log).

## What to look for if something fails

- `engine_raycast installed=False` or `raycast=off`: the resolver or the core failed; the startup log names why.
- `engine_raycast_disabled fault_number=N`: the game's line test faulted and was switched off; the matching
  `engine_core_fault number=N in=raycast line test` line has the addresses.
- Vehicle checks fail with `kind=World`: the vehicle's entity is not at `[instance+0x0C]` (open question R1). The
  `raydebug forward 12 FFFFFFFF 1 -0.3` line in the spike run shows where it is (`link=`).

## Open questions

R1–R6 in [Raycast.md](../research/Raycast.md#6-open-questions). Objects (props with collision) are not yet tested in
game; `RayMask.Objects` relies on the same pool lookup as peds.
