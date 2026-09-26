# GTA IV model format (T-2 model pipeline)

**Status (2026-09-25):** the reader, the writer and a round-trip test are verified offline. The first generated models (sling straps) are installed; the in-game load is not tested yet.

**Tool:** `tools/models` builds `LibertyModel.exe`, which `package-phase2.ps1` compiles and runs.

| Command | What it does |
|---|---|
| `survey <game> <archive...>` | Parses every `.wdr` in an IMG or RPF and reports vertex layouts, shaders and failures. |
| `export <game> <archive> <model> <out.obj>` | Writes the high-LOD geometry as OBJ plus a PNG preview. |
| `selftest <game> <img>` | Rebuilds every single-geometry drawable from its own mesh and compares the result with the original. |
| `sling <game> config/models/sling.json <out>` | Builds the body-fitted straps, their texture, `LibertyModels.img` and `lf_models.ide`. |

All inputs are read from the player's own game files. Nothing from the game is committed.

## Evidence

- **Surveys:**
  - `weapons.img`: 79/79 drawables parse, including the Realistic Weapon Overhaul models.
  - `playerped.rpf`: 45/45 parse.
  - Geometry was checked visually through the exported previews (AK-47, Niko's jacket).
- **Round trip:** `selftest` rebuilt all 49 of 49 single-geometry props byte-identical. Two exceptions:
  - Bounds get a 1 mm tolerance: the game stores pre-quantisation bounds, while the builder derives them from the vertices.
  - Page flags are not compared.
- **Skeleton:** rebuilding Niko's model-space bone positions from local offsets and rotations matches the stored positions to 1.5e-7 m.

## Resource container
- **RSC05 header:** magic `RSC\x05`, type (110 = `.wdr`, 8 = `.wtd`), then flags.
- **Flags:**
  - System segment size = `(f & 0x7FF) << (((f >> 11) & 0xF) + 8)`.
  - Graphics segment size = `((f >> 15) & 0x7FF) << (((f >> 26) & 0xF) + 8)`.
- **Pointers:** `0x5xxxxxxx` point into the system segment, `0x6xxxxxxx` into the graphics segment.
- **Oversized body:** some third-party files (the installed `w_m4`) inflate to more bytes than the flags declare. The reader keeps the declared size, which is what the game allocates.
- **Page size:** the game's files size pages from the largest buffer: page shift = `log2(nextPow2(largest buffer)) - 12`.
- **Buffer placement:** a buffer never straddles an aligned block of `max(4 KB, 8 pages)`; it moves to the next block instead. The index buffer follows the vertices, 16-byte aligned. With both rules, the rebuilt props match byte for byte.

## In-game finding: one graphics page for generated models (2026-09-25)

The first strap build used Rockstar's placement: 12 pages of 2 KB, with the 21 KB vertex buffer at offset 0. In game, part of the strap rendered correctly and the rest became huge planes. So vertex data beyond a page block was not where the pointer arithmetic expected it.

The same mesh with the whole graphics segment in **one** 32 KB page (flags count 1, shift 7) rendered correctly from all sides (autopilot run `sling-review`). The builder therefore writes generated models as a single page. The multi-page placement stays only for the round-trip self-test against Rockstar's files. Why their multi-page buffers work is still open.

## Texture dictionary (.wtd)

**Status (2026-09-26):** the reader's offsets were verified against `w_glock.wtd` (finish pipeline). The writer
(`tools/content/TextureDictionaryWriter.cs`, [T-027](../tasks/T-027-lcc-native-textures.md)) reproduces the rules below.
They were matched against `coronas.wtd`, `hud.wtd` and `amb_nailgun.wtd` while it was written. A systematic comparison
over the game's archives (`LibertyContent wtdcheck`) and the in-game load are still open.

RSC05 type 8. Pointers as for drawables (`0x5…` system, `0x6…` graphics).

| Structure | Offsets |
|---|---|
| Dictionary header (0x20) | +0x00 vtable slot (varies between files; fixed up at load), +0x04 block map pointer (+0x20 in every file read), +0x10 hash array pointer, +0x14 u16 count, +0x16 u16 capacity, +0x18 texture pointer array, +0x1C u16 count, +0x1E u16 capacity. +0x08 and +0x0C are copied, not understood |
| Block map (0x210 at +0x20) | a zero word, then 0xCD fill in every file read |
| Texture record (0x50) | +0x00 vtable slot, +0x14 name pointer, +0x1C u16 width, +0x1E u16 height, +0x20 format FourCC (`DXT1`/`DXT3`/`DXT5`) or a D3DFORMAT code (21, 22, 50 in HD packs), +0x24 u16 row stride, +0x27 u8 mip levels, +0x48 pixel data pointer (graphics). +0x40 is written as 0 by the writer; `wtdcheck` counts the game's values. The rest is copied |
| Name | `pack:/<name>.dds`, NUL-terminated |

**Rules the writer relies on:**
- **Name hash:** Jenkins one-at-a-time over the lower-case name without `pack:/` and `.dds` (`TextureNameHash`).
- **Order:** the hash array is ascending and the texture pointer array follows the same order.
- **Row stride (+0x24):** one row of 4×4 blocks divided by 4: 128 for a 256-wide DXT1, 256 for a 256-wide DXT5.
- **Mip levels:** a full chain stops at the level whose smaller side is 4 pixels (256×256: 7, 128×128: 6, 64×64: 5), or a
  single level. The levels are stored back to back, largest first.
- **Flags:** bits 30–31 are set in every file read (meaning unknown; copied).
- **Texture data alignment:** at least 128 bytes in the files read (`coronas.wtd` puts `imp_car` at 0x5580). The writer
  uses 256.

**Writer choices, not yet confirmed in game:** records at +0x230, then names, hash array and pointer array (16-byte
aligned, unused bytes 0xCD); one graphics page when the data fits 8 MB (the choice that fixed generated drawables),
otherwise 8 MB pages with no texture straddling a page.

## RPF2 archives (`playerped.rpf`)
- **Header:** magic `RPF2`, TOC size, entry count, an unused word, then an encrypted flag.
- **TOC:** located at 0x800 and AES-encrypted with the IMG key.
- **Entries:** 16 bytes each (name offset, size, `offset | resource type`, RSC flags); the name table follows the entries.
- **Directories:** the third word has bit 31 set.
- **Resource data:** a complete RSC05 file.

## Drawable (gtaDrawable) — field offsets
| Structure | Offsets |
|---|---|
| Drawable | +0x08 shader group, +0x0C skeleton, +0x10 centre, +0x20 box min, +0x30 box max, +0x40 four LOD model-collection pointers, +0x50 LOD distances, +0x70 radius |
| Collection | pointer array, u16 count, u16 capacity |
| Model | +0x04 geometry collection, +0x0C bounding sphere(s), +0x10 shader index per geometry (u16) |
| Geometry | +0x0C vertex buffers ×4, +0x1C index buffers ×4, +0x2C index count, +0x30 face count, +0x34 u16 vertex count, +0x36 u16 primitive (3 = triangle list), +0x3C u16 stride, +0x3E u16 bone count |
| Vertex buffer | +0x04 u16 count, +0x08 data, +0x0C stride, +0x10 declaration, +0x18 data (second copy) |
| Index buffer | +0x04 count, +0x08 data (u16 indices) |
| Declaration | +0 usage mask, +4 u8 stride, +7 u8 element count, +8 u64 type codes (4 bits per semantic) |
| Shader group | +0x04 embedded texture dictionary, +0x08 shader collection |
| Shader | +0x14 parameter pointers, +0x1C parameter count, +0x24 parameter types (0 = texture), +0x44 name, +0x48 preset (`.sps`) |
| Texture reference | +0x14 name pointer |

**Vertex count quirks:**
- Some files leave the vertex buffer's first data pointer 0 and set only the second.
- The installed `w_m4` stores 0 in the geometry's vertex, index and face counts; the buffers' counts are authoritative.

**Semantic bits:** 0 position, 1 blend weight, 2 blend indices, 3 normal, 4 colour, 5 specular, 6–13 texcoords, 14 tangent, 15 binormal.

**Type sizes confirmed by stride sums:**
- 5 = float2
- 6 = float3
- 7 = float4
- 9 = 4 bytes (colour, packed indices or weights)

**Layouts seen:**
- Props: `0x59` (position, normal, colour, uv), 36 bytes.
- Props with tangent: `0x4059`, 52 bytes.
- Peds: `0x405F`, 60 bytes (adds blend weights and indices).

**Winding:** triangles are counter-clockwise around the vertex normal in every surveyed model (46,440 of 46,440 on `w_ak47`).

## Skeleton (crSkeletonData)
- **Header:** +0x00 bone array, +0x14 u16 bone count.
- **Bone record:** 0xE0 bytes:
  - +0x00 name
  - +0x10 parent pointer
  - +0x16 u16 bone id (the tag the natives use; Spine2 = 0x36A0, Head = 0x04B5)
  - +0x20 local offset
  - +0x40 local rotation quaternion (xyzw)
  - +0x60 model-space bind position
- **Model space:** chain the (offset, rotation) pairs from the root, including the root's own rotation.
- **Niko's Spine2 axes in model space:** X points up the spine, Y forward, Z to his left. The model faces +Y and is Z-up; left-side bones are at −X.

## Open question
**Rotation convention of `ATTACH_OBJECT_TO_PED` (through SHDN `AttachToPed`):** unknown. SHDN forwards the values to native code; the units (degrees or radians) and the Euler order are not visible.

The straps avoid the question by attaching with zero offset and zero rotation. The holster controller logs `holster_frame` once per placement: the prop's origin and axes in the bone's frame, from `GetOffsetPosition` and the engine's `CopyBoneMatrix`. The next playtest log settles the convention. After that, the weapon placements can be computed offline against Niko's body.
