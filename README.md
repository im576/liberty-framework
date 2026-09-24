# Liberty Framework

Modern gunplay + developer tooling foundation for **GTA IV: The Complete Edition**.

Phase 1 goal: three gold test weapons (pistol, carbine, pump shotgun) that use free aim,
data-driven recoil, and a clean spread crosshair — tuned live from an in-game developer menu —
while every vanilla weapon stays vanilla for A/B comparison.

> **Status:** T-001 loads and reloads in gameplay. T-002's live config reload, malformed-input retention, and recovery log checks passed; tester confirmation of continued gameplay is pending. No gunplay changes yet. See [docs/PROJECT_STATE.md](docs/PROJECT_STATE.md).

## For AI agents

Start at [AGENTS.md](AGENTS.md). Then [docs/tasks/README.md](docs/tasks/README.md).

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
| `src/` | C# runtime probe source |
| `config/` | JSON tuning/config files |
| `assets/` | Models/textures (gold finish etc.) |
| `tools/` | Build/deploy scripts |
| `third_party/` | Third-party code/binaries + license tracking |
