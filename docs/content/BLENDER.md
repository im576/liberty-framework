# Authoring in Blender (Liberty Exporter add-on)

Blender models and textures the asset. The Liberty Exporter add-on (`tools/blender/liberty_exporter`, v0.4.0) turns
the scene into a validated asset folder. LibertyContent (docs/content/README.md) then compiles it for GTA IV.

```
Blender scene -> Liberty panel: Check -> Export (glTF + asset.json, LCC validate) -> Build (WDR/WTD, read-back, previews)
              -> tools/package-phase2.ps1 + install-phase2.ps1 -> autopilot asset-review (spawn, screenshots)
```

Tested with Blender 5.2.2 LTS. The minimum version is 4.2, where extensions start.

## Install

1. Build the compiler once: `tools/build-content.ps1`. That produces `tools/content/bin/LibertyContent.exe`.
2. Build the add-on zip:
   ```
   blender --background --factory-startup --command extension build --source-dir tools/blender/liberty_exporter --output-dir staging
   ```
3. Install `staging/liberty_exporter-0.4.0.zip` in Blender: Edit > Preferences > Get Extensions > ⌄ > Install from Disk.
4. In the add-on's preferences, set:
   - **Content folder:** the repository's `content` folder.
   - **GTA IV folder:** the folder with `GTAIV.exe`. Builds read the template drawable from the game archives.
   - **LibertyContent.exe:** leave it empty to use `<content>/../tools/content/bin/LibertyContent.exe`.
   - **Build output:** leave it empty to use `%TEMP%\liberty_build`.

## Modelling rules

- **Units and axes:** 1 Blender unit = 1 m, Z up, Unit Scale 1. The asset keeps Blender's axes in game. The tests
  prove this with an asymmetric box: its bounds after compiling equal its bounds in Blender.
- **Spawn point:** the world origin (0, 0, 0) is the prop's spawn point, not the object origin. Object transforms and
  modifiers are baked on export. Props usually rest on Z = 0.
- **Materials:** a Principled BSDF with an Image Texture linked straight into Base Color. Each material of a LOD becomes
  one geometry. Compiler v1 writes **one material per LOD**. With more, the add-on warns (LBX009), exports anyway, and
  LibertyContent refuses to build (LCC016). Merge materials or bake an atlas to build now; keep them for the
  multi-geometry writer.
  - Without an image, the base colour fills the texture.
  - **Texture mode Template** (default, proven in game): alpha is ignored, output is DXT1 and opaque, and the texture is
    resampled to the template's texture size, 256×256 for `amb_nailgun`.
  - **Texture mode Native** (NEEDS-PLAYTEST, [T-028](../tasks/T-028-lcc-native-textures.md)): the texture keeps its own
    size (nearest power of two, 4–2048) with a full mip chain. A material with alpha (Principled BSDF Alpha below 1 or
    linked) is written as DXT5, otherwise DXT1. How `gta_default` draws alpha in game is still open.
- **UV map** is required on textured meshes.
- **LODs:**
  - Name objects `<name>_lod1`, `_lod2` or `_lod3` (a Blender `.001` suffix is fine), or use the panel's LOD buttons,
    which set the `liberty_lod` custom property.
  - Children inherit their parent's LOD.
  - Each LOD should have fewer triangles than the one before.
  - Compiler v1 compiles LOD 0 only, and checks and reports the rest.
  - **LOD distances** (panel: LOD distances): how far each LOD is drawn, in metres, ascending, the last at most the
    draw distance. Only the entries for the LODs the asset has are written (`lodDistancesMeters`). v1 checks them and
    does not write them yet.
- **Hidden objects** in the asset collection are still exported, so an artist can hide LOD 1 while working on LOD 0.
- **Static only:** armature-deformed meshes export in their current pose (warning LBX015).
- **Shader:** a `liberty_shader` custom property on the material (Materials panel: pencil button). It defaults to
  `gta_default`, the only shader v1 writes. Other names are exported, with a warning (LBX019), and refused by the
  compiler (LCC019).
- **Collision:** extra mesh objects that are never drawn (Object panel: **Collision** menu, **Surface**).
  - Shapes: **Mesh** (the triangles), or **Box**, **Sphere** or **Capsule**. A primitive is fitted to the object's local
    bounds and transform, and a capsule runs along the object's local Z. Rules:
    [README](README.md#collision-authoring).
  - An object named `<anything>_col` is mesh collision; Collision > None on it writes `none`.
  - Tagged objects draw as wireframe. They take no LOD.
  - Compiler v1 writes no collision: the add-on warns (LBX028) and exports, and LibertyContent refuses to build
    (LCC032).
- **World objects:** Type **World object** (`type: object`, folder `content/objects/<name>`) is a static map object
  with collision. It is authored and checked now; v1 refuses to build it (LCC033).
- **Errors and warnings:** errors (LBX) are authoring mistakes and stop the export. What only a later compiler can write
  is a warning, and the export goes ahead so the asset is kept as authored.

## The Liberty panel (3D View > Sidebar > Liberty)

| Field | asset.json |
|---|---|
| Name (eyedropper: from the active object) | `name`, and the folder `content/props/<name>` |
| Type | `type`: Prop (`prop`) or World object (`object`) |
| Collection | what to export, nested collections included. Empty exports the selection |
| Template / Model | `template` |
| Texture dictionary | `textureDictionary` (empty uses the name) |
| Texture mode | `textureMode`: Template (default, not written) or Native |
| Drawable writer | `drawableWriter`: Template (default, not written) or Structure (every LOD, one geometry per material; needs Native; NEEDS-PLAYTEST, T-031) |
| Structure template / Model | `structureTemplate` (Structure only): `*` / `auto` search every IMG for the first drawable that fits |
| Draw distance (m) | `drawDistanceMeters` |
| Audio material | `audioMaterial` |
| LOD distances | `lodDistancesMeters` (when ticked; one entry per LOD the asset has) |

**Buttons:**
- **Check:** the add-on's own checks (below).
- **Export:** runs Check, stops on errors, writes glTF + `asset.json`, then runs `LibertyContent validate --report`.
  The Report button opens that `report.json` (status, structure, issues).
- **Build:** runs Export, then `LibertyContent build`. It shows the status and opens the preview, the decoded texture,
  `report.json` or the folder.

The same export is under File > Export > Liberty Asset.

**Subpanels:**
- **Object:** the active object's LOD buttons, or its collision shape and surface. The Collision menu tags the selected
  objects.
- **Materials:** each material slot of the active object, with its shader and texture. The pencil sets the active
  material's shader.

The exported glTF carries `liberty_exporter`, `liberty_blender` and `liberty_source` as scene extras. `report.json`
lists them under `metadata`, so every compiled asset records what made it.

## Check codes (LBX)

They mirror the compiler's LCC codes and add what only Blender can see. LibertyContent's codes are still reported after
export.

| Code | Severity | Meaning |
|---|---|---|
| LBX001 | error | nothing to export (no render mesh in the collection/selection; collision alone is not an asset) |
| LBX002 / LBX003 | error | asset / texture dictionary name not 1–23 letters, digits or `_` |
| LBX004 | warning | scene Unit Scale is not 1 |
| LBX006 | warning | largest extent outside 0.02–200 m |
| LBX007 | warning | model centre far from the world origin (spawn point) |
| LBX008 | info | lowest point not at Z = 0 |
| LBX009 | warning | more materials in a LOD than compiler v1 writes (exported; LibertyContent refuses: LCC016) |
| LBX010 | warning | faces without a material |
| LBX011 | warning | material the glTF exporter can't translate (no Principled BSDF, Base Color not a direct Image Texture) |
| LBX012 | error | image file missing, not a still image, or no pixels |
| LBX013 | warning | texture not a power of two, or above 2048 |
| LBX014 | error | textured mesh without a UV map |
| LBX015 | warning | skinned mesh (exported static, current pose) |
| LBX016 | warning | more than 65,535 face corners (may exceed the vertex limit) |
| LBX017 | error | no LOD 0, or LOD outside 0–3 |
| LBX018 | warning | LOD not lighter than the previous one |
| LBX019 | warning | `liberty_shader` compiler v1 does not write (exported; LibertyContent refuses: LCC019) |
| LBX020 | warning | material alpha (Template mode: output is opaque; Native mode: DXT5, drawing unverified in game) |
| LBX021 | info | summary: objects, triangles per LOD, bounds |
| LBX022 | warning | object with no faces (ignored) |
| LBX023 | error | unknown collision shape, or a surface name that is not 1–31 letters, digits or `_` |
| LBX024 | error | collision without faces or vertices, or a primitive that cannot be fitted (sheared, zero scale, capsule shorter than its diameter) |
| LBX025 | warning | sphere or capsule on an object that is not round |
| LBX026 | warning | LOD tag on a collision object (ignored) |
| LBX027 | warning | collision outside the model's bounds |
| LBX028 | warning | collision, which compiler v1 does not write (exported; LibertyContent refuses: LCC032) |
| LBX029 | warning | asset type compiler v1 does not build (exported; LibertyContent refuses: LCC033) |
| LBX030 | error | LOD distance not above 0, not ascending, or beyond the draw distance |
| LBX031 | warning | LOD levels with a gap |
| LBX032 | warning | world object without collision |
| LBX033 | error | Drawable writer Structure with Texture mode Template (LCC038) |

## Scripts and tests

- `tools/blender/run-tests.ps1 -GameDirectory <game> -Blender <blender.exe>`:
  - Blender's extension validation, plus 92 headless tests (0.4.0). Without the game (`--no-game`, the cloud) 83
    run and the 9 that build against the game's archives are NOT-RUN.
  - The tests cover registration, LOD tags, export files and extras, validate and build against the real game
    archives, and axis/metre round trips.
  - They also cover modifiers baked, rejected assets writing nothing, and every error check, plus collection export
    with inherited LODs and metadata.
  - 0.3.0 adds the five authoring fixtures (export, then LibertyContent's status, errors and structure against
    `tests/fixtures.py`), collision and LOD-distance mistakes, the collision/surface/shader operators, and a check that
    `checks.CAPABILITIES` equals `LibertyContent capabilities`.
  - 0.4.0 adds the structure writer settings (asset.json fields, LBX033, validation with the structure capabilities) and
    `checks.STRUCTURE_CAPABILITIES` against `capabilities --writer structure`.
- `tools/blender/make-examples.ps1 -Blender <blender.exe>` regenerates the example assets from code:
  - `content/props/lf_blender_barrel`: a 55-gallon drum with hoops, an original procedural texture, and LOD 0/1.
  - The drum's `.blend` and texture are kept in `source/` for hand editing.
  - `tests/content/fixtures`: the authoring fixtures (`make_fixtures.py`), checked by `LibertyContent fixtures`.
  - `content/props/lf_lod_post`: the structure writer's four-LOD test post (`make_lf_lod_post.py`, T-031).
- A portable Blender for tests lives outside the repository. On the owner's PC it is
  `D:\LibertyTools\blender-5.2.2-windows-x64`; set `LIBERTY_BLENDER` to its `blender.exe`.

## License note

The add-on folder is GPL-3.0-or-later, as Blender requires for add-ons that use `bpy`. It talks to LibertyContent only
through files and a process call, so nothing else in the repository is affected.
