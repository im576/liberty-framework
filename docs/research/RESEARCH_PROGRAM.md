# Research program: capabilities for the full mod

Updated 2026-10-02 for the owner's request for deeper reverse engineering, harsh gore,
owned vehicles and a much broader overhaul. Research supports the vision now; it is not
blocked by a historical rule that all Phase 3 work must wait. Runtime changes still need evidence.

Current next focus is grounded/severe gore and tuning the existing atmosphere across
day/night and weather, plus a cloud overhaul. Rain was an example, not the whole
scope. Use the current Liberty+ `docs/design/GORE_AND_ATMOSPHERE.md` brief. The
experiments below remain evidence-backed starting points; their historical numbering
does not require vehicle/audio work to finish before atmosphere research. Research
and feature implementation are not automatically dispatched by this document.

## What is already here

`native/LibertyCore` supplies native invocation, world snapshots, hooks and ray queries. C# engine
services add module lifecycle, resource ownership, scheduling, UI, state, streaming and hot reload.
The core's damage record contains victim/damager, weapon, component/bone and health/armour loss;
that does not establish exact surface UV/contact coordinates. Main's old combat controller still
polls damage; lane C contains the event-driven migration. Do not confuse an available engine API
with integration into every reference gameplay module.

The skeleton collapse path and limb-clone technique exist. The open problem is stable lifetime,
cleanup, fidelity and bounded cost under combined load. Another round of approximate damage
polling is not the missing engine capability. Original stump caps require authored geometry and
an attachment path; particles alone cannot create a convincing cut surface.

## Prioritized experiments

These are prepared research tasks, not claims that the experiments ran. Each runtime experiment
uses the existing game lock, exact install/rollback receipt, unchanged budgets and a bounded batch.

| Priority / question | Starting evidence and code | Smallest discriminating experiment | Capability unlocked |
|---|---|---|---|
| 1. What invalidates the skeleton/clone near the recurring dismember crash? | C `4cae50e`, `Dismemberment`, `SkeletonCollapseEngine`, native crash capture; historical FusionFix+0xA24E0 and GTAIV+0x664503 faults | First reproduce setup with cuts off, then the same fixture with one controlled cut. Record ped/pool identity, skeleton pointer/count, collapse table generation, clone hide/show and release order. Correlate fault instruction with loaded module hashes. Only after that evidence vary one companion/plugin at a time. | Reliable damage-to-sever lifecycle and resource cleanup; hook/API changes only where proven necessary |
| 2. Which calls cause combat and UI spikes? | C clone/sever scopes; B `20261001-202311-2297a17` trunk p95/p99 failure; earlier trunk sample includes combat 258 ms peak | Use paired scene/setup samples, distinguish engine tick/draw and native-call cost, alternate baseline/candidate order. Separate cold asset work from steady state. Do not attribute total frame p95 to UI solely because the menu is open. | Budgeted admission/streaming/preparation based on measurements; density stays unchanged |
| 3. Can ownership survive every interrupted purchase and actual GTA save/load? | MIT LVS purchase/registration/load/save; Liberty `IState`, Arsenal instance IDs and read-only LVS parser | First offline fault-inject failures before/after charge, spawn, durable registration and refund. Define one durable operation ID, ownership identity and retry semantics. Then a minimal vehicle module round-trip using actual save/reload and mission/cutscene guards. | Owned vehicle records, insurance/garage storage and bounded nearby restoration |
| 4. What is the hit material's effective mapping? | SDK 1.2, `HitMaterial.md`, R fixtures; result low-byte material consumer | Pin effective materials data, use visible isolated wood/glass/water/vehicle/object targets and correlate hit entity/position to the capture. A CAR_VOID row at window height is a hypothesis, not a glass classification. | Material effects/audio with explicit unknown fallback |
| 5. Why does a second drawable geometry crash? | `tools/models`, T-031 bisect; one geometry succeeds with both weapon/map templates | Reconstruct a known-good two-geometry resource, compare every pointer/count/alignment/relocation and page span. Vary geometry count with shared shader/texture, then shader count separately. Queue a single spawn only after offline invariants pass. | Authored gore caps and richer world/vehicle assets without unstable writer assumptions |
| 6. How are collision shapes owned and serialized? | `Collision.md`, WBD hash-to-target decoding, T-032, collision proxies | Decode one primitive and its game consumer; validate round trip, bounds ownership and transforms before writing new data. Test ray, player and vehicle collision separately. | Authored collision rather than claiming proxy ray hits prove all physical behavior |
| 7. Which audio and frontend paths support new authored behavior? | T-053/T-055, local sound archive comparison, R research branch | Follow descriptor/payload consumers; preserve existing IDs first, validate one original sample. For HUD/frontend follow actual consumers and test hide/restore with native help/story text and real aim input. | Weapon audio replacement and composable UI; deeper hooks follow proven contracts |

The highest-value native research answers object lifetime, threading and ownership—not merely more offsets.
The crash module is where the fault surfaced, not proof of the responsible mod. Preserve original dumps and hashes.
The previous quick setup/active run timed out before engine readiness and cannot answer the gore hypothesis.

## Existing-mod source strategy

Local archive inventories can be regenerated without extraction or execution:

```powershell
python tools/review/inspect_mod.py '<downloaded.zip>' --output results-local/offline/mod-inventory.json
```

October 2 reinspection:

| Archive | SHA-256 | What was directly observed | Integration choice |
|---|---|---|---|
| Liberty-Vehicle-Services-CE-main(1).zip | `d83c63c12e6e9382abbc98d3e1af6b3df2c8891a2b885ad8fc9237ac03407b6c` | 2 source files, 2 binaries, MIT notice; ZIP comment pins `30387e9dd779f157cbebcefb68663a75273419d8` | Adapt selected database/service/ownership logic with notices; replace polling, rendering, state and spawn integration through Liberty services |
| Violent Liberty 1.1 ZIP | `459a66bbde3bbf48c80092ca57c34455ba5fda770b628835cb6d160c3ab7a2a6` | No source, one binary, no license notice in archive | Binary/runtime research and controlled coexistence experiments; create original renderer/assets for shipped Liberty implementation |
| Liberty Rush 1.12 ZIP | `64cc1e865bd74da4d9cbe5bd2ac7769b5db207cc9e0fb6033626e6f0a1adde1d` | No source, eight binary entries, no license notice in archive | Study scenario data, streaming limits and population behavior; older-version package is not a CE runtime implementation |

Additional local references: `D:/GTAIV-Reborn-Tools/sources/FusionFix`, `sources/DXVK`,
`research/lvs-zip-20260930/extracted`, and owner-downloaded sound/weapon archives.
Record the precise checkout/file/license before selecting additional code. See [third-party register](../../third_party/README.md).

The preserved LVS source review identified charge-before-registration without refund on that failure path,
non-transactional saves, restoration identity concerns and fractional refuelling accounting. Do not carry those
defects into a port. The local archive's MIT notice covers the licensed work; separately establish provenance for
any selected third-party artwork/component. This review imports no external implementation or assets.

## Evidence contract for every discovery

Record question, input hash/version/commit, exact producer and consumer, source/disassembly location,
calling convention/thread, object lifetime, expected bytes/resolution, test control, observed output and limitation.
Use SOURCE OBSERVATION / VERIFIED OFFLINE / VERIFIED IN GAME / HYPOTHESIS / UNKNOWN.
Promotion to production requires a narrow SDK contract, capability gating, failed-resolution fallback,
unload restoration, offline tests and the affected game check. Save negative findings so agents do not repeat them.

Historical supporting reviews are in the chat evidence folder:
`C:/Users/IM576/OneDrive/Documents/ChatGPT/GTA4-Reborn/research/OtherMods-2026-09-30.md`
and `LVS-Zip-Review-and-Approach-2026-09-30.md`. They are source/evidence indexes; current source and fresh
observations override their old scheduling and feature-scope restrictions.
