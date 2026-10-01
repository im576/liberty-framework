# Codex Lane D handoff — September 30, 2026

T-049 is **BLOCKED on verified visible hiding**, with a conservative source guard ready for review. The owner lifted the temporary testing prohibition;
D completed a bounded game batch and the normal verifier restored D's installation at 8:57 p.m. Pacific. No further D game run is queued.
Tonight's hard stop is September 30 at **9:30 p.m. Pacific / October 1 04:30 UTC**. No background work or automation is scheduled.

**The matching Claude Sonnet Lane D must review ALL Codex changes, including any uncommitted files, before continuing. Do not merge or push.**
B/C/research worktrees are preserved. Deeper research remains deferred; do not guess a replacement native, offset, struct or hook.

## Checkout and review

- Engine checkout: `C:/Users/IM576/GTAIV-Reborn-lane-d`; branch `codex/T-049-hud-continuation`, tracking `origin/stage1/T-049`.
- Parent engine repo `C:/Users/IM576/GTAIV-Reborn`; attached research directory is not the engine checkout.
- Started at `e2f8181`, already containing main `f5679a5`, game lock `db08832` and cache/fail-fast `45561b4`. No integration merge was needed.
- Local commits before the final guard: `c07dbb5` probe/config restoration and UI gates; `d56bee0` prompt caching/input/budget tests; `4ac4fcc` initial handoff.
- The final local commit adds the visible-hiding guard, updated scenario/tests and current results documentation. Read `git log -4 --oneline` for its hash.
- No push, main change, B/C edit or orchestration chat was made. Ignored local build/results artifacts are retained for review.

```powershell
Set-Location C:/Users/IM576/GTAIV-Reborn-lane-d
git status --short --branch
git log -4 --oneline
git diff e2f8181..HEAD
git diff
git ls-files --others --exclude-standard
```

Inspect `git diff main..HEAD` for the full inherited implementation; do not replace it with the older Claude cloud branch.

## Actual game batch and conclusions

`results-local/20260930-205120-4ac4fcc/summary.json`, run 8:51–8:57 p.m. Pacific, on clean commit `4ac4fcc`:

```powershell
& ./tools/verify-local.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -AnyBranch -Only T049-hud-components,T049-stage1-hud -ScenarioTimeoutMinutes 6 -NoPush -Restore -NoManual
```

- Package/install PASS, build 4 seconds. Backup `C:/Games/Grand Theft Auto IV/GTAIV/scripts/LibertyFramework/backups/phase2-20260930-205133`.
- First launch failed before engine boot; the verifier's normal second attempt succeeded. No installed-build override or lock-holder override used.
- Component script **NEEDS-REVIEW**: steps completed, but visible hiding is NOT accepted.
- Normal HUD script **FAIL**: 110 steps, one failed wanted transition assertion, zero log errors, game alive at end.
- Normal verifier restoration recorded `restored from .../phase2-20260930-205133`; batch finished 03:57:16 UTC. D verifier exited, lock released.
  Another lane acquired the lock afterward. Do not read that lane's current installation as D's, or stop/restore its processes.
- CE 1.2.0.59, exe SHA256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`, FusionFix 5.0.1.

All 11 component captures and 12 HUD captures were visually reviewed. Detailed findings: [local report](../reports/2026-09-30-lane-d-hud-local.md).

**Failure:** wanted/radar remain visible during diagnostic hides. Normal HUD captures show duplicate weapon/ammo and wanted displays.
`HudReticle.IsHidden` reflects saved-value ownership, not the renderer. Even `hud_component alpha=0 size=0.00001x0.00001` did not establish disappearance.
The probe weapon/ammo baseline had no visible weapon/ammo, making those isolated captures inconclusive; cash was absent as well.

**Partial evidence:** initial pistol total 150 / clip 17 matches vanilla reserve 133 and Liberty `17 / 133`; firing/reload supports this pistol's total-includes-clip.
The first-shot event used a stale polled total with a fresh clip (16 / 134), so sample consistency remains a follow-up; do not extrapolate to every weapon.
Change/shot/reload, health/low-health/armour, forced pad/keyboard prompts and real config-off/re-enable steps passed. Config-off screenshot has no Liberty group;
re-enable on the tested pre-guard build brings the duplicate stars back. Byte restoration succeeded through the hook; no independent before/after hash was captured.
The final 72-frame diagnostic sample passed the gates: **draw.hud 0.306 ms / draw.ui 0.310 ms average**. No B wheel/trunk integration was included.

## Final source safety guard (NOT game-tested)

- `HudPlan.Decide` now requires separate resolution and visible-hiding predicates. All counterpart components must satisfy both.
- `HudModule.ApplyPlan` reports no verified T-049 hiding capabilities; shipped weapon/ammo/health/armour/wanted plans retain vanilla with zero planned hides.
- Prompt restyling remains active. Explicit `drawWithoutHidingVanilla` and diagnostic `layout-test` still permit duplicates, without writing unverified replacement globals.
- Probe hide/restore commands remain available for existing diagnostics; D never takes ownership of the four gunplay reticle components.
- Three new negative cases prove resolved-only and partially verified groups do not replace vanilla, and explicit diagnostics do not write unverified globals.
- Status now exposes sampled `wanted_stars`; the scenario checks state rather than requiring a new shown transition when already wanted. Shipped-plan assertions now expect vanilla/zero hides.
- Health/armour component arrays remain empty and duplicate permission false. Radar stays bottom left. Top-right layout is preserved for future clean replacement.

No new memory mechanism, native, hook, renderer/OS/hardware change or population thinning. Final guard requires a fresh committed-build game run before its runtime behavior can be accepted.

## Other Codex changes requiring review

| Files | Changes |
|---|---|
| `src/LibertyFramework/Hud/HudModule.cs` | Probe mode suspends Liberty frames, restores prior probes and reapplies diagnostic hides. Config-test saves/restores original JSON bytes on request/stop/unload/failure, clears probe ownership when disabled. Correct status prefix; mouse activity participates in device switching. Latest guard and wanted-state status above. |
| `src/LibertyFramework/Hud/Logic/HudPromptText.cs` | Cache glyph expansion by text/device/glyph-table identity; retain fade-out text and refresh reopening. No per-frame expansion or repeated unknown-token logs. No measured improvement claimed. |
| `src/LibertyFramework/Hud/Logic/HudPlan.cs` | Separate visible capability from resolved globals. |
| `tools/verify/Stage1HudChecks.cs` | Prompt cache cases, actual scenario budget expressions with negative rejection cases, three capability regression cases. |
| `tools/autopilot/scenarios/hud-components.txt`, `stage1-hud.txt` | Isolated baseline/captures, explicit unarmed-to-pistol selection before the next probe baseline, 150 pistol cap, actual config-off, combined draw gate, latest conservative-plan/state assertions. Final assertion changes unrun. |
| `config/atmosphere.json`, `tools/verify/AtmosphereChecks.cs` | Owner density governor OFF, retain algorithm/tuning tests; B independently changed this, integrate deliberately. T-040 baseline preserved. |
| `tools/verify/LogicChecks.cs` | Recognize packaged `staging/phase2` WeaponInfo ahead of old staging locations; final repository verifier now executes those packaged checks. |
| `tests/local/checks.json`, generated verification plan | Accurate cap, gates, diagnostics versus acceptance, config-off and conservative defaults. Queue still QUEUED for the final source; old batch not relabeled PASS. |
| Task/state/schema/research/report/handoff docs | Current findings supersede temporary restriction and inherited unproven hiding claims. |

Config-test caveats: a process crash before cleanup may leave its temporary disabled JSON; normal verifier backup restoration is the recovery path.
Concurrent config edits could be overwritten by its original-byte restoration. Do not run the diagnostic while editing that file.

## Offline checks actually completed

| Check | Result / local evidence |
|---|---|
| Final guard managed build | PASS, zero errors; warnings as errors. SDK 101 sources, engine 210, Autopilot 4, World 3. `results-local/offline/build-hiding-guard.log`. Engine SHA256 `1842B141E5DC470D889AF56C120588CBE1B11BA6A48320DD49C813DCF5AF00B1`. |
| Final guard repository verifier | **481 passed / 0 failed / 5 notrun**, `results-local/offline/verify-nogame-hiding-guard.log`. Packaged WeaponInfo checks now run after the successful package; executable/archive-specific omissions remain. |
| Native core/tests | PASS fault-containment `natives_test`, `ray_walk_test`; `results-local/20260930-194804-e2f8181/LOOP-build.log`. No native changes since. |
| PowerShell simulated tooling | **212 passed / 0 failed**, `results-local/offline/powershell-tests.log`. |
| Content | Build PASS, selftest 364/0, fixtures 5/0; `results-local/20260930-195611-c07dbb5/LOOP-content-selftest.log`. |
| Model packaging flags | PASS warnings as errors, `results-local/offline/LibertyModel.exe`. |
| Art | Selftest 18/0; 12 requests validated, zero problems. No art changes. |
| Queue/plan | 75 checks (13 offline, 4 probes, 37 scenarios, 21 manual), regenerated and validated; diff whitespace checked. |

Cloud `test-all.sh` was not executed (no Bash/Mono Windows environment); relevant Windows equivalents above ran. Blender/bpy tests NOT RUN, no Blender changes.
Earlier final pre-guard `-NoGame` was 469/0/7; the initial real-executable file scan was 1029/2/1, not PASS (obsolete staging path/density policy tests).
The successful package and final 481/0/5 verifier cover the staging correction; no full executable/archive verifier rerun occurred after that correction.

## Earlier interruptions and remaining work

Batches `20260930-194804-e2f8181` and `20260930-195611-c07dbb5` stopped before installation (density policy, then temporary owner restriction).
Their incomplete summaries and `INTERRUPTED.md` remain historical evidence; do not resume them. The later completed batch supersedes the initial no-install handoff.

Remaining: evidence-backed isolated hiding and restoration; final guard runtime validation; stable ammo sampling; all weapon/native ammo semantics;
real input-device switching; pause/cutscene/death/off/failure release; B/D wheel/trunk coexistence and combined UI sampling; owner look/feel sign-off.
Historical B screenshot lacked radar/HUD and was not integration evidence. B source `9979355` panel y=220 versus HUD bottom=142 gives static clearance only.

After Sonnet's complete review and fresh authorization appropriate to that session, coordinate through the normal verifier/lock and use a fresh committed-build batch.
Never set `LIBERTY_GAME_LOCK_HOLDER`, bypass installed-build checks, edit another lane's checkout, or kill another lane's verifier. Stop by the owner's deadline.
If research remains deferred, review and offline work only; the known hiding blocker is not solved by repeated screenshots or by weakening acceptance.
