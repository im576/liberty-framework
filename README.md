# Liberty Framework

Liberty is our engine and mod project for **GTA IV: The Complete Edition 1.2.0.59**.
A C++ native core, C# SDK and module host support gunplay, physical weapons, storage,
gore, UI and world features, with content tools, hot reload and automated game testing.
The larger vision includes deeper gore, vehicle ownership and engine-level overhauls.

Start with [current project state](docs/PROJECT_STATE.md), [the mod design](docs/design/STAGE1.md)
and [the research program](docs/research/RESEARCH_PROGRAM.md). The installed preview and
unmerged feature lanes have different capabilities; a passing engine build is not full mod acceptance.

- Agents: [AGENTS.md](AGENTS.md), [current coordination](docs/workflow/ORCHESTRATOR.md).
- Build/test commands: [tools index](tools/README.md).
- Mod authors: [SDK](docs/sdk/README.md), [engine](docs/architecture/ENGINE.md), [content pipeline](docs/content/README.md).
- Current review: [October 2 findings](docs/reports/2026-10-02-repository-review.md).
- Playtesting: [feature test plan](docs/testing/FEATURE_PLAYTEST.md), [report template](docs/testing/PLAYTEST_REPORT_TEMPLATE.md).

| Path | Purpose |
|---|---|
| `native/LibertyCore` | Native invocation, hooks, snapshots, ray queries and crash capture |
| `src/LibertyFramework` | Engine host/services and reference gameplay modules |
| `sdk/Liberty.Sdk` | Public mod API |
| `mods/` | SDK-only mods and autopilot |
| `config/` | Switches, gameplay tuning and data |
| `content/`, `art/` | Authored assets and generation/approval provenance |
| `tools/`, `tests/` | Build/content tools, offline checks and game test queue |
| `.agents/skills/` | Project-specific evidence review and engine research skills |
| `docs/research/`, `third_party/` | Engine findings and source/license provenance |
| `docs/archive/` | Historical plans, completed sessions and superseded handoffs |

Downloaded mods, game files, generated packages and local run/crash evidence stay outside
tracked source. Local worktrees can contain changes newer than GitHub; inspect them before integration.
