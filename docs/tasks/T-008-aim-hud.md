# T-008 — Per-weapon free aim and HUD spike

Status: **BLOCKED on T-007**. Scope: test a reversible targeting/HUD technique on one proven test weapon. Preserve vanilla aim and reticle when returning to a vanilla weapon; restore original settings after error/unload.

Record exact wrapper/native or hook in [NATIVES.md](../game-api/NATIVES.md) or [MEMORY.md](../game-api/MEMORY.md), with CE evidence. Human compares both weapons while aiming at empty space and targets, then switches during cover, vehicle use, death/reload, and mission state. No guessed global setting or duplicate custom reticle. Agent sets `NEEDS-PLAYTEST` only after a runnable spike and exact steps.
