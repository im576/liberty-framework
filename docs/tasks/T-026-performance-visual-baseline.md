# T-026 — Performance and visual baseline

Status: **NEEDS-PLAYTEST**

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

1. Launch GTA IV through Steam and load the same save/location used for visual testing. Leave the current mod/settings stack in place for the first run.
2. Open **PowerShell as Administrator**. From `C:\Users\IM576\GTAIV-Reborn`, run `./tools/capture-performance.ps1 -Label baseline-street -Seconds 120` while standing or walking in the same street for two minutes. A nonempty CSV should appear under `D:\GTAIV-Reborn-Tools\captures`.
3. Repeat with `-Label baseline-drive` while driving a repeatable city route, then `-Label baseline-combat` during the gunfight/limb test. If the slowdown accumulates, capture `-Label late-session` after it begins.
4. Report which capture corresponds to the worst choppiness; provide a short clip or screenshot of the world flicker if possible. Do not mark this task DONE until in-game captures and a visual observation exist.

## Next analysis

Read CSV frame-time percentiles and CPU/GPU busy time, align them with Liberty Framework logs, then test one variable at a time: script/gore load, FusionFix shadow configuration, DXVK/driver behavior, and streaming/texture pressure. Prioritize fixes that improve p95/p99 frame pacing. Choose a dark but legible graphics treatment only after stable performance and flicker cause are measured.
