# T-002 — Configuration and logging skeleton

Status: **DONE** (owner confirmed gameplay remained responsive after the live test). Scope: load/validate/reload one harmless JSON sample profile, retain last valid data on malformed input, and write structured context to a bounded log. No gameplay tuning yet.

Live test on 2026-09-24: `tools/test-t002-live.ps1` passed valid reload and heartbeat (`16:58:04Z`), malformed JSON with the last valid label retained and a continuing heartbeat (`16:58:14Z`), and valid recovery with a matching heartbeat (`16:58:24Z`). The script restored the original `default` config; the game logged its reload and another heartbeat at `16:58:34Z`. GTA IV remained running after the sequence, and the owner confirmed gameplay still worked.

The sample is [`config/probe.json`](../../config/probe.json). The script polls it every ten seconds and reloads only when its bytes change. Bad input logs `config_reload_failed` and keeps the prior label. The log rotates at 1 MiB, retaining one prior file. Build with [`tools/build.ps1`](../../tools/build.ps1), then deploy only after GTA IV closes with [`tools/deploy-t002.ps1`](../../tools/deploy-t002.ps1). The installer backs up the previous probe DLL and preserves an existing config file.

Build result on 2026-09-24: Windows .NET Framework C# compiler, x86, zero errors and zero warnings. A separate offline harness passed valid edit, malformed JSON retention, and recovery with a subsequent valid edit. Deployment was blocked while GTA IV ran; after it closed, the installer placed the new DLL and sample config. The installed DLL SHA256 matches the build: `D848A5D09DA39E7D8A9A35246AADABCBFD47FA92BFC719C199C5B4B30A7AD4C4`. The prior T-001 DLL is backed up. The in-game log confirms T-002 startup, `config_loaded ... probe_label=default`, and later default heartbeats. Live edits, malformed-input retention, and recovery still need a gameplay test.

## Human test steps

1. The T-002 DLL and sample config are installed on this machine. For a fresh installation, save and close GTA IV, build as described in [`tools/README.md`](../../tools/README.md), then run `./tools/deploy-t002.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'` from the repository root.
2. Launch GTA IV through Steam and load gameplay. Keep moving normally. No gun is needed for this test.
3. The agent runs `./tools/test-t002-live.ps1 -GameDirectory 'D:\SteamLibrary\steamapps\common\Grand Theft Auto IV\GTAIV'` from the repository root while gameplay stays active. It makes a valid label edit, writes malformed JSON, then writes a restored valid label. Each step waits for the matching log entry and heartbeat. It restores the original config in all cases.
4. Confirm the game remained responsive throughout the approximately one to five minute sequence. The agent checks the script result and project log. If a check times out, record the exact failure and log excerpt before changing code.

## Exit

The human sets `DONE` after the in-game log and gameplay checks pass. If the script fails to load or crashes, capture the exact fresh log excerpt before changing code.
