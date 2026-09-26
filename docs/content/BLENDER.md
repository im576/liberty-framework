# Authoring in Blender (Liberty Exporter add-on)

Blender models and textures the asset. The Liberty Exporter add-on (`tools/blender/liberty_exporter`, v0.2.0) turns
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
3. Install `staging/liberty_exporter-0.2.0.zip` in Blender: Edit > Preferences > Get Extensions > ⌄ > Install from Disk.
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
- **One material per LOD** in compiler v1. The material is a Principled BSDF with an Image Texture linked straight into
  Base Color.
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
- **Hidden objects** in the asset collection are still exported, so an artist can hide LOD 1 while working on LOD 0.
- **Static only:** armature-deformed meshes export in their current pose (warning LBX015).
- **Shader:** a `liberty_shader` custom property on the material. It defaults to `gta_default`, the only shader v1 writes.

## The Liberty panel (3D View > Sidebar > Liberty)

| Field | asset.json |
|---|---|
| Name (eyedropper: from the active object) | `name`, and the folder `content/props/<name>` |
| Type | `type` (`prop`) |
| Collection | what to export, nested collections included. Empty exports the selection |
| Template / Model | `template` |
| Texture dictionary | `textureDictionary` (empty uses the name) |
| Texture mode | `textureMode`: Template (default, not written) or Native |
| Draw distance (m) | `drawDistanceMeters` |
| Audio material | `audioMaterial` |

**Buttons:**
- **Check:** the add-on's own checks (below).
- **Export:** runs Check, stops on errors, writes glTF + `asset.json`, then runs `LibertyContent validate`.
- **Build:** runs Export, then `LibertyContent build`. It shows the status and opens the preview, the decoded texture,
  `report.json` or the folder.

The same export is under File > Export > Liberty Asset.

The exported glTF carries `liberty_exporter`, `liberty_blender` and `liberty_source` as scene extras. `report.json`
lists them under `metadata`, so every compiled asset records what made it.

## Check codes (LBX)

They mirror the compiler's LCC codes and add what only Blender can see. LibertyContent's codes are still reported after
export.

| Code | Severity | Meaning |
|---|---|---|
| LBX001 | error | nothing to export (no mesh in the collection/selection) |
| LBX002 / LBX003 | error | asset / texture dictionary name not 1–23 letters, digits or `_` |
| LBX004 | warning | scene Unit Scale is not 1 |
| LBX006 | warning | largest extent outside 0.02–200 m |
| LBX007 | warning | model centre far from the world origin (spawn point) |
| LBX008 | info | lowest point not at Z = 0 |
| LBX009 | error | more than one material in a LOD |
| LBX010 | warning | faces without a material |
| LBX011 | warning | material the glTF exporter can't translate (no Principled BSDF, Base Color not a direct Image Texture) |
| LBX012 | error | image file missing, not a still image, or no pixels |
| LBX013 | warning | texture not a power of two, or above 2048 |
| LBX014 | error | textured mesh without a UV map |
| LBX015 | warning | skinned mesh (exported static, current pose) |
| LBX016 | warning | more than 65,535 face corners (may exceed the vertex limit) |
| LBX017 | error | no LOD 0, or LOD outside 0–3 |
| LBX018 | warning | LOD not lighter than the previous one |
| LBX019 | error | unsupported `liberty_shader` |
| LBX020 | warning | material alpha (Template mode: output is opaque; Native mode: DXT5, drawing unverified in game) |
| LBX021 | info | summary: objects, triangles per LOD, bounds |
| LBX022 | warning | object with no faces (ignored) |

## Scripts and tests

- `tools/blender/run-tests.ps1 -GameDirectory <game> -Blender <blender.exe>`:
  - Blender's extension validation, plus 25 headless tests (23 from 0.1.0 and 2 for the native texture build, case 3b;
    0.2.0 has not been run through the suite yet).
  - The tests cover registration, LOD tags, export files and extras, validate and build against the real game
    archives, and axis/metre round trips.
  - They also cover modifiers baked, rejected assets writing nothing, and every error check, plus collection export
    with inherited LODs and metadata.
- `tools/blender/make-examples.ps1 -Blender <blender.exe>` regenerates the example assets from code:
  - `content/props/lf_blender_barrel`: a 55-gallon drum with hoops, an original procedural texture, and LOD 0/1.
  - The drum's `.blend` and texture are kept in `source/` for hand editing.
- A portable Blender for tests lives outside the repository. On the owner's PC it is
  `D:\LibertyTools\blender-5.2.2-windows-x64`; set `LIBERTY_BLENDER` to its `blender.exe`.

## License note

The add-on folder is GPL-3.0-or-later, as Blender requires for add-ons that use `bpy`. It talks to LibertyContent only
through files and a process call, so nothing else in the repository is affected.
