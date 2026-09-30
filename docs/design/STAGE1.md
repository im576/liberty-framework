# Liberty Vanilla+ — Stage 1 (first production mod)

Status: **DESIGN, approved direction (owner, 2026-09-30). Not started.** Working title: *Liberty Vanilla+ / Gunplay V2 — Stage 1*.
This document is the build plan for the first mod on the Liberty Engine. Engine facts it relies on are in
[PROJECT_STATE.md](../PROJECT_STATE.md) and the [regression report](../reports/2026-09-30-regression.md).

## 1. Goal

> What would GTA IV feel like if Rockstar remastered and expanded it today, without changing what GTA IV is?

**Keep:** the grime, the cold grey identity, physicality, melancholy, danger, density, grounded weapons, the late-2000s
setting. **Modernize:** gunplay, weapons, violence, UI/HUD, inventory, audio, ambience, visuals, effects, usability,
performance. It is not GTA V transplanted into GTA IV.

**Success test:** someone who knows GTA IV plays Stage 1 and thinks *"this still feels like GTA IV, just far more
polished, brutal, atmospheric and modern"*, never *"a pile of unrelated mods"*. Every system (graphics, UI, gunplay,
blood, inventory, audio, ambience) must read as one product.

## 2. Pillars

1. **GTA IV first.** Every redesigned system must look and feel as if it belongs in GTA IV.
2. **Harsh violence.** Graphic and grounded, never comedic. Gunshots are consequential.
3. **Physical world.** Weapons, ammunition and storage are objects, not menu entries.
4. **Dangerous Liberty City.** The city sounds, looks and reacts like a hostile place (unsafe, not horror).
5. **Performance conscious.** Designed around measured cost. *"Do not optimize Liberty City by removing Liberty City."*
   Ped/traffic reduction is the last resort.

## 3. Visual philosophy

> **Use existing rendering technology where it is already good, while creating an original Liberty Vanilla+ art pass on
> top of it.**

- "Vanilla+" is **not** "reuse vanilla/FusionFix assets and settings". It is a real remaster pass with newly authored art
  wherever that materially improves the game.
- Rendering technology (FusionFix, DXVK, the game's shaders) is reused where it already solves a problem; Liberty does
  not rewrite it. Reuse never limits the creative scope of the art.
- Target look: cold daylight, heavy overcast, a strong wet-city atmosphere, very dark but readable nights, warm artificial
  light against cold surroundings, dirty glass, wet asphalt, dense urban depth, still recognizably GTA IV.
- Not a blanket 4K pass: good materials, correct mipmaps, fixed assets and better lighting beat raw resolution.

### Asset policy

All art is **original, procedurally generated, permissively licensed (CC0/CC-BY/MIT and similar), or otherwise legally
usable**, and every non-original source is credited in `third_party/README.md`. No ripped commercial assets
(AGENTS.md rule 7). Realistic sources: procedural materials (Blender, Material Maker), CC0 libraries (ambientCG, Poly
Haven) as raw material, and hand-authored art. Hand-painted hero art (signage, graffiti, icons) needs an artist or an
explicit per-asset decision; agents can build pipelines, procedural materials and variants, not guarantee art quality.

## 4. Scope

**Stage 1 = Broker/Dukes and early-game Liberty City.** This scopes *progression, weapon availability and the
environment art pass*. Systems (gunplay, gore, UI, HUD, screen effects, audio events) are citywide by nature.

**Not in Stage 1:** full crime or police overhaul, businesses, heists, the full economy, a full vehicle ownership
overhaul, all-island progression, TLAD/TBoGT, complete reverse engineering.

## 5. Engine constraints (what Phase 1 proved)

- Authored models: **one geometry / one material per model** (texture atlases). Multi-geometry drawables crash at spawn.
- **No authored collision**: use collision proxies (hidden vanilla props, `Liberty.World` `collisionProxies`).
- **No LOD slots 1-3** in authored drawables: separate models or IDE draw distances.
- Map geometry cannot be rebuilt; the environment pass is **textures and materials on existing geometry**, plus new
  single-material props.
- No silent unsafe workarounds. A genuinely missing primitive gets the smallest safe engine capability, documented for
  Phase 3 reverse engineering.

## 6. Reuse first

Before any slice starts, audit and extend: Gunplay, Arsenal, CombatEffects, Holsters/slings, Trunk, `Liberty.World`,
the SDK, exact damage, raycast, Liberty.Ui radial/list menus (`StorageWheel`), the weapon catalog, Atmosphere/Mood
configs, the content compiler, the Blender add-on, the finishes (weapon texture) pipeline and the performance tooling.
Nothing is rebuilt because a new version would look cleaner. Stage 1 exists to prove the engine.

## 7. Build plan

Each feature lists its engine status: **Ready** (existing code or data), **Extend** (existing system, new work),
**Spike** (depends on a research item in section 8, not promised until it passes).

### P0 / Slice A — Combat & Inventory

Functional first; Slice C gives these screens their final unified look.

| Feature | Status | Notes / acceptance |
|---|---|---|
| Weapons and availability | Ready/Extend | Weapon catalog, per-weapon config, WeaponInfo. Tiers: common (cheap/service pistols, revolvers, basic shotguns, Uzi), less common (better pistols/shotguns, MP5-type), rare (AK-type, tactical by circumstance). No snipers/LMGs/P90/military gear as normal Stage 1 availability. Availability comes from the world, contacts and money, never XP |
| Gunplay tuning | Extend | Per-weapon recoil, first-shot accuracy, burst control, sustained-fire climb, stance/movement influence, caliber differences. Shoulder swap finished (T-015), not recreated. Accept: tuned numbers in `config/`, owner feel sign-off per weapon class |
| Physical weapons | Extend | Sidearm + visible long gun (second long gun: owner decision), limited equipment and ammunition; the rest in trunk/safehouse. Missing today: hide/show on vehicle entry and cutscenes, per-outfit clipping, draw/holster transitions |
| Harsh gore | Extend / Spike | Exact-damage driven (no proximity guessing): entry-hit response, bleeding, severe headshots, shotgun and limb trauma, dismemberment where reliable, budgeted persistent aftermath. Blood pools/trails/decals: **Spike R3** |
| Combat effects | Extend / Spike | Per-weapon muzzle flash, smoke, casings, sparks, night muzzle light. Material-specific impacts (concrete, wood, glass, metal): **Spike R2** |
| Weapon wheel | Ready/Extend | Liberty.Ui radial menu; shows the physical loadout (carried slots, equipped, ammo, category, finish), not every owned gun |
| Trunk UI | Ready/Extend | Loadout ↔ vehicle storage, carried vs stored, ammo, capacity. Moving a gun makes it visibly no longer carried |
| Basic Liberty HUD | Extend | Our own health/armour/ammo/wanted/prompts drawn by Liberty.Ui, contextual (shown when relevant). IV's radar stays until **Spike R5** |

### P1 / Slice B — Visual Remaster & Atmosphere

| Feature | Status | Notes / acceptance |
|---|---|---|
| Lighting, timecycle, weather | Ready/Extend | Mood timecycle generator and weather director exist; tuned to the target look |
| Broker/Dukes environment art pass | Extend (pipeline) | Texture/material overrides on existing geometry: roads, sidewalks, storefronts, signage, graffiti, glass, emissive/night textures, selected props, vegetation, visible interiors, visibly broken LOD/material assets. Needs a **texture override pipeline** (replace textures in map dictionaries via FusionFix's update-folder overloading; our WTD writer round-trips 68/68). Accept: before/after captures at fixed spots, VRAM/streaming budget met |
| Weapon materials/textures | Ready/Extend | The finishes pipeline already rewrites weapon texture dictionaries |
| Blood/wound/decal and muzzle/effect art | Extend / Spike | New textures in the particle/decal dictionaries; where they live and what IV allows: **Spike R3** |
| Screen effects | Spike | Rain on the lens (droplets, streaks, movement clearing), blood on screen (directional, severe hits), injury (restrained desaturation, vignette, blur, audio muffling). Timecycle modifiers for grading; sprites on the Liberty.Ui canvas. Cost: **Spike R4**. No Call of Duty red overlays |
| Ambient city audio | Ready | Scripted events using the game's own sounds and speech: screams after severe violence, shouting, panic, arguments, sirens, alarms, distant violence |
| Population and vehicle variety | Extend | Popgroups/neighbourhood data, clothing variations, props, contextual street activity. New ped/vehicle *models* are deferred (skinned meshes/fragments) |
| Performance-conscious art pipeline | Extend | Mipmaps, sizes per surface class, a VRAM budget report per pack, measured before/after |

### P1 / Slice C — Unified Liberty UI

| Feature | Status | Notes |
|---|---|---|
| Design language | New (design) | *"Modern functionality designed through GTA IV's visual language."* Keep: dark, grey/black translucent surfaces, restrained amber/orange/red accents, gritty type, industrial feel, sharp iconography. Modernize: hierarchy, spacing, animation, readability, navigation. No mobile UI, big rounded cards, neon, live-service or GTA Online styling |
| Weapon wheel, trunk UI, HUD, prompts | Extend | Slice A's screens rebuilt in the final language |
| Inventory/stats menu | Extend | A Liberty menu (inventory, weapons, progress, stats, settings) as its own screen; room for vehicles/properties/contacts later without showing unfinished features. **Not** the Rockstar pause menu (Phase 3) |
| Icon family | New (art) | One coherent GTA IV-inspired family for weapons, HUD, menus and later map categories |

## 8. Research track (parallel)

Research-dependent features stay in the design, but are **not promised** until their spike passes. Each spike ends with
a written answer (works / works with limits / Phase 3) and, if it works, the smallest documented engine capability.

| # | Question | Unlocks |
|---|---|---|
| R1 | Can new weapon sounds be injected into IV's audio banks (format, tools, licensing of tools)? | Weapon audio overhaul (reports, action, reloads, tails, cracks, indoor/outdoor) |
| R2 | Can the bullet impact / line test report the hit surface material? | Material-specific impacts and impact audio |
| R3 | Which decal and blood-pool capabilities does IV expose (natives, particle/decal dictionaries), and their limits? | Blood pools, trails, wall splatter, persistent aftermath |
| R4 | What does full-screen sprite drawing cost through the draw path at 60 fps? | Rain/blood screen effects |
| R5 | Can the radar be hidden and redrawn (map tiles, blips) at acceptable cost? | Custom radar |
| R6 | Where are the frontend/pause-menu entry points, and are they hookable safely? | Informs Phase 3 frontend work |

## 9. Deferred to Phase 3

Real Rockstar pause-menu/frontend replacement; real map replacement or integration; deeper audio engine control;
authored collision; multi-geometry drawables and LOD slots; deeper renderer and internal hooks.

## 10. Performance budget

Measured with the existing tooling (frame p50/p95/p99, module cost, memory pressure, streaming stalls) at fixed
Broker/Dukes capture points, against the same scene with the mod off.

- Stage 1's total script cost is part of `engine.frame`: target **≤ 3 ms average**, no module above its `moduleBudgetMs`.
- **Frame p95 no more than 10% worse** than mod-off at each capture point.
- Art packs ship with a VRAM/streaming report; no pack raises streaming stalls measurably.
- Gore/effects have hard caps (bodies kept, decals, active effects), set in `config/`.

These numbers are proposals until the owner confirms them.

## 11. Compatibility

- Story missions: mission-given weapons, cutscenes, busted/wasted and saves must work with the physical inventory
  (Arsenal already tracks mission weapons). Every Stage 1 system is disabled during cutscenes where it could conflict.
- FusionFix is the rendering baseline; DXVK is benchmarked (DX9 vs Vulkan pacing), not reimplemented.
- Liberty Vehicle Services CE is a reference and compatibility target, not a required dependency.
- Keyboard/mouse and controller both supported.

## 12. Open decisions (owner)

1. One or two long guns carried?
2. Art sourcing: which surfaces get hand-authored art (artist or per-asset decision) versus procedural/CC0-derived?
3. Confirm the performance budget in section 10.
4. Gore ceiling: executions, NPC suffering and persistence limits.
5. Order of the Broker/Dukes art pass (which streets and areas first).
