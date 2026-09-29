# T-031 — Content compiler structure writer: several geometries, shaders and LODs

Status: **NEEDS-PLAYTEST** (queued: `T031-drawable-roundtrip`, `T031-lod-post-report`, `T031-lod-review`, and the
extended `PROBE-drawables`)

**Windows result (2026-09-28):** `T031-drawable-roundtrip` failed: 5,254 of 12,688 eligible WDRs rebuilt identically;
7,434 differed. `PROBE-drawables` found no parsed WDR with more than LOD slot 0. `lf_lod_post` consequently used the
template fallback and kept LOD 0 only, despite the authored four levels. The screenshot sequence shows the green
one-band post at 6, 18 and 38 m; it does not verify LOD switching. Details and evidence are in
[`2026-09-28-windows-verification.md`](../reports/2026-09-28-windows-verification.md). The proposed default promotion
and four-LOD claim remain on hold. Research the game's actual LOD resource path and diagnose representative system-byte
roundtrip differences before reworking the writer.

Depends on: T-030 (authoring side), T-028 (native texture dictionaries). Session 4 of
[NEXT_SESSIONS](../workflow/NEXT_SESSIONS.md).

## Why this shape

The research documents the drawable's field offsets but not the full sizes of its structures: geometry, model, shader,
the sphere-record array ([ModelFormat.md](../research/ModelFormat.md)). Writing new structures would mean guessing
layouts (AGENTS.md rule 4). The `PROBE-drawables` results were not in yet.

The writer therefore **never synthesises a structure**. It fills a game drawable whose structure is the same or larger
(a *structure template*), and trims what the asset does not use through fields whose meaning is documented. Each fact it
depends on that is not established is either measured per file or tested against the game's own files by a byte-for-byte
round trip.

The default stays v1. `CompilerCapabilities.Current` is unchanged, and an asset opts in with
`"drawableWriter": "structure"`, the way T-028 made `"textureMode": "native"` an opt-in. Making the structure writer the
default is a later decision, after the checks below pass.

## What was built

- **`DrawableStructureBuilder`** (`tools/models`). Patches every kept geometry: buffers, counts and pointers.
  - **Trimming.** Trailing geometries and models are trimmed through the collection counts. An unused LOD slot has its
    pointer cleared. The shader collection is shortened, and every kept shader is renamed to one of the asset's textures.
  - **Graphics.** One graphics page for generated drawables, the finding that fixed generated models in game.
  - **Bounds.** The drawable's box, centre and radius enclose every kept geometry, which is conservative for culling.
  - **Buffer order** has two candidates, `Interleaved` (v0 i0 v1 i1 ...) and `VerticesFirst`. The round trip reports
    which one the game's files use.
  - **Sphere records at model +0x0C** are *measured* per template: n+1, n or 1 records, each accepted only if it is a
    sphere enclosing its geometry. Bytes past the real array (pointers) do not pass. Generated drawables write the
    whole model's sphere into every record, which encloses whatever a record stands for.
- **`LibertyContent roundtrip --game <game> [--out <json>] [archive...]`.** Rebuilds every eligible game drawable under
  both orders and compares the system segment byte for byte, bounds excepted (1 mm containment). It also compares every
  geometry's vertex and index bytes. It is the multi-geometry and multi-LOD extension of LibertyModel's single-geometry
  self-test.
- **Compiler** (`StructureCompiler`).
  - LOD l's material groups go onto the template's first model in slot l, geometry j = group j.
  - One native texture per material, named `<asset>`, `<asset>_1`, ...
  - `lodDistancesMeters` is written to drawable +0x50.
  - `structureTemplate` names a drawable, or `"model": "auto"` takes the first that fits (by name). `"archive": "*"`
    searches every IMG. `Match` says why a drawable does not fit.
  - An explicit template that does not fit is an error. An auto search that finds nothing falls back to v1 only when v1
    can build the asset, and the report says `"template (fallback)"`.
- **Capabilities, codes and report.**
  - `CompilerCapabilities.Structure` (`v2-structure`): LODs 0–3, up to 16 materials per LOD (bounded by the template),
    LOD distances.
  - LCC038: the structure writer needs `textureMode: native`.
  - `capabilities --writer structure` prints the set.
  - `report.json` `compiled` gains `drawableWriter`, `templateUsed`, `lodCount`, `geometries`, `textures` and
    `lodDistancesWritten`.
- **Read-back.** Every LOD slot holds exactly its parts (meshes field for field, each shader naming its material's
  texture). Unused slots are empty, the distances are as written, and every texture is checked byte for byte with PSNR.
- **Probe.** `PROBE-drawables` also reports structure-writer eligibility (and why not), the measured sphere records, the
  buffer order of every multi-geometry file, and structure templates by shape (five names per shape).
- **Blender add-on 0.4.0.** Drawable writer (Template / Structure) and the structure template fields; LBX033 (mirrors
  LCC038). `checks.STRUCTURE_CAPABILITIES` is tested against `capabilities --writer structure`.
- **In-game test asset `content/props/lf_lod_post`**, made by `tools/blender/examples/make_lf_lod_post.py`.
  - A 3 m post in four LODs of 48, 24, 12 and 6 sides.
  - One material; each LOD samples its own quadrant of the texture: green, yellow, orange, red with 1–4 dark bands.
  - LOD distances 12, 25, 50 and 100 m, draw distance 150 m, structure template `*`/`auto`.
  - Scenario `lod-review`.
- **Offline dry run.** `selftest --out <dir>` leaves `<dir>/synthetic_game`, a game folder with a four-LOD structure
  template. `build <dir>/synthetic_game content/props/lf_lod_post/asset.json <out>` compiles the asset without the game.

No gameplay code, native code, memory access or config change. Vanilla is untouched: `lf_lod_post` exists only in
`LibertyContent.img` and appears only when a script spawns it.

## Offline evidence (2026-09-26, cloud session; no game)

`tools/cloud/test-all.sh`: every step PASS (summary in the PR).

- **Content self-test:** 248 → 308 checks.
  - Synthetic drawables round-trip in their own buffer order only, and sphere records are measured for all three
    layouts.
  - Trimming and each refusal are covered.
  - The compiler runs on a synthetic game folder: auto search, a template that does not fit, and the fallback refusal.
  - `roundtrip` and the probe run end to end.
- **Mutations:** swapping the buffer order, reversing the record preference, or not clearing an unused LOD slot each
  fails the self-test.
- **Dry run:** `lf_lod_post` builds `ok` against `synthetic_game`, with four LODs read back and the texture at 40.9 dB.
  Each LOD's UVs were checked to sample its own quadrant.
- **Other steps:**
  - Blender add-on tests: 83 passed, 9 NOT RUN (builds need the game).
  - PowerShell tests: 109, including the `drawable-roundtrip` step in simulated runs.
  - The five authoring fixtures and the four shipped assets are unchanged.
- **NEEDS LOCAL VERIFY:** everything about Rockstar's files and the game (below).

## Open questions (answered by the queued checks)

1. **Do real drawables round-trip, and in which buffer order?** `T031-drawable-roundtrip`. A failure lists the file and
   the first differing offsets.
2. **How many sphere records do multi-geometry models carry?** The round-trip report (`boundsRecords`) and
   `PROBE-drawables` (`boundsRecordsMultiGeometry`).
3. **Is there a structure template with four LOD slots and gta_default geometries?** `T031-lod-post-report`: `templateUsed`,
   or `drawableWriter: "template (fallback)"` when there is none. `PROBE-drawables` `structureTemplates` lists what
   exists.
4. **Does the game draw a trimmed, renamed, one-page multi-LOD drawable, and which LOD does it pick at each distance?**
   `T031-lod-review`.
5. **Does drawable +0x50 set the switch distances?** `T031-lod-review`, compared with the distances written.

**After the run:** if the round trip passes and the scenario shows the LODs, the next session proposes making the
structure writer the default (`CompilerCapabilities.Current`) and updates the fixtures' expectations. If only a fallback
happened, it picks a template from the probe's `structureTemplates` and names it in `lf_lod_post`'s asset.json.

## Session 4b (2026-09-26): the automatic writer

The owner asked for the default switch before the PC run. It was made in the form that cannot regress a proven asset.
`drawableWriter` absent now means `auto` (`AssetManifest.ResolveWriter`):
- **Structure writer** when a LOD has several materials (v1 cannot write them at all), or when there are several LODs and
  `textureMode` is explicitly native. It uses native textures unless `textureMode` is explicitly `template` (LCC038).
- **v1 otherwise.** Every shipped single-geometry asset keeps the build proven in game: `lf_test_crate`,
  `lf_blender_barrel`, `lf_native_crate` and `lf_alpha_panel` resolve to `template`. Checked, and tested in the self-test
  group "writer choice".
- An absent `structureTemplate` searches every archive (`*` / `auto`). `report.json` gains
  `writer {drawable, reason, textureMode}`. The fixtures `lf_fx_multimat` and `lf_fx_multigeo` are now valid and
  dry-build against the synthetic game.
- Blender add-on 0.5.0: Drawable writer **Automatic** (default, not written). LBX033 is information.

The verification run still decides: if `T031-drawable-roundtrip` or `T031-lod-review` fails, multi-material assets fail
with it. Single-geometry assets are unaffected.

## Human test steps

One verification run covers it. Close GTA IV first.

1. From the repository root on `develop`, run:
   `./tools/verify-local.ps1 -Only PROBE-drawables,T031-drawable-roundtrip,T031-lod-post-report,T031-lod-review`
   The script adds `LOOP-package-install`, which builds and installs `lf_lod_post` with the rest of the package.
2. **Expected in `summary.md`:**
   - `PROBE-drawables`: NEEDS-REVIEW (a report for the review session).
   - `T031-drawable-roundtrip`: PASS. Its log ends with `roundtrip: ok ... failed=0`, and `orders[...]` names the buffer
     order.
   - `T031-lod-post-report`: PASS (`drawableWriter` `structure`, four LODs).
   - `T031-lod-review`: NEEDS-REVIEW with five screenshots, reviewed below.
3. **Reviewing `T031-lod-review`:**
   - Colours and bands identify the LOD: green with 1 band = LOD 0, yellow with 2 = LOD 1, orange with 3 = LOD 2, red
     with 4 = LOD 3.
   - Note the colour and band count in each shot: `lod_6m`, `lod_18m`, `lod_38m`, `lod_75m`, `lod_130m`.
   - Report garbage geometry, stretched triangles, a missing post, or a white/grey post.
4. If the game crashes when the post spawns, attach `LibertyFramework.log` and the run's `report.md`. Rollback:
   `./tools/rollback-phase2.ps1 -GameDirectory '<GTAIV folder>'`.
