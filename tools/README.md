# Build and deployment tooling

T-001 is a load/log/reload probe only. It has no gameplay changes. The build uses the Windows .NET Framework compiler already installed on the tester's machine. The project targets .NET Framework 4.0 and x86; in-game load and reload were verified on 2026-09-24.

1. Download [Tomasak's v1.7.1.8 release](https://github.com/Tomasak/gta4_scripthookdotnet/releases/tag/release): `scripthookdotnet_v1.7.1.8.zip`. Expected SHA256: `5669E4423F93BEDFB0AE34579E922213775B46BBEE4DB6ADC953CB53E7AD9058`.
2. From the repository root, build with `./tools/build.ps1 -ScriptHookDotNetReference <path-to-extracted-ScriptHookDotNet.asi>`. This writes `src/LibertyFramework/bin/Release/LibertyFramework.net.dll`. The runtime binary is a local compiler reference and is not committed.
3. After closing GTA IV, run `./tools/deploy-t001.ps1 -GameDirectory <path-containing-GTAIV.exe> -RuntimeArchivePath <path-to-release-zip>`. It verifies the game version and archive hash, checks for existing files, then installs only the three required runtime files and the probe DLL. It does not copy upstream examples or alter FusionFix.
4. Launch through Steam and follow [T-001 human test steps](../docs/tasks/T-001-runtime-spike.md).

On this machine the game directory is `D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV`. No absolute machine path is baked into the scripts.

## T-002 config probe

Run the same build command above to compile the T-002 source. After GTA IV closes, deploy with `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'`. When the installed DLL differs, the installer backs it up; it preserves the first `LibertyFramework.net.dll.t001.bak` backup on later deployments. It installs the new DLL and copies `config/probe.json` into `scripts/LibertyFramework/config/probe.json` only if that file does not exist. It leaves ScriptHookDotNet and FusionFix untouched. Follow the exact [T-002 human test steps](../docs/tasks/T-002-config-logging.md). Do not copy the new DLL into a running game.

For the remaining live reload test, launch GTA IV once and load gameplay, then run `./tools/test-t002-live.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'` from the repository root. The script waits for the valid edit, malformed edit, and recovery log events, and restores the original config even if a check fails. Keep gameplay active until it finishes and report whether the game remained responsive. The script changes only `probe.json`; it does not redeploy the DLL.

## T-003 DevTools menu

Build with the same command. The new DLL adds a separate read-only menu script. After GTA IV closes, deploy with the guarded `deploy-t002.ps1` command above; it backs up the installed DLL and preserves the current config. Follow [T-003's test steps](../docs/tasks/T-003-devtools-menu.md) during the next game session. Do not copy the DLL while GTA IV is running.

## T-007 custom weapon identity probe

`./tools/prepare-t007.ps1 -GameDirectory '<GTAIV folder>'` generates an ignored, local-only `staging/t007/WeaponInfo.xml` by cloning the installed vanilla pistol entry as `LF_GOLD_PISTOL`. The model and stats remain vanilla for the identity test. After closing the game, rebuild the DLL, deploy it with `deploy-t002.ps1`, and run `deploy-t007.ps1`. The installer refuses to overwrite an existing weapon override or install while the game runs. `remove-t007.ps1` removes only a hash-matching T-007 override while the game is closed. See [T-007](../docs/tasks/T-007-weapon-slots.md) for console commands and test steps.
