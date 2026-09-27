# Liberty Content Compiler (LCC)

Blender stays the modelling and animation tool. Liberty is the GTA IV-specific compiler and validator:

```
Blender -> glTF (+ Liberty metadata) -> LCC: import -> validate -> compile -> read back -> preview/report
        -> package (IMG + IDE) -> install -> autopilot: spawn, photograph, check the log
```

Version 1 status (2026-09-25): **working end to end in game**.
- `content/props/lf_test_crate` (glTF written by `lcc sample`) compiled to WDR/WTD in `LibertyContent.img`.
- The autopilot's `asset-review` scenario spawned it and photographed it, textured, upright and lit.
- `content/props/lf_blender_barrel` is made in Blender and exported by the Liberty Exporter add-on
  ([BLENDER.md](BLENDER.md)).

Version 2 status (2026-09-26): **native texture dictionaries, NEEDS-PLAYTEST** ([T-028](../tasks/T-028-lcc-native-textures.md)).
- `"textureMode": "native"` writes the `.wtd` from scratch: source size, full mip chain, DXT1 or DXT5 with alpha.
- Verified offline only (`selftest`, read-back). `lf_native_crate` and `lf_alpha_panel` are the in-game test assets.

Authoring side (2026-09-26, [T-030](../tasks/T-030-lcc-authoring-side.md)): **materials, LODs, collision and world objects
are authored, imported and validated; the writers do not emit them yet.**
- Blender tags collision shapes, shaders, LOD distances and world objects. The IR carries them, and `report.json`
  shows them under `structure`.
- `CompilerCapabilities` states what the writers emit. The validator refuses the rest with a named error, so a
  writer that gains a feature turns it on in one place.
- Five Blender-made fixtures in `tests/content/fixtures` pin all of this offline (`LibertyContent fixtures`).

Structure writer (2026-09-26, [T-031](../tasks/T-031-structure-writer.md)): **several geometries, shaders and LODs,
NEEDS-PLAYTEST, chosen automatically for multi-material assets (session 4b).**
- It fills a game drawable of the same or larger structure (a structure template): every LOD, one geometry per material,
  LOD distances. No structure is synthesised.
- Without `drawableWriter`, the compiler picks it when a LOD has several materials, or several LODs with native
  textures. Otherwise it uses v1, so shipped single-geometry assets keep their proven build.
- `LibertyContent roundtrip` tests the writer against the game's own drawables on the PC.

Collision and world objects (2026-09-26, [T-032](../tasks/T-032-collision.md), [T-033](../tasks/T-033-world-objects.md)):
**NEEDS-PLAYTEST.**
- `collision.borrow` ships a vanilla prop's own bounds resource under the asset's name, as the experiment before a
  collision writer. `probe bounds` measures the collision layout that writer needs.
- `type: object` builds with either writer. The world mod (`mods/Liberty.World`) places objects from
  `config/world/objects.json`. `content/props/lf_lod_post`
  and the `lod-review` scenario test it in game.

## Layout

```
content/<kind>/<name>/
  asset.json          manifest (below)
  <name>.gltf|.glb    source (from Blender's glTF exporter or any glTF 2.0 tool)
  *.bin, *.png        glTF resources
```

`<kind>` is `props` for `type: prop` and `objects` for `type: object`.

`tools/package-phase2.ps1` builds every `content/**/asset.json`. One failed asset fails the package, so a broken asset
never installs.

## asset.json

| Field | Meaning |
|---|---|
| `schemaVersion` | 1 |
| `name` | model name in the game (1–23 characters) |
| `type` | `prop`: a model spawned by scripts. `object`: a static world object, placed by the world mod from `config/world/objects.json` ([T-033](../tasks/T-033-world-objects.md)). Both are built the same way and registered with the IDE `weap` entry the proven spawn path uses (an `objs` entry and map placement files are not written: their formats are not established). LCC034 warns about an object without collision |
| `source` | glTF file, relative to the manifest |
| `template` | `{ archive, model }`: a game drawable whose structure v1 reuses. List candidates with `LibertyContent templates <game> <archive>`. `amb_nailgun` in `pc/models/cdimages/weapons.img` is a 256x256 gta_default prop |
| `textureDictionary` | WTD name (1–23 characters) |
| `drawDistanceMeters` | IDE draw distance |
| `audioMaterial` | optional `amat` entry |
| `textureMode` | optional. `template` (default): the template's dictionary with its texture's pixels replaced (template size, DXT1, opaque). `native`: the dictionary is written from scratch (below) |
| `drawableWriter` | optional. Absent or `auto`: the structure writer when a LOD has several materials, or several LODs with explicit `textureMode: native`, else v1 (`report.json` `writer` says which and why). `template`: v1. `structure` (NEEDS-PLAYTEST): the structure writer, [below](#structure-writer), with native textures unless `textureMode` is explicitly `template` (LCC038) |
| `structureTemplate` | optional, structure writer only: `{ archive, model }`. `model: "auto"` takes the first drawable (by name) that fits; `archive: "*"` searches every IMG. Absent: `*` / `auto` |
| `collision` | optional: `{ "borrow": { archive, model } }` ships that vanilla prop's own bounds resource, unchanged, as `<name>.wbn` (NEEDS-PLAYTEST, [T-032](../tasks/T-032-collision.md)). `model: "auto"` is `PROBE-collision`'s first prop candidate; without one the build ships no collision and says so. Authored collision shapes are not written yet (LCC032, or LCC040 with a borrow) |
| `lodDistancesMeters` | optional. How far each LOD is drawn: one entry per LOD level from LOD 0, ascending, the last at most `drawDistanceMeters` (LCC035). Authoring intent for the LOD writer; v1 checks it but does not write it (LCC037) |

## Commands (`tools/build-content.ps1` → `tools/content/bin/LibertyContent.exe`)

| Command | Does |
|---|---|
| `sample <dir> <name>` | writes the original test crate as glTF (no Blender needed) |
| `validate <asset.json> [--report <report.json>]` | import + validation, exit 2 on errors. `--report` writes `report.json` (status `valid`/`invalid`, the structure, every issue) without the game; the Blender add-on's Export uses it |
| `capabilities [--writer structure]` | what this compiler writes, as JSON: materials per LOD, compiled LODs, shaders, asset types, collision shapes, LOD distances. `--writer structure`: the structure writer's set |
| `probe bounds --game <game> --out <json>` | read only: each collision class's measured layout (word classes, small-integer values, root size bound). PC check `PROBE-bounds-layout` |
| `roundtrip --game <game> [--out <json>] [archive...]` | read only: rebuilds every drawable the structure writer can use with it and compares byte for byte (both buffer orders). No archive: every IMG. PC check `T031-drawable-roundtrip` |
| `fixtures <dir>` | validates every `<dir>/<name>/asset.json` and compares the result with its `expect.json` (below). No game |
| `build <game> <asset.json> <out>` | validate, compile, read back, previews, `report.json` |
| `package <game> <out> <img> <ide> <asset.json...>` | build all, then IMG + IDE |
| `templates <game> <archive>` | usable prop templates with their texture sizes |
| `selftest [--out <dir>]` | offline tests, no game: DXT codecs, name hash, mip chain, WTD writer, native read-back, validator, manifests, glTF import. `--out` also writes test PNGs and WTDs. `package-phase2.ps1` runs it first |
| `wtdcheck [--game <game>] <file.wtd\|folder\|archive.img...>` | read only: checks the game's dictionaries against the writer's rules and rebuilds each one with the writer, then compares everything but placement. Archive paths are relative to `--game` |

**Build outputs** go to `<out>/<name>/`:
- `<name>.wdr` and `<textureDictionary>.wtd`
- `<name>_preview.png`, drawn from the geometry read back out of the compiled `.wdr`
- `<name>_texture.png`, the texture's top level decoded back out of the `.wtd` (with alpha for DXT5)
- `report.json`: status, stats, every validation issue and every read-back problem. Agents read this.
  - `compiled` holds `textureMode`, `textureFormat`, `textureLevels` and, in native mode, `textureQuality`
    (`psnrRgbDb`, `maxErrorRgb`, and for DXT5 `psnrAlphaDb`, `maxErrorAlpha`).
  - `type` and `capabilities` (the `capabilities` JSON) say what was asked and what this compiler writes.
  - Structure builds add `drawableWriter`, `templateUsed`, `lodCount`, `geometries` (LOD, material, texture, counts),
    `textures` (one per material, with PSNR) and `lodDistancesWritten` to `compiled`. A fallback build says
    `"drawableWriter": "template (fallback)"`.
  - `structure` is the asset as the writers see it:
    - `lods`: each level's triangles, vertices and `geometries` (one per material: `material`, `shader`, `textured`,
      `alphaMode`, `meshes`, `triangles`, `vertices`).
    - `lodDistancesMeters`.
    - `collision`: each shape's `name`, `shape`, `surface`. A mesh gives `triangles`. A primitive gives `centre`, `axes`,
      and `halfExtents` (box), `radius` (sphere, capsule) or `length` (capsule). A shape that could not be fitted gives
      `problem`.

## Native texture mode

- **Texture:** the LOD 0 material's base colour texture times its base colour factor. The size is the nearest power of two
  on a log scale, 4–2048 per side (note in the report when resampled). Without a texture: 4×4 of the base colour.
- **Resampling:** an edge-clamped tent filter per channel (bilinear up, area-weighted down, exact at the same size).
  GDI+ is not used here: it can fade alpha at the image edges (seen with Mono's libgdiplus).
- **Format:** DXT5 when the material's alpha mode is not `OPAQUE` and a pixel is translucent, else DXT1.
- **Mips:** the full chain down to a 4-pixel smaller side (256×256 has 7 levels), box-filtered.
- **Name:** the template drawable's texture name, so the drawable needs no change.
- **Prototype:** the dictionary bytes whose meaning is not established are copied from the template's own `.wtd`. A note
  in the report lists any that differ from the builtin prototype. Layout: [ModelFormat.md](../research/ModelFormat.md#texture-dictionary-wtd).
- **Read-back:** format, size, level count and every level's bytes must match what was encoded. The decoded top level
  must score at least 20 dB PSNR against the source, colour and alpha.
- **Open (T-028):** whether the game loads these dictionaries, and how `gta_default` draws DXT5 alpha.

## Structure writer

`"drawableWriter": "structure"` (NEEDS-PLAYTEST, [T-031](../tasks/T-031-structure-writer.md)):
- **Template.** The structure template's first model in LOD slot l receives the asset's LOD l; its geometry j receives
  material group j. The template needs, for every LOD the asset has:
  - a model with at least that many geometries, in layout 0x59;
  - a gta_default shader with one texture per geometry, with no shader showing two materials;
  - no skeleton and no embedded texture dictionary;
  - measurable sphere records.

  The build error, or the `auto` search, names what is missing.
- **Trimming.** Extra geometries, models, LOD slots and shaders are trimmed. Every kept shader names one of the asset's
  textures, `<asset>`, `<asset>_1` and so on, all in one native dictionary.
- **Graphics** go in one page. The drawable's bounds enclose every LOD, and every sphere record gets the model's sphere.
- **LOD distances.** `lodDistancesMeters` goes to drawable +0x50.
- **Fallback.** An `auto` search that finds nothing builds with v1 when v1 can (LOD 0), and says so in the report.
  Otherwise it is an error.
- **Offline dry run.** `selftest --out <dir>` leaves `<dir>/synthetic_game`, and
  `build <dir>/synthetic_game <asset.json> <out>` compiles a structure asset against its four-LOD template.
- **Research labels:** [ModelFormat.md](../research/ModelFormat.md#structure-writer-t-031-2026-09-26-what-it-relies-on-and-how-each-point-is-proven).

## Collision (authoring)

A collision shape is a mesh object that is never drawn. It is tagged with the `liberty_collision` custom property
(`mesh`, `box`, `sphere`, `capsule`; `none` or empty means a render object). An object whose name ends in `_col`,
optionally followed by digits and Blender's `.001`, is mesh collision unless the property says otherwise.
`liberty_surface` optionally names the surface (1–31 letters, digits or `_`).

- **mesh:** the object's triangles, baked like render meshes.
- **Primitives** are fitted to the object's **local** bounding box, placed by its world transform. Apply nothing: rotate
  and scale the object as you like, but no shear, which comes from a rotated child of a non-uniformly scaled parent
  (LCC028).
  - **box:** the local box, scaled and rotated.
  - **sphere:** radius = the largest scaled half extent (LCC029 when the object is not round).
  - **capsule:** along the object's local **Z** axis. Radius = the larger of the X/Y half extents. `length` = the height
    minus the diameter, which is the distance between the hemisphere centres. A capsule shorter than its diameter is
    LCC028.
- LODs do not apply to collision (LCC030), and collision should overlap the model (LCC031).
- What the game's bounds resources can hold is not established ([Collision.md](../research/Collision.md)). These shapes
  are authoring intent, and the collision writer decides how each maps. Compiler v1 writes no collision (LCC032).

## Coordinate rules

- glTF is Y-up. Blender's exporter writes Blender (x, y, z) as glTF (x, z, −y).
- LCC converts glTF → GTA IV (Z-up, metres) with (x, y, z) → (x, −z, y), which returns Blender's axes.
- Model in Blender as you want it in game: Z up, 1 unit = 1 m, props rest on Z=0.
- The **world origin** is the spawn point. Object transforms (and modifiers) are baked. Mirrored transforms are handled
  (winding restored).
- Triangle winding is measured against the template's own geometry and matched.
- UVs are not flipped: glTF and Direct3D share the top-left origin.

## Validation codes

| Code | Severity | Meaning |
|---|---|---|
| LCC001 | error | no triangle meshes |
| LCC002 | error | point/line primitives |
| LCC003 | warning | no normals (smooth normals generated) |
| LCC004 | error | more than 65,535 vertices in one material group of a LOD (one geometry, 16-bit indices) |
| LCC005 | error | textured mesh without UVs |
| LCC006 | error | out-of-range indices |
| LCC007 | warning | degenerate triangles (removed) |
| LCC008 | error | NaN/infinite vertex data |
| LCC009 | warning | non-unit normals (renormalised) |
| LCC010 | warning | mesh without material |
| LCC011 | warning | size outside 0.02–200 m (unit scale / unapplied scale) |
| LCC012 | warning | centre far from the origin (pivot) |
| LCC013 | info | model far above/below Z=0 |
| LCC014 | info | bounds |
| LCC015 | warning | skin present (v1 writes static props) |
| LCC016 | error | more materials (geometries) per LOD than the compiler's capabilities allow (v1: 1) |
| LCC017 | warning | LOD n not lighter than LOD n−1 |
| LCC018 | error | no LOD 0 |
| LCC019 | error | shader the compiler's capabilities do not write (v1: `gta_default`) |
| LCC020 | warning | alpha mode other than opaque (template mode: output is opaque; native mode: DXT5, drawing unverified in game) |
| LCC021 / LCC022 | warning | texture not a power of two / larger than 2048 (resampled) |
| LCC023 | error | texture could not be decoded |
| LCC024 | error | LOD outside 0–3 (a drawable has four LOD slots) |
| LCC025 | info | LOD validated but not compiled by this compiler version (v1 compiles LOD 0) |
| LCC026 | info | per LOD: its geometries (one per material) with triangle and vertex counts |
| LCC027 | error | collision metadata: unknown shape, or a surface name that is not 1–31 letters, digits or `_` |
| LCC028 | error | collision shape without size or not fittable: mesh without triangles, flat primitive, zero scale, sheared transform, capsule shorter than its diameter |
| LCC029 | warning | sphere or capsule fitted to an object that is not round (radius = largest extent) |
| LCC030 | warning | LOD tag on a collision object (ignored) |
| LCC031 | warning | collision shape outside the model's bounds |
| LCC032 | error | collision shapes the compiler's capabilities do not write (v1: none) |
| LCC033 | error | asset type the compiler does not build (both writers build `prop` and `object`) |
| LCC034 | warning | world object (`type: object`) without collision |
| LCC035 | error | `lodDistancesMeters`: not one entry per LOD, not ascending, or the last beyond `drawDistanceMeters` |
| LCC036 | warning | LOD levels with a gap (LOD 2 without LOD 1) |
| LCC037 | info | `lodDistancesMeters` checked but not written by this compiler version |
| LCC038 | error | the structure writer with `textureMode` explicitly `template` |
| LCC039 | info | collision is borrowed at build (from `collision.borrow`) |
| LCC040 | warning | authored collision shapes are not written; the borrowed collision ships instead |

**Capabilities.** Errors LCC016, LCC019, LCC032 and LCC033 are not authoring mistakes. They are what this compiler
version cannot write yet, and they name the version. The Blender add-on reports the same limits as warnings and exports
anyway, so an asset authored for a later writer is kept as authored.

## Liberty metadata (glTF extras)

- `liberty_shader` on a material: default `gta_default`.
- `liberty_lod` on a node, or a node name ending in `_lod<N>`.
- `liberty_collision` (shape) and `liberty_surface` on a node, or a node name ending in `_col` (above).
- Scene extras `liberty_*` are copied into the report.

The Blender add-on (`tools/blender/liberty_exporter`, guide: [BLENDER.md](BLENDER.md)) sets these. It also writes
`liberty_exporter`/`liberty_blender`/`liberty_source`, which `report.json` lists under `metadata`.

## Authoring fixtures

`tests/content/fixtures/<name>/` are made in Blender by the add-on (`tools/blender/examples/make_fixtures.py`, from the
builders in `tools/blender/tests/fixtures.py`). Each has an `expect.json` worked out by hand from the scene:
`status`, the exact set of `errors`, `codes` that must appear, and a subset of `report.json` (numbers within 0.001 m).

| Fixture | Content | v1 result |
|---|---|---|
| `lf_fx_multimat` | one object, two material slots (textured body, red lid) | invalid: LCC016 |
| `lf_fx_multigeo` | a table from five objects, two materials (top 1 mesh, legs 4 meshes) | invalid: LCC016 |
| `lf_fx_lods` | a post in LOD 0–3 (48/24/12/6 sides; green/yellow/orange/red), LOD distances 25/50/100/200 m | valid: LOD 0 built, LCC025, LCC037 |
| `lf_fx_collision` | a crate with a `_col` hull mesh, a turned box, a sphere and a capsule, with surfaces | invalid: LCC032 |
| `lf_fx_world` | `type: object`: a wall of five meshes (one geometry), LOD 1, a box collision, LOD distances | invalid: LCC032, LCC033 |

`LibertyContent fixtures tests/content/fixtures` checks them (`test-all.sh`, and on the PC the `content-selftest` step).
The Blender tests rebuild the same scenes and check the same expectations. Fixtures stay outside `content/`: a refused
asset there would fail `package-phase2.ps1`. When a writer gains a feature, its fixture's expectations change with it.

## Roadmap

1. **v1 (done):** static props, one material, LOD 0, DXT1, built by template patching (the path proven by the slings), IMG/IDE packaging, read-back, previews, autopilot scenario.
2. **Structure writer:** from-scratch texture dictionaries (any size, DXT5 alpha: written, T-028 NEEDS-PLAYTEST); validator IR for material groups and LODs with compiler capabilities (done); authoring of materials, LODs, LOD distances, collision and world objects with fixtures (done, T-030); several geometries, shaders and LODs by the structure writer over game templates (opt-in, T-031 NEEDS-PLAYTEST; default once its checks pass); structures of any size once their layouts are established; then solving multi-page resources for large assets.
3. **Collision:** WBN/phBound import or generation, plus world objects (IDE `objs`) with collision. The authored shapes and the `object` type are ready for it (T-030).
4. **Skinned meshes (WDD), then fragments (WFT) and vehicles.**
5. **Animations (WAD), then audio.**
6. **Textured preview renders** (Blender in background mode on the read-back glTF) and automatic screenshot comparison in the autopilot.
