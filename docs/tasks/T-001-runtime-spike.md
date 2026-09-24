# T-001 — Minimal CE C# runtime spike

Status: **BLOCKED on T-000**. Owner: agent plus human playtest. Scope: compile and load one script that logs startup, a periodic heartbeat, caught errors, and clean unload/reload. No gameplay changes.

## Questions

Which .NET target/platform/C# language level actually loads? Where do logs go? Does ScriptHookDotNet's reload command work reliably on CE with FusionFix and the tester's controller configuration?

## Deliverables

Minimal source and project file; reproducible build/deploy commands in `tools/README.md`; dependency sourcing; exact build output; startup/reload log sample; updated [ADR-0001](../architecture/decisions/ADR-0001-runtime.md). Do not vendor ScriptHookDotNet binaries.

## Human test steps

To be written with exact paths/keys after T-000 confirms the installed runtime and menu bindings. Tester launches game, observes startup log, triggers one script reload, then verifies a second startup log and continued normal vanilla play.

## Exit

Agent sets `NEEDS-PLAYTEST`; human sets `DONE` only after logs and game behavior confirm the spike. If loading fails, record the exact error under `## Blocked` and stop.
