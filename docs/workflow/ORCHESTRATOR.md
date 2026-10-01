# Orchestrator handoff (Stage 1)

For the agent that coordinates Stage 1 for the owner: it plans, reviews the lane threads, answers the owner,
files task cards and art requests, and keeps docs and decisions current. It does not build lane features itself.
Current review: 2026-10-01. The merge queue below overrides the historical snapshots further down.
Evidence and limits: [integration review](../reports/2026-10-01-orchestrator-integration.md).

## Read first

`AGENTS.md`, `docs/PROJECT_STATE.md`, `docs/design/STAGE1.md` (design, decisions in section 12, budgets in section 10),
`docs/tasks/README.md` (lanes), the T-040 baseline `docs/reports/2026-09-30-stage1-baseline.md`, and this file.

For a lane takeover, read [the Sol handoff guide](../handoffs/sol/README.md) and its binding rules. The prompt command is:

```powershell
powershell -ExecutionPolicy Bypass -File tools\handoff\Get-SolPrompt.ps1 -Lane B
```

Use `-Lane Orchestrator` for this role; use `-NoClipboard` for a read-only printout. The command collects current git,
run and lock state; it does not dispatch or stop a lane. Do not take over a lane while its worker is still active.
For the next Codex workers, use [the current continuation brief](../handoffs/sol/CONTINUATION.md), included by the
generator. Initial milestones are C's crash/cleanup review, D's clean hiding experiment and R's SDK 1.2 validation.
Replacement B/C/D/R workers are now dispatched on GPT-6.1 Sol (C High, others Medium):
[agent roster and initial milestones](../handoffs/sol/AGENT-ROSTER.md). Coding may run in parallel; heavy builds and
game tests are scheduled centrally. Initial assignments are offline; game slots are assigned explicitly.
Preparation: all five generated briefings validated; tooling tests 236/0. Full startup smoke
`20261001-114509-15cd2d9` PASS (fresh heartbeat, zero log errors); restored from `phase2-20261001-115146`.
C's latest `c3e2b16`/`d96aaf2` work needs review: 110420 launch unavailable, and effects cleanup assertions were relaxed.
Keep the original requirements and repair the fixture. R starts in the SDK 1.2 t050 worktree; SDK 1.3 stays separate.
The current lanes finish the combat/inventory milestone. The full first mod also includes the approved visual remaster/
atmosphere and unified UI slices: [completion gaps](../reports/2026-10-01-stage1-completion-gaps.md).

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

## Merge queue (keep current; a Sol orchestrator takes over from here)

| Order | Item | Branch / worktree | State |
|---|---|---|---|
| 1 | Engine stall fix and audio recovery | main `868368b`, `55e70b8`, `ee71eba` | merged; full audio/SDK 1.1 selftest/events PASS; hot reload 24/24 PASS on repeat (first driver fixture failed); broader stalls/crashes remain |
| 2 | T-045/T-046 wheel, trunk, sprite text | main `f823e45` from Lane B `1744416` | merged; full `20261001-092349-e470758` passed assertions/budgets; orchestrator viewed all 12 captures; NEEDS-PLAYTEST |
| 3 | Wheel config polling follow-up | `codex/orchestrator-stage1-integration` (`1f6f501`, tip `9be5315`), `GTAIV-Reborn-orchestrator` | isolated, NOT merged; build/verifier PASS; last full run wheel CRASH before opening, trunk 133/133 PASS; all runs restored; require fresh wheel acceptance |
| 4 | T-050 hit material (SDK 1.2) | main `e547d92`, merged by the concurrent Claude coordinator | additive API reviewed; quick coverage confirms wood, not glass/water/object correctness; preserve merge, full SDK 1.2/material acceptance pending |
| 5 | T-047/T-048 gore, effects | `stage1/T-048`, `GTAIV-Reborn-lane-c-t048` | unmerged; full `20261001-103314-0b4f558` restored with dismember CRASH, firefight/effects-night FAIL; lane continues independently; budgets unchanged |
| 6 | T-049 HUD | `codex/T-049-hud-continuation`, `GTAIV-Reborn-lane-d` | unmerged; full `20260930-205120-4ac4fcc` HUD FAIL; clean DISPLAY_HUD experiment and B/D coexistence unrun |
| 7 | Research rest (T-052/T-054 probes, notes) | `research/stage1` | still separate; SDK 1.3 radar ownership/probes need full validation and renderer coexistence review |
| then | T-056 performance pass, owner playtest | main | after 1-7 |

Active workers and their worktrees are preserved. Do not stop their verifiers or overwrite their source. Integration runs
use `verify-local -AnyBranch -NoPush -Restore -NoManual`, the same machine-wide lock and a clean committed build.
B's controller, real save/load, safehouse/gunsmith and HUD coexistence still need checking; only the owner marks DONE.
Host log-history fixes and isolated lock-test metadata are merged (`1cf9098`, `0bdf1ea`, `632642e`): tooling 236/0.
Current main (including the peer's SDK 1.2 merge): build PASS, offline verifier 433/0/5 not-run. These are not full-mod
runtime acceptance. Own verifier runs are finished; the last restored backup was `phase2-20261001-110404`.
Recent C/R crash receipts name `GTAIV.EFLC.FusionFix.asi` at relative offset `0xA24E0`; this identifies the faulting module,
not the cause. Dumps and hashes are preserved in the coordination workspace's `research/integration-review-2026-10-01`.

## Historical snapshots (preserved; use the current queue above)

## Review of the Sol/Claude work (2026-10-01 06:40 Pacific)

- main `b74ddc4` (Sol coordinator corrections + Sol kit) checked: tool tests 224/0, offline verifier 389/0/5, pushed.
  Sol fixed the game cap to count only lock-held time, bounded scenario timeouts by the remaining cap, made restoration
  run in `finally`, recorded ASI inventories, and set density OFF on main. All lanes merged it; nothing is uncommitted.
- Evidence caveat: research installed ColAccel 07:11-08:03 UTC while B's and C's full runs ran; their functional results
  stand, their timing numbers need a clean rerun. ColAccel is removed (only its backup receipt remains).
- Shared risks for the performance pass (T-056), across lanes:
  1. `ConfigService.Poll` (and module config polling) stats files on the game thread every second; an 11 s stall there
     ended B's wheel run with a crash. Move the stamp checks off the game thread.
  2. Repeated GTAIV.exe crashes at fault offsets 0x7f471a46 / 0x7f4c16ad in B, C and R runs (also before ColAccel).
     Cause unknown; capture the faulting module (minidump/WER) before guessing.
- Owner decisions now due: C's 4 ms per-frame peak (a thrown limb needs `CreatePed`, about 21 ms); B's wheel bindings
  (Back/Tab), trunk sizes and ammo caps (proposals in the B live handoff); D's HUD policy once the DISPLAY_HUD experiment runs.

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
2. VRAM ceiling: the GPU is an RX 570 **4 GB** (driver and registry). Proposed: hard ceiling +300 MB. Not answered;
   STAGE1 retains the original +350 MB criterion, explicitly pending the owner's decision on the lower proposal.
3. RESOLVED 2026-09-30: the main monitor runs above 240 Hz (ASUS VG279QM, 280 Hz), so a steady 40 fps frame cap is possible (T-056). Found 2026-10-01: Windows lists an ASUS VG279QM
   and a VG248 (both 144 Hz or faster), so the 40 fps cap is possible; the owner still has to confirm which one is the main screen.
   Testing speed (45561b4, 2026-10-01): package 411 s cold / 4 s cached, build outside the lock, fail-fast on frozen or
   unstartable games. Lanes must merge main to get it; tell lanes to batch checks in one verify-local run and not poll.
4. T-044 open questions (melee/thrown rule, ammo caps, unassigned stash), T-040 and T-043 test steps (owner playtests).
5. T-049: if the in-game probe finds no hideable component for the radar's health/armour arcs, either show Liberty's
   top-right bars as well as the vanilla arcs (`drawWithoutHidingVanilla: true`), or keep the vanilla arcs until the
   radar redraw (T-054). Recommended: wait for the probe result.
6. Lane D is cloud session https://claude.ai/code/session_01VkfXFwcWXicsfUSssWNvfn (ListAgents "Liberty Vanilla+ Stage 1 Lane D"); it can't message back or run the game. It works on origin/codex/T-049-hud-continuation (Codex WIP committed as 168812a) and asks for game runs in docs/handoffs/Lane-D-next-run.md; the orchestrator runs them locally and pushes the evidence. Earlier: owner approved a local thread to run its in-game checks (2026-10-01); prompt given in the second orchestrator
   thread (worktree `GTAIV-Reborn-lane-d`, branch `stage1/T-049` from `origin/claude/ecstatic-keller-qu3sto` rebased on main).

## Owner context

- PC: Ryzen 3 2300X 4C/4T, one 8 GB DDR4-3000 stick (single channel; ~0.3 GB free during play), RX 570 4 GB, game on
  SSD (`C:\Games\...`), pagefile 24 GB on the HDD, SSD 16 GB free. **Owner cannot buy hardware.**
- 60 fps is not reachable on this PC (game alone ~40-45 fps normal, ~27 worst case; the mod adds ~2 ms). Plan: remove
  the mod's spikes and cost (T-056), steady frame cap, owner-side Windows fixes (pagefile to SSD, close background apps,
  Steam overlay off, Defender exclusion; only the owner applies these, after all threads are idle since it needs a restart).
  Profiling and patching GTA IV's own hot spots is Phase 3.
- The owner prefers short, plain answers and ready-to-paste prompts; does not want to be asked questions by lane threads.
