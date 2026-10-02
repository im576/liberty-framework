# Preserved agent roster

Current scheduling lives only in `docs/workflow/ORCHESTRATOR.md`. This is a historical index to
existing workers, not an instruction to dispatch or message them. Check their actual current status.

| Lane | Worker | Thread | Model at dispatch | Checkout | Initial assignment (historical) |
|---|---|---|---|---|---|
| B | Planck | 01a0f8db-b3e1-7c90-9b11-53d78a4f6514 | gpt-6.1-sol / medium | GTAIV-Reborn-lane-b2 | Review/integrate only isolated 1f6f501 after current main; verify watcher lifecycle offline and propose fresh full wheel validation. |
| C | Herschel | 01a0f8db-a498-79a3-8084-34158f84bb49 | gpt-6.1-sol / high | GTAIV-Reborn-lane-c-t048 | Repair cleanup fixtures without accepting relaxed thresholds; review crash/performance evidence and propose a supported next experiment. |
| D | Archimedes | 01a0f8db-a65b-7ea2-b3fd-9c68745f2269 | gpt-6.1-sol / medium | GTAIV-Reborn-lane-d | Prepare bounded HUD hiding/text-preservation/restoration diagnostic and repair demonstrated ammo sampling defects. |
| R | Socrates | 01a0f8db-b0bb-7972-babc-3855f12d6eb2 | gpt-6.1-sol / medium | GTAIV-Reborn-research-t050 | Improve material target/position fixtures and prepare full SDK 1.2 coverage; keep SDK 1.3 research read-only. |

[Original dispatch](../../archive/review-2026-10-02/docs/handoffs/sol/AGENT-ROSTER.md).
Reuse their preserved source/context when asked to resume. This review dispatched no workers and
changed no model settings. Do not reproduce old milestones as a current schedule.
