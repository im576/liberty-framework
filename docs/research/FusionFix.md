# FusionFix

- Source URL: https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix
- Game version: Complete Edition only (legacy exes need a separate "Legacy Addon") [SOURCE]
- Dependencies: none (it *is* the loader: ships `dinput8.dll` = Ultimate ASI Loader)
- Source available?: yes — C++20 modules (`source/*.ixx`)
- License: **GPL-3.0** [SOURCE]
- Active: yes, issues filed as of Sept 2026 [SECONDARY]
- Can reuse?: only if our code becomes GPL-3.0. Default: **reference only**.
- Potential value: **foundation dependency** — we build on top of it, not beside it.

## What FusionFix already gives us (don't rebuild these)

| Feature | Detail | Confidence |
|---|---|---|
| **FusionOverloader** | "Replace game files without actually replacing them" — drop files in `update\` folder. Use this for `weaponinfo.xml`, gold weapon models/textures. | [SOURCE] |
| `RecoilFix=1` | Makes K/M recoil behave like controller recoil. So the vanilla game **already has a recoil code path** that differs by input. | [SOURCE] |
| `AimingZoomFix=1` | Aiming zoom fixes across episodes. | [SOURCE] |
| `GamepadAimSensitivityRange`, `GamepadLookSensitivityRange` | Widen min/max of the in-menu sensitivity sliders (default 0.1–2.0). | [SOURCE] |
| Raw input | For menu + camera (mouse). | [SOURCE] |
| Gamepad icon styles | Xbox 360, PS3/4/5, Switch, Steam Deck. | [SOURCE] |
| `DisableCameraCenteringInCover=1` | Stops forced camera centering at cover edges. | [SOURCE] |
| `AlwaysDisplayHealthOnReticle=1` | Lets **K/M** show NPC health on reticle. Global, not per weapon. Controller shows it anyway. | [SOURCE] |
| Camera shake FPS fix | Shake consistent at high FPS. | [SOURCE] |
| `EpisodicWeapons` / `ExtendedLimits` | Cross-episode weapon support; raises modelinfo/handling/pool limits. Relevant to gold weapon slots (T-007). | [SOURCE] |
| Sniper module | Zoomed movement, controller sniper aim remap with 0.25 dead zone, scope toggle, **recoil multiplier patch** (sets a game recoil multiplier to 0.1 under extended controls). | [SOURCE] via `source/sniper.ixx` |

## Source layout worth reading (reference only)

`source/` modules relevant to us: `natives.ixx` (CE native invoker + hash list), `sniper.ixx`
(aiming/recoil patterns), `rawinput.ixx`, `buttons.ixx`, `centeredcam.ixx`, `settings.ixx`
(how they add in-game menu options), `fusiondxhook.ixx` (D3D hook), `memory.ixx`.

Technique: **byte-pattern scanning** (`hook::pattern("F3 0F 59 05 ? ? ? ? ...")`) instead of hardcoded
addresses. If we ever go C++, copy this *technique* (not code).

## Correction from the T-010 disassembly

`RecoilFix` does not touch camera kick: its hook (`fixes.ixx`, pattern `F3 0F 59 05 ? ? ? ? EB ? E8 ? ? ? ? 84 C0`) sits in `CWeapon::DoAccuracy` and multiplies the **bullet-spread** input term by 0.65. LF's `spreadCalibration.tangentPerAccuracyUnit` includes that 0.65. See [MEMORY.md](../game-api/MEMORY.md). In GTA IV, "recoil" in these files mostly means spread.

## Important finding for recoil (pass 1, partly superseded above)

`sniper.ixx` patches an instruction that multiplies by a recoil constant (`F3 0F 59 05 ...` = `mulss xmm0, [addr]`).
So **GTA IV has an internal recoil system with a global multiplier** [SOURCE]. A future C++ spike could
locate the per-weapon recoil path near that pattern. [HYPOTHESIS]

## Overlap / conflicts with our project

- Don't add our own sensitivity-range, raw-input, or camera-centering options — FusionFix has them.
- Our gold weapons' data goes in FusionFix's `update\` folder via FusionOverloader (T-007).
- Tester should record their `GTAIV.EFLC.FusionFix.ini` in playtest reports if aiming feels off.
