# Next sessions

Start a session with one line: **"Do the next session in `docs/workflow/NEXT_SESSIONS.md`."** The agent takes the first
session below whose status is `NEXT` (and whose prerequisites hold), sets it to `IN PROGRESS` in its branch, and at the
end sets it `DONE` (work merged into `develop`) or `BLOCKED` (why, in one line). Other one-line prompts:

- **"Process the newest verification results."**: the review procedure in [CLOUD_LOCAL_LOOP.md](CLOUD_LOCAL_LOOP.md).
  Takes priority over any session below whenever a new run is on `verification-results`.
- **"Write the mod pack design."**: session D below, any time.

Every session: read `AGENTS.md`, `docs/PROJECT_STATE.md`, `docs/workflow/CLOUD_LOCAL_LOOP.md` and this section; work on
the session's own `claude/*` branch; PR into `develop`; `tools/cloud/test-all.sh` with no FAIL; add checks for
everything that needs the game in the same PR; commit in logical stages; report with the evidence labels
RAN-PASS / RAN-FAIL / NOT RUN (reason) / NEEDS LOCAL VERIFY. Stop and report instead of guessing engine facts.

| # | Session | Status | Needs first |
|---|---|---|---|
| 1 | Cloud loop, integration of raycast (T-027) and native textures (T-028) | DONE (T-029) | none |
| 2 | Engine audit and hardening | DONE (PR into `develop`; [report](../reports/2026-09-26-engine-audit.md)) | none |
| 3 | Blender and compiler authoring side: materials, LODs, collision and world metadata | NEXT | session 2 merged |
| 4 | Multi-geometry and LOD drawable writer | queued | session 3; `PROBE-drawables` results strongly preferred |
| 5 | Collision: research, then writer | queued | `PROBE-collision` results (research may start before) |
| 6 | Static world objects (IDE, placement, packaging) | queued | session 5 |
| 7+ | SDK features for the mod pack | queued | session D |
| D | Mod pack design document | any time | none |

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

Extend `DrawableBuilder` to several geometries, several shaders and LOD slots 1-3, from `docs/research/ModelFormat.md`
and the `PROBE-drawables` report. Prove it offline by extending the round trip to the game's own multi-geometry and
multi-LOD drawables (queued as a `pc-offline` check: rebuild them byte-identical, the method the single-geometry
self-test already uses). Read-back of every compiled asset; representative fixtures; an autopilot scenario that
shows the LODs switching with distance. If the probe results are not in yet, write the builder against the documented
layout, keep the capability off in `CompilerCapabilities`, and queue the round trip; turn the capability on only after
the local run passes.

## Session 5: collision

Follow [Collision.md](../research/Collision.md): read the `PROBE-collision` inventory, pick the resource a static prop
uses, write a decoding probe for that class, round-trip the game's own files, then write the collision writer and a
scenario proving player collision, vehicle collision, and a raycast hit on the authored object.

## Session 6: static world objects

Blender -> export -> compile (geometry, materials, textures, LODs, collision) -> package -> IDE / placement -> spawn or
place -> scenario. Not a map editor; custom environment props that work.

## Session D: mod pack design

A document in `docs/design/` describing the mods and mod pack the owner wants, written with the owner: what each mod
does, which engine features it needs, which exist, which are missing. Its "missing" list sets the order of session 7+.
Ask the owner questions; do not invent their design.

## Deferred on purpose

Skinned characters (WDD), fragments and destruction (WFT), custom vehicles, animation compilation (WAD) and audio. Each
is a large compiler expansion; they stay on the roadmap and never block moving on to mods.
