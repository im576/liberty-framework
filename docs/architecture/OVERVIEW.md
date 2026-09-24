# Architecture

State after T-010 (Phase 1 integrated build). One DLL, `LibertyFramework.net.dll`, loaded by ScriptHookDotNet; four `Script` classes share one app domain.

```text
GunplayController (every frame)
  -> Core/Memory: CodeScanner + GameAddresses resolve engine data once (fail closed per feature)
  -> GameApi: Natives, AimCamera, WeaponInfoTable, HudReticle, GamePrefs, PlayerMemory, BulletLog
  -> universal (toggles): FreeAimMode, crosshair / vanilla reticle hiding
  -> registered test weapon (IDs 58+ in gunplay.json)?
        yes: RecoilSolver -> aim-camera kick; SpreadModel -> CWeaponInfo accuracy; shot audit -> SpreadCalibrator
        no:  leave the weapon vanilla (crosshair shows its own accuracy)
DevToolsMenu      -> pages: weapons, gunplay, live tuning, presets/config, teleport, test range, inspect, help
WeaponSlotProbe   -> console commands LFWeaponStatus / LFWeaponGive / LFWeaponVanilla
RuntimeProbe      -> heartbeat + probe.json (T-001/T-002)
GunplayConfigStore -> gunplay.json polling, presets, save with .bak
RuntimeLog        -> scripts/LibertyFramework/logs/LibertyFramework.log (1 MiB rotation, one backup)
```

Rules that shape the code:

- **Configuration holds numbers, code holds algorithms** (`config/`, [CONFIG_SCHEMA.md](CONFIG_SCHEMA.md)).
- **Engine access** follows [ADR-0004](decisions/ADR-0004-engine-memory.md): pattern-resolved, validated before the first write, restored on disable/error/unload/exit. Offline proof: `tools/verify.ps1`.
- **Threads:** natives are called only from script ticks (and `DomainUnload`), never from `PerFrameDrawing` (SHDN's Direct3D hook) or `ProcessExit`.
- **Failure isolation:** each feature catches, logs, restores and disables only itself; the game loop keeps running.
- **Vanilla containment:** recoil and spread apply only to registered test weapons; free aim and the crosshair are owner-approved universal toggles (AGENTS.md rule 2).

Build-time only: `tools/finishes` (gold finish variants from the player's own game files), `tools/verify` (offline checks), `tools/*-phase1.ps1` (package/install/rollback).

## Logging

The logger records startup, engine resolution, validation results, state every 5 s, weapon changes, shot audits, config loads/rejections, DevTools actions and every caught error. It writes under the game's `scripts/LibertyFramework/logs/`, never in Git.

## Dependencies and gates

[ADR-0001](decisions/ADR-0001-runtime.md) runtime (verified), [ADR-0002](decisions/ADR-0002-weapon-slots.md) weapon identity (verified), [ADR-0003](decisions/ADR-0003-config.md) configuration, [ADR-0004](decisions/ADR-0004-engine-memory.md) engine memory (awaiting playtest).
