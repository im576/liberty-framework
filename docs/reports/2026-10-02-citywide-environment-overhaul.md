# Citywide environment overhaul — initial integrated candidate

Owner authorized implementation across the whole city on 2026-10-02, replacing the art-only hold and station-first sequence. One owner authored the candidate; no agents or new lanes were dispatched. This is the beginning of the overhaul, not a claim that the complete visual remaster is finished.

## Implemented

All eight weather types and eleven time samples are generated together from pristine FusionFix tables. The profile targets natural color, neutral overcast/rain daylight and readable ambient night light, with restrained bloom/contrast. Dusk includes 7PM, matching the approved 19:00 reference rather than treating it as ordinary daytime. Existing renderer, gameplay, weather scheduling and density OFF stay intact. Actual evidence, rather than these intended effects, decides acceptance.

Optional validated ambient/directional/color-correction and sky/cloud RGB fields use FusionFix 5.0.1's parser at upstream commit 619f52d. The initial header-based mapping was incorrect and is superseded by the correction below. The corrected generator preserves grain, fog alpha, exposure and reflection controls. This controls color and lighting; it does not create the AI reference's cloud shapes, new materials or local light fixtures.

The installer now holds the shared game lock, refuses a running game, requires explicit pristine source files, stages generated output, records hashes and the exact previous installed files, and supports exact `-Rollback`. Its older `-Restore` explicitly returns to pristine FusionFix rather than the owner's prior tuned state.

## Evidence

### Final v3 evidence and installed state

`run-20261002-06/citywide-environment-20261002-040338` captured every scene with the exact v3 configuration/file receipts: **171 steps, 0 failed steps, 20 screenshots, game alive, NEEDS-REVIEW**. One ERROR inside the scenario window is `engine_stall_dump written=True` at 11:03:41.683Z. The originating engine.commands stall is at 11:03:33.555Z (5008 ms), during pre-capture state reads; the dump finished after the scenario began. This does not establish its cause or clear performance acceptance. No log error was removed or reclassified to PASS.

V3 visual review: overcast/rain daylight is neutral and much closer to the references; clear/dusk remain distinct. Night sky/cloud detail and unlit roads are more readable than v2, with existing sign colors retained. Remaining target gaps are significant: station night needs more brightness/warm local illumination, Star Junction lacks warm facade depth, sky cloud shapes differ and wet/grimy material detail requires actual asset work. The global grade is not the complete remaster. All twenty frames were inspected together; no alternate AI targets were invented for shops/docks.

The final single page has 50 images and 20 scene rows, original/approved target/actual together (two columns where no approved AI target exists). Desktop and 320/360 px QA: all images load, no overflow, no browser errors; fragment under 1 MB. `review-manifest-v03.json` records config/generator/installed-file/screenshot/comparison hashes. V1/v2 source and comparisons are archived.

Settled cleanup after v3: health 100; position 892.384 -499.885 19.430, within 0.1 m of the recorded original; menus closed/control enabled; no owned test entities; carried weapons and stored count 650 unchanged within the run. Startup increased stored counts across separate sessions (641 → 644 → 647 → 650), not during the photo scenarios; this existing reconciliation behavior is retained as context, not repaired or claimed unchanged across restarts.

`final-ready-v03` then installed v3 for one short readiness check. Its single launch process exited before engine load (GAME-UNAVAILABLE); it rolled back. No further automatic launches were made. Two older candidate receipts were subsequently rolled back to the exact original owner appearance. `final-v03-baseline-restoration.json` verifies both original hashes; game closed, test lock released, no candidate currently installed. Candidate v3 remains prepared and reviewable, with startup/stability and full visual acceptance open.

### Successful capture and renderer correction (latest)

After the owner's manual-launch check, candidate v1 booted at 09:58:53.763Z in `run-20261002-03`: **171 steps passed, 0 failed, 0 log errors, 20 actual screenshots**. Final settled cleanup verifies health 100, the recorded original player position, closed menus and no owned autopilot entities. Carried found weapons were retained; the capture contains no weapon give/strip/storage action. The four state files and manifest were preserved before the batch.

Visual review identified excess grain and cyan/green bias. The initial candidate incorrectly followed the legacy timecyc header for sky RGB at 9–11; the installed FusionFix parser actually uses column 10 for film grain and 11 for fog alpha. Candidate v2 preserves 9–11 and writes normalized sky/horizon/cloud fields at 64–75, 81–83 and 99–101, with validated ambient/directional/color-correction RGB and cloud alpha. Source reference: https://raw.githubusercontent.com/ThirteenAG/GTAIV.EFLC.FusionFix/619f52d/source/timecyc.ixx (saved locally for provenance). This is a meaningful mapping error in v1, not owner acceptance; the corrected candidate requires fresh actual captures. The inherited JSON name sunScale remains compatible but scales directional specular at column 61.

V2 generation passes all 88 rows, unrelated-column and grain/fog-alpha preservation, normalized RGB bounds, deterministic generation and invalid RGB/cloud-alpha rejection. The wrapper now waits for asynchronous teleport cleanup and checks settled position instead of trusting the immediate reply. The v1 board and configuration remain archived; the single comparison page will use v2 evidence when available.

### Earlier offline checks

`run-20261002-04` booted candidate v2 but stopped before any screenshot: the first teleport logged a 16.2 s scene load/stall and the host consumed an empty reply during file publication. Cleanup restored the recorded player position and exact rollback completed. The host reader now waits for all expected, newline-terminated replies, excluding blank/comment commands. A delayed empty/partial/unterminated publication fixture passes (1/0); focused autopilot logic tests pass (58/0). The engine stall is preserved as failed evidence; the host fix does not suppress it or change gameplay/watchdog code.

`run-20261002-05` booted candidate v2 at 10:53:14.843Z and passed **171 steps, 0 failed, 0 log errors, 20 captures**, with settled cleanup and identical carried/stored Arsenal counts within the run. Review found grey/rain daylight much closer, but night sky and unlit areas too dark. Candidate v3 raises night sky/horizon/cloud RGB, ambient RGB and color-correction RGB together. Day/dusk tuning is unchanged. V2 configuration and comparison are archived; v3 requires fresh full-batch evidence.

The single-page review contains 50 images across 20 scene rows: original + approved AI target + actual for station/Star Junction, original + actual for shops/docks where no AI target is approved. V2 page QA passed at desktop and 320/360 px with no missing images, overflow or browser errors. A supported FusionFix graphics-menu reload was considered to avoid another launch, but Windows state capture failed twice (`FrameArrived timed out`, then `window capture timed out`). No blind menu input or live file installation was performed; the authorized restart wrapper remains the verification path.

- Mood generator build: zero errors/warnings with warnings treated as errors.
- Actual local FusionFix-source checks: 88/88 main weather/time rows covered; unrelated columns retained; extended changes isolated to fog density; deterministic output; out-of-range sky RGB rejected.
- Isolated installer fixture: staged installation and exact two-file rollback hashes verified. A malformed source row was rejected without modifying fixture installed files. The fixture mocked the process guard only inside the fixture test; no running game or production files were touched.
- Repository offline verifier: **441 passed, 0 failed, 5 not run**. Log: `results-local/citywide-overhaul/offline-verify.log`. Not-run game checks are not passes.
- Check queue validates: 81 checks; T058-citywide-environment queued, verification plan regenerated.
- Broader PowerShell tooling: **249 passed, 1 failed**. The failure is in the unchanged `StartupReadiness.Tests.ps1` fixture: missing `near-full-sleeps.jsonl` at line 119. A focused PowerShell 7 rerun also fails (11 passed, 1 failed). Retain this unresolved tooling failure; it is not a visual-overhaul pass and no unrelated startup repair is included.
- Candidate tables saved in `results-local/citywide-overhaul/timecyc.dat` and `timecycext.dat`.
- Continuation installer fixture: install → pristine restore → rollback to candidate → rollback to original all preserve the corresponding exact hashes. PowerShell source syntax checks pass. Focused autopilot simulation reports **30 passed, 0 failed**; the launching shell observed exit 1 from the last intended negative child case, so keep both the assertion summary and exit evidence. Log: `results-local/citywide-overhaul/scenario-tests.log`.

## Initial runtime status (before owner restart authorization)

GTA IV was already running as PID 17888, start 2026-10-01 22:08 local. It did not match the recorded autopilot launch PID/start identity. Installation correctly refused; the owner has been asked whether it may be closed/restarted. No candidate was installed in the production game, and no in-game capture or visual/performance acceptance is claimed.

The ready single runtime batch samples station, Star Junction, Hove Beach shops and East Hook docks under cloudy day, clear day, rain day, dusk and cloudy night, with final camera/owned-prop cleanup. Compare original + AI target (where approved) + actual on one page after execution. Further material/local-light differences, moving performance and interior/mission exposure remain to be established from actual evidence.

Continuation: `tools/mood/Run-CitywideReview.ps1` coordinates installation, bounded startup, the twenty-frame batch and cleanup under one shared lock. It refuses a running game unless the human-authorized `-RestartRunningGame` flag is passed. Runtime failure automatically stops the test process and rolls back the exact prior appearance. The final teleport is restored from the runtime position receipt rather than a hard-coded historical player position. Successful automated capture does not establish visual acceptance.

The scenario runner requires the dedicated installed-mood receipt, matching configuration hash and matching installed file hashes before it permits this capture scenario. Running the queue against an unchanged gameplay baseline cannot silently produce a false citywide-overhaul pass.

## Owner-authorized runtime attempts

The owner approved restarting the game. `run-20261002-01` installed the verified candidate; its three Steam launches exited before `engine_booted` and automatic exact rollback completed. A one-attempt baseline startup control then reached gameplay at 09:30:49Z, with a FusionFix `Error building shader!` warning acknowledged by the existing engine guard. The baseline control proves gameplay availability for that launch, not candidate appearance or long-term stability.

`run-20261002-02` made one controlled candidate launch after that successful baseline; it exited before gameplay and rolled back. `run-20261002-local` tried the verified executable from its local game directory once and also exited; rollback succeeded. The local diagnostic's `boot.txt` is stale baseline history, not a valid new-launch receipt: Wait-LogLine ran without a new session boundary. Its command failed on process exit. Preserve this failed diagnostic and do not claim candidate readiness from it.

Windows events report access violation 0xc0000005 with an unknown module. The preserved WER loaded-module list includes DXVK, DINPUT8 and MTLX, but no FusionFix; no new ScriptHook/engine session was recorded for those failed launches. This supports an early startup blocker, but does not establish its cause or rule out every candidate interaction. No game binary, DRM, renderer, shader or system-setting workaround was attempted.

All installed timecycle files are restored to their exact pre-overhaul hashes (`final-mood-restoration.json`, both true). No candidate frames were captured, so no comparison page or visual/performance pass can be delivered. The prepared integrated candidate is retained. `baseline-return.jsonl` records the final bounded return-to-baseline launch separately.

The wrapper now permits a bounded one-attempt startup and no longer requires an empty carried loadout: its capture scenario contains no weapon give/strip/storage action. Closed menus and no owned autopilot entities are still required. Original per-run player position is restored on cleanup. The approved brighter/colorful gritty-night direction is also recorded in STAGE1.md.

Final return-to-baseline launch: two bounded Steam attempts produced no observed GTAIV process and ended GAME-UNAVAILABLE at 09:40:28Z (`baseline-return-error.txt`). The game is not running; no test process or game lock remains. Both original mood-file hashes remain restored. Windows state capture also timed out for game/Steam, so it did not provide a usable launcher observation or any screenshot evidence. Automatic retries are stopped; a normal manual game launch is the next required external-state check before continuing in-game validation.
