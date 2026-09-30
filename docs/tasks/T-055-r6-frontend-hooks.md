# T-055 — R6 — Frontend and pause-menu entry points

Status: **BLOCKED — Phase 3 handoff; static spike recorded** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Where does GTA IV build its frontend/pause menu and map, and can it be hooked safely (ADR-0007 hook manager)?

## Unlocks

Informs Phase 3 frontend and map replacement; nothing ships in Stage 1.

## Method

Static analysis only (Ghidra project under D:\GTAIV-Reborn-Tools): locate the frontend update/render entry points and menu data, document candidate hook sites with byte patterns in `docs/research/Frontend.md`. No hooks installed.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

**VERIFIED OFFLINE:** None for this static-only spike. No hooks installed, game launched or runtime check queued.

## Offline result — 2026-09-29 America/Los_Angeles

- **VERIFIED OFFLINE:** [Frontend.md](../research/Frontend.md), independent [PE probe](../../tools/research/frontend_t055_probe.py), [JSON evidence](../../tools/research/frontend_t055_evidence.json), and [read-only Ghidra script](../../tools/research/frontend_t055_ghidra.java)/[log](../../tools/research/frontend_t055_ghidra_evidence.txt) pin CE executable and static candidates.
- **VERIFIED OFFLINE:** Initialization calls a selector that calls a parser comparing `sMenuDisplayValue` and `sMenuScreen`. Unique reader/parser/selector and map-body signatures found; candidate map prologue matches twice, menu-active source lead zero times. Original/override menu XML and layout inputs inventoried.
- **PLAUSIBLE:** The map-body lead identifies crosshair drawing. **UNKNOWN:** Complete pause update/render ownership, ABI, lifecycle and FusionFix coexistence. Answer: **Phase 3**; no Stage 1 frontend replacement capability is claimed.
- **VERIFIED OFFLINE:** ADR-0007/native hook manager reviewed. Relative calls are not relocated by its verbatim trampoline, and source `create_mid` sites are not entry-only hooks. Shared API and PROJECT_STATE files untouched.

## Blocked

**UNKNOWN:** A safe frontend hook requires F1–F5 in Frontend.md: complete update/render/state chains, validated ABI and thread/lifecycle, a unique semantic resolver, and coexistence with FusionFix's existing hooks. Static uniqueness alone is insufficient. The Ghidra project was processed with `-readOnly -noanalysis` using non-mutating pseudo-disassembly. No installed hooks are permitted by this spike. Task is not DONE or NEEDS-PLAYTEST.
