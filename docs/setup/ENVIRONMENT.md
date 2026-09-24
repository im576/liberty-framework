# Human baseline setup checklist (T-000)

This is a reproducibility checklist, **not** a claim that a particular install works. Back up the game directory and saves first. Use official releases; do not copy third-party binaries into this repository.

1. Record GTA IV executable version and whether the install is Steam or Rockstar.
2. Install [FusionFix from its official release](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/releases) according to its README. Record release version and relevant `GTAIV.EFLC.FusionFix.ini` settings.
3. Start the game once without ScriptHookDotNet. Confirm it reaches gameplay and the controller works.
4. Record controller model, Steam Input on/off and layout, in-game aim mode, look/aim sensitivity, and display mode.
5. Install the [proposed CE ScriptHookDotNet release](https://github.com/Tomasak/gta4_scripthookdotnet/releases) following its release instructions. Record every installed file/version and launch result.
6. If a launch fails, revert the last addition, preserve its log, and report exact symptoms. Do not combine multiple unknown fixes at once.

The human fills [T-000](../tasks/T-000-baseline.md) with observed results. T-001 is the first agent-authored runtime experiment. No setup dependencies or game files are committed to Git.
