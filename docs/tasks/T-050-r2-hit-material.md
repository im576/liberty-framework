# T-050 — R2 — Hit surface material

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Can the bullet impact or the engine line test (ADR-0008, `lc_raycast`, `docs/research/Raycast.md`) report the material of the surface hit (concrete, wood, glass, metal, flesh, water)?

## Unlocks

Material-specific impacts and impact audio (T-048).

## Method

Read the line-test result structure already traced in Raycast.md; find where the physics hit carries a material index (bounds polygon material, per `docs/research/Collision.md`) and how it maps to material names (materials.dat or equivalent in the game data). Prove it with a spike command that prints the material for rays at known surfaces, verified by screenshots.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
