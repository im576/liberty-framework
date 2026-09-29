# T-032 Session 5b: CE WBD table structure

Source: Mac evidence branch `70e7fd4`; read-only Windows implementation branch `codex/pr7-wbd-reader` at `4892c63`. GTA IV CE 1.2.0.59. `LibertyContent` built from 68 C# sources, and `selftest` passed **354/354**, including seven synthetic WBD reader checks and mutation cases. No game process was launched or install command used.

## Verified structural relationships

`wbdtrace` followed consecutive same-IMG WDR-name hash runs in ten selected CE IMG archives. Thirteen WBDs used the same root vtable, `0x00695360`. In each, root `+0x10` points to an array of 32-bit hashes, and the two 16-bit fields at `+0x14` hold equal count/capacity. Root `+0x18` points to a separate array of system pointers, with equal count/capacity at `+0x1C`. Every **100/100** hash entries in these 13 WBDs matched a WDR basename in the same IMG, and the 100 same-index pointers led to in-range structured targets.

| CE WBD | Hash array | Target-pointer array | Both counts | Example same-index pair |
| --- | ---: | ---: | ---: | --- |
| `bronx_e.wbd` | `0x4FB0` | `0x57B0` | 12/12 | index 0: `bxe_el_windows04`, target `0x48D0`, target vtable `0x0069C19C` |
| `bronx_w2.wbd` | `0x3AFD0` | `0x27FD0` | 11/11 | `wi_wt_mixer2` begins the hash array |
| `procobj.wbd` | `0x2FD0` | `0x2F70` | 21/21 | `cj_proc_wheel` begins the hash array; targets include four vtable classes |

The read-only `WbdDictionaryReader` validates that root layout and each indexed target pointer for CE WBD class `0x00695360`. Across the installed IMG set, `wbdcheck` found **348** WBD resources, of which **309** had this CE class. It parsed all **309/309**, with **3,239** table entries and **3,239** valid indexed system targets. **3,039** entry hashes matched WDR basenames in the same IMG; 200 did not under that limited rule. The targets had six first-word vtable values; the totals are in `wbdcheck.json`.

The reader re-encodes the decoded root pointer/count records and both arrays into fresh buffers, then compares those known fields with each original resource. This **table-field roundtrip passed 309/309**. Synthetic checks confirm that changing a hash or target pointer fails the comparison and that unequal counts or out-of-range targets are rejected. This does **not** round-trip the unknown remainder of a WBD or decode the pointed-to object's shape.

## Interpretation and limits

**Measured:** CE WBD class `0x00695360` has index-aligned hash and structured-target arrays, with verified local WDR-name matches in the selected samples. This establishes a structural hash-to-target association at each index.

**Inference:** those targets are likely collision bounds for the named models. Their vtables and WBD context support that interpretation, but the pointed-to classes and their geometry have not been decoded. The 200 unmatched names may refer to another archive, an IDE name, or another naming rule. The scan does not establish GTA IV's runtime lookup or whether a script-created object gets this collision.

Five IMG archives could not be opened (`Value was invalid.`); another 95 paths were skipped, mostly RPF. The 39 WBDs outside the CE root class were intentionally excluded from this parser. Authored collision remains blocked pending target-class decoding, a full relevant resource roundtrip, and an in-game pairing/raycast test.

The published JSON files contain names, offsets, counts, pointer targets/classes, and vtable identifiers only. They contain no resource bytes, geometry, textures, or other game assets. SHA-256, size, and UTC modification time for four installed executable/DLL files matched the prior baseline after this work; GTA IV was not running.
