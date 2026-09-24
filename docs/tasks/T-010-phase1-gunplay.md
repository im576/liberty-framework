# T-010 — Phase 1 integrated gunplay build

Status: **NEEDS-PLAYTEST**. Supersedes the T-008 (aim/HUD) and T-009 (camera recoil) spikes; builds on T-003 (DevTools) and T-007 (IDs 58/59/60).

## Scope (owner instructions 2026-09-24)

1. Universal, reversible free aim: Auto-Aim pref off, `DISABLE_PLAYER_LOCKON`, target health/armour ring hidden; prior settings restored on disable/unload/next start.
2. Data-driven recoil (camera kick, accumulation, recovery) and real spread (game accuracy written per frame) for the three test weapons, with movement/crouch/cover/blind-fire/vehicle multipliers.
3. Neutral four-segment crosshair whose gap is the live spread cone; vanilla reticle hidden while it is on.
4. Gold finish on the pistol through a reusable finish pipeline (`assets/finishes`, `tools/finishes`).
5. DevTools: weapons, gunplay toggles, live tuning, presets, config reload/save, teleports, test range, state inspector.

Techniques and evidence: [MEMORY.md](../game-api/MEMORY.md), [NATIVES.md](../game-api/NATIVES.md), [ADR-0004](../architecture/decisions/ADR-0004-engine-memory.md). Config: [CONFIG_SCHEMA.md](../architecture/CONFIG_SCHEMA.md).

## Verified offline by the agent (no game running)

- `tools/build.ps1`: x86 DLL, 59 files, zero errors, zero warnings (warnings are errors).
- `tools/verify.ps1`: 132/132 — every engine address resolved from the real GTAIV.exe matches an independent disassembly (including a simulated FusionFix bound-check NOP); all 22 natives the DLL calls are registered by the exe; config/presets/locations parse and validate; recoil, spread, calibration convergence, shot geometry, save/backup round-trip, malformed-config retention.
- `tools/build-finishes.ps1`: gold textures generated from the installed `w_glock` assets; resource and IMG written, re-read and byte-compared; before/after previews in `staging/phase1/previews`.
- `tools/package-phase1.ps1` + install/rollback dry run on a copy of the game files: install verified by SHA-256, rollback restored every file byte-identically.

Not verifiable without the game: all in-game behaviour below. The DLL logs proof points for each (search the log for the quoted tags).

## Human test steps

See the consolidated checklist in [../testing/PHASE1_PLAYTEST.md](../testing/PHASE1_PLAYTEST.md).
