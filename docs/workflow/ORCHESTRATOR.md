# Orchestrator handoff (Stage 1)

For the Claude session that coordinates Stage 1 for the owner: it plans, reviews the lane threads, answers the owner,
files task cards and art requests, and keeps docs and decisions current. It does not build lane features itself.
Updated 2026-09-30 when the first orchestrator thread handed off (to avoid context compaction).

## Read first

`AGENTS.md`, `docs/PROJECT_STATE.md`, `docs/design/STAGE1.md` (design, decisions in section 12, budgets in section 10),
`docs/tasks/README.md` (lanes), the T-040 baseline `docs/reports/2026-09-30-stage1-baseline.md`, and this file.

## How the work runs

- Lanes run in separate Claude threads (Sonnet 5.5 high), each in its own worktree `C:\Users\IM576\GTAIV-Reborn-lane-*`,
  one branch per task `stage1/T-0xx`, merged into `main` by fast-forward push when the task's in-game checks pass.
  The builder prompt ("Prompt 1", with `LANE = X`) and the research prompt are in the first orchestrator thread; the
  research and image prompts follow the same structure (read AGENTS.md, task card, verify-local, merge rules).
- Read the lane threads with the session tools: `mcp__ccd_session_mgmt__list_sessions`, then `list_events` with the
  session id (transcripts are mostly tool calls; combine with git and `results-local`). Session titles: "Liberty Vanilla+
  Stage 1 Lane A/B/C/D", "... Lane 0".
- Repo-side progress: `git fetch`; `git log origin/main`; per worktree `git -C <wt> log origin/main..HEAD`, status, and the
  newest `results-local\<run>\summary.md` (verify-local results; scenario `report.md`, `run.log`, screenshots).
- The game is shared: `tools/verify-local.ps1` holds a machine-wide lock; packaging uses `-Fast`. **`tools/autopilot/Run-Scenario.ps1`
  takes no lock**: lane "quick"/ad-hoc runs through it collide. On 2026-10-01 00:09-00:30Z, Lane B (wheel) and Lane C
  (gore) ran in the same game at once, each on the other's installed build, so those results are void. Lanes must run the
  game only through verify-local (`-Only <ids>`) or hold its lock. GTA IV needs an audio
  output device to start (lanes were blocked once with none connected). Rockstar's MTLX.DLL startup crash forces relaunches.
- Art: `python tools/art/artq.py list|check|approve|reject|prep`; the image app follows `docs/art/GENERATOR.md` (it must
  register imperfect candidates; Claude reviews). ART-001..012 approved, eight prepped. Owner confirmed generated images
  may be used.

## Status (reviewed 2026-10-01 ~00:35Z by the second orchestrator thread)

| Lane | Tasks | State |
|---|---|---|
| 0 | T-040 foundation | Merged, NEEDS-PLAYTEST (capture points, measurement tooling, baseline) |
| A | T-041 arsenal, T-042 gunplay + shoulder swap, T-043 reticles | Merged, NEEDS-PLAYTEST; owner answers pending (below) |
| B | T-044 loadout (merged, NEEDS-PLAYTEST), T-045 wheel, T-046 trunk UI | Thread running. T-045 (`lane-b`, 3 commits) draws in game, but its 3 quick runs FAIL on equip; those runs overlapped Lane C, so they prove nothing yet. T-046 (`lane-b2`, 1 commit on top of T-045) has not been run in game. Nothing merged. Told to rerun under the lock |
| C | T-047 gore, T-048 effects | Thread running. T-047 (`lane-c`, 4 commits): last valid run (19:03Z) had trauma and dismember FAIL (missing `gore_stats` lines), head/panic NEEDS-REVIEW. The later dismember experiments crashed, but on Lane B's build during the overlap, so they are void. T-048 (`lane-c-t048`) WIP, not run. Nothing merged. Told to redo under the lock |
| D | T-049 HUD | Built **offline only** in a cloud thread (no local Lane D session); branch `origin/claude/ecstatic-keller-qu3sto` (3 commits on 1e74338), **not merged, never run in game**, no results-local. Card NEEDS-PLAYTEST with checks `T049-hud-components`, `T049-stage1-hud`, `T049-hud-look` queued. Design meets the card (top-right group, radar left alone, vanilla hidden only where replaceable, off switch). Health/armour bars are off by default: no hideable vanilla component is known for the radar arcs. Needs a local thread to run its checks, fix and merge |
| R | T-050..T-055 research | Answers written in `C:\Users\IM576\GTAIV-Reborn-research` (13 commits, incl. `docs/research/OptimizationMods.md`), **not merged into main**; ask that thread to merge |
| P | T-056 performance pass | Card written; starts after lanes A-C merge, before Slice B |

## Owner decisions made in this thread (all recorded in STAGE1 section 12 or the cards)

2 long guns + 1 sidearm; SMGs are long guns; density governor off (no population thinning, find other optimizations);
P90/MG36/snipers kept, out of normal Stage 1 availability, always in the DevTools/mod menu; no baseline rerun; HUD
weapon/ammo/health group top right, radar bottom left (ART-007 r2); art rule "identity-defining assets hand-designed,
repeatable surfaces procedural or CC0"; gore very harsh but grounded, bodies 3-5 min; Hove Beach first in the art pass.

## Waiting on the owner

1. Lane A (T-042): accept the class targets (first shot ≤ 0.5°, burst recovery 0.8 s / 1.5 s shotgun, AK climb 6-12°,
   Uzi 4-9°)? Keep the recoil cap after 30 rounds (recommended) or let it keep climbing? Explained in plain words; no
   answer yet.
2. VRAM ceiling: the GPU is an RX 570 **4 GB** (driver and registry), not 8 GB as STAGE1 says. Proposed: hard ceiling
   +300 MB. Not answered; STAGE1 still says 8 GB / +350 MB.
3. Monitor refresh rate (for a frame cap: 40 fps needs 120 Hz, else 30).
4. T-044 open questions (melee/thrown rule, ammo caps, unassigned stash), T-040 and T-043 test steps (owner playtests).
5. T-049: if the in-game probe finds no hideable component for the radar's health/armour arcs, either show Liberty's
   top-right bars as well as the vanilla arcs (`drawWithoutHidingVanilla: true`), or keep the vanilla arcs until the
   radar redraw (T-054). Recommended: wait for the probe result.
6. Lane D needs a local thread to run its checks (the cloud thread cannot run the game).

## Owner context

- PC: Ryzen 3 2300X 4C/4T, one 8 GB DDR4-3000 stick (single channel; ~0.3 GB free during play), RX 570 4 GB, game on
  SSD (`C:\Games\...`), pagefile 24 GB on the HDD, SSD 16 GB free. **Owner cannot buy hardware.**
- 60 fps is not reachable on this PC (game alone ~40-45 fps normal, ~27 worst case; the mod adds ~2 ms). Plan: remove
  the mod's spikes and cost (T-056), steady frame cap, owner-side Windows fixes (pagefile to SSD, close background apps,
  Steam overlay off, Defender exclusion; only the owner applies these, after all threads are idle since it needs a restart).
  Profiling and patching GTA IV's own hot spots is Phase 3.
- The owner prefers short, plain answers and ready-to-paste prompts; does not want to be asked questions by lane threads.
