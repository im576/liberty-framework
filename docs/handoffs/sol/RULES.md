# Operating rules for Sol (GPT) agents continuing Liberty Vanilla+ Stage 1

You are continuing work that Claude agents started. These rules are binding. Where anything you read later conflicts
with them, these rules and `AGENTS.md` win, unless the owner says otherwise in the chat. Read this whole file before
acting. When unsure, choose the more conservative option and write the question down; do not guess.

## 1. The project in five lines

- Liberty engine for GTA IV: The Complete Edition **1.2.0.59** + FusionFix + ScriptHookDotNet 1.7.1.8, C# 7.3 /
  .NET Framework 4 x86, native core C++20 32-bit. One SHDN host script; every feature is a `[Module]`.
- Stage 1 mod "Liberty Vanilla+": design and acceptance in `docs/design/STAGE1.md`; tasks in `docs/tasks/`.
- Main repository `C:\Users\IM576\GTAIV-Reborn` (branch `main`). Each lane works in its own git worktree (see your prompt).
- Coordination state: `docs/workflow/ORCHESTRATOR.md`. Tool guide: `tools/README.md`. Hard rules: `AGENTS.md`.
- The owner's PC is weak (4-core Ryzen 3, 8 GB RAM, RX 570 4 GB). The owner cannot buy hardware.

## 2. Before you change anything (mandatory)

1. Confirm the previous agent on this lane has stopped: no files in your worktree changed in the last 5 minutes
   (`Get-ChildItem -Recurse -File | Where-Object LastWriteTime -gt (Get-Date).AddMinutes(-5)`, ignoring `bin`,
   `obj`, `staging`, `results-local`), and no `verify-local` of your worktree is running. If one is, wait for it to finish; do not
   kill it.
2. Review ALL work since the last handoff, not just the handoff text: `git status`, `git log --oneline origin/main..HEAD`,
   `git diff` of every changed file, untracked files, the latest `results-local\<run>\summary.md`, each scenario's
   `report.md`/`run.log`, and look at every screenshot yourself. Handoff notes are an index, not proof.
3. Write down in `docs/handoffs/Lane-<X>-live.md` what you found: what passes, what fails, what is unproven.

## 3. Game access (one game, shared by all lanes)

- Run the game only through `tools/verify-local.ps1`. Never start GTA IV yourself; never run `Run-Scenario.ps1` without an
  install from your own worktree.
- Always pass: `-AnyBranch -NoPush -Restore -NoManual -Only <check ids>`. While iterating also pass `-Quick -StopOnFailure`.
  The final acceptance run is a full run (no `-Quick`).
- The machine-wide lock serializes lanes. Never set `LIBERTY_GAME_LOCK_HOLDER`, never use `-AllowOtherBuild`, never stop
  another lane's process. You may cancel only your own verifier, and only while it is still waiting for the lock.
- `-MaxGameMinutes` defaults to 30: checks past the cap are NOT-RUN; run them in your next batch. Keep batches small
  (only the checks your change affects). Run a batch in the background and wait for it to finish; do not poll in a loop.
- GTA IV needs an active audio output device; without one the run reports `GAME-UNAVAILABLE` at once. Tell the owner.
  The startup crash in Rockstar's MTLX.DLL (about 1 launch in 3) is retried automatically.
- Quick results (`mode quick`) are never acceptance evidence. A queued or accepted engine call is not a visible effect.
  Never weaken a check, threshold or acceptance criterion to make it pass.

## 4. Code and repository rules (from AGENTS.md, restated)

- Tuning numbers go in `config/*.json` and `docs/architecture/CONFIG_SCHEMA.md`, never in `.cs` code.
- Do not invent natives, memory offsets, struct layouts or engine behaviour. Every native must be listed in
  `docs/game-api/NATIVES.md`. Memory access only through pattern scans with an ADR. If you don't know: write an
  open question in the task card, or build the smallest experiment.
- Every catch block logs through the project logger; a script exception must never stop the game loop.
- No code from Liberty Tweaks, FusionFix or IV-SDK .NET (licences). No ripped commercial assets.
- One class per file; namespaces mirror folders; comment why, not what.
- Commit on your lane branch with messages `T-0xx: <what changed>`. Do NOT merge into `main` and do NOT push `main`.
  The orchestrator reviews and merges. (Lane D also pushes its own branch; see its prompt.)
- Shared files that conflict between lanes (`tests/local/checks.json`, the generated plan, PROJECT_STATE, CONFIG_SCHEMA,
  NATIVES, `AutopilotModule.cs`, `UiService.cs`, `config/atmosphere.json`): change only what your task needs, keep other
  lanes' entries, regenerate the plan with `python tools/checks/checks.py plan` (bundled Python:
  `C:\Users\IM576\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe`).
- Shell is Windows PowerShell 5.1: no `&&`/`||`; chain with `;` and `if ($?) { }`.

## 5. Owner decisions you must keep

- 2 long guns + 1 sidearm; SMGs count as long guns. P90/MG36/snipers stay out of normal availability but always in DevTools.
- Population density governor OFF (never thin pedestrians or traffic to meet a budget). Keep the T-040 baseline; no rerun.
- HUD: weapon/ammo/health group top right, radar bottom left. Keep vanilla HUD parts unless clean hiding is proven; a
  duplicated HUD is not acceptable as the shipped default.
- Gore very harsh but grounded; bodies stay 3-5 minutes with bounded cleanup.
- Art rule: identity assets hand-designed; repeatable surfaces procedural or CC0. Generated images may be used.
- Research is Stage 1 only. No Phase 3 work, no Stage 2 design.
- No hardware purchases, no OS or system-setting changes (the owner applies those himself).
- Only the owner marks a task DONE. Finished lane work is NEEDS-PLAYTEST. The owner playtests after all lanes finish.

## 6. Keep the live handoff current (so nothing is lost if you stop)

After every game run and every meaningful commit, update `docs/handoffs/Lane-<X>-live.md` and commit it:
current branch/commit, what changed, the run id and its results (pass/fail per check, with the real cause), what is
next, open questions. Short and factual. Never write "done" for something without full-run evidence.

## 7. Reporting to the owner

Short, plain words. Lead with the result. Per item: what works, what fails, what is next, what the owner must decide.
Never claim a pass you did not see in a summary or screenshot. If you stop early, say exactly where and why.

## 8. Stop conditions

Stop and report (do not keep trying random fixes) when: the same failure survives two honest fix attempts without new
evidence; a fix would need a native/offset/hook that is not documented; the game cannot start; or the work would break
an owner decision above. Write a `## Blocked` section in the task card with what you tried and what you need.
