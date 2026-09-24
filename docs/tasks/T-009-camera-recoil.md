# T-009 — CE aim-camera control spike

Status: **BLOCKED on T-001 and T-007**. Scope: find the smallest supported way to apply a visible, reversible pitch/yaw impulse to the player's current aim camera on one test weapon. Start with ScriptHookDotNet wrappers, then documented natives; memory requires a separate ADR and pattern scan.

Measure controller behavior, camera recovery, cover, vehicles, blind fire, and FusionFix interaction. Keep Real Recoil disabled during isolation testing to avoid double effects. Document exact API and CE status; do not implement a full recoil engine before the camera path is proven. Agent sets `NEEDS-PLAYTEST` after build and exact test instructions.
