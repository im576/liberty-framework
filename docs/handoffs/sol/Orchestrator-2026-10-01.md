# October 1 coordinator corrections and continuation rules

Owner authorized source/tool fixes and a thorough review while the lanes are paused. No lane agent, game verifier,
game launch, installation, push, or research experiment was started for these corrections. Only local offline builds,
simulated tooling and existing evidence were used. The old September 30 9:30 stop does not govern a new dispatch;
its one-night automation is paused. Automatic Claude transfer remains canceled.

The owner separately resumed Claude around 09:31 UTC, then explicitly paused the sessions again. Claude committed
the coordinator's B/C/R edits and wrote the Sol kit on main (`0e53942`). Background B/C verifiers outlived UI pauses;
the coordinator stopped only identified project workers/owned tests and restored the exact backup chain. New offline
work resumes after cleanup; no coordinator gameplay test was launched. See the coordination corrections report for
final tips, interrupted batches and the restoration receipt. The historical 08:03 receipt below is superseded by cleanup.

## Reviewed source and evidence

Active paths are `C:/Users/IM576/GTAIV-Reborn-lane-b2` (B), `GTAIV-Reborn-lane-c-t048` (C), `GTAIV-Reborn-lane-d` (D),
and `GTAIV-Reborn-research` (R), all under `C:/Users/IM576/`. Preserve the older B/C worktrees. Main was `c38b322`;
the review started at B `ce497ea`, C `b516f4c`, D `168812a`, R `f422d36`. Lane feature changes are not yet merged to main.

Full review, Claude conversation IDs, screenshots and crash records are retained in the coordination workspace:
`C:/Users/IM576/OneDrive/Documents/ChatGPT/GTA4-Reborn/research/`. See `Codex-Orchestrator-Review-2026-10-01.md`
and `review-2026-10-01/`. Local Claude main/B/C/R logs were reviewed; D's original cloud session is
`https://claude.ai/code/session_01VkfXFwcWXicsfUSssWNvfn`. Its clean-HUD experiment has no completed response.
Original Codex chats predate the newer Claude work and must receive these live handoffs before continuation.

## Shared corrections

- Population density governor OFF consistently; T-040 evidence preserved.
- Active game allowance starts after lock acquisition, records queue wait separately, and bounds active scenario timeout.
  Necessary owned-game shutdown/restoration can take additional time.
- `-StopOnFailure` stops the whole batch after FAIL/CRASH/ERROR, including avoiding installation after an offline failure.
- Restoration runs in `finally` before releasing the lock even if summary writing throws. Original failure is preserved.
- Record relative path, SHA-256 and size of installed root/plugins ASIs under lock. The package receipt alone omits them.
- Queue validator recognizes mode-qualified screenshot lines (also corrected independently in C `b2f46c8`).
- Cache verifier compilation by source/compiler inputs; execute all current checks every invocation. Full warm-up stays
  12 seconds; Quick startup settling is 2 seconds followed by the existing readiness protocol.

Evidence: `results-local/offline/tools-final-tests.log` = **224 passed / 0 failed**; uncached and cached verifier runs
each **380 / 0 / 7 notrun**. Local elapsed time 11.67 s versus 1.16 s (`verify-timings.json`); this is offline compilation
reuse, not an in-game performance improvement. Results/build outputs are ignored local evidence.

## Evidence qualification discovered during review

ColAccel was installed at 07:11:45 UTC October 1, before B's `20261001-001218-7d63be6` run and C's
`20261001-003330-b516f4c` run. It remained until rollback at 08:03. The scratch experiment released the game lock
between installation and its verifier runs, allowing other lanes to test with the candidate present. Their mechanics
and captures remain useful, but isolated plugin-free performance attribution is unproven. Do not adopt ColAccel or
declare acceptance from those timings. R's new controlled wrapper owns the lock through baseline/candidate/rollback.

The final R run `20261001-010123-f422d36` crashed after one 23.347-second LOAD_SCENE, during the next destination.
Cause is unknown. Read-only WER review found earlier BEX/StackHash records, but no matching final crash record.
Neither ColAccel nor a specific graphics module is established as the cause. Preserve all failed attempts and rollback.

## Next dispatch, when authorized

Use existing active worktrees and the owner's GPT-6.1 Sol / high preference. Review **all** commits, dirty files and
evidence, including Claude's newer work and these corrections. Do not blindly resume an old Codex turn.

1. B: renderer fix review, cache churn/narrow labels, remaining physical controller/save-load and B/D integration.
2. C: watched-subject retention/cleanup, new per-operation dismember profiling, impact/smoke evidence. Budget remains
   failed (73.178 ms peak); preserve 0.8 ms average / 4 ms maximum criteria. No unsupported tuning or ownership bypass.
3. D: latest ammo work is now built/reviewed offline. Clean HUD hiding/restoration experiment and runtime/coexistence
   remain unrun. Keep vanilla guard until visible clean replacement is demonstrated.
4. R: consolidate Stage 1 material/blood support, SDK/UI visibility ownership and actual confidence. No Phase 3 expansion.

One gameplay lane at a time via the normal shared lock, installed-source identity and exact-backup restoration.
Use focused `-Quick -StopOnFailure -NoPush -Restore -NoManual -AnyBranch -Only <ids>` for development. Quick cannot
prove full acceptance. Final acceptance uses committed source/full mode, normal budgets/trial counts and visual review.
Do not manually set the holder variable, rerun T-040, merge all feature branches unreviewed, or mark DONE for the owner.
