# T-058 — Repository review, evidence tooling and research workflow

Status: NEEDS-PLAYTEST (offline review and repairs complete; owner review remains)
Owner request: 2026-10-02; broad review/cleanup of the current local project, agent failures,
test costs and reverse-engineering/mod adaptation foundation. Remote: im576/liberty-framework.

## Scope

Inventory primary source and all local worktrees; review architecture and failure evidence;
remove redundant live guidance; repair demonstrated offline tooling faults; provide reusable
evidence/research skills and a concrete research program for the larger mod vision.
Preserve installed preview, unique lane source, art provenance and crash evidence.

## Changes and evidence

See [review report](../reports/2026-10-02-repository-review.md). Detailed local audit and validation
logs: `results-local/offline/repo-review-2026-10-02/`. They are not redistributed game evidence.
No new gameplay implementation or acceptance is implied by the tooling fixes.

Final offline validation: forced C# build zero errors; repository verifier 441/0/5 not-run;
native build/tests PASS; content 364/0 and fixtures 5/0; PowerShell 5.1 and 7 each 270/0;
Python review tools 3/0; art lifecycle 18/0 and 12 requests valid; 79-check queue/plan valid;
both project skills valid. Handoff and current Markdown references checked. No game run or install.

## Human test steps

1. Open the review report and current dashboard; confirm they reflect the feature-first mod vision.
2. Run `python tools/review/audit.py --output results-local/offline/audit.json`; inspect the
   failed-check entries and their raw receipt links.
3. Run `pwsh -NoProfile -File tools/handoff/Get-SolPrompt.ps1 -Lane Orchestrator -NoClipboard`;
   confirm current worktrees/preview are listed and no game is started or clipboard changed.
4. After preview playtest ends, schedule affected game checks through the existing verifier.
   Full gore/HUD/trunk/material acceptance remains on its own task cards.

## Remaining research/runtime work

[Research program](../research/RESEARCH_PROGRAM.md) orders crash/cost attribution, owned-vehicle
transactions, multi-geometry/collision and deeper audio/UI capabilities. No guessed engine
addresses or offsets were introduced. Only the owner marks gameplay cards DONE.
