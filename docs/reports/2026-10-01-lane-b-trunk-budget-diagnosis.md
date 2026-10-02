# B trunk percentile failure: bounded offline diagnosis

Candidate2297a17 held unmerged after full20261001-202311; no retry. D holds current heavy/game slot.
This is log/source analysis, not fresh runtime evidence or an implementation repair.

## Original paired measurement

Closed begin03:30:27.266Z to baseline03:30:37.684Z:394 frames, avg26.44/p9540.7/p9968.0/max186.3ms.
Open begin03:30:46.676Z to check03:30:57.260Z:379 frames, avg27.92/p9546.4/p9999.7/max141.7ms.
Original avg/p95 ratio1.10, p99 ratio1.15, draw average0.5ms and minimum30 frames remain unchanged.
Avg+5.60% passes; p95+14.00% fails first; p99+46.62% independently exceeds its gate. Neither window records a>=1s
stall. More slow frames despite lower max: over33ms39/394 closed vs51/379 open; over100ms2 vs3.
Nothing here proves random hitches or the initial snapshot caused the percentile tail.

The fail-fast scenario ends at check41; store/take/swap/capacity/round-trip/close flows NOT-RUN. Initial snapshot
capture is readable but cannot substitute for those actions or the failed budget. Full receipt/captures:
[Lane-B-live](../handoffs/Lane-B-live.md), ignored results-local/20261001-202311-2297a17/{summary.json,T046-trunk-ui}.

## Choreography is active browsing, not unfinished opening

Trunk begins03:30:38.354Z. Arsenal emits arsenal_storage_open at Begin, before wheel-ready, so that log alone does
not prove the radial exists. At03:30:44.101Z storage status confirms wheel open. Source ArsenalCore opens wheel only
when TrunkSequence.WheelReady (Active && browsing && !closeRequested). Browsing becomes true AFTER turn/open steps,
then starts idle. Thus wheel-ready status, capture and measured open window occur in browsing. No complete/cancel
event occurs before failure; completion is not expected until the player requests close.

TrunkSequence.Begin deliberately LoopUntil(closeRequested) over WaitUntil(closeRequested || handleRequested ||
StepDone, browseStepTimeout3000ms) and Next. Choreography resumes through Scheduler every engine tick. While idle:
CastAlive checks Peds.Exists and IsDead (supported natives) each tick, then WaitUntil polls StepDone; after configured
idleMin300ms, Animation.IsPlaying queries SHDN cached ped/animation-set wrapper (native fallback only if no wrapper).
When the idle clip ends, Next calls StepDone again and starts idle again; a timeout can cause Next to recheck without
restarting a still-playing clip. Nested loop enumerators are created at transitions, not every wait tick. Trunk.Update
also checks vehicle existence during Arsenal updates. These are pre-existing continuous owner-tick game calls.

Scheduler CostMeter encloses all coroutines, so no named breakdown of cast checks/animation queries exists in this
receipt. Scheduler.Run moves waiting choreography each tick; actual module context restored in finally. Nothing was
added to scheduler/choreography by snapshot patch. Do not remove safety queries, stop animation/modules or assert a
duplicate/unfinished-coroutine bug without evidence.

## Actual open-window comparison to historical passing trunk

Same reported scopes from preserved full122159-4330603 vs fresh202311-2297a17:

| Scope | Prior PASS avg/max/count ms | Current FAIL avg/max/count ms |
|---|---|---|
| engine.frame | 2.385/6.3/397 | 2.512/21.0/379 |
| engine.scheduler | 0.348/1.7/397 | 0.362/2.1/379 |
| draw.ui | 0.301/2.1/397 | 0.345/5.8/379 |
| ui.snapshot | 0.137/1.4/397 | 0.137/1.7/379 |
| module.arsenal | 0.388/2.2/261 | 0.421/2.2/261 |
| ar.storage | 0.186/1.5/261 | 0.205/2.1/261 |
| tick.holsters | 0.073/0.6/146 | 0.209/19.0/146 |
| ho.show | 0.011/0.0/146 | 0.012/0.0/146 |
| combat.sample | 1.905/2.7/25 | 2.078/3.5/25 |
| gp.freeaim | 0.598/2.1/397 | 0.590/2.3/379 |

Historical PASS confirms coroutines1 during browsing (19:28:12 engine_status), choreography_complete only at19:28:38
after closing. The roughly0.001 quiet scheduler scope is an earlier no-coroutine logging interval, not comparable to
either open window. Current scheduler is only0.014ms higher average than historical open, not a new0.36ms regression.
Snapshot avg unchanged; initial PrepareSnapshot happens only at open, BEFORE open budget begin resets measurements.
Normal per-tick snapshot is same extracted logic, so no continuously-added snapshot operation identified.

Storage centre recalculates carried/selected/swap/hints and BuildPanel on every update, allocating arrays and formatting
strings; current fixture has empty stored list and only3 carried entries. Segment delegates scan carried list; icons
use UiService's per-weapon cache. RadialArt queries cached texture handles, with generation only on cache miss; same
5-segment art already used during wheel. Draw uses snapshots; Canvas submission/text-cache path unchanged. This is real
work to measure if necessary, but neither unchanged0.137 snapshot avg nor compliant0.345 draw avg proves tail safety.

Holster19ms outlier is outside ho.show (max rounds to0.0), so cannot attribute it to repeated prop creation from these
scopes. Tick body has player/ped visibility/held/vehicle reads, config timestamp check, outfit refresh and carried read
before ho.show. Log does not resolve which call or wait produced19ms. One peak cannot alone explain p95 across379
frames; higher tail could have several causes. Named costs overlap/nest, and submitting drawing does not measure GPU
completion; do not add nested averages as independent contributions or equate engine.frame time with frame interval.

## Limits and next bounded proposal (not authorized/executed)

PerfService.UiBudgetCommand baseline resets AND discards CostReader.Command report, logging only frame statistics.
Periodic performance_scripts uses a separate logger reader over a broader interval, not the exact closed baseline.
Therefore there is no exact paired baseline cost attribution to reconstruct. Histogram percentiles and aggregate
cost maxima have no per-frame timestamps/native latencies; logs cannot prove temporal correlation. Memory/status
differences between runs are observations, not source attribution or justification to change OS/settings.

Next offline work, if parent assigns it: design a small observational fixture recording exact closed/open command-cost
reports and explicit supported engine/storage status around window boundaries while preserving existing ui-budget
commands/thresholds/windows. If finer instrumentation is needed, propose bounded owner-tick choreography/cast/query
and holster pre-show scopes with frame-aligned slow-event samples for review BEFORE implementation; avoid unconditional
per-frame disk logs that alter the measurement. No general Engine.Frame change, thresholds, arbitrary warm-up,
module disabling or unrelated engine/SDK/canvas repair. The evidence must distinguish active browsing from completed
close and correlate slow intervals with work, not assume opening delay or idling means failure.

No further build/test/game/capture or verifier invocation executed for this diagnosis. Future
LOOP-package-install,T046-trunk-ui and separate watcher IDs require new parent assignment/slot. Candidate remains held.
