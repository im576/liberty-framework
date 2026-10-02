---
name: liberty-research
description: Investigate GTA IV CE engine behavior or adapt existing mod source into Liberty, with versioned evidence, narrow experiments and SDK integration.
---

# Liberty engine research

Read the assigned gameplay question and `docs/research/RESEARCH_PROGRAM.md`. Deeper research
supports concrete gore, vehicle, content, audio or UI capabilities; breadth alone is not progress.
Check existing source, research and lane branches before rebuilding an already working mechanism.

For each investigated binary/source archive record SHA-256, version/commit, source path, license
evidence and the precise function/data consumer. Use local downloaded source where available.
`third_party/README.md` records reuse decisions. LVS's pinned local MIT source can supply selected
algorithms/data, with its notice; do not import its entire polling/runtime architecture. Closed mods
provide observations and disassembly leads, not automatic permission or source-equivalent certainty.

Follow the producer and consumer of a value. Strings, a hash, a decompiler type name, a guessed offset
or a successful native return cannot establish layout, threading, lifetime or visual semantics.
Identify calling convention, executable fingerprint, expected bytes and valid object lifetime before
any hook. Distinguish forensic image addresses from runtime pattern resolution. Reject mismatches.

Record claims as SOURCE OBSERVATION, VERIFIED OFFLINE, VERIFIED IN GAME, HYPOTHESIS or UNKNOWN,
with the exact evidence. Identify the smallest falsifying experiment, its control, unchanged plugins,
fixture entity, expected result, cleanup and failure stop. Expose one narrow diagnostic first;
promote it into an SDK capability only after its contract has evidence and a disable/fallback path.

Use the native core for validated hooks/data capture and SDK events/snapshots for mechanics. Keep
per-frame work bounded, no natives on the draw thread, and resources owned across unload/hot reload.
Persistent ownership needs durable operation IDs, error recovery and GTA save reconciliation;
atomic replacement of one JSON file alone cannot transact both game money and external state.

Research artifacts belong under `docs/research/` with binary/source references outside tracked code.
Do not commit game binaries, dumps, decompiled proprietary source or downloaded mod archives.
Return discoveries, their confidence, integration choices and the next unanswered question; do not
claim a feature is reliable from disassembly or a Quick probe alone.
