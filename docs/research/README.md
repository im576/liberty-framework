# Research notes

Research pass 1 was done on 2026-09-24 (web research only — nothing has been tested in-game yet).
Start with [SUMMARY.md](SUMMARY.md).

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
| [SUMMARY.md](SUMMARY.md) | Research deliverable (handoff §48) — read this first |
| [GameVersionAndLoaders.md](GameVersionAndLoaders.md) | Exe versions, loaders, script hooks, what runs on CE |
| [FusionFix.md](FusionFix.md) | FusionFix features, config, source layout, overlap with us |
| [LibertyTweaks.md](LibertyTweaks.md) | Liberty Tweaks features + techniques (reference only, no license) |
| [RealRecoil.md](RealRecoil.md) | Real Recoil / Real Recoil Enhanced CE / other recoil mods |
| [WeaponData.md](WeaponData.md) | `weaponinfo.xml`, weapon type IDs, spare slots, flags |
| [ControllerAndAimAssist.md](ControllerAndAimAssist.md) | Controller, aim assist, response curves |
| [HudAndCrosshair.md](HudAndCrosshair.md) | Reticle, health ring, custom crosshair options |
| [Mafia3Combat.md](Mafia3Combat.md) | Why Mafia III combat feels good, tools |
| [OtherReferences.md](OtherReferences.md) | Max Payne 3, GTA V, GDC talks, other CE mods |
| [Raycast.md](Raycast.md) | The game's physics line test, what is verified, engine raycast design (ADR-0008) |

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
