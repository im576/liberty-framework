# Local branch review and integration — 2026-10-02

The owner requested review and safe merging of local work, a plan for the mod, advice on agents/reverse engineering,
and questions at the end. Review started with all ten worktrees clean and primary main at `a2bbe74`.
The integration was prepared on `codex/integration-2026-10-02` before updating main.

## Integration decisions

| Source | Reviewed tip | Decision |
|---|---|---|
| Main review/tooling | `a2bbe74` | Base; preserve dependency/resume/restore fixes, skills and concise current instructions |
| Feature preview and citywide look | `e3f8392` | Merge source, config and evidence; candidate appearance stays opt-in through the separate installer |
| B wheel/trunk follow-up | `392e36b` | Merge initial radial snapshot publication, watcher fixtures and bounded metrics; retain trunk performance failure |
| Older orchestrator | `9be5315` | `git cherry` confirms its watcher and log-rotation patches already have equivalents on main; no redundant merge |
| C gore/effects | `4cae50e` | Preserve separately: setup crashes, 45.373 ms peak versus 4 ms gate, strict cleanup and actual cut acceptance unresolved |
| D replacement HUD | `6ff67ab` | Preserve separately: source deliberately reports hiding unverified; held-weapon/aim/native text/restoration gaps remain |
| R material fixture | `9dbc702` | Preserve separately: effective table, glass/water/object evidence and stall/parking restoration remain unresolved |
| Broader research / SDK 1.3 | `2491687` | Preserve separately: broad research/probe/API work is not needed to combine the preview; reviewed inventory, not every finding reverified |
| Older B and C worktrees | `0f5825f`, `bbcf12d` | Preserve history/evidence; older C changes superseded by the still-experimental C branch |

Preview merge `f102729` and B merge `61df23f` preserve their original branch ancestry. Conflicts were confined to
the current dashboard; retain concise current status and actual failures rather than old dispatch assignments.
Automatic runner merge was inspected to preserve startup telemetry, wheel-fixture finally restoration and mood checks.
No experimental C/D/R production code was copied into the integrated preview.

The preview profile disables `combat` and `atmosphere`; density remains OFF and HUD remains vanilla. This is the
existing limited preview's deliberate profile, not evidence that those unfinished modules have passed acceptance.
The integrated mood config is v3. Historical v3 receipts remain tied to the preview worktree, not to this new build.

## Additional defects found and repaired

- Mood capture previously trusted any `installedFiles` list, including an empty or incomplete one. Shared receipt
  validation now requires exactly both expected filenames, valid hashes and matching bytes/config before capture.
- Rollback now checks both backup files and the expected backup location before mutating either target. Empty,
  duplicate, unexpected-path and stale-hash receipts are rejected. Existing prior-receipt chaining is preserved.
- The citywide wrapper could retain success when the game exited after captures and before cleanup. It now fails,
  records cleanup evidence, rolls back the candidate and releases the lock. The success message follows cleanup.
  Output-directory creation also sits inside the lock's try/finally.
- Mood scratch cleanup verifies its resolved directory stays directly under temp with the expected prefix.
- Citywide task/check becomes T-059/T059-citywide-environment. Repo review retains T-058; this integration is T-060.
  Old branch commits and run receipts keep their original identifiers, explicitly documented on the renamed card.

## Local visual evidence

Read v3 manifest and raw result: 171 steps, zero failed steps, one logged error, 20 captures, NEEDS-REVIEW.
Inspected the actual v3 station cloudy-day and Star Junction cloudy-night images. Daylight is strongly subdued/grey;
night signs are vivid while building facades remain dark. These are limited observations of two frames, not a fresh
20-scene approval or proof of the AI targets' material/light fidelity. Prior report records the full comparison.
The candidate's final launch failure and startup stall remain open.

The integrated mood configuration matches the captured v3 byte hash. Generator source differs only in newline
encoding (normalized text and Git content match); regeneration produces both captured v3 table hashes exactly.
This confirms the preserved visual candidate's data, not runtime compatibility with the new gameplay build.

Existing visual evidence stays in `C:/Users/IM576/GTAIV-Reborn-feature-preview/results-local/citywide-overhaul/`.
The final baseline restoration receipt matches the currently installed timecycle hashes:
`593B463E4B116D9317E4CAA13D328C60F8C2BD7171EAC3DCB2D533CCE1E97814` and
`8F4979D541E9CEBC314269DEA214051F239BCE982584E93ACF5FEB0DD9E697C7`.
Installed gameplay still identifies preview commit `36901ab`. No game launch/install/rollback occurred in this integration.

## Validation

Logs: `results-local/offline/integration-2026-10-02/`. These checks exercised the combined source offline.

- Combined SDK/engine/mod build: PASS; 101 SDK and 207 engine sources, zero errors.
- Repository verifier: 441 PASS, zero FAIL, five game-file checks NOT-RUN.
- Actual-source radial publication harness: 25 PASS / zero FAIL.
- Actual-source UI-budget observation harness: 25 PASS / zero FAIL; original failing budget case still fails as expected.
- Mood receipt fault tests: 16 PASS. Real wrapper with simulated game boundaries: eight PASS covering settled success,
  game exit after capture, and wrong settled position; fault cases preserve evidence and roll back.
- PowerShell 5.1 and PowerShell 7 suites: each 312 PASS / zero FAIL.
- Mood generator: 88/88 weather-time rows; unrelated/grain/fog-alpha fields preserved, deterministic output,
  invalid RGB/cloud-alpha rejection. Compiled with warnings as errors.
- Actual mood installer in a disposable fixture: candidate install, pristine restore and two exact chained rollbacks
  PASS; malformed source rejected without changing original fixture bytes. Only the fixture lock name/note was isolated.
- Queue: 83 active checks validate (89 total including retired history); generated plan refreshed. The migrated T059 check is QUEUED for the new integration,
  not marked accepted using another branch's historical receipt.

During fixture development, Windows PowerShell promoted intentional child stderr into an exception; the fixture now
captures the child exit explicitly. A test output-directory/exit-marker name collision was corrected. Failed scratch
fixtures remain preserved. These were fixture defects, not game evidence or concealed passing production runs.

No fresh combined in-game acceptance, screenshot suite, controller/mission/save-load test or cloud container run.
Passing offline checks reduces integration risk; it cannot promise that the combined gameplay is regression-free.
Next priority/team recommendations and owner questions: [NEXT_MILESTONE](../workflow/NEXT_MILESTONE.md).

Local main is advanced by fast-forward only after this integration's checks. Original worktrees and branch tips
remain available; no remote push, branch deletion, history reset or new agent dispatch is part of this work.
