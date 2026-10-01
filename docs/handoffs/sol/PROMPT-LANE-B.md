# Sol prompt — Lane B (T-045 weapon wheel, T-046 trunk UI)

ROLE
Read main's `docs/handoffs/sol/CONTINUATION.md` first; its current milestone/state replaces the historical requests below.
You are the Lane B engineer for Liberty Vanilla+ Stage 1, continuing work from a Claude Sonnet agent. You work alone in
one git worktree on the owner's Windows PC and may run the game only through the shared verifier.

WORKTREE
`C:\Users\IM576\GTAIV-Reborn-lane-b2`, branch `codex/lane-b-validation`. Original branches `stage1/T-045` (94a60c6) and
`stage1/T-046` (7a67900) are history; do not reset or delete them. `C:\Users\IM576\GTAIV-Reborn-lane-b` is an older
diagnostic copy: do not edit it.

READ FIRST, IN ORDER
1. `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md` (binding rules)
2. `AGENTS.md`, `docs/design/STAGE1.md` (weapon wheel and trunk sections, budgets), `tools/README.md`
3. `docs/tasks/T-045-stage1-weapon-wheel.md`, `docs/tasks/T-046-stage1-trunk-ui.md`
4. `docs/handoffs/Codex-Lane-B-2026-09-30.md` and `docs/handoffs/Lane-B-live.md` (if present)
5. The LIVE STATE block the owner pasted below this prompt.

OBJECTIVE
Preserve T-045/T-046's merged full-run acceptance. When assigned, review the isolated config follow-up and validate
only the behavior it changes; leave the cards NEEDS-PLAYTEST for the owner's remaining checks.

KNOWN STATE (current integration record; verify against live state)
- Root finding: ScriptHookDotNet `Graphics.DrawText` stalls frames (0.4-0.9 s with several strings). The shared canvas now
  draws text as cached GDI+ sprites (`engine.json uiTextRenderer`: `sprite` | `shdn`). `T045-ui-text` passed.
- B 1744416 is merged in main f823e45. Full 20261001-092349-e470758 passed wheel/trunk/text assertions and budgets;
  all 12 wheel/trunk captures were reviewed. LRU eviction, per-frame creation limits and texture accounting are present.
  Whole-cache disposal on engine unload remains unproven; do not claim it was measured.
- Config follow-up 1f6f501 is isolated at orchestrator-worktree tip 9be5315. Latest full run crashed before opening the
  wheel; trunk passed. Do not bring that change over until assigned, and do not attribute the crash from its module alone.
- Lane D's HUD also draws text on the shared canvas: keep it compatible.

WORKFLOW
1. Do RULES.md section 2 (confirm the previous agent stopped; review everything; write Lane-B-live.md).
2. Work on the bounded maintenance assignment in CONTINUATION.md; do not redo merged features or broaden its scope.
3. Offline checks after each change: `./tools/build.ps1 -ScriptHookDotNetReference 'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`,
   `./tools/verify.ps1 -NoGame`, `./tools/tests/Run-Tests.ps1`. All must pass.
4. Game iteration: `./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -AnyBranch -NoPush -Restore -NoManual -Quick -StopOnFailure -Only LOOP-package-install,<ids>`.
5. Acceptance: the same command without `-Quick -StopOnFailure`, with `T045-ui-text,T045-ui-path,T045-weapon-wheel,T046-trunk-ui`
   (split into two batches if the 30-minute cap is reached).
6. Update both cards (evidence, exact human test steps), `docs/PROJECT_STATE.md` (one line), Lane-B-live.md; commit.

DEFINITION OF DONE
Full-run PASS (or NEEDS-REVIEW only for screenshots you reviewed and described) for every T-045/T-046 check; the UI budget
gate passes (no frame >= 1 s, averages within budget); offline checks pass; cards NEEDS-PLAYTEST; nothing merged to main.

REPORT
Short and plain: what passes, what fails and why, what is next, what the owner must decide.
