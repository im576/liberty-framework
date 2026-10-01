# Stage 1 orchestrator integration review — 2026-10-01

## Integrated

Lane B `17444165d153a999e2e5fb1aabcb4df21b7f3ef9` is integrated in main `f823e45` and pushed. It supplies T-045's
weapon wheel, T-046's trunk UI, shared cached sprite text and their diagnostics/tests. Main's newer game lock,
audio recovery, config watcher, logging and handoff rules were preserved. The only merge conflict was the generated
local verification plan; regenerated from the combined queue, retaining all 77 checks.

Reviewed source and completed full run `20261001-092349-e470758` from `GTAIV-Reborn-lane-b2`:

| Check | Evidence |
|---|---|
| Package install | PASS; restored after the batch |
| UI text | PASS, 133 steps, no failed steps or log errors |
| Weapon wheel | All assertions passed; 12/12 equips/readbacks; first draw 0–1 frames |
| Trunk | Store/take/swap, ownership/ammo, full-capacity refusal, persisted-state round trips and capture release passed |
| Screenshots | Orchestrator inspected all 12 original wheel/trunk captures; readable text, amber highlights and swap/capacity feedback; knife's raw `Melee_Knife` name is cosmetic |
| Wheel budget | Average 22.75 ms, p95 34.4, p99 36.2; draw.ui average 0.217 ms |
| Trunk budget | Average 25.45 ms, p95 37.5, p99 46.0; draw.ui average 0.303 ms |

No 1 s stalls or scenario log errors in those wheel/trunk runs. ASI inventory recorded; ColAccel absent.
NEEDS-REVIEW screenshot status is distinct from failed assertions. This review does not mark a task DONE.
Physical controller, real save/load, safehouse/gunsmith, B/D HUD coexistence and owner judgement remain open.

Combined result: warnings-as-errors build PASS, repository verifier 424 passed / 0 failed / 7 not-run, PowerShell tool
tests 230 passed / 0 failed; queue/plan and art validation passed. Offline logs are preserved in
`GTAIV-Reborn-orchestrator/results-local/offline/orchestrator-b-{build,verify,tool-tests}.log`.

## Isolated follow-up

Review found the wheel synchronously reading/hashing arsenal.json every second even after main's background stamp
watcher fix. Commit `1f6f501` loads once on module start and registers the shared config through the same ConfigService
watcher. Changed files still reload on the engine thread, preserving validation and last-valid retention; idle ticks
perform no wheel file reads. Owner cleanup removes the watch.

Build PASS; verifier 431 passed / 0 failed / 7 not-run. Seven temporary-file tests exercise the actual service: no idle
reload, no game-thread stamp detection, both owners see a shared-file change, consumption once, owner removal, stopped
owner and timer disposal. No game directory is touched by these tests.

Full clean integration run `20261001-100907-1f6f501` is queued through verify-local with `-AnyBranch -NoPush -Restore
-NoManual -StopOnFailure -MaxGameMinutes 20`, no Quick: package, audio smoke, SDK selftest/events/hot reload, wheel,
trunk. Package preparation passed in 624 s outside the lock. Runtime evidence is pending; keep the follow-up isolated
until reviewed. This is not a claim that all engine stalls are fixed.

### Runtime review and host log fix

Full `20261001-100907-1f6f501` restored successfully: package/audio/SDK selftest/events PASS; hot reload FAIL because
the spawned self-test ped did not become the driver (48 passed / 1 failed). Wheel/trunk NOT-RUN after fail-fast.
Full `20261001-103726-1f6f501` also restored: hot reload PASS (24 steps, selftest 49/0), wheel FAIL on a missing log
expect despite its command returning `pass=True` (avg 21.09 ms, p95 32.3, p99 35.0, draw.ui 0.224 ms), trunk FAIL on
an actual frame-average budget miss (avg 24.37 ms, p99 69.3, combat peak 258.1 ms). These runs remain failures.

The host log cache clears its history if it observes the gap between the writer moving the old log and opening the
replacement. A deterministic temporary-file reproduction failed three existing/new rotation assertions. Fixed by
retaining the same session's history/partial line during that gap and opening current/backup logs with delete sharing.
A different missing path still returns no cached evidence. Four new regression checks; full PowerShell suite 234/0.
This confirms a reader defect; it does not prove that defect caused the wheel's missing line. All scenario assertions,
freshness checks and production budget limits remain unchanged. Full wheel/trunk acceptance is still required.

## Kept separate

- **C:** full `20261001-094219-2db0bf5` completed: package PASS, head/trauma NEEDS-REVIEW, dismember CRASH,
  firefight/effects-night FAIL. Earlier full firefight measured average 0.533 ms / peak 57.259 ms against 0.8/4 ms limits.
  Follow-up `dd8d380`, full `20261001-101856-dd8d380`, is active; its dismember check also crashed, at `gore clear`.
  No C source was changed or merged by this review. Do not weaken budgets.
- **R:** additive SDK 1.2 material API reviewed on `research/t050-material` (`e538722`). Full wood/glass/water/prop
  and SDK acceptance still required. Broader `research/stage1` (`b899b71`) contains SDK 1.3 radar ownership/probes;
  merge separately after the material work and renderer coexistence review. Quick runs are not acceptance.
- **D:** `codex/T-049-hud-continuation` remains unmerged. Saved full `20260930-205120-4ac4fcc` HUD FAIL;
  component evidence needs review and the clean hiding experiment is unrun. Preserve vanilla HUD through the guard.

Main density remains OFF. No baseline rerun, hardware/system changes, plugin removal, lane stop or worktree retirement.

## Preserved crash evidence and unresolved risks

Crash sidecars `095151`, `100559` and `102151` on October 1 identify `GTAIV.EFLC.FusionFix.asi` at relative offset
`0xA24E0`. Sidecar `101852` identifies `GTAIV.exe` at `0x664503`. These identify faulting modules, not the underlying
cause or the responsible lane. Full `.dmp`/`.txt` copies, `stall-20261001-100548` and SHA-256 hashes are preserved at:

`C:\Users\IM576\OneDrive\Documents\ChatGPT\GTA4-Reborn\research\integration-review-2026-10-01`.

T-056 now records two static risks requiring measurement: RuntimeLog holds its callers' lock during disk writes;
ArsenalCore snapshots/saves inventory synchronously during reconciliation. C's roughly 8.9 s `ar.reconcile` sample
does not prove either caused the stall. Persistence fixes must retain inventory/ammo integrity.

Sprite text is bounded by entry count and per-frame creation limits; LRU eviction explicitly disposes textures on the
draw pass. Explicit whole-cache disposal on engine unload is not shown in the reviewed source. Do not interpret the
lane's unload note as measured resource-release evidence; check unload/GC behaviour in the performance pass.
