# T-001 — Minimal CE C# runtime spike

Status: **DONE**. Owner: agent plus human playtest. Scope: compile and load one script that logs startup, a periodic heartbeat, caught errors, and a script-domain unload signal on reload. No gameplay changes.

## Questions

Which .NET target/platform/C# language level actually loads? Where do logs go? Does ScriptHookDotNet's reload command work reliably on CE with FusionFix and the tester's controller configuration?

## Deliverables

Minimal source and project file; reproducible build/deploy commands in [tools/README.md](../../tools/README.md); dependency sourcing; exact build output; startup/reload log sample after human test; updated [ADR-0001](../architecture/decisions/ADR-0001-runtime.md). Do not vendor ScriptHookDotNet binaries.

Build result on 2026-09-24: Windows .NET Framework C# compiler produced `LibertyFramework.net.dll` for x86 with **zero errors and zero warnings**. Its .NET Framework 4.0 project file also built with zero errors and one warning (the 4.0 targeting pack is absent, so MSBuild used installed framework assemblies). The compiler script is the reproducible build path.

The official Tomasak v1.7.1.8 archive SHA256 is `5669E4423F93BEDFB0AE34579E922213775B46BBEE4DB6ADC953CB53E7AD9058`. Only three upstream runtime files and our probe DLL were deployed to the tester's game folder while GTA IV was closed. No upstream binaries are in Git.

## Playtest evidence — 2026-09-24

The tester launched through Steam, reached gameplay, ran `ReloadScripts` in the in-game console, reported that it succeeded, and confirmed gameplay continued. The fresh `ScriptHookDotNet.log` identified GTA IV 1.2.0.59, ScriptHookDotNet 1.7.1.8, and `LibertyFramework.RuntimeProbe`; it reported the script started successfully both before and after reload. The project log shows startup at `11:01:46Z`, heartbeats every ten seconds, domain unload at `11:02:55Z`, new startup at `11:02:55Z`, and subsequent heartbeats through `11:04:05Z`. No error line appeared in either log. This confirms that the x86 .NET Framework 4.0 DLL loads, logs, and reloads on this installation with FusionFix present. In the later T-002 test session, the tester confirmed pistol aim, fire, reload, cover and vehicle shooting, and save/load all passed with the runtime installed.

## Human test steps

1. Start GTA IV through Steam and load a save. The existing DualSense layout can remain active; this probe has no controller binding.
2. Wait at least 15 seconds in gameplay. Check `D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV\scripts\LibertyFramework\logs\LibertyFramework.log`. Expect one `T-001 runtime probe started` line followed by at least one `T-001 heartbeat` line. Check that normal player movement and vanilla pistol aim/fire still work.
3. On the keyboard, press the grave/tilde key (left of `1`) to open ScriptHookDotNet's console. Type `ReloadScripts` and press Enter. The console key and command come from the upstream ScriptHookDotNet readme; this game build must confirm they work.
4. Wait at least 15 seconds. Expect a second `T-001 runtime probe started` line and another heartbeat. An unload line may appear; absence of that line alone does not fail the test, because the hook's unload event behavior has not yet been verified on CE.
5. Close the console with the same grave/tilde key or Escape. Confirm normal gameplay continues. Report the last 30 lines of the project log and any new `ScriptHookDotNet.log` error lines, along with whether the console/reload worked.

If the game fails to launch, stop the test and report the exact error and log timestamps. The project DLL and three runtime files were absent before deployment, so those four files can be removed after the game closes to restore the prior state.

## Exit

Human sets `DONE` only after logs and game behavior confirm the spike. If loading fails, record the exact error under `## Blocked` and stop.
