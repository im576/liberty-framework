# Lane handoff rules

`AGENTS.md` contains the shared engineering rules; the user's current assignment controls scope.
Current schedule: primary checkout `docs/workflow/ORCHESTRATOR.md`. Do not copy historical
rules or dispatch snapshots into a new assignment.

On takeover, inspect git status/diffs, the task card, live handoff and relevant raw receipts. Preserve
unfinished work. Check actual process ownership before editing a checkout another worker may be using;
file modification time alone is not proof that an agent has stopped, and no arbitrary five-minute wait
is required. Do not start replacement workers or message existing tasks without user authorization.

Use `tools/handoff/Get-SolPrompt.ps1 -NoClipboard` for a fresh local snapshot. It does not fetch by
default, launch workers or allocate a game slot. Keep handoffs to current commit, changed paths,
proven results, unresolved question and next experiment; put historical narratives in reports.

Use the project's game lock and restoration wrappers. Never overwrite a preview or restore an old
backup solely because an interrupted run's summary names it: first compare installed identity and
ownership. Never alter thresholds/stimulus requirements merely to obtain PASS. Review visual evidence
when appearance matters; a native return value is not a visible effect.

Research may go deeper when requested by the owner. Scope it to documented engine questions and
retain executable hash/version and confidence labels. License/provenance decisions are recorded in
`third_party/README.md`; adapt permitted code selectively through SDK services rather than importing
another complete runtime. Keep density OFF and the original gameplay budgets.
