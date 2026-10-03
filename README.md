# Liberty Framework

Reusable mod runtime and research toolkit for GTA IV Complete Edition 1.2.0.59.
GTA IV remains the underlying game engine. Liberty supplies a native bridge,
shared services, a public SDK, content tools and automated verification.

The showcase overhaul is now **[Liberty+](../LibertyPlus/README.md)**, in its own Git
repository. Weapons, inventory, holsters, wheel, trunk storage, gore, custom HUD,
vehicle gameplay, AI artwork and visual tuning belong there.

Start with [current status](docs/PROJECT_STATE.md), [repository ownership](docs/architecture/REPOSITORIES.md),
[engine internals](docs/architecture/ENGINE.md), [SDK 1.2](docs/sdk/README.md),
[research program](docs/research/RESEARCH_PROGRAM.md) and [tools](tools/README.md).
Operating rules: [AGENTS.md](AGENTS.md).

## Build and verify the framework

```powershell
pwsh -NoProfile -File tools/build.ps1 -ScriptHookDotNetReference '<game>/ScriptHookDotNet.asi'
pwsh -NoProfile -File tools/verify.ps1 -NoGame
pwsh -NoProfile -File tools/tests/Run-Tests.ps1
```

These commands require the recorded compiler toolchain. They do not build the
showcase gameplay or start the game. SDK example mods and compiler fixtures remain
here. Combined gameplay verification and packaging start from Liberty+ tools.

Original history and all old worktrees are preserved. The last combined source is
tagged `archive/pre-liberty-plus-split`. Historical reports describe their own
revisions and are not acceptance of the split build. No game binaries are tracked.
