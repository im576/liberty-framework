---
name: liberty-feature-delivery
description: Specify or deliver a Liberty mod feature across gameplay, framework dependencies and integration, with explicit player behavior and acceptance evidence.
---

# Deliver a connected mod feature

Locate the owning mod's current product brief, AGENTS.md and PROJECT_STATE.md.
Distinguish planning from implementation authorization. A draft brief or task marked
READY does not dispatch agents, authorize a game installation or lift a user pause.

For a brief, describe observable player behavior, appearance/feel, retained existing
features, exclusions, state transitions and acceptance. Label unresolved artistic
choices separately from unverified engine capabilities. Put mod decisions in the
mod repo; put reusable mechanisms and engine discoveries in Liberty Framework.

Inspect existing main and candidate branches before planning replacement work.
Separate what exists in main, what exists only in an unaccepted candidate, and what
needs research. A compiled module is not evidence that the promised experience works.

Agree shared state before dividing implementation: identify each state's producer,
consumers, durable identity where needed, version/invalidation rules and resource
owner. Derive presentation from the authoritative gameplay state; do not build a
second approximate simulation for HUD/reticles. Document proposed contracts as
proposed until their source and behavior have been verified.

Once implementation is authorized, scope workers by owned files and deliverables.
Independent work may advance in parallel; coordinate shared APIs and keep one game
slot. Do not impose a feature-by-feature owner playtest sequence on a user who wants
an assembled candidate. Integration checks are checkpoints, not separate products.

Use the existing evidence workflow for focused offline checks and bounded runtime
checks when warranted and authorized. Inspect first failures before retries. Report
implementation, offline evidence, runtime evidence and owner feel approval separately.
Include disable/unload restoration, failure behavior, cleanup and measured cost in
acceptance. Leave a clear next experiment for every unresolved engine blocker.

Do not declare a feature finished solely because its own checks pass: verify its
affected interactions in the assembled mod. Never turn an unknown capability into
a promise or silently remove the owner's requested feature to make a gate pass.
