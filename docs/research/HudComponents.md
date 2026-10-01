# Vanilla HUD components: what can be hidden on its own (T-049, rule 4)

**Current answer: no clean T-049 replacement hiding is verified.** Resolved hud.dat globals do not by themselves prove visible hiding.
The September 30 local run shows wanted stars and radar still visible during diagnostic hides, and duplicate weapon/ammo/wanted in the normal HUD.
T-049 is **BLOCKED**. A final source guard keeps shipped replacement elements vanilla until visible hiding is established independently.
See [local capture report](../reports/2026-09-30-lane-d-hud-local.md) and [Sonnet handoff](../handoffs/Codex-Lane-D-2026-09-30.md).
Confidence tags follow [README](README.md). Deeper research remains deferred.

## Evidence

- **[VERIFIED OFFLINE + LIVE PARSING]** CE 1.2.0.59 executable SHA256 `08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D`.
  Resolver yields 31 components, no parsed layout mismatch, 23 unparsed registrations. Weapon/icon/ammo/wanted names resolve.
  File-only evidence `results-local/offline/verify-game.log`; live evidence `results-local/20260930-205120-4ac4fcc` in D checkout.
- **[VERIFIED LIVE NEGATIVE]** `hud_hidden_wanted`, `hud_hidden_radar`, and `hud_hidden_everything_in_table` retain vanilla stars/radar despite saved-value ownership.
  `hud_component alpha=0 size=0.00001x0.00001` proves the global write, not disappearance. `HudReticle.IsHidden` is a restoration ledger.
- **[INCONCLUSIVE PROBE]** Weapon/ammo/cash are absent from the baseline, so isolated hides cannot establish their disappearance.
  Normal HUD captures do show vanilla and Liberty weapon/ammo together, independently establishing replacement failure for that run.
- **[UNRESOLVED]** Player health/armour arcs have no verified separable component. Parsed HEALTH/ARMOUR names are gunplay reticle targets, deliberately excluded.
  Matching counts were zero. 23 unparsed registrations prevent treating the table as exhaustive. Retain radar and vanilla arcs; no implicit duplicate-bar approval.
- **[VERIFIED PARTIAL]** Pistol native total 150 / clip 17 agrees with vanilla reserve 133 and Liberty `17 / 133`; firing/reload supports total-includes-clip for this pistol.
  A first-shot log mixed stale polled total with fresh clip; atomic/consistent sampling remains unproven.
- **[VERIFIED PARTIAL]** Diagnostic top-right layout, both forced-device help texts and actual config disable/re-enable have images/logs.
  Final diagnostic draw sample HUD 0.306 ms, all Liberty UI 0.310 ms (72 frames). No newer B wheel/trunk integration or real-device switching tested.
- **[VERIFIED OFFLINE]** Packaged icons 7/10/14 are 256x128, matching configured 96x48 ratio. Final guard build passes; repository verifier 481/0/5.

Owner authorization chronology: initial batches stopped before install (density policy, then temporary no-testing instruction).
The owner lifted that restriction; clean commit `4ac4fcc` ran both scenarios under the normal game lock, bounded at six minutes per scenario, and verifier restoration completed.
That batch is install PASS / probe NEEDS-REVIEW / normal HUD FAIL. Final guard source has not been installed or run.

## Existing mechanism and its limit

**[VERIFIED offline + owner report 2026-09-24]** T-010, [HudAndCrosshair.md](HudAndCrosshair.md), [MEMORY.md](../game-api/MEMORY.md), ADR-0004:
register call sites use `push [alpha]; push [colour]; push imm8; push &size; push &position; push imm8 type; push "<NAME>"; call register`.
For the four reticle components the draw code copies globals each frame; writing zero alpha/tiny size hides them and saved values restore them.
Size is position + 8; alpha is size + 8. This established reticle behavior does not establish other HUD components' behavior.

`GameAddresses.ResolveHudTable` finds calls and parses only the documented layout; unparsed shapes are logged, never guessed.
`HudReticle.Hide/Restore` retains saved values and repeatedly writes those globals. The real run demonstrates that this write path is insufficient for D's replacement elements.
The cause (copy timing, other live storage or another draw path) is **unknown**; no guess has been promoted to an address, native or struct.

`HudPlan` now requires both resolution and separately verified visible hiding. No T-049 name currently qualifies.
Default weapon/ammo/health/armour/wanted remain vanilla, zero planned hides; prompt restyling remains enabled.
Explicit `drawWithoutHidingVanilla` or diagnostic `layout-test` permits duplicate drawings, without writing unverified replacement globals.
Probe commands still suspend Liberty drawing, isolate existing writes and restore them between groups; none of D's probes touches gunplay's four reticle components.

## Unblock questions

| Question | Current answer / required evidence |
|---|---|
| Do weapon/ammo/wanted globals visibly hide their counterparts? | Not established; normal HUD duplicates show current replacement fails. Need visible baseline, disappearance and restoration for each. |
| Can player health/armour arcs hide without radar? | Unresolved; default bars remain off. Future T-054 radar redraw may be needed. |
| Are other components' globals reread each frame like reticle globals? | Current write path did not remove wanted/radar. Exact cause unknown; deeper research deferred. |
| Are all registrations parsed? | No, 23 unparsed; table not exhaustive. |
| What would `DISPLAY_HUD(false)` with radar enabled remove? | Diagnostic native-display scenario/check prepared offline October 1; no runtime result. Real native text comparisons required separately; broad suppression remains unaccepted. |
| Does configuration-off restore every changed path? | Current script and image support disable/re-enable; failure/death/unload and independent byte-hash checks remain. |
| Does wheel/trunk coexist with the group and radar? | No combined current B/D runtime evidence. B source y=220 vs HUD bottom=142 is static clearance only. |

Future work needs evidence-backed isolated hiding, appropriate owner authorization after Sonnet review, fresh committed-build normal verification and all named screenshot review.
Do not solve the blocker by accepting table ownership as visible disappearance, weakening gates, broad HUD suppression or approving duplicate bars implicitly.

## October 1 Sol diagnostic (offline only)

The queued T049-hud-native-display experiment enforces documented HUD=false/radar=true while an explicit lease is
active and records baseline/hiding/config-off/restore/public-off/expiry/stop captures. It preserves public visibility
owners; unrelated raw-native visibility state has no documented getter and cannot be claimed restored. Real native
story text/domain unload is a separate T049-hud-native-story-text review. All these runtime results remain unproven;
no disappearance capability, replacement policy, changed budget or restyling is adopted.
