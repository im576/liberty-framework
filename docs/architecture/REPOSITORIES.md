# Repository ownership and dependency contract

Current architecture boundary, 2026-10-02. Supersedes the combined repository layout.

| Capability / product | Owner |
|---|---|
| Native bridge, validated memory, skeleton operations, damage and ray events | Liberty Framework |
| SDK, host, scheduling, snapshots, lifecycle, streaming, resource ledger | Liberty Framework |
| Canvas, textures, radial/list menus, input routing, HUD visibility and restoration | Liberty Framework |
| Weapon tuning, recoil/spread policies, reticle design, catalog and starting roster | Liberty+ |
| Inventory, holsters, weapon wheel, trunk storage and vehicle gameplay | Liberty+ |
| Gore rules, bleeding/injury behavior, dismemberment policy and presentation | Liberty+ |
| Custom HUD layout, contextual displays, art and interaction presentation | Liberty+ |
| Mood/weather choices, AI artwork, finishes and approved asset requests | Liberty+ |
| Content compilers, archive readers, art queue tooling, research and reusable skills | Liberty Framework |
| SDK self-tests and content compiler fixtures | Liberty Framework |
| Assembled overhaul scenarios, acceptance queue, gameplay and packaging tests | Liberty+ |

Source locations: `C:/Users/IM576/GTAIV-Reborn` (existing framework Git repository)
and `C:/Users/IM576/LibertyPlus` ([Liberty+ on GitHub](https://github.com/im576/liberty-plus)). Neither source
tree contains a copy of the other. No submodule, automatic fetch or floating remote dependency.

## Current compatibility boundary

Framework builds independently into `LibertyFramework.net.dll`; Liberty+ builds
into `LibertyPlus.dll`, installed under `scripts/LibertyFramework/mods/` and discovered
by the existing module loader. Both bind to the same `Liberty.Sdk.dll`.

The preview predates SDK-only gameplay. Its extracted modules still derive from the
legacy `Engine.Module`, reference ScriptHookDotNet and use framework internals. One
explicit `InternalsVisibleTo("LibertyPlus")` preserves those existing operations.
This is a privileged compatibility boundary, NOT a claim that the extraction has
migrated every system to the public SDK. Framework source never references gameplay
classes, catalogs, artwork or LibertyPlus.dll. Old source namespaces are retained
to minimize behavior changes; folder and assembly ownership are authoritative.

`LibertyPlus/framework.lock.json` pins the tested framework revision. New mods use
only the public SDK. New Liberty+ features should use SDK services where available;
promote missing reusable capabilities into narrow, validated services rather than
adding more gameplay policy to framework code. Further removal of the privileged
adapter is an explicit future API migration, not a hidden requirement of this split.

## Integrated build and evidence

`tools/repository/workspace.py --framework <checkout> --mod <LibertyPlus>` exports
current Git-managed source into a fresh ignored directory beneath Liberty+ results.
It records both commits, dirty states, file hashes and ownership, rejects collisions,
and excludes ignored binaries/game archives and local settings. A local toolchain
locator is copied separately. Source edits always happen in the owning repository.

The workspace has old-path mirrors solely because historical test fixtures inspect
those paths. Ownership prevents mod mirrors from compiling into the framework.
The integration build still emits separate DLLs. There is no maintained third repo.
Each disposable workspace has a local Git snapshot with no remote, so existing
verification can enforce source identity and clean-tree/resume checks correctly.
Legacy consumer scripts live under Liberty+ `tools/integration/`; public entry points
are `tools/build.ps1`, `tools/test.ps1`, `tools/package.ps1` and `tools/prepare-workspace.ps1`.

Package manifests record both revisions and input hashes. The player's installation
is one package; source ownership does not fragment gameplay. Offline PASS does not
prove game startup, compatibility, visuals, feel or frame rate.

## History, branches and assets

Original history is retained under `archive/pre-liberty-plus-split` and all original
worktrees. Unmerged lane work remains unaccepted; Liberty+ archive branches carry
the mod-owned snapshots with provenance. Framework-side native/SDK candidates stay
on their original framework branches. Archived branch code is not automatically
part of the new main build.

Liberty+ `docs/migration/extraction.json` records original paths/hashes. Artwork
deduplication removes only identical approved copies, points approval to the original
generation, and retains the old path/hash in `art-deduplication.json`. Unique artwork,
prepared textures, prompts, tuning, research and license evidence are preserved.

## Operating rules and skills

Each repo's AGENTS.md and docs/PROJECT_STATE.md govern its current assignment/status.
Framework owns verified GTA knowledge and reusable skills. Liberty+ owns the product
brief and mod-specific acceptance. Temporary agent conversations/settings are local.
Historical cards/reports are evidence, not competing current plans. Follow current
owner instructions before historical schedules or old task statuses.
