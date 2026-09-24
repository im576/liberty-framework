# Controller and aim assist research

The tester uses a controller through Steam. GTA IV's targeting mode, Steam Input mapping, and FusionFix sensitivity settings can all affect observations. [FusionFix's config](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/master/data/plugins/GTAIV.EFLC.FusionFix.ini) exposes gamepad sensitivity range options; record the actual values used during tests.

The current repository has **no verified per-weapon free-aim switch**. Liberty Tweaks changes a targeting setting through an IV-SDK .NET API that is unavailable on Complete Edition; see [LibertyTweaks.md](LibertyTweaks.md). That suggests an experiment, not a portable solution.

For T-000, record controller model, Steam Input state/layout, in-game control scheme, aim mode, sensitivity, and FusionFix config. For T-008, first measure baseline behavior of a vanilla pistol and a proposed gold-slot weapon; then test any targeting-mode control and watch whether it persists after switching weapons, cover, vehicles, missions, and reload. Restore the player's prior setting on every exit/error path.

Desired Phase 1 gold-weapon profile: no lock-on, snapping, tracking, or target health UI. Vanilla weapons retain the baseline behavior. Stick curves, slowdown, and magnetism are future profile fields until a safe API and human preference test exist.
