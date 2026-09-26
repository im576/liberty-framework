# Liberty Engine: developer guide

This guide explains how to add a mechanic. The design rationale is in [ADR-0006](decisions/ADR-0006-engine.md).

## Layers

```
LibertyCore.dll (C++20)   native invoker, ped pool walk, world snapshot, change events, parked-thread guard
      |  C ABI (native/LibertyCore/include/liberty_core.h)
Engine (C#)               EngineHost (the only ScriptHookDotNet script) -> LibertyEngine
      |                   world snapshot, EventBus, Scheduler, services
Modules (C#)              [Module("id")] classes: gunplay, combat, arsenal, holsters, atmosphere, devtools, autopilot, ...
```

## Frame order

`LibertyEngine.RunFrame` runs once per game frame:

1. On the first frame only: resolve memory (`EngineMemory`), load and verify the native core, and start the modules.
2. Build the world snapshot, from the core or from the SHDN fallback, and publish its events.
3. Poll input once (`Engine.Input`).
4. Run coroutines (`Engine.Scheduler`).
5. Update modules in `Order`, each at its own `Interval`.
6. Handle file-channel commands (the autopilot).

The draw pass runs separately: `Render` on each module, then `Engine.Ui`. It must never call natives. Draw only from state cached on the tick.

## Writing a module

```csharp
using LibertyFramework.Engine;
using LibertyFramework.Engine.Events;
using LibertyFramework.Engine.Scheduling;

[Module("bleedout", Order = 60)]
internal sealed class BleedOutModule : Module
{
    protected internal override void Started()
    {
        Interval = 100;                                             // Update at most every 100 ms
        Engine.Events.Subscribe<PedDamaged>(this, OnDamaged);       // reacts instead of polling
        Engine.Commands.Register(this, "bleed", "bleed status", args => "ok");
    }

    private void OnDamaged(PedDamaged e)
    {
        if (e.HealthAfter < 20 && e.ByPlayer) { Engine.Scheduler.Start(this, "crawl", Crawl(e.Handle)); }
    }

    private System.Collections.IEnumerator Crawl(int handle)
    {
        yield return Wait.Milliseconds(800);                        // straight-line sequences
        // ... play animations with Engine.Animations.PlayAndWait(...)
    }

    protected internal override void Update() { /* periodic work; read Engine.World instead of calling natives */ }
    protected internal override void Stopped() { /* restore anything you changed in the game */ }
}
```

- **Discovery.** The engine finds every non-abstract `Module` subclass that has `[Module]`, both in this assembly and in `scripts\LibertyFramework\mods\*.dll` when `engine.json` has `loadModAssemblies` set. It constructs them in `Order`.
- **Error isolation.** An exception from any handler, update, draw or coroutine disables only that module. The engine then:
  - calls its `Stopped()`
  - stops its coroutines
  - drops its event subscriptions and commands
  - deletes the entities it owns
  - logs `engine_module_failed`
  - publishes a `ModuleFailed` event
- **Switching off.** `engine.json` has a `disabledModules` list for turning a module off without rebuilding.
- **Migrated code.** Code moved over from `GTA.Script` can keep using `Tick += ...`, `PerFrameDrawing += ...`, `Player` and `BindConsoleCommand`. The `Module` base provides them.

## World snapshot (`Engine.World`)

**What it holds:**
- `Player` (`PlayerState`): position, heading, health, armour, weapon, ammo in clip, vehicle, flags.
- `Peds` (`PedState` list): up to 128 peds within `pedRadiusMeters`.
- `Info` (`WorldInfo`): clock, weather, pause, fade.
- Validity flags `HasPlayer`, `HasPeds`, `HasWorld`, and `FromCore`.

Health is on SHDN's `Ped.Health` scale.

**Rule:** read facts from the snapshot rather than calling natives for them. The core reads them once per frame, taking about 10 µs in total. A single SHDN native call costs 30–180 µs.

The core only uses natives that match SHDN's result at startup (`engine_verify accepted=21/21`). A spot check compares the player's position every 300 frames. A frame-counter guard proves the game thread is parked while the core runs. Any failure switches that session to the SHDN fallback, which provides the player only, with no ped list or ped events.

## Events (`Engine.Events`)

**Game events** (from `Engine.Events` namespace structs):
- `PedAppeared` and `PedRemoved`.
- `PedDamaged`: health before and after, the game's last damage bone, and `ByPlayer`.
- `PedDied`.
- `PlayerShot` and `PlayerReloaded`: clip before and after.
- `PlayerWeaponChanged`, `PlayerEnteredVehicle`, `PlayerExitedVehicle`, `PlayerDied`, `PlayerDamaged`.
- `WeatherChanged`, `PauseChanged`, `FadeChanged`.

**Engine event:** `ModuleFailed`.

**Custom events:** a module can publish its own struct with `Engine.Events.Publish(new MyEvent { ... })`, and other modules subscribe to it. This is how mechanics compose, for example "limb severed" leading to "NPC screams" and "police alerted".

## Services

| Service | Use |
|---|---|
| `Engine.Scheduler` | Coroutines: `yield return Wait.Milliseconds(ms)`, `Wait.NextFrame()`, `Wait.FramesCount(n)`, `Wait.Until(condition, timeoutMs)`. They stop with their module. |
| `Engine.Animations` | `Play`, `IsPlaying`, `PlayAndWait` (coroutine with timeout); clip sets are cached. |
| `Engine.Entities` | `CreateObject`, `CreatePed` and `Track`. The module owns them, they are deleted when it stops, and a journal cleans them up after a script reload. |
| `Engine.Ui` | `ShowHelp(owner, text, ms)` (GTA IV help box) and `Notify(text, ms)`. |
| `Engine.Input` | One XInput poll per frame: `Pressed`/`Down`/`Released(ControllerInput.AButton ...)`, `KeyDown(Keys.X)`. |
| `Engine.State` | `Load<T>(module, name)` / `Save` to `state\<module>\<name>.json`. |
| `Engine.Commands` | `Register(module, name, usage, handler)`. Reached from the console (`lf <command>`) and from the autopilot file channel. |
| `Engine.Memory` | The session's one scan of GTAIV.exe (`GameAddresses`, native table) and the shared `LiveMemory`, with trusted permanent ranges. |
| `Engine.Query` | SDK `IWorldQuery`: snapshot queries, ground/water, and (ADR-0008) `Raycast`/`HasLineOfSight` through the core's call into the game's line test. Engine thread only (`InGameContext`); never from the draw pass. |
| `ScreenInfo.Size` | Game window size. **Never** use `GTA.Game.Resolution`: it stalls or deadlocks the game. |
| `DialogGuard` | Presses OK on known harmless modal boxes (FusionFix "Error building shader!") and logs any other box. |

## Content (models, textures, audio)

- **Models:** `tools/models` (`LibertyModel`) reads and writes GTA IV drawables and texture dictionaries. Packaging builds `update\LibertyFramework\LibertyModels.img` plus `lf_models.ide` from configs such as `config/models/sling.json`. Generated models use a single graphics page ([ModelFormat.md](../research/ModelFormat.md)).
- **Textures:** icon and finish textures come from `tools/ui` and `tools/finishes`.
- **Planned:** content packs (`mods/<pack>/manifest.json` with models, textures, audio and configs) built by the same tools. Nothing from the game or other mods is committed; see `third_party/README.md`.

## Testing a change

1. Build with `tools/build.ps1` (Roslyn C# 7.3, warnings are errors) and `tools/build-core.ps1` (clang; it also builds and runs the core's unit tests in `native/LibertyCore/tests`).
2. Run offline checks with `tools/verify.ps1`. Every native must be in `docs/game-api/native-hashes.csv`.
3. Package and install with `tools/package-phase2.ps1` and `tools/install-phase2.ps1` (game closed).
4. Test in game with `tools/autopilot/Run-Scenario.ps1 -Scenario tools/autopilot/scenarios/<name>.txt`. It launches the game (retrying the known startup crash), runs the steps, takes screenshots and writes a `report.md`. Add a scenario for every new mechanic.
