# Liberty SDK 1.0 — writing mods

The Liberty SDK (`Liberty.Sdk.dll`) is the public API of the Liberty engine for GTA IV: The Complete Edition 1.2.0.59.
A mod is a .NET Framework 4 class library that references **only** `Liberty.Sdk.dll`. It never references
ScriptHookDotNet, the engine assembly, raw natives or memory addresses. The engine handles those, verifies them and
cleans up after the mod.

Working example: `mods/Liberty.Autopilot`. It is the SDK's first client and includes a self-test of every service.

## 1. Make a mod

```
mods/
  MyMod/
    MyModule.cs      (any number of .cs files)
```

```csharp
using Liberty.Sdk;
using Liberty.Sdk.Events;

[Module("my-mod", Version = "1.0.0", Description = "What it does")]
public sealed class MyModule : LibertyModule
{
    protected override void OnStart()
    {
        Liberty.Events.Subscribe<PedDied>(this, e => Liberty.Log.Info(this, "ped died, by player: " + e.ByPlayer));
        Liberty.Commands.Register(this, "hello", "hello - say hi", args => "hi from my-mod");
    }

    protected override void OnUpdate() { /* every frame, or every Interval ms */ }
}
```

- **Build:** `tools/build.ps1` builds every folder under `mods/` into `mods/<Name>/bin/<Name>.dll`, referencing only the SDK. Warnings are errors.
- **Install:** `tools/package-phase2.ps1` + `tools/install-phase2.ps1` put it in `scripts\LibertyFramework\mods\`. They require `loadModAssemblies: true` in `engine.json`, which is the default.
- **Test:**
  - In the game console: `lf hello`.
  - Autopilot: add a scenario under `tools/autopilot/scenarios/`.

## 2. Lifecycle and rules

| Method | When | Notes |
|---|---|---|
| `OnStart()` | first engine frame, in dependency order | subscribe, register commands, load config |
| `OnUpdate()` | every frame, or every `Interval` ms | game work goes here |
| `OnDraw(ICanvas)` | render pass | **draw only** from state gathered in `OnUpdate`; never call game services here |
| `OnStop()` | module stopped or failed | the engine releases your resources afterwards anyway |
| `OnUnload()` | script domain unloading (reload/exit) | game functions are unavailable; managed state only |

- An exception from any of your code stops **only your module**. The engine logs it, publishes `ModuleFailed`, and releases everything you owned. It also stops modules that `Requires` yours.
- **Ownership:** everything a service creates for you is released when you stop, unless you hand it back with `Release`. That covers:
  - peds, vehicles, props, cameras, FX, sounds, blips
  - streaming requests, menus, input capture, control locks
  - weather, clock and density overrides, HUD hiding, memory patches
- **Performance:**
  - Read `Liberty.World` (the per-frame snapshot) instead of asking the game again.
  - Scale optional work by `1 - Liberty.Perf.Pressure`.
  - A mod over its budget (`BudgetMs`, default `moduleBudgetMs`) is throttled, and stopped if it stays far over.

## 3. Manifest (`[Module]`)

| Field | Meaning |
|---|---|
| `Id` | unique id; names your log lines, config and state folders |
| `Version` | your version |
| `Order` | start/update order among modules without dependencies (lower first) |
| `Requires` | module ids that must be running first |
| `Capabilities` | privileges (below); undeclared privileged calls throw |
| `SdkVersion` | SDK you built against (default: the SDK you compiled with); loads when the major versions match and your minor is not newer |
| `BudgetMs` | expected average ms per update |

| Capability | Allows |
|---|---|
| `player.control` | `Player.LockControl` |
| `input.capture` | `Input.Capture` (menus capture automatically, no capability needed) |
| `memory.patch` | `Memory.*` (reads and patches; patches are restored automatically) |
| `developer` | test/world commands (convention) |
| `engine.internal` | everything, including `Natives.*` — engine modules only |

## 4. Services (`Liberty.*`)

| Service | Highlights |
|---|---|
| `World` | per-frame snapshot: `Player`, `Peds`, `Vehicles`, `Info` (time, weather, pause, fade, mission, cutscene), nearest queries |
| `Events` | typed events (`Liberty.Sdk.Events`): see §5; your own struct events too |
| `Scheduler` | coroutines: `yield return Wait.Milliseconds(500)`, `Wait.Until(cond, timeout)`, `Wait.NextFrame()` |
| `Player` | ped, money, wanted level, control lock, teleport (streams the area first), invincibility |
| `Peds` / `Vehicles` / `Props` | spawn (streams the model, non-blocking), delete/release, position, heading, health, bones, doors, extras, attach (rotations in **degrees**) |
| `Weapons` | give, remove, select, ammo, clip, inventory, weapon model and slot |
| `Tasks` | clear, stand, go to, wander, turn, look, flee, attack, aim, shoot, hands up, cower, enter/leave/drive vehicles |
| `Animation` | play clips (loop, upper body, secondary, hold last frame), query time, stop; **choreography** (below) |
| `Fx` / `Audio` | particle bursts and loops (on peds, vehicles, points); frontend and positional sounds; ambient speech |
| `Streaming` | model / animation requests shared between mods and released when the last holder stops |
| `Input` | controller (edges, sticks, triggers) and keyboard; capture routes input to one module |
| `Ui` | help box, notifications, subtitles, **list menus**, **radial menus**, textures, weapon icons, HUD visibility, `OnDraw` canvas (1280x720 virtual) |
| `WorldControl` | time, clock freeze, weather, population density (lowest request wins), ground height, clear area |
| `Blips` | radar blips for points, peds, vehicles |
| `Config` / `State` | `config\<id>\<name>.json` (created from your defaults, validated, live reload with `Watch`) / save data in `state\<id>\` |
| `Commands` | console and autopilot commands |
| `Log` | one log file, prefixed with your id |
| `Perf` | cost samples, frame time and p95, pressure, private bytes, free address space, managed heap |
| `Modules` | loaded modules and their status; typed access to another mod's public module |
| `Memory` / `Natives` | privileged low-level access (capabilities) |

### Choreography

```csharp
Liberty.Animation.Choreography(this, "trunk")
    .TurnTo(ped, trunkPosition, 450)
    .At(520, () => Liberty.Vehicles.OpenDoor(car, VehicleDoor.Trunk))   // 520 ms into the next step
    .Play(ped, new AnimClip("amb@car_stash", "open_boot"), AnimOptions.Default, 700, 2600)
    .LoopUntil(() => done, body => body.WaitUntil(() => done || busy, 3000).Do(Next))
    .OnComplete(() => Liberty.Tasks.Clear(ped))
    .OnCancel(() => Liberty.Vehicles.CloseDoor(car, VehicleDoor.Trunk))   // also runs if your module stops
    .Begin();
```

- Every step ends by its own rule or a maximum time, so a clip that never loads cannot trap the sequence.
- The sequence cancels itself if a ped taking part disappears or dies.
- The Arsenal trunk (`src/LibertyFramework/Arsenal/Ui/TrunkSequence.cs`) is a real example.

### Menus

- `Ui.OpenList(this, new ListMenu { ... })` and `Ui.OpenRadial(this, new RadialMenu { ... })` return an `IMenu`.
- Labels, icons and centre lines are functions, evaluated every frame on the engine tick and drawn from a snapshot.
- Menus capture input and lock player control while open, unless you set `LockPlayerControl = false`.
- The Arsenal weapon wheel (`Arsenal/Ui/StorageWheel.cs`) is a radial menu with a gunsmith list on top.

## 5. Events

| Event | Fields |
|---|---|
| **Peds** | |
| `PedAppeared` / `PedRemoved` | `Ped` |
| `PedDamaged` | `Ped`, `Attacker`, `Weapon`, `Bone`, `HealthBefore/After`, `ByPlayer`, `Exact` |
| `PedDied` | `Ped`, `Killer`, `Weapon`, `Bone`, `ByPlayer` |
| **Player** | |
| `PlayerShot` | `Weapon`, `ClipBefore`, `ClipAfter` |
| `ReloadStarted` / `ReloadFinished` | `Ped`, `Weapon`, clip |
| `PlayerWeaponChanged` | `Previous`, `Current` |
| `PlayerEnteredVehicle` / `PlayerExitedVehicle` | `Vehicle` |
| `PlayerDamaged` | health and armour before/after |
| `PlayerDied` | — |
| **Vehicles** | |
| `VehicleAppeared` / `VehicleRemoved` / `VehicleDestroyed` | `Vehicle` |
| `VehicleDamaged` | body and engine before/after |
| **Combat** | |
| `BulletFired` | `Shooter`, `Weapon`, `From`, `To`, `ByPlayer` |
| **World** | |
| `WeatherChanged`, `PauseChanged`, `FadeChanged`, `MissionChanged`, `CutsceneChanged` | state |
| **Engine** | |
| `ModuleFailed` | `ModuleId`, `Error` |

`PedDamaged.Exact` is false until the engine's damage hook lands (ADR-0007). Until then attacker, weapon and bone come
from the game's last-damage records.

## 6. Deployment facts

- **SDK location:** `Liberty.Sdk.dll` lives **next to GTAIV.exe**. ScriptHookDotNet loads script assemblies from bytes into a domain whose base directory is the game folder, so the SDK can only be found there.
- **Mod loading:** mods are also loaded from bytes, so a mod DLL can be replaced while the game runs.

## 7. Developer loop (hot reload)

| Command | Does |
|---|---|
| `lf reload <module>` | Loads the module's DLL from `scripts\LibertyFramework\mods` again and swaps every module in it |
| `lf restart <module>` | Stops the module and its dependents, then starts fresh instances of the same code (engine modules too) |
| `lf hotreload on/off` | Reloads a mod automatically when its DLL changes (also `hotReload` in `engine.json`; default off) |
| `lf inspector on/off` | Shows an on-screen panel: frame time, pressure, memory, pools, and one row per module (state, avg/max ms, interval, owned resources, reloads) |

How a reload works:
1. The old modules stop exactly as they would on failure: `OnStop`, then everything they own is released. Modules
   that `Require` them stop too.
2. Fresh instances of the new types take over the same slots. Modules new in the DLL get new slots.
3. Everything that was running starts again in dependency order.

The watcher waits until the file has stopped changing for one poll, so a build or copy still writing it is skipped. It
also ignores a file whose content is unchanged.

Keep in mind:
- **State:** fields start fresh after a reload. Keep what must survive in `Liberty.State`, which is keyed by module id
  and survives reloads.
- **Memory:** the old assembly stays loaded, because .NET Framework cannot unload it. Reloads count against
  `hotReloadMaxLeakMegabytes` (32 MB by default) of the 32-bit address space, then are refused until the game restarts.
- **Engine modules:** the ones built into `LibertyFramework.net.dll` can only be restarted. New engine code needs SHDN's
  `ReloadScripts` or a game restart.
- **Custom event types:** event types a mod defines are new types after a reload. Other mods subscribed to the old
  types need a restart too.
- **Versioning:** SDK 1.x keeps binary compatibility for mods built against 1.0. The freeze criteria are in `docs/sdk/ROADMAP.md`.
