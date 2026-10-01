# T-057: Recover missing playback output for unattended GTA IV tests

Status: NEEDS-PLAYTEST

## Problem and scope

On 2026-10-01 the owner reported agents unable to run their tests because of the audio/sound-card error. Realtek,
AMD and SteelSeries drivers report OK; Audiosrv and AudioEndpointBuilder are running with automatic startup.
All physical render endpoints are disconnected or absent, and every installed Sonar virtual endpoint was disabled.
The 32-bit MMDevice API had no default output, waveOut had zero devices, and DirectSoundCreate8 returned 0x88780078.
This is a missing playback endpoint, not an absent driver or cutscene synchronization issue.

Host-tooling scope only. No game binary, audio bank, gameplay setting, driver or compiled engine change.

## Change

`tools/local/AudioOutput.psm1` preserves any active playback device. Otherwise it discovers the installed, healthy
SteelSeries Sonar Gaming endpoint and enables it through Windows endpoint policy. It resolves the endpoint ID on the
current PC and reads the native registry view in both x86 and x64 hosts. Recovery is bounded and verified through PnP;
a failed enable is never reported as success. No driver is downloaded or installed and no default device is explicitly
overridden. This virtual output may be silent when no physical listening device is connected.

`Test-AudioOutput` supplies recovery to verify-local, standalone scenarios, Start-Game, Start-GameReady and Test-Boot.
The virtual output stays enabled after testing, so unplugging a controller/headset need not remove the final output.
There is no persistent watcher or background helper. If Sonar is later disabled, the next preflight recovers it again.

## Evidence (2026-10-01)

- PowerShell tooling suite: 230 passed, 0 failed (including six recovery-decision tests).
- Production x86 preflight: deliberately disabled the sole active endpoint, confirmed zero outputs, then recovered it.
  The default output reappeared, waveOut returned one device, and DirectSoundCreate8 returned S_OK. Repeated preflight PASS.
- Actual game boot and engine commands: quick audio-output-smoke PASS, three steps, zero log errors, engine alive.
  Evidence: `D:\GTAIV-Reborn-Tools\autopilot-runs\audio-output-smoke-20261001-091624`.
- A follow-up full run using modules/perf/pools hit existing script stalls (three command timeouts, one engine log error).
  Evidence: `audio-output-smoke-20261001-092008`. This is not an audio success claim for those gameplay checks.
  The committed smoke scenario instead requires a fresh engine heartbeat after startup.
- Another fresh-boot run was deferred because lane C acquired the game lock for its authorized verification run.
  Do not interrupt another lane to run this check. No gameplay acceptance is claimed by this host fix.
- Full `20261001-100907-1f6f501`: audio-output-smoke PASS, fresh engine heartbeat, no failed steps or log errors.
  The verifier restored its installation. SDK selftest/events passed in that batch; its later hot-reload driver-fixture
  failure remains recorded in the integration report and does not invalidate the separate audio assertion.

## Human test steps

1. Leave the physical headset/controller disconnected; leave the installed Sonar driver available.
2. Run the local verifier with `-AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T057-audio-output-smoke`
   after the game becomes free. Use its installed build and shared lock.
3. With no active output, expect the host to log `recovered audio output using installed SteelSeries Sonar - Gaming`.
4. GTA IV must reach the engine and emit a fresh `T-001 heartbeat`; the smoke report must say PASS.
5. With a working physical output selected, repeat preflight and confirm the helper leaves that output alone.

## Limit

Recovery requires the already-installed Sonar driver. Removing the driver or disabling its adapter still requires a
working physical device or another virtual playback driver. This does not promise audible playback through disconnected
hardware or eliminate unrelated game/script crashes.
