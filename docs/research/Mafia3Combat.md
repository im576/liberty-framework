# Mafia III combat reference

Mafia III is a **feel target** named by the project owner, not a source of code or numerical values. No direct instrumented gameplay review has been performed yet; the observations below are hypotheses to evaluate with the human tester.

## Candidate principles

- A responsive first shot and a clearly different sustained-fire state may make weapons readable.
- Camera impulse, muzzle flash, sound, impact, target reaction, and recovery should be assessed together.
- Pistol, carbine, and shotgun need distinct timing and strength, while remaining usable with manual controller aim.
- Quick camera recovery can preserve control after a powerful shot; long bursts should still require player correction.

## Research/test follow-up

Capture side-by-side controller footage of Mafia III and the three GTA IV gold prototypes in similar situations. Note input latency, first-shot accuracy, kick/recovery time, burst behavior, hit reactions, reload and cover transitions. Identify which effect can be achieved with GTA IV's existing assets/settings before adding a new module.

[MafiaToolkit](https://github.com/Greavesy1899/MafiaToolkit/blob/master/README.md) documents support for Mafia III and tools for reading game tables including weapon data. Its [PolyForm Strict license](https://github.com/Greavesy1899/MafiaToolkit/blob/master/LICENSE) is source-available, **not open source**, and does not allow incorporating its code into this project. It can be a private research tool within its license terms. A [Mafia III Immersion and Realism mod](https://www.nexusmods.com/mafia3/mods/82) advertises rebalanced weapon stats and stronger combat feedback; its implementation and reuse permissions have not been checked. No Mafia III concepts have yet been incorporated in code.
