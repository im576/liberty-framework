# T-031 targeted Windows roundtrip: Bronx east

Source: `origin/codex/pr7-verification-fixes` at `4f56e61`. The content tool built from 63 C# sources. Its new `Template` candidate was read-only and no GTA IV process was launched.

## Result

Command: `LibertyContent roundtrip --game <game> --out <report.json> pc/data/maps/east/bronx_e.img`

| Measure | Count |
| --- | ---: |
| WDRs in archive | 352 |
| Eligible for this writer | 187 |
| Eligible with multiple geometries | 90 |
| Identical under at least one candidate | 105 |
| Still different | 82 |
| Ineligible | 165 |

Candidate combinations among the 105 identical: `Interleaved+VerticesFirst+Template` 67, `Interleaved+Template` 10, `VerticesFirst+Template` 5, and **Template only** 23. Thus the new order resolves 23 files that neither previous candidate resolved: the previous candidates accounted for 82 identical and 105 different in this archive. This is the tool's system-segment, bounds, and vertex/index data comparison; the RSC page flags are not part of its pass condition. `pageFlagsEqual` is 66 of 187 eligible files.

The 165 ineligible WDRs have unsupported vertex layouts: 162 with mask `0x4059`, one `0x1D9`, and one `0x7D9`, one `0xFD9`.

## Remaining differences

I temporarily labeled every `Template` system offset against the parsed geometry, vertex-buffer, and index-buffer structure addresses, and retained all 82 failed files in `roundtrip-allfields.json`. The labels and numeric values are structural pointers, not asset contents. The temporary diagnostic change was removed after generating the report.

All **82** remaining files have changed `indexBuffer.data` pointer fields (250 differing offsets). **77** also have changed `vertexBuffer.data` and `vertexBuffer.data2` pointer fields (236 offsets of each kind). No vertex or index **data-content** mismatch was reported. Two files also fail the bounds containment check: `bxe_projbasedecal_01.wdr` (box axes 0 and 2) and `bx_eltrain_4.wdr` (axis 2). No other system field category appeared in the full comparison.

Examples:

- `agld04_03.wdr`: `sys+0xC78` and `sys+0xC88` are geometry 3's two vertex-buffer data pointers. The source value `0x6003FCF0` becomes `0x60019970` under `Template`. Geometry 4's pair at `0xCB8/0xCC8` and geometry 0's pair at `0xCF8/0xD08` also move.
- `burger_emissiv_ind5.wdr`: one-geometry case. Vertex pointers at `sys+0x518/0x528` move from `0x60015550` to `0x60000000`; the index pointer at `sys+0x558` moves from `0x6001B440` to `0x60005EF0`.
- `bxe_cp_emlights.wdr`: the only reported system difference is its index pointer at `sys+0x558`, `0x60002AA0` to `0x60002000`.

The source buffer **ordering alone** is insufficient. The remaining pointer changes show the rebuild assigns different graphics offsets, including in a one-geometry file; page placement, gaps, or padding need a separate diagnostic before changing the writer. The two bounds cases need independent review.

## Self-test and install integrity

The new commit initially failed three bounds-record self-test assertions because they still expected one matching order. A one-line assertion fix in `4ea6af6` expects both `Interleaved` and `Template`; after rebuilding, `selftest: ok passed=345 failed=0`. That fix is on `codex/pr7-windows-check` for cherry-pick/review and was not merged.

Before and after the offline probe, SHA-256, size, and modification time matched for `GTAIV.exe`, `pc/data/maps/east/bronx_e.img`, `scripts/LibertyFramework.net.dll`, `scripts/LibertyFramework/bin/LibertyCore.dll`, and `Liberty.Sdk.dll`. GTA IV was not running. No install command was used.

`roundtrip.json` is the unmodified tool report; `roundtrip-allfields.json` adds full structural field labels for the 82 remaining files. Neither report contains geometry buffers, textures, or other game asset bytes.
