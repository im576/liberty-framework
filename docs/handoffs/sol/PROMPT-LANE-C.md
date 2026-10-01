# Sol prompt — Lane C (T-047 harsh gore, T-048 combat effects)

ROLE
You are the Lane C engineer for Liberty Vanilla+ Stage 1, continuing work from a Claude Sonnet agent. You work alone in
one git worktree on the owner's Windows PC and may run the game only through the shared verifier.

WORKTREE
`C:\Users\IM576\GTAIV-Reborn-lane-c-t048`, branch `stage1/T-048` (contains T-047). `C:\Users\IM576\GTAIV-Reborn-lane-c`
(`stage1/T-047`) is older history: do not edit it.

READ FIRST, IN ORDER
1. `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md` (binding rules)
2. `AGENTS.md`, `docs/design/STAGE1.md` (gore and combat effects sections, budgets), `tools/README.md`,
   `docs/architecture/decisions/ADR-0005-skeleton-hook.md`
3. `docs/tasks/T-047-stage1-gore.md`, `docs/tasks/T-048-stage1-combat-effects.md`, `docs/reports/2026-09-30-lane-c-continuation.md`
4. `docs/handoffs/Codex-Lane-C-2026-09-30.md` and `docs/handoffs/Lane-C-live.md` (if present)
5. The LIVE STATE block the owner pasted below this prompt.

OBJECTIVE
Full-run acceptance for T-047 and T-048 without weakening any criterion, then leave both cards NEEDS-PLAYTEST.

KNOWN STATE (verify against the live state)
- Last full batch (20260930-213901): panic PASS; trauma, NPC and firefight-off NEEDS-REVIEW; dismember, persist, firefight,
  effects-day FAIL; gore-head and effects-night CRASH; cleanup ERROR. Later commits added faster bone scans, limb clones
  re-killed until dead, a settle wait, a `release` command for retention fixtures and split day/night effects scenarios.
- Required, from the orchestrator's review:
  1. Floating limbs: deleting a floating limb hides the bug. Use the clone diagnostics (clone_dead, clone_air,
     clone_origin_height) to split the causes. For a lying clone whose kept limb is still raised, ground-snap the limb:
     you already write this clone's bone matrices every tick through the ADR-0005 path, so lower the cut bone's local
     translation in parent space by (joint height - ground Z) and re-apply until confirmed. The safety removal stays a
     counted failure. Acceptance: 50 completed cuts, 0 floating, 0 flashing, with screenshots of landed limbs.
  2. Fix the two crashes first (gore-head, effects-night): read the run logs and the LibertyFramework log for the cause.
  3. Retention 3-5 min, expiry and config-off release, tested with released (non-mission) fixtures. Never loosen the
     production ownership rule for real mission peds. In quick mode use a short test lifetime instead of waiting minutes.
  4. Class-specific weapon effects must be visible (day and night) and cleaned up; a queued or accepted call is not visible.
  5. The paired firefight budget (Measure-CombatAcceptance) must pass.

WORKFLOW
1. Do RULES.md section 2 (confirm the previous agent stopped; review everything; write Lane-C-live.md).
2. Crashes, then floating limbs, then retention/cleanup, then effects, then the firefight budget.
3. Offline checks after each change: `./tools/build.ps1 -ScriptHookDotNetReference 'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`,
   `./tools/verify.ps1 -NoGame`, `./tools/tests/Run-Tests.ps1`. All must pass.
4. Game iteration: `./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -AnyBranch -NoPush -Restore -NoManual -Quick -StopOnFailure -Only LOOP-package-install,<ids>`.
   Scenario generators mark long loops `{quick:A|B}` and long waits `@full`.
5. Acceptance: full runs (no `-Quick`) of every T-047/T-048 check, in batches that fit the 30-minute cap.
6. Update both cards (evidence, exact human test steps), `docs/PROJECT_STATE.md` (one line), Lane-C-live.md; commit.

DEFINITION OF DONE
Every T-047/T-048 check passes in a full run (NEEDS-REVIEW only for screenshots you reviewed and described), 50 clean cuts,
no crash, budgets met, offline checks pass, cards NEEDS-PLAYTEST, nothing merged to main.

REPORT
Short and plain: what passes, what fails and why, what is next, what the owner must decide.
