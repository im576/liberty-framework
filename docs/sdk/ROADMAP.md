# Framework capability roadmap

Current index after the Liberty+ split. SDK code is 1.2.0; engine runtime version
is independent. Historical milestone status/evidence is preserved in
[the original roadmap](../archive/pre-split/SDK_ROADMAP.md), not current acceptance.

| Area | Present capability | Remaining limits |
|---|---|---|
| Runtime/SDK | Modules, dependencies, events, scheduling, ownership, configuration/state, streaming and profiling | Full-trust guardrails are not a sandbox; native crashes remain possible |
| Native core | Invocation, validated reads/hooks, snapshots, damage events, ray queries and crash capture | Game version/thread/object lifetime constraints; exact damage is not precise wound UVs |
| Queries/materials | Ray/line tests and numeric hit material in SDK 1.2 | Effective mapping and direct glass/water/object coverage incomplete; SDK 1.3 candidate separate |
| Generic UI | Canvas, textures, lists/radials, input routing and visibility/restoration tools | Selective native HUD/story behavior still needs proof; Liberty+ owns custom HUD design |
| Content | Compilers/read-back tools, Blender authoring and single-geometry examples | Multi-geometry crashes, richer LOD and authored collision remain limited/unproven |
| Development | Autopilot, build caching, receipts, offline verification and hot reload | Hot reload off in current profile; fresh combined runtime and owner review remain separate |

Research order: capabilities needed by Liberty+ first, broader engine exploration
after the showcase. See [research program](../research/RESEARCH_PROGRAM.md),
[current status](../PROJECT_STATE.md) and [ownership](../architecture/REPOSITORIES.md).
