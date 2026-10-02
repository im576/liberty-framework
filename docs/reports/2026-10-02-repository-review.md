# Repository and workflow review — October 2, 2026

The engine is a useful foundation, but the full mod is not yet stable or integrated. The immediate
improvement is to make failures cheaper to investigate and evidence harder to mislabel, preserve
the useful lane work, and tie deeper engine research to specific capabilities. This review makes
those changes; it does not claim to have repaired the gore crash or completed the larger overhaul.

## Scope and source of truth

Reviewed the architecture, native/core boundaries, SDK/services, reference gameplay modules,
content/build pipeline, test orchestration, agent guidance, source-mod research and failure receipts.
Inventory at start: **901 tracked files, 10 registered worktrees, 74 verification summaries**.
This is broad repository review and targeted source/failure analysis, not a claim that every line,
asset and historical screenshot has been individually audited or every engine path exercised.

Actual repository: `C:/Users/IM576/GTAIV-Reborn`, remote `im576/liberty-framework`.
`git ls-remote origin refs/heads/main` confirmed `65e272609315455a70abc95fd04122cc435df749`, matching
local main before this work. Feature branches contain additional work that is not integrated into main.
The chat's `Documents/ChatGPT/GTA4-Reborn` directory contains earlier evidence; new routing files
there point future agents to the real source without moving or discarding that evidence.

Read current B/C worker handoff turns as well as repository logs. Both explicitly retained their
candidates and the installed preview. No workers were dispatched, messaged or reconfigured.
The installed receipt still identifies preview source `36901ab`; this review did not install, launch,
rollback, stop the game or alter the owner's persistent game state.

## Architecture assessment

- **Native core:** version-dependent native resolution, snapshots, guarded calls, damage hooks,
  ray queries and crash capture behind a C ABI. The C#/C++ ABI has offline checks. Deeper hooks
  need lifetime/thread contracts and resolver validation, not merely additional offsets.
- **Managed engine/SDK:** a single SHDN host, module manifests, event dispatch, scheduler,
  resource ledger, commands, config/state, streaming and tick/draw separation. These are the
  right integration boundaries for porting useful mod logic.
- **Gameplay:** main includes the earlier gunplay/Arsenal/holster/wheel/trunk implementation;
  C/D/R hold unmerged work. Main's reference combat controller still uses sampling even though
  the engine provides exact damage events and C has migrated toward them. Availability of a
  core feature does not prove all gameplay consumers use it.
- **Content:** the single-geometry/template writer works within documented limits. The second
  geometry crash and incomplete authored collision remain real capability gaps. A round-trip
  parser test or a ray hitting a collision proxy cannot establish full authored physics support.
- **Vehicle ownership:** current Arsenal integration reads LVS-owned records. A complete owned
  vehicle system needs durable identity, transactions, bounded restoration and GTA save/load
  reconciliation. Copying LVS's entire script would also import its known persistence/runtime weaknesses.

## Repairs made

| Finding | Change | Verification |
|---|---|---|
| Resume could mix saved passes with a new source/mode and assume an old successful install still existed | Store full commit and queue hash; reject incompatible resume; require a fresh batch for pending game work after historical install; preserve unselected evidence and original start time | Negative identity tests and actual simulated republish; republish no longer changes a historical kept install into a claimed new restoration |
| Failed/timed-out rollback was text in the summary but not a failing batch count | Add `LOOP-restore: ERROR`, preserve backup/log evidence, and let existing CLI failure handling return failure | Failed exit and timeout tests, plus existing restoration-on-exception regression |
| Check selection only expanded one dependency level and stage sorting could put dependencies after consumers | Expand all prerequisites and order them before dependents; reject cycles before execution | Transitive, explicitly selected shared dependency and cycle tests |
| Startup test clock lost UTC information under newer PowerShell JSON deserialization | Preserve DateTime kind in the stub and assertions | Original baseline failed; corrected startup suite passes in both PowerShell versions |
| Test runner could report success for an empty filter and erased failed fixtures | Refuse empty suite selection, retain failed scratch files, add optional per-suite timing JSON, validate cleanup path | Empty-filter negative check; real failed fixtures were used to diagnose the timestamp issue; full suites |
| Build input scanning visited every backup before excluding it; external file identity omitted its directory | Prune backup/link subtrees before descent; use direct directory enumeration; include full external path; correct repository path-prefix boundary | Cache/fingerprint tests, actual installation timing and final builds |
| Handoff generation fetched once per worktree and recursively scanned ignored files | Offline default, optional single shared fetch, git-indexed source enumeration, primary-repo discovery and local settings | Actual no-clipboard handoff completes, includes preview identity and marks remote refs as cached |
| Live guidance combined obsolete restrictions, old game slots and completed cloud sessions | Archive history and make one current dashboard/schedule; remove forced five-minute inactivity delay and obsolete no-Phase-3 research rule | Local link review, preserved snapshots, source/task routing checks |
| Agents lacked a concise way to aggregate failures or inspect downloaded source | Add `tools/review/audit.py`, ZIP provenance inspector and two project skills | Python tests, all 74 receipts parsed with zero read errors, three local archives inspected; both skills validate |

The verifier's default branch is now `main`, matching current integration. The cloud/local workflow
was corrected: local agents exist and can run authorized tests; `develop` is not an automatic target.
No gameplay thresholds, density setting or production engine/native logic changed in this review.
Tooling repairs are committed locally as `6ef0446`. Preserved lane branches were not automatically
merged or rewritten; they must review the shared tooling changes before their next test. Nothing was pushed.

## What the logs say about wasted time and failures

The 74 local runs contain **58 full and 16 quick** batches. Across their check rows:
125 PASS, 73 NEEDS-REVIEW, 56 FAIL, 17 CRASH, 11 ERROR, 42 NOT-RUN.
These span multiple commits, scenes and configurations; they are historical outcomes, not a current
failure rate or acceptance percentage. The audit does not count `_runs` evidence copies as new batches.

**Repeated failures:** dismemberment and firefight each have ten failed/crashed/error check rows;
night effects have five. Eight timeout rows consumed about two hours of recorded check time,
including one 1,804-second loadout check. Repeating a long full batch without a discriminating change
is costly. Quick probes, first-failure review and exact stimulus attribution belong before full acceptance.

**Hidden setup costs:** summed run elapsed time is about 21.61 hours versus 14.11 hours in check
durations. The difference includes package work, waiting, cleanup, pauses/resumes and other overhead;
it is not all waste or CPU time. One saved audio smoke records a **390-second package build** in text
while its package/install check reports only five seconds. New runs expose `packageBuildSeconds`.
Do not optimize only the visible scenario duration.

**Fixture errors are not gameplay defects:** D's `20261001-203435-5c0cb8d` expected an unarmed clip
of 17 and failed at 26 steps. C's `20261001-165752-844c6a1` active comparison timed out before readiness;
it establishes no active-effects result. The earlier generated-XML ordering failure was packaging
preparation, not a HUD regression. Those distinctions are now explicit in the evidence skill.

**Performance attribution:** B's full `20261001-202311-2297a17` trunk sample failed p95 (46.4 ms
versus 40.7 ms baseline) and p99 (99.7 versus 68.0 ms); UI draw averaged 0.345 ms. Another historical
trunk sample includes a 258 ms combat peak. Menu-open frame cost alone does not identify rendering
as the cause. C's clone/sever/native-call peaks still need controlled attribution under original limits.

**Agent overhead:** the previous seven default guidance files totaled 82,402 bytes. Their replacements
totaled 17,509 bytes at cleanup, about **79% less entrypoint text**, with original content archived.
Stale schedules, repeated policies and arbitrary takeover waits were removed from the live path.
Source inspection and tests can continue while runtime work waits; no blanket stop is needed for an
unrelated unverified gameplay feature.

## Measured improvement and cleanup decisions

Three alternating fingerprint measurements on this installation gave median **507.2 ms before /
60.3 ms after**, approximately 8.4x faster for this helper. This is a small local sample, not a whole
package-build speedup or in-game FPS gain. The first implementation was slower when warm and was
replaced with direct directory enumeration before final validation. Both measurement sets are preserved.
The read-only orchestrator handoff completed in 6.44 seconds with no network fetch.

Tracked files initially occupied 116,193,565 bytes; art accounted for 94.82%. Twelve equal-image
groups use 24,943,348 duplicate working-tree bytes, but they are generated/approved provenance
copies used by the art workflow and Git already deduplicates identical blobs. They were deliberately
preserved. Deleting them without changing that workflow would exchange a small disk saving for
broken provenance. Art lifecycle validation passes.

The cleanup removes obsolete instructions from the active path, fixes the malformed/stale README,
updates task/research indexes and excludes crash/trace dumps from future source commits. It is not
a destructive history rewrite or a claim to reclaim gigabytes. Unique lane code, prior crash dumps,
screenshots, authored assets and owner downloads remain intact. Generated build caches are useful
for iteration and were not indiscriminately purged.

## Mod reuse and deeper engine work

Re-inspected the actual downloaded LVS, Violent Liberty and Liberty Rush ZIPs. LVS contains two
source files and an MIT notice; the other two inspected archives contain binaries but no source
or license notice. Recorded archive hashes and paths are in the local review outputs. This is an
inventory, not proof that a binary reproduces the included source or that every bundled asset has
the same provenance.

The [research program](../research/RESEARCH_PROGRAM.md) now specifies concrete controls, inputs,
observations and integration boundaries for skeleton/clone lifetime, combat/UI cost, owned-vehicle
transactions, hit materials, multi-geometry assets, collision, audio and frontend control. It directs
agents to adapt permitted source through Liberty services and use binary research to answer precise
questions. No downloaded implementation, decompiled game code or proprietary asset was committed.

## Remaining risks and limits

1. **Gore crash and performance acceptance remain open.** A fault in FusionFix identifies the fault
   location, not the originating cause. Do not weaken cleanup/peak limits or infer stability from this review.
2. **Trunk/HUD/material integration and real gameplay compatibility remain open.** Controller,
   mission/cutscene, actual game save/load, visible effects and owner feel need their own evidence.
3. **Native hook extension contract needs hardening before additional consumers.** Source inspection
   of `native/LibertyCore/src/hooks.cpp` shows an inactive named hook can retain its old trampoline
   when retargeted, while an already-active hook accepts the same target without comparing detour/length.
   Current damage-hook usage has one stable target; no connection to the recorded crash is established.
   Before exposing arbitrary reinstall/retarget operations, make the contract reject incompatible reuse
   or safely replace the trampoline and add native lifecycle tests.
4. **Shared-cache concurrent access needs a dedicated race test before allowing parallel heavy builds.**
   `Invoke-CachedStep` publishes via rename but can delete an existing entry before publishing/pruning.
   The current scheduling serializes heavy work; that is not proof of race-free cache readers/writers.
5. **Timing identity remains a tradeoff:** large external archive identity uses path/size/time, not
   full content hashes. Same-path edits preserving both size and timestamp can evade it; disable cache
   for such experiments and record the actual archive hashes with the run.
6. No new in-game run, visual review, Blender render or complete cloud-container run occurred here.
   Offline tests do not establish deeper reverse-engineered layouts or a complete mod.

## Validation and receipts

Local outputs: `results-local/offline/repo-review-2026-10-02/`.

| Check | Result |
|---|---|
| Forced C# SDK/engine/mod rebuild, warnings as errors | PASS, zero errors; SDK 101 sources, engine 207, mods 4 + 3 |
| Repository verifier, freshly rebuilt with `-NoGame` | 441 passed, 0 failed, 5 not-run (game-file sections) |
| Native core build and native unit tests | PASS |
| Content compiler self-test / fixtures | 364/0 and 5/0 |
| PowerShell 5.1 complete tooling suite | 270 passed, 0 failed |
| PowerShell 7 complete tooling suite | 270 passed, 0 failed |
| Python audit/archive tests | 3 passed |
| Art queue lifecycle / repository validation | 18/0; 12 requests valid |
| Local check queue / generated plan | 79 checks valid and plan in sync |
| Project skills | Both pass `quick_validate.py` |
| Handoff, mod inventories, receipt audit, diff/link checks | Passed; final report link check after this file was saved |

Key receipts: `audit.json`, `baseline-tools.log`, `tools-ps51.log`, `tools-ps7-final.log`,
`tool-timings-*.json`, `build-final.log`, `verify-final.log`, `native.log`, `content-test.log`,
`content-fixtures.log`, `fingerprint-timing-final.json`, `handoff.log`, and the three archive inventories.
The initial failed tooling run is retained alongside the repaired results.
