# Liberty Framework / Liberty+ repository split

T-061, 2026-10-02. Source extraction and offline validation; no game launch/install.

## Result

Two maintained local source repositories:

- `C:/Users/IM576/GTAIV-Reborn`: framework/native/SDK/UI services, tools, research and shared skills.
- `C:/Users/IM576/LibertyPlus`: preview gameplay, weapon profiles/catalog, inventory/holsters,
  wheel/trunks, baseline gore/atmosphere, custom HUD candidate archives, art, tuning and acceptance.

Initial extraction moved 317 tracked files from `56ee76055478a39b91d1384cd455a1320130e0eb`.
Additional ownership cleanup moved the gunplay timing model and consumer-aware harnesses;
generic weapon stats/location readers and color-table/receipt tools remain in Framework.
The current map and original hashes are in Liberty+ `docs/migration/`.

Framework builds independently. Liberty+ is a separately discovered DLL. There is
no framework-to-gameplay assembly dependency. Generic canvas/menu/HUD visibility
services remain reusable; HUD layout and art belong to the mod.

## Compatibility and changes

The extracted preview still uses a privileged legacy adapter with one explicit
friend assembly. It is not yet entirely SDK-only. Existing source namespaces and
runtime config/state paths are retained. Necessary changes were assembly separation,
lifecycle override visibility, mod-specific config paths, generic weapon-data type
ownership, source selection, packaging and evidence routing. No new gameplay feature
or tuning change was developed. The framework's standalone default drops the preview
module-disable policy; the combined mod profile preserves it exactly.

Liberty+ pins the framework source contract/revision. Integration workspaces export
managed current files with both commits, dirty states, raw hashes and ownership.
Disposable Git snapshots (no remote) retain existing clean-tree/resume protections.
Legacy test-path mirrors are generated only and excluded from framework compilation.
Package and installed-build receipts preserve both source identities.

## Preservation and cleanup

All original worktrees/history remain; `archive/pre-liberty-plus-split` pins the
combined baseline. Four Liberty+ archive branches preserve C, C effects, D HUD and
B validation mod snapshots, with exact source commits. These are unaccepted raw
candidate snapshots, potentially requiring companion framework APIs and adaptation.
They are not silently merged or enabled in main.

Twelve hash-identical approved art copies were removed from the working tree,
saving 24,943,348 bytes. Approvals now reference identical original generations;
unique images, prepared textures, prompts, hashes and historical paths remain.
Git already deduplicates identical blobs; this saving is working-tree space.

READMEs/status/operating rules now distinguish the two products. Historical combined
schema, dashboard, schedule, SDK roadmap and original research summary are archived.
The obsolete no-gameplay statement is explicitly corrected; actual SDK version is 1.2.
Task indexes and active links route to owning repositories. Shared skills record the
new source/evidence boundary. No new external source/assets were imported.

## Validation

| Check | Observed result |
|---|---|
| Standalone framework and extracted mod compilation | PASS, separate assemblies |
| Assembly dependencies and all nine preview module manifests | 17 PASS, metadata only |
| Framework-only NoGame verifier | 135 PASS, 0 FAIL, 4 NOT-RUN |
| Fresh combined NoGame verifier | 432 PASS, 0 FAIL, 7 NOT-RUN |
| Original combined PowerShell suite | 312 PASS, 0 FAIL |
| New two-source installed receipt suite | 5 PASS, 0 FAIL |
| Framework-only original tooling suite | 140 PASS, 0 FAIL |
| Workspace ownership/collision/ignore/isolation and contract fingerprints | 7 tests PASS |
| Evidence audit unit tests | 3 tests PASS |
| Mod art request validation | 12 requests, 0 problems |
| Initial combined package | 45 files staged, not installed |
| Final staged combined NoGame verifier | 441 PASS, 0 FAIL, 5 NOT-RUN |
| Full combined tooling, PowerShell 7 / 5.1 | Each 317 PASS, 0 FAIL; isolated/sequential final runs |
| Preservation/source ownership audit | 82 PASS, 0 FAIL; preview tuning/profile and artwork intact |
| Package identity/file audit | 45 files, 0 hash mismatches; snapshot identity matches, separate LibertyPlus.dll present |
| Active documentation links / PowerShell syntax | 0 missing links / 0 syntax errors |

Fresh combined NOT-RUN includes two sections requiring generated WeaponInfo staging;
after packaging those run successfully. The complete legacy combined check list is
preserved. Final package/check evidence is retained under Liberty+ `results-local/`.
The validated package workspace is `integration/7cbc8c71a133`, snapshot `f8be55a`.

Validation found and corrected generated-plan cross-repository link routing and
missing local Git author identity in disposable snapshots. Concurrent PowerShell
suite runs sharing one workspace also collided with the worktree-preservation
assertion; isolated/sequential reruns passed without changing that assertion.
Public mod test invocations create separate workspaces automatically.

## Runtime limits

Installed preview receipt remains commit `36901ab`, same UTC/source worktree. Restored
timecycle hashes remain `593B463E4B116D9317E4CAA13D328C60F8C2BD7171EAC3DCB2D533CCE1E97814`
and `8F4979D541E9CEBC314269DEA214051F239BCE982584E93ACF5FEB0DD9E697C7`.
No fresh split-build startup, gameplay, controller/save/story, visuals or 60 FPS proof.
Startup/gore/trunk/HUD/content/cloud blockers remain. Unfinished feature development
remains paused. No GitHub repository was created and nothing was pushed.
