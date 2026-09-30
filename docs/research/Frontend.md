# T-055 / R6 — Frontend and pause menu, CE static spike

Date: **2026-09-29, America/Los_Angeles**. Answer: **Phase 3**. Static entry/data candidates are recorded; no hooks installed or game launched.

Confidence convention: **VERIFIED OFFLINE** = observed local files/bytes/source; **PLAUSIBLE** = interpretation not established by execution; **UNKNOWN** = not established. These labels do not promote source comments or a signature hit into runtime safety.

## Decision

- **VERIFIED OFFLINE:** The executable has a frontend initialization/selection/parser call chain with matches to the frontend XML section names, existing menu/layout override files, and a distinct code region matching FusionFix's map-crosshair lead.
- **UNKNOWN:** Full pause update/render entry points, calling contracts, thread/paused-state lifecycle, FusionFix hook coexistence, and safe replacement of the map. Therefore this is a Phase 3 handoff; nothing ships in Stage 1, as T-055/STAGE1 require.
- **PLAUSIBLE:** The smallest next capability is a read-only frontend-state/render observation, with an independently proven entry resolver and lifecycle, before menu or map replacement. The initialization parser is not itself a per-frame frontend renderer.

## Reproduction and source pins

**VERIFIED OFFLINE:** [Independent PE probe](../../tools/research/frontend_t055_probe.py) scans executable PE sections for exact/wildcard signatures and records disassembly, match counts, raw pointer occurrences, file hashes, XML sections/counts and layout rows in [frontend evidence](../../tools/research/frontend_t055_evidence.json). Raw pointer occurrences are explicitly not treated as proven xrefs.

```powershell
Set-Location 'C:\Users\IM576\GTAIV-Reborn-research'
python tools/research/frontend_t055_probe.py --game 'C:\Games\Grand Theft Auto IV\GTAIV' > tools/research/frontend_t055_evidence.json
$env:JAVA_HOME='D:\GTAIV-Reborn-Tools\jdk-25.0.4.1+1'
& 'D:\GTAIV-Reborn-Tools\ghidra_12.1.4_PUBLIC\support\analyzeHeadless.bat' 'D:\GTAIV-Reborn-Tools\analysis' GTAIV-CE -process GTAIV.exe -readOnly -noanalysis -scriptPath 'C:\Users\IM576\GTAIV-Reborn-research\tools\research' -postScript frontend_t055_ghidra.java 005be150 005be250 005be340 005adea0 005a9412 -log 'C:\Users\IM576\GTAIV-Reborn-research\tools\research\frontend_t055_ghidra_run.log' -scriptlog 'C:\Users\IM576\GTAIV-Reborn-research\tools\research\frontend_t055_ghidra_evidence.txt'
```

- **VERIFIED OFFLINE:** [Ghidra script](../../tools/research/frontend_t055_ghidra.java) uses `PseudoDisassembler`, not database-mutating analysis or function creation. The [script log](../../tools/research/frontend_t055_ghidra_evidence.txt) and [run log](../../tools/research/frontend_t055_ghidra_run.log) identify read-only processing. Ghidra's local `support/analyzeHeadlessREADME.md`, sections `-readOnly`/`-noanalysis`, documents discard of changes and disabled auto-analysis.
- **VERIFIED OFFLINE:** The project reports `x86:LE:32:default`, image base `0x00400000`, 134 existing functions, executable SHA-256 `08759a5516f9837920ea504436236bbab89d0826a8e4d04ff106345177b5345d`, matching the actual 17,425,752-byte CE 1.2.0.59 executable. Ghidra reports `function=UNKNOWN` for the inspected anchors; these routine boundaries are byte/control-flow candidates, not imported authoritative symbols.
- **VERIFIED OFFLINE:** Local FusionFix commit `cbfebd2e672c15277e33f5554281a8d49fcabd03`; relevant files: `source/settings.ixx:1350`, `source/fixes.ixx:662`, `source/comvars.ixx:2692,2836`, `source/dllmain.cpp:11–48`. File hashes are in [audio evidence](../../tools/research/audio_t053_evidence.json). License GPL-3.0, research only; no source implementation copied. Locally derived signatures below come from observed CE instruction bytes.

## Menu data already present

| Path under game root | Observation |
|---|---|
| `common\data\frontend_menus.xml` | **VERIFIED OFFLINE:** 63,998 bytes; root `FrontendMenu`, version 1; sections `sMenuDisplayValue`, `sMenuScreen`; 30 `menupc`, 68 `optionspc` elements |
| `update\common\data\frontend_menus.xml` | **VERIFIED OFFLINE:** 89,828 bytes; same sections; 38 `menupc`, 173 `optionspc`; includes `PREF_TRANSPARENTMAPMENU` |
| `common\data\frontend_pc.dat` / `update\common\data\frontend_pc.dat` | **VERIFIED OFFLINE:** 23,257 / 23,351 bytes; text layout/timing/map rows |
| `common\data\frontend.dat`, `frontend_360.dat` | **VERIFIED OFFLINE:** 13,455 / 13,883 bytes; additional layout inputs |
| `pc\textures\frontend_360.wtd`, `hud.wtd` | **VERIFIED OFFLINE:** Files exist; texture contents not decoded by this spike |

- **VERIFIED OFFLINE:** XML has `<menu HeaderText="MH_MAP" enum="SCR_MAP">`; entries use action/label/value/scaler/displayValue attributes. DAT keys include `MAP_map_scroll_zoom`, `MAP_map_WestNorth_limit`, `MAP_map_EastSouth_limit`, `FAD_main_fade_time`, and text/layout dimensions. Exact hashes and all observed map rows are in the frontend evidence.
- **UNKNOWN:** Which file is effectively loaded under the installed loader, what edits are safe, and whether XML enum IDs permit arbitrary new screens. Existing overrides are not evidence of a public frontend API or map replacement.

## Static chain and candidates

**VERIFIED OFFLINE:** All VAs here are navigation evidence for the pinned executable only; never implementation constants. RVA = VA minus `0x400000`; wildcard absolute/relative operands and validate executable boundaries before proposing any runtime resolver.

| Anchor (VA / RVA) | Observation and interpretation |
|---|---|
| `005BE150` / `001BE150` | **VERIFIED OFFLINE:** Unique reader-lead signature; clears/frees two arrays with 12-byte and 24-byte strides, then calls `005BE250` at `005BE1BE`. **PLAUSIBLE:** Frontend definition initialization. |
| `005BE250` / `001BE250` | **VERIFIED OFFLINE:** Derived signature unique; aligned stack frame; selects data and calls `005BE340` at `005BE297`, with fallback call at `005BE322`. **PLAUSIBLE:** Definition-file selector/loader. |
| `005BE340` / `001BE340` | **VERIFIED OFFLINE:** Derived signature unique; pushes a format string, invokes an opening helper, compares XML section names at `005BE3C0` (`sMenuDisplayValue`) and `005BE4C6` (`sMenuScreen`). **PLAUSIBLE:** Menu XML parser. |
| `005ADEA0` / `001ADEA0`, body `005AE033` / `001AE033` | **VERIFIED OFFLINE:** Candidate boundary has stack alignment/allocation; body matches unique map-crosshair lead and computes SSE coordinates/packed color. **PLAUSIBLE:** Map crosshair renderer, not whole-map render/update. |
| `005A9412` / `001A9412` | **VERIFIED OFFLINE:** Unique menu-tab lead reads `[0x01160C40]`, tests 8, 49 and 3, then branches into drawing-related code. **PLAUSIBLE:** Tab-dependent frontend background path. Entry/ownership of this region **UNKNOWN**. |
| `005C2A41` / `001C2A41` | **VERIFIED OFFLINE:** Unique call-sequence lead used in FusionFix `dllmain.cpp`; first rel32 call decodes to navigation VA `004016A0`. **UNKNOWN:** Actual target semantics; pseudo decoding at that target produced implausible instructions. Do not call or hook it based on the source label. |

- **VERIFIED OFFLINE:** Raw pointer occurrence at `005BE3C1` is confirmed as the operand of `push 00F8D61C`, followed by a compare helper call; string at that operand is `sMenuDisplayValue`. Likewise `005BE4C7` is the push operand for `sMenuScreen` at `00F8D5BC`. Thus these two are validated instruction references, not merely string-search associations.
- **PLAUSIBLE:** Observed array bases near `019D30C0` and `019D3390` hold display definitions and screen definitions respectively, inferred from the reset loops and parser indexing. Field ownership, capacities, allocation rules and published struct layouts remain **UNKNOWN**; do not expose these as writable SDK structures.
- **UNKNOWN:** The call target's odd decoding could reflect protected/transformed code, a thunk or another static-view limitation. No decryptor or live-memory dump was attempted.

## Candidate byte patterns and detour constraints

**VERIFIED OFFLINE:** Match counts below are from executable PE sections, not a live process. `?` denotes one wildcard byte. Derived signatures describe this binary only; uniqueness does not establish version portability.

| Candidate | Pattern / observed count |
|---|---|
| Reader lead (FusionFix settings) | **VERIFIED OFFLINE:** `51 56 57 64 8B 3D` — 1; alternative `51 53 56 BE ? ? ? ? 33 DB` — 0 |
| Selector, derived | **VERIFIED OFFLINE:** `55 8B EC 83 E4 F8 81 EC 8C 02 00 00 8B 0D ? ? ? ? 53 0F B7 41 04 56` — 1 |
| XML parser, derived | **VERIFIED OFFLINE:** `51 53 55 56 57 8B 3D ? ? ? ? 6A 01 6A 00 68 ? ? ? ? 51 B9` — 1 |
| Crosshair body (FusionFix fixes) | **VERIFIED OFFLINE:** `F3 0F 10 15 ? ? ? ? F3 0F 10 5C 24 ? 0F B6 C0` — 1; alternative with `10 1D ... 54 24 ... B6 C8` — 0 |
| Crosshair candidate prologue, derived | **VERIFIED OFFLINE:** `55 8B EC 83 E4 F8 83 EC 58 A1 ? ? ? ? 33 C4 89 44 24 54 56 57` — 2 (`005ADEA0`, `00921DA0`); not a standalone unique resolver |
| Menu-tab lead | **VERIFIED OFFLINE:** `A1 ? ? ? ? 83 F8 08 74 17` — 1; alternative ending `74 0C` — 0 |
| Menu-active lead (FusionFix comvars) | **VERIFIED OFFLINE:** `80 3D ? ? ? ? ? 74 4B E8 ? ? ? ? 84 C0` — 0; unresolved, not a verified CE global |

- **VERIFIED OFFLINE:** Reader's first instructions are `push ecx; push esi; push edi; mov edi,fs:[2Ch]`; a complete displacement is 10 bytes, with no relative branch in those bytes. Selector's `push ebp; mov ebp,esp; and esp,-8` is a complete 6-byte prefix. XML parser's first five pushes total 5 bytes. The candidate crosshair entry shares the selector's 6-byte prefix. These are static relocation candidates only; ABI/entry confirmation is **UNKNOWN**.
- **VERIFIED OFFLINE:** The crosshair body begins an 8-byte `movss xmm2,[absolute]`; the next reads `[esp+14h]`. It is a mid-function site, with live register/stack dependencies. Source names use `create_mid`; that is not the LibertyCore entry-only interface.
- **VERIFIED OFFLINE:** `native/LibertyCore/src/hooks.cpp` copies caller-supplied displaced bytes verbatim. It does not relocate relative calls/branches or automatically infer instruction boundaries. `ADR-0007` requires expected-byte validation and owned removal. The call-sequence lead starts with rel32 `E8`; it is unsuitable for this verbatim trampoline. No hook or API capability was added.
- **PLAUSIBLE:** Source-named FusionFix hooks may already modify these sites in the running baseline, especially the reader/map code. Static executable bytes cannot prove coexistence. Rejection of mismatched live bytes is required, but safe chaining or coexistence remains **UNKNOWN**.
- **VERIFIED OFFLINE:** FusionFix `onMenuDrawingEvent` is dispatched inside its `CGameProcessHook` when a menu-active flag is set, before returning to the original routine. **UNKNOWN:** Whether that callback corresponds to the actual render boundary or runs on every paused frame. The callback name does not establish a frontend renderer ABI.

## Tool licenses and remaining work

- **VERIFIED OFFLINE:** Ghidra 12.1.4 PUBLIC local `LICENSE` is Apache-2.0. Capstone 5.0.9 local `C:\Users\IM576\AppData\Roaming\Python\Python314\site-packages\capstone-5.0.9.dist-info\LICENSE.TXT` gives BSD-3-Clause terms. pefile 2024.8.26 local `pefile-2024.8.26.dist-info\LICENSE` is MIT, Ero Carrera. These installed tools were used externally; none is vendored. Game assets/source are not redistributed, and GPL FusionFix implementations are reference-only.

**UNKNOWN — F1:** Prove the top-level pause update and render call chains, including input, menu-active global and route-map ownership. **UNKNOWN — F2:** Confirm calling conventions, full instruction boundaries, thread, pause/loading behavior, and reset/shutdown lifetimes. **UNKNOWN — F3:** Establish safe observation while FusionFix owns its existing hooks; no automatic detour chaining. **UNKNOWN — F4:** Validate an independently derived, unique entry signature tied to a second semantic anchor, not a borrowed source name. **UNKNOWN — F5:** Identify whether map tiles/blips/input can be observed independently of the pause UI. These are recorded questions, not requests to the owner.

**PLAUSIBLE — future Phase 3 procedure:** Start with logged read-only state transitions; only after F1–F4 pass propose a small observer ADR and explicit runtime checks. Full frontend/map replacement follows after keyboard/controller navigation, pause/unpause, mission/cutscene/loading, device reset and teardown are evidenced. No checks are queued and no installed hooks are authorized by this static spike.
