# Engine audit and hardening (2026-09-26)

Session 2 of [NEXT_SESSIONS.md](../workflow/NEXT_SESSIONS.md), Claude, branch `claude/nice-edison-xyxc6j` (stacked on
session 1, `claude/friendly-euler-m7hn4e`). Read as if other developers will build serious mods on this engine.

Evidence labels: **RAN-PASS** / **RAN-FAIL** (ran in the cloud container), **NOT RUN** (with the reason),
**NEEDS LOCAL VERIFY** (needs Windows, the game's files or the running game; queued in `tests/local/checks.json`).
Nothing that needs the game is claimed here.

## Result

| Step (`tools/cloud/test-all.sh`) | Before | After |
|---|---|---|
| C# build (SDK, engine, mods; warnings are errors) | RAN-PASS | RAN-PASS |
| Native core + unit tests | RAN-PASS, 13 assertions (1 test) | RAN-PASS, 23 assertions (2 tests) |
| Content compiler self-test | RAN-PASS 178 | RAN-PASS 179 |
| Offline verifier (`-NoGame`) | RAN-PASS 144, NOT RUN 7 | RAN-PASS 229, NOT RUN 7 (game files or Windows) |
| Blender manifest + add-on tests | RAN-PASS 19, NOT RUN 9 | RAN-PASS 19, NOT RUN 9 (builds need the game) |
| PowerShell unit tests | RAN-PASS 88 | RAN-PASS 107 |
| Check queue + plan | RAN-PASS 44 checks | RAN-PASS 45 checks |

The new engine-plumbing tests were also run against the previous code: 9 of them fail there (RAN-FAIL, as intended),
and a mutated `liberty_core.h` (version, enum order, a field type) fails 3 ABI checks.

## Findings

Severity: **High** = can stop the engine, corrupt game state or leave the game changed for the session; **Medium** =
wrong or lost results, silent misbehaviour; **Low** = diagnostics, docs, tooling. Status: **FIXED** (with the test
that proves it), **VISIBLE** (made observable, not changed), **OPEN** (owner decision or later session).

### Engine (C#)

| # | Sev | Finding | Status | Proof |
|---|---|---|---|---|
| E1 | High | `Scheduler.Run` evaluated `Wait.Until` conditions (module code) outside the module's try: one throwing condition escaped `Run` every frame and stopped every module update, UI, command and entity flush for the session. | FIXED: condition runs inside the try as its module; `RunFrame` also guards the scheduler and config poll | verifier `scheduler:` checks (RAN-PASS; RAN-FAIL on old code) |
| E2 | High | `Scheduler.Start(null, ...)` did the same (`Run` reads `Owner.Running`); reachable from any owner-taking SDK call that schedules. | FIXED: refused at `Start` | `scheduler: a coroutine without an owner is refused` |
| E3 | High | Null owner: the ledger ignored it, but services changed the game first. `LockControl(null)` locked controls forever; `SetHudVisible`, `FreezeTime`, `ForceWeather`, `SetInvincible`, `Input.Capture`, menus likewise; `Memory.Patch(null)` passed the capability check (null meant "engine"), wrote game memory, was never restored, then threw. | FIXED: `RequireOwner` at the top of every owning call; `RequireCapability` refuses null; the ledger refuses null as a backstop | `ledger: an entry without an owner is refused`; `commands: SDK registration needs an owner` |
| E4 | Medium | `EventBus.Publish`: a handler whose module failed removed that module's handlers from the list being iterated, so the next module's handler was skipped (event loss). Unsubscribe during a publish did the same; a handler added during a publish got the current event. | FIXED: deferred removal; each publish sees the handlers present when it started | `events:` checks (4) |
| E5 | High | Menu callbacks (`Items`, labels, select, close) ran as engine code; a persistently throwing `Items()` kept `Back` from being processed, so the menu stayed open with player control locked. Capability checks also saw "engine" there. | FIXED: callbacks run as the owning module (`LibertyEngine.RunAs`); a throw stops it, and the ledger closes its menus. Label errors log once per menu, not every frame | NEEDS LOCAL VERIFY: `SDK-ui-review`, `T024-trunk-review` |
| E6 | Medium | Capabilities were checked only against the owner argument (a module could pass another module as owner), and `ModuleManifest.Capabilities/Requires` returned the live arrays (a reader could rewrite any manifest). | FIXED: checked against the owner and the running module; arrays are copies. Documented as a guardrail, not a sandbox (mods are full-trust .NET) | `manifest:` check; `SDK-selftest` capability checks |
| E7 | Medium | Draw pass: enumerated the live module list on the render thread while hot reload can add to it; a throwing `OnDraw` logged a full stack every frame (sync file I/O; the 1 MB log rotation evicts startup evidence). | FIXED: draw iterates a published array; a draw failure logs once and the tick fails the module | NEEDS LOCAL VERIFY: `SDK-hot-reload`, `SDK-inspector-review` |
| E8 | Medium | Commands: last registration silently won, so a module could take over another module's or an engine command; a malformed argument (`int.Parse`, index) in a handler stopped the whole module (the autopilot); engine command failures had no stack. | FIXED: a held name is refused and logged (`command_refused`); `ArgumentException`/`FormatException` is an error reply with the usage; autopilot `Args` throws clear `ArgumentException`s; `command_failed` logs the stack | `commands:` checks (6) |
| E9 | High | `Memory.Patch`: two modules patching overlapping bytes each saved the other's bytes as "original"; whichever stopped second wrote a stale patch back into the game. | FIXED: overlapping patches from another module are refused (`memory_patch_refused`) | code review; no mod patches memory today |
| E10 | Medium | Config reloads ran as engine code, and a failing reload removed watches from the list being iterated (skipping others). | FIXED: `RunAs`, iterate a copy | code review |
| E11 | Medium | `restart`/`reload`: a throwing constructor aborted the command after the modules were stopped; the rest never restarted. | FIXED: `TryReplace` keeps the others going and records the reason | NEEDS LOCAL VERIFY: `SDK-hot-reload` |
| E12 | Medium | Exact kills: records were pruned after 300 frames but consulted for 600, so a corpse hit 5–10 s after death published a second exact `PedDied`; ped handles are reused, so stale records could also hide a new ped's kill. | FIXED: pruned at the 600-frame window, dropped on `PedRemoved` | NEEDS LOCAL VERIFY: `SDK-exact-damage` |
| E13 | Medium | `Vehicles.GetDriver` called `GET_DRIVER_OF_CAR`, which faulted on pooled vehicles and corrupted game state (NATIVES.md). | FIXED: the snapshot driver when the vehicle is in the snapshot; the native only outside it | NEEDS LOCAL VERIFY: new self-test check `vehicle-driver` (the `+0xF50` read was untested) |
| E14 | Low | `TASK_PLAY_ANIM`, `_UPPER_BODY`, `_SECONDARY` were chosen by a ternary, so the verifier never saw them, and they were missing from `native-hashes.csv`. | FIXED: hashes added (checked against FusionFix `natives.ixx`); the scan reads `string native = ...` literals; a repository-only check runs in the cloud | `every native the DLL calls is listed` (RAN-PASS); game half NEEDS LOCAL VERIFY (`LOOP-verify`) |
| E15 | Low | `SdkSelfTest` subscribed the vehicle events again on every run. | FIXED | NEEDS LOCAL VERIFY: `SDK-selftest` |
| E16 | Low | Silent catch blocks (AGENTS.md rule 8): `ModuleReloader` (file busy), `LiveMemory` (section scan). | FIXED: logged | build |
| E17 | Low | Engine heuristics typed in code (AGENTS.md rule 3 grey area): bullet hit match 2.5 m, nearest-vehicle 60 m, GoTo 45 s / enter-car 10 s timeouts, radial dead zone 0.55, governor streaks. Engine constants, not gameplay tuning. | OPEN: owner decides whether they move to `engine.json` | — |

### Native core and ABI

| # | Sev | Finding | Status | Proof |
|---|---|---|---|---|
| N1 | High | `Natives::call` never checked whether a native had faulted: callers check `ready()` once per frame, so after a contained fault the same native ran again for every remaining ped or vehicle in that frame (a contained fault already corrupted game state once). | FIXED: a faulted native is never invoked again that session; verification cannot switch it back on | `natives_test` (RAN-PASS, host build) |
| N2 | Medium | Hooks could outlive the core: an escaped fault or a snapshot-size mismatch set `Available = false` without `lc_shutdown`, and `Shutdown()` then returned early, leaving the damage detour installed. | FIXED: every switch-off runs `lc_shutdown` once (`engine_core_shutdown hooks removed`) | code review; NEEDS LOCAL VERIFY that a normal session is unchanged (`SDK-exact-damage`) |
| N3 | Low | Damage records dropped by a full ring (64 between two engine frames) were counted but never reported. | VISIBLE: `lf hooks` prints `damage_ring dropped=N`; a snapshot field needs ABI 6 | build |
| N4 | — | ABI: `liberty_core.h` against `CoreAbi.cs` and `CoreBridge`'s snapshot accessors. | No mismatch at ABI 5. Now checked mechanically: 14 structs word for word, enums, every flag family, version, snapshot offsets | verifier "LibertyCore C ABI" (53 checks, RAN-PASS) |
| N5 | Low | `liberty_core.h` required `__declspec`, so core sources could not be unit-tested on the host. | FIXED: exports only on Windows; tests get the include path | build-core |

### Raycast (T-027)

The public API is coherent: kind filtering happens in the core's pass-through walk (unit-tested); the ignore list is
explicit (the first entity goes to the game); failures are `Unavailable` (off, faulted, outside the engine tick,
including `OnDraw`) or `Inconclusive` (pass budget spent); `raystats` counts queries, tests, hits, clears, passes,
inconclusive and faults; `raydebug`/`raybits` are labelled research. One doc gap was fixed:
`HasLineOfSight(viewer, target)` is also false beyond `raycastMaxLengthMeters`.

**`RayMask.Objects`:** no doc or probe result named a vanilla prop with collision, so none was picked from memory.
`PROBE-collision` now lists `propCandidates` (a drawable shipped next to a same-named bounds resource). Scenario lines
can use `{probe:<id>:<field>}`, a value from a probe report of the same run, and checks can declare `needs`.
New check `T027-raycast-objects` spawns the first candidate and requires `Hit kind=Object` with the matching handle (all
kinds and objects only), and a pass-through without objects. NEEDS LOCAL VERIFY.

### Tooling

| # | Finding | Status | Proof |
|---|---|---|---|
| T1 | `tools/verify.ps1` compiled with the .NET Framework's C# 5 compiler on Windows (C# 5 pinned elsewhere) while everything else uses the pinned Roslyn C# 7.3: two toolchains. | FIXED: Roslyn C# 7.3 everywhere; the verifier builds its own `Liberty.Sdk` copy and can test engine plumbing | verifier RAN-PASS; Windows NEEDS LOCAL VERIFY (`LOOP-verify`) |
| T2 | `Save-Screenshot` looked only in `C:\Program Files (x86)\Steam\userdata`: any other Steam folder lost every screenshot step. | FIXED: `LIBERTY_STEAM_DIR`, registry (HKCU `SteamPath`, HKLM `InstallPath`), default; a clear error names the places searched | `steam:` tests (RAN-PASS) |
| T3 | `Get-SessionLog` re-read the whole log on every 500 ms poll, and every scenario after the first (a new PowerShell with the game running) had no session start, so it read every earlier session. | FIXED: incremental reads (appended bytes, complete lines, reset on rotation or a new session); the start comes from the game's process | `log:` tests (RAN-PASS) |
| T4 | SDK doc examples could drift from the SDK (ENGINE.md's sample used `Started/Update/Stopped`, `PlayAndWait`, `Entities.CreateObject`, `e.Handle`). | FIXED: examples live in `docs/sdk/examples` and compile against the SDK (warnings as errors); every C# block in the SDK README and ENGINE.md must come from them | verifier "SDK examples" (RAN-PASS) |

### Research claims

`docs/game-api/MEMORY.md` now carries an evidence register: every reverse-engineered claim the code relies on,
labelled VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN, with its evidence and its fail-safe. No game address
is a constant in code (comments and the verifier's pins only; ADR-0004 holds). Code relies on two PLAUSIBLE claims,
both fail-safe and now covered by a queued check: the vehicle driver offset `+0xF50` (self-test `vehicle-driver`) and
the raycast link for vehicles and objects (`T027-raycast`, `raycast-vehicle`, `T027-raycast-objects`). Raycast.md and
Collision.md already labelled their claims.

### Documentation

- README: status, map (SDK, core, mods, content, local checks), mod-author entry points.
- ENGINE.md: module sample on the current SDK (compiled), error isolation and ownership as they are now, snapshot,
  events and services tables against the real APIs.
- SDK README: exact damage fields (the hook exists), `loadModAssemblies` default, command, owner and capability rules.
- NATIVES.md: stale autopilot path; a cp1252 line in a UTF-8 file (every tracked text file is now UTF-8).
- T-024 no longer calls T-015 blocked; PROJECT_STATE's stale "Not done yet"; CLOUD_LOCAL_LOOP documents `needs` and
  probe values; Raycast.md/T-027 paths.
- Task numbers are unique (the second table in `docs/tasks/README.md` is the original backlog for the same cards).

## `cloud/audit-cleanup` (WIP, evidence only)

Each of its 21 findings was re-checked against the current code and not merged. 1-3 → E1-E3 (its patches were
uncompiled; the fixes here are wider: every owning call, the ledger, capabilities). 4 → E6. 5 → E5 (policy decided:
callbacks are module code, consistent with the SDK contract). 6 → E13. 7 no longer applies (session 1 bumped both
sides to ABI 5; now checked mechanically). 8 → E12. 9 → E7. 10 → N1. 11 → N2. 12 → N3. 13 → E14. 14 → E8. 15 → E8
(refused instead of logged-and-replaced). 16 was accurate: the verifier gates the vehicle list on `GET_DRIVER_OF_CAR`,
which the core no longer calls per frame; left as is (the check runs once on a live vehicle, and relaxing it changes
in-game behaviour without evidence). 17 → E15. 18 and 19 → Documentation. 20 → E17 (open). 21 → E16.

## `codex/phase2-combat` and `codex/phase2-systems`

They share no history with `main`. Each of their two commits has a rebased counterpart on `main` (`fa04dd8`, `14dd7c8`,
`1655a30`, `905e57c`): every code, config and tool file those commits produced is byte-identical on `main`, and every
line they added to `ArsenalCore.cs`, `CONFIG_SCHEMA.md` and `PROJECT_STATE.md` is on `main` except one changelog
sentence per branch that `main` carries reworded. **No unique work**: the owner can archive and delete both branches
(`codex/phase2-foundations` is an ancestor of `main`). Agents do not delete branches.

## Checks added or changed (`tests/local/checks.json`)

- **Added** `T027-raycast-objects` (scenario, needs `PROBE-collision`).
- **Changed** `PROBE-collision` (reports `propCandidates`), `SDK-selftest` (new `vehicle-driver` check).
- **In-game behaviour changed by this PR, covered by queued checks:** `SDK-selftest`, `SDK-ui-review`,
  `T024-trunk-review` (menus run as their module), `SDK-hot-reload` (restart/reload, draw), `SDK-inspector-review`,
  `SDK-exact-damage` (kill de-duplication, core shutdown path), `SDK-engine-events`, `SDK-vehicle-events`, `LOOP-build`
  (core unit tests on Windows), `LOOP-verify` (verifier on Windows with the game).

## Open

- E17: engine heuristic constants in code (owner's call).
- N3: a snapshot field for dropped damage records needs ABI 6.
- The first `raycast-objects` candidate may have no collision in game; the review session then picks the next one from
  the probe report (the check's `unproven` says so).
