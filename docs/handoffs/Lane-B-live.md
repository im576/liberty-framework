# Lane B live handoff (T-045 weapon wheel, T-046 trunk UI; T-044 is finished and on main)

Worktree `C:\Users\IM576\GTAIV-Reborn-lane-b2`, branch `codex/lane-b-validation`. Not merged, not pushed (orchestrator integrates).
Update this file after every game run and meaningful commit. Rules for a successor: `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md`.

## Current milestone — Sol replacement, offline only (2026-10-01)

This section supersedes the historical next-step instructions below. No game slot has been assigned.

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

### Scheduled validation proposal and remaining questions

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
