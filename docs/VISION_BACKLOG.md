# Liberty Framework: master vision backlog

This is the full list of what we want GTA IV to become, broken into systems and their smaller components, for discussion and prioritisation. It started 2026-09-25 from the owner's direction: tear the game apart, make it feel new, more violent, gloomy and intense; RDR2/GTA 6-style weapons and storage; real car customisation; efficient on a low-end PC.

**Status tags:** ✅ done/working · 🟡 partial · 🔲 planned · 🔬 needs research or a spike · 🎨 needs assets (models, textures, audio, animations).

## 0. Engine core and performance (the base everything stands on)

**Why "deep optimisation" means two different things here.**

- **Our code:**
  - We can re-engineer it completely, and have started: direct natives (~200× cheaper engine reads), memory fast paths, native x86 hooks.
  - It now costs about 5 ms per frame (down from about 20), with a target of ≤ 2 ms.
  - The next step is a single *LibertyCore* scheduler plus event hooks, possibly moving the hot core to a native ASI.
- **The game engine:**
  - We can't rewrite Rockstar's renderer or streamer (closed code). We **can** reverse-engineer and hook it where it helps, as we did for skeletons.
  - Examples: budgeted population, streaming priorities, cheaper shadow and LOD paths, and patching wasteful per-frame work.
  - This is real engine work, done one measured hotspot at a time with the profilers (Ghidra, PresentMon, our CostMeter).

| ID | Component | Status |
|---|---|---|
| E-1 | Frame-budget system: every module reports µs per frame; DevTools performance page; over-budget warnings | 🟡 CostMeter logs; page 🔲 |
| E-2 | Direct engine access layer (verified natives, memory reads, pool iteration) extended to all hot paths (Arsenal 1.4 ms, gunplay setup 1.8 ms left) | 🟡 18 natives |
| E-3 | LibertyCore: one scheduler, per-frame world snapshot (player, weapon, camera, nearby peds/vehicles), time-sliced jobs | 🔲 |
| E-4 | Engine event hooks: damage/impact (exact hit position, bone, weapon), weapon fire, ped death, vehicle enter/exit → no polling | 🔬 |
| E-5 | Adaptive density governor (peds and cars scaled by frame time) | 🔲 |
| E-6 | Engine hotspot profiling: Ghidra + sampling capture to find the game's own worst per-frame functions, then targeted hooks (population update, shadow cascades, streaming requests) | 🔬 |
| E-7 | Streaming on the SSD (junctioned `pc\data`, models, anim, textures) | ✅ 2026-09-25 |
| E-8 | Async shader DXVK (GPLAsync) and shader cache | ✅ |
| E-9 | Startup stability: Rockstar `MTLX.DLL` startup race; LVS `scripthook.dll` crash | 🔬 |
| E-10 | Native ASI core (C++) for hooks and the scheduler, with C# kept for gameplay logic, if E-2/E-3 still leave cost | 🔬 |

## 1. Weapons (RDR2/GTA 6-style arsenal)

| ID | Component | Status |
|---|---|---|
| W-1 | **Replace every GTA IV weapon with ours**: new models, stats and names on the vanilla weapon IDs, so NPCs, shops and pickups use them automatically (WeaponInfo.xml + IMG models) | 🔬🎨 |
| W-2 | Weapon catalogue: families, calibres, ammo types, fire modes, tiers | 🟡 catalogue v1 |
| W-3 | Physical weapon records (instance, finish, attachments, progression) persisted through carry, trunk, safehouse and death | ✅ |
| W-4 | Limited carry (sidearm, long guns, melee) with realistic slots | ✅ (edge bugs open) |
| W-5 | **Visible weapons on the body: strapped, not floating**. Per-weapon attach points on real bones (spine, thigh, chest), a sling/strap mesh, holster models, idle sway from movement, clean hide/show when entering vehicles and cutscenes | 🟡 floating props; straps 🎨 |
| W-6 | Attachments that are visible and change handling (grips, stocks, optics, suppressors, lights) | 🟡 stats only; models 🎨 |
| W-7 | Gunsmith: buy, customise, finishes/camos, tune | 🟡 |
| W-8 | Weapon condition (dirt, wear, jams), cleaning | 🔲 |
| W-9 | **Weapon audio**: punchy shots, distance tails, interior reverb, reload/handling foley, suppressed variants | 🔲🎨 |
| W-10 | Muzzle flash, smoke, shell ejection, tracers tuned per weapon | 🔲 |
| W-11 | Ballistics: penetration (doors, cars), per-calibre damage, headshot rules | 🔬 |
| W-12 | NPC loadouts that fit the new arsenal (gangs, police tiers) | 🔲 |

## 2. Aiming and gunplay feel

| ID | Component | Status |
|---|---|---|
| A-1 | Free aim, real spread, recoil patterns, crosshair | ✅ |
| A-2 | Shoulder swap | ✅ |
| A-3 | Camera feel: aim-in easing, FOV per weapon, recoil camera kick, subtle sway | 🟡 |
| A-4 | Hit feedback: impact sounds, hit markers (optional), victim reactions per region | 🟡 |
| A-5 | **Euphoria/NM reactions**: stumbles, clutching wounds, writhing, shot-while-running falls | 🔬 |
| A-6 | Injury states: limping on leg hits, dropping the weapon on arm hits, crawling survivors | 🔬 |
| A-7 | Cover and blind-fire improvements, lean | 🔬 |
| A-8 | Melee and executions (brutal finishers, knife) | 🔬🎨 |
| A-9 | AI combat: flanking, suppression, retreat, surrender, reacting to gore | 🔬 |

## 3. Gore and violence

| ID | Component | Status |
|---|---|---|
| G-1 | Bone-collapse dismemberment (limbs, head) via engine hook | ✅ |
| G-2 | Thrown limb (victim clone) that lies on the ground, never floats or flashes | 🟡 fixed, awaiting test |
| G-3 | **Stump caps**: original gore meshes on neck, shoulder, elbow, hip and knee | 🔲🎨 |
| G-4 | Exact impact wounds at the bullet position (needs E-4) | 🔬 |
| G-5 | Blood streams, leaks, pools and wall splatter (Violent Liberty companion plus ours) | 🟡 |
| G-6 | Weapon-dependent gore (shotgun close range, sniper, explosions → multiple parts) | 🟡 |
| G-7 | Bodies persist longer; blood trails when dragging or crawling | 🔬 |
| G-8 | Vehicle impact gore (run-over) | 🔬 |

## 4. Storage, inventory and interaction UX

| ID | Component | Status |
|---|---|---|
| S-1 | Trunk, safehouse and owned-vehicle storage with ownership rules (busted: lose everything; death: owned weapons go to the last safehouse) | ✅/🟡 (safehouse-on-death bug) |
| S-2 | **Weapon-wheel-style storage UI** that looks like GTA IV (radial, IV fonts and colours, weapon icons from the game's own HUD textures) replacing the panel | 🟡 installed, needs playtest |
| S-3 | **Niko animations**: opening/closing the trunk, reaching in, holstering and slinging (from IV's own animation dictionaries; research which exist) | 🟡 trunk installed (`amb@car_stash`, `car_boot`); holster/sling 🔬 |
| S-4 | Physical interaction prompts in IV's style (contextual, minimal) | 🟡 storage prompt restyled |
| S-5 | Weight/space per container (trunk size per vehicle class) | 🔲 |

## 5. Vehicles

| ID | Component | Status |
|---|---|---|
| V-1 | LVS CE base: ownership, garages, tuning, extras labels | ✅/🟡 |
| V-2 | **Real exterior modifications**: bumpers, skirts, spoilers, hoods, exhausts, wheels as swappable model parts (new fragments/extras per car) | 🔬🎨 |
| V-3 | Wheel swaps and tyre profiles | 🔬 |
| V-4 | Paint system (colours, finishes, pearls, liveries) | 🟡 LVS |
| V-5 | Performance tuning with a visible/audible effect (engine sound, handling) | 🟡 |
| V-6 | Vehicle wear, dirt and damage persistence | 🔲 |
| V-7 | Workshop UI in IV style (shared design with S-2) | 🔲 |

## 6. World, atmosphere and mood (gloomy, wet, cold, intense)

| ID | Component | Status |
|---|---|---|
| M-1 | **Liberty Mood timecycle**: cooler, desaturated grading; denser fog on grey days; readable nights (dark fix) | 🔲 |
| M-2 | **Weather director**: mostly overcast, drizzle, rain and fog, cold fronts, persistent days | 🔲 |
| M-3 | Cold details: breath vapour (`ped_breath`), steam from vents/manholes, people hunched in the rain | 🔲 |
| M-4 | Wet details: puddles, drips, wet peds, wiper/rain audio | 🔬 |
| M-5 | Night: fewer but harsher lights, deeper shadows that stay readable | 🔲 (with M-1) |
| M-6 | Population mood: more homeless, police presence, sirens, street violence events | 🔬 |
| M-7 | Ambient audio: rain on surfaces, distant sirens, city hum | 🔬🎨 |
| M-8 | Optional texture upgrades (AI-upscaled selected surfaces) only after measuring VRAM/streaming | 🔬 |

## 7. UI, HUD and presentation

| ID | Component | Status |
|---|---|---|
| U-1 | One visual language for all LF UI, matching IV (fonts, colours, animation timing) | 🔲 |
| U-2 | Minimal HUD options (hide radar in interiors, contextual health/ammo) | 🔲 |
| U-3 | Notifications in IV style | 🔲 |
| U-4 | DevTools stays developer-only; player-facing menus are separate | 🟡 |

## 8. Progression and economy

| ID | Component | Status |
|---|---|---|
| P-1 | Weapon ownership, buying and gunsmith economy | 🟡 |
| P-2 | World-based progression (unlocking dealers, contacts, gear tiers) | 🔲 |
| P-3 | Prices and money sinks that fit a tougher world | 🔲 |

## 9. Content pipeline and tooling (so we can make all of this efficiently)

| ID | Component | Status |
|---|---|---|
| T-1 | IMG/RSC/WTD read/write (finishes pipeline) | ✅ |
| T-2 | **Model pipeline**: import/export WDR/WFT (weapons, stump caps, body parts, straps) with an open tool chain; validate in game | 🔬 |
| T-3 | Animation research: list IV's animation dictionaries, test playback of trunk/holster/interaction anims | 🔬 |
| T-4 | Timecycle tool: generate and verify Liberty Mood from FusionFix's timecycle, with live reload (F3) | 🔲 |
| T-5 | AI texture workflow (Real-ESRGAN ncnn-vulkan on the RX 570) with a VRAM budget report | 🔬 |
| T-6 | Offline verifier (374 checks) and install/rollback tooling for every change | ✅ |
| T-7 | Asset licensing ledger: every reused asset is credited and permitted | ✅ third_party/README |

## Open decisions to talk through

1. **Weapons (W-1):** where do the replacement models come from? The options are permissioned mod packs, commissioned assets, or original low-poly models. This decides how far W-1, W-5 and W-6 go.
2. **Straps (W-5):** a separate strap mesh per weapon, or one generic sling that fits all long guns?
3. **Storage UI (S-2):** a radial wheel for everything (trunk, safehouse, gunsmith), or the wheel for quick access and a detail view for the gunsmith?
4. **Car mods (V-2):** which cars first (a short list of 5–10), and should parts be ours or from permissioned packs?
5. **Mood (M-1/M-2):** how dark are nights, how often is it grey or wet, and are cold fronts rare events or a whole season?
6. **Violence tone:** how far do executions, NPC suffering and gore go? This sets A-5, A-6, A-8, G-4 and G-7.
7. **Architecture (E-3/E-10):** continue in C# with direct engine access, or start the native ASI core now?
