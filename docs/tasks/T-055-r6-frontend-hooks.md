# T-055 — R6 — Frontend and pause-menu entry points

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Where does GTA IV build its frontend/pause menu and map, and can it be hooked safely (ADR-0007 hook manager)?

## Unlocks

Informs Phase 3 frontend and map replacement; nothing ships in Stage 1.

## Method

Static analysis only (Ghidra project under D:\GTAIV-Reborn-Tools): locate the frontend update/render entry points and menu data, document candidate hook sites with byte patterns in `docs/research/Frontend.md`. No hooks installed.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
