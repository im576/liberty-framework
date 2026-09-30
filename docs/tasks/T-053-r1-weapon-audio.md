# T-053 — R1 — Weapon audio injection

Status: **READY** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Can new weapon sounds (reports, actions, reloads, tails, cracks, indoor/outdoor) be injected into GTA IV's audio banks, with what format, tools and licences?

## Unlocks

The weapon audio overhaul (STAGE1 section 10 audio, Slice A/B).

## Method

Inventory the audio RPFs/banks and their formats offline; find existing community tools and check their licences (reference only if unlicensed, AGENTS rule 7); try replacing one weapon report with an original test tone and confirm in game. If impossible without deep RE, answer "Phase 3" with what was learned.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.
