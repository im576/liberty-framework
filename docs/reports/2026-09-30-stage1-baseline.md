# Stage 1 measurement baseline (T-040)

Measured 2026-09-29/30 on the owner's PC (RX 570 8 GB, hard-disk install, CE 1.2.0.59 + FusionFix, DXVK), build
`stage1/T-040`, through `tools/verify-local.ps1` and the `stage1-*` scenarios. Raw data and the comparison table:
[stage1-baseline-2026-09-30/](stage1-baseline-2026-09-30/) (`summary-mod-on.json`, `summary-mod-off.json`, `compare.md`,
`capture-points.jpg`). Findings from the code: [reuse audit](2026-09-29-stage1-reuse-audit.md). Owner sign-off items are
marked **NEEDS OWNER**.

## What was measured, and how

| | |
|---|---|
| Capture points | 12 fixed spots (`s1_*` in `config/devtools/locations.json`): 8 Broker/Dukes (Hove Beach x2, Firefly Island, Rotterdam Hill, BOABO, East Hook, Meadow Hills, East Island City) and 4 elsewhere (Star Junction, Chinatown, Bohan Boulevard, Alderney City). Coordinates from the game's own `paths.ipl` road nodes and `info.zon` zones. |
| Per point | Teleport (`teleport_done` required), 8 s streaming wait (24 s at Alderney), then day (13:00, overcast) and night (23:00, rain): 5 s settle, an 8 s sample window, then a clean screenshot. |
| Worst case | Hove Beach station street, night, rain, ambient traffic and pedestrians at the game's density, 12 armed subjects fighting in pairs, 25 extra pedestrians, the player shooting the nearest subject every 10 s, HUD on. One 60 s window. |
| Metrics | Frame statistics over the whole window (`framestats`: avg, p50, p95, p99, max, frames over 33 and 100 ms, stalls of 1 s or more), per-section script cost (`costs`), `perf` (private bytes, free address space, pressure), `pools`, dedicated GPU memory of the GTAIV process (Windows "GPU Process Memory" counters), CPU and game-disk load of the PC. |
| Mod-on | The current build, all Liberty modules running. |
| Mod-off | `stop gunplay/combat/arsenal/holsters/atmosphere` at the start (engine, DevTools and the autopilot stay), `restart` at the end. Cross-checked against `engine.json` `disabledModules` (below). |

## Results

### Capture points (24 windows of 8 s: 12 points, day and night)

| Metric | Mod-on | Mod-off |
|---|---|---|
| Frame time, mean of the 24 windows: avg / p50 / p95 / p99 | 25.0 / 22.4 / 39.5 / 66.7 ms | 25.1 / 22.9 / 37.9 / 50.1 ms |
| Frames over 100 ms (total in 24 windows), stalls of 1 s or more | 32, 0 | 7, 0 |
| `engine.frame` script cost, mean (range over points) | 2.08 ms (1.61 to 2.58) | 0.22 ms |
| Dedicated GPU memory, range over the run | 1212 to 1313 MB | 1245 to 1285 MB |
| Private bytes, highest | 2343 MB | 2330 MB |
| Free address space, lowest | 1096 MB | 1073 MB |
| Script cost per module (mean of the points) | gunplay 1.17, arsenal 0.58, atmosphere 0.19, combat 0.10, holsters 0.06, devtools 0.02 ms | devtools 0.02 ms |

Per-point rows with p50/p95/p99 and the budget verdicts are in [compare.md](stage1-baseline-2026-09-30/compare.md).

### Worst-case scene (60 s)

| Metric | Mod-on | Mod-off (`stop`) | Mod-off (`engine.json`) | Mod-on, density governor off |
|---|---|---|---|---|
| Frames sampled | 2 147 | 2 139 | 2 254 | 1 762 |
| avg / p50 ms | 37.85 / 34.6 | 38.10 / 35.2 | 35.72 / 32.8 | 49.83 / 48.6 |
| p95 / p99 ms | 60.1 / 108.6 | 58.9 / 91.0 | 56.3 / 83.7 | 75.3 / 113.0 |
| Max ms, frames over 100 ms, stalls of 1 s | 218.6, 28, 0 | 290.4, 17, 0 | 172.0, 13, 1 | 195.3, 30, 0 |
| `engine.frame` avg | 3.36 ms | 0.37 ms | (one 4.5 s load frame in the window skews the mean) | 3.69 ms |
| Module cost: gunplay / arsenal / holsters / combat / atmosphere | 1.24 / 0.60 / 0.07 / 0.98 / 0.19 ms | not running | not built | 1.29 / 0.75 / n/a / 1.10 / off |
| Worst single tick: combat / arsenal / gunplay | 147.7 / 22.1 / 8.2 ms | | | 139.4 / 17.4 / 8.8 ms |
| GPU dedicated | 1285 MB | 1285 MB | 1230 MB | 1262 MB |
| Private bytes | 2034 MB | 2083 MB | 1789 MB | 1789 MB (game session ages differ; not a mod cost) |
| Blood and effects evidence (log) | 10 `combat_hit`, 22 particle triggers | 0 | 0 | 7, 22 |
| Density governor's lowest values | peds 0.55, cars 0.60 | n/a | n/a | off |

Draw cost: `draw.ui` 0.002 to 0.003 ms and `draw.engine` 0.006 ms (nothing but the inspector is drawn today);
`draw.crosshair` appears only while aiming.

## Reading the numbers (limits of this baseline)

1. **The machine is CPU-bound and noisy.** System CPU was 70 to 98% in almost every window, with the game plus the other
   sessions' builds and browsers. Single 8 s windows vary by tens of percent between two visits to the same spot (hove_beach_b:
   p95 90 ms on, 39 ms off; dukes_meadow_hills: 17 ms on, 35 ms off). Point-level p95/p99 verdicts in `compare.md` (OVER)
   are therefore **not** evidence about the mod; only aggregates are. On the 24 windows the mean frame time is the same
   (25.0 vs 25.1 ms); p95 is +4% and p99 +33% on average, driven by a few hitch windows (frames over 100 ms: 32 vs 7).
2. **The density governor flatters mod-on.** `atmosphere.json` `density` lowered peds to 0.55 and cars to 0.60 in the
   mod-on runs (log lines `density`), so the mod-on scenes had fewer peds and cars than mod-off, exactly what STAGE1
   Pillar 5 forbids as a way to meet budgets. With the governor switched off (fourth column) the mod-on worst case ran at
   49.8 ms average, but that is a single sample of a random scene (peds 49 vs 47, similar counts) and is not directly comparable.
   The reliable statement: **the budget numbers below are measured with the governor's thinning included**.
3. **Mod-off by `stop` matches mod-off by `engine.json`.** Both leave `devtools`, `probe`, `world` and `autopilot`
   running and no Liberty gameplay module (`modules` reply logged); frame statistics agree within run-to-run noise
   (avg 38.1 vs 35.7 ms, p95 58.9 vs 56.3 ms). One caveat of `stop`: `restart combat` afterwards logs
   `skeleton_collapse_engine_failed ... hooked by another mod?` (the stopped instance's hook is still in place), so
   dismemberment falls back to its tick path until the game restarts. Run mod-off checks last in a session (the queue does).
4. **Windows without a full 8 s sample:** none; every window has 180 to 730 frames.
5. **Not measured here:** the 1-hour soak (memory growth per hour, `engine_stall` count), streaming hitching by eye
   (**NEEDS OWNER**), reticle/HUD/wheel draw cost (those features do not exist yet; the meters `draw.ui`,
   `draw.crosshair` and `draw.engine` are in place for T-043, T-045, T-046 and T-049).

## Budgets against the baseline (STAGE1 section 10)

| Budget (proposal unless noted) | Measured | Verdict |
|---|---|---|
| VRAM over vanilla: +250 to +300 MB normal, +350 MB ceiling (owner-confirmed) | mod-on vs mod-off: up to +68 MB at the capture points, 0 MB in the worst case. The mod adds no textures yet. | Room for the art passes: about 280 MB of the 350 MB ceiling is unused. |
| Stage 1 script cost in `engine.frame` <= 3 ms average at every point | 1.6 to 2.6 ms at the 24 windows; **3.36 ms in the worst case** (gunplay 1.24, combat 0.98, arsenal 0.60) | **Over in the worst case**, before any Stage 1 feature is added. |
| Gunplay + arsenal + holsters <= 1.5 ms average combined | 1.17 + 0.58 + 0.06 = **1.81 ms** idle at the points; 1.91 ms in the worst case | **Over** (gunplay's free-aim step alone is 0.6 ms). |
| No single tick of those modules above 5 ms outside menus | gunplay max 8.2 ms, arsenal max 22.1 ms in the worst case | **Over.** |
| Gore + effects <= 0.8 ms average, <= 4 ms peak (10-ped firefight) | `combat` 0.98 ms average, **147.7 ms** peak (`combat.pending`, 146 ms) | **Over** on both. |
| Frame p95 <= +10%, p99 <= +15% vs mod-off | worst case +2.0% / +19.3% (`stop` reference); 24-window mean +4% / +33% | p95 within; p99 over, but inside the noise (item 1). |
| No stall over 1 s outside teleports and loading | 0 in every window (one 4.5 s load frame in the `engine.json` run) | Met. |
| Density never below 0.8, no reduction to meet a budget | governor reached 0.55 / 0.60 | **Not met** by default config (finding F5). |
| Free address space >= 600 MB | lowest 1073 MB | Met. |

## Capture points and screenshots

`capture-points.jpg` lays out the 12 points: mod-on day, mod-off day, mod-on night, mod-off night. Reviewed by the agent
(RAN-PASS): every point is at ground level on a street or open ground in streamed-in city, none inside geometry or water.
Hove Beach A was moved from a service yard facing a wall to the station street, Firefly Island faces the fairground.
Composition varies because the game camera differs between runs. Several night frames are very dark (rain, no street
light nearby); that is the vanilla look at those spots, not a capture fault. Alderney needs about 20 s of streaming: its
mod-off day frame still shows the ground missing.
**NEEDS OWNER:** judge whether each point is representative enough of its area for the Slice B visual comparisons.

## Rerun

```
./tools/verify-local.ps1 -GameDirectory "<GTAIV>" -Branch <branch> -AnyBranch -NoPush -Restore -NoManual `
  -Only LOOP-package-install,T040-capture-broker,T040-capture-city,T040-worst-case,T040-capture-broker-off,T040-capture-city-off,T040-worst-case-off
./tools/perf/Measure-Stage1.ps1 -Reports results-local\<run> -Out summary.json
./tools/perf/Measure-Stage1.ps1 -On summary.mod-on.json -Off summary.mod-off.json -Markdown compare.md
./tools/perf/New-ContactSheet.ps1 -OnReports <broker,city report folders> -OffReports <off folders> -Out capture-points.jpg
```

Close other programs and leave the PC alone while it runs (it takes about 45 minutes). The summary lists log lines that show
input that did not come from the scenario and marks those windows NOT CLEAN; the first full attempt of this baseline was
disturbed that way (weapon switching, a sniper shot and a trunk opening in Bohan) and was rerun.
