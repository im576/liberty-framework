# T-049 Lane D local verification — September 30, 2026

**Result: clean replacement BLOCKED.** The bounded owner-authorized batch ran successfully enough to expose visible duplicates; it did not pass T-049 acceptance.
The normal verifier restored the prior installation. A later source guard retains vanilla replacement elements until visible hiding is verified; that guard is offline-tested only.

Checkout `C:/Users/IM576/GTAIV-Reborn-lane-d`, clean tested branch `codex/T-049-hud-continuation`, commit `4ac4fcc`.
Evidence root `results-local/20260930-205120-4ac4fcc`. Machine game lock used normally; first launch failed and normal retry succeeded.
Run 03:51:20–03:57:16 UTC (8:51–8:57 p.m. Pacific). Package/install PASS; components NEEDS-REVIEW; HUD FAIL.
Backup `C:/Games/Grand Theft Auto IV/GTAIV/scripts/LibertyFramework/backups/phase2-20260930-205133`; summary records normal restoration.
CE 1.2.0.59, FusionFix 5.0.1, exe SHA256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`.

## Component capture review

All captures below are in `_runs/hud-components-20260930-205136` under the evidence root, retained as JPGs by the verifier.
The scene became wanted and police changed the camera/player conditions; several later views differ, so exact image subtraction is inappropriate.

| Capture stem | What the image establishes |
|---|---|
| `hud_vanilla_baseline` | Radar/green health arc and six white vanilla stars visible. Weapon/ammo and cash absent; cannot certify their later removal. |
| `hud_hidden_weapon_only` | No visible baseline weapon icon to compare; inconclusive. |
| `hud_hidden_ammo_only` | No visible baseline ammo to compare; inconclusive. |
| `hud_hidden_weapon_ammo` | Same baseline limitation; not a visible hide pass. |
| `hud_hidden_wanted` | White vanilla stars remain; wanted hiding failed visually. |
| `hud_hidden_cash` | Cash absent in both views; inconclusive. |
| `hud_hidden_radar` | Radar and its arc remain visible; radar hiding failed visually. |
| `hud_hidden_health` | Radar arc remains; matching count 0 because reticle targets are excluded. No separate player-health component established. |
| `hud_hidden_armour` | Radar remains; no matching owned component and no useful baseline armour arc. No separate player-armour component established. |
| `hud_hidden_everything_in_table` | Radar and white vanilla stars remain despite 27 diagnostic hides; no blanket removal established. |
| `hud_restored` | Radar/stars visible; since their prior disappearance failed, this cannot independently prove visible restoration. |

Runtime table resolves 31 components and lists 23 unparsed registrations. Resolved weapon/ammo/wanted globals have the expected layout.
Logs show alpha zero and near-zero sizes, and `hud_probe_hide ... hidden=True`, while screenshots still show wanted/radar.
`HudReticle.IsHidden` checks the saved-value dictionary; it must not be used as visual acceptance. No renderer/root-cause assertion or new memory mechanism was invented.

## HUD capture review

All captures below are in `_runs/stage1-hud-20260930-205513` under the evidence root.
The scenario had 110 steps, one failure, zero log errors, game alive at end. The failed step required a fresh `wanted shown ... stars=3` transition;
wanted was already visible. The three-star image itself is correct. The next source scenario queries sampled state rather than relying on a new transition.

| Capture stem | What the image establishes |
|---|---|
| `hud_rest` | Radar bottom left. White vanilla and gold Liberty wanted rows both visible: duplicate even before diagnostic layout-test. |
| `hud_weapon_change` | Vanilla icon/ammo at upper right plus Liberty silhouette and `17/133` beneath: duplicate weapon/ammo. Vanilla displays reserve 133 / clip 17. |
| `hud_weapon_firing` | Both weapon displays visible; vanilla reserve 133 / clip 13 agrees with Liberty `13/133` in this frame. Reticle visible. |
| `hud_health_hit` | Green Liberty health bar top right, vanilla radar arc retained. This is diagnostic layout-test, not accepted replacement. |
| `hud_health_low` | Short red health bar top right. One image cannot establish pulse timing. |
| `hud_armour` | Green/blue diagnostic bars visible top right; radar still bottom left, vanilla white stars and Liberty gold stars both visible. |
| `hud_wanted` | Three vanilla white stars plus three filled / three empty Liberty stars; duplicate. |
| `hud_prompt_pad` | Top-left dark help box reads `Press X to open the weapon stash.` Forced device, not automatic physical switching. |
| `hud_prompt_keyboard` | Top-left help box uses `E` and `Backspace`. Forced device, not physical switching. |
| `hud_everything` | Compact Liberty group top right, help top left, radar bottom left; known vanilla weapon/star duplicates. No B wheel/trunk panel was open. |
| `hud_config_off_vanilla` | Vanilla stars/radar visible, Liberty group absent after actual JSON disable/normal poll. |
| `hud_config_restored` | Tested pre-guard plan re-enabled; duplicate gold/white stars return. |

Pistol idle snapshot: `weapon=7 clip=17 total=150 includes_clip=True shown="17 / 133"` agrees with the vanilla image.
After 17 shots/reload the forced group shows `17/116`, consistent with reserve transfer. This supports total-includes-clip for this pistol, not every weapon.
The first-shot event briefly logged `16 / 134` from old total 150 and fresh clip 16; the 100-ms poll can mix ages. Sample consistency remains a follow-up.

Two final gates matched the same explicit 72-frame diagnostic window: **draw.hud=0.306/0.7/72**, **draw.ui=0.310/0.7/72** (average/max/count).
These averages meet HUD <0.35 ms and combined UI <=0.5 ms, with narrow HUD margin and no integrated B wheel/trunk.
Other module/tick costs are separate and were not declared passing. No density lowering or hardware/OS/renderer tuning was used.

Config-test disable/restore commands completed, normal poll logged disabled/enabled, and disabled status reported hidden=0, help_by_hud=False, empty drawn list.
The hook restored original bytes by implementation and reported success; no separate before/after file hash was saved. Scenario-level release on death/error is still unrun.

## Final guard and further acceptance

`HudPlan` now requires both resolved globals and an independently verified visible-hiding capability. D has no such capability, so defaults retain vanilla weapon/ammo,
health/armour and wanted with zero planned hides. Diagnostic layout-test/explicit duplicate overrides still draw; prompts remain enabled.
Final guard build: zero errors, warnings as errors; SDK 101 / engine 210 / Autopilot 4 / World 3 sources.
Engine SHA256 `1842B141E5DC470D889AF56C120588CBE1B11BA6A48320DD49C813DCF5AF00B1`, `results-local/offline/build-hiding-guard.log`.
Final repository verifier **481/0/5**, `results-local/offline/verify-nogame-hiding-guard.log`; three new capability cases, packaged WeaponInfo checks now run.

Final source guard/scenario changes have not run in game. Do not relabel the old failing batch as PASS. Checks remain queued for the final source.
Clean visible hiding/restoration, stable sampling, real-device switching, suppression/release and integrated B/D captures plus owner visual sign-off remain necessary.
Matching Sonnet must review all Codex changes including uncommitted files. Deeper research is deferred; no push/merge or additional D game run was made.
