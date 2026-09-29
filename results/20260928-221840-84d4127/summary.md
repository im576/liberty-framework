# Verification run 20260928-221840-84d4127

- Commit: 84d4127 (codex/pr7-smoke-ray-fix)  Mode: full
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T05:18:40.7671910Z  Finished: 2026-09-29T05:55:04.0213479Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260928-223341
- Counts: PASS=12 NEEDS-REVIEW=10 FAIL=12 CRASH=1 ERROR=0 NOT-RUN=19

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-build` | pc-offline | **PASS** | exit code 0 | LOOP-build.log |
| `LOOP-verify` | pc-offline | **PASS** | exit code 0 | LOOP-verify.log |
| `LOOP-content-selftest` | pc-offline | **PASS** | exit code 0 | LOOP-content-selftest.log |
| `T028-wtdcheck` | pc-offline | **FAIL** | exit code 1 | T028-wtdcheck.log |
| `LOOP-blender-tests` | pc-offline | **NOT-RUN** | Blender is not configured (-Blender <blender.exe> once; it is remembered) |  |
| `T031-drawable-roundtrip` | pc-offline | **FAIL** | exit code 1 | T031-drawable-roundtrip.log, T031-drawable-roundtrip.json |
| `PROBE-drawables` | probe | **NEEDS-REVIEW** | report written; the review session reads it | PROBE-drawables.log, PROBE-drawables.json |
| `PROBE-collision` | probe | **NEEDS-REVIEW** | report written; the review session reads it | PROBE-collision.log, PROBE-collision.json |
| `PROBE-bounds-layout` | probe | **NEEDS-REVIEW** | report written; the review session reads it | PROBE-bounds-layout.log, PROBE-bounds-layout.json |
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260928-223341 | LOOP-package-install.log, LOOP-package-install-install.log |
| `T028-native-crate-report` | pc-offline | **PASS** | all expected fields match | T028-native-crate-report |
| `T028-alpha-panel-report` | pc-offline | **PASS** | all expected fields match | T028-alpha-panel-report |
| `T031-lod-post-report` | pc-offline | **FAIL** | compiled.drawableWriter=template (fallback) (expected structure); compiled.lodCount= (expected 4) | T031-lod-post-report |
| `T032-collision-borrow-report` | pc-offline | **FAIL** | compiled.collision.mode= (expected borrowed); compiled.collision.file= (expected lf_col_crate.wbn) | T032-collision-borrow-report |
| `T033-world-wall-report` | pc-offline | **FAIL** | compiled.collision.mode= (expected borrowed) | T033-world-wall-report |
| `T027-raycast` | scenario | **FAIL** | FAIL; steps=40 failed=1 logErrors=0 gameAlive=True | T027-raycast.log, T027-raycast |
| `T027-raycast-objects` | scenario | **FAIL** | FAIL; steps=21 failed=5 logErrors=0 gameAlive=True | T027-raycast-objects.log, T027-raycast-objects |
| `T027-raycast-spike` | scenario | **PASS** | PASS; steps=24 failed=0 logErrors=0 gameAlive=True | T027-raycast-spike.log, T027-raycast-spike |
| `SDK-selftest` | scenario | **PASS** | PASS; steps=13 failed=0 logErrors=0 gameAlive=True | SDK-selftest.log, SDK-selftest |
| `T028-native-texture-review` | scenario | **NEEDS-REVIEW** | NEEDS-REVIEW; steps=37 failed=0 logErrors=1 gameAlive=True | T028-native-texture-review.log, T028-native-texture-review |
| `T031-lod-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: lod_6m, lod_18m, lod_38m, lod_75m, lod_130m | T031-lod-review.log, T031-lod-review |
| `SDK-asset-review` | scenario | **NEEDS-REVIEW** | NEEDS-REVIEW; steps=33 failed=0 logErrors=1 gameAlive=True | SDK-asset-review.log, SDK-asset-review |
| `SDK-engine-events` | scenario | **PASS** | PASS; steps=23 failed=0 logErrors=0 gameAlive=True | SDK-engine-events.log, SDK-engine-events |
| `SDK-bullet-events` | scenario | **PASS** | PASS; steps=13 failed=0 logErrors=0 gameAlive=True | SDK-bullet-events.log, SDK-bullet-events |
| `SDK-exact-damage` | scenario | **PASS** | PASS; steps=17 failed=0 logErrors=0 gameAlive=True | SDK-exact-damage.log, SDK-exact-damage |
| `SDK-vehicle-events` | scenario | **PASS** | PASS; steps=13 failed=0 logErrors=0 gameAlive=True | SDK-vehicle-events.log, SDK-vehicle-events |
| `SDK-hot-reload` | scenario | **FAIL** | FAIL; steps=22 failed=1 logErrors=1 gameAlive=True | SDK-hot-reload.log, SDK-hot-reload |
| `SDK-inspector-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: inspector, inspector_after_restart | SDK-inspector-review.log, SDK-inspector-review |
| `SDK-ui-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: sdk_list_menu, sdk_list_menu_moved, sdk_radial_menu | SDK-ui-review.log, SDK-ui-review |
| `T022-gore-review` | scenario | **NEEDS-REVIEW** | passed; screenshots to judge: arm_cut, leg_cut, head_cut | T022-gore-review.log, T022-gore-review |
| `T021-sling-review` | scenario | **FAIL** | FAIL; steps=31 failed=1 logErrors=1 gameAlive=True | T021-sling-review.log, T021-sling-review |
| `T024-trunk-review` | scenario | **FAIL** | FAIL; steps=34 failed=2 logErrors=0 gameAlive=True | T024-trunk-review.log, T024-trunk-review |
| `T026-perf-baseline` | scenario | **NEEDS-REVIEW** | NEEDS-REVIEW; steps=10 failed=0 logErrors=1 gameAlive=True | T026-perf-baseline.log, T026-perf-baseline |
| `T026-perf-stress` | scenario | **CRASH** | CRASH; steps=6 failed=1 logErrors=0 gameAlive=False | T026-perf-stress.log, T026-perf-stress |
| `T032-collision-borrow` | scenario | **FAIL** | FAIL; steps=20 failed=2 logErrors=0 gameAlive=True | T032-collision-borrow.log, T032-collision-borrow |
| `T033-world-objects` | scenario | **FAIL** | FAIL; steps=16 failed=1 logErrors=0 gameAlive=True | T033-world-objects.log, T033-world-objects |
| `T007-confirm-done` | manual | **NOT-RUN** | skipped by the owner |  |
| `T010-phase1` | manual | **NOT-RUN** | skipped by the owner |  |
| `T011-gold-finishes` | manual | **NOT-RUN** | skipped by the owner |  |
| `T013-debug-hit-info` | manual | **NOT-RUN** | skipped by the owner |  |
| `T014-aim-profiles` | manual | **NOT-RUN** | skipped by the owner |  |
| `T015-shoulder-swap` | manual | **NOT-RUN** | skipped by the owner |  |
| `T016-switch-while-aiming` | manual | **NOT-RUN** | skipped by the owner |  |
| `T017-feel-shake-fov` | manual | **NOT-RUN** | skipped by the owner |  |
| `T024-gun-feel` | manual | **NOT-RUN** | skipped by the owner |  |
| `T020-storage-basics` | manual | **NOT-RUN** | skipped by the owner |  |
| `T024-storage-prompt` | manual | **NOT-RUN** | skipped by the owner |  |
| `T025-gunsmith` | manual | **NOT-RUN** | skipped by the owner |  |
| `T020-death-mission-persistence` | manual | **NOT-RUN** | skipped by the owner |  |
| `T021-holsters` | manual | **NOT-RUN** | skipped by the owner |  |
| `T022-combat-effects` | manual | **NOT-RUN** | skipped by the owner |  |
| `T023-lvs-labels` | manual | **NOT-RUN** | skipped by the owner |  |
| `T026-performance-feel` | manual | **NOT-RUN** | skipped by the owner |  |
| `T033-world-walk` | manual | **NOT-RUN** | skipped by the owner |  |
