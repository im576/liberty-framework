# Codex continuation — current launch brief (2026-10-01)

Use this brief with the lane prompt and LIVE STATE. It replaces older known-state snapshots and obsolete work
requests in those prompts. The current main copy is authoritative; a lane's old copy is not sufficient.
Rules remain in RULES.md and AGENTS.md. Preparation itself launched no workers; the subsequent owner-authorized
replacement dispatch is recorded in [AGENT-ROSTER.md](AGENT-ROSTER.md).

## Current owner priority and resumed work

The owner explicitly prioritizes all developed/in-progress gameplay feature modules before the remaster: weapons,
gunplay/reticles, physical loadout, wheel, trunk/storage, gore/combat effects, functional HUD/menus and supporting
systems. Produce a combined feature test build with compatibility/performance evidence and clear controls; environment
remaster, texture deployment and atmosphere polish come later. Functional UI is not deferred with visual UI polish.
Art direction can continue independently without delaying feature repairs. "Fix that and then resume" revokes the
temporary no-build/no-edit hold. Resume preserved patches, not fresh implementations. Models/efforts remain as dispatched.
Heavy builds and game tests still require a scheduled slot; lightweight offline work may proceed concurrently.

## Ownership and first milestone

| Role | Existing worktree / branch | First reviewable result |
|---|---|---|
| C | GTAIV-Reborn-lane-c-t048 / stage1/T-048 | Review c3e2b16 and the unfinished d96aaf2 changes; identify a supported crash/performance hypothesis and repair the effects cleanup fixture without weakening cleanup requirements. |
| D | GTAIV-Reborn-lane-d / codex/T-049-hud-continuation | A bounded DISPLAY_HUD / DISPLAY_RADAR experiment with readable captures and explicit results for mission/help/subtitle text, vanilla fallback and config-off restoration. |
| R | GTAIV-Reborn-research-t050 / research/t050-material | Full SDK 1.2/material validation; report glass, water and object-hit correctness honestly. Keep the SDK 1.3 research worktree unchanged in this milestone. |
| B | GTAIV-Reborn-lane-b2 / codex/lane-b-validation | Preserve merged wheel/trunk/text acceptance; review/integrate only isolated 1f6f501 for watcher checks and fresh full wheel validation. Assigned in the replacement dispatch. |
| Orchestrator | GTAIV-Reborn / main | Shared host-tool fixes, launch readiness, test scheduling, review, integration and status. |

These are exclusive worktree assignments, not separate game installations. Do not write in another worker's worktree.
R owns the t050 worktree for this milestone and reads GTAIV-Reborn-research's live handoff/history; its material API is
already on main. GTAIV-Reborn-research / research/stage1 stays unchanged until the SDK 1.3 assignment. The orchestrator worktree
at 9be5315 contains the unmerged config change 1f6f501: do not copy or merge it incidentally.

## Shared readiness and test scheduling

- Before edits, check the previous worker stopped, preserve dirty/untracked work, review the real diffs and receipts,
  and update the lane's live handoff. Fetch and integrate current main into the lane before testing so the audio recovery,
  game lock, log rotation reader and isolated lock self-tests are present. Review conflicts; never reset lane history.
- Coding/review can run in parallel. Stagger heavy builds on this 8 GB PC. Only one game-test batch is scheduled at a
  time; other workers continue offline work rather than queue repeated verifiers. The machine-wide mutex remains the
  final authority. Missing holder notes do not prove it is free. Recheck state before installing.
- Before the first gameplay batch, the orchestrator checks audio readiness and a bounded T057 heartbeat smoke.
  C's 110420-d96aaf2 result says six launches failed, not that an audio error was observed. The Sonar render endpoint
  and audio services were active at this review; that is not proof the next launch will work. Do not change OS settings,
  drivers or plugins to guess at a cause.
- Use committed builds and verify-local with -AnyBranch -NoPush -Restore -NoManual and task-specific -Only IDs.
  Quick runs diagnose; full runs establish acceptance. Do not use -AllowOtherBuild. Confirm restoration after each run.
- Run checks appropriate to the changed behavior and the required build/offline checks before handoff. Do not rerun
  the entire suite after every documentation edit or collect more game runs without a new hypothesis.

## Evidence and scope corrections

- B's 1744416 work is merged in main f823e45. Full 092349-e470758 passed assertions/budgets and all 12 captures were
  reviewed. Do not redo its completed sprite LRU/creation limits. Whole-cache unload disposal remains unproven.
- C's latest restored batch 110420-d96aaf2 had no gameplay steps (launch unavailable). The earlier 103314-0b4f558
  still has dismember CRASH and firefight/night failures. A faulting module names a location, not the cause.
  d96aaf2 changed effects cleanup from zero lights/effects to allowing 3/4: do not accept that relaxation as a fix.
  Isolate or account for ambient shooters, then prove owned effects are released against the original requirement.
  The 4 ms peak budget remains unchanged. Do not prescribe new bone/memory manipulation from old prompt suggestions
  without establishing the failure and checking the existing ADR/API evidence.
- R's SDK 1.2/material work is already merged in e547d92. Quick 104655-2ae2a31 confirms wood numerically; direct
  glass/water and object correctness remain unproven, and captures lag their targets. Full SDK 1.2 acceptance is pending.
  ColAccel is closed, not adopted; do not reinstall it or repeat that experiment. SDK 1.3 is still separate.
- D must keep the vanilla guard until clean hiding and important text preservation are proven. Its source/remote
  history may diverge: inspect both sides rather than replacing local work with the cloud branch.

## Preparation receipt (11:53 Pacific)

Prompt generator parsed successfully in Windows PowerShell; all five briefings were generated with this current brief
and live state. R and orchestrator were checked again after assigning R to t050. Tooling suite: 236 passed / 0 failed.
Full 20261001-114509-15cd2d9: package PASS, T057-audio-output-smoke PASS (one heartbeat assertion, zero failed steps/log
errors), restored from phase2-20261001-115146. This proves that startup succeeded once; intermittent crashes remain.
The tested build is clean 15cd2d9. The subsequent 485442a changes only the R assignment/docs/prompt generator, with
no gameplay/SDK/config/runtime-tool source changes. No replacement worker was launched and no lane source was edited.

C's earlier failed launch batch also has Windows Application access violations at 11:11:52, 11:14:10 and 11:16:26,
module unknown, offsets 0x7f471a46 / 0x7f4c16ad. They do not establish the cause. Preserved events and tooling/startup
logs are in the coordination workspace's research/integration-review-2026-10-01 directory.

## Delivery and communication

Each assignment ends at its first milestone: commit the bounded patch on the lane branch and update the live handoff
with changed paths, commit, exact run IDs/mode/build, assertions, captures actually inspected, restoration, remaining
failures and next step. Distinguish PASS, visual review and owner-only feel/controller/save-load checks. Only the owner
marks DONE; the orchestrator reviews and merges. Preserve failed evidence and stop random retries.

For workers delegated in this chat, use the agent reporting channel; the orchestrator can direct its own subagents.
Separate app tasks are messaged only when the owner authorizes it. File handoffs remain the durable record in either
case. One orchestrator owns main, shared host-tool repairs and the game schedule; lane changes to shared SDK/canvas/
engine infrastructure need a scoped coordinated assignment. Do not retire any worktree during this continuation.
