# Game version, loaders, and script runtimes

**Bottom line:** Target **CE 1.2.0.59** + **FusionFix** + **ScriptHookDotNet (CE-compatible build)**.
IV-SDK .NET (what Liberty Tweaks uses) does **not** run on CE, so we cannot use it without a downgrade.

## Game executable versions

| Version | What | Notes |
|---|---|---|
| 1.0.7.0 / 1.0.8.0 | Pre-2020 "classic" exes | Most old mods target these. Needs a downgrade from Steam today. |
| 1.2.0.43 | Complete Edition (2020) | First CE build. |
| **1.2.0.59** | Complete Edition, current Steam build | **Our target.** [SECONDARY] ([GTAForums](https://gtaforums.com/topic/988627-update-12059/), Steam guide) |

Handoff says: avoid downgrade. Research agrees — FusionFix officially supports CE only [SOURCE].

## Loaders / hooks / runtimes

### Ultimate ASI Loader (UAL)
- Loads `.asi` plugins. Works with all versions [SECONDARY] ([Gillian's guide](https://gillian-guide.github.io/resources/mod-dependencies/)).
- **FusionFix ships its own UAL as `dinput8.dll`.** If using FusionFix, delete any extra `dsound.dll` loader [SECONDARY].
- Script mods go in `scripts\`, not FusionFix's `plugins\` — some mods break in `plugins\` [SECONDARY].

### Aru's C++ ScriptHook + LMS "Compatibility Patch for CE"
- Author LMS (LCPDFR). v0.4, 2021-01-27. Files: `aCompleteEditionHook.asi`, `ScriptHookDotNet.asi`,
  `AdvancedHook.dll`, `AdvancedHookInit.asi` [SOURCE] ([LCPDFR](https://www.lcpdfr.com/downloads/gta4mods/g17media/26726-compatibility-patch-for-gta-iv-complete-edition/)).
- `aCompleteEditionHook.asi` is a runtime fix **on top of** Aru's original C++ ScriptHook (still required).
- Targets 1.2.0.43. Author offers no support.

### ScriptHookDotNet (HazardX) — **our chosen runtime**
- Original: [HazardX/gta4_scripthookdotnet](https://github.com/HazardX/gta4_scripthookdotnet). C++/CLI, full source.
  Needs .NET Framework 4. Scripts: `scripts\*.net.dll` (also raw `.cs`/`.vb`) [SOURCE].
- **CE build:** [Tomasak/gta4_scripthookdotnet](https://github.com/Tomasak/gta4_scripthookdotnet/releases) v1.7.1.8 —
  "Added 1.2.0.59 compatibility", bundles LMS's `aCompleteEditionHook.asi` [SOURCE].
- In-game console (`~`) with **`ReloadScripts`** → hot reload of scripts without restarting the game [SOURCE]. Huge for our loop.
- APIs: `Game`, `Player`, `Ped`, `Vehicle`, `Weapon`, `Camera`, `Graphics` (draw text/rects), `Forms` (UI),
  `GTA.Native.Function.Call` for raw natives, per-script `.ini` settings [SOURCE].
- **Proof it works on CE today:** Liberty Vehicle Services CE (C#, MIT, 2026) and Real Recoil Enhanced CE
  (`WeaponRecoil.net.dll`) both ship as ScriptHookDotNet scripts on CE [SOURCE]/[SECONDARY].
- License: readme is "as-is, link to official thread, don't mirror" — **do not redistribute the SHDN binary in this repo**. The tester installs it.

### IV-SDK (C++, Zolika1351) — not usable on CE
- Supports EFIGS 1.0.7.0 and 1.0.8.0 only [SOURCE] ([repo](https://github.com/Zolika1351/iv-sdk)).

### IV-SDK .NET (ClonkAndre) — not usable on CE
- "Only works on GTA IV version 1.0.7.0 and 1.0.8.0" [SOURCE] ([repo](https://github.com/ClonkAndre/IV-SDK-DotNet)).
- CE support request: [issue #13](https://github.com/ClonkAndre/IV-SDK-DotNet/issues/13) — open since 2024-05, acknowledged, no timeline [SOURCE].
- License GPL-3.0. Has Dear ImGui integration (nice, but unavailable to us on CE).
- **Watch this issue.** If CE support ships, re-evaluate ADR-0001 (it would give ImGui menus + memory classes like `IVWeaponInfo`, `IVCamera`).

### FusionFix native invoker (C++)
- FusionFix's `source/natives.ixx` has a CE native invoker + 5000+ native hash constants, found via
  `rage::scrEngine` native table [SOURCE]. GPL-3.0. Useful as **reference** if we ever write a C++ ASI.

## Recommended install (tester)

See [../setup/ENVIRONMENT.md](../setup/ENVIRONMENT.md).

## Risks

- SHDN on CE depends on a 2021 patch by an author who doesn't support it. If something native is
  broken on CE, we may need a small C++ ASI (ADR required).
- Steam may update the exe (unlikely after years). Log the exe version at startup (T-002).
