# Liberty Framework

The **Liberty engine** for **GTA IV: The Complete Edition**: a native core (
ative/), a public C# SDK (sdk/), a content
compiler and Blender add-on for authoring models and textures, hot reload, an autopilot that launches and tests the game, and
the gameplay modules built on them (Gunplay, Arsenal, holsters/slings, trunk, gore), which are kept as reference mods.
The next phase builds the first complex mod on the engine; after that, Phase 3 is reverse engineering toward FiveM-level control.

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
| `docs/archive/` | History: original spec (`HANDOFF.md`), Phase 2 plan and patch notes, old task cards, agent reports, the full project diary |
| `docs/PROJECT_STATE.md` | Short status dashboard (engine components, decisions, where things are) |
| `docs/research/` | Research notes (FusionFix, Liberty Tweaks, recoil mods, Mafia III, …) |
| `docs/architecture/` | Overview, config schema, decisions (ADRs) |
| `docs/game-api/` | Registry of natives and memory patterns we use, with CE status |
| `docs/tasks/` | Active task cards for agents (finished ones are in `docs/archive/tasks/`) |
| `docs/testing/` | Test matrix, playtest report template, generated local verification plan |
| `src/` | C# engine host and built-in modules (LibertyFramework.net.dll), including the reference gameplay modules |
| `sdk/` | Liberty.Sdk, the public API mods build against |
| `native/` | LibertyCore, the C++ core (world snapshot, hooks, raycast) |
| `mods/` | SDK-only mods (the autopilot, world objects) |
| `content/` | Source assets for the content compiler (glTF, Blender) |
| `tests/local/` | The queue of checks that need the owner's PC |
| `config/` | JSON tuning/config files |
| `assets/` | Finish definitions (no game assets are committed) |
| `tools/` | Build, verify, package, autopilot, content compiler, Blender add-on, cloud and local verification; index in [tools/README.md](tools/README.md), old one-shot scripts in `tools/archive/` |
| `third_party/` | Third-party code/binaries + license tracking |
