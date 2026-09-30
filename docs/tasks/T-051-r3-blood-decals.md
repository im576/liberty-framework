# T-051 — R3 — Blood, decals and particle textures

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Which decal and blood capabilities does GTA IV expose to scripts (decal/blood-pool natives, particle effects that leave decals, their lifetimes and caps), and where do particle and decal textures live so new art can replace them?

## Unlocks

Blood pools, trails, surface blood and persistent aftermath (T-047); new muzzle-flash, impact and blood textures (T-048, art queue).

## Method

Search the native registry and FusionFix `natives.ixx` for decal/blood/ptfx natives; inventory the particle dictionaries (`gta_core.wpfl` and related) with the existing readers; spike each candidate in game (spawn, measure lifetime, cap, cost); document which textures a texture override can replace.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
