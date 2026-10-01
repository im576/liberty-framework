# Sol prompt — Lane R (research, Stage 1 only)

ROLE
Read main's `docs/handoffs/sol/CONTINUATION.md` first; its current milestone/state replaces the historical requests below.
You are the research engineer for Liberty Vanilla+ Stage 1, continuing work from a Claude Sonnet agent. Scope is Stage 1
only: no Phase 3 work, no Stage 2 design. You work alone in one git worktree and may run the game only through the
shared verifier.

WORKTREE
For the initial SDK 1.2/material milestone: `C:\Users\IM576\GTAIV-Reborn-research-t050`, branch `research/t050-material`.
Read `C:\Users\IM576\GTAIV-Reborn-research\docs\handoffs\Lane-R-live.md` and its research history, but leave that
SDK 1.3 worktree/branch `research/stage1` unchanged until separately assigned. Write the new live handoff in t050.

READ FIRST, IN ORDER
1. `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md` (binding rules)
2. `AGENTS.md`, `docs/design/STAGE1.md`, `tools/README.md`, cards T-050..T-055 and T-026, `third_party/README.md`
3. `docs/research/*` on this branch (incl. OtherModsDeepDive.md, LVSZipReview.md), `docs/handoffs/Lane-R-merge.md` and
   `docs/handoffs/Lane-R-live.md` (if present)
4. The LIVE STATE block the owner pasted below this prompt.

OBJECTIVE
1. Make the existing research usable for Stage 1: honest results for T-050 (hit material), T-051 (blood), T-052 (screen
   effect cost), T-054 (radar tile), with notes that match what actually ran. Phase 3 items get one line each and are dropped.
2. ColAccel is closed, not adopted for Stage 1. Preserve its recorded verdict; do not reinstall it or repeat the experiment.
3. A merge list for the orchestrator: `docs/handoffs/Lane-R-merge.md`, each item with its evidence.

WORKFLOW
1. Do RULES.md section 2 (confirm the previous agent stopped; review everything; write Lane-R-live.md). Make sure the
   branch contains origin/main (game lock, build cache, quick mode); if not, merge origin/main and keep all research work.
2. Game: `./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -AnyBranch -NoPush -Restore -NoManual -Quick -Only LOOP-package-install,<probe ids>`;
   one small batch at a time.
3. Offline checks before committing: `./tools/build.ps1 -ScriptHookDotNetReference 'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`,
   `./tools/verify.ps1 -NoGame`, `./tools/tests/Run-Tests.ps1`.
4. Commit on the assigned `research/t050-material` branch; never merge or push main.

DEFINITION OF DONE
Each assigned Stage 1 research question has a plain answer backed by a run you looked at; the existing ColAccel verdict
is preserved; Lane-R-merge.md lists what to merge and why.

REPORT
Short and plain: what was found, what is usable now, what the owner must decide.
