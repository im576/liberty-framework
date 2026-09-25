# Third-party dependencies and asset policy

Nothing in this repository vendors a mod binary, proprietary game asset, or copied third-party source. Install game dependencies from their official releases and record local versions in playtest reports.

| Project | Role | License/permission | Handling |
|---|---|---|---|
| [FusionFix](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix) | CE fixes/overloader | GPL-3.0 in repository | Dependency and research reference; no source copied |
| [ScriptHookDotNet CE fork](https://github.com/Tomasak/gta4_scripthookdotnet) | C# runtime (verified T-001) | Check per-file upstream notices before redistribution | External install; no binary copied |
| [Liberty Tweaks](https://github.com/catsmackaroo/LibertyTweaks) | Feature/technique reference | No repository license found | Do not copy code or assets |
| Real Recoil Enhanced CE (Priler) | Recoil reference | Nexus permissions: modification/asset use need the author's permission (checked 2026-09-24) | Reference only; no code or files used |
| [Liberty Vehicle Services CE](https://github.com/ekzestean/Liberty-Vehicle-Services-CE) by ekzestean | Vehicle base (dealers, ownership, insurance, tuning, fuel); Arsenal trunks key on its owned vehicles | MIT (Copyright (c) 2026 ekzestean) | Unmodified release files bundled in the Phase 1 package (`scripts/LibertyVehicleServicesCE.*`, `plugins/000_LVSCE_Dashboard_Bridge.asi`); LICENSE and CREDITS shipped alongside; any adapted code is credited in the file that uses it |
| [ScriptHookDotNet 1.7.1.9 fork](https://github.com/Priler/gta4_scripthookdotnet) by Priler | Candidate newer runtime (CE-first, leak fixes) | MIT | Reviewed only; the project still uses Tomasak 1.7.1.8 |
| Holsterable Weapons (Xatriya), Equip Gun | Visible-weapons prior art | Nexus: modification needs permission | Technique only; T-021 is written from scratch |

T-001 uses the [Tomasak v1.7.1.8 release archive](https://github.com/Tomasak/gta4_scripthookdotnet/releases/tag/release), SHA256 `5669E4423F93BEDFB0AE34579E922213775B46BBEE4DB6ADC953CB53E7AD9058`. Only `aCompleteEditionHook.asi`, `ScriptHook.dll`, and `ScriptHookDotNet.asi` are installed into the game root. The `.asi` is also used as a local compile reference. No runtime binary is committed to this repository.

Future models, textures, audio, and code require source URL, author, license/permission, modifications, and attribution in this directory before inclusion. Do not include ripped commercial-game assets.

T-020 reads the INI format documented by [Liberty Vehicle Services CE](https://github.com/ekzestean/Liberty-Vehicle-Services-CE) (ekzestean, MIT) as an external, read-only integration. The Arsenal parser is newly written from the `[owned.<id>]`, `modelhash`, `episode`, `x/y/z`, and `destroyed` fields; no LVS source is copied into this repository.

T-023 reviewed the LVS CE MIT source (SHA-256 `5D4A3CC92A619E46A9B90CDC8D63620CB209ADC3B6E903733CD63F978F33F67C`) by ekzestean. Its existing generic Extras workshop handles preview, purchase, and owned-car restore. No derivative or GTA IV geometry is committed because an exact body-extra model/slot has not yet been verified.
