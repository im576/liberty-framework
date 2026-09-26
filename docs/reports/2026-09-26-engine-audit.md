# Engine audit (2026-09-26) — WIP

**Status: WORK IN PROGRESS, stopped early by the coordinator.** Findings below are from a read-through of
`src/LibertyFramework/Engine/**`, `sdk/Liberty.Sdk/**`, `native/LibertyCore/src/**` and `mods/Liberty.Autopilot/**` at
`0e4d357`. The code fixes listed under "Applied" are **not compiled or tested yet** (no build was run). Docs/README
refresh and the GitHub branch cleanup (Task B) were not started. CONFIRMED = traced in code; PLAUSIBLE = likely, not proven.

## Findings (by severity)

### Critical / High

1. **Scheduler: a throwing `Wait.Until` condition stops the whole engine** — CONFIRMED.
   `Scheduling/Scheduler.cs:45` ran `Wait.Step()` (which calls the mod's condition) outside the try; `LibertyEngine.RunFrame`
   (`LibertyEngine.cs:598`) called `Scheduler.Run` unguarded, so the exception escaped every frame: no module updates,
   UI, commands or entity flush. **Applied:** Step moved inside the try (fails only the owning module); RunFrame wraps
   `Scheduler.Run` and `ModuleConfig.Poll` in try/log.
2. **Scheduler: `Start(null, ...)` poisons every frame** — CONFIRMED. `Run` reads `c.Owner.Running` outside the try.
   Reachable from any SDK call that takes an owner and schedules (`Peds/Vehicles/Props.Spawn`, choreography).
   **Applied:** `ArgumentNullException` on null owner/routine.
3. **Null owner = permanent, unowned game-state changes** — CONFIRMED. `ResourceLedger.Add` ignores a null owner, but
   services mutate first: `Player.LockControl(null)` (control locked forever), `SetInvincible(null,true)`,
   `Ui.SetHudVisible(null,false)`, `WorldControl.FreezeTime(null,true)`, `ForceWeather(null,..)`. Worst:
   `Memory.Patch(null, ...)` passes `RequireCapability` (null = engine), **writes game memory without the memory.patch
   capability, is never restored**, then throws on `owner.Id`. **Applied:** null-owner guards in those services, plus
   `OpenList/OpenRadial`, `Input.Capture`, `Natives.*`.
4. **Capability model can be bypassed** — CONFIRMED (not fixed, behavioural). (a) `RequireCapability` trusts the
   caller-supplied owner: a mod can pass another module's instance (from `Liberty.Modules.Get<T>()`) as owner.
   (b) `ModuleManifest.Capabilities`/`Requires` return the live arrays, so a module can rewrite its own (or, via
   `Modules.List()`, any) manifest. Suggest: check `CurrentModule ?? owner`, and copy arrays in the manifest. Note mods are
   full-trust .NET code, so capabilities are a guardrail, not a sandbox — document that.
5. **Menu callback exceptions are not attributed to the module** — CONFIRMED (not fixed, behavioural).
   `UiService.Update` runs menu callbacks with `CurrentModule == null` inside the engine's catch
   (`engine_ui_update_failed`). If `ListMenu.Items()` throws persistently, `Back` is never processed
   (`ListMenuView.cs:54` before `:70`) → menu can't close and the player-control lock stays. Suggest: catch per menu,
   fail the owner for persistent errors (engine modules like the Arsenal wheel currently survive one-off errors, so
   decide the policy first). Also contradicts docs/sdk/README.md §2 ("stops only your module").
6. **`Vehicles.GetDriver` calls `GET_DRIVER_OF_CAR` through SHDN** (`VehicleService.cs:108`) — PLAUSIBLE crash. NATIVES.md
   records that this native faulted on pooled vehicles and corrupted game state; the core stopped calling it. Suggest:
   return the snapshot's `Driver` when the vehicle is in `World`, and avoid the native otherwise. `CoreVerifier` also still
   calls it (direct + SHDN) during vehicle verification (`CoreVerifier.cs:95`).
7. **Uncommitted WIP in the main checkout bumps `LC_ABI_VERSION` to 4** (raycast) while `CoreAbi.Version` is still 3 —
   CONFIRMED in the working tree (not in `main`). A core built from that tree is refused (`engine_core_abi_mismatch`):
   no snapshot, crash capture or exact damage. Bump `CoreAbi.cs` in the same change.

### Medium

8. **Kill de-duplication window broken** — CONFIRMED. `WorldBuilder.PublishDamages` checks `Recent(exactKillFrame, v, 600)`
   but `Forget` prunes entries older than 300 frames (`WorldBuilder.cs:174,240`): a corpse hit 5–10 s after death can
   publish a second exact `PedDied`. Suggest pruning at the largest window (600).
9. **Draw-thread races / log flood** — PLAUSIBLE. `LibertyEngine.Draw/DrawModules` enumerate `runtimes` on the render
   thread while hot reload can `Add` on the tick; a throwing `OnDraw` logs a full stack **every frame** (sync file I/O on
   an HDD, 1 MB log rotation evicts startup evidence). Suggest a per-module draw-failure flag the tick turns into `Fail`.
10. **Core natives keep running after a contained fault in the same frame** — CONFIRMED. `Natives::call` never checks
    `verified_`; `read_ped`/`read_vehicle` are gated once per frame, so a faulting native is called for the rest of the
    ped/vehicle list. Suggest early-out in `call` when a fault switched the native off.
11. **Hooks can outlive the core** — CONFIRMED. `CoreBridge.Frame` sets `Available=false` on an escaped AV or snapshot-size
    mismatch without `lc_shutdown`, and `Shutdown()` returns early when `!Available`, so the damage detour stays installed
    (contradicts the `lc_shutdown` comment). Observe-only, so low impact.
12. **Damage ring drops are never reported** — CONFIRMED. `read_damages` discards the `dropped` count from `drain` (`core.cpp:443`).
13. **TASK_PLAY_ANIM / _UPPER_BODY / _SECONDARY missing from `native-hashes.csv`** — CONFIRMED. The name is chosen by a
    ternary (`AnimationService.cs:43`), so `tools/verify` never checks it (hashes are in FusionFix natives.ixx:
    0x28EE78D8 / 0x02534709 / 0x273C2D35). Not yet applied.
14. **Autopilot: a malformed console command stops the autopilot module** — CONFIRMED. `Args.Int/Float` index/parse
    exceptions reach `CommandRegistry.Execute`, which calls `Fail(owner)`.
15. **Command name collisions are silent** — CONFIRMED. Last `Register` wins; `RemoveOwner` of the winner removes the name
    for everyone. **Applied:** logged as `command_replaced` (behaviour unchanged). Engine-command exceptions now also log
    `command_failed` with the stack.

### Low / docs

16. `CoreVerifier.cs:83` comment says all eight vehicle natives gate the list; the core needs seven (driver read from
    memory), so `engine_core_vehicles_off` can be logged while vehicles are listed.
17. `SdkSelfTest` subscribes `VehicleAppeared/Removed` on every run and never unsubscribes.
18. `EngineConfig.Defaults()` has `loadModAssemblies=false`; docs/sdk/README.md calls `true` "the default" (true only in
    the shipped `config/engine.json`; a rejected engine.json silently disables mods).
19. Stale docs: docs/sdk/README.md §5 says `PedDamaged.Exact` is false "until the damage hook lands" and omits the
    exact-damage fields; docs/architecture/ENGINE.md sample code uses APIs that no longer exist (`Started/Update/Stopped`,
    `Engine.Animations.PlayAndWait`, `Engine.Entities.CreateObject`, `e.Handle`) and lists the autopilot as an engine module;
    PROJECT_STATE "SDK 1.0 build-out ... Not done yet" contradicts the M2–M5 section; NATIVES.md:110 cites
    `Testing/AutopilotModule` (now `mods/Liberty.Autopilot`).
20. Rule 3 gray area (engine heuristics, not gameplay tuning): hit-match radius 2.5 m, closest-vehicle 60 m
    (`WorldBuilder`), GoTo 45 s / enter-car 10 s timeouts (`TaskService`), radial stick dead zone 0.55, governor streaks.
21. Rule 8: `ModuleReloader.cs:79-88` catch blocks return without logging (intentional while a build holds the file).

## Applied in this branch (uncompiled)

`Scheduler.cs` (Step inside try, null guards), `LibertyEngine.cs` (guarded scheduler/config poll), null-owner guards in
`PlayerService`, `UiService`, `WorldControlService`, `MemoryService`, `InputService`, `NativeService`; logging in
`CommandRegistry`. Next session must build (`tools/build.ps1`), run `tools/verify.ps1`, and re-run the autopilot
`sdk-selftest`, `ui-review`, `trunk-review`, `hot-reload` scenarios.

## Not done

Remaining audit items (13, 16, 19 doc fixes, 21), build/verify, README refresh, .gitignore/binary check, and all of
Task B (no tags created, no branches deleted). Branch facts gathered: `codex/phase2-foundations` is an ancestor of
`main` (safe to archive+delete); `codex/phase2-combat` and `codex/phase2-systems` are not ancestors but each of their two
commits has a rebased equivalent in `main` (14dd7c8, fa04dd8, 905e57c, 1655a30). No remote tags exist.
