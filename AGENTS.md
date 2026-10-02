# Working on Liberty

Liberty is the engine and mod project for GTA IV Complete Edition 1.2.0.59: FusionFix,
ScriptHookDotNet 1.7.1.8, C# 7.3 / .NET Framework 4 x86 and a C++20 x86 native core.
One SHDN host drives modules, the public SDK, content tools and automated game scenarios.

## Start with the current assignment

1. Read the user's request, `docs/PROJECT_STATE.md` and `git status --short`.
2. Read the relevant task card and only the code/docs it links to. A repository-wide review
   is an explicit exception: inspect across areas, as requested. Do not choose a different task
   just because the user did not name a card; create a card for their actual request.
3. For a game run or lane takeover, read `docs/workflow/ORCHESTRATOR.md` in the primary
   checkout. It alone records current game ownership. Timestamped reports are historical evidence.
4. Use `tools/review/audit.py` to find receipts and repeat failures before repeating experiments.
   Source for the current build may be ahead of GitHub or on another worktree. Inspect it; never
   reset, replace or merge all lane branches merely to match the remote.

Task-specific project skills: `.agents/skills/liberty-evidence/SKILL.md` for test/failure review;
`.agents/skills/liberty-research/SKILL.md` for reverse engineering and mod adaptation. Load only
what the task needs. Claude uses this file through `CLAUDE.md`; these skills are also linked here
so either agent can use the same instructions.

## Engine invariants

- New mechanics use `[Module]` / `LibertyModule`, preferably SDK-only code in `mods/`.
  Use snapshots and events before adding per-frame native calls. The draw thread must not call natives.
- Keep gameplay tuning in `config/`, document new fields in `docs/architecture/CONFIG_SCHEMA.md`,
  and support disable/unload/config-off restoration. Outside the Stage 1 catalog, weapons stay vanilla.
- Never invent natives, offsets or layouts. Record native CE status in `docs/game-api/NATIVES.md`;
  memory access uses validated resolution with `docs/game-api/MEMORY.md` and an ADR. A failed
  resolver disables its capability. Offline decoding is not in-game proof.
- Resource ownership, exception isolation and hot-reload cleanup are mandatory. Important failures
  use the project logger. Do not hide errors, swallow exceptions, or weaken acceptance thresholds.
- Owner decisions: density governor OFF; 2 long guns plus 1 sidearm (SMGs are long guns);
  grounded severe gore with bounded 3–5 minute body retention; vanilla HUD stays until replacement
  and restoration are proven. Preserve mission/cutscene/save compatibility and the original baseline.
- Use source where its recorded license permits adaptation; retain notices and per-file provenance.
  Consult `third_party/README.md`. The locally inspected LVS source is MIT and a candidate for a
  selective port. Closed binaries can inform documented research; they do not establish reusable source.
  Restricted source/assets are research references unless reuse permission has been established.

## Build, test and evidence

Commands and prerequisites: `tools/README.md`. Typical offline sequence on the PC:

```powershell
pwsh -NoProfile -File tools/build.ps1 -ScriptHookDotNetReference '<game>/ScriptHookDotNet.asi'
pwsh -NoProfile -File tools/verify.ps1 -NoGame
pwsh -NoProfile -File tools/tests/Run-Tests.ps1
python tools/checks/checks.py validate
```

Check each exit code. Run the affected suite while iterating (`-Filter ResumeEvidence`, for example);
run required integration checks after the final change. Cloud: `tools/cloud/test-all.sh`.
Do not repeatedly run the whole suite after documentation-only edits or unchanged successful checks.

Use the existing machine-wide game lock through `tools/verify-local.ps1`. Choose affected check IDs,
use `-AnyBranch -NoPush -Restore -NoManual -StopOnFailure -MaxGameMinutes 30`, and add `-Quick`
for development probes. Full acceptance keeps original counts, durations and budgets. Inspect the first
failed assertion, startup journal or crash record before retrying. A retry must test a changed input
or an explicit environmental hypothesis; repeated identical batches are not progress.

A saved install result is not the current installed state. `-Resume` cannot cross source/mode/queue
changes or reuse an old successful install for pending gameplay. Start a fresh batch when required.
Do not stop another agent's processes or replace an owner preview based on an old handoff.

## Finish the assignment

Track changes and exact evidence on its task card. Add game checks to `tests/local/checks.json`
when runtime behavior changes; regenerate with `python tools/checks/checks.py plan`.
Separate offline PASS, full gameplay PASS, quick observations, NOT-RUN and owner visual/feel sign-off.
A screenshot path is not visual review. Only the owner declares gameplay DONE.
Update the short dashboard only if current state changes; keep long evidence in reports or task cards.
Commit locally with `T-0xx: <change>` after validation; do not push or message other tasks unless requested.

Architecture: `docs/architecture/ENGINE.md`; design/budgets: `docs/design/STAGE1.md`;
research priorities: `docs/research/RESEARCH_PROGRAM.md`. Historical rules are under
`docs/archive/review-2026-10-02/` and must not override current user instructions.
