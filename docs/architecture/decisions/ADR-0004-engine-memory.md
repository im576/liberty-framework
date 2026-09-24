# ADR-0004: Pattern-resolved engine memory for gunplay

Status: **accepted for Phase 1; awaiting in-game confirmation (T-010)** (2026-09-24).

## Context

No supported native moves the third-person aim camera by a delta, hides only the reticle, or changes real bullet spread. `SET_GAME_CAM_PITCH/HEADING` set absolute angles relative to the ped, `DISPLAY_HUD`/`HIDE_HUD_AND_RADAR_THIS_FRAME` hide the whole HUD, and `SET_CHAR_ACCURACY` affects AI accuracy. The owner accepts a universal free-aim change.

## Decision

Use in-process reads/writes of engine data (never code patches) located by the resolvers listed in [MEMORY.md](../../game-api/MEMORY.md):

- recoil: add to `CCamAimWeapon` pitch/heading (the fields stick input feeds);
- spread: write `CWeaponInfo` accuracy for registered test weapons only;
- reticle/health ring: shrink hud.dat reticle globals while the LF crosshair or free aim is active;
- free aim: `PREF_AUTO_AIM` = 0 plus the `DISABLE_PLAYER_LOCKON` native;
- evidence: read the per-frame bullet trace list to measure real deviations.

Guards: VirtualQuery before every access; instruction-shape checks; per-feature failure isolation; runtime validation (aim fields vs `GET_CAM_ROT`, accuracy vs WeaponInfo.xml) before the first write; originals saved and restored on disable, error, domain unload and process exit (memory first, the lock-on native last; process exit and drawing callbacks call no natives); the auto-aim pref is also persisted to disk for crash recovery.

## Consequences

- Only GTAIV.exe 1.2.0.59 is verified. Another build fails closed (resolver report in the log).
- FusionFix patches near these sites were checked: the weapon-info bound check NOP is handled; no other FusionFix v5.0.1 byte pattern (1,160 checked, each with a 16-byte margin) overlaps any byte a resolver reads, and FusionFix's native overrides touch no anchor native (2026-09-24 review). The offline verifier simulates the NOP.
- Rollback: disable the feature in DevTools, or remove the DLL; nothing is persisted in game files except the auto-aim pref, which is restored.
