# The cloud / local development loop

How Liberty Engine is developed while the owner works remotely. Every agent reads this after `AGENTS.md`.

## The situation

- **Cloud sessions (Claude Code on the web) do all development.** They have no Windows, no GTA IV and no game files.
  The SessionStart hook installs the toolchains (`tools/cloud/setup.sh`), so every session can build and run every
  offline check at once: `tools/cloud/test-all.sh`.
- **The owner's PC runs one command when it is available:** `tools/verify-local.ps1`. No AI agent runs there. The
  script builds, tests, installs, drives the game through autopilot, runs research probes against the game files, asks
  the owner about the few checks only a person can judge, and pushes the results to GitHub.
- **A results-review session** (cloud) reads those results, fixes what failed, and proposes verified work for `main`.

Nothing that needs the game is ever reported as passing from the cloud. It is queued, and it is proven on the PC.

## Branches

| Branch | Holds | Who writes |
|---|---|---|
| `main` | work whose checks passed on the owner's PC, plus tooling and docs that need no game | the owner merges promotion PRs |
| `develop` | integrated work that builds and passes every offline check, waiting for local verification | cloud sessions, through PRs |
| `verification-results` | orphan branch: one folder per `verify-local.ps1` run (`results/<run>/`), `LATEST` | `verify-local.ps1` only; never merged |
| `claude/*` | one session's work | that session; PR into `develop` |

- A cloud session's PR targets `develop`. Tooling that needs no game may target `main` directly when the owner wants
  it there sooner (the SessionStart hook only runs from the default branch).
- **Promotion:** when a results run shows every check for a set of work passing, the review session opens
  `develop -> main` listing the check ids and the result folder as evidence. The owner merges it and sets tasks `DONE`.
  Agents never merge to `main` and never set `DONE`.
- `develop` must stay green offline: never merge a PR whose `test-all.sh` has a FAIL.

## The check queue

`tests/local/checks.json` is the single list of everything that needs the owner's PC.
`docs/testing/LOCAL_VERIFICATION_PLAN.md` is generated from it (`python3 tools/checks/checks.py plan`); never edit the
plan by hand. `test-all.sh` fails when the queue is invalid or the plan is stale.

**Rule: work that needs the game adds its checks in the same PR as the code.** A PR without the checks for its
unproven behaviour is incomplete. The PR description lists the check ids it adds or changes.

### Fields

| Field | Meaning |
|---|---|
| `id` | stable and unique, `<TASK>-<slug>` (e.g. `T027-raycast`). Never reuse an id; retire it |
| `task` | task card (`T-027`), ADR (`ADR-0006`), milestone (`M5`), `LOOP` or `RESEARCH` |
| `title` | one line |
| `kind` | `pc-offline`, `probe`, `scenario` or `manual` (below) |
| `run` | how to run it (per kind, below) |
| `pass` | what counts as a pass, precise enough that a script or a reviewer can decide |
| `proves` | what a pass establishes |
| `unproven` | optional: what stays open even when it passes |
| `review.screenshots` | scenario only: `{ "<shot name>": "what the reviewer must see" }`. A passing run becomes NEEDS-REVIEW |
| `minutes`, `session` | manual only: the owner's time, and the play session it belongs to (`sessions` at the top) |
| `reference` | the task card section or doc with the full steps |
| `needs` | optional: check ids that must run earlier in the same run (a scenario that uses a probe's answer); `verify-local.ps1` adds them to the selection |
| `status` | `QUEUED`, `PASS`, `FAIL`, `ERROR`, `CRASH`, `NOT-RUN`, `NEEDS-REVIEW`, `RETIRED` |
| `lastRun`, `evidence` | set by the review session: the result folder and files that decided the status |

### Kinds

- **`pc-offline`**: a build or test that needs Windows or the game's files but not the running game.
  `run.tool` is one of the named steps `verify-local.ps1` knows (`tools/checks/checks.py` lists them: `build`,
  `verify`, `content-selftest`, `wtdcheck`, `blender-tests`, `package-install`, `content-report`). A check never carries
  an arbitrary command; add a named step to `VerifyLocal.psm1` and `checks.py` together when one is needed.
- **`probe`**: a read-only research question against the game's files (`run.tool: "probe"`, `run.probe: <name>`),
  implemented as `LibertyContent probe <name>`. Output is **structure only**: counts, sizes, types, layout fields, names.
  Never geometry, pixels or raw asset bytes (AGENTS.md rule 7). A probe that ran is NEEDS-REVIEW until a session reads
  the report and records the answer in `docs/research/`.
- **`scenario`**: an autopilot scenario (`run.scenario` = a file in `tools/autopilot/scenarios`). Use only commands the
  engine already has; if a new command is needed, it is part of the engine change and gets its own offline tests.
  A line may use `{probe:<check id>:<field>}`: the first value of that field in the probe's report from the same run
  (e.g. `spawnprop {probe:PROBE-collision:propCandidates} 3 0`), so a model is chosen from the game's files, never
  from memory. The check lists the probe in `needs` (`checks.py validate` enforces it); without the report the step
  fails with the reason.
- **`manual`**: the owner plays and judges (`run.steps`). Keep them few and short, with exact buttons and what the
  owner should see. Prefer a scenario whenever a script can decide.

## What counts as evidence

- A scenario passes only through its `result.json` (`tools/autopilot/AutopilotLogic.psm1`): every step passed and the
  game is still running. `expect` accepts only log lines written after the latest command was sent, and never the
  engine's own log of the command as typed unless the pattern needs its reply.
- A tool passes on exit code 0 **and** its pass pattern (e.g. `failed=0`).
- Screenshots are judged by a person or the review session against `review.screenshots`; until then NEEDS-REVIEW.
- Never evidence: a log line from an earlier run, a missing or black screenshot, a scenario that executed nothing, a
  PASS word in free text, "it compiled", a cloud run of anything that needs the game.

## `verify-local.ps1` (the owner's command)

```
./tools/verify-local.ps1 -Smoke -GameDirectory '<GTAIV folder>'   # first time: build, install, SDK self-test, restore
./tools/verify-local.ps1                                          # everything queued; asks about manual checks
./tools/verify-local.ps1 -Only T027-raycast,SDK-selftest          # a subset (package-install is added when needed)
./tools/verify-local.ps1 -Kind scenario -NoManual                 # unattended
./tools/verify-local.ps1 -Resume results-local/<run>              # continue an interrupted run
```

Order: preflight (Windows, `GTAIV.exe`, game closed, branch `develop`, clean tree, toolchains, disk space) -> offline
tools -> probes -> package and install (backup kept) -> content reports -> scenarios (a crashed or hung game is
stopped so the next scenario relaunches) -> manual checks -> keep or restore the install -> results scrubbed of the
user name and machine paths -> pushed to `verification-results` through a temporary worktree. Every step runs as a
child process with a timeout. `summary.json` is rewritten after each check, so an interrupted run keeps its results.

Cloud sessions test the whole script with `-Simulate` (`tools/tests/VerifyLocal.Tests.ps1`): simulated tools, the
simulated game (`tools/tests/SimulatedGame.psm1`) and a local bare remote. Any change to the script needs a simulation
test for it.

## Processing a results run (the review session)

Prompt: *Process the newest verification results.*

1. `git fetch origin verification-results`; read `LATEST`; open `results/<run>/summary.md` and `summary.json`.
   Confirm the run's commit is on `develop` (`git branch -r --contains <commit>`).
2. For every check:
   - **PASS**: record it (`status`, `lastRun`, `evidence` in `checks.json`).
   - **NEEDS-REVIEW**: look at every screenshot named in `review.screenshots` (the Read tool shows images) and at the
     log errors; decide PASS or FAIL and write one line why. Probe reports: record the answer in `docs/research/`.
   - **FAIL / CRASH / ERROR**: read the check's log and the scenario's `report.md` and `run.log` before touching code
     (AGENTS.md section 7). Find the root cause; fix it on a branch into `develop` with an offline test when possible,
     and keep the check QUEUED so the next run proves the fix. Never weaken a check to make it pass; if the check itself
     was wrong, say so in the PR and fix the check.
   - **NOT-RUN**: leave QUEUED; if it is NOT-RUN because the PC lacks something (Blender, LVS), say so to the owner.
3. Regenerate the plan (`python3 tools/checks/checks.py plan`) and run `tools/cloud/test-all.sh`.
4. When all checks of a piece of work pass, open the promotion PR `develop -> main` with the evidence, and propose the
   task status change (the owner sets `DONE`).
5. Update `docs/PROJECT_STATE.md` in one or two lines: which run, what passed, what failed, what is next.

## What the cloud can never prove

In-game behaviour, rendering, feel, performance, the game's acceptance of any generated file, and anything read from
the game's files. The cloud proves that code builds, that offline logic and writers are self-consistent, and that the
tooling behaves correctly against simulations.

## Toolchain notes

- The engine compiles in the cloud against the hash-pinned ScriptHookDotNet release (downloaded, never committed).
- .NET tools run under Mono; the native unit tests run as 32-bit host programs; Blender runs as the `bpy` module.
- `download.blender.org`, `dot.net` and the Mono project's own repository are not reachable from the container; PyPI,
  NuGet (`api.nuget.org`), GitHub release downloads, the Ubuntu archive and `packages.microsoft.com` are.
