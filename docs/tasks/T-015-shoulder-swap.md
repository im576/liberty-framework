# T-015 — Shoulder swap

Status: **BLOCKED** (Codex Agent B). Controller-first shoulder swap while aiming remains an engine API spike. Liberty Tweaks was used as behavioural reference only.

## Blocked

Checked the registered CE native list and ScriptHookDotNet camera wrapper. `SET_CAM_ATTACH_OFFSET` and `SET_CAM_POINT_OFFSET` exist, but their behaviour on the gameplay third-person aim camera is not documented or validated. The existing ADR-0004 aim-camera resolver validates pitch/heading only, and `docs/game-api/MEMORY.md` has no lateral offset field or restore contract. Applying a guessed offset could alter cover, vehicles or mission cameras. No code was written for this feature. Need a disassembly-backed resolver with runtime validation and original-value restore, or a human-tested native spike showing a camera-local offset on CE 1.2.0.59.

Phase 2 research (2026-09-24): the current [Liberty Tweaks source](https://github.com/catsmackaroo/LibertyTweaks/blob/main/LibertyTweaks/Features/Combat/ShoulderSwap.cs) swaps the shoulder by attaching an invisible, collision-enabled object near the ped while aiming. That is a camera-obstruction technique rather than a validated camera offset, and it has no documented safe restore behavior for this project's CE camera. The repository has no reuse license. We will not copy that code or add an unseen collision object to the integrated build. `SET_CAM_ATTACH_OFFSET` exists in the CE native list, but its behavior on the active gameplay aim camera and its original value remain unverified. The task stays blocked while the camera control is investigated.

## Human test steps after unblock

1. Hold L2/LT on foot, press the configured shoulder button and verify the view changes right/left without moving aim or player position.
2. Repeat while aiming from cover, in a vehicle and during mission camera changes; the setting must restore after aim ends and script reload.
