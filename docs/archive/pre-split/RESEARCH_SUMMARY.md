# Research summary — pass 1 (2026-09-24)

> **Status update (after T-010, 2026-09-24).** The unknowns below are answered: (1) SHDN loads on CE — verified T-001; (2) separate IDs 58/59/60 — verified T-007, but each shares its vanilla weapon's inventory slot; (3) free aim — answered offline as a *universal* switch (Auto-Aim pref + `DISABLE_PLAYER_LOCKON` + hud.dat globals), accepted by the owner; (4) aim camera — `CCamAimWeapon` pitch/heading fields found by pattern and validated at runtime (ADR-0004); (5) Real Recoil Enhanced CE permission — **still open**. Mafia III ideas now appear only as the B/C presets (lighter/heavier kick and recovery). Items 3–4 await the T-010 playtest. Below is the original pass-1 text.

This is a source review, not an in-game verification. No game behavior or runtime compatibility has been tested by the project.

## Decisions supported by research

| Area | Finding | Consequence |
|---|---|---|
| Game baseline | [FusionFix](https://github.com/ThirteenAG/GTAIV.EFLC.FusionFix/blob/master/readme.md) targets Complete Edition and provides file overriding plus several aiming/controller fixes. | Start from an un-downgraded CE install. Record the actual executable and FusionFix versions before coding. |
| C# runtime | [IV-SDK .NET installation requirements](https://github.com/ClonkAndre/IV-SDK-DotNet/blob/main/Documentation/Installation.md) name 1.0.7.0/1.0.8.0. [Tomasak's ScriptHookDotNet fork](https://github.com/Tomasak/gta4_scripthookdotnet/releases) is the proposed CE route. | Treat the C# stack as a hypothesis until a tiny load/log/reload spike works on the tester's install. |
| Liberty Tweaks | Its [repository](https://github.com/catsmackaroo/LibertyTweaks) has recoil, controller shoulder swap and switching while aiming, but depends on IV-SDK .NET and has no visible license. | Use behavior as a reference; do not copy code. Its recoil is primarily camera shake, so actual aim kick needs separate investigation. |
| Existing recoil | FusionFix already changes recoil behavior. A CE Real Recoil release is reported, but its implementation and permissions remain unverified. | Establish a vanilla/FusionFix baseline. Test for double recoil before integrating any other mod. |
| Gold weapons | Spare weapon slot behavior, per-weapon free aim, reticle hiding, and CE camera control have no verified path yet. | Make each a small spike. Do not claim the three gold weapons are ready. |

## Transferable combat principles

Mafia III is a feel reference: responsive aim, distinct first-shot and sustained-fire behavior, synchronized camera/audio/impact feedback, and quick recovery. These are design hypotheses from the handoff, **not measured facts about Mafia III**. See [Mafia3Combat.md](Mafia3Combat.md) for the test plan and documented Mafia III modding references. No Mafia III concept has been incorporated into code because no gameplay code exists.

Other references are catalogued in [OtherReferences.md](OtherReferences.md). No third-party code, assets, or binaries are in this repository.

## Critical unknowns before implementation

1. Does Tomasak's ScriptHookDotNet load a .NET Framework script on the tester's exact CE install? (T-001)
2. Can three distinguishable gold weapons occupy separate slots without replacing vanilla behavior? (T-007)
3. Can free aim and target-health reticle behavior be switched only for those weapons? (T-008)
4. What supported CE API moves the live aim camera and how does it interact with FusionFix? (T-009)
5. What is the license/permission status of Real Recoil Enhanced CE? (Research follow-up)

The [task queue](../tasks/README.md) turns those unknowns into bounded experiments. Human playtests are required before marking any in-game result verified.
