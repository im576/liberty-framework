# T-049 native display — full bounded run, October 1

**Partial visual result, no hiding-policy adoption.** Cash and wanted stars disappear under the native pair while
radar remains; all tested releases restore those visible elements. Weapon/ammo lack a visible baseline and native
story text/domain unload remain unproven. The shipped vanilla guard is unchanged. Heavy/game slot released;
no batch B or other game batch was run.

## Exact full receipt

Assigned lane codex/T-049-hud-continuation, tested clean commit **1f6c28937eaeb978e177e0cecb045e9abf3ca5db**.
Main **2fac4cd** merged cleanly, retaining both verifier/queue/docs/source histories. Command:

```powershell
& ./tools/verify-local.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV' -AnyBranch -NoPush -Restore -NoManual -MaxGameMinutes 30 -Only LOOP-build,LOOP-verify,LOOP-package-install,T049-hud-native-display
```

Full **20261001-155808-1f6c289**, 22:58:08.548 UTC to 23:03:02.399 UTC (15:58–16:03 Pacific).
CE 1.2.0.59; executable SHA256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D.

| Check | Actual result |
|---|---|
| LOOP-build | PASS; zero compilation errors, warnings-as-errors; SDK 101, engine 218, Autopilot 4, World 3 sources. Native core cached successful test build. |
| LOOP-verify | **FAIL**, 1119 passed / 1 failed / 1 notrun. Missing generated staging/t007/WeaponInfo.xml fallback; no phase2 staged XML existed yet. Failure preserved. |
| LOOP-package-install | PASS, package built in 4 s; manifest clean 1f6c289; 45 files staged. Backup phase2-20261001-155855. |
| T049-hud-native-display | Runner PASS, **95 steps / zero failed / zero log errors**, game alive at end; visual check remains NEEDS-REVIEW/incomplete due absent weapon/ammo baseline. |

Initial batch counts remain **PASS=2 / NEEDS-REVIEW=1 / FAIL=1**, exit 1. Do not rewrite it as PASS.
Attempt 1 did not reach the engine; the verifier's built-in attempt 2 did. No separate/manual launch was performed.
Scenario 23:01:56.009 to 23:02:47.590 UTC; no GAME-UNAVAILABLE result or further startup batch.

Engine SHA256 **6EF8703B00F7DF861BB196CF3ED336A9D30DB359F04302AA239D5B0902CC72E1**.
Native core SHA256 **47445F3B3000362E45DEF3F84374046607022E2C4A9C4F4CFF50B766925F1111**.

## Generated prerequisite failure and required recheck

Full verifier runs before package Stage. Its gold-weapon accuracy test selects phase2 XML when present, then legacy
phase1/t007 paths; without a generated staged XML it failed with DirectoryNotFoundException. The catalog stats
section explicitly reported NOT-RUN for the same absent generated artifact. Package Stage subsequently generated
staging/phase2/update/common/data/WeaponInfo.xml for the clean tested manifest. No gameplay/config/source defect
was demonstrated and no check/threshold was weakened or skipped to fix it.

After the scenario and restoration, ran only:

```powershell
& ./tools/verify.ps1 -GameDirectory 'C:/Games/Grand Theft Auto IV/GTAIV'
```

**1134 passed / zero failed**, exit 0; cached verifier compiler outputs, all current file checks executed. Separate
receipt `results-local/20261001-155808-1f6c289/LOOP-verify-after-stage.log`. Original summary/log failure is untouched.
Future fresh-worktree ordering of staged-artifact checks is a coordinator tooling question; do not mask it as a
native/HUD or engine defect. No second install/game batch, no batch B.

## Nine captures personally inspected

All files below in results-local/20261001-155808-1f6c289/T049-hud-native-display (JPGs; original PNGs retained in
_runs/hud-native-display-20261001-155857). All nine were opened and inspected individually.

| Stem | Actual visible finding |
|---|---|
| hud_native_baseline | Cash $87 and three wanted stars top right; radar bottom left, visible green rim. Weapon icon/ammo **absent** despite sampled armed pistol. No native story text. |
| hud_native_hidden | Cash/wanted gone; radar retained. Weapon/ammo absent here too, therefore their disappearance is **inconclusive**, not proven. |
| hud_native_config_off | Cash $87 with transaction line, three stars and radar return after real config-off poll/release. No Liberty replacement group. |
| hud_native_config_restored | Cash $87, three stars and radar visible; restoring original config did not rearm the lease or draw duplicates. |
| hud_native_explicit_off | Cash/transaction line, three stars and radar restored on explicit native-display off. |
| hud_native_public_off | Cash/stars/radar all absent: another public HUD-off owner wins and diagnostic has cancelled. |
| hud_native_public_restored | Cash $87, three stars and radar visible after public hud on. |
| hud_native_expired | Cash $86 with transaction line, three stars and radar after automatic expiry. |
| hud_native_stopped | Cash $86 with transaction line, three stars and radar after module stop; camera moved slightly. |

Radar's wanted tint changes between views; this is not disappearance. Visible green rim appears retained but
independent health/armour arc replacement is not proven. No help/subtitle/mission/location text was present;
no native text preservation can be inferred. No domain unload, death/pause/cutscene/failure injection or integrated
B/D performance trial was performed. Missing categories remain unproven; no override to vanilla guard.

## Restoration and sampled state

Scenario config saved/readback hashes both:
**0119BAA5575BC80E7F3AC85660065E39689087AF04E7FBDF9501F15D1F5CFD64**.
Each wallet pulse logged original=86/current=87; explicit restores and module-stop original-wallet restoration logged.
Pistol sample before probe mode: weapon=7 clip=17 total=150 shown="17 / 133"; this is idle numerical evidence only,
not weapon/ammo visual hiding, firing/reload timing or every weapon's native semantics.
Restart logs default vanilla_kept plans, no hides, native_requested=False/native_applied=False. Stop log
hud_released reason=stopped. Domain unload remains separate T049-hud-native-story-text evidence.

Verifier summary and installed-build.json both confirm **restored from phase2-20261001-155855**, completed
23:03:02.219 UTC in installed-build receipt. Source/status remained clean through the tested batch and standalone
file recheck. Process recheck after completion showed no game/verifier/compiler process. Slot released; only docs
and queue review recording followed. No OS/plugin/driver changes, no density governor change, no budget changes.

## Next (requires orchestrator review/assignment)

- Do not run batch B now. Pending IDs: T049-hud-components and T049-stage1-hud, then integrated menu/HUD evidence.
- Native diagnostic needs a demonstrably visible weapon/ammo baseline (candidate scoped fixture: refresh weapon
  selection just before capture, then verify actual image; selection/native logs alone are insufficient).
- Real help/subtitle/mission/location and ReloadScripts/unload: T049-hud-native-story-text; owner look/controls T049-hud-look.
- Review current partial cash/wanted/radar/restoration evidence before assigning any follow-up slot. No shipped hiding
  capability adoption or task DONE claim.
