# Project state

Updated 2026-10-02. This is the current dashboard; history is in
[archive/PROJECT_STATE_history.md](archive/PROJECT_STATE_history.md) and the
[pre-review snapshot](archive/review-2026-10-02/docs/PROJECT_STATE.md).

## Source and objective

Primary checkout: `C:/Users/IM576/GTAIV-Reborn`, remote `im576/liberty-framework`.
The chat folder `Documents/ChatGPT/GTA4-Reborn` contains earlier research evidence, not the engine source.
Main and GitHub main were both `65e2726` at the start of this review; newer feature work also exists
on separate local branches. The installed preview is its own worktree, not main.

The owner's October 2 scope includes repository/tooling reliability, deeper engine research,
gore, vehicle ownership and the citywide visual overhaul. Current work reviews and combines local
changes into a tested source baseline; the next playable priority is being clarified with the owner.

The visual candidate v3 has 20 actual captures and a single original/target/actual comparison.
It remains NEEDS-REVIEW: startup/stall failures and night/local-light/material fidelity are open.
The prior visual session restored the exact original appearance. See the
[visual report](reports/2026-10-02-citywide-environment-overhaul.md).


## Current capabilities and limits

| Area | Current evidence | Open work |
|---|---|---|
| Phase 1 engine | Native core, SDK, event/scheduler/resource lifecycle, hot reload, content compiler and autopilot have prior in-game evidence | Full mod integration remains separate from engine completion |
| Weapons/loadout/wheel/storage | Integrated implementation and earlier scripted passes | Trunk frame budgets, real save/load, physical controller and mission compatibility |
| Gore/effects (lane C) | Exact damage hook and skeleton-collapse capability exist; lane changes remain unmerged | Recurring dismemberment crashes, cleanup and peak-cost failures; authored caps/aftermath |
| HUD (lane D) | Conservative vanilla guard; partial display/restoration observations | Correct weapon/ammo fixtures, clean hiding, native story text, unload and combined UI |
| Materials (lane R) | SDK 1.2 material API integrated; partial surface correlations | Effective mapping, direct glass/water/object evidence and stall-free coverage; SDK 1.3 separate |
| Vehicle ownership | External LVS integration keys Arsenal trunks to owned IDs | Selective MIT source port, durable ownership transactions and save reconciliation |
| Content/physics | Single-geometry models and collision proxies | Second geometry crashes at spawn; authored collision and deeper renderer/audio/frontend control unproven |
| Test host | Startup deadline/telemetry and audio preflight exist | October 2 review hardens resume evidence and tests; host fixes are not gore fixes |

## Installed owner preview

The October 1 preview is `GTAIV-Reborn-feature-preview`, branch `codex/feature-preview-2026-10-01`,
installed source `36901ab`. Weapons/gunplay/loadout/wheel/existing trunk/DevTools; combat and atmosphere
disabled, vanilla HUD, density OFF. Prior full functional smoke: 87 steps, zero failures/log errors;
four captures reviewed; file verifier 1020/0 and 44 installed package hashes checked.
Controls: `docs/testing/FEATURE_PREVIEW.md` in that worktree.

The October 2 review does not replace that installation. Current ownership and lane routing are in
[workflow/ORCHESTRATOR.md](workflow/ORCHESTRATOR.md). No game slot is implied by an old prompt.

## Review and next work

[T-058](tasks/T-058-repository-review.md) tracks cleanup, tooling repairs and agent improvements.
Offline review/repairs complete: fresh build zero errors, verifier 441/0/5 not-run, PowerShell 5.1/7
each 270/0, native/content/art/queue checks pass. Resume/restore evidence, dependency ordering,
startup tests, fingerprint scanning and handoff generation are repaired; the preview is unchanged.
[Review report](reports/2026-10-02-repository-review.md) records scope, findings, tests and limitations.
[Research program](research/RESEARCH_PROGRAM.md) connects engine questions to controlled experiments
and concrete gore/vehicle outcomes. Historical test statistics describe multiple versions, not current acceptance.

Entry points: [task queue](tasks/README.md), [engine](architecture/ENGINE.md), [SDK](sdk/README.md),
[tools](../tools/README.md), [design](design/STAGE1.md), [third-party sources](../third_party/README.md).
