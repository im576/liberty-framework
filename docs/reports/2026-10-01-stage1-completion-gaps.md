# First-mod completion gaps — 2026-10-01

**Later owner correction:** gameplay features and their supporting functional UI come first; the remaster comes later.
The immediate completion target is the integrated feature test build and playtest, not visual/environment acceptance.
Art direction may continue independently. The longer-term scope below remains history/planning, not a blocker for
feature testing. The owner subsequently revoked the build hold and resumed implementation/testing.

Scope: the approved Liberty Vanilla+ Stage 1 in docs/design/STAGE1.md, including Slice A (combat/inventory), Slice B
(visual remaster/atmosphere) and Slice C (unified UI). Completing the current lanes is the Slice A milestone, not
completion of the whole approved mod. Engine completion is also distinct from mod acceptance.

## Current combat milestone

Foundation, weapon catalog, gunplay/reticles, physical loadout, wheel/trunk/text are integrated with existing automated
evidence. Owner feel/controller/real save-load and mission compatibility remain. Material SDK 1.2 is on main with full
acceptance still pending. The latest main startup smoke passed; this does not resolve intermittent crashes.

Remaining immediate gates:

1. C: reliable dismemberment, original effects cleanup behavior, full gore/effects acceptance and peak-cost failures.
2. D: clean HUD hiding without losing important text, conservative fallback, config-off restoration and coexistence.
3. R: direct glass/water/object correctness and full SDK 1.2 acceptance; review SDK 1.3 separately before adoption.
4. B: isolated config follow-up review/runtime acceptance; retain the already accepted wheel/trunk features.
5. Combined integration/regression and T-056 performance pass, then owner combat/inventory playtest.

## Whole Stage 1 beyond that milestone

The task queue explicitly leaves Slice B/C cards to be filed after Slice A's audit and relevant research. Reusable
pipelines and prepared assets are progress, not completed feature/visual acceptance.

- Slice B: tune/validate lighting, weather and atmosphere; integrate citywide texture/material overrides and the
  Broker/Dukes environment pass (Hove Beach first, eight showcase capture points); weapon/effect art, supported screen
  effects, ambient events and population/vehicle variety, with provenance, mipmaps and measured memory/streaming costs.
- Slice C: finish the common UI language, wheel/trunk/HUD/reticle/prompt styling, Liberty inventory/stats/settings
  screen and coherent production icon family. Radar remains conditional on accepted research; deeper frontend/audio
  and other Phase 3 features are outside this completion scope.
- Final whole-mod validation: all affected scenarios, missions/cutscenes/save-load/config-off compatibility, 12 capture
  comparisons, performance/VRAM limits and one-hour stability/memory soak; then owner identity/feel/visual approval.

Art registry at this review: eight prepped assets and four approved concept references; none marked integrated.
That registry is an index rather than proof of deployment, but there is no basis here to call the remaster finished.

## Estimate limits

**Updated for the current feature-first target:** see the [status and planning estimate](2026-10-01-feature-status-and-estimate.md).
It provides low-confidence allowances for a limited preview and the complete gameplay candidate; the paragraph
below is the earlier whole-Stage-1 assessment, including the now-deferred remaster.

No defensible hours/days or percentage-complete estimate yet. The crash cause and C's peak-budget feasibility are
unresolved, while Slice B/C implementation acceptance is substantial and not yet fully tasked. Report progress against
the gates above. The current combat milestone is nearer than the full mod; neither is only an owner playtest away.

Owner-only decisions still recorded in ORCHESTRATOR/STAGE1 include gunplay class targets/recoil cap, proposed VRAM
ceiling, loadout/storage defaults and HUD policy if clean hiding remains unproven. Do not silently change them or
claim the deferred remaster is finished. The owner has authorized the feature-first milestone above; preserve density
OFF and the existing T-040 baseline.
