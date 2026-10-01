# Switching a lane from Claude Sonnet to Sol (GPT)

Use this when a Claude agent runs out of usage. Each lane can be switched on its own; the others keep running.

## The lanes

| Agent | Worktree | Branch | Prompt |
|---|---|---|---|
| Lane B (weapon wheel, trunk) | `C:\Users\IM576\GTAIV-Reborn-lane-b2` | `codex/lane-b-validation` | `PROMPT-LANE-B.md` |
| Lane C (gore, combat effects) | `C:\Users\IM576\GTAIV-Reborn-lane-c-t048` | `stage1/T-048` | `PROMPT-LANE-C.md` |
| Lane D (HUD) | `C:\Users\IM576\GTAIV-Reborn-lane-d` | `codex/T-049-hud-continuation` | `PROMPT-LANE-D.md` |
| Lane R (initial SDK 1.2/material validation) | `C:\Users\IM576\GTAIV-Reborn-research-t050` | `research/t050-material` | `PROMPT-LANE-R.md` |
| Orchestrator | `C:\Users\IM576\GTAIV-Reborn` | `main` | `PROMPT-ORCHESTRATOR.md` |

All agents follow `RULES.md` (this folder) and `AGENTS.md`.

For the 2026-10-01 Codex continuation, read main's [current launch brief](CONTINUATION.md) first. It overrides the
older known-state snapshots, bounds each initial assignment and closes the obsolete ColAccel request. The prompt
generator includes it automatically. This chat can delegate workers to these existing worktrees; manual new chats
below are an alternative. Preparing a prompt does not launch a worker.

## Steps for one lane (about two minutes)

1. Make sure the Claude thread for that lane is stopped: it shows its usage-limit message, or you press Stop. Leave its
   game run alone if one is going; the verifier finishes and restores the game by itself.
2. Open PowerShell in `C:\Users\IM576\GTAIV-Reborn` and run (replace `B` with `C`, `D`, `R` or `Orchestrator`):

   ```powershell
   powershell -ExecutionPolicy Bypass -File tools\handoff\Get-SolPrompt.ps1 -Lane B
   ```

   It copies the lane's prompt plus its current state (commits, uncommitted files, latest test results, who has the
   game) to the clipboard, and saves the same text to `%TEMP%\sol-prompt-B.md`.
3. Open a new Sol chat with its working folder set to the lane's worktree (table above), with permission to run
   PowerShell commands, and paste. Name the chat "Liberty Vanilla+ Stage 1 Lane B (Sol)".
4. The Sol agent first checks the Claude agent stopped, reviews all the work, and writes `docs/handoffs/Lane-B-live.md`
   before changing anything. That is expected; let it.

## Notes

- Lane D ran as a Claude cloud thread. Sol takes it over locally (it can run the game here); its prompt fetches the cloud
  thread's pushed work first. Stop the cloud thread before you switch.
- If the orchestrator (this coordinating chat) runs out too, switch it last, with `-Lane Orchestrator`.
- The Claude lanes keep `docs/handoffs/Lane-<X>-live.md` current after each run, so a sudden cutoff loses little.
- Sol agents never merge into `main`; the orchestrator does. Only you mark a task DONE, after your playtest.
- To switch a lane back to Claude later, use the same live handoff file: tell the Claude thread to read
  `docs/handoffs/Lane-<X>-live.md` and review everything Sol did before continuing.
