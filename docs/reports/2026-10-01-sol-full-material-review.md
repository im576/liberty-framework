# Independent full SDK/material review after usage recovery

Run `20261001-200706-7d80c1d`, R checkout `GTAIV-Reborn-research-t050`, full mode, tested clean commit `7d80c1d`.
Gameplay remains first; remaster deferred. This receipt does not merge the candidate or mark a task DONE.

## Build, verifier and restoration

Production build, file verification and package installation passed. The file verifier reports 1020 passed / 0 failed.
This full run corroborates main `689bac5` staging generated inputs before verification and installation.
SDK, baseline material and extended coverage each remain NEEDS-REVIEW. No failed scripted steps or crash was reported.
Finished 2026-10-02T03:16:34.838Z; restored `phase2-20261001-200759`. Parent independently read summary, rollback and
installed metadata (03:16:34.465Z), then confirmed no game/compiler process before assigning B the next slot.

SDK selftest: all 53 service assertions passed, including ground material present/id13, Clear with no material,
ped material present/id99 and vehicle material present/id79. Its scenario had 14 steps / 0 failed / 2 ERROR lines:
engine stall frame661, 5169 ms, phase engine.commands, followed by the dump receipt. This prevents clean acceptance.
Extended coverage had 190 steps / 0 failed / 2 ERROR lines: frame3181, 5728 ms, phase module.arsenal, plus dump.
Phase labels locate the observed stall; they do not establish its underlying cause. Original budgets remain unchanged.

## Independent visual and numeric review

Parent opened all eight baseline and all 32 stamped coverage images. New stamps corroborate capture freshness;
filenames and model names alone do not establish the sampled surface. Baseline had 57 steps / 0 failed / 0 ERROR.
Its vehicle/prop frames do not show the intended target clearly; the raw forward ray hits a ped and the Vehicles ray
clears. Several world scenes/camera views do not establish the exact logged endpoint. Preserve numeric observations
without certifying those visual subjects or claiming a demonstrated screenshot-selection defect.

| Extended sample | What the actual evidence supports | Limit |
|---|---|---|
| Vehicle | Target/hit handle1030 agrees; lower ray id79, two higher rays id135. Correct stamps and vehicle visible. | Camera views rear of car; exact door/pane intersection is not established. Glass name needs effective-table identity and surface correspondence. |
| Crate | Handle16138 agrees in Objects and All at local z0.1, id84; z0.25 and z0.5 clear in both masks. | Player/holster/sling obscures the intended surface. Numeric attribution passes; visible wood correspondence remains unproven. |
| Planks | Handle17676 agrees in both masks, id20, same endpoints and hit plane. | Target partly visible behind player/holster/sling; clear surface correspondence remains unproven. |
| Barrier | Handle40975 agrees in both masks, id74, same endpoints and hit plane. | Target obstructed; id74 is not evidence of visible glass or a decoder bug. Ambient police fire also appears. |
| Solid control | Visible fountain/plaza; WaterHeight false, World hit z9.372, id3. | A numeric solid control, not a water hit. |
| Pool | Visible pool; WaterHeight true/z14, World hit z13.819, id3. | No water material hit observed. WaterHeight does not change the returned surface material. |
| Coast | Visible loaded water; WaterHeight true/z0, query Clear with UNKNOWN material. | No water material hit observed here; one Clear sample does not prove global lack of support. |

Effective loaded materials.dat/override provenance remains unproven. The committed offline row map cannot certify
runtime names. SDK/native/decoder sources require no speculative fix based on these observations.

## Disposition

R records the full receipt and repairs camera/readiness fixtures offline before another assigned run. No unchanged
retry or extra R launch is authorized. B alone gets full wheel/trunk/radial validation next; D remains ready after its
reviewed expiry repair. C prepares a separate host deadline/telemetry fix so a truncated startup can retain its result.
Original C gameplay failures, D hiding/text/unload gaps, B one-frame criterion and owner playtest remain open.

Evidence lives in R's `results-local/20261001-200706-7d80c1d`: summary, LOOP logs, each scenario report/result/run log,
all 40 captures and restore.log. R's live handoff carries the durable candidate receipt.
