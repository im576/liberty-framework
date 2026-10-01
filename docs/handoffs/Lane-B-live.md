# Lane B live handoff (T-045 weapon wheel, T-046 trunk UI; T-044 is finished and on main)

Worktree `C:\Users\IM576\GTAIV-Reborn-lane-b2`, branch `codex/lane-b-validation`. Not merged, not pushed (orchestrator integrates).
Update this file after every game run and meaningful commit. Rules for a successor: `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md`.

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
