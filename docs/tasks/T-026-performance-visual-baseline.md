# T-026 — Performance and visual baseline

Status: **NEEDS-PLAYTEST**

## Step 1 probe and async DXVK (2026-09-25, Claude)

**Owner run with the memory fast paths:**

- Normal-play frames were 15â€“24 ms p50; the camera phase dropped to 0.01â€“0.3 ms.
- `combat.dismember` was the remaining spike (45â€“66 ms per tick) until the one-cut-per-body pass.
- Per `native_cost`, an SHDN native costs 109â€“182 Âµs.

**Changes:**

- **`EngineThreadProbe`** (GameApi).
  - Reads the engine frame counter at gunplay tick start and end: `GET_FRAME_COUNT` â†’ getter â†’ global `0x1173604` on 1.2.0.59 (verified).
  - Logs `engine_thread_probe ticks= frame_advanced_during_tick= ticks_same_frame= ticks_next_frame= ticks_after_skipped_frames=` every 30 s.
  - It also calls `GET_CHAR_HEALTH`'s own handler (0xB9EE50) directly from our thread on the player: 20 calls, compared with the SHDN call and timed (`direct_native get_char_health ... match= direct_us= shdn_us=`).
  - Read-only; this is the evidence gate for direct natives.
- **DXVK GPLAsync 2.6.2** (owner-downloaded Nexus mod 385), installed with `tools/install-dxvk-gplasync.ps1`.
  - Settings: async shader compilation, 2 compiler threads, frame latency 1, the FusionFix 5.0.1 prebuilt shader cache.
  - Backup: `scripts/LibertyFramework/backups/dxvk-gplasync-20260925-024524`.
  - Violent Liberty stays on Vulkan.

## Memory-check pass (2026-09-25, Claude, after owner run 09:01-09:05)

**Evidence from the owner's run:**

- **Scripts run on separate threads.** gunplay is thread 11, combat 9, arsenal 7, holsters 8, devtools 10.
- **Native calls are not the bottleneck.** One script native call costs 150-166 us (`native_cost`), and `cam.aim_key`, a native, averaged 0.01 ms.
- **Memory checks are.** `cam.handle` and `cam.find_active` still cost about 5 ms each while doing only memory reads. The cause is `LiveMemory`'s `VirtualQuery` checks, which take the process address-space lock and wait while DXVK and the streamer allocate.
- **The gore script scales with the fight.** `tick.combat` rose from 0.35 ms to 18-62 ms per tick as severed corpses accumulated, and frame p50 went from 28-30 ms to 84-92 ms. Early frames were already better than the previous run's 60-92 ms.

**Changes:**

- **`LiveMemory` fast paths.** Reads inside GTAIV.exe's non-executable sections skip `VirtualQuery`. So do ranges registered with `Trust()` after one full check: the camera and ped rage pools (header, object array, flag array), which are allocated once. Heap objects (skeletons, fragInsts) are still checked on every access.
- **Dismemberment upkeep.** With the engine collapse installed, upkeep runs every `dismemberRefreshMilliseconds` (150 ms) once all records are older than 1 s.
- **Finer combat timings.** New `combat.dismember`, `combat.pending`, `combat.blood` and `combat.sample` sections in `performance_scripts`.

**Open:** one `dismember_skip cut_bone_unresolved part=right_leg_knee tag=0x1A8` (the knee matrix did not match uniquely on that model).

## Script cost pass (2026-09-25, Claude)

**Findings from the logs and WER archive:**

- **Startup crashes are not Vulkan or Violent Liberty.** Every `0xc0000005` / `StackHash_2beb` startup crash since 2026-09-20 has the same signature, in `ntdll+0x7379C`. That includes 09-20 (before any mods) and 09-23/09-24 (DirectX 9). In each report the module list ends right after Rockstar's `MTLX.DLL` loads, with `gameoverlayrenderer.dll` loaded before it. No FusionFix ASI, ScriptHook or Liberty Framework module is loaded yet. Suggested owner-side mitigations:
  - start the Rockstar Games Launcher and let it sign in before pressing Play;
  - A/B with the Steam overlay disabled for GTA IV.
- **The single `scripthook.dll` `0xc0000417` crash** while loading LVS (09-25 00:13) is separate.
- **The RX 570 driver reports no graphics pipeline library** (`DXVK: Graphics pipeline libraries not supported`), so DXVK compiles pipelines on first use. That explains hitches that remain with the framework off. Vulkan stays, because Violent Liberty requires it.
- **The framework-off run was smooth** with Vulkan, DXVK and Violent Liberty still installed, so the framework's own cost is the main fixable part.

**Changes:**

- **`CostMeter` (Core/Performance/Logic).** Every script tick (gunplay, combat, holsters, arsenal, devtools) and each gunplay camera step records its wall-clock cost and thread. The 30 s `performance` log line is followed by a `performance_scripts` line with average/maximum/count per section. A one-time `native_cost` line measures one script native call.
- **Gunplay camera phase.**
  - The game camera handle is refreshed from `GET_GAME_CAM` every `performance.gameCameraRefreshMilliseconds` (500 ms). In between, its camera-pool slot/generation is validated from memory, replacing `DOES_CAM_EXIST`.
  - FOV is read every tick only while aiming; otherwise every `fovRefreshMilliseconds` (250 ms).
  - The aim-camera lookup is skipped when the game camera is invalid.
- **Combat damage sampling.**
  - Full rate (50 ms) only within `activeSampleWindowMilliseconds` (3000 ms) of the player's last detected shot. Otherwise the scan runs every `idleSampleIntervalMilliseconds` (400 ms) to keep health baselines.
  - Per ped: one health read. Vehicle, attribution and death checks happen only when health dropped or a recent hit awaits its death. The per-ped `isDead` call every scan is gone.

## Diagnostic run and latest launch report (2026-09-25)

The owner reports a failed launch and intends to retry. The installed diagnostic DLL did complete an earlier run from roughly 01:08:45 to 01:17:57: all eight scripts started, the Direct3D device was later lost, and scripts terminated normally. Across 18 half-minute timing windows / 17,094 GunplayController ticks, its weighted average work was 16.71 ms; the camera phase alone averaged 10.11 ms (60.5%). Camera work stayed near 9–10 ms in the initially faster windows, then rose to about 12–13 ms as frame spacing worsened. Bullet audit averaged roughly 0.2 ms and HUD roughly 0.6 ms. The internal frame-spacing p50 rose from 24–25 ms early to 80–92 ms late; this remains a script tick proxy, not PresentMon frame data. CSV: `D:\GTAIV-Reborn-Tools\captures\gunplay-phase-timings-20260925-0108.csv`.

WER recorded multiple earlier startup attempts with `0xc0000005` / unknown fault module, including 00:50, 00:52, and 01:05. The latest archived report has `d3d9.dll` and local `vulkan.dll` loaded but no ScriptHook or ASI module yet. A separate 00:13 crash faulted in `scripthook.dll`. The repeated early crashes and runtime slowdown are distinct observations; neither has a proven root cause. No installed game file was changed during the owner's retry. Next performance step is to split the dominant camera phase into native handle, active-camera lookup, aim state, and FOV/projection timings before optimizing it. Startup reliability needs a separate renderer-path A/B after the retry.

## Framework-off result and targeted profile (2026-09-25)

The owner reports that with Liberty Framework's DLL absent, the game feels much smoother and generally playable, though random drops remain. The game loaded LVS, ran from about 00:34:13 to 00:36:50, and exited normally. WER also shows a separate attempted startup crash at 00:32:10 (`0xc0000005`, unknown module), so startup reliability remains open even with the framework absent. The smoothness report is qualitative: no PresentMon CSV was captured, and game focus/scene were not instrumented.

A diagnostic build now adds five low-cost per-tick timing buckets to `GunplayController`: setup/input, camera, bullet audit, weapon updates, and HUD/debug. It logs their average and maximum alongside the existing 30-second performance line. It does not change the gameplay calculations. Build: 113 source files, zero errors/warnings; offline verifier: 339 passed, 0 failed. The diagnostic DLL SHA-256 is `32E2FAEDA9DE19DD4C9C7468C778D7089BDC7ADBB98B549791BBDA5F7456699C`. The prior installed DLL remains at `D:\GTAIV-Reborn-Tools\baseline\framework-off-test-20260925\LibertyFramework.net.dll` with SHA-256 `F45293030B0C1CCB2562FC5A08F1D2E76A955C0ECAE2CCA808B0F43A0FEE9E51`.

With GTA IV closed, the diagnostic DLL was copied into `scripts\LibertyFramework.net.dll` and its installed SHA-256 rechecked. The prior backup was checked before installation. No other game file changed. The in-game phase output is summarized above.

## Startup crash isolation (2026-09-25)

The owner's next attempt froze on the loading screen, then crashed at 00:13:37. Windows Application Error 1000 records `GTAIV.exe` 1.2.0.59 faulting in `scripthook.dll` 0.5.1.0 at offset `0x00020861`, exception `0xc0000417`. ScriptHookDotNet logged Direct3D device creation at 00:13:23, then found the Liberty Framework assembly and began loading `LibertyVehicleServicesCE.CS` at 00:13:27; no Liberty Framework or LVS startup entry appeared for this attempt. DXVK initialized the RX 570 without an explicit error in its log. Every copied baseline config hash still matches. This narrows the failure to startup script/hook interaction, but the faulting module alone does not prove which mod caused it.

With GTA IV closed, `scripts\LibertyVehicleServicesCE.CS` was moved byte-for-byte to `D:\GTAIV-Reborn-Tools\baseline\startup-crash-20260925\LibertyVehicleServicesCE.CS` (SHA-256 `CE8C5797C8C41430DBCA82D26768C64BD27131A59E98DE6C1AE30A1A342F5C90`). An attempted launch at 00:21:08 still crashed before ASI modules loaded: WER reports `StackHash_2beb`, `0xc0000005`, with `d3d9.dll`/`vulkan.dll` present. A retry at 00:21:25 loaded successfully without LVS, so its removal did not eliminate startup crashes. After the successful run closed at 00:27:29, the LVS script was restored and its hash rechecked. The two fault signatures should be investigated separately; neither identifies a root cause yet.

## First live performance run (2026-09-25)

The successful no-LVS run loaded all seven Liberty Framework scripts. Its internal tick-spacing proxy (not an external frame trace) went from 22 ms p50 / 30 ms p95 in the first 30-second window to 50–94 ms p50 and 103–188 ms p95 in later windows. Tick counts fell from 1240 to 273–520 per 30 seconds. The GunplayController's own measured tick work averaged 14–36 ms across windows, a substantial contributor, but below total frame spacing. Its p99 histogram caps at 200 ms, so worse spikes cannot be quantified from this log. Timing table: `D:\GTAIV-Reborn-Tools\captures\lf-timings-20260925-0022.csv`.

A 45-second process sample averaged 27.1% CPU across four logical cores (maximum 38.7%), with private memory rising from 1621 to 1699 MB. Three GPU 3D readings ranged from 21% to 65%; dedicated GPU memory held near 1203 MB. Those snapshots do not prove a single bottleneck or confirm the game stayed foreground throughout. Process sample: `D:\GTAIV-Reborn-Tools\captures\process-baseline-20260925-002433.csv`. The game exited cleanly after the run; no new crash was recorded at exit.

For the next A/B, with the game closed, `scripts\LibertyFramework.net.dll` was moved to `D:\GTAIV-Reborn-Tools\baseline\framework-off-test-20260925\LibertyFramework.net.dll` (SHA-256 `F45293030B0C1CCB2562FC5A08F1D2E76A955C0ECAE2CCA808B0F43A0FEE9E51`). FusionFix, DXVK, Violent Liberty, and LVS remain installed. This temporarily removes Liberty Framework gunplay, gore, DevTools, Arsenal, and holsters. Restore the DLL after the comparison with GTA IV closed and verify the same hash.

## Scope

The owner reports 15–20 FPS with continual dips and flicker in shadows, lights, or buildings. This task prepares repeatable frame-time measurement and a read-only engine research workspace before choosing graphics or gameplay changes. The goal is a darker, more cohesive GTA IV look that remains visible and runs smoothly on the owner's Ryzen 3 2300X / RX 570 (~4 GB VRAM).

## Setup completed (2026-09-25)

- Portable Intel PresentMon 2.6.0 and its signed MSI are downloaded to `D:\GTAIV-Reborn-Tools\downloads`. The portable CLI runs; the MSI install failed with Windows Installer 1603 / insufficient privilege, so capture uses the portable CLI.
- Ghidra 12.1.4 and JDK 25.0.4+1 are extracted under `D:\GTAIV-Reborn-Tools`. Headless import of the installed CE `GTAIV.exe` succeeded into `D:\GTAIV-Reborn-Tools\analysis\GTAIV-CE.gpr` without automatic analysis. This is a read-only research copy, not game source code.
- FusionFix and DXVK sources are cloned into `D:\GTAIV-Reborn-Tools\sources` for reference, with no code copied into this project.
- `tools/capture-performance.ps1` captures a labeled PresentMon CSV while the owner runs the game. Windows requires an elevated shell for the ETW capture; non-elevated capture returned access denied.
- A copy and hash manifest of installed configuration is under `D:\GTAIV-Reborn-Tools\baseline`. No installed game file was changed by this setup.

## Known evidence and working hypotheses

- Installed stack: GTA IV CE 1.2.0.59, FusionFix 5.0.1, DXVK 2.6.2 (Vulkan), Violent Liberty, Liberty Framework, 1920×1080.
- Prior Liberty Framework log windows showed frame times worsening from roughly 25–32 ms p50 early to 86–124 ms, sometimes 165 ms, then recovering. GunplayController tick averaged roughly 15–18 ms in heavy windows. This suggests script work may contribute to poor pacing, but is not a full CPU/GPU breakdown.
- The owner identifies the flicker as shadows/lights/buildings. FusionFix currently sets `ExtraDynamicShadows=2`; test whether a rendering setting or mod interaction is involved only after a comparable baseline capture. Do not assume the blood system causes world flicker.
- The Nexus HQ Vanilla Textures City Revitalization LITE pack is a possible future visual comparison, not part of the required baseline. It replaces large city texture archives and may increase streaming/VRAM pressure on this hardware. The Nexus page currently requires login to download.

## Human test steps

1. With the diagnostic Liberty Framework DLL installed, launch GTA IV through Steam and load the same save/location. All framework features should be present; report any loading failure.
2. Stay in the game window for at least 90 seconds, using the same street/route and then briefly aiming/firing. The framework log should print at least two `performance ... phase_setup_avg_ms=... phase_camera_avg_ms=...` lines. Report whether choppiness returns and where it is most noticeable.
3. For exact external frame timing, open **PowerShell as Administrator** after the game loads. From `C:\Users\IM576\GTAIV-Reborn`, run `./tools/capture-performance.ps1 -Label framework-profile -Seconds 120`, then return focus to the game for the capture. A nonempty CSV should appear under `D:\GTAIV-Reborn-Tools\captures`.
4. Close GTA IV before changing DLLs. The backup above can restore the prior framework build, or be left out for the proven smoother comparison state. Do not mark this task DONE until the phase logs, comparable frame traces, and visual observation exist.

## Next analysis

Read CSV frame-time percentiles and CPU/GPU busy time, align them with Liberty Framework logs, then test one variable at a time: script/gore load, FusionFix shadow configuration, DXVK/driver behavior, and streaming/texture pressure. Prioritize fixes that improve p95/p99 frame pacing. Choose a dark but legible graphics treatment only after stable performance and flicker cause are measured.
