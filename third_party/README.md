# Third-party dependencies and asset policy

Nothing in this repository vendors a mod binary, proprietary game asset, or copied third-party source. Install game dependencies from their official releases and record local versions in playtest reports.

| Project | Role | License/permission | Handling |
|---|---|---|---|
| [FusionFix](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix) | CE fixes/overloader | GPL-3.0 in repository | Dependency and research reference; no source copied |
| [ScriptHookDotNet CE fork](https://github.com/Tomasak/gta4_scripthookdotnet) | C# runtime (verified T-001) | Check per-file upstream notices before redistribution | External install; no binary copied |
| [Liberty Tweaks](https://github.com/catsmackaroo/LibertyTweaks) | Feature/technique reference | No repository license found | Do not copy code or assets |
| Real Recoil Enhanced CE | Recoil reference | Unknown | Verify permission before reuse |

T-001 uses the [Tomasak v1.7.1.8 release archive](https://github.com/Tomasak/gta4_scripthookdotnet/releases/tag/release), SHA256 `5669E4423F93BEDFB0AE34579E922213775B46BBEE4DB6ADC953CB53E7AD9058`. Only `aCompleteEditionHook.asi`, `ScriptHook.dll`, and `ScriptHookDotNet.asi` are installed into the game root. The `.asi` is also used as a local compile reference. No runtime binary is committed to this repository.

Future models, textures, audio, and code require source URL, author, license/permission, modifications, and attribution in this directory before inclusion. Do not include ripped commercial-game assets.
