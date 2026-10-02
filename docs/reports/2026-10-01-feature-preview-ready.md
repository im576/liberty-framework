# Limited gameplay preview ready — October 1, 2026, 10:00 p.m. Pacific

Installed for the owner's manual test. Launch GTA IV normally; F10 opens DevTools.
Contents, exact controls and short checklist: [FEATURE_PREVIEW.md](../testing/FEATURE_PREVIEW.md).

Tested source `36901ab062a8a4ad93b864db2d48457d78c186ee`, branch `codex/feature-preview-2026-10-01`,
base main `aa6d463`. Worktree `C:\Users\IM576\GTAIV-Reborn-feature-preview`. The separate preview disables
`combat, atmosphere`, preserves density OFF, the 2-long-gun/1-sidearm loadout and original tuning/budgets.
Pending lane candidates remain excluded. This is a limited functional preview, not full feature acceptance.

## Actual evidence

Full run `20261001-215249-36901ab`, 04:52:49Z to 04:57:51Z on October 2 UTC (October 1 Pacific):

- Production build PASS, zero errors; native `ray_walk_test` passes. SDK101/engine207/Autopilot4/World3 sources.
- Full game-file verifier **1020 passed / 0 failed**; staged inputs verified before installation.
- Package/install PASS, then restored backup `phase2-20261001-215357`.
- Dedicated functional scenario **87 steps / 0 failed / 0 log errors**, game alive, startup attempt1.
  Queue status NEEDS-REVIEW reflects the required visual review; parent independently opened all four images.
- Fresh module list contains gunplay, weapon-catalog, arsenal, holsters, weapon-wheel, devtools and supporting modules;
  no combat/atmosphere. Catalog/profile/loaded WeaponInfo check agrees for all six Stage 1 entries.
- Keyboard sticky wheel equips a different slot; live left/right shoulder probes settled=True/live_ok=True,
  15 records, zero wrong. Trunk stores and takes AK47 with identical instance/ammo100/ownedTrue.
- Final storage open=False/closing=False/animation=False/locked=False/control=True.
- Images: wheel readable with pistol/two long guns; empty trunk0/8; AK47 stored1/8 and removed from carried slot;
  interface closed with vanilla HUD/radar and normal trunk prompt. Camera is an uncontrolled low angle under
  the Hove Beach structure; these are functional UI evidence, not world art or camera-clipping acceptance.

No original wheel/trunk budget pass is claimed. Aggregate smoke framestats include startup/teleport/choreography;
they are not comparable closed/open acceptance windows. Full capacity/swap, controller, real save/load,
mission/cutscene, long stability and owner feel remain as documented. Gore/HUD development continues separately.

## Installation and state protection

Six persistent state files were backed up before smoke, then restored with exact hashes and original timestamps
after verifier shutdown/restoration. The smoke-created `holsters_props.json` was archived separately, preserving
the original state set. Local backup/manifest: `results-local/offline/owner-state-before-preview-smoke/`.

Final installer installed the same tested staged package at 10:00 p.m. Pacific:

- Installed metadata: preview worktree/branch, commit36901ab, dirtyFalse, UTC `2026-10-02T05:00:56.9950152Z`.
- All **44 installed package files** independently hash-match the staged manifest; all six state hashes still match.
- Engine DLL SHA256: `6B685F3C3536404A5719A09ACCEE01FE902BF3CFA7691D79838B9A8DE37FB739`.
- Manifest SHA256: `7C0650BD6F8AD332D53D6B9937F79E5F6852D08C08C3774FC4BBA2C973DDB73A`.
- Final backup: `C:\Games\Grand Theft Auto IV\GTAIV\scripts\LibertyFramework\backups\phase2-20261001-220055`.

To undo after closing the game, run from the preview worktree:

```powershell
& ./tools/rollback-phase2.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -BackupDirectory 'C:/Games/Grand Theft Auto IV/GTAIV/scripts/LibertyFramework/backups/phase2-20261001-220055'
```

Other lanes remain offline. No unattended test/install/rollback may replace this preview or interrupt the owner
until the owner ends the manual session or explicitly requests another test installation.
