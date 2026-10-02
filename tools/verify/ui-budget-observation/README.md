# Paired UI budget observation regression

Run `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/tests/Run-UiBudgetObservationChecks.ps1`.
Uses the existing SDK DLL and pinned Roslyn; compiles only PerfService, CostMeter, UiBudgetLogic, RuntimeLog,
ResourceLedger, TrunkSequence and TrunkTimings plus test boundaries. Outputs/log files are in ignored
`results-local/offline/lane-b-ui-budget-observation`. No full build, verifier, game files, package or install.

The harness supplies frame samples to the actual histogram using reflection, and cost samples through actual
CostMeter.Add. It exercises the actual asynchronous RuntimeLog and reads its temporary file with shared-read flags.
The actual TrunkSequence observer reads managed fields from an IChoreography fixture. Engine/SHDN boundaries throw
if observation attempts game queries; no actual choreography scheduling/native implementation is exercised.
Actual owner ResourceLedger cleanup removes the observer, including replacement-owner protection.

Checks prove exact closed/open command-cost separation, unchanged resets and original gate evaluation, independent
periodic logger-reader counters, boundary state transitions, no observation reads/logs during sample collection,
strict p95 failure with compliant mean/draw, and logged observer failure without losing metric results.
They do not establish production Arsenal registration compilation, live window metrics, performance neutrality or
the cause of the preserved trunk failure. Required production checks and one full trunk run need a scheduled slot.

Observation format is additive to existing begin/baseline/check output. Each begin assigns a diagnostic ui_window
(not an engine frame counter) and samples managed state. Baseline preserves its already-collected command costs
beside frame statistics. Check references baseline_window and adds matching start/end engine frame/tick values and
state strings. Engine.Frame and all original metric reset calls/thresholds remain unchanged. State is observed at
boundaries only; an intermediate transition that reverses before the next command cannot be reconstructed.
No extra runtime native queries, per-frame logging, draw work, trace, tuning or module switches are added.
