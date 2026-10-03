# Liberty Framework — current state

Updated 2026-10-02. This is the authoritative framework dashboard.

## Repositories

- Framework: `C:/Users/IM576/GTAIV-Reborn`, existing `im576/liberty-framework` remote.
- Showcase: [Liberty+](https://github.com/im576/liberty-plus), `C:/Users/IM576/LibertyPlus`, GitHub repository `im576/liberty-plus`.
- Chat folder: earlier research/raw evidence; preserved, not the source checkout.

[Repository ownership and compatibility](architecture/REPOSITORIES.md) defines the boundary.
Reusable SDK/native/UI services, tools, skills and GTA knowledge live here.
Gameplay, HUD design, generated artwork and tuning live in Liberty+.

## Capabilities and limits

Native core, snapshots, damage observation, scheduler/events/resource lifecycle,
SDK 1.2, streaming, canvas/menus, content tools and autopilot exist. The framework
builds without Liberty+ and does not depend on its gameplay types or assets.
Liberty+ is presently an extracted privileged legacy client, not entirely SDK-only.

Research gaps remain: skeleton lifetimes/crashes, richer geometry/collision,
material mapping, precise wound/decal placement, blood pool control, clouds,
audio/frontend and selective HUD hiding. [Research priorities](research/RESEARCH_PROGRAM.md)
serve the first mod; broad engine replacement is later and unproven.

## Evidence and preserved state

T-061 splits the source without installing or launching the game. Preliminary
standalone framework and Liberty+ builds PASS; framework-only verifier 135/0/4
NOT-RUN; clean combined verifier 432/0/7 NOT-RUN; original tooling suite 312/0.
Final staged combined verifier 441/0/5 NOT-RUN; PowerShell 7 and 5.1 each 317/0;
separate assembly boundary 17/0, preservation audit 82/0, and 45 package hashes
verified. No checks were dropped from the combined verifier. Final evidence is in
[the split report](reports/2026-10-02-repository-split.md).

The installed owner preview remains source `36901ab` from the preview worktree.
Original timecycle files remain restored. No split-build game acceptance exists.
All ten original worktrees and unmerged candidate histories are preserved.

## Work and rules

[T-061](tasks/T-061-repository-split.md) tracks this extraction. The owner authorized
repository separation and verification, not development of unfinished showcase
features. Do not start feature agents or game runs without a current assignment.
Read AGENTS.md; game ownership is in [ORCHESTRATOR](workflow/ORCHESTRATOR.md).
The pre-split dashboard/schedule/schema are under `docs/archive/pre-split/`.
