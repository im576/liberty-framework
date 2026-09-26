# Collision (bounds) in GTA IV: what is known

**Status (2026-09-26): research not started in this project.** Nothing below is verified here. It sets out what to
establish before any collision writer is built (roadmap: content compiler step 3, `docs/content/README.md`).

Evidence labels: **VERIFIED IN GAME**, **VERIFIED OFFLINE** (from the owner's game files by a repository tool),
**PLAUSIBLE** (public community documentation, not checked here), **UNKNOWN**.

## Claims

| # | Claim | Label | Source |
|---|---|---|---|
| C1 | Static world collision ships as RAGE bounds resources in the map IMG archives: `.wbd` (a dictionary of bounds) and `.wbn` (one bounds object). | PLAUSIBLE | Community documentation of IV formats. To be checked by `PROBE-collision` |
| C2 | Fragment types (`.wft`: breakable props, vehicles) carry their own bounds inside the fragment. | PLAUSIBLE | Community documentation |
| C3 | Bounds classes include composite, geometry and primitive shapes (box, sphere, capsule). The class layouts in IV's resources are not documented here. | PLAUSIBLE / layouts UNKNOWN | Community documentation |
| C4 | How the game pairs an IDE object with its collision: a bounds resource of the same name, the map section's `.wbd`, or a fragment. | UNKNOWN | Needs the inventory, then an in-game test |
| C5 | Whether a script-created object (`CREATE_OBJECT`, `spawnprop`) gets collision from static bounds, or only from a fragment. | UNKNOWN | In-game test |
| C6 | Whether the engine's raycast (ADR-0008, the game's line test against the physics world) hits an authored object's collision. | UNKNOWN | Follows from C4/C5; `RayMask.Objects` is also untested against vanilla props |

## Plan

1. **Inventory (queued):** `PROBE-collision` (`LibertyContent probe collision`) lists which collision-like resources the
   game ships, in which archives, how many, with their RSC types, sizes and root vtable words. Structure only.
2. **Decide the target:** from the inventory, pick the resource a single static prop would use (C4). If props rely on
   fragments, that changes the plan (WFT is a deferred compiler expansion).
3. **Decode one class:** a follow-up probe that parses that resource type's root structure across all files and
   round-trips it (the method `wtdcheck` and the drawable self-test use: rebuild the game's own files byte for byte).
4. **Writer:** only after step 3 round-trips the game's files.
5. **In game:** a scenario that places an authored object with collision and proves player collision, vehicle collision
   and a raycast hit (C5, C6).

Nothing here may be implemented from memory of other tools (AGENTS.md rules 4 and 7).
