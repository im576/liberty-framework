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
- The game is shared. Since 2026-10-01, verify-local, install/rollback, Run-Scenario and Run-Suite all hold one machine-wide
  lock (`tools/local/GameLock.psm1`), and Run-Scenario refuses a build installed from another worktree
  (`installed-build.json`). Before that, Run-Scenario had no lock, and on 2026-10-01 00:09-00:30Z Lanes B and C drove the
  same game at once, each on the other's build (those results are void). **A worktree only gets the lock once it has merged
  `main`**: check that every running lane has. Packaging uses `-Fast`. GTA IV needs an audio
  output device to start (lanes were blocked once with none connected). Rockstar's MTLX.DLL startup crash forces relaunches.
- Art: `python tools/art/artq.py list|check|approve|reject|prep`; the image app follows `docs/art/GENERATOR.md` (it must
  register imperfect candidates; Claude reviews). ART-001..012 approved, eight prepped. Owner confirmed generated images
  may be used.

## Codex takeover and return (2026-09-30 evening, Pacific)

Codex ran lanes B, C and D from about 19:40 to 21:30 while Claude usage refilled. Its reports are in
`C:\Users\IM576\OneDrive\Documents\ChatGPT\GTA4-Reborn\research\` (takeover review, Sonnet handoff setup), and each lane has
`docs/handoffs/Codex-Lane-<X>-2026-09-30.md` in its worktree. At 21:32: Codex was idle, no verifier was running, the lock was
free, and the game was restored (`installed-build.json`: restored from `phase2-20260930-212221`).
- **B**: lane-b2 `codex/lane-b-validation` (= lane-b `codex/lane-b-wheel-trunk`) at `0f5825f`, with the original T-045/T-046
  tips kept. T045-ui-path PASS shows the text-drawing path causes ticks of 1 s or more (primitives and control capture don't);
  T045-weapon-wheel FAIL (4 steps). Sonnet Lane B was told to review all Codex work, then fix the text stall, then run
  wheel/trunk acceptance.
- **C**: lane-c-t048 `stage1/T-048` at `dec20b9`. Trauma 21/22 (needs review); persist and cleanup FAIL (mission-owned
  fixtures are rejected by the ownership rule); effects ERROR (killed after 600 s); dismember 8/20 floating limbs from the
  earlier run. Sonnet Lane C was told to review, then continue.
- **D**: lane-d `codex/T-049-hud-continuation` at `b1dc3d7`, plus uncommitted Codex edits (HudModule, Stage1HudChecks, new
  HudAmmoSample.cs), kept as they are. BLOCKED on visible hiding: wanted and radar stayed visible, and weapon/ammo got
  duplicated, so the guard keeps the vanilla HUD. Its original cloud session can't be reached from here; the review bundle
  (git bundle, patches, uncommitted diff) is in `research\lane-d-review-bundle-2026-09-30`. Owner to choose: link the cloud
  session, or start a local Sonnet Lane D thread.

## Cleanup and review (2026-09-30 ~21:45 Pacific)

- Worktrees retired (all merged and clean; their branches stay): lane-0, lane-a, lane-a2, lane-a3, art-generator. Their
  test evidence was moved to `D:\GTAIV-Reborn-Tools\archive\results\<lane>`. Active worktrees now: main, lane-b2 (B), lane-b
  (B, diagnostic; retire after B merges), lane-c-t048 (C), lane-c (older T-047, contained in T-048; retire after C merges),
  lane-d (D), research.
- Bloat moved, not deleted, to `D:\GTAIV-Reborn-Tools\_to-delete` (8.6 GB; the owner deletes it): the extracted mod archives
  from the other-mods research (7.6 GB; the findings are in `research/OtherModsDeepDive.md`), installer zips that were already
  extracted, 72 old game install backups (kept: the oldest phase2, everything from 2026-09-30, the single dxvk/phase1/weapon
  backups, and the oldest and newest violent backup), and stale temp test folders. `C:\Users\IM576\GTAIV-Reborn-pr7-macfix` is
  an orphan non-git folder with one locked file; delete it after a reboot.
- The research worktree's 8 uncommitted notes were committed on `research/stage1` (`67414a6`, local only, not merged).
- Main bug found by Codex: `config/atmosphere.json` on main still has `density.enabled: true`, against the owner's
  "density governor off". Lanes B, C and D each switch it off; take it once when integrating.

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
3. Monitor refresh rate (for a frame cap: 40 fps needs 120 Hz, else 30). Found 2026-10-01: Windows lists an ASUS VG279QM
   and a VG248 (both 144 Hz or faster), so the 40 fps cap is possible; the owner still has to confirm which one is the main screen.
   Testing speed (45561b4, 2026-10-01): package 411 s cold / 4 s cached, build outside the lock, fail-fast on frozen or
   unstartable games. Lanes must merge main to get it; tell lanes to batch checks in one verify-local run and not poll.
4. T-044 open questions (melee/thrown rule, ammo caps, unassigned stash), T-040 and T-043 test steps (owner playtests).
5. T-049: if the in-game probe finds no hideable component for the radar's health/armour arcs, either show Liberty's
   top-right bars as well as the vanilla arcs (`drawWithoutHidingVanilla: true`), or keep the vanilla arcs until the
   radar redraw (T-054). Recommended: wait for the probe result.
6. Lane D: owner approved a local thread to run its in-game checks (2026-10-01); prompt given in the second orchestrator
   thread (worktree `GTAIV-Reborn-lane-d`, branch `stage1/T-049` from `origin/claude/ecstatic-keller-qu3sto` rebased on main).

## Owner context

- PC: Ryzen 3 2300X 4C/4T, one 8 GB DDR4-3000 stick (single channel; ~0.3 GB free during play), RX 570 4 GB, game on
  SSD (`C:\Games\...`), pagefile 24 GB on the HDD, SSD 16 GB free. **Owner cannot buy hardware.**
- 60 fps is not reachable on this PC (game alone ~40-45 fps normal, ~27 worst case; the mod adds ~2 ms). Plan: remove
  the mod's spikes and cost (T-056), steady frame cap, owner-side Windows fixes (pagefile to SSD, close background apps,
  Steam overlay off, Defender exclusion; only the owner applies these, after all threads are idle since it needs a restart).
  Profiling and patching GTA IV's own hot spots is Phase 3.
- The owner prefers short, plain answers and ready-to-paste prompts; does not want to be asked questions by lane threads.
