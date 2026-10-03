---
name: liberty-evidence
description: Diagnose Liberty Framework GTA IV test failures, review lane evidence, or prepare a bounded verification batch from local receipts and logs.
---

# Liberty evidence workflow

Use the current checkout and `docs/workflow/ORCHESTRATOR.md` in the primary repository.
Liberty+ owns gameplay and its combined check queue. From its checkout use
`tools/prepare-workspace.ps1` for existing integration/scenario tools; never edit that
generated workspace. Record both repository revisions and dirty input hashes.
The attached ChatGPT folder may contain only research; use git root/remote and the handoff paths
to locate the actual source. Do not reset local branches to the remote.

Run `python tools/review/audit.py --output results-local/offline/audit.json` when reviewing
multiple runs. It indexes registered worktrees and reports modes, repeated failures and slow checks.
Historical aggregate counts do not establish current acceptance. Do not count `_runs` copies twice.

For one failure, read its `summary.json`, the check's `result.json` / `failedSteps`, then the relevant
log window and startup journal or crash record. Distinguish launch failure, fixture failure,
engine crash, assertion failure and visual uncertainty before changing production behavior.
Correlate commit, mode, installed receipt, executable/plugin hashes and the exact entity/handle.
A later passing run does not erase the old failure or prove an unrelated patch caused it.

State one hypothesis and a discriminating observation. Fix or probe that cause; use affected offline
tests before a small Quick batch. Full acceptance retains original trial counts, waits and budgets.
After a repeated failure, change the hypothesis or input before paying for another launch.
Keep setup, startup, lock wait, package build and scenario time distinct; old summaries omit some time.

Tests: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1 -Filter <SuiteName>`.
Omit `.Tests.ps1` from the filter. `-ResultsPath <existing-directory/file.json>` records suite times;
failed fixtures are preserved for diagnosis. The filter must match at least one suite.

Use `verify-local.ps1` with explicit check IDs, `-AnyBranch -NoPush -Restore -NoManual
-StopOnFailure -MaxGameMinutes 30` when the current task/schedule authorizes the game slot.
Do not replace the owner preview based on an old assignment. Never reuse a saved successful install
as evidence that its build remains installed. Start a new batch after source/mode/queue changes or
when pending game checks need an install; `-Resume` is deliberately conservative.

Return the actual fix, first-failure evidence, tests run and remaining uncertainty. A native call
returning successfully is not proof of a visible effect. Inspect captures when appearance matters.
Owner feel/controller/save/mission sign-offs remain distinct from scripted and offline passes.
