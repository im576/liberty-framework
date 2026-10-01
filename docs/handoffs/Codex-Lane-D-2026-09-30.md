# Codex Lane D handoff — September 30, 2026

T-049 remains **NEEDS-PLAYTEST**. Work stopped with source changes and offline evidence preserved. No Lane D game install or gameplay scenario completed.
The owner changed instructions to **NO IN-GAME TESTING**. Do not launch the game, install/rollback into it, run gameplay scenarios/probes, or start
verify-local game checks. Research remains deferred. Tonight's hard stop is September 30 at **9:30 p.m. Pacific / October 1 04:30 UTC**.
This handoff was prepared before that deadline; no future/background work is scheduled.

**The matching Claude Sonnet Lane D must review ALL Codex changes, including any uncommitted files, before continuing. If nothing remains within
the authorized offline scope, review only. Do not merge or push.** Existing B/C/research worktrees are preserved and must not be edited by D.

## Checkout and commit state

- Engine checkout: `C:/Users/IM576/GTAIV-Reborn-lane-d`.
- Branch: `codex/T-049-hud-continuation`, tracking `origin/stage1/T-049`.
- Parent engine repository: `C:/Users/IM576/GTAIV-Reborn`. The attached research project is **not** the engine checkout.
- Started from `origin/stage1/T-049` at `e2f8181`, which already includes current local main `f5679a5`, `db08832` (game lock) and `45561b4` (cache/fail-fast).
  No merge was needed; `git merge main` reported already up to date. No B/C/research worktree was changed, no branches reset/rebased, no push performed.
- Codex source commit **`c07dbb5808919549c05eb59e4a9a913cd5cae8cc`**: `T-049: make HUD probes isolated and gate restoration and combined UI cost`.
- Codex source commit **`d56bee0`**: `T-049: cache prompt glyphs, cover budget rejection, and record offline-only limits`.
- This handoff is delivered in a separate documentation commit after those source commits; inspect `git log -3 --oneline` for its exact hash.
- No outstanding tracked/untracked source edits at handoff creation. Only this handoff is newly added for the final documentation commit.
  Check `git status --short --branch` before continuing; any later edits must also be reviewed. Ignored build/results files are retained locally.

Review commands, from this checkout:

```powershell
git status --short --branch
git log -3 --oneline
git diff e2f8181..HEAD
git diff
git ls-files --others --exclude-standard
```

The complete inherited HUD implementation is newer than the old Claude cloud branch. For integration context also inspect `git diff main..HEAD`;
do not replace it with `origin/claude/ecstatic-keller-qu3sto`.

## Changes and reasoning

| Files | Change / reason |
|---|---|
| `src/LibertyFramework/Hud/HudModule.cs` | Added diagnostic `probe-mode on/off` to suspend Liberty replacement drawing for genuine vanilla baseline captures; reapply probe hides per tick; clear probe ownership on config-off. Added real JSON `config-test off/restore`, retaining original bytes and restoring them on stop/unload/failure. Corrected duplicated `hud_status hud enabled` prefix and report empty `drawn=` while disabled. Mouse buttons join glyph device switching. |
| `src/LibertyFramework/Hud/Logic/HudPromptText.cs` | Cache glyph expansion until prompt text, input device or hot-reloaded glyph table changes. Retain text during fade-out, re-evaluate reopening. Avoid per-frame StringBuilder/list work and repeated unknown-token logs. No measured performance gain claimed. |
| `tools/autopilot/scenarios/hud-components.txt` | Genuine vanilla probe mode, individual icon/ammo captures plus combined capture, explicit restoration and return to shipped policy. Uses T-044 pistol cap. These source changes have not run in game. |
| `tools/autopilot/scenarios/stage1-hud.txt` | Start unarmed; request 150 pistol rounds instead of 400 (existing loadout cap), expect `17 / 133`. Enforce HUD `<0.35 ms` and combined `draw.ui <=0.5 ms`; test config disable via actual file edit/normal poll, zero hidden components/no HUD-help ownership, and restoration. These steps remain queued. |
| `tools/verify/Stage1HudChecks.cs` | Eight prompt-cache cases; eight acceptance-budget cases. Uses actual scenario expressions and rejects over-budget, multi-ms and missing samples. This cannot certify live gameplay cost. |
| `config/atmosphere.json`, `tools/verify/AtmosphereChecks.cs` | Disable adaptive population density and assert that policy, per existing owner decision. Retain tuning and algorithm tests. T-040 baseline files unchanged. B independently made an equivalent density change; integration must preserve the policy and resolve the overlapping test additions deliberately. |
| `tools/verify/LogicChecks.cs` | Recognize current `staging/phase2/update/common/data/WeaponInfo.xml` before old Phase 1/t007 fallbacks. A pre-restriction verifier failed on the obsolete lookup in the fresh checkout; a fully packaged rerun has not established this correction locally. |
| `tests/local/checks.json`, `docs/testing/LOCAL_VERIFICATION_PLAN.md` | Retain task-specific queued checks; update accurate ammo cap, combined cost gate and restoration/capture expectations, and correct manual health/armour steps. Regenerated plan. Check statuses remain QUEUED, not PASS. |
| `docs/research/HudComponents.md`, `docs/tasks/T-049-stage1-hud.md`, `docs/architecture/CONFIG_SCHEMA.md`, `docs/PROJECT_STATE.md` | Reconcile existing file-only findings and historical screenshot review; distinguish assumptions, diagnostics, owner restriction and acceptance. Task stays NEEDS-PLAYTEST. |
| `docs/handoffs/Codex-Lane-D-2026-09-30.md` | This handoff. |

No new natives, guessed addresses, hooks or renderer/OS/hardware changes. The inherited hud.dat mechanism follows ADR-0004. Health/armour config lists
remain empty with `drawWithoutHidingVanilla=false`; top-right layout is preserved, radar stays bottom-left. `layout-test` is a diagnostic override only.

## Offline checks actually completed

| Check | Result / evidence (paths within this checkout) |
|---|---|
| Final Windows managed build | PASS, zero errors; warnings as errors. SDK 101 sources, engine 210 sources, Autopilot 4 sources, World 3 sources. `results-local/offline/build-final.log`. Engine SHA-256 `D62610448F9094AA7DBD9AED29EAAC97058F6B43BB51799CC136E74F1F6A7891`. |
| Native Windows core and unit tests | PASS: native fault-containment `natives_test`, `ray_walk_test`. First run log `results-local/20260930-194804-e2f8181/LOOP-build.log`; second run reused unchanged successful core inputs. |
| Final repository-only verifier | **passed=469 failed=0 notrun=7**. `results-local/offline/verify-nogame-final.log`. Seven omissions include executable/archive sections and unstaged WeaponInfo; not acceptance passes. Earlier source stage had 453/0/7. |
| PowerShell tooling simulations | **passed=212 failed=0**. `results-local/offline/powershell-tests.log`; simulated game only. No real game launched by these tests. |
| Content compiler/selftest/fixtures | Build PASS; **selftest 364/0**, **fixtures 5/0**. `results-local/20260930-195611-c07dbb5/LOOP-content-selftest.log`. |
| Model tool with package flags | PASS, `/warn:4 /warnaserror+`, Windows Roslyn x86 build to `results-local/offline/LibertyModel.exe`. No assets installed. |
| Art lifecycle/provenance | Selftest 18/0; validate 12 requests, 0 problems. No generated art changed. |
| Queue/plan | `checks: ok checks=75 pc-offline=13 probe=4 scenario=37 manual=21`; final generated plan current; `git diff --check` clean. |

`tools/cloud/test-all.sh` was not executed as a shell script: this Windows setup has no Bash/Mono cloud environment. Relevant Windows equivalents
above ran. Blender extension/add-on tests are NOT RUN (no configured Blender/bpy toolchain here; no Blender changes). No claim of an all-cloud-suite pass.

## Interrupted verifier state and game safety

1. `results-local/20260930-194804-e2f8181`: build/content checks passed; manually stopped **before install** upon discovering density-enabled defaults.
2. `results-local/20260930-195611-c07dbb5`: build/content checks passed; package-build was unfinished when owner prohibited testing. Run no longer exists.
   It did not reach game installation. `LOOP-package-install.log.out` holds partial pre-install build output.

Both summaries intentionally remain incomplete (no `finishedUtc`, install `not installed`, no install backup); an `INTERRUPTED.md` explains each.
Do not read their two PASS counts as completion of the selected batch. **Package-install, T028-wtdcheck and both T049 scenarios were NOT RUN.**
No Lane D game scenario, screenshot or measured draw-cost result exists. No Lane D restoration was needed because neither run installed.
Before the owner restriction, installed metadata identified B's `9f5dad2` build; that was B's responsibility, not a D build. D did not touch B's
verifier or the owner's game. A process audit after restriction found no surviving D verifier/package processes. Do not launch a rollback on D's behalf.

## Existing file-only and screenshot evidence

Before the restriction, an exploratory `tools/verify.ps1 -GameDirectory` read the executable **on disk**, without launching/installing.
CE **1.2.0.59**, exe SHA-256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`.
The inherited resolver found **31 parsed components, layout_differs=0, 23 unparsed registrations**. Weapon/ammo/wanted component names exist with
the expected layout. This establishes parsing, **not that the corresponding vanilla elements visibly disappear or restore**.
The full run result was **1029 passed / 2 failed / 1 notrun**, not PASS: obsolete WeaponInfo staging path and density-enabled assertion failed.
Evidence: `results-local/offline/verify-game.log`. Those source checks are corrected; a fully packaged verifier rerun is outstanding.

No independently hideable **player** health/armour arcs were established. Parsed HEALTH/ARMOUR names are target-reticle components, excluded from D ownership.
The unparsed registrations mean the table is not exhaustive. Recommendation: retain vanilla arcs until future owner-authorized evidence proves replacement;
do not silently approve duplicated bars or hide/relocate radar.

Packaging output inspected before restriction: icons 7/10/14 are all **256x128**, matching configured 96x48 ratio.
Historical B capture reviewed: `C:/Users/IM576/GTAIV-Reborn-lane-b/results-local/quick/stage1-trunk-ui-20260930-174525/trunk_ui_open.png`.
Wheel/storage panel visible, radar and Liberty HUD absent. It does **not** prove coexistence. B's newer source at `9979355` puts the storage panel at
virtual y=220, versus D default group bottom y=142: 78 virtual units of source-level clearance. No integrated screenshot proves that layout or retained radar.

## Unfinished work and review risks

- Sonnet review of both Codex commits and this handoff; inspect uncommitted changes before continuation.
- Actual component hide/restore results, individual ammo/icon proof, radar-arc separation and unparsed registrations.
- Actual HUD top-right rendering, radar bottom-left visibility, fades/reload/low-health state, glyph auto-switching on real mouse/pad input.
- `totalIncludesClip=true` remains unverified; expecting the arithmetic does not prove native semantics. Pistol cap is inherited 150, not adjusted to fit a test.
- Config-test hook was compiled but never run: it retains original bytes in memory and writes them back on restore/stop/unload/failure. A process crash
  before cleanup can leave the diagnostic JSON edit; future verifier `-Restore` must recover backup. Concurrent owner edits during the diagnostic would be
  replaced by its saved original. Sonnet must inspect restoration behavior; do not claim exact-byte restoration succeeded in game.
- Real HUD and combined UI cost, frame responsiveness and screenshot acceptance. Negative gate tests cannot prove a workload stays inside a budget.
- Package-install/full offline verifier including packaged stats remains outstanding. Partial `staging/phase2-build` is **not** an installable certified package.
- Wheel/trunk coexistence requires deliberate integration by the orchestrator and future authorized captures; D did not merge B's implementation.
- Owner visual/feel/timing judgment still required. T-049 must not be DONE.

## Safe restart

Under current authorization, continue **source review/build/offline-only** work in this exact checkout after Sonnet's complete review:

```powershell
Set-Location C:/Users/IM576/GTAIV-Reborn-lane-d
git status --short --branch
git log -3 --oneline
git diff e2f8181..HEAD
git diff
python tools/checks/checks.py validate
pwsh -NoProfile -File tools/verify.ps1 -NoGame
pwsh -NoProfile -File tools/build.ps1 -ScriptHookDotNetReference 'C:/Games/Grand Theft Auto IV/GTAIV/ScriptHookDotNet.asi'
```

`tools/toolchains.local.json` is ignored and references the existing `D:/GTAIV-Reborn-Tools/toolchains` runtimes; no toolchain install required.
PowerShell tooling tests, if new tooling changes warrant them: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1` (simulated only).
Do not resume the interrupted verifier batches or run gameplay checks under the current restriction. Do not edit/build a checkout still used by a verifier.

**Only after a future explicit owner authorization to resume game testing**, and after Sonnet review and a clean committed tree updated with current main,
use a fresh verifier batch from this checkout (no installed-build override, never set the lock-holder environment variable):

```powershell
# FUTURE ONLY — not authorized now
pwsh -NoProfile -File tools/verify-local.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -AnyBranch -Only LOOP-build,LOOP-verify,LOOP-content-selftest,T028-wtdcheck,LOOP-package-install,T049-hud-components,T049-stage1-hud -NoPush -Restore -NoManual
```

Review logs and every named screenshot yourself. Owner/orchestrator must first authorize and coordinate any combined B/D integration;
do not modify B/C/research worktrees, merge main, start chats or push branches to accomplish it.
