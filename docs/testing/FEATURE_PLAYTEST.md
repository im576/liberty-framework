# Gameplay feature test build — current milestone

Owner direction (2026-10-01): test gameplay features first, remaster later; keep art direction in parallel. Builds and
implementation resumed with "fix that and then resume". This is the test-build plan, not a claim that every feature is
already accepted. Only the owner marks DONE after playing; failed or unproven behavior stays labeled.

## Build contents and readiness

| Feature group | Existing evidence / remaining work |
|---|---|
| Weapons, gunplay, shoulder swap, reticles | Integrated lane A; owner feel/class tuning and input checks remain. |
| Physical 2+1 loadout, holsters/slings | Integrated T-044; real save/load, vehicle/cutscene/mission compatibility and owner clipping review remain. |
| Wheel, trunk/storage | Integrated B acceptance retained; validate watcher follow-up, real persistence/safehouse/gunsmith flows and HUD coexistence. |
| Gore and combat effects | C still has crash, peak-budget and cleanup evidence gaps; do not ship its unvalidated patch as stable. |
| Functional HUD/menus | D's diagnostic is unfinished; retain vanilla guard until hiding/text preservation and restoration pass. |
| Supporting material/SDK features | R must establish target/handle correctness, glass/water results and full SDK 1.2 acceptance. Radar/SDK 1.3 stays separate pending review. |

Review other existing/in-progress feature modules against their task cards before composing the build; do not silently
omit gameplay modules because they are outside the original Slice A table. Functional UI is priority; final remaster
styling is deferred. Experimental components must be explicitly described and switchable, with known failures recorded.

## Sequence

1. Preserve and finish the held lane patches; review their source and focused checks.
2. Stagger heavy builds and run small task-specific verifier batches under the shared mutex, always restoring.
3. Review full acceptance/captures before merging; test affected combined behavior and performance on the integrated build.
4. Record a precise build manifest: commit, enabled modules/config, validated features, experimental features and known limits.
5. Prepare task-derived controls and feedback steps, then install the reviewed feature build for the owner's playtest.
   The owner authorizes continued builds/testing; do not replace their manual game session with unattended tests.

## Owner test checklist to populate from the reviewed build

- Weapons: obtain each catalog weapon, hip/aim fire, recoil/spread recovery, shoulder swap, reticle behavior.
- Loadout/storage: equip the 2+1 loadout, verify visible carry changes, enter/exit a vehicle, store/take/swap and capacity
  refusal, safehouse/gunsmith flow, save and reload; verify inventory and ammo survive.
- Gore/effects: controlled head/shotgun trials, visible impacts/muzzle/smoke at day/night, NPC response, persistence and
  cleanup; record crash or hitch triggers. Do not run uncontrolled destructive trials in the owner's main save.
- HUD/UI: readable wheel/trunk/HUD, pad and keyboard/mouse navigation, ammo shot/reload updates, help/subtitles/mission
  text preserved, and config-off restoring vanilla behavior.
- Combined: ordinary free roam, a short mission/cutscene sequence and a feature-off check, with exact observed results.

These are testing categories; before delivery fill exact buttons/commands from the shipped binding configs and task
cards, and expected outcomes from accepted evidence. Do not invent controls or report a playtest that has not happened.

## Current documented controls for the integrated B features

These are the current config/task-card bindings, not proof of a delivered combined test build. Recheck them against
the final manifest/config before installation. Source: config/arsenal.json and T-045/T-046 Human test steps.

| Test | Keyboard/mouse | Controller | Expected result / feedback |
|---|---|---|---|
| Held weapon wheel, on foot | Hold Tab, select with Left/Right, release Tab | Hold Back/View, right stick or D-pad, release Back | Highlight equips; report wrong slot, movement/phone interference or missed release. |
| Sticky weapon wheel | Tap Tab, select, Enter; Backspace cancels | Tap Back/View, select, A; B cancels | Confirm equips; cancel retains weapon. Empty thrown slot changes no weapon. |
| Open vehicle storage, behind trunk | E | X | Lid/animation completes and carried/trunk groups appear; player controls resume after close. |
| Store highlighted carried weapon | Space | X | Gun leaves carried slot/sling, enters trunk with unchanged ammo. |
| Choose stored weapon; take/swap | Page Up/Down, Enter | LB/RB, A | Preview identifies outgoing weapon; transfer preserves ammo/identity/ownership. |
| Close storage | Backspace | B | Lid closes, movement returns; repeated open/close does not stick. |
| Capacity refusal | Store a fifth gun in a four-capacity Banshee trunk | Same flow | Full/refusal shown; weapon remains carried; take/swap still works. |

Owner-only next: real safehouse save/load of a noted trunk weapon/ammo count; safehouse/gunsmith storage and exit;
physical controller navigation; story/cutscene compatibility; outfit clipping; B/D HUD/radar coexistence after D is
integrated. Use the task cards for the longer exact sequences. Do not interpret autopilot round-trip checks as a real
GTA save/load playtest. Gore/effects and the diagnostic HUD remain experimental until their separate gates pass.

## Independent art direction

Use the existing approved references (ART-007 UI, ART-008 Hove Beach, ART-012 icons) to discuss palette, typography,
hierarchy, shapes, icon language and mood. Concepts/mockups can continue separately; asset generation/deployment and
environment/atmosphere polish must not delay feature fixes or invalidate comparable gameplay test measurements.
