# Proposed next milestone and agent plan

2026-10-02. These priorities are recommendations awaiting the owner's answers, not dispatched assignments.
Current source integration is tracked by [T-060](../tasks/T-060-local-integration.md).

## Deliver one reliable playable slice

Start with the existing weapons/loadout/wheel/trunk preview and the prepared visual candidate. Establish
repeatable startup and restoration, then test the combined source with the original acceptance thresholds.
Keep combat/atmosphere disabled in the preview profile until their individual and combined gates pass.
The lighting candidate is generated/installed separately; merging its source does not install its tables.

The first owner playtest should cover walking/driving between two locations, combat with the existing
weapon catalog, switching the 2+1 loadout, storing/retrieving a weapon, and an actual game save/reload.
Use one current build/config/plugin receipt and record controller, mission and visual limitations explicitly.
The owner chooses whether visuals, gore or vehicle ownership takes priority after this baseline.

## Sequence

1. **Source integration:** reviewed preview/mood and B radial/diagnostic work join the tooling fixes.
   Build the engine; run offline verifier, both PowerShell versions, radial and metrics harnesses,
   generator checks and queue validation. Keep experimental C/D/R code on its preserved branches.
2. **Runtime baseline:** schedule one bounded readiness attempt with a fresh journal and exact restoration.
   A launch failure is investigated at its first failed stage; do not repeat an unchanged batch. Once startup
   is available, run a short combined smoke, then the affected full wheel/trunk and config-lifecycle gates.
   The historical trunk p95/p99 failure remains unresolved even if the new preview disables combat.
3. **Visual iteration:** use the approved references and matched actual captures. Separate global color/lighting
   from cloud shapes, material textures and local lights. Record one change family per candidate and inspect
   street-level moving scenes, interiors and weather transitions in addition to the existing static views.
4. **First deeper engine experiment:** distinguish the C setup/scene crash from the actual sever operation.
   Complete the matched control/active setup pair before assuming skeleton changes cause a crash that occurred
   before a scripted cut. Then instrument one ped/cut with object generation, skeleton pointers, counts and
   release order. Require cleanup and original performance gates before promoting gore to the combined preview.
5. **Vehicle ownership:** choose the required features, selectively adapt the licensed LVS behavior, and build
   durable ownership/purchase transactions with fault tests. A purchase, storage and actual save/load round-trip
   precedes broader garages, insurance, dealerships or economy work.
6. **HUD/materials/assets:** promote only the specific contracts each next feature needs. Vanilla HUD remains
   until hiding/restoration and native story text are demonstrated. Material names require effective-table and
   visible hit-target evidence. Multi-geometry/collision research becomes priority when required for chosen assets.

## Small team, one shared game

Recommended initial team: one integrator and two workers. No new agents have been created for this proposal.

| Role | Owns | Deliverable and boundary |
|---|---|---|
| Integrator | Main, test host, shared SDK contracts, schedule and owner playtest | Reviews patches/evidence, runs the sole game/install slot, combines accepted changes |
| Gameplay/engine worker | One chosen blocker, initially setup/gore lifetime or vehicle transactions | One hypothesis or vertical slice, bounded files, focused tests; no speculative unrelated hooks |
| Visual worker | Mood generator/config, capture fixtures and later selected assets | Matched baseline/candidate evidence against approved targets; coordinates host changes with integrator |

Start each worker from the reviewed integration commit, in a separate worktree. Do not give both ownership of
shared host/SDK/UI files. Workers request the game slot through the integrator; heavy builds are serialized.
Every handoff includes source commit, changed behavior, actual check status, first failure and the next experiment.
Create or resume threads only after the owner chooses the team; do not dispatch from this document automatically.

## Questions for the owner

- Which next milestone comes first: stable gameplay plus visuals, severe gore, or vehicle ownership?
- Describe the ideal five minutes of gameplay. Which three new behaviors must be present?
- How far should gore go: realistic wounds, severed limbs, persistent aftermath, or a specific combination?
- Which ownership features matter first: persistent personal cars, trunks, garages, dealerships, fuel/repairs,
  insurance/recovery, customization? Which existing mod behaviors should be retained or changed?
- Do the approved visual references remain the target, and what resolution/frame-rate goal should guide tradeoffs?
- Must existing saves and the full story remain compatible, or should early experiments use a separate modded save?
- Prefer one agent, or the proposed integrator plus two workers? Should research stay tied to the next feature,
  or receive a larger dedicated effort before more gameplay?

Detailed experiment/provenance requirements: [research program](../research/RESEARCH_PROGRAM.md).
