# T-024 — Phase 2 gun feel, storage interaction, and performance baseline

Status: **NEEDS-PLAYTEST**. The September 29 focused Windows rerun completed the trunk close choreography and passed
34/34 scripted steps. The wheel visibly closes, but the selection and store screenshots still show the same pistol
slot with nothing stored, and the trunk lid is partly obscured. The scenario now holds Right, Left and Space for
1,200 ms, as it already does Backspace, and requires an `arsenal_store id=14` log event. That revised selection/store
check needs a focused in-game run. Evidence: `verification-results/results/20260929-090327-a61fdfe/`.

The later `14010f6` run missed the initial 80 ms E press and never opened storage (five failed expectations; game
alive, zero log errors). Its screenshots retain the trunk prompt. E now uses the same 1,200 ms hold as the remaining
wheel controls, pending a focused retry. Evidence: `verification-results/results/20260929-115421-14010f6/`.

**Latest run `6931061`:** 35/35 steps passed with zero log errors. The open screenshot shows the rifle slot,
Right navigates to sniper rifles, the store screenshot shows `In here: AK-47 120`, and the final shot has no menu.
The boot lid is partly obscured, so the full visual check remains NEEDS-REVIEW for lid closure. No acceptance claim
is made for weapon taking, save persistence or animation quality from these four shots.
Evidence: `verification-results/results/20260929-121911-6931061/T024-trunk-review/`.

## Changes

- The three registered test weapons use a separate early-burst bloom increment, a chain reset pause, and faster short-burst recovery. All values are in `config/gunplay.json` and the generated presets. Vanilla weapons retain their own spread.
- Square/X or E at a vehicle rear or marked safehouse opens a compact store/take panel. A transfers, B closes. Player controls are held while the panel is open and restored on close, error, or script reload. The rear is searched every 250 ms. DevTools cannot open over this panel.
- The gunplay loop reports 30-second frame interval percentiles and its own average/maximum tick time in `performance` log lines. The holster scan now runs every 50 ms; safehouse observation and temporary trunk pruning run every 250 and 1000 ms; the debug hit scan uses a configurable interval and smaller radius; crosshair projection is recalculated when FOV or screen height changes. Detailed shot audit logs stop after 40 bullets per weapon, with periodic summaries retained.

## Human test steps

1. Launch a free-roam save with the Phase 2 build. Give gold pistol 58, carbine 59, and shotgun 60 through DevTools. At the range, fire single shots, three-round bursts, then one long carbine spray at the same distance. Expect tight taps, a useful short burst, and visibly broader long spray. Compare bullet impacts with the crosshair gap. Repeat while moving, crouched, in cover, and from a vehicle; record the weapon and context if impacts repeatedly fall outside the displayed cone.
2. Stand behind a parked car, within `trunkDistanceMeters` of its boot. Expect `Square / X or E  Open trunk`. Press **Square** on a controller or **E** on keyboard. Expect the boot to open and a small TRUNK panel. Use **D-pad up/down**, **A** to store a pistol, **A** to take it, and **B** to close. Confirm ammo and ownership remain correct; the boot closes and movement/phone/vehicle controls return. Repeat next to a marked safehouse stash; expect SAFEHOUSE STORAGE and, while carrying pistol ID 7 or 58, the gunsmith finish choice. Move away, enter a car, begin a mission, and open DevTools; the nearby prompt must disappear or be unavailable.
3. With the panel open, press **F10** or hold **L3+R3**; DevTools should not cover the panel. Close storage with **B**, then open DevTools normally. Reload scripts while the panel is open and confirm normal controls return after reload. Save/reload and check the stash remains correct.
4. Play for at least one minute in the same location and repeat one firefight with the debug overlay off and on. Attach `performance` log lines (frame p50/p95/p99 and gunplay tick average/maximum), shot audit summaries, and any stutter timestamps. Record resolution, graphics settings, and other active mods so the next pass can compare like with like. The log's frame interval is a ScriptHook tick proxy; use PresentMon for authoritative presented-frame timings.

## Offline evidence and limits

`tools/build.ps1` compiled 104 sources with zero warnings/errors; `tools/verify.ps1` passed 270/270 against installed CE binaries. The final DLL SHA-256 and rollback path are in `docs/archive/PHASE2_PATCHNOTES.md`; the integrated installer dry run restored every file byte-for-byte, and the actual install verified all 10 manifest files. Gun feel, input conflicts, frame pacing, and camera projection still require the owner's in-game test. Shoulder swap remains T-015 BLOCKED pending a CE-validated lateral camera control. *(Superseded the same day: the control was found by static analysis and T-015 is NEEDS-PLAYTEST; see [T-015](../../../LibertyPlus/docs/tasks/T-015-shoulder-swap.md).)*
