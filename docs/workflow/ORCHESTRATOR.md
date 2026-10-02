# Current coordination

Updated 2026-10-02. This file is the single current schedule. Older reports, prompts and
[the previous queue](../archive/review-2026-10-02/docs/workflow/ORCHESTRATOR.md) are evidence, not assignments.
The latest explicit user instruction takes precedence.

## Current activity

T-058: repository review/cleanup, offline tooling fixes, agent workflow and research preparation.
The owner preview remains installed. No lane has an assigned game/install/rollback slot in this review.
Offline work is authorized; serialize heavy builds on this host. Do not infer that a quiet process
list means an owner preview can be replaced.

Shared tooling repair commit: `6ef0446` on main (T-058). Before a future lane test, deliberately
integrate/review those tooling hunks against the lane's own host changes and run its offline checks.
This review did not auto-merge into the preserved lane branches or alter their tested candidates.

## Preserved work

Paths below are siblings of the primary `GTAIV-Reborn` checkout. Read current git state and the
lane's live handoff; these tips are a review-start snapshot, not a requirement to reset a branch.

| Lane | Checkout suffix | Reviewed-start tip | Next evidence needed |
|---|---|---|---|
| B | `-lane-b2` | `392e36b` | Paired trunk cost observations; preserve wheel pass and trunk budget failure |
| C | `-lane-c-t048` | `4cae50e` | Valid setup/active comparison; crash attribution, cleanup and cost gates |
| D | `-lane-d` | `6ff67ab` | Held-weapon/aim fixture, native HUD hide/restore and story text |
| R | `-research-t050` | `9dbc702` | Material mapping/targets, safe camera/parking, stall investigation |
| Research rest | `-research` | `2491687` | SDK 1.3 and remaining research; review separately |
| Preview | `-feature-preview` | `e3f8392` | Owner observations; installed source is earlier `36901ab` |

Older `-lane-b`, `-lane-c` and `-orchestrator` worktrees contain source/evidence history. Do not
remove them as generated bloat or merge them wholesale. Review branch differences before selecting fixes.

## Working cycle

1. Read the task, actual diff and first relevant failure. Use `python tools/review/audit.py --output
   results-local/offline/audit.json` for cross-worktree receipts. Do not re-read every historical report.
2. Make the smallest testable correction or hypothesis probe. Run affected offline checks and record results.
3. Prepare the specific game check IDs, stimulus, expected observation, failure stop and restoration receipt.
   Scheduling an actual game batch must respect the current owner installation and explicit task authorization.
4. Full acceptance keeps original budgets and trial counts; quick diagnostics are labelled. Stop on first failure,
   inspect it and change the hypothesis/input before another batch.
5. Review source and raw evidence before integrating. Keep independent changes separate so one failing feature
   does not block a proven tooling fix. A build, log assertion and visual acceptance establish different things.

Handoff command (offline; no dispatch):
`pwsh -NoProfile -File tools/handoff/Get-SolPrompt.ps1 -Lane Orchestrator -NoClipboard`.
Add `-RefreshRemote` only when current remote refs are needed. All worktrees share one fetch.
Role prompts supply area-specific context; this file supplies current scheduling.
