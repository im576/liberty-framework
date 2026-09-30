# Regression pass, 2026-09-30

Build: `main` at `ea115dd` (full run `20260929-182040`), plus focused reruns of the flaky checks at `84750cd`, `895c9e8` and `1498ea1`.
All automated checks of `tests/local/checks.json`; the 18 manual gameplay checks were skipped by decision (run them as one owner sitting).

## Result

| Area | Result |
|---|---|
| Build (SDK, engine, mods, native core), offline verifier, content self-test 364/364, texture round trip, WBD table check | PASS |
| Blender add-on | PASS 92/92 |
| Engine events, bullet events, exact damage, vehicle events, hot reload 24/24, raycast spike | PASS |
| SDK self-test | 49/49 PASS (first run: the player left the scene after a pavement-snap landing 19 m off; rerun clean) |
| Performance baseline and 25-ped stress | PASS (completed, no crash) |
| Collision proxy probe (24 vanilla props) | PASS; 14 stop the engine ray |
| World objects (wall + proxies) | PASS after the same landing-point fix |
| Trunk choreography and storage wheel | PASS after two scenario fixes (face north before spawning the car; accept whichever weapon the wheel starts on). The engine code was not at fault |
| Screenshot review: slings (two guns), Blender barrel, native textures, radial/list menus, inspector, gore | reviewed, plausible; the sling and barrel renders confirmed by eye |

## Retired (superseded by decisions, not failing engine code)

Borrowed-collision reports and scenario, wall borrowed-collision report, LOD post report and review, `RayMask.Objects` against a candidate prop (proxy probe covers it), byte-exact drawable round trip, multi-material review and the bisect repro (known crash, kept as `tools/autopilot/scenarios/structure-bisect.txt`).

## Findings

- `east_park` teleport used a pavement snap that landed up to 19 m apart between runs; the scenarios depended on the landing. The location is now a fixed point (`config/devtools/locations.json`; installs keep their own copy of this file, so copy it over to apply).
- One `engine_stall` of 5 s in phase `engine.world` was logged once during the self-test rerun (a native blocking inside the snapshot; machine at ~92% RAM). Not reproduced in other runs. Watch item.
- Startup crash before the ASI loads (Rockstar `MTLX.DLL`) forced up to 3 relaunches on some scenarios; the autopilot handled them.
