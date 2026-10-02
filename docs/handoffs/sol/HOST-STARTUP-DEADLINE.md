# T-029 host startup deadline candidate — October 1, 2026

Isolated host follow-up on lane C; parent cherry-picks the commit carrying this receipt, not the C branch.
Parent source `689bac5` was safely merged as `3423295` before this patch. No B/D/R source was integrated.
Gameplay changes and control/active receipts remain in earlier C commits. No game, install, production build,
NoGame verification or full tooling suite was run for this host patch. Parent reviews and tests after D's slot.

## Bounded behavior

- VerifyLocal passes an absolute UTC deadline derived from its existing scenario timeout clipped by the remaining
  game allowance. The outer cap is unchanged. Every built-in launch attempt shares that deadline.
- Run-Scenario reserves the unchanged **12 s full / 2 s quick** settling period plus **5 s serialization** inside
  that allowance. It checks remaining time again after readiness returns and fails instead of shortening settling
  or starting a sleep that would consume the serialization reserve. Omitted deadlines preserve standalone behavior.
- The actual production Start-GameReady loop is extracted into a dot-sourced file for stubbed tests. Run-Scenario
  still requests **six attempts**; the directly callable helper retains its existing default **five**, 240 s boot,
  90 s unseen-process and 20 s continued-absence behavior. No retry count, mode or gameplay budget increased.
- Append-only telemetry goes explicitly to **parent results/<check-id>-startup.jsonl**. A prior journal is preserved
  under its original name; another invocation gets a unique name. UTC/attempt, launch request, observed PID/start UTC,
  fresh engine_booted line, observed dialog title and actual exit/timeout/error reasons are flushed during startup.
  Process absence is not labelled a guessed crash cause. Nonfatal dialogs are observed without dismissal.
- A late blocking log read retains any observed boot but cannot report readiness after the shared deadline.
  Failed startup uses the last actual attempt number, not the exhausted loop's incremented counter.
- Graceful failure writes ERROR/result.json with startup observations and a JSONL snapshot. Parent evidence attaches
  an existing journal even when a hard kill produces neither AUTOPILOT_RESULT nor a telemetry stdout marker. Parent
  storage does not depend on `_runs`, result parsing or child finalization. JSONL uses existing results redaction.
- Mutex, installed-build checks, owned-process cleanup and restoration ownership remain unchanged. Existing
  communication/readiness errors remain errors; a secondary telemetry failure is logged without replacing them.

## Focused evidence

Windows PowerShell 5.1 commands, from the assigned lane:

```powershell
powershell -NoProfile -File tools/tests/Run-Tests.ps1 -Filter StartupReadiness
powershell -NoProfile -File tools/tests/Run-Tests.ps1 -Filter AutopilotLogic
git diff --check
```

StartupReadiness **18 passed / 0 failed**; AutopilotLogic **58 passed / 0 failed**; diff whitespace check clean.
Startup tests execute the actual production loop with stubbed clock/process/launch dependencies and the actual
Run-Scenario report path. Full and quick boot at virtual +53 s, two seconds before startup's +55 s deadline;
both retain their entire settling duration and at least five seconds before the outer deadline. Other cases cover
clipped relaunch, late blocking read, fatal/nonfatal dialogs, audio/launch failures and observed process absence.
The parent hard-kill case runs real Invoke-ScenarioCheck/Invoke-ChildProcess against a disposable PowerShell child,
then verifies ERROR and the preserved journal without stdout markers or a child result. The old SimulatedGame helper
signature also works with the new optional runner arguments. No game/Steam/native/UI/audio-host calls in these tests.
Early test iterations exposed fixture errors (a boot marker in the absent-process fixture, a removed module export,
and a zero-command fixture correctly returning ERROR). Those were repaired; no runtime failure was reclassified.

## Limits and next review

Graceful completion is best effort: audio preflight, Stop-Game, process/log access, module loading and result I/O
can block past the deadline. Five seconds is a reserve, not a guaranteed completion time. Already-flushed parent
telemetry survives outer termination; operations before the first write or an interrupted final write can still
leave absent/incomplete evidence. No additional allowance or arbitrary modal dismissal is introduced.

Parent should review/cherry-pick this isolated commit, then run appropriate host/full tooling checks sequentially.
Only after a new slot assignment, proposed startup diagnostic IDs are **LOOP-package-install**, then
**T057-audio-output-smoke**, with the same plugins/config and timeout evidence. No unchanged active/effects/dismember
retry is queued by this patch. C's control164156 remains NEEDS-REVIEW (43/0, one shader ERROR); active165752 remains
ERROR (300 s pre-engine readiness timeout, no valid gameplay comparison). Original 4 ms peak and zero owned
lights/loops/effects cleanup gates, enabled-state restoration and full runtime acceptance remain unproven.

Proposed command from C, after parent review/integration and an explicit new slot (not executed):

```powershell
powershell -NoProfile -Command "& './tools/verify-local.ps1' -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -AnyBranch -NoPush -Restore -NoManual -Quick -StopOnFailure -ScenarioTimeoutMinutes 5 -MaxGameMinutes 8 -Only @('LOOP-package-install','T057-audio-output-smoke')"
```
