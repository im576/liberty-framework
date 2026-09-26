# Liberty Engine: developer guide

This guide explains how to add a mechanic. The design rationale is in [ADR-0006](decisions/ADR-0006-engine.md).

## Layers

```
LibertyCore.dll (C++20)   native invoker, ped pool walk, world snapshot, change events, parked-thread guard
      |  C ABI (native/LibertyCore/include/liberty_core.h)
Engine (C#)               EngineHost (the only ScriptHookDotNet script) -> LibertyEngine
      |                   world snapshot, EventBus, Scheduler, services
Modules (C#)              [Module("id")] classes: gunplay, combat, arsenal, holsters, atmosphere, devtools, ...
Mods (C#, SDK only)       scripts\LibertyFramework\mods\*.dll, e.g. mods/Liberty.Autopilot
```

## Frame order

`LibertyEngine.RunFrame` runs once per game frame:

1. On the first frame only: resolve memory (`EngineMemory`), load and verify the native core, and start the modules.
2. Build the world snapshot, from the core or from the SHDN fallback, and publish its events.
3. Poll input once (`Engine.Input`).
4. Run coroutines (`Engine.Scheduler`).
5. Update modules in `Order`, each at its own `Interval`.
6. Handle file-channel commands (the autopilot).

The draw pass runs separately (on the render thread): migrated `PerFrameDrawing` handlers, then each module's `OnDraw(canvas)`, then `Engine.Ui`. It must never call natives. Draw only from state cached on the tick.

## Writing a module

A gameplay module is an SDK module (`Liberty.Sdk.LibertyModule`, [docs/sdk/README.md](../sdk/README.md)); the same
code runs built into the engine or as a mod in `scripts\LibertyFramework\mods`. This example is
[docs/sdk/examples/BleedOutModule.cs](../sdk/examples/BleedOutModule.cs), which `tools/verify.ps1` compiles against the
current SDK.

```csharp
[Module("bleedout", Order = 60, Description = "Badly wounded peds cower before they die")]
public sealed class BleedOutModule : LibertyModule
{
    protected override void OnStart()
    {
        Interval = 100;                                                  // OnUpdate at most every 100 ms
        Liberty.Events.Subscribe<PedDamaged>(this, OnDamaged);           // react instead of polling
        Liberty.Commands.Register(this, "bleed", "bleed - status", args => "ok");
    }

    private void OnDamaged(PedDamaged e)
    {
        if (e.HealthAfter < 20 && e.ByPlayer) { Liberty.Scheduler.Start(this, "cower", Cower(e.Ped)); }
    }

    private IEnumerator Cower(PedRef ped)
    {
        yield return Wait.Milliseconds(800);                             // straight-line sequences
        if (!Liberty.Peds.Exists(ped)) { yield break; }
        Liberty.Tasks.Cower(ped);
        yield return Wait.Until(() => !Liberty.Peds.Exists(ped) || Liberty.Peds.IsDead(ped), 10000);
    }

    protected override void OnUpdate() { /* periodic work: read Liberty.World instead of calling the game */ }
    protected override void OnStop() { /* entities, menus, locks and patches it owns are released for it */ }
}
```

Engine-level modules (`LibertyFramework.Engine.Module`, capability `engine.internal`) may also use ScriptHookDotNet and
engine internals; only code moved over from `GTA.Script` needs that.

- **Discovery.** The engine finds every non-abstract `LibertyModule` subclass that has `[Module]`, both in this assembly and in `scripts\LibertyFramework\mods\*.dll` when `engine.json` has `loadModAssemblies` set (the shipped `engine.json` sets it; `EngineConfig.Defaults()`, used when `engine.json` is missing or rejected, does not). It starts them in dependency order (`Requires`), then `Order`.
- **Error isolation.** An exception from a module's code disables only that module: its `OnUpdate`, event handlers, coroutines (including `Wait.Until` conditions), menu callbacks, config reloads, command handlers and draw (a draw failure is logged once and turned into the failure on the next tick). A command handler's `ArgumentException` or `FormatException` is bad input: an error reply, not a failure. The engine then:
  - calls its `OnStop()`
  - stops its coroutines
  - drops its event subscriptions, commands and config watches
  - releases everything it owns through the resource ledger (entities, cameras, FX, sounds, blips, streaming, menus, input capture, control locks, HUD, clock, weather and density overrides, memory patches), newest first
  - logs `engine_module_failed`
  - publishes a `ModuleFailed` event, and stops the modules that require it
- **Ownership.** Every service call that creates or changes something takes the owning module (`this`); `null` is refused, because nothing could ever release it.
- **Capabilities** (`Capabilities` in the manifest) guard privileged services: both the owner passed in and the module whose code is running must declare them. Mods are full-trust .NET code, so this is a guardrail against mistakes, not a sandbox.
- **Switching off.** `engine.json` has a `disabledModules` list for turning a module off without rebuilding.
- **Migrated code.** Code moved over from `GTA.Script` can keep using `Tick += ...`, `PerFrameDrawing += ...`, `Player` and `BindConsoleCommand`. The `Module` base provides them.

## World snapshot (`Liberty.World`)

**What it holds:**
- `Player` (`PlayerState`): position, heading, health, armour, weapon, ammo in clip, vehicle, flags.
- `Peds` (`PedState` list): up to 128 peds within `pedRadiusMeters`.
- `Vehicles` (`VehicleState` list): up to 64 vehicles within `vehicleRadiusMeters`, with driver, speed, body and engine health.
- `Info` (`WorldInfo`): clock, weather, pause, fade. Engine code also sees `Pools` (pool occupancy, `lf pools`).
- Validity flags `HasPlayer`, `HasPeds`, `HasVehicles`, and `FromCore`.

Health is on SHDN's `Ped.Health` scale.

**Rule:** read facts from the snapshot rather than calling natives for them. The core reads them once per frame, taking about 10 µs in total. A single SHDN native call costs 30–180 µs.

The core only uses natives that match SHDN's result at startup (`engine_verify accepted=21/21`). A spot check compares the player's position every 300 frames. A frame-counter guard proves the game thread is parked while the core runs. A native that faults once is never called again that session. Any failure switches that session to the SHDN fallback, which provides the player only, with no ped list or ped events; the core is then shut down, which also removes its hooks.

## Events (`Liberty.Events`)

Game events (`Liberty.Sdk.Events`): peds appearing, damaged (exact through the damage hook, ADR-0007), dying and removed; the player shooting, reloading, changing weapon, entering and leaving vehicles, damaged and dying; vehicles appearing, damaged, destroyed and removed; `BulletFired`; weather, pause, fade, mission and cutscene changes; and the engine's `ModuleFailed`. The full list with fields is in [docs/sdk/README.md](../sdk/README.md#5-events).

Each handler registered when an event is published sees it once, in subscription order, even if another handler's module fails during the publish; a handler added during a publish starts with the next event.

**Custom events:** a module can publish its own struct with `Liberty.Events.Publish(new MyEvent { ... })`, and other modules subscribe to it. This is how mechanics compose, for example "limb severed" leading to "NPC screams" and "police alerted".

## Services

The SDK services (`Liberty.*`, [docs/sdk/README.md](../sdk/README.md#4-services-liberty)) are what modules use. Engine-level notes:

| Service | Use |
|---|---|
| `Liberty.Scheduler` | Coroutines: `yield return Wait.Milliseconds(ms)`, `Wait.NextFrame()`, `Wait.FramesCount(n)`, `Wait.Until(condition, timeoutMs)`. They stop with their module. |
| `Liberty.Animation` | `Play`, `IsPlaying`, `Stop`, and `Choreography(owner, name)` for timed sequences that cancel cleanly when their module stops. |
| `Liberty.Peds` / `Vehicles` / `Props` | Spawn (with streaming), query and delete. The module owns what it spawns; it is deleted when the module stops, and a journal cleans it up after a script reload (`EntityService`). |
| `Liberty.Ui` | Help box, notifications, subtitles, list and radial menus, textures and the per-module canvas. |
| `Liberty.Input` | One XInput poll per frame shared by every module (`Pressed`/`Down`/`Released(PadButton...)`, `KeyPressed(VirtualKey...)`) and input capture. |
| `Liberty.Config` / `State` | `config\<module>\<name>.json` with defaults, validation and live reload / `state\<module>\<name>.json`. |
| `Liberty.Commands` | `Register(module, name, usage, handler)`. Reached from the console (`lf <command>`) and from the autopilot file channel. |
| `Liberty.Query` | Snapshot queries, ground/water, and (ADR-0008) `Raycast`/`HasLineOfSight` through the core's call into the game's line test. Engine thread only (`InGameContext`); never from the draw pass. |
| `Liberty.Memory` / `Natives` | Privileged (`memory.patch`, `engine.internal`): pattern scans and ledger-owned patches (two modules may not patch overlapping bytes); raw natives. |
| `Engine.Memory` (engine code) | The session's one scan of GTAIV.exe (`GameAddresses`, native table) and the shared `LiveMemory`, with trusted permanent ranges. |
| `ScreenInfo.Size` | Game window size. **Never** use `GTA.Game.Resolution`: it stalls or deadlocks the game. |
| `DialogGuard` | Presses OK on known harmless modal boxes (FusionFix "Error building shader!") and logs any other box. |

## Content (models, textures, audio)

- **Models:** `tools/models` (`LibertyModel`) reads and writes GTA IV drawables and texture dictionaries. Packaging builds `update\LibertyFramework\LibertyModels.img` plus `lf_models.ide` from configs such as `config/models/sling.json`. Generated models use a single graphics page ([ModelFormat.md](../research/ModelFormat.md)).
- **Textures:** icon and finish textures come from `tools/ui` and `tools/finishes`.
- **Planned:** content packs (`mods/<pack>/manifest.json` with models, textures, audio and configs) built by the same tools. Nothing from the game or other mods is committed; see `third_party/README.md`.

## Testing a change

1. Build with `tools/build.ps1` (Roslyn C# 7.3, warnings are errors) and `tools/build-core.ps1` (clang; it also builds and runs the core's unit tests in `native/LibertyCore/tests`).
2. Run offline checks with `tools/verify.ps1` (`-NoGame` in the cloud; `tools/cloud/test-all.sh` runs everything offline). Every native must be in `docs/game-api/native-hashes.csv`; the core's ABI is checked against `CoreAbi.cs`; the SDK examples must compile.
3. Package and install with `tools/package-phase2.ps1` and `tools/install-phase2.ps1` (game closed).
4. Test in game with `tools/autopilot/Run-Scenario.ps1 -Scenario tools/autopilot/scenarios/<name>.txt`. It launches the game (retrying the known startup crash), runs the steps, takes screenshots and writes a `report.md`. Add a scenario for every new mechanic.
