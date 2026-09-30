# T-052 — R4 — Screen-effect draw cost

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

What do full-screen and many-sprite overlays cost through the draw path (SHDN `Graphics`, Liberty.Ui canvas) at 60 fps, and what do timecycle modifiers give for blur/desaturation/vignette?

## Unlocks

Rain-on-lens, blood-on-screen and injury effects (Slice B).

## Method

Build a throwaway test module: N textured sprites (10/50/200) with alpha, full-screen quads, measured with `costs` and frame p95; test timecycle-modifier natives from the registry for grading. Report cost curves and a recommended budget.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
