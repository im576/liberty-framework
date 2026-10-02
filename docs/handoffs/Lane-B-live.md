# Lane B live handoff (T-045 weapon wheel, T-046 trunk UI; T-044 is finished and on main)

Worktree `C:\Users\IM576\GTAIV-Reborn-lane-b2`, branch `codex/lane-b-validation`. Not merged, not pushed (orchestrator integrates).
Update this file after every game run and meaningful commit. Rules for a successor: `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md`.

## Current milestone — full run reviewed, no new slot (2026-10-01)

This section supersedes the historical next-step instructions below. The finished B slot is released; do not rerun it.

### Full receipt 20261001-202311-2297a17 — latest, candidate HELD unmerged

### Paired window observation candidate — offline follow-up, latest

Owner authorized bounded metrics/state observation after6864e38 diagnosis; resumed preserved WIP without discarding
files. Separate patch changes only PerfService, ArsenalCore observer registration and TrunkSequence managed state,
plus focused runner/harness/docs. No speculative trunk repair or original failure removal.
Source/harness/docs candidate committed as `d9fa8a7363e792882a09aa47fbe325b3d76a9eae`; this follow-up records its SHA.

PerfService retains exact CostReader.Command text already returned/reset by `ui-budget baseline`, logging it beside
the same histogram percentiles. Begin assigns diagnostic ui_window identity, records existing Engine.Frame/ticks and
managed state, and emits one command-boundary INFO receipt; baseline/check append window/baseline identity and
start/end state/frame/ticks. Engine.Frame is read only. Original resets/order, histogram/cost collection, thresholds,
return success/error prefixes, scenario windows/config/density/modules remain unchanged. Logger reader independent.
Arsenal registers a managed-state delegate with owner ledger cleanup; reads storage open/closing/locked/wheel state
and trunk Active/Completed/StepIndex/browsing/closeRequested/handleRequested. These do not query player/control/vehicle
or animations. Callback only runs at metric command boundaries; no draw work/native queries/per-frame file writes.
Missing/stopped observer returns unavailable; observer exceptions log ERROR and keep metrics/report evaluation.
This adds one owned diagnostic ledger entry while Arsenal runs, released on stop; not input capture or control lock.

Focused command `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/tests/Run-UiBudgetObservationChecks.ps1`
completed **25 passed / 0 failed**, zero compiler warnings/errors, C#7.3/x86 warnings-as-errors, existing SDK SHA256
`5D82021B3A3371A8AD1BF633945318F6777CF9DFF9401E0AD12EECD7194502E4`.
Actual PerfService/CostMeter/UiBudgetLogic/RuntimeLog/ResourceLedger/TrunkSequence sources compiled, with engine/SHDN
boundaries and IChoreography state fixture. Checks verify paired baseline/open frames+costs, periodic reader independence,
unchanged begin/baseline/check resets, invalid-command no-reset behavior, no observation during sample collection,
managed active/completed/browsing states, owner/replacement cleanup, observer failure logging and original relative p95
FAIL with compliant mean/draw average. Actual logger writes temporary files; no production logger implementation changed.
Harness initially needed FileShare.ReadWrite/Delete because real logger holds writer open, and failing return has
`error ` prefix while existing INFO log does not; both harness expectations corrected before final25/0 receipt.

Receipt `results-local/offline/ui-budget-observation-checks.log`; actual final logger evidence
`results-local/offline/lane-b-ui-budget-observation/323d36b3c8eb40a8b05bae073d2c6e01/logs/LibertyFramework.log`.
No unchanged passing checks rerun after usage resume. Diff whitespace check PASS. Full production build/verifier/tool
suite/runtime unrun for this observation patch; ArsenalCore registration is source-reviewed, not harness-compiled.
Boundary observation cannot reconstruct transient state changes or correlate individual cost maxima to frame tails.
No cause asserted, acceptance unchanged: fresh202311 trunk FAIL and later flows NOT-RUN remain; candidate heldunmerged.

Proposed ONE future full batch after parent host-startup review and explicit slot:
`tools/verify-local.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -AnyBranch -NoPush -Restore -NoManual
-MaxGameMinutes 30 -StopOnFailure -Only @('LOOP-build','LOOP-verify','LOOP-package-install','T046-trunk-ui')`.
No Quick/AllowOtherBuild; preserve existing scenario windows and gate. Required scheduled production checks first,
then committed full run. Estimate8–12min including startup/stage/verifier/restoration, cap30 unchanged; estimate only.
Inspect exact paired costs/frame stats/state, captures and restoration; stop at failed/unavailable return, no second
invocation. Passing wheel/SDK not selected; watcher batch not authorized. Parent reviewing isolated host fix; B has
no heavy/game/install slot. No main push/merge, no other worktree edit or subdelegation.

### Previous full receipt (preserved)

Single authorized full batch tested clean2297a17 (radial b3db1bc, main host689bac5); no Quick/AllowOtherBuild.
Exact Only IDs: LOOP-build,LOOP-verify,LOOP-package-install,T045-weapon-wheel,T046-trunk-ui,SDK-ui-review;
AnyBranch/NoPush/Restore/NoManual/MaxGameMinutes30/StopOnFailure. Queue runs SDK before wheel/trunk.
Production warnings-as-errors build zero warnings/errors, NoGame441/0/5 not-run, full tools255/0.
LOOP-build cached PASS; full game-file LOOP-verify1020/0 after Stage; package/install PASS. Engine DLL SHA256
8C52778AF2D8FE20F31E25C823186DEFD3E33505159468824C317B0A3EA0835A. Offline logs:
results-local/offline/lane-b-production-2297a17/{build,verify-nogame,tool-tests,full-batch-console}.log.
Run03:23:11.070Z–03:31:10.264Z (<8min). Startup permitted built-in attempt2 booted; attempt1 seen=False preserved.
No second verifier or failed-check rerun. Summary retains PASS3/NEEDS-REVIEW2/FAIL1, not whole-batch acceptance.

SDK21/0/0 log errors, wheel99/0/0, trunk41 executed/1 failed/0 errors; game alive at each scenario end.
Wheel12/12 cycle equips plus keyboard hold/tap pass. All opening groups paired with original target<=1 frame:

| Group | Opens/draws | Engine frames | Wall-time ms |
|---|---:|---|---|
| Initial | 1/1 | 0 | 15 |
| Three cycle rounds | 12/12 | all0 | 15,16,15,15,47,15,31,16,31,16,62,16 |
| Keyboard hold | 1/1 | 0 | 0 |
| Keyboard tap | 1/1 | 0 | 0 |
| Later budget opening | 1/1 | 0 | 0 |

All16/16, max0 frames/62ms, no missing/orphan/negative pairs. Fresh full run passes measured timing criterion for
these groups; not visible presentation/all controller/load acceptance. Old2-frame/359ms failure remains preserved.
Wheel budget PASS closed/open avg22.49/23.58 (+4.85%), p9539.0/29.3, p9953.9/34.3, max104.7/42.1ms;
draw.ui avg/max0.227/0.7ms, baseline457/open443 frames, no measured>=1s stalls in either window.
Trunk FAIL unchanged `ui-budget check trunk 0.5 1.10 1.15 30`, reason frame_p95: closed/open avg26.44/27.92
(+5.60%), p9540.7/46.4 (+14.00%, limit+10%), p9968.0/99.7 (+46.62%, also beyond+15% though p95 stops first),
max186.3/141.7ms, baseline394/open379 frames, draw.ui avg/max0.345/5.8ms, no measured>=1s stalls.
Compliant draw average does not erase frame-gate failure. Later store/take/swap/capacity/state round-trip/close flows
NOT-RUN due fail-fast; no fresh PASS substituted from historical trunk acceptance. Cause unproven.

All10 stored960x540 JPGs individually viewed (SDK/wheel originals also viewed before compression):
- SDK: sdk_list_menu.jpg, sdk_list_menu_moved.jpg, sdk_radial_menu.jpg. Readable list, highlight moves to Notify,
  radial eight slots/icons/initial0/centre visible; long explanatory line ellipsized. Parent independently viewed3.
- Wheel: wheel_open_sidearm.jpg, wheel_long_gun_1.jpg, wheel_long_gun_2.jpg, wheel_melee.jpg,
  wheel_empty_slot.jpg, wheel_keyboard_highlight.jpg. Correct amber selection/names/ammo/finish/empty slot/keyboard
  AK highlight and readable footer; Melee_Knife cosmetic remains. Parent independently viewed6. No observed clipping.
- Trunk: trunk_ui_open.jpg. Sidearm selected0, Glock17/100 centre, empty TRUNK0/8 right panel/footer readable.
  Parent independently viewed1; no later captures exist. Summary NEEDS-REVIEW labels retained, review separate.

Installed identity checked during B mutex: lane-b2/codex-lane-b-validation/2297a17/dirty=False; wait0.017s.
All five ASI paths/SHA256 identical to prior122159 B run; exact inventory/game hashes in summary.json, no ColAccel.
Density OFF, T040 baseline/budgets unchanged. Restore confirmed summary AND installed metadata
phase2-20261001-202329 at03:31:10.031Z. Post-run no GTAIV/compiler/verifier, holder note absent, actual named mutex
WaitOne(0) acquired/released successfully (free). B slot released; D now holds next heavy/game slot.

Parent holds entire candidate unmerged because shared UI also affects storage. Offline bounded trunk diagnosis
follows receipt; initial snapshot once/open is not assumed cause of continuous p95. No counters/budgets/warmups/module
switch changes, automatic retry or new run. Watcher rejection/recovery/restart fixtures remain unrun/new-slot only:
LOOP-package-install,T045-config-watch-reload,T045-config-watch-restart. Future T046-trunk-ui requires evidence-based
plan and slot; unchanged passing wheel/SDK not automatically rerun. Owner controller/real save-load/safehouse/gunsmith/
HUD coexistence/feel remain; no task DONE.

Offline source/log diagnosis completed after receipt, no runtime/source changes:
[paired trunk diagnosis](../reports/2026-10-01-lane-b-trunk-budget-diagnosis.md). Browsing intentionally keeps choreography
active throughout open window; wheel-ready status proves opening steps ended, not choreography completion. Historical
PASS open scheduler0.348/1.7 vs fresh0.362/2.1 (quiet~0.001 not valid paired baseline), snapshot avg identical0.137.
Initial snapshot occurs before open budget reset, so not directly included as repeated work. Holster19ms peak is
outside near-zero ho.show; native/call culprit unproven. Exact closed cost baseline is unavailable because existing
ui-budget baseline discards command costs; periodic logger is a different reader/window. Aggregate maxima lack frame
alignment, so no proved p95 cause or speculative fix. B remains heldunmerged; D current slot. Proposal for parent:
bounded observational paired costs/state and, only if assigned, slow frame-correlated owner-tick query scopes.

### Initial radial snapshot fix — historical offline source milestone

Ready-to-test update: parent independently reviewed `b3db1bc`/`b6da19e`, including the actual-source harness and
negative control. Clean B then fetched origin and merged pushed main `689bac5` as `85faad7` without conflicts.
Both sets of documentation retained. Merge changes only host VerifyLocal staging/reuse, verifier phase2 input lookup,
host tests and coordinator docs; diff confirms radial production/harness/SDK/config/scenarios/queue unchanged.
No redundant focused tests, full build, verifier, package/install or game launched. R owns the current slot; B waits.

Proposed slot production checks: `tools/build.ps1 -ScriptHookDotNetReference
'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`, `tools/verify.ps1 -NoGame`, and
`tools/tests/Run-Tests.ps1`. These are PLANNED, not executed/passed for this patch. Full batch A IDs:
`LOOP-verify,LOOP-package-install,T045-weapon-wheel,T046-trunk-ui,SDK-ui-review`; includes generated-input verification
under parent's corrected Build -> Stage -> Verify -> Install ordering. Full batch B IDs:
`LOOP-package-install,T045-config-watch-reload,T045-config-watch-restart`.
Both use `tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -AnyBranch -NoPush -Restore
-NoManual -MaxGameMinutes 30 -Only <IDs>` with real PowerShell comma-separated arguments; no Quick acceptance.
Do not run either until B is assigned a slot. Recheck shared state before installing; always record restoration.

Estimate A: 12–18 minutes of lock/game allowance including stage/verify/startup/captures; B: 5–8 minutes including
startup/callback waits/cleanup. These are estimates, not reserved slots or promises. Prior full receipt measured wheel
300 s and trunk 142 s; SDK UI scripted waits total about 9 s plus launch/capture overhead. Heavy compilation is
separately scheduled outside these estimates. If generated-input verification consumes the cap, record remaining
checks NOT-RUN and request a bounded follow-up rather than removing gates. Review every fresh capture and original
budget/<=1-frame result; repeated identical crash stops retries and preserves logs/dumps. Watcher invalid-config
rejection errors stay visible, and byte/state finally restoration must be checked separately from outer install restore.

Owner resumed the four preserved dirty source files from `2544b12`; no work was discarded. The scoped fix prepares
an input-free RadialMenuView snapshot under owner RunAs before publishing the menu. Capture/ledger cleanup is
registered before callbacks; the new view stays out of the menu list until preparation succeeds, so even callback
cleanup of another menu cannot publish an unprepared radial. The draw-menu array is volatile. Empty radials close
and failed initial callbacks use existing owner failure/ledger cleanup; tolerated segment failures still log once.
No draw-thread callback/native, public API/tuning expansion, counter normalization or threshold change.

Internal selected-slot overload lets WeaponWheelModule choose its held/first-filled slot before snapshot preparation;
sticky state is also assigned first because Centre builds tap/command hints. Existing timing start now precedes
OpenRadial, including snapshot preparation and avoiding a first draw earlier than the recorded start. Engine.Frame
and FirstDrawFrame remain unchanged. StorageWheel computes its first carried slot before opening, and its Centre
builds the right-hand panel without depending on the menu assignment. Both callers handle a closed return after
preparation failure. All other callers audited: SDK self-test (six slots) and autopilot radial sample (eight slots)
use default slot zero, with delegates depending only on owner services/selected argument; public IUi is unchanged.
Later explicit selection commands keep their existing next-update snapshot behavior.

`tools/tests/Run-RadialSnapshotChecks.ps1` compiles only actual UiService/RadialMenuView/MenuInput/ResourceLedger/
StorageWheel and contracts/logic against the existing SDK; no full verifier or engine build. **25/0** in PS7 and
Windows PS5.1, zero compiler warnings/errors (warnings-as-errors). Tests exercise immediate published draw/centre/
highlight before Update, selected-slot clamp, pending confirm/navigation/X/Y/cancel left untouched at open, owner
callback context, draw-only snapshot use, unchanged frame recording, close/empty/fault cleanup, other-owner survival,
once-only segment failure logging, throwing close callback cleanup and actual StorageWheel initial selection/panel/
external-close/owner-stop/failure behavior. Boundary spies model engine dispatch/input/canvas/SHDN, so real module
OnStop/native locks/thread scheduling and the complete WeaponWheelModule remain runtime/production-build gaps.
README documents these limits. The native spy rejects every call and art construction rejects draw context.

Identical final harness against preserved pre-fix `2544b12`: **12 passes / 13 expected regression failures**, including
no drawable initial snapshot and storage first-selection content. This is a negative control, not a fresh game failure.
Logs: `results-local/offline/radial-snapshot-current.log`, `radial-snapshot-current-ps51.log`,
`radial-snapshot-baseline.log`. Existing SDK SHA256:
`61EBF3AB50132A9D8D4E95C626774F93DA123A4233677547256E67A9BEC08C21`.
Initial runner path-resolution error was corrected before these receipts; no game or installed config was touched.
Diff whitespace check PASS. No heavy/full build, package, verifier, install, rollback or game run authorized/performed.

Scoped source/tests/cards/dashboard patch committed as `b3db1bc`; clean lane then integrated pinned main
`5901230` in merge `7ceb350` without conflicts. Merge changed only five documentation files; production/test sources
are byte-identical to the focused receipts, so no redundant compile was run. Reviewed the dashboard auto-merge;
both parent's integration state and this pending radial entry remain. Parent owns the host verifier ordering repair.
Next exact scheduled full IDs (split to retain <=30 game minutes):
`LOOP-package-install,T045-weapon-wheel,T046-trunk-ui,SDK-ui-review` for affected shared radial callers;
`LOOP-package-install,T045-config-watch-reload,T045-config-watch-restart` for the separate watcher lifecycle gates.
Before those batches, the assigned slot must run the required production build/offline checks with parent's corrected
ordering. Always `-AnyBranch -NoPush -Restore -NoManual`, no Quick acceptance. Original <=1-frame gate/budgets unchanged.
**P1 timing acceptance gap remains** until a fresh full run establishes every opening group; hitches still prove no
root cause. Watcher runtime fixtures remain unrun; deliberate rejection errors require independent review.
Owner-only controller/real save-load/safehouse/gunsmith and combined HUD coexistence remain. Ready for bounded source
review, not full T045/T046 acceptance. No main push/merge or other-lane edits. No task marked DONE.

### Scoped watcher fixtures and later-opening coverage (offline, current)

Parent accepted the full-run review and authorized fixture work, not a new build/game slot. Lane entry was clean at
`e7cf0f1`; existing source and all run evidence preserved. No gameplay/UI scheduling implementation was changed.
Fixture/queue/scenario/test/card patch committed as `bcb89892753b48f37cdd306642a0ef38f72a390a` on
`codex/lane-b-validation`; this handoff commit records that exact tested source. Script parse checks also PASS.

Registered `T045-config-watch-reload` and `T045-config-watch-restart` as QUEUED scenarios. New host-side
`wheel-config` actions edit only the flat weaponWheel block of installed arsenal.json using atomic replacement.
They capture exact original bytes/timestamp and wheel running/config-enabled state before mutation. Runner finally
restores bytes first, then uses supported `restart weapon-wheel` or `stop weapon-wheel` plus `modules`/`wheel status`
to restore/verify state and write `wheel-config-restored.txt`. Fail-fast cannot skip cleanup. Game exit or cleanup
command refusal leaves bytes restored but explicitly fails live-state restoration. Outer verify-local restore is
still required. No arbitrary shell/script or native command is added.

- Reload: enable/start, write parseable JSON with invalid `keyboardKey=Enter` and candidate enabled=False, require
  fresh rejection while active enabled=True is retained, then valid disabled/enabled recovery without restart.
  Final change must produce exactly one accepted callback.
- Restart: stop, change disabled, require no accepted/rejected wheel callback in a 2.5 s interval, restart reads
  disabled, restart again, then enable and require exactly one accepted callback. This is bounded evidence, not
  full unload/soak proof.
- Intentional invalid binding also rejects the shared Arsenal validator. ERROR lines are not filtered; reload may
  be NEEDS-REVIEW and requires reviewing deliberate rejection errors against unexpected errors. Both fixtures
  remain unrun in game.

`wheel-latency` pairs actual INFO opening/first-draw events since mark and fails missing/orphan measurements,
negative values or frames>1. Added to initial, all cycle, keyboard hold/tap and later budget-window openings.
The <=1-frame and UI budget thresholds are unchanged. Queue retains every original ID and all other entries;
only the existing wheel pass description is strengthened. Plan regenerated/valid: 81 active checks
(13 pc-offline, 4 probe, 44 scenario, 20 manual).

Instrumentation: EngineHost forwards SHDN Tick and PerFrameDrawing separately; RunFrame increments Engine.Frame
at tick entry. Wheel Open records that counter after publishing the radial; RadialMenuView writes FirstDrawTicks
then volatile FirstDrawFrame at Draw entry after a non-null snapshot, and the wheel logs their delta next update.
This measures engine ticks at draw submission, not visible presentation/animation completion. Ui.Update builds
snapshots before Commands.PumpFileChannel; command-created radials initially have no snapshot. Preserved open
tick3334 and delta2 are consistent with first draw reading tick3336. Callback/snapshot timing remain hypotheses;
the adjacent 328 ms hitch establishes no cause. No counter normalization, SDK/canvas change or actual timing fix.

Focused logs in `results-local/offline/lane-b-wheel-fixtures/`: `focused-tests.log` PS7 **17/0**;
`focused-tests-ps51.log` Windows PS5.1 **17/0**; `runner-regressions.log` existing runner simulations **30/0**.
Checks exercise byte/state restoration (including originally stopped), command refusal, game exit, actual runner
fail-fast finally, callback duplicates/rejection, later-two-frame and missing-draw failures. Initial focused testing
caught PowerShell converting a null File.Replace backup path to empty; explicit NullString fixed that host-call
defect before these passes. `preserved-run-latency-replay.log` confirms the new checker rejects the old run at
frames=2/ms=359; log replay is not a new gameplay run/failure. Queue preservation/validation and diff check PASS.
No full build/suite, game, install, rollback, main push/merge or new slot used in this milestone.

Next exact scheduled IDs: `LOOP-package-install,T045-config-watch-reload,T045-config-watch-restart,T045-weapon-wheel`.
Expect strengthened wheel coverage to reject a repeated two-frame opening; diagnose recorded tick/draw evidence
before changing scheduling. No unchanged text/trunk/historical probe rerun proposed. Independent integration review
remains conditional on unrun fixture/later-opening gates and owner-only checks. No task marked DONE.

### Completed full receipt and merge review

Tested clean build `4330603` includes main `c01f0da` (merge `1383b5b`), isolated watcher cherry-pick `192bd50` and
case-insensitive callback delivery fix `9dc7948`. No code changed after the tested build. On resumption the lane was
clean; no game/verifier/build process was found. This milestone changes documentation only, without starting tests.

Full `20261001-122159-4330603`, 19:21:59.713Z–19:34:00.547Z, command:
`./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -Branch codex/lane-b-validation
-AnyBranch -NoPush -Restore -NoManual -MaxGameMinutes 30
-Only LOOP-package-install,T045-ui-text,T045-weapon-wheel,T046-trunk-ui`.
Queue order was package/wheel/trunk/text. No Quick. Lock wait 0.015 s; completed inside the cap.

| Check | Exact result |
|---|---|
| LOOP-package-install | PASS; package preparation 4 s, install 5 s |
| T045-weapon-wheel | Scenario PASS, 92 steps/0 failed, 12/12 scripted equips, keyboard hold/tap/readback; 0 log errors |
| T046-trunk-ui | Scenario PASS, 133 steps/0 failed, store/take/swap/capacity, 2 identical state-file round trips, controls released; 0 log errors |
| T045-ui-text | PASS, 133 steps/0 failed, 0 log errors |
| Stored summary | PASS=2, NEEDS-REVIEW=2, FAIL=0, CRASH=0, ERROR=0, NOT-RUN=0; wheel/trunk retain their original screenshot-review status |

Wheel launched on attempt 2; this does not resolve intermittent startup failures. Original failed/crashed integration
runs remain failures. The successful full run proves ordinary functional scenarios on this build, not every criterion.

Budget gates unchanged: draw.ui average <=0.5 ms, paired frame avg/p95 <=+10%, p99 <=+15%, >=30 samples, no >=1 s
stall in the measured windows. Values are open versus closed:

| Window | Frames | Average ms | p95 ms | p99 ms | Max ms | draw.ui avg/max ms |
|---|---:|---|---|---|---:|---|
| Wheel | 304 (closed 289) | 34.97 / 36.95 (-5.36%) | 59.5 / 68.9 (-13.64%) | 130.1 / 132.7 (-1.96%) | 663.8 | 0.256 / 1.9 |
| Trunk | 397 (closed 416) | 25.82 / 24.72 (+4.45%) | 35.5 / 41.9 (-15.27%) | 56.9 / 73.4 (-22.48%) | 123.0 | 0.301 / 2.1 |

Both `ui_budget ... pass=True`; no engine_stall or ERROR in the wheel/trunk run logs. Absolute wheel pacing is variable.
**Separate acceptance gap:** first-draw log includes `frames=2 ms=359` at 19:26:50.751Z after wheel_open frame=3334
at 19:26:50.380Z; adjacent wheel_hitch gap_ms=328. The written <=1-frame target is not fully met. Another opening
records frames=1/ms=344. The scenario only asserts frames=[01] for its initial opening, so its PASS does not cover
all later openings. This is an observed timing gap, not an established cause or an animation-duration measurement.
No threshold was weakened and no UI/engine implementation was changed in this review.

All 12 stored JPGs individually viewed at their available 960x540 resolution:
- Wheel: wheel_open_sidearm, wheel_long_gun_1, wheel_long_gun_2, wheel_melee, wheel_empty_slot,
  wheel_keyboard_highlight. Glock/Street Sweeper/AK, empty thrown slot and amber selected sectors match; centre
  names/ammo/finish and footer visible without clipping. Raw Melee_Knife remains cosmetic. Small footer text is
  legible in these downscaled files; they do not establish exact source pixel size or owner-resolution readability.
- Trunk: trunk_ui_open, trunk_ui_after_store, trunk_ui_swap_preview, trunk_ui_after_swap, trunk_ui_full,
  trunk_ui_full_refused. Empty 0/8, stored AK 1/8, swap-in AK/out Street Sweeper, post-swap Street Sweeper list,
  full 4/4 and Trunk full (4) refusal are visible. Labels/list/footer have no observed clipping. These are screenshot
  observations, not controller/real-save-load/combined-D acceptance.

ASI inventory recorded under lock in `summary.json` (full SHA256 and byte sizes retained): aCompleteEditionHook.asi
(20480), ScriptHookDotNet.asi (647168), plugins/000_LVSCE_Dashboard_Bridge.asi (47616),
plugins/GTAIV.EFLC.FusionFix.asi (16022048), plugins/ViolentLiberty.asi (2630656). FusionFix 5.0.1; GTA IV 1.2.0.59,
exe SHA256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`. No ColAccel inventory entry.
No plugin/OS/driver changes. Density config remains OFF; T040 and original budgets preserved.

Restoration: summary and `restore.log` confirm rollback complete from `phase2-20261001-122209`;
installed-build.json independently records that backup at 19:34:00.304Z with blank source identity and dirty=false.
No fresh install was left behind. Slot released by the orchestrator; no new game slot is held or requested by this run.

Evidence: `results-local/20261001-122159-4330603/{summary.md,summary.json,restore.log}` plus each scenario's
`report.md,result.json,run.log` and wheel/trunk JPGs. Required pre-run offline evidence remains
`results-local/offline/lane-b-resumed/{build.log,verify.log,tool-tests.log}`: production build PASS, verifier
**441/0/5 not-run**, PowerShell **236/0**. Focused watcher service tests **8/0** (including reproduced casing failure).
The five NoGame omissions are engine address resolution, native names used by the DLL, core native table,
vehicle body parts (T023), and dismemberment plans/particles (T022); all require game-file inputs and are not passes.

**Merge disposition:** source patch is ready for independent orchestrator integration review with ordinary functional
and budget passes, but not unconditional T045 acceptance. Preserve the first-draw timing gap and remaining lifecycle
gaps. Invalid-config rejection/last-valid retention/recovery and module stop/restart have source/offline evidence only;
the ordinary wheel run does not perturb config or restart the module. Whole sprite-cache unload disposal remains unproven.
Owner-only controller, real game save/load, safehouse/gunsmith, B/D coexistence and resolution judgment remain.
Cards stay NEEDS-PLAYTEST, not DONE. No main merge/push or further subdelegation.

**Next authorized work:** prepare focused config reject/correct + stop/restart fixtures and strengthen first-draw
coverage without weakening the <=1-frame criterion. No existing registered ID directly proves watcher lifecycle.
Proposed new IDs `T045-config-watch-reload` and `T045-config-watch-restart` are NOT registered/executed yet.
After a scoped fix/fixture and explicit slot, affected existing IDs are `LOOP-package-install,T045-weapon-wheel`;
`T045-ui-path` is available if a shared UI timing diagnostic is assigned. Do not repeat unchanged text/trunk probes.

### Prior offline milestone and slot preparation (preserved history)

### Resumed first exclusive build/game slot (2026-10-01)

Owner revoked the hold and assigned one full batch. Clean lane merged docs-only main `c01f0da` as `1383b5b`, without
conflicts; feature testing precedes remaster deployment/polish. Required production build PASS (zero errors;
framework SHA256 `3C04DC932C0983E609708567A847E767361F700B447C5872B29462563BE0F547`),
`tools/verify.ps1 -NoGame` **441/0/5 not-run**, `tools/tests/Run-Tests.ps1` **236/0**.
Logs: `results-local/offline/lane-b-resumed/{build.log,verify.log,tool-tests.log}`.
Next: one committed full `LOOP-package-install,T045-ui-text,T045-weapon-wheel,T046-trunk-ui` batch with
`-AnyBranch -NoPush -Restore -NoManual -MaxGameMinutes 30`, original budgets, then captures/restoration review.
No fresh runtime result yet. The ordinary scenarios do not establish watcher invalid-config recovery or stop/restart.

- Entry was clean at `1744416`; no non-output files had changed in the preceding five minutes. Process inspection
  found no game/verifier/compiler. Fetched origin and fast-forwarded to current main `7058612`, preserving all history.
  Read main's continuation, rules, B prompt and integration review; reviewed the isolated six-file `1f6f501` diff.
  Main advanced during review to docs-only `0189d02`; reviewed and merged that dispatch/roster/completion-gates update
  without conflicts in this handoff commit. Focused checks need no rerun for those documentation-only changes.
- Cherry-picked **only** `1f6f501` as `192bd503a33f4bb12d9d8f0beaf9cf8c05f0cdbb`; no conflicts. The integration
  branch was not merged. Wheel start loads config and registers an owner-scoped shared-file watch; idle ticks no longer
  read/hash arsenal.json. The accepted hash is assigned after successful parse/validation, retaining last-valid config.
- Found and reproduced a shared-path delivery defect: stamps/changed paths use OrdinalIgnoreCase but Poll used
  case-sensitive Array.IndexOf. An owner using differently cased spelling missed the reload. Fixed path matching and
  added one regression check in `9dc7948bb74d3e096928cf423ab3ba2717ae2439`. No other lifecycle or last-valid
  regression was established in source review; runtime rejection/recovery and restart behavior remain unproven here.
- Fresh focused check: actual ConfigService + the original seven temporary-file checks + the new casing check,
  compiled with the current SDK sources, C# 7.3/x86. Before fix: **7 PASS / 1 FAIL**; after fix: **8 PASS / 0 FAIL**.
  Final focused compilation: zero errors/warnings. This is not a full production build or repository verifier run.
  `git diff --check` PASS. Logs and reproducible harness:
  `results-local/offline/lane-b-config-watch/{before-fix.log,after-fix.log,Run-Focused.ps1,WatchRunner.cs}`.
- Historical full `20261001-092349-e470758` summary and wheel/trunk reports rechecked; all 12 captures inspected
  in `results-local/offline/lane-b-config-watch/historical-b-captures.jpg`. Amber selections, centre labels, swap preview
  and full-capacity refusal remain visible; raw Melee_Knife label remains cosmetic. This preserves earlier acceptance,
  not fresh acceptance for this follow-up. Existing failed/crashed integration runs remain failures (see main review).
- No game/verify-local/install/rollback was run. Shared startup `20261001-114509-15cd2d9` is startup evidence only;
  no new restoration receipt is needed for this offline milestone. Density OFF, T040 and budgets preserved.

Changed paths relative to main:

1. `src/LibertyFramework/Arsenal/Ui/WeaponWheelModule.cs`
2. `src/LibertyFramework/Engine/Services/ConfigService.cs`
3. `tools/verify.ps1`
4. `tools/verify/ConfigWatchChecks.cs`
5. `tools/verify/Program.cs`
6. `docs/tasks/T-045-stage1-weapon-wheel.md`
7. `docs/PROJECT_STATE.md`
8. `docs/handoffs/Lane-B-live.md`

### Initial validation proposal (superseded by the completed receipt above)

Heavy/full checks were reported before execution and remain deferred for a staggered slot:
`./tools/build.ps1 -ScriptHookDotNetReference 'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`,
`./tools/verify.ps1 -NoGame`, `./tools/tests/Run-Tests.ps1`. No full-build pass is claimed on this lane tip.

After explicit game-slot assignment, use committed source and full mode:
`./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -Branch codex/lane-b-validation
-AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T045-ui-text,T045-ui-path,T045-weapon-wheel,T046-trunk-ui`.
Split batches if needed for the cap; inspect new captures and restoration. Config reject/correct and module stop/start
need focused runtime observation as part of that slot. No new full wheel acceptance is claimed.

No owner decision blocks this offline patch. Existing Back/Tab binding, melee/thrown, capacity/ammo questions remain;
physical controller and real game save/load remain owner-only, alongside safehouse/gunsmith and HUD coexistence review.
Task stays NEEDS-PLAYTEST. No push or main merge; ready for orchestrator review and scheduled offline/full validation.

## State (2026-10-01, Claude Sonnet)

- T-044 physical loadout: on main (`1e74338`), NEEDS-PLAYTEST, in-game evidence in its card (100 vehicle cycles, 50 death cycles).
- T-045 / T-046: NEEDS-PLAYTEST. Both full scenarios pass in game (run `20261001-001218-7d63be6`, no -Quick, budgets unchanged).
- Commits on this branch after the Codex work: `186ed36` (LRU text cache, creation cap, probes, quick markers), `7d63be6`
  (DevTools-style probes), `aee404b` (cards with root-cause evidence), `ce497ea` (PROJECT_STATE), and the commit that adds this
  file plus the coordinator's text-cache correctness fixes (below). `git log --oneline -8` shows the tip.

## What changed and why

- Shared UI text stall: ScriptHookDotNet `Graphics.DrawText` costs about 15 ms per string per frame at its cheapest (font
  effect none, 17 px) and 75-90 ms with the canvas fonts' default effect, so a menu with several strings made frames take 0.4-0.9 s.
  Rectangles and sprites cost nothing. Cause inside DrawText not established. Fix: `Canvas.Text` renders each string once with
  GDI+ into a cached texture (`engine.json` `uiTextRenderer` = `sprite` default, `shdn` switches back; live `ui-text-renderer`).
  Cache: least recently used, 600 entries (`uiTextCacheEntries`), at most 8 new textures per frame (`uiTextNewSpritesPerFrame`),
  `ui-text-stats` prints count and estimated bytes. Text textures belong to the canvas (engine lifetime), not to a module.
- Wheel (`Arsenal/Ui/WeaponWheelModule.cs`, `Logic/WeaponWheelLogic.cs`, `WeaponWheelConfig.cs`, block `weaponWheel` of
  `config/arsenal.json`): hold the Back button or Tab to open, release equips; a tap keeps it open (arrows/stick + A/Enter);
  equip is read back next frame (`weapon_wheel_equipped ... match=True`).
- Trunk (`Arsenal/Ui/StorageWheel.cs`, `Logic/TrunkCapacityRules.cs`, `ArsenalPolicy.DisplacedOnTake`, block `trunkCapacity`):
  carried slots as a radial plus a container list with capacity; store refused when full; a take swaps out the carried weapon
  of the same category, else the least recently used of a full group; centre names the swap before it is made.
- Coordinator correction (uncommitted Codex edits reviewed and kept): generated texture keys are case-sensitive (file paths
  are normalised separately), text cache key uses exact font size and pixel width, truncation measures candidates plus the
  ellipsis (`UiTextLogic.FitText`) and keeps text-element boundaries. Offline verifier 1012/0, PowerShell tests 219/0.
  NOT run in game yet.

## Runs

| Run | Checks | Result and real cause |
|---|---|---|
| `20260930-213920-ec5e244` | wheel, trunk, ui-text (full) | ui-text PASS. Wheel CRASH: the game exited after an 11-14 s stall in the holsters and wheel config-file polling (`tick.holsters` max 11403 ms, `engine_stall` in `module.holsters`); other lanes' package builds were running on the same disk, so disk contention is likely, not proven. Trunk FAIL on `ui-budget` only: p95 47.2 ms against baseline 42.7 (+10.5% over a +10% gate), every functional step passed; windows were then lengthened to 10 s, thresholds unchanged. |
| `20260930-224612-186ed36` | wheel, trunk, ui-text (quick) | all pass. |
| `20261001-000645` (7d63be6) | ui-text (quick) | PASS; probe table in the T-045 card. |
| `20261001-001218-7d63be6` | wheel, trunk (full) | both passed (NEEDS-REVIEW for screenshots only): wheel 12/12 equips, window avg 28.8 vs 31.1 ms closed; trunk store/take/swap/capacity/2 round trips, window avg 30.3 vs 28.7, p95 +7.4%, p99 +11.6%. Codex notes ColAccel was installed during this batch (07:11-08:03 UTC), so isolated performance numbers need a fresh controlled run. |
| `20261001-023630-4ab6fbd` | wheel, trunk, ui-text (full) | NO EVIDENCE. Package installed; the wheel scenario had started when the coordinator paused the lane at about 02:41 and restored the install (`coordinator-pause-restore.log`: rollback waiting on the lock held by the main checkout). No check result, crash or failure was produced by the game. The text-cache fixes (`4ab6fbd`) are therefore still unproven in game. |

## Unproven

- Physical controller input (Back, right stick, A/B) and whether Back or Tab collides with a vanilla action on foot.
- A real game save and load (only the saved state file round trip is automated); safehouse stash and gunsmith through the new interface.
- Lane D HUD coexistence (storage list is placed below the top-right band; no combined build tested).
- The coordinator text-cache fixes in game; cache churn with changing strings (Lane D ammo text creates one texture per distinct
  string; per-character digit sprites would avoid it and are not built).
- Owner judgement of screenshots (look, clipping, text size at the owner's resolution).

## Next step

1. Commit is done; run one batch with `./tools/verify-local.ps1 -GameDirectory "C:\Games\Grand Theft Auto IV\GTAIV" -Branch codex/lane-b-validation -AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T045-weapon-wheel,T046-trunk-ui,T045-ui-text` to prove the coordinator fixes (add `-Quick -StopOnFailure` while iterating; the final run is full).
2. Hand to the orchestrator for integration with Lane D and R visibility ownership; the owner playtests the pad, save/load and safehouse items.

## Open questions for the owner

- Are Back (pad) and Tab (keyboard) acceptable wheel bindings? (config `weaponWheel`)
- Trunk sizes are proposals (default 8, sports 4, utility 16, stash unlimited) and ammo caps are proposals (handgun 150, shotgun 60, SMG 240, rifle 240, sniper 40, heavy 12).
- Melee and thrown weapons keep their current rules; confirm.

## Coordinator completion — October 1 (lanes paused)

Shared scheduling/fail-fast/interruption-restoration/ASI evidence/compiler-cache fixes and the Sol kit are merged locally.
Integrated offline verifier: 433 passed, 0 failed, 5 notrun; queue/plan validation PASS. See
Codex-Lane-B-Live-2026-10-01.md and sol/Orchestrator-2026-10-01.md for reviewed source and next work.
The separate Claude resume's 20261001-023630-4ab6fbd batch was interrupted after the owner paused all lanes.
No new wheel/trunk acceptance result was collected. Coordinator restored exact backup chain to phase2-20261001-023918;
44 affected files/actions matched saved originals or expected absence. No game/launcher/verifier remains.
All feature integration, physical controller/save-load/B-D coexistence and owner judgment remain pending. No push.

## Resumed 2026-10-01 (Claude) - READY FOR MERGE (run 20261001-092349-e470758)

- Merged origin/main (`868368b`, log writes and config timestamp checks off the game thread) into this branch (`e986a88`). Offline on the merged tip: verifier 1012/0, PowerShell tests 224/0, `checks.py plan` ok, `artq.py validate` ok (12 requests, 0 problems).
- Orchestrator held game runs: Windows has no audio output device, GTA IV cannot start (GAME-UNAVAILABLE). No game run was attempted. When the orchestrator says audio is back, run once, full (no -Quick):
  `./tools/verify-local.ps1 -GameDirectory "C:\Games\Grand Theft Auto IV\GTAIV" -Branch codex/lane-b-validation -AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T045-ui-text,T045-weapon-wheel,T046-trunk-ui`
  Look at every screenshot. On a failure fix with `-Quick -StopOnFailure`, then rerun full. Then write "READY FOR MERGE" plus the run id here and commit. Do not merge, do not touch ConfigService polling.
- Cards T-045/T-046: human test steps rewritten (controller, real save/load, safehouse, owner-only) and an "Owner questions" section added (Back/Tab, trunk sizes 8/4/16/unlimited, ammo caps, melee/thrown). Defaults unchanged.

## READY FOR MERGE - run `20261001-092349-e470758` (2026-10-01, commit e470758, full, no -Quick, audio back, no ColAccel noted)

- LOOP-package-install PASS; T045-ui-text PASS (133 steps, 0 failed, 0 log errors); T045-weapon-wheel and T046-trunk-ui passed every step (NEEDS-REVIEW only for screenshots, all 12 viewed by Claude: text legible, highlight and centre text correct, trunk list/capacity/"Full" and "Trunk full (4)" and swap preview correct; cosmetic: the knife shows its raw name `Melee_Knife`).
- Budgets (unchanged thresholds): wheel window frames=459 avg 22.75 ms p95 34.4 p99 36.2 max 76.1, draw.ui 0.217 ms avg / 0.7 max, 0 stalls over 1 s; trunk window frames=404 avg 25.45 p95 37.5 p99 46.0 max 81.8, draw.ui 0.303 ms avg / 1.2 max, 0 stalls over 1 s. Wheel open to first draw: 0-1 frames (15-47 ms wall, one 47 ms on the very first open). No engine_stall, no [ERROR] lines in either scenario log.
- This run includes main `868368b` (log writes and config checks off the game thread): no 11-14 s stall, no crash.
- Still owner-only: physical controller (Back/stick/A/B/X/LB/RB), a real game save and load, safehouse stash and gunsmith, Lane D HUD/radar coexistence, judgement at the owner's resolution. Exact steps are in the T-045 and T-046 cards, with the owner questions (Back/Tab, trunk sizes 8/4/16/unlimited, ammo caps, melee/thrown).
- Install restored after the run. Not merged, not pushed. Next: orchestrator integrates with Lane D and R.
