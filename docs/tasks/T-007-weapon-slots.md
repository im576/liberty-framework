# T-007 — Separate gold weapon identifier spike

Status: **READY**. T-001's runtime and post-install vanilla pistol checks passed. Scope: investigate candidate unused/episodic weapon slots and prove at least one can coexist with its vanilla counterpart. Extend to three only if the first is stable. No finished gold art or custom gunplay.

Record source and license for temporary assets, exact game identifiers, data override path, API-visible identity, and behavior in on-foot, cover, vehicle, mission, and save/load conditions. If no safe slots exist, return a documented alternative analysis. Human test steps must enumerate spawn method and observations once actual APIs are known. Update [ADR-0002](../architecture/decisions/ADR-0002-weapon-slots.md). Agent sets `NEEDS-PLAYTEST` after build.
