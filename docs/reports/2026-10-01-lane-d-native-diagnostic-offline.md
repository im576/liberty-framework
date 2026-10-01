# T-049 native display diagnostic — offline milestone, October 1

Prepared on codex/T-049-hud-continuation in the assigned lane-D checkout, resumed from f6dff18. No production build,
full verifier, installation or game run. The temporary owner hold was respected; B retains the heavy/game slot.

## Patch

Explicit `hudctl native-display on <milliseconds>|off` requires enabled HUD, probe mode and layout-test off. It
holds documented DISPLAY_HUD(false)/DISPLAY_RADAR(true), expires within at most two minutes, and cancels on config
or unsafe gameplay, stop/unload/failure. It yields to other public HUD-off owners using the existing visibility
ledger plus a small internal query. Default vanilla guard and all config/tuning/budgets are unchanged.
Cash baseline pulses restore exact wallet; config-test independently reads/hashes restored original bytes.

Scenario T049-hud-native-display captures baseline/hidden/config-off/config-restored/explicit-off/public-off/public-
restored/expiry/stop. T049-hud-native-story-text requires real native help/subtitle/mission/location comparisons and
ReloadScripts/domain unload. SDK overlays cannot stand in for those text paths. An absent visible baseline, inactive
lease or missing native category is inconclusive/NOT-RUN. No disappearance capability is adopted from native logs.

## Focused verification

Roslyn x86 harness, warnings as errors (unused harness/Checker fields warning 0649 suppressed): eight sources,
including real HudNativeDisplayProbe, HudAmmoSample, HudConfig/HudText/HudInputDevice/HudPlan and Checker. Harness
extracts the exact existing NativeDisplayProbe/AmmoSampling/Plan test methods, registry scan, HudModule's native
wrapper and UiService's owner query/SetHudVisible. Game/world/ledger/native calls are small stubs; no game-file or
engine production compilation. Outputs remain in results-local/offline/sol-hud-focused/Focused.cs / Focused.exe /
checks.log. **40 passed, zero failed**:

- 8 lease/default/expiry/config/unsafe/cancel checks.
- 9 inherited ammo pair/invalidation/reset checks.
- 11 guard/fallback/diagnostic plan checks.
- 3 registry checks: all 176 referenced native names listed.
- 9 wrapper/owner checks: normal-on baseline, active pair, config-off, no rearm, concurrent/prior public-off,
  last-owner restoration, idempotent cancel (stop/unload model), cutscene cancellation.

Scenario's 30 expect lines parsed through AutopilotLogic.ConvertFrom-ExpectLine; all regexes compile.
Regenerated queue/plan validates: 84 checks (13 offline, 4 probe, 45 scenario, 22 manual). Diff whitespace PASS.

## Limits and exact next tests

Production compile of the complete module/command handler is pending tools/build.ps1 with the installed SHDN
reference; tools/verify.ps1 -NoGame and scheduled LOOP-build/LOOP-verify follow. Automated full batch A:
LOOP-build,LOOP-verify,LOOP-package-install,T049-hud-native-display. Batch B:
LOOP-package-install,T049-hud-components,T049-stage1-hud. Use -AnyBranch -NoPush -Restore -NoManual, full mode.
Manual T049-hud-native-story-text and T049-hud-look remain necessary. No new runtime capture was taken.

No getter for arbitrary external raw-native visibility is documented. The diagnostic requires an original visible
public-on baseline and preserves known public-off owners; it cannot claim to snapshot unknown outside native flags.
Real unload native availability, text preservation, rendering and combined menu/HUD costs remain unproven.
Existing 20260930-205120-4ac4fcc is still HUD FAIL/components NEEDS-REVIEW, normally restored. Original mixed-age
16/150 log remains evidence; inherited pair sampler repairs that source path offline without new timing claims.
