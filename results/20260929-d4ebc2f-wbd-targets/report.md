# T-032 target vtable prefix inventory

Source `d4ebc2f`; read-only Windows probe implementation `8b6e8cb`. `T032-wbdcheck` passed through the standard verifier, with its JSON captured under `results/20260929-093318-d4ebc2f/` and **Install: not installed**. That check parsed and table-field-round-tripped 309/309 CE WBD resources.

The `wbdtargets` probe grouped all 3,239 indexed target pointers in those CE WBDs by the first 32-bit word at their targets. It reports only names, offsets, word classes, small integer counts and pointer classes. Five IMG archives could not be opened, as in the earlier WBD checks.

| Target first-word class | Entries | WBD resources | Distinguishing prefix observation |
| --- | ---: | ---: | --- |
| `0x0069C19C` | 2,154 | 298 | System pointers at target `+0x8C`, `+0xB0`, `+0xD0`, `+0xE0` in 2,154/2,154 samples. |
| `0x0069D56C` | 634 | 46 | System pointers at `+0x8C` and `+0xB0` in 634/634; not at `+0xD0` or `+0xE0`. |
| `0x0069BBEC` | 227 | 38 | System pointer at `+0x8C` in 227/227; `+0xB0` is a pointer in only 2/95 bounded samples. |
| `0x0069AAF4` | 185 | 40 | System pointers at `+0x8C`, `+0xB0`, `+0xD0` in 185/185. |
| `0x0069D9E4` | 35 | 7 | No consistent system pointer at `+0x8C` or `+0xB0`. |
| `0x0069D7F4` | 4 | 2 | No consistent system pointer at `+0x8C` or `+0xB0`; very small sample. |

All six classes share a prefix whose words `+0x08` through `+0x28` mostly classify as floats or opaque words, and `+0x7C` is the small integer `1` in every sampled target. No float values or geometry are included in the JSON. The probe stops each prefix scan before the next *dictionary target* address; other allocations may intervene, so classifications beyond the stable offsets are exploratory and do **not** establish object size or field meaning.

**Candidate for next read-only decode:** `0x0069C19C` is the most common class and appears in 298 WBD resources, including `bronx_e.wbd` and `bronx_w2.wbd`. Its four consistent system-pointer fields make it the smallest useful *research slice* to follow. This is an inference about research priority, not a verified shape or container label. The other five classes remain distinguishable but undecoded. No authored collision writer or runtime pairing claim follows from this inventory.

The game executable and three installed DLLs matched the previous SHA-256/size/UTC-time baseline after this offline work. GTA IV was not running and no install command was used for the target probe.
