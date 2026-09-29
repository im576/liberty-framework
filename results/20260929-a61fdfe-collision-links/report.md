# T-032 collision links: targeted Windows probe

Source: `origin/codex/pr7-verification-fixes` at `a61fdfe`, GTA IV CE 1.2.0.59. `LibertyContent` built from 64 C# sources and its self-test passed **347/347**. The command was `LibertyContent probe collision-links --game <game> --out collision-links.json`. It read installed IMG resources and wrote names, counts, and offsets only. GTA IV was not launched for this probe.

## Verified scan results

| Measure | Count |
| --- | ---: |
| IMG archives with both WDR and WBD/WBN | 195 |
| WBD/WBN resources scanned | 1,080 |
| Resources with at least one matching word | 172 |
| Matching aligned 32-bit words | 3,572 |
| Distinct same-IMG WDR model names matched | 3,569 |
| Matches in WBD | 3,564 |
| Matches in WBN | 8 |

The probe computes Jenkins hashes of each WDR basename and checks every aligned 32-bit word of WBD/WBN system segments **in the same IMG**. Five IMG archives returned `Value was invalid.` and were omitted. Another 93 non-IMG paths, mostly RPF, were skipped. These counts apply only to successfully opened archives and this matching rule.

Examples recorded in the JSON:

- `pc/data/maps/east/bronx_e.img`: `bronx_e.wbd` contains hashes matching twelve local WDR names in a consecutive word run at system offsets `0x4FB0` through `0x4FDC` (for example `bxe_el_windows04` at `0x4FB0` and `bx_hp2_grge_door01` at `0x4FDC`). This is a dense structural sequence, not a one-off match.
- `pc/data/maps/east/bronx_w2.img`: `bronx_w2.wbd` has an eleven-word consecutive run beginning at `0x3AFD0`, matching local WDRs including `wi_wt_mixer2` and `wt_pipe`.
- WBN matches are sparse. The capped example list includes `queens_m_8.wbn` / `am_qm_leaf_ho3` at `0x424B8` and `queens_m_9.wbn` / `pav_qm_ho3` at `0x8EBA4`.

## Interpretation and next gate

**Inference:** the dense WBD runs strongly support a WBD table keyed by model-name hashes. They make WBD a better candidate for the map collision relationship than a same-named per-model WBN file; the earlier same-name WDR/WBN inventory found zero pairs. This probe does **not** establish that these words are actual dictionary keys, which bounds pointer belongs to a given hash, whether WBN references use the same scheme, or whether script-created objects receive that collision at runtime. A 32-bit hash match can also be incidental.

The next read-only step is to decode one small WBD's containing structure and adjacent pointer/count fields, then confirm a hash-to-bounds association and round-trip that resource. An in-game pairing/raycast test is still required. This result does not authorize an authored collision writer.

Before and after the probe, SHA-256, size, and UTC modification time matched for `GTAIV.exe`, `scripts/LibertyFramework.net.dll`, `scripts/LibertyFramework/bin/LibertyCore.dll`, and `Liberty.Sdk.dll`. No install command ran during this probe. The JSON contains no resource or game asset bytes.
