# ADR-0002: Proposed separate gold weapon identities

Status: **revised hypothesis; verify in T-007** (2026-09-24).

Prefer separate weapon identifiers/assets over replacing vanilla pistol, rifle, and shotgun behavior. FusionFix v5.0.1's `ExtendedLimits` hook registers unknown weapon names starting at ID 58, and this installation already has `ExtendedLimits=1`. The first spike adds `LF_GOLD_PISTOL` to an overloader copy of the base `WeaponInfo.xml`, initially reusing the vanilla pistol model and stats so the test isolates identity. The ID 58 assignment is an assumption until observed in-game; other custom weapon registrations could change the order. ScriptHookDotNet can pass numeric weapon IDs to the game, but working selection, inventory coexistence, save/load, and mission behavior remain unverified.

The old proposal to use `EPISODIC_22`–`EPISODIC_24` is deferred: the installed IV/TLAD/TBoGT weapon files contain no definitions for them, and this FusionFix setup has `EpisodicWeapons=0`. T-007 must prove one custom identity first, then expand to three only if it is stable. If the new-name route fails, document alternatives and limitations before changing vanilla weapon data.
