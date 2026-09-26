# T-032 — Collision: layout research tooling and borrowed collision

Status: **NEEDS-PLAYTEST** (queued: `PROBE-collision`, `PROBE-bounds-layout`, `T032-collision-borrow-report`,
`T032-collision-borrow`; `T033-world-walk` covers the player)

Depends on: T-030 (authored collision shapes), T-027 (engine raycast). Session 5 of
[NEXT_SESSIONS](../workflow/NEXT_SESSIONS.md).

## What can and cannot be done before the game's files are read

[Collision.md](../research/Collision.md) sets the order:
1. inventory;
2. pick the resource a prop uses;
3. decode that class and round-trip the game's own files;
4. only then write collision.

Nothing about the bounds classes is established in this repository. Writing collision from memory of other tools is
excluded (AGENTS.md rules 4 and 7). This session therefore builds everything up to step 3 and the one experiment that
needs no decoding:

- **Step 1**, `PROBE-collision`: already queued. The collision inventory and `propCandidates`.
- **Step 3's input**, `PROBE-bounds-layout` (`LibertyContent probe bounds`, `BoundsLayout.cs`). Per class (extension, RSC
  type, root word), it classifies every 4-byte word of the root structure across all the game's files, and the same one
  level down:
  - zero, system pointer, graphics pointer;
  - small integer, with its values;
  - float or other.

  It also bounds the root's size by its smallest pointer target. The report holds only word classes and small-integer
  values, never float values or geometry. The review session decodes one class from it with labels, then writes a
  decoding probe that round-trips the game's files.
- **Borrowed collision**, the experiment for C4–C6:
  - `asset.json` `"collision": { "borrow": { "archive", "model" } }` ships a vanilla prop's own bounds resource
    **unchanged** under the asset's name (`<name>.wbn`, packaged with its resource type).
  - `"model": "auto"` takes `PROBE-collision`'s first prop candidate (same rule, tested). That is the model
    `raycast-objects` spawns, so the two scenarios separate "the shape does not stop rays" from "our model is not paired
    with its bounds".
  - Read from the owner's files at build time, never committed.
  - Codes: LCC039 (borrowed) and LCC040 (authored shapes ignored with a borrow). The report has `compiled.collision`.
  - With `auto` and no candidate, the build ships without collision and says so, so one asset cannot fail the package.
- **Not done: the collision writer for authored shapes (step 4).** It needs steps 2 and 3 from the probe reports.
  Authored shapes stay refused with LCC032, and `CompilerCapabilities.CollisionShapes` stays empty until then.

## Built

- `tools/content/BoundsLayout.cs` (`probe bounds`) and `tools/content/BorrowedCollision.cs`.
- Manifest `collision.borrow`. `Program.Build` writes `<name>.wbn` (a stale one is deleted), and `package` adds it to the
  IMG.
- Capabilities `collisionBorrow: true`.
- Blender add-on 0.6.0: Collision Authored/Borrow with archive and model, written to `asset.json`; LBX034.
- Test asset `content/props/lf_col_crate`: the sample crate, v1 writer, borrowed collision. Scenario `collision-borrow`.

## Offline evidence

- **Content self-test** 319 → 344:
  - borrow candidates follow the probe's rule, and both name the same first candidate;
  - a named model, a missing one, and `auto` without candidates;
  - the `.wbn` is written unchanged, reported, and packaged with its resource type (32 in the test);
  - a stale `.wbn` is removed;
  - LCC039/LCC040 and world objects with a borrow;
  - bounds probe classes and word classification, float values never reported.
- **Blender:** 86 passed (borrow written to `asset.json`, LCC039 reported).
- **NEEDS LOCAL VERIFY:** everything in game and in the game's files (below).

## Open questions

1. Which classes exist and what their roots hold: `PROBE-collision` and `PROBE-bounds-layout`.
2. Does a model in `LibertyContent.img` get collision from a same-named `.wbn`, for a script-created object registered
   as `weap`? `T032-collision-borrow` (C4, C5).
3. Does the engine's raycast hit it? The same scenario (C6).
4. Does the player collide with it? `T033-world-walk`.

## Human test steps

In the same verification run as the other queued checks:
`./tools/verify-local.ps1 -Only PROBE-collision,PROBE-bounds-layout,T032-collision-borrow-report,T032-collision-borrow,T027-raycast-objects`

Expected:
- **Probes:** NEEDS-REVIEW (reports for the review session).
- **Report check:** PASS, with `compiled.collision.from` naming the borrowed prop.
- **`T032-collision-borrow`:** PASS, or FAIL at a `rayto` step. Both are answers. A FAIL with `T027-raycast-objects`
  passing means the pairing, not the shape, is missing. The screenshot shows the crate.
