# T-030 — Content compiler authoring side: materials, LODs, collision, world objects

Status: **NEEDS-PLAYTEST** (the owner's PC runs `LOOP-content-selftest` and `LOOP-blender-tests`; nothing here runs in
game)

Depends on: T-028 (validator IR groundwork). Session 3 of [NEXT_SESSIONS](../workflow/NEXT_SESSIONS.md).

## Scope

Everything between Blender and the writers that needs no game files. **No new writer output**: the compiler builds
exactly what it built before, and refuses the new features with named errors until a writer emits them.

- **IR:**
  - `ContentCollision`: mesh, box, sphere and capsule. The glTF importer reads nodes tagged `liberty_collision` or
    named `*_col`. Primitives are fitted to the object's local bounds and world transform in Blender's frame; a
    capsule runs along local Z.
  - An object that cannot be fitted (sheared, flat, zero scale, or a capsule shorter than its diameter) carries a
    problem, never a guessed shape.
  - Manifest: `type: "object"` (static world object) and `lodDistancesMeters`.
- **Capabilities:**
  - `CompilerCapabilities` states what the writers emit: materials per LOD, compiled LODs, shaders, asset types,
    collision shapes and LOD distances.
  - The validator refuses what v1 cannot write: LCC016, LCC019, LCC032, LCC033. It reports what v1 ignores: LCC025,
    LCC037.
  - `LibertyContent capabilities` prints the capabilities as JSON.
- **Validator:** LCC026–LCC037, [codes](../content/README.md#validation-codes).
- **Report:** `report.json` gains `type`, `capabilities` and `structure`. `structure` holds the LOD geometries per
  material, the LOD distances and the collision shapes with their fitted sizes.
- **New commands:** `validate --report` writes the report without the game. `fixtures <dir>` checks fixtures against
  their `expect.json`.
- **Fix:** glTF node `matrix` transforms were read from a `Matrix4` key, a rename accident, so they were ignored and the
  node sat at the origin. Blender writes TRS, so no shipped asset was affected.
- **Blender add-on 0.3.0:**
  - collision and surface tags (wireframe display), a Materials subpanel with the shader per material, type World
    object, LOD distances;
  - checks LBX023–LBX032.
  - Capability limits are now warnings (LBX009, LBX019, LBX028, LBX029), so an asset is exported as authored and
    LibertyContent's refusal is the one authority.
  - Export writes the validation report.
- **Fixtures:** `tests/content/fixtures` (made in Blender by `tools/blender/examples/make_fixtures.py`):
  `lf_fx_multimat`, `lf_fx_multigeo`, `lf_fx_lods` (LOD 0–3 with visibly different complexity and colour),
  `lf_fx_collision`, `lf_fx_world`. Each `expect.json` is worked out by hand from the scene.

No gameplay code, native code, memory access or config change. Vanilla is untouched.

## Offline evidence (2026-09-26, cloud session; no game)

`tools/cloud/test-all.sh`: every step PASS.

| Step | Before | After |
|---|---|---|
| Content compiler self-test | RAN-PASS 179 | RAN-PASS 248 |
| Content authoring fixtures (new step) | — | RAN-PASS 5/5 |
| Blender add-on tests (`--no-game`) | RAN-PASS 19, NOT RUN 9 | RAN-PASS 78, NOT RUN 9 (builds need the game) |
| C# build, native core, verifier, PowerShell tests, check queue | RAN-PASS | RAN-PASS (unchanged) |

- **Mutation checks:**
  - Reverting the `matrix` fix fails the self-test, and so does fitting the capsule radius from the wrong axis.
  - Changing one expected half extent or one expected error code in a fixture fails `fixtures` with the exact path.
- **Shipped assets:** the four assets under `content/props` still validate, with the same verdicts. Their reports only
  gain the new sections.
- **NEEDS LOCAL VERIFY:**
  - the fixtures and self-test on Windows .NET (`LOOP-content-selftest`);
  - the add-on suite on the owner's Blender with the game, where `lf_fx_lods` builds `ok` (`LOOP-blender-tests`).

## Open questions (for the writer sessions, not this task)

1. How the game stores collision, and which authored shapes map onto which bounds class
   ([Collision.md](../research/Collision.md), session 5).
2. What drawable +0x50 holds for used and unused LOD slots, and so how `lodDistancesMeters` is written
   (`PROBE-drawables`, session 4).
3. Surface names: what the game's collision materials are (session 5).

## Human test steps

Both steps run from the repository root on `develop`, with GTA IV closed.

1. Run `./tools/verify-local.ps1 -Only LOOP-content-selftest,LOOP-blender-tests`. Blender must be configured once, with
   `-Blender <blender.exe>`; without it `LOOP-blender-tests` is NOT-RUN.
2. Expect both checks PASS in the run's `summary.md`:
   - `LOOP-content-selftest`: its log ends with `selftest: ok passed=248 failed=0` and `fixtures: ok passed=5 failed=0`.
   - `LOOP-blender-tests`: the log shows `RESULT passed=87 failed=0` and `Blender add-on tests passed`.
3. Optional, about 5 minutes, only if you want to look at the UI. Install `liberty_exporter-0.3.0.zip`
   ([BLENDER.md](../content/BLENDER.md#install)), then open the Liberty sidebar in a new scene:
   - Select a cube and choose **Collision > Box**. The cube turns wireframe and the Object panel reads
     "collision box". The Materials panel lists the cube's material with `gta_default`.
   - Press **Check**: LBX001 (collision alone is not an asset).
   - Add a second cube, press **Export**: a warning LBX028, the asset folder is written, and the status is `invalid`
     with `LCC032`.
   - Report anything confusing in the panel.
