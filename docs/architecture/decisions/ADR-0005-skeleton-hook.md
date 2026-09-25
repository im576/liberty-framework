# ADR-0005: After-call hook on the fragInst skeleton rebuild (dismemberment)

Status: **accepted by the owner for the gore pass; awaiting in-game confirmation (T-022)** (2026-09-24).

## Context

Dismemberment collapses a limb's bone matrices into the cut joint so the skinned mesh disappears there (T-022). Writing the matrices from a script tick is not enough if the engine rebuilds a ragdoll's skeleton from its physics fragments every frame: the collapse is overwritten before the frame is drawn. ADR-0004 allowed data writes only, never code patches. The owner asked for visible gore in this pass, which needs an exception.

## Decision

Hook the two engine functions that rebuild a fragInst's skeleton, and re-apply active collapses right after they return:

- `0x5F7D70` fragInst skeleton sync (thiscall, no arguments, called from `0x61183C`);
- `0x5F6FB0` fragInst pose (virtual, four vtables).

Both are resolved by long unique patterns (MEMORY.md, `skeleton_hooks`) and verified offline. `GameApi/SkeletonHook.cs` moves the first 11/12 entry bytes to a trampoline and jumps to a stub that calls the original. When the stub's enable flag is set, it then calls a managed stdcall callback with `this`. The callback (`Dismemberment.OnSkeletonRebuilt`) matches the fragInst against an immutable snapshot of active collapses and writes the matrices through `Marshal`. It never throws and calls no natives.

Guards:

- The hook installs only over the exact expected bytes. A different build, or another mod's hook, makes it fail closed.
- It installs from a script tick, while the game thread is parked.
- The enable flag is set only while at least one collapse is active, so the engine pays one compare otherwise.
- On script unload and at process exit, the flag is cleared and the original bytes are restored (memory only). The stub and trampoline stay allocated, because a thread could still be returning through them.
- The per-tick collapse remains as a fallback when the hooks can't be installed.

## Consequences

- Only GTAIV.exe 1.2.0.59 is supported; other builds fall back to per-tick collapse (`dismemberment_ready hooks=False`).
- If the callback runs on a thread that SHDN's CLR can't enter, the game could stall. The owner can set `dismembermentEnabled=false` in `combat_effects.json`: the hooks are then never installed.
- Rollback: disable the feature in config, or remove the DLL. No game file is changed.
