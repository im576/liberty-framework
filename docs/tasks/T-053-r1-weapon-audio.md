# T-053 — R1 — Weapon audio injection

Status: **BLOCKED — Phase 3 handoff; offline spike recorded** · Research lane (parallel) · Design: STAGE1 section 8

## Question

Can new weapon sounds (reports, actions, reloads, tails, cracks, indoor/outdoor) be injected into GTA IV's audio banks, with what format, tools and licences?

## Unlocks

The weapon audio overhaul (STAGE1 section 10 audio, Slice A/B).

## Method

Inventory the audio RPFs/banks and their formats offline; find existing community tools and check their licences (reference only if unlicensed, AGENTS rule 7); try replacing one weapon report with an original test tone and confirm in game. If impossible without deep RE, answer "Phase 3" with what was learned.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

**VERIFIED OFFLINE:** No game run, installation or owner action is required by this helper. No check is queued: an original audio writer is not yet viable. Future original-tone playback procedure and its preconditions are in the research note.

## Offline result — 2026-09-29 America/Los_Angeles

- **VERIFIED OFFLINE:** [WeaponAudio.md](../research/WeaponAudio.md) and its independent [probe](../../tools/research/audio_t053_probe.py)/[evidence](../../tools/research/audio_t053_evidence.json) inventory audio archives, pin hashes, and compare the installed WEAPONS override to a uniquely matching base-bank candidate.
- **VERIFIED OFFLINE:** 396 descriptor identities preserved; 39 changed payloads; override 17,605,310 bytes versus XML slot size 13,174,784. Two override records fail the observed byte-length/sample-count relation. All metadata +8 identifiers equal their descriptor hashes; none is demonstrated as a checksum.
- **PLAUSIBLE:** Existing-sample replacement is feasible, but **UNKNOWN:** original writer correctness, codec/flag/channel semantics, report-to-sample mapping and installed loader/allocation behavior. Answer: **Phase 3**, limited to readiness, not a claim of impossibility.
- **VERIFIED OFFLINE:** [ReferenceMods.md](../research/ReferenceMods.md) is a separate author-page/license/compatibility deliverable. No community implementation or game assets were copied; no API or PROJECT_STATE changes.

## Blocked

**UNKNOWN:** Injection remains blocked by an unverified format writer and missing weapon-report/event mapping, independently of third-party editor licensing. Static parsing checked 396 payload bounds and record identities but cannot establish playback. Recorded A1–A5 in WeaponAudio.md define the remaining work. Only main may queue/run a later injection check after an original writer is viable; this helper stays offline. Task is not DONE or NEEDS-PLAYTEST.
