# T-056 — Stage 1 performance pass (software only)

Status: **READY after lanes A-C merge** · Lane P · Depends on: T-041 to T-048 on main · Design: STAGE1 section 10
(all pillars), baseline [2026-09-30-stage1-baseline.md](../reports/2026-09-30-stage1-baseline.md), research
`docs/research/OptimizationMods.md`

## Goal

Make the whole mod cheap and spike-free on the owner's PC without removing features, without thinning the city
(owner decision: density governor off) and without hardware changes. Target: Stage 1 script cost well under the 3 ms
budget in the worst-case scene, no module tick above 5 ms, and fewer frames over 100 ms than the T-040 mod-on baseline.
GTA IV's own frame time is out of scope (Phase 3: profiling and patching the game's hot spots).

Owner PC (measured 2026-09-30): Ryzen 3 2300X 4C/4T, one 8 GB DDR4-3000 stick (single channel, ~0.3 GB free while
playing), RX 570 **4 GB**, game on SSD, pagefile on HDD. Lanes keep their own budgets while they build; this task is
the cross-module pass afterwards.

## Scope (measure each change against the baseline; keep only what helps)

1. **Profile first:** per-module and per-section costs (`costs`, `framestats`) in `stage1-worst-case` and the capture
   scenarios; allocation rate and .NET GC pauses; list the top ten costs and spikes before changing code.
2. **Batch game reads in the C++ core:** extend the per-frame snapshot (camera, current weapon, ammo, aim state, player
   flags) so gunplay, arsenal, reticles and HUD read the snapshot instead of calling natives through ScriptHookDotNet
   (~0.15 ms per call). Extend the direct-native table only with read-only natives already verified (ADR-0006).
3. **Event-driven:** gunplay and arsenal do near-zero work when not aiming/firing/changing inventory; add a
   weapon-switch event derived from snapshot changes if nothing else provides it.
4. **Time-slicing:** every queue (gore hits, effects, cleanup, inventory reconciliation) processes a bounded amount
   per frame through the scheduler; no single tick > 5 ms.
5. **Simulation LOD and a significance manager:** effects, bodies, blood and wounded-ped behaviour ranked by distance and
   visibility; full detail near the player, reduced or dormant far away; hard caps from config.
6. **Dirty flags and caching** for reticle/HUD layout and anything recomputed every frame from unchanged inputs.
7. **No per-frame garbage:** reuse lists and buffers in hot paths; confirm with the allocation measurement.
8. **Pre-loading:** request models/animations/textures before they are needed (approaching a car, drawing a weapon).
9. **Frame pacing:** evaluate the FusionFix frame limiter (steady cap) and the DXVK settings one at a time, using the
   protocol in `docs/research/OptimizationMods.md`; record results, change nothing the owner did not approve.
10. **Other mods' cost:** measure Violent Liberty and LVS on/off in the worst-case scene (report only).

## Acceptance

- Worst-case scene: `engine.frame` Stage 1 cost ≤ 3 ms average (target ≤ 1.5 ms), every module tick ≤ 5 ms, frames over
  100 ms below the mod-off baseline + 25%; capture points: p95/p99 within the STAGE1 limits vs mod-off.
- No feature removed, density unchanged, all Stage 1 scenarios still pass.
- A report in `docs/reports/` with before/after numbers per change and the owner-side recommendations (pagefile on the
  SSD, background apps, Steam overlay, Defender exclusion), which only the owner applies.

## Human test steps

Fill in when done.
