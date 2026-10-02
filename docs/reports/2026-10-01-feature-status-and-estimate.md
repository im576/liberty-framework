# Gameplay feature status and completion estimate — October 1, 2026

**Later owner priority update:** the limited preview is now installed at 10:00 p.m. Pacific, source36901ab on
a separate preview branch. Functional smoke87/0/0errors, four captures parent-reviewed, verifier1020/0 and44 final
installed files hash-verified; pre-smoke state restored. Gore/atmosphere excluded; vanilla HUD retained. The interim
preview milestone is achieved; full gameplay acceptance gaps and estimate below remain open. Agents work offline
while the owner tests. Controls/receipt are in `C:\Users\IM576\GTAIV-Reborn-feature-preview\docs\testing\FEATURE_PREVIEW.md`
and `docs\reports\2026-10-01-feature-preview-ready.md` in that worktree.

The immediate target is the owner's integrated gameplay feature mod: weapons/gunplay/loadout,
wheel/trunks/storage, gore/combat effects, functional HUD/menus and supporting feature systems.
Environment remaster and atmosphere deployment come later. Art direction can continue independently.
The mod is not yet feature-complete or ready for final acceptance; more remains than the owner's playtest.

## Verified progress and remaining work

| Area | Evidence | Remaining acceptance |
|---|---|---|
| Weapons, gunplay, reticles | Lane A is integrated with prior full scenario evidence. | Owner feel/class tuning, physical input checks, mission/cutscene compatibility on the combined build. |
| Physical 2+1 loadout, holsters/slings | T-044 is integrated; prior scripted vehicle/death/state round trips passed. | Real save/load, outfit clipping and combined gameplay checks. Scripted round trips do not establish real GTA save persistence. |
| Wheel and menus | Fresh B full `20261001-202311-2297a17`: SDK UI 21 steps/0 failed; wheel 99/0, all 16 openings draw at frame 0; wheel performance passes. Parent reviewed the produced captures. | The shared follow-up remains unmerged because trunk performance fails. Config rejection/recovery/restart coverage and physical controller checks remain. |
| Trunks/storage | Earlier integrated storage flows passed. Fresh follow-up shows readable initial storage, but stops at 41 steps on its performance gate. | p95 46.4 ms versus 40.7 ms closed baseline; p99 99.7 versus 68 ms. Diagnose without relaxing limits, then repeat full transfer/take/swap/capacity/close acceptance. These later fresh steps were not run. Real safehouse/gunsmith/save-load checks remain. |
| Gore/combat effects | C's cleanup/admission candidate compiles and passes offline checks. | Dismemberment crash, firefight/night failures, strict resource cleanup and peak performance remain unresolved. The latest active comparison timed out before engine readiness, so it provides no active-effects acceptance. |
| Functional HUD | D's conservative vanilla guard remains. Prior nine captures support cash/wanted/radar restoration; expiry and fixture corrections pass focused checks. | Latest held baseline stopped on an invalid unarmed clip expectation; correction is offline only. Idle/aim weapon/ammo captures, clean hiding, story/help/subtitle preservation, unload/config-off restoration and coexistence remain. No new hiding default is accepted. |
| Supporting SDK/materials | R full `20261001-200706-7d80c1d`: all 53 SDK assertions pass; parent reviewed all 40 material captures. | Two multi-second stall errors and obstructed surface views prevent clean acceptance. Direct glass correspondence, effective material-name mapping and water material hits remain unproven. Camera/parking safety fixes await runtime validation. SDK 1.3 remains separate. |
| Build/test host | Generated-input staging fix is integrated and previously passed full offline tooling and real file verification. Startup shared-deadline/partial-evidence fix is integrated as `9cdc097`; complete main tooling regression passes 256/0. | A bounded runtime check must establish its behavior on the actual host. A tooling fix does not resolve the gore crash. |

## Agents and current schedule

The original four replacement workers are reused, preserving their branches and unfinished work:

- B / Planck: GPT-6.1 Sol Medium; finishing bounded paired trunk cost/status observations and focused tests offline.
- C / Herschel: GPT-6.1 Sol High; narrowing the effects/crash acceptance plan offline; host candidate is under parent integration review.
- D / Archimedes: GPT-6.1 Sol Medium; corrected held-baseline candidate ready, awaiting an explicit game slot.
- R / Socrates: GPT-6.1 Sol Medium; reviewing preserved stall evidence offline; parking/safety candidate held.

After main offline regression passed 256/0, D was assigned the single corrected full held-baseline slot.
The other three lanes remain offline; assignment alone is not a runtime pass.
Only one heavy/game batch is assigned at a time; every batch restores the previous installation.
The latest schedule in ORCHESTRATOR.md overrides this timestamped snapshot.

## Completion estimate

These are **low-confidence planning allowances**, not measured remaining effort or a delivery promise.
They assume continued agent access, a working test host and no substantial new feature requests.
Usage interruptions and the owner's availability add calendar time.

| Milestone | Planning allowance | Meaning |
|---|---|---|
| Limited preview of already validated, integrated features | About 1–3 focused workdays | Review combined behavior and prepare a manifest/controls for weapons, loadout and existing wheel/storage. Gore and the new HUD cannot be represented as stable. This is an optional interim preview, not the complete requested milestone. |
| Integrated candidate covering all requested gameplay features | Plan roughly 1–3 weeks of continued work | Resolve C's stability/cleanup/performance failures, B's trunk performance, D's HUD and R's supporting acceptance; review/merge and run combined regression. |
| Owner-accepted gameplay mod | Candidate estimate plus owner playtest and resulting fixes | Feel, controller, real save/load, mission/cutscene compatibility and visual judgement must be checked by the owner. Their duration cannot be inferred from automated receipts. |

The gore crash and original peak-cost limit are the largest uncertainties. If those require a deeper
native/plugin investigation, the complete candidate can exceed three weeks; the current evidence does
not establish a cause or a bounded repair time. A percentage-complete estimate would conceal that risk.
Revise this allowance after a valid C setup/active comparison and paired trunk performance evidence.

## Next work

1. Complete main offline regression of the startup deadline/evidence patch; preserve diagnostics even on timeout.
2. Assign D's corrected full held baseline under the single game slot, with fail-fast and restoration.
3. Review B's paired observation patch and run only the bounded trunk diagnostic it supports.
4. Schedule C's evidence-driven setup/active experiment, preserving original cleanup and cost requirements;
   validate R's corrected surface/camera fixtures in a separate bounded slot.
5. Merge only accepted changes; run affected combined gameplay/performance checks, prepare the exact build
   manifest and task-derived controls, then provide the feature playtest candidate.

This report supersedes the estimate section of the earlier whole-Stage-1 gap analysis for the owner's
current feature-first milestone. It does not declare the deferred remaster complete.
