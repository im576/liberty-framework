# T-026 — Performance and visual baseline

Status: **NEEDS-PLAYTEST**

## Framework-off result and targeted profile (2026-09-25)

The owner reports that with Liberty Framework's DLL absent, the game feels much smoother and generally playable, though random drops remain. The game loaded LVS, ran from about 00:34:13 to 00:36:50, and exited normally. WER also shows a separate attempted startup crash at 00:32:10 (`0xc0000005`, unknown module), so startup reliability remains open even with the framework absent. The smoothness report is qualitative: no PresentMon CSV was captured, and game focus/scene were not instrumented.

A diagnostic build now adds five low-cost per-tick timing buckets to `GunplayController`: setup/input, camera, bullet audit, weapon updates, and HUD/debug. It logs their average and maximum alongside the existing 30-second performance line. It does not change the gameplay calculations. Build: 113 source files, zero errors/warnings; offline verifier: 339 passed, 0 failed. The diagnostic DLL SHA-256 is `32E2FAEDA9DE19DD4C9C7468C778D7089BDC7ADBB98B549791BBDA5F7456699C`. The prior installed DLL remains at `D:\GTAIV-Reborn-Tools\baseline\framework-off-test-20260925\LibertyFramework.net.dll` with SHA-256 `F45293030B0C1CCB2562FC5A08F1D2E76A955C0ECAE2CCA808B0F43A0FEE9E51`.

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
