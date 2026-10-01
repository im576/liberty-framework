# Tools index

Run everything from the repository root in PowerShell. The GTA IV folder is `<GTAIV>` (the one containing `GTAIV.exe`);
close the game before anything that installs or rolls back.

## Build

- `get-toolchains.ps1 -Directory <outside repo>`: downloads the pinned Roslyn (C# 7.3) and llvm-mingw; writes the git-ignored `toolchains.local.json` (`toolchains.ps1` reads it).
- `build.ps1 -ScriptHookDotNetReference <ScriptHookDotNet.asi>`: builds `LibertyFramework.net.dll` (warnings are errors).
- `build-core.ps1`: builds `native/LibertyCore/bin/LibertyCore.dll` (32-bit C++20).
- `build-content.ps1`: builds the content compiler `tools/content/bin/LibertyContent.exe`. `build-finishes.ps1` and `generate-presets.ps1` are the old gold-finish and preset generators.

## Verify

- `verify.ps1 -GameDirectory <GTAIV>` (or `-NoGame` in the cloud): offline verifier (resolvers vs disassembly, natives, config and logic tests, SDK examples). Sources in `tools/verify/`.
- `tools/tests/Run-Tests.ps1`: PowerShell unit tests for the autopilot and the local verifier against a simulated game.

## Package, install, rollback

- `package-phase2.ps1` builds the whole stack (DLL, core, models, config) into `staging/phase2`; `install-phase2.ps1 -GameDirectory <GTAIV>` installs it with a backup; `rollback-phase2.ps1` restores the newest backup; `test-phase2-install.ps1` dry-runs install and rollback on a mock game folder.
- Optional companions: `install-violent-liberty.ps1` / `rollback-violent-liberty.ps1` (owner-downloaded archive, hash-checked), `install-weapon-pack.ps1` / `rollback-weapon-pack.ps1` (Realistic Weapon Overhaul), `install-mood.ps1` (timecycle from `config/mood.json`, `-Restore` undoes it).

## Cloud tests

`.claude/hooks/session-start.sh` runs `cloud/setup.sh` (mono, PowerShell 7, Roslyn, llvm-mingw, Blender `bpy`). `cloud/test-all.sh` runs every offline check and prints PASS / FAIL / NOT-RUN. Anything NOT-RUN is covered on the PC by the local verifier. Details: [CLOUD_LOCAL_LOOP.md](../docs/workflow/CLOUD_LOCAL_LOOP.md).

## Local verification on the PC

`verify-local.ps1 [-Smoke] [-GameDirectory <GTAIV>]` runs every check in `tests/local/checks.json` (builds, verifier, probes, package and install with backup, autopilot scenarios, manual checks) and pushes results to the `verification-results` branch. Settings: git-ignored `verify-local.settings.json`; results: `results-local/`. Code in `local/VerifyLocal.psm1`. **One game:** `verify-local.ps1`, `install-phase2.ps1`, `rollback-phase2.ps1`, `autopilot/Run-Scenario.ps1` and `autopilot/Run-Suite.ps1` all hold the machine-wide game lock (`local/GameLock.psm1`; child processes inherit it), so parallel sessions wait their turn. The installer records the installed worktree and commit in `<game>\scripts\LibertyFramework\installed-build.json`; `Run-Scenario.ps1` refuses a build from another worktree (`-AllowOtherBuild` overrides) and logs it as the first step of its report. **Speed:** verify-local builds the package (`package-phase2.ps1 -Phase Build`, below-normal priority) *before* taking the game lock, then only stages and installs under it (`-Phase Stage`, seconds) and releases it before publishing. Unchanged steps are skipped (`BuildCache.psm1`: per-worktree stamps for the C#, core and content-compiler builds; a shared cache in `%LOCALAPPDATA%\LibertyFramework\build-cache` for vehicle extras, icons, sling and content models; `LIBERTY_NO_BUILD_CACHE=1` rebuilds everything). A scenario ends at once when the engine log goes silent for 150 s (`GAME-FROZEN`); a game that cannot start (no audio device, its own fatal error, every launch failed: `GAME-UNAVAILABLE`) makes the run's remaining scenarios NOT-RUN instead of retrying. A game an earlier test left running is stopped automatically; the owner's own game is never touched. **Iterating:** `-Quick` runs each scenario's short variant (a line starting `@full ` runs only in full runs, `@quick ` only in quick runs, and `{quick:A|B}` becomes A in quick and B in full, e.g. `cycle-deaths {quick:5|25}`); quick results are recorded as `mode quick` and are never acceptance evidence. `-StopOnFailure` ends a scenario at its first failed step and marks later checks NOT-RUN after the first FAIL/CRASH/ERROR; restoration still runs. `-MaxGameMinutes` (default 30) starts after lock acquisition (queue waiting is recorded separately), bounds each scenario by the remaining allowance, and marks later scenarios NOT-RUN. Owned-game shutdown/restoration can take additional time. Final acceptance is a full run without `-Quick`. `checks/checks.py` validates the queue and regenerates [LOCAL_VERIFICATION_PLAN.md](../docs/testing/LOCAL_VERIFICATION_PLAN.md).

## Content, models, Blender

- `content/` is the Liberty Content Compiler (`LibertyContent`: build, validate, probes, selftest); see [docs/content](../docs/content/README.md).
- `models/` builds `LibertyModel.exe` (read/write GTA IV drawables; `export`, `survey`, `selftest`); see [ModelFormat.md](../docs/research/ModelFormat.md). `finishes/`, `ui/`, `mood/`, `vehicles/` are smaller asset helpers.
- `blender/`: the Liberty Exporter add-on. `blender/run-tests.ps1 -Blender <blender.exe>` runs its headless tests; `blender/make-examples.ps1` regenerates examples and fixtures. See [BLENDER.md](../docs/content/BLENDER.md).

## Autopilot

`Import-Module tools/autopilot/Autopilot.psm1`, then `Test-Boot`, `Start-GameReady`, `Invoke-EngineCommand`, `Save-Screenshot`. `autopilot/Run-Scenario.ps1 -GameDirectory <GTAIV> -Scenario tools/autopilot/scenarios/<name>.txt -OutputDirectory <runs>` runs one scenario; `Run-Suite.ps1` runs many and exits 1 unless all pass. Each run writes `report.md` and `result.json` and prints `AUTOPILOT_RESULT <path>`; statuses are PASS, NEEDS-REVIEW, FAIL, CRASH, ERROR. Scenario lines are engine commands (`lf help`) plus `wait`, `shot`, `expect`, `key`, `mark`/`expectmarked` and `gpumem <label>`. Screenshots use Steam F12 (GDI is black under Vulkan). Decision logic: `autopilot/AutopilotLogic.psm1`.

## Stage 1 measurement (T-040)

- `perf/Measure-GunplayRange.ps1 -Reports <report folder> [-Out range.json] [-Markdown range.md]` (T-042) reads the `range_shot` lines of scenario `stage1-gunplay-range` and tabulates delivered spread per weapon (first shot, burst 2-3, sustained 4+): mean/p95/max deviation, the cone the model wrote, and the share of bullets inside it.
- `perf/New-Stage1Scenarios.ps1` writes the six `stage1-*` scenarios from the `s1_*` capture points in `config/devtools/locations.json` (`-Check` verifies they are up to date). `stage1-capture-broker` (8 points) and `stage1-capture-city` (4) visit each point in day/overcast and night/rain, sample 8 s of frame statistics and script costs, then take a clean screenshot; `stage1-worst-case` is the 60 s firefight scene. The `-off` variants stop the Liberty gameplay modules first (mod-off) and restart them at the end.
- Engine commands they use: `label <name>` (autopilot mod; groups what follows), `framestats` (avg, p50, p95, p99, max, slow frames, stalls since the last call), `perf`, `costs`, `pools`, plus the scenario line `gpumem <label>` (dedicated GPU memory of the GTAIV process from the Windows "GPU Process Memory" counters, private bytes, working set; written to `measurements.json` in the report).
- `perf/Measure-Stage1.ps1 -Reports <results folder> -Out summary.json` writes one JSON summary per condition; `-On summary.mod-on.json -Off summary.mod-off.json -Markdown compare.md` prints the comparison with the Stage 1 budgets (STAGE1 section 10). `perf/New-ContactSheet.ps1` lays the 12 points out as one image (mod-on/mod-off, day/night). Parsing lives in `perf/Stage1Metrics.psm1`, tested by `tests/Stage1Metrics.Tests.ps1`.

## Ops

- `capture-performance.ps1 -Label <name> -Seconds 120` (PowerShell as Administrator, needs PresentMon): frame-time capture, see [T-026](../docs/tasks/T-026-performance-visual-baseline.md).
- `install-dxvk-gplasync.ps1` / `rollback-dxvk-gplasync.ps1`: optional async-shader DXVK build (inspected archive hashes only, files backed up).
- Violent Liberty: `install-violent-liberty.ps1 -GameDirectory <GTAIV> -ArchivePath <zip>` prints a backup path for `rollback-violent-liberty.ps1 -BackupDirectory`. Third-party binaries never enter Git.

## Archive

One-shot Phase 1 scripts (T-001/T-002/T-007 deploys, Phase 1 package/install/rollback, SSD moves) live in [archive/](archive/README.md).