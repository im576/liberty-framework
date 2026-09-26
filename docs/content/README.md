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
| `textureMode` | optional. `template` (default): the template's dictionary with its texture's pixels replaced (template size, DXT1, opaque). `native`: the dictionary is written from scratch (below) |

## Commands (`tools/build-content.ps1` → `tools/content/bin/LibertyContent.exe`)

| Command | Does |
|---|---|
| `sample <dir> <name>` | writes the original test crate as glTF (no Blender needed) |
| `validate <asset.json>` | import + validation, exit 2 on errors |
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
  `compiled` holds `textureMode`, `textureFormat`, `textureLevels` and, in native mode, `textureQuality`
  (`psnrRgbDb`, `maxErrorRgb`, and for DXT5 `psnrAlphaDb`, `maxErrorAlpha`).

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
| LCC016 | error | more materials per LOD than the compiler's capabilities allow (v1: 1) |
| LCC017 | warning | LOD n not lighter than LOD n−1 |
| LCC018 | error | no LOD 0 |
| LCC019 | error | unsupported shader (v1: `gta_default`) |
| LCC020 | warning | alpha mode other than opaque (template mode: output is opaque; native mode: DXT5, drawing unverified in game) |
| LCC021 / LCC022 | warning | texture not a power of two / larger than 2048 (resampled) |
| LCC023 | error | texture could not be decoded |
| LCC024 | error | LOD outside 0–3 (a drawable has four LOD slots) |
| LCC025 | info | LOD validated but not compiled by this compiler version (v1 compiles LOD 0) |

## Liberty metadata (glTF extras)

- `liberty_shader` on a material: default `gta_default`.
- `liberty_lod` on a node, or a node name ending in `_lod<N>`.
- Scene extras `liberty_*` are copied into the report.

The Blender add-on (`tools/blender/liberty_exporter`, guide: [BLENDER.md](BLENDER.md)) sets these. It also writes
`liberty_exporter`/`liberty_blender`/`liberty_source`, which `report.json` lists under `metadata`.

## Roadmap

1. **v1 (done):** static props, one material, LOD 0, DXT1, built by template patching (the path proven by the slings), IMG/IDE packaging, read-back, previews, autopilot scenario.
2. **Structure writer:** from-scratch texture dictionaries (any size, DXT5 alpha: written, T-028 NEEDS-PLAYTEST); validator IR for material groups and LODs with compiler capabilities (done); multiple geometries/materials and LOD models appended to the resource (next); then solving multi-page resources for large assets.
3. **Collision:** WBN/phBound import or generation, plus world objects (IDE `objs`) with collision.
4. **Skinned meshes (WDD), then fragments (WFT) and vehicles.**
5. **Animations (WAD), then audio.**
6. **Textured preview renders** (Blender in background mode on the read-back glTF) and automatic screenshot comparison in the autopilot.
