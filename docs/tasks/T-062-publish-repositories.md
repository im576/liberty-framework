# T-062 — Publish the separated repositories

Status: COMPLETE — repository publication only; gameplay development remains paused.

## Authorized scope

Publish the validated local framework split and create/publish Liberty+ on GitHub.
Keep existing history, preserved branches, game installation and source ownership intact.

## Repositories

- Framework: https://github.com/im576/liberty-framework
- Liberty+: https://github.com/im576/liberty-plus

Liberty+ matches the framework's existing public visibility. It starts from the
local extraction history; no generated validation workspaces or game installation
files are published. Documentation entry points link to the actual GitHub repos.

## Validation

Framework remote main was 26 commits behind local main, with zero remote-only
commits. Publication uses ordinary fast-forward pushes, never force-pushes.
Source/build behavior is unchanged; existing T-061 validation remains applicable.
Verify remote main hashes against both local heads and publish the pre-split
framework tag plus the four explicitly archived Liberty+ candidate branches.
These archived candidates remain unaccepted; publishing them does not merge them.
