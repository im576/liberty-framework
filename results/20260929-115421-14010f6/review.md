# Windows review of focused runtime verification

Source commit `14010f6`, GTA IV CE 1.2.0.59. The automated summary and scenario logs in this directory are the primary evidence.

| Check | Review |
| --- | --- |
| SDK hot reload | Failed two steps. `goto east_park` exceeded the 20-second command deadline during a 29.9-second engine frame stall; its reply arrived later and teleport completed. Hot reload of autopilot and restart of devtools then worked. `selftest_done passed=48 failed=1` **did appear** at 19:04:40.611Z; the scenario timed out because it required `failed=0`. Immediately before that, the log records `arsenal_loss reason=wasted` and `choreography_cancel selftest step=1 reason=a ped is gone`. The player died during the selftest after autopilot reload released its owned invincibility. |
| T-021 sling | Script passed 31/31 steps with no log errors. Five screenshots show the slung long gun attached across Niko's back, including a close rear view; no obvious detached or missing model is visible. Front and right views show less of it due to body occlusion. Visual review supports the attachment, but does not establish animation behavior. |
| T-024 trunk | Failed five expected log events. Spawn and `at-trunk` succeeded, yet `choreography_begin trunk`, storage open/store/close and choreography complete never appeared. The open, after-store and closed screenshots all show the prompt `Press X / E to use the trunk.` and no storage UI. The interaction did not start in this run; the reason the simulated E press was ineffective is not established. |

Restore verification: the Phase 2 rollback reported completion from `phase2-20260929-120203`. An independent SHA-256 comparison of all 42 recorded restored targets against their saved originals found **0 mismatches**. The game install was restored.

PowerShell simulation tests for `mark`/`expectmarked` and other autopilot behavior: **19 passed, 0 failed** (`tools/tests/Run-Tests.ps1 -Filter AutopilotSimulation`).
