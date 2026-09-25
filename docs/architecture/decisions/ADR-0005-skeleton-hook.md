# ADR-0005: Engine hooks for the dismemberment collapse

Status: **accepted by the owner for the gore pass; revision 2 after playtest 2, awaiting in-game confirmation (T-022)** (2026-09-24).

## Context

Dismemberment collapses a limb's bone matrices into the cut joint so the skinned mesh disappears there (T-022). ADR-0004 allows data writes only, never code patches. The owner asked for visible gore, which needs an exception.

Playtest 2 results:

- **Script-tick writes flicker.** Writes from a script tick only survive frames on which the engine skips the ped's pose update. The owner saw a head "spawning and despawning".
- **The revision-1 hooks never matched a ped** (`hook_calls=0`). They were managed after-call hooks on two fragInst methods, matched by fragInst.

## Decision (revision 2)

**crSkeleton::Update.** `crSkeleton::Update(parentMatrix, globalMatrices)` (0x466BE0 on 1.2.0.59) is the single routine that turns local bone transforms into the global matrices that get skinned. Its body is encrypted on disk, but its call sites are not.

- The 8 call sites that update a skeleton in place (`push [r+14h]; push [r+8]; call`, ecx = skeleton) are redirected to stubs.
- Only the call's rel32 changes, written with a single 4-byte store.
- The resolver finds the function from a unique anchor (0x60BA5E), then enumerates the call sites.

**The stub:**

- It calls the original update, then, while the enable flag is set, calls a native x86 routine with the skeleton.
- The routine looks up the skeleton's global-matrix pointer and bone count in a double-buffered table that the script publishes.
- It writes the collapse for the listed bones: axes = 0.01 × identity, origin = cut joint.
- No managed code runs on engine threads.
- Entries cover the ped's own matrices and the frag cache entry's copy skeleton (`[fragInst+64h]+168h`), which the engine refreshes from the live one.

**Ragdoll sync hook.** The fragInst ragdoll sync (0x5F7D70) writes physics-driven bones directly. It keeps an after-call hook (11 validated stolen bytes) that runs the same routine on the skeleton from fragInst vfunc +E0h. The sync method itself calls that getter on entry.

**Collapse scale.** Collapsed bones get a tiny uniform scale instead of zero axes. Zero axes were a suspect in corpses vanishing; the log later tied that to the limb clone's spawn instead (see Consequences).

**Fallback and removal.** The script-tick write remains as a fallback. On unload and exit, the flag is cleared and every patched byte is restored (memory only). The code block stays allocated.

## Verification

`tools/verify/CollapseEngineChecks.cs` executes the real generated machine code in the x86 verifier. It uses a fake skeleton, a fake update call site and a fake fragInst method, and checks:

- argument, `this` and return-value pass-through;
- table gating;
- bone-count matching;
- the collapsed values;
- byte-exact restore.

The address checks pin 0x466BE0 and the 8 call sites.

## Consequences

- Only GTAIV.exe 1.2.0.59 is supported. If the anchor or a call site differs, the engine is not installed (`skeleton_collapse_engine_unavailable`) and the tick fallback is used.
- `dismembermentEnabled=false` in `combat_effects.json` means no patch is ever written.
- Corpses that get a thrown limb are made mission-owned (`SET_CHAR_AS_MISSION_CHAR`) until their record ends. The corpse vanished right after the clone spawned in playtest 2.
- Rollback: disable the feature in config, or remove the DLL. No game file is changed.
