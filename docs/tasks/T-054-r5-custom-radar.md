# T-054 — R5 — Custom radar

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Can the vanilla radar be hidden and redrawn (map tiles, rotation, blips, route) at acceptable cost?

## Unlocks

A Liberty radar/minimap (Slice C).

## Method

Find how the radar is hidden (natives, hud globals); find the radar map textures and blip data sources; prototype a minimal redrawn radar with the canvas and measure cost.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
