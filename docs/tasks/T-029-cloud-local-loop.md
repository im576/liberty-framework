# T-029 — Cloud development loop and local verification

Status: **NEEDS-PLAYTEST** (the first `verify-local.ps1 -Smoke` run on the owner's PC is the test).

## Why

The owner works remotely without the PC. Engine development continues in Claude Code cloud sessions, which have no
game; everything that needs the game must be queued and proven the first time the PC is available, with nothing
forgotten and no false passes. Rules: [CLOUD_LOCAL_LOOP.md](../workflow/CLOUD_LOCAL_LOOP.md).

## What was built (Session 1, 2026-09-26)

- **Cloud toolchain:** SessionStart hook -> `tools/cloud/setup.sh` (mono, PowerShell 7, pinned Roslyn and llvm-mingw,
  hash-pinned ScriptHookDotNet reference, Blender 5.2.2 as `bpy`). `tools/cloud/test-all.sh`: every offline check in one
  PASS / FAIL / NOT-RUN table. Build scripts run on Linux without changing Windows behaviour.
- **Honest offline checks:** `verify.ps1 -NoGame` (game-file and x86-code sections NOT-RUN, never PASS); verifier
  sections isolated; Blender tests `--no-game`.
- **Autopilot results you can trust:** `result.json` per scenario, statuses PASS / NEEDS-REVIEW / FAIL / CRASH / ERROR,
  no stale or command-echo `expect` matches, crash on the last step is CRASH, the suite reads result files.
- **Check queue:** `tests/local/checks.json` (44 checks covering every NEEDS-PLAYTEST task), validator and generated
  [plan](../testing/LOCAL_VERIFICATION_PLAN.md).
- **`tools/verify-local.ps1`:** the owner's one command (preflight, build, tests, probes, install with backup,
  scenarios, manual prompts, keep/restore, scrubbed results pushed to `verification-results`).
- **Probes:** `LibertyContent probe drawables|collision` (structure only) for the next content sessions.
- **Tests:** 88 PowerShell tests (autopilot logic, simulated scenario runs, simulated `verify-local` runs against a
  local bare remote), 10 probe self-tests; all checked against deliberately broken logic.

## Human test steps

1. Close GTA IV. In PowerShell at the repository root: `git fetch`, `git checkout develop`, `git pull`.
2. If `tools/toolchains.local.json` is missing: `./tools/get-toolchains.ps1 -Directory <folder outside the repo>`.
3. Run `./tools/verify-local.ps1 -Smoke -GameDirectory '<GTAIV folder>'`. Expect, in about 10 minutes:
   - the preflight passes (or tells you exactly what to fix);
   - `LOOP-build -> PASS`, `LOOP-package-install -> PASS`, `SDK-selftest -> PASS`;
   - "restored from ..." (Smoke always puts your previous install back);
   - "pushed to verification-results: results/<run>".
4. Start a cloud session: *Process the newest verification results.* That session confirms the loop works end to end.
5. Then the full run whenever you have time: `./tools/verify-local.ps1`.

If the script itself fails, the results folder under `results-local/` still holds `summary.json` and the logs; push
them by hand or paste `summary.md` into the cloud session.

## Known limits

- PowerShell 5.1 compatibility is by construction (no 7-only syntax); only PowerShell 7 ran here.
- The probes' drawable statistics and the Windows-only paths (install, rollback, Steam launch, JPEG conversion,
  `taskkill`) run for the first time on the PC.
