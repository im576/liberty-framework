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

## Layout

```
content/<kind>/<name>/
  asset.json          manifest (below)
  <name>.gltf|.glb    source (from Blender's glTF exporter or any glTF 2.0 tool)
  *.bin, *.png        glTF resources
```

`tools/package-phase2.ps1` builds every `content/**/asset.json`. One failed asset fails the package, so a broken asset
never installs.

## asset.json

| Field | Meaning |
|---|---|
| `schemaVersion` | 1 |
| `name` | model name in the game (1–23 characters) |
| `type` | `prop` (v1) |
| `source` | glTF file, relative to the manifest |
| `template` | `{ archive, model }`: a game drawable whose structure v1 reuses. List candidates with `LibertyContent templates <game> <archive>`. `amb_nailgun` in `pc/models/cdimages/weapons.img` is a 256x256 gta_default prop |
| `textureDictionary` | WTD name (1–23 characters) |
| `drawDistanceMeters` | IDE draw distance |
| `audioMaterial` | optional `amat` entry |

## Commands (`tools/build-content.ps1` → `tools/content/bin/LibertyContent.exe`)

| Command | Does |
|---|---|
| `sample <dir> <name>` | writes the original test crate as glTF (no Blender needed) |
| `validate <asset.json>` | import + validation, exit 2 on errors |
| `build <game> <asset.json> <out>` | validate, compile, read back, previews, `report.json` |
| `package <game> <out> <img> <ide> <asset.json...>` | build all, then IMG + IDE |
| `templates <game> <archive>` | usable prop templates with their texture sizes |

**Build outputs** go to `<out>/<name>/`:
- `<name>.wdr` and `<textureDictionary>.wtd`
- `<name>_preview.png`, drawn from the geometry read back out of the compiled `.wdr`
- `<name>_texture.png`, the DXT1 texture decoded back
- `report.json`: status, stats, every validation issue and every read-back problem. Agents read this.

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
| LCC004 | error | more than 65,535 vertices in one geometry (16-bit indices) |
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
| LCC016 | error | more materials per LOD than the writer supports (v1: 1) |
| LCC017 | warning | LOD n not lighter than LOD n−1 |
| LCC018 | error | no LOD 0 |
| LCC019 | error | unsupported shader (v1: `gta_default`) |
| LCC020 | warning | alpha mode other than opaque |
| LCC021 / LCC022 | warning | texture not a power of two / larger than 2048 (resampled) |
| LCC023 | error | texture could not be decoded |

## Liberty metadata (glTF extras)

- `liberty_shader` on a material: default `gta_default`.
- `liberty_lod` on a node, or a node name ending in `_lod<N>`.
- Scene extras `liberty_*` are copied into the report.

The Blender add-on (`tools/blender/liberty_exporter`, guide: [BLENDER.md](BLENDER.md)) sets these. It also writes
`liberty_exporter`/`liberty_blender`/`liberty_source`, which `report.json` lists under `metadata`.

## Roadmap

1. **v1 (done):** static props, one material, LOD 0, DXT1, built by template patching (the path proven by the slings), IMG/IDE packaging, read-back, previews, autopilot scenario.
2. **Structure writer:** multiple geometries/materials and LOD models appended to the resource; then from-scratch texture dictionaries (any size, DXT5 alpha); then solving multi-page resources for large assets.
3. **Collision:** WBN/phBound import or generation, plus world objects (IDE `objs`) with collision.
4. **Skinned meshes (WDD), then fragments (WFT) and vehicles.**
5. **Animations (WAD), then audio.**
6. **Textured preview renders** (Blender in background mode on the read-back glTF) and automatic screenshot comparison in the autopilot.
