# Research notes

Current work: [RESEARCH_PROGRAM.md](RESEARCH_PROGRAM.md), updated 2026-10-02. Engine research now includes
local source, offline binary analysis and runtime evidence; each note's date and confidence matter.
[SUMMARY.md](SUMMARY.md) is the original September 24 web-research snapshot, not the current capability list.

## Confidence tags (used in every research note)

| Tag | Meaning |
|---|---|
| **[SOURCE]** | Read directly from the project's own repo, README, source file, or official page. |
| **[SECONDARY]** | From a guide, forum, mod page summary, or search result. Probably right; re-check before relying on it. |
| **[HYPOTHESIS]** | Our inference. Must be proven by a spike/playtest before code depends on it. |
| **[VERIFIED]** | Confirmed in-game by the human tester. Add date + task ID. |

When a spike confirms or disproves something, **edit the note** and change the tag.

## Index

| File | Topic |
|---|---|
| [SUMMARY.md](SUMMARY.md) | Research deliverable (handoff Â§48) â€” read this first |
| [GameVersionAndLoaders.md](GameVersionAndLoaders.md) | Exe versions, loaders, script hooks, what runs on CE |
| [FusionFix.md](FusionFix.md) | FusionFix features, config, source layout, overlap with us |
| [LibertyTweaks.md](LibertyTweaks.md) | Liberty Tweaks features + techniques (reference only, no license) |
| [RealRecoil.md](RealRecoil.md) | Real Recoil / Real Recoil Enhanced CE / other recoil mods |
| [WeaponData.md](WeaponData.md) | `weaponinfo.xml`, weapon type IDs, spare slots, flags |
| [WeaponAvailability.md](WeaponAvailability.md) | What a script can control about weapon availability (T-041) |
| [ControllerAndAimAssist.md](ControllerAndAimAssist.md) | Controller, aim assist, response curves |
| [HudAndCrosshair.md](HudAndCrosshair.md) | Reticle, health ring, custom crosshair options |
| [Mafia3Combat.md](Mafia3Combat.md) | Why Mafia III combat feels good, tools |
| [OtherReferences.md](OtherReferences.md) | Max Payne 3, GTA V, GDC talks, other CE mods |
| [Raycast.md](Raycast.md) | The game's physics line test, what is verified, engine raycast design (ADR-0008) |
| [RESEARCH_PROGRAM.md](RESEARCH_PROGRAM.md) | Prioritized engine experiments, source adaptation and evidence contract |
| [HitMaterial.md](HitMaterial.md) | Material consumer, mapping hypotheses and coverage limits |
| [Dismemberment.md](Dismemberment.md) | Historical collapse/clone approach; current C branch extends it |
| [Collision.md](Collision.md) | Collision formats and proxy limits |
| [ModelFormat.md](ModelFormat.md) | Drawable layouts and content writer evidence |
| [Performance.md](Performance.md) | Recorded costs and stalls; date-bound observations |
| [ViolentLiberty.md](ViolentLiberty.md) | Local companion inspection and coexistence boundary |

## Template for a new project entry

```text
### <Project name>
- Source URL:
- Game version:
- Dependencies:
- Source available?:
- License:
- Relevant features:
- Technique used:
- Can reuse?:            (yes / only under GPL / no)
- Can reference?:
- Compatibility concerns:
- Potential value:
- Confidence:            [SOURCE] / [SECONDARY] / [HYPOTHESIS] / [VERIFIED]
```
