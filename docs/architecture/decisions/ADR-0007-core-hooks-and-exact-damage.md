# ADR-0007: LibertyCore hook manager and exact ped damage

Status: **proposed and implemented for the autopilot's in-game test** (2026-09-25). Owner approval: "Go" on the
M2 plan (hooks built together with exact damage).

## Context

`PedDamaged` inferred damage from health differences between frames, so it knew neither the attacker, the weapon nor
the amount for certain, and it missed peds outside the snapshot radius. The game computes every ped damage in one
routine. Observing that routine gives exact data. ADR-0004 allows data writes only; ADR-0005 made a call-site exception
for dismemberment. A general, owned and restorable hook mechanism is needed for this and for later work (native
interception, physics queries).

## Findings (GTAIV.exe 1.2.0.59, disassembly)

**The damage routine: `CA3820`**, `thiscall(calculator, CPed* ped, response*)`, `ret 8`.
- It is the only real writer of the ped's last-damage component (`[ped+0xA78]` at `CA3AB4`; the two other writers reset it to -1).
- `GET_CHAR_LAST_DAMAGE_BONE` reads that field.
- It stores the damager through a helper into `[ped+0x1E4]`, the field `HAS_CHAR_BEEN_DAMAGED_BY_CHAR` reads.
- It has 21 callers.

| Structure | Offset | Field |
|---|---|---|
| Calculator | +0x0 | damager (`CEntity*`) |
| Calculator | +0x4 | damage (float) |
| Calculator | +0x8 | body component |
| Calculator | +0xC | weapon type (SHDN `Weapon`: 0–20 weapons, 21–44 episodic, 49 rammed by car, 50 run over, 51 explosion, 53 drowning, 54 fall, 55 unidentified) |
| Response | +0x4 | flags |
| Response | +0x8 | health lost (float) |
| Response | +0xC | armour lost (float) |

**Component to bone tag:** `A76700(CPed*, component)`, cdecl, as called by the `GET_CHAR_LAST_DAMAGE_BONE` worker.

**Prologue:** `83 EC 0C 53 8B 5C 24 18` (`sub esp,0Ch; push ebx; mov ebx,[esp+18h]`). It has no relative operands, so these 8 bytes can move to a trampoline unchanged.

## Decision

**LibertyCore hook manager (`native/LibertyCore/src/hooks.cpp`).**
- **Entry detours only when the displaced bytes are known.** The caller supplies the expected prologue, which comes from the signature. The manager refuses to hook if the live bytes differ, including when they already start with a jump (another mod's hook).
- **Trampolines** are built in memory that is written first, then made read/execute (never writable and executable at once).
- **Every hook is named and listed.** `lf hooks` shows them.
- **Removal** restores the original bytes, and only if the jump is still ours. Hooks are removed in `lc_shutdown`, which runs on script unload.
- **Installation runs only on the engine tick**, when the game thread is parked in the script.

**Damage observer.**
- The detour calls the original routine, then records the calculator and response values into a 64-entry ring.
- It records nothing it has not just seen the game use, and it never changes the result.
- The record step runs inside the core's SEH guard (`safe_call.cpp`).
- `lc_frame` drains the ring into the snapshot (`damages[]`, ABI 3): it maps pointers to handles through the pools and marks kills with `IS_CHAR_DEAD`.
- `PedDamaged` then carries `Exact = true` with attacker, attacker vehicle, weapon, damage type, bone, amount, health and armour lost, and kill.
- For bullets it also carries the hit point and direction, from the bullet trace list: the trace fired by the attacker that ended nearest the victim.

**Addresses** are resolved by `GameAddresses.ResolveDamage`, never hardcoded:
- the prologue signature, plus the component store at +0x291;
- the component-to-bone helper, via the `GET_CHAR_LAST_DAMAGE_BONE` handler → worker → call chain.

The offline verifier pins both.

## Consequences

- Damage events are exact for every ped in the world, the player included.
- The inferred path remains the fallback: if the hook is refused or the signature is missing, `Exact` stays false.
- The detour is on a hot path: every damage event of every ped. It does a handful of reads and one ring write; the component-to-bone call is the game's own.
- Migrating the ADR-0005 skeleton call-site hooks onto the manager is follow-up work. They keep working as they are.
- Mods do not get hooks yet. A capability-checked `IMemory` hook API comes after this path has run in game.
