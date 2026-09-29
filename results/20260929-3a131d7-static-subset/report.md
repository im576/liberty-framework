# T-031 static drawable subset and two-material fixture

At source commit `3a131d7`, content selftest passed **364/364**. Bronx east broad roundtrip still reports 352 drawables, 187 eligible, 105 identical and 82 differing; its command correctly returns failure. The narrower **external-texture static subset** is 95 eligible, 91 identical, four differing. The subset excludes 81 with embedded dictionaries, 10 with skeletons and one with both; it is a diagnostic category, not a redefinition of broad success.

To capture the four failures beyond the standard first-25 report limit, branch `codex/pr7-bronx-layout-all` at `4f42c66` changes only the maximum listed diagnostics to 100. `four-failures.json` contains those four files' difference locations, page flags, graphics buffer ranges and aggregate classes of unmodeled ranges. It contains no original resource bytes.

| File | Source / rebuilt graphics bytes | Template-order differences |
| --- | ---: | --- |
| `bx_bay_barriers01.wdr` | 81,920 / 77,824 | `sys+0x8A8`; flags differ |
| `bx_eltrain_1.wdr` | 212,992 / 212,992 | `sys+0xCF8`; flags equal |
| `bx_eltrain_4.wdr` | 442,368 / 442,368 | `sys+0xDE8`, `sys+0xE78`, `box axis 2`; flags equal |
| `el_lights04.wdr` | 163,840 / 155,648 | `sys+0x998`, `sys+0x9A8`, `sys+0xA98`, `sys+0xAF8`; flags differ |

The existing `lf_fx_multimat` fixture was also compiled against the owner's game into local results only. Build and readback status were **ok** with the structure writer and automatic template `pc/data/maps/east/bronx_e.img/big_fence2_bxe`. It produced one LOD with two material geometries (20 vertices/10 triangles and 4 vertices/2 triangles), and two DXT1 textures (`64x64` with five levels, `4x4` with one level). This proves the compiler selected a fitting two-material template and read back the output; it does not establish in-game rendering or collision.
