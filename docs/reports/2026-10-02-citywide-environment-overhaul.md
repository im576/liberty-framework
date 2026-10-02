# Citywide environment overhaul — initial integrated candidate

Owner authorized implementation across the whole city on 2026-10-02, replacing the art-only hold and station-first sequence. One owner authored the candidate; no agents or new lanes were dispatched. This is the beginning of the overhaul, not a claim that the complete visual remaster is finished.

## Implemented

All eight weather types and eleven time samples are generated together from pristine FusionFix tables. The profile preserves more natural color, neutralizes overcast/rain daylight sky colors, retains readable ambient night light, and uses restrained bloom/contrast. Dusk includes 7PM, matching the approved 19:00 reference rather than treating it as ordinary daytime. Existing renderer, gameplay, weather scheduling and density OFF stay intact.

Optional validated sky/cloud RGB fields were added to the existing generator using the installed source header's X360 column layout. No unrelated exposure, reflection, geometry or shader field is changed. This controls color and lighting; it does not create the AI reference's cloud shapes, new materials or local light fixtures.

The installer now holds the shared game lock, refuses a running game, requires explicit pristine source files, stages generated output, records hashes and the exact previous installed files, and supports exact `-Rollback`. Its older `-Restore` explicitly returns to pristine FusionFix rather than the owner's prior tuned state.

## Evidence

- Mood generator build: zero errors/warnings with warnings treated as errors.
- Actual local FusionFix-source checks: 88/88 main weather/time rows covered; unrelated columns retained; extended changes isolated to fog density; deterministic output; out-of-range sky RGB rejected.
- Isolated installer fixture: staged installation and exact two-file rollback hashes verified. A malformed source row was rejected without modifying fixture installed files. The fixture mocked the process guard only inside the fixture test; no running game or production files were touched.
- Repository offline verifier: **441 passed, 0 failed, 5 not run**. Log: `results-local/citywide-overhaul/offline-verify.log`. Not-run game checks are not passes.
- Check queue validates: 81 checks; T058-citywide-environment queued, verification plan regenerated.
- Broader PowerShell tooling: **249 passed, 1 failed**. The failure is in the unchanged `StartupReadiness.Tests.ps1` fixture: missing `near-full-sleeps.jsonl` at line 119. A focused PowerShell 7 rerun also fails (11 passed, 1 failed). Retain this unresolved tooling failure; it is not a visual-overhaul pass and no unrelated startup repair is included.
- Candidate tables saved in `results-local/citywide-overhaul/timecyc.dat` and `timecycext.dat`.
- Continuation installer fixture: install → pristine restore → rollback to candidate → rollback to original all preserve the corresponding exact hashes. PowerShell source syntax checks pass. Focused autopilot simulation reports **30 passed, 0 failed**; the launching shell observed exit 1 from the last intended negative child case, so keep both the assertion summary and exit evidence. Log: `results-local/citywide-overhaul/scenario-tests.log`.

## Runtime status

GTA IV was already running as PID 17888, start 2026-10-01 22:08 local. It did not match the recorded autopilot launch PID/start identity. Installation correctly refused; the owner has been asked whether it may be closed/restarted. No candidate was installed in the production game, and no in-game capture or visual/performance acceptance is claimed.

The ready single runtime batch samples station, Star Junction, Hove Beach shops and East Hook docks under cloudy day, clear day, rain day, dusk and cloudy night, with final camera/owned-prop cleanup. Compare original + AI target (where approved) + actual on one page after execution. Further material/local-light differences, moving performance and interior/mission exposure remain to be established from actual evidence.

Continuation: `tools/mood/Run-CitywideReview.ps1` coordinates installation, bounded startup, the twenty-frame batch and cleanup under one shared lock. It refuses a running game unless the human-authorized `-RestartRunningGame` flag is passed. Runtime failure automatically stops the test process and rolls back the exact prior appearance. The final teleport is restored from the runtime position receipt rather than a hard-coded historical player position. Successful automated capture does not establish visual acceptance.

The scenario runner requires the dedicated installed-mood receipt, matching configuration hash and matching installed file hashes before it permits this capture scenario. Running the queue against an unchanged gameplay baseline cannot silently produce a false citywide-overhaul pass.
