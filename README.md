# Liberty Framework

Modern gunplay + developer tooling foundation for **GTA IV: The Complete Edition**.

Phase 1 goal: three gold test weapons (pistol, carbine, pump shotgun) that use free aim,
data-driven recoil, and a clean spread crosshair — tuned live from an in-game developer menu —
while every vanilla weapon stays vanilla for A/B comparison.

> **Status:** Liberty Engine (a native core plus C# modules) and the Liberty SDK carry Phase 1 and 2 gameplay, a content
> compiler and the autopilot. Development runs in cloud sessions; the owner's PC verifies with one command
> ([workflow](docs/workflow/CLOUD_LOCAL_LOOP.md), [what is queued](docs/testing/LOCAL_VERIFICATION_PLAN.md)). See
> [docs/PROJECT_STATE.md](docs/PROJECT_STATE.md) and [docs/workflow/NEXT_SESSIONS.md](docs/workflow/NEXT_SESSIONS.md).

## For AI agents

Start at [AGENTS.md](AGENTS.md), then [docs/workflow/CLOUD_LOCAL_LOOP.md](docs/workflow/CLOUD_LOCAL_LOOP.md) and [docs/workflow/NEXT_SESSIONS.md](docs/workflow/NEXT_SESSIONS.md). Offline checks: `tools/cloud/test-all.sh`.

## For mod authors

[docs/sdk/README.md](docs/sdk/README.md) (SDK), [docs/architecture/ENGINE.md](docs/architecture/ENGINE.md) (engine), [docs/content/README.md](docs/content/README.md) (models and textures).

## For the human tester

- Game setup: [docs/setup/ENVIRONMENT.md](docs/setup/ENVIRONMENT.md)
- Reporting a playtest: [docs/testing/PLAYTEST_REPORT_TEMPLATE.md](docs/testing/PLAYTEST_REPORT_TEMPLATE.md)
- Test matrix: [docs/testing/TEST_MATRIX.md](docs/testing/TEST_MATRIX.md)

## Map

| Path | What |
|---|---|
| `AGENTS.md` | Rules every agent follows |
| `docs/HANDOFF.md` | Original project spec (design intent) |
| `docs/PROJECT_STATE.md` | Live status: done / verified / blocked |
| `docs/research/` | Research notes (FusionFix, Liberty Tweaks, recoil mods, Mafia III, …) |
| `docs/architecture/` | Overview, config schema, decisions (ADRs) |
| `docs/game-api/` | Registry of natives and memory patterns we use, with CE status |
| `docs/tasks/` | Small, ordered task cards for agents |
| `docs/testing/` | Test matrix, playtest report template |
| `src/` | C# engine and built-in modules (LibertyFramework.net.dll) |
| `sdk/` | Liberty.Sdk, the public API mods build against |
| `native/` | LibertyCore, the C++ core (world snapshot, hooks, raycast) |
| `mods/` | SDK-only mods (the autopilot) |
| `content/` | Source assets for the content compiler (glTF, Blender) |
| `tests/local/` | The queue of checks that need the owner's PC |
| `config/` | JSON tuning/config files |
| `assets/` | Finish definitions (no game assets are committed) |
| `tools/` | Build, verify, package, autopilot, content compiler, Blender add-on, cloud and local verification |
| `third_party/` | Third-party code/binaries + license tracking |
