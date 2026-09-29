# T-032 common WBD target nested pointers

Read-only probe branch `codex/pr7-wbd-nested` at `cb1b81f`, derived from the earlier target-class probe. `LibertyContent wbdnested` parsed 309 CE WBD resources and measured 2,154 targets with first word `0x0069C19C`. It emitted only offset, pointer, integer and word-class measurements; no resource bytes or float values. Five archives failed to open, matching the known archive-reader limitation. The build completed without errors.

All 2,154 samples contain valid system pointers at target offsets `+0x8C`, `+0xB0`, `+0xD0` and `+0xE0`. Their pointed-to first words differ:

| Field | First word at pointee | Bound before next known system-pointer target |
| --- | --- | --- |
| `+0x8C` | 1,121 zero; 1,012 float-class; 21 other | 1,210 at least `0x200` bytes |
| `+0xB0` | 1,947 other; 137 float-class; 67 small integer; 3 zero | Mixed; 926 between `0x20` and `0x7F` |
| `+0xD0` | 934 small integer; 785 other; 251 zero; 184 float-class | 1,826 below `0x20` bytes |
| `+0xE0` | 2,154 system pointers | 2,151 between `0x20` and `0x7F` bytes |

The pointer at `+0xE0` therefore consistently leads to a second system pointer. Candidate adjacent integers are listed in `nested.json`; for example, `+0xD8` holds small values in many samples. The stride scan found only one initial system-pointer word at the `+0xE0` pointee and none at the other three, across every tested stride (4–64 bytes). This does **not** identify an array stride, count, class size or shape. The next-known-pointer gap is only an upper bound because unreferenced allocations may lie between pointers. Further decoding and full-resource roundtrip remain open; authored collision remains disabled.
