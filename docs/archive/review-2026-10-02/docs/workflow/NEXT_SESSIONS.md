> Historical snapshot from before the October 2 repo review. Not current instructions.

# Next sessions

Start a session with one line: **"Do the next session in `docs/workflow/NEXT_SESSIONS.md`."** The agent takes the first
session below whose status is `NEXT` (and whose prerequisites hold), sets it to `IN PROGRESS` in its branch, and at the
end sets it `DONE` (work merged into `develop`) or `BLOCKED` (why, in one line). Other one-line prompts:

- **"Process the newest verification results."**: the review procedure in [CLOUD_LOCAL_LOOP.md](../../../../workflow/CLOUD_LOCAL_LOOP.md).
  Takes priority over any session below whenever a new run is on `verification-results`.
- **"Write the mod pack design."**: session D below, any time.

Every session: read `AGENTS.md`, `docs/PROJECT_STATE.md`, `docs/workflow/CLOUD_LOCAL_LOOP.md` and this section; work on
the session's own `claude/*` branch; PR into `develop`; `tools/cloud/test-all.sh` with no FAIL; add checks for
everything that needs the game in the same PR; commit in logical stages; report with the evidence labels
RAN-PASS / RAN-FAIL / NOT RUN (reason) / NEEDS LOCAL VERIFY. Stop and report instead of guessing engine facts.

| # | Session | Status | Needs first |
|---|---|---|---|
| 1 | Cloud loop, integration of raycast (T-027) and native textures (T-028) | DONE (T-029) | none |
| 2 | Engine audit and hardening | DONE (PR into `develop`; [report](../../../../reports/2026-09-26-engine-audit.md)) | none |
| 3 | Blender and compiler authoring side: materials, LODs, collision and world metadata | DONE ([T-030](../../../../tasks/T-030-lcc-authoring-side.md), PR into `develop`) | session 2 merged |
| 4 | Multi-geometry and LOD drawable writer | DONE, capability off ([T-031](../../../../tasks/T-031-structure-writer.md); opt-in `drawableWriter: structure`; round trip, LOD scenario and probe queued) | session 3 merged; `PROBE-drawables` results strongly preferred |
| 4b | Structure writer as the default | DONE (automatic for multi-material assets; single-geometry assets stay v1; [T-031](../../../../tasks/T-031-structure-writer.md#session-4b-2026-09-26-the-automatic-writer)) | owner's request, before the PC run |
| 5 | Collision: research, then writer | DONE up to the writer ([T-032](../../../../tasks/T-032-collision.md): layout probe, borrowed-collision experiment); **5b, the collision writer, waits for `PROBE-collision` and `PROBE-bounds-layout`** | `PROBE-collision` results (research may start before) |
| 6 | Static world objects (IDE, placement, packaging) | DONE ([T-033](../../../../tasks/T-033-world-objects.md): world mod, `config/world/objects.json`, borrowed collision) | session 5 |
| 7+ | SDK features for the mod pack | queued | session D |
| D | Mod pack design document | DONE ([STAGE1.md](../../../../design/STAGE1.md)) | none |

## Session 2: engine audit and hardening

Audit the whole repository as if other developers will build serious mods on it: `sdk/`, `src/`, `native/`, `tools/`,
`mods/`, `content/`, `config/`, install/package scripts, autopilot, validators, the Blender add-on, and every
architecture, research and task document.

- **Code:** crashes, null handling, unsafe ownership, leaks, hooks not restored, stale pointers, ABI mismatches between
  `native/LibertyCore/include/liberty_core.h` and `CoreAbi.cs`, scheduler isolation, a module failure taking down the
  engine, draw/tick races, unsafe native calls, memory assumptions, capability enforcement, hot-reload behaviour, event
  duplication or loss, unbounded scans, silent catches (AGENTS.md rules 8-9), wrong exception attribution, config
  compatibility, SDK inconsistencies. Fix what is real, smallest change first, each with an offline test where the
  logic allows (the verifier's `Logic` folders, the native unit tests, `tools/tests`). Anything that changes in-game
  behaviour gets a check in the queue.
- **`cloud/audit-cleanup`** (WIP, uncompiled, based on `0e4d357`): read its report and patches as evidence only;
  re-check each finding against the current code; never merge it wholesale.
- **Research claims:** label every reverse-engineered claim VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN;
  check that addresses are found by pattern (ADR-0004, `docs/game-api/MEMORY.md`), and that code never relies on more
  than the research established. Unproven assumptions become fail-safe code or queued checks.
- **Raycast (T-027):** coherent public API, correct filtering per kind, safe ignore semantics, line of sight built on the
  real query, failures as `Unavailable`/`Inconclusive`, useful counters, research commands clearly diagnostic. Queue a
  scenario for `RayMask.Objects` against a vanilla prop with collision, choosing the model from evidence (a probe or an
  existing doc), not memory.
- **Docs:** README, `ENGINE.md` against the real APIs, SDK examples that compile against the current SDK, ROADMAP,
  PROJECT_STATE (short), unique task numbers, stale paths and class names.
- **Leads noticed in session 1** (verify each; none is confirmed as a bug):
  - `tools/verify.ps1` compiles with the .NET Framework's own C# 5 compiler on Windows while everything else uses the
    pinned Roslyn (C# 7.3): two toolchains for one repository.
  - `Save-Screenshot` looks for Steam only under `C:\Program Files (x86)\Steam\userdata`; elsewhere every screenshot
    step fails (a FAIL, not a false pass, but a whole run lost).
  - `Get-SessionLog` re-reads the whole log file on every 500 ms poll; with a long log that is slow, and with the game
    already running when a new PowerShell starts it reads every earlier session too.
  - The `codex/phase2-combat` and `codex/phase2-systems` branches share no history with `main`; `main` has about
    12,600 lines they lack, and about 900 lines differ the other way. Decide file by file whether anything there is
    unique work; do not merge the branches.
  - T-015 is `NEEDS-PLAYTEST` in the task table while T-024's card calls shoulder swap BLOCKED.
- **Deliverable:** an audit report in `docs/reports/`, fixes in logical commits, new checks queued, PR into `develop`.

## Session 3: authoring side for materials, LODs, collision and world objects

Everything between Blender and the writers that needs no game files: Blender add-on properties and panels (materials,
LOD0-LOD3, collision designation: dedicated collision meshes, simple primitives, collision metadata; asset metadata for
world objects), glTF extras, the IR (`ContentAsset`, `ContentLod`, `ContentMaterialGroup`), the validator (clear codes and
messages), `CompilerCapabilities` (what the current writer can emit; the validator refuses the rest with a clear error),
and fixtures: multi-material, multi-geometry, LOD0-LOD3 with visibly different complexity, a collision mesh, a static
world object. Blender tests for each (they run in the cloud with `--no-game`). No new writer output yet.

## Session 4: multi-geometry and LOD drawable writer

Starting point from session 3 (T-030):
- The IR's `ContentLod.Groups` are the geometries: one per material, in material-index order, as `report.json` `structure.lods`
  lists them.
- Turning the feature on is `CompilerCapabilities` (`maxMaterialsPerLod`, `compiledLodLevels`, `lodDistances`, and new
  shaders). Keep the add-on's `checks.CAPABILITIES` equal: its tests compare the two.
- `tests/content/fixtures/lf_fx_multimat`, `lf_fx_multigeo` and `lf_fx_lods` are the inputs. `lf_fx_lods` colours each
  LOD (green, yellow, orange, red) for the LOD-switch scenario. Update their `expect.json` (and `tests/fixtures.py`) as
  the refusals turn into builds.

Extend `DrawableBuilder` to several geometries, several shaders and LOD slots 1-3, from `docs/research/ModelFormat.md`
and the `PROBE-drawables` report. Prove it offline by extending the round trip to the game's own multi-geometry and
multi-LOD drawables (queued as a `pc-offline` check: rebuild them byte-identical, the method the single-geometry
self-test already uses). Read-back of every compiled asset; representative fixtures; an autopilot scenario that
shows the LODs switching with distance. If the probe results are not in yet, write the builder against the documented
layout, keep the capability off in `CompilerCapabilities`, and queue the round trip; turn the capability on only after
the local run passes.

## Session 4b: structure writer as the default

Only after a run where `T031-drawable-roundtrip` passes and `T031-lod-review` shows the LODs drawn:
- make `CompilerCapabilities.Current` the structure set, and keep the add-on's `checks.CAPABILITIES` equal;
- update the fixtures' `expect.json` and `tests/fixtures.py` (the LCC016 refusals become builds);
- pick default templates from `PROBE-drawables` `structureTemplates`.

If the round trip fails: read its report, fix the rule it names (buffer order, records), and keep the check queued.

## Session 5b: collision writer (after the verification run)

Read `PROBE-collision` and `PROBE-bounds-layout`, decode the class a static prop uses (labels in Collision.md), write a
probe that round-trips the game's files, then the writer, turning shapes on in `CompilerCapabilities.CollisionShapes`.
`lf_fx_collision` and `lf_world_wall`'s authored shapes are its inputs. `T032-collision-borrow` says whether same-named
bounds are paired at all.

## Session 5: collision

Follow [Collision.md](../../../../research/Collision.md): read the `PROBE-collision` inventory, pick the resource a static prop
uses, write a decoding probe for that class, round-trip the game's own files, then write the collision writer and a
scenario proving player collision, vehicle collision, and a raycast hit on the authored object. The authored shapes
(`ContentCollision`: mesh, box, sphere, capsule, surface) and `tests/content/fixtures/lf_fx_collision` are ready (T-030);
the writer lists what it emits in `CompilerCapabilities.CollisionShapes`.

## Session 6: static world objects

Blender -> export -> compile (geometry, materials, textures, LODs, collision) -> package -> IDE / placement -> spawn or
place -> scenario. Not a map editor; custom environment props that work. `type: "object"` assets
(`content/objects/<name>`) and `tests/content/fixtures/lf_fx_world` are authored and validated already (T-030); v1
refuses them with LCC033 until this session adds `object` to `CompilerCapabilities.AssetTypes`.

## Session D: mod pack design

A document in `docs/design/` describing the mods and mod pack the owner wants, written with the owner: what each mod
does, which engine features it needs, which exist, which are missing. Its "missing" list sets the order of session 7+.
Ask the owner questions; do not invent their design.

## Deferred on purpose

Skinned characters (WDD), fragments and destruction (WFT), custom vehicles, animation compilation (WAD) and audio. Each
is a large compiler expansion; they stay on the roadmap and never block moving on to mods.
