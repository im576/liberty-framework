# T-002 — Configuration and logging skeleton

Status: **NEEDS-PLAYTEST**. The owner asked development to continue after the T-001 load/reload success, while its vanilla-pistol check remains open. Scope: load/validate/reload one harmless JSON sample profile, retain last valid data on malformed input, and write structured context to a bounded log. No gameplay tuning yet.

The sample is [`config/probe.json`](../../config/probe.json). The script polls it every ten seconds and reloads only when its bytes change. Bad input logs `config_reload_failed` and keeps the prior label. The log rotates at 1 MiB, retaining one prior file. Build with [`tools/build.ps1`](../../tools/build.ps1), then deploy only after GTA IV closes with [`tools/deploy-t002.ps1`](../../tools/deploy-t002.ps1). The installer backs up the previous probe DLL and preserves an existing config file.

Build result on 2026-09-24: Windows .NET Framework C# compiler, x86, zero errors and zero warnings. A separate offline harness passed valid edit, malformed JSON retention, and recovery with a subsequent valid edit. The T-002 DLL has **not** been installed or tested in GTA IV because the game is running. No game files were changed during this task.

## Human test steps

1. Save and close GTA IV. From the repository root, run `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'` after building as described in [`tools/README.md`](../../tools/README.md).
2. Open `D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV\scripts\LibertyFramework\config\probe.json` in a text editor. Confirm it has `schemaVersion` 1 and a `probeLabel` such as `default`.
3. Launch GTA IV through Steam and load gameplay. Wait 15 seconds, then inspect `scripts\LibertyFramework\logs\LibertyFramework.log`. Expect `config_loaded ... probe_label=default` and a heartbeat with `probe_label=default`. No gun is needed for this test.
4. In the text editor, change `probeLabel` to `changed`, save, wait 15 seconds, and check the log for `config_loaded ... probe_label=changed` and `heartbeat probe_label=changed`.
5. Replace the JSON contents with `{broken json`, save, wait 15 seconds, and check for `config_reload_failed ... retained_label=changed` followed by `heartbeat probe_label=changed`. Gameplay should continue.
6. Restore valid JSON with `probeLabel` set to `restored`, save, wait 15 seconds, and check for `config_loaded ... probe_label=restored` and a matching heartbeat. Report any missing line or game issue. The agent will review the logs.

## Exit

The human sets `DONE` after the in-game log and gameplay checks pass. If the script fails to load or crashes, capture the exact fresh log excerpt before changing code.
