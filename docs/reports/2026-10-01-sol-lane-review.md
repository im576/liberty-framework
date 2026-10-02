# Independent review of the resumed Sol lanes

Reviewed against main `2fac4cd`, with lane commits pinned below. This is source/evidence review, not full-mod
acceptance. Gameplay features remain first; remaster is deferred. Original budgets and density OFF are preserved.

## Latest full B follow-up

Reviewed source `b3db1bc`, tested full `20261001-202311-2297a17`; production build zero errors, NoGame441/0/5,
tooling255/0. SDK UI21/0 and wheel99/0 with zero scenario errors. Parent reran the strict real-log latency parser:
16 openings, 16 draws, max_frames=0, first-draw timings 0..62 ms. Wheel UI budget passed. All three SDK and six wheel
captures independently viewed: coherent selection/details/empty slot and readable rendering. This closes the observed
wheel first-draw regression in the candidate; physical controller/owner feel remain unproven.

Trunk41 steps/1 failed/zero log errors: budget p95 rose 40.7 -> 46.4 ms (+14%), above its unchanged gate; p99 rose
68.0 -> 99.7 ms. Draw average0.345 ms was below0.5; scheduler average0.362 ms and holsters peak19 ms identify measured
costs, not a demonstrated cause. Only initial trunk capture exists, independently viewed with first carried selection,
empty right panel and capacity0/8 readable. Fail-fast prevented later transfer/capacity/close flows; do not count them
as passed from this run or discard the failure because an older run passed.

Finished03:31:10.263Z/restored `phase2-20261001-202329`; installed metadata03:31:10.031Z and parent process check agree.
Shared UiService/Radial/Storage patch remains unmerged pending scoped diagnosis/regression. No unchanged retry assigned.
D gets the next bounded held-baseline slot; B/C/R continue offline. R's independent full review is linked in ORCHESTRATOR.

## Findings requiring follow-up

1. **B: wheel responsiveness still fails a written criterion.** Full `20261001-122159-4330603` records a later opening
   at frame 3334 followed by first draw `frames=2 ms=359`. The old scenario checked only the initial opening, so its
   PASS did not establish the one-frame target for later openings. `bcb8989` adds strict pairing for every opening
   group and correctly rejects this preserved log. It does not fix responsiveness. Source review confirms that
   `UiService.OpenRadial` publishes a view before its first snapshot, while `Ui.Update` precedes file-command pumping.
   This leaves command-created menus without a drawable snapshot until another tick; the contribution to the
   measured hitch needs fresh runtime evidence. B is assigned a bounded snapshot-publication fix and regression,
   preserving owner-thread callbacks, selection, input and ledger cleanup. No frame-counter normalization.

2. **D: incomplete visible baseline prevents weapon/ammo hiding acceptance.** Full `20261001-155808-1f6c289` passes
   95 steps with zero log errors. The orchestrator independently opened all nine captures. Baseline cash/stars/radar
   are visible; cash/stars disappear under the native pair, radar remains, and tested release paths restore those
   elements. Weapon/ammo are absent in the baseline. Numeric clip/total readback does not establish their visibility.
   Real native help/subtitle/mission/location text and domain unload remain unrun. Retain the vanilla guard; do not
   adopt the native pair as the shipped policy. D is assigned baseline/held-weapon fixture investigation offline.

3. **Host verification prerequisite/order needs correction before a clean full receipt.** D's LOOP-verify initially
   failed 1119/1/1 because generated WeaponInfo.xml was absent. Package Stage later supplied it; a separate file-only
   verifier recheck passed 1134/0. Both receipts are preserved. Package Build invalidates staged phase2, and the
   pre-install verifier can run before Stage. The existing phase2 fallback does not solve an absent generated input.
   Arrange the generated-input verification after its prerequisite, rather than hiding the exception or counting an
   omitted check as passed. This failure is not evidence of a HUD gameplay regression.

## Lane dispositions and evidence

| Lane / reviewed tip | What review supports | Remaining gate / disposition |
|---|---|---|
| B `2544b12` (`bcb8989` source) | Scoped atomic config fixture edits; exact bytes restored first in runner finally; original module running/config-enabled state checked; stopped-owner and duplicate-callback tests; strict later-opening latency coverage. Focused receipts 17/0 in PS7 and PS5.1, existing runner regressions 30/0. | Live rejection/recovery/restart fixtures unrun; expected rejection ERRORs remain visible. Timing fix pending. New fixture changes remain unmerged; earlier reviewed watcher production fix is already on main. |
| C `a167da5`, plus prior `ebea28c` | Effect-only admission gate preserves config flags and resets at lifecycle exits. Failed FX stop retains ownership and counts until successful retry. Blood loops acquire ledger ownership; weapon cleanup continues past failures. Shared ledger changes apply retention only to FX. Focused fault tests 13/0; gate/setup checks give 36/0 total. Original zero cleanup requirement restored. | Shared engine FX service/ledger changes require production compile and integration/runtime evidence. Setup fixtures explicitly rely on verifier ending/restoring the owned session, not an in-session snapshot. Old dismember crashes and 45.373 ms peak against the unchanged 4 ms limit remain. Unmerged. |
| D `15d91db` (tested `1f6c289`) | Explicit bounded diagnostic, owner-ledger restoration and public HUD-off precedence; guarded default unchanged. Full captures substantiate cash/wanted/radar observations above; config hash/wallet restored. Build zero errors. | Weapon/ammo baseline, real native text, unload and B/D coexistence incomplete. Original verifier failure retained. Unmerged; no batch B automatically authorized. |
| R `1316142` (`ae7d8a5`, `576b757` source/tests) | Target-space endpoints, exact target/hit handles, stamped before/after captures and bounded prop sweeps; fixed water-ray origins. Four additive SDK checks preserve existing assertions. Decoder/API unchanged; no demonstrated decoder regression found. | Full SDK/material runtime coverage, effective materials-table identity and direct production conversion/status tests remain incomplete. Glass/water names cannot be inferred from a model or WaterHeight. Unmerged. |

The orchestrator reran R's actual decoder tests **9/9**, generator `--check` and 79-check queue validation: PASS.
These are lightweight parser/fixture checks, not production ray/material acceptance. B/C focused compiled tests were
reviewed from their receipts; no competing parent compilation was started while C held the heavy slot.

## Schedule following review

D completed and restored `phase2-20261001-155855`; installed-build.json independently confirms restoration, and no
D game/verifier remained at review. C safely integrated main in `844c6a1` and now owns the single heavy/game slot:
production build, NoGame verifier and tool tests, then only the bounded quick `T047-gore-setup-control` invocation.
Inspect its result/restoration before authorizing the separate active comparison or effects checks. No automatic
dismember retry. B and D continue their scoped fixes offline; R stays offline awaiting the next assigned slot.

No unfinished C/D/R production patch was merged by this review. All four agents retain their original Sol 6.1
settings/worktrees. No task is DONE; owner controls, real save/load, mission compatibility and feel remain required.

Evidence indexes: lane `docs/handoffs/Lane-{B,C,D,R}-live.md`, B's full run above, D's full run above and
`docs/research/MaterialAcceptance.md` in the R checkout. Preserved failures remain part of the acceptance record.
