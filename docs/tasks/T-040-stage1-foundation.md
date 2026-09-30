# T-040 — Stage 1 foundation: reuse audit, capture points, measurement baseline

Status: **READY** · Lane 0 (do first) · Depends on: nothing · Design: [STAGE1.md](../design/STAGE1.md) sections 5, 6, 10

## Goal

Everything later Stage 1 tasks need to be measured against, and a written map of what already exists. No gameplay
changes in this task.

## Scope

1. **Reuse audit** (STAGE1 section 6) → `docs/reports/<date>-stage1-reuse-audit.md`. For each Slice A feature (T-041 to
   T-049): the existing code, config and scenarios it extends (file paths), what is missing, risks, and anything that
   contradicts the design. Read the code; do not guess. Keep it factual and short (a table per feature).
2. **Capture points:** 12 fixed spots, `snap: "none"`, in `config/devtools/locations.json` with ids `s1_*`: 8 in
   Broker/Dukes (at least Hove Beach ×2, Firefly Island, Schottler/Rotterdam Hill, BOABO, East Hook, one Dukes road,
   one East Island City) and 4 elsewhere (Algonquin ×2, Bohan, Alderney). Take coordinates from existing data (the
   map's placement files via LibertyContent/LibertyModel readers, or existing locations), never from memory; confirm
   each with an in-game screenshot and fix any that land inside geometry or water.
3. **Scenarios** (`tools/autopilot/scenarios/`) and queue checks (`tests/local/checks.json`, regenerate the plan):
   - `stage1-capture`: at each capture point (day 13:00 overcast, night 23:00 rain), wait for streaming, screenshot,
     `perf`, `costs`. Parameterize by time/weather; keep the run under ~15 minutes.
   - `stage1-worst-case`: Broker/Dukes, night, rain, normal-to-high traffic, normal pedestrians, a firefight
     (`fight`/`fire`/`stress`), blood and effects active, HUD on; 60 s sample with `perf`/`costs`/memory.
4. **VRAM and memory measurement:** a script `tools/perf/Measure-Stage1.ps1` (or an extension of existing tooling)
   that records the game process's dedicated GPU memory (Windows performance counter `GPU Process Memory` for the
   GTAIV PID), private bytes and the engine's perf lines during a scenario, and writes a JSON summary (avg frame, p50,
   p95, p99, per-module cost, VRAM, memory, stalls).
5. **Baseline:** run both scenarios **mod-on** (current build) and **mod-off** (Liberty gameplay modules disabled via
   `engine.json` `disabledModules`, engine still loaded for measurement). Record the numbers in
   `docs/reports/<date>-stage1-baseline.md`. This is the reference every Stage 1 budget is compared to.

## Acceptance

- Audit report exists and covers T-041 to T-049.
- 12 capture points verified by screenshot; `stage1-capture` and `stage1-worst-case` pass in game.
- Baseline report has every metric STAGE1 section 10 Pillar 5 names, for mod-on and mod-off.
- Offline checks pass (build, verify, content self-test, `checks.py`, `artq.py validate`).

## Human test steps

Fill in when done: what the owner should look at in the capture screenshots, and how to rerun the baseline.
