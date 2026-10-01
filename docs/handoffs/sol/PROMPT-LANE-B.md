# Sol prompt — Lane B (T-045 weapon wheel, T-046 trunk UI)

ROLE
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
Get T-045 and T-046 to full-run acceptance: every queued check for both tasks passes in a full (non-quick) verify-local
run, inside the UI frame budget, with screenshots you have looked at. Then leave both cards NEEDS-PLAYTEST.

KNOWN STATE (verify against the live state; it may have moved on)
- Root finding: ScriptHookDotNet `Graphics.DrawText` stalls frames (0.4-0.9 s with several strings). The shared canvas now
  draws text as cached GDI+ sprites (`engine.json uiTextRenderer`: `sprite` | `shdn`). `T045-ui-text` passed.
- Open review items from the orchestrator, all still required:
  1. The text-sprite cache evicts first-in-first-out; make it least-recently-used (HUD ammo strings would evict menu labels).
  2. Cap new text textures created per frame (number in config) and pre-render a menu's labels when it opens; draw
     changing numbers from per-character sprites created once.
  3. Count text textures in the VRAM budget (4 GB card): log count and estimated bytes; release them on module stop/hot reload.
  4. Root cause with evidence only: compare one font vs several sizes, a font without Effect, one string drawn N times vs N
     different strings. Keep `sprite` the default only if it still wins.
- Last runs: `T045-weapon-wheel` CRASHED (full run 20260930-213920) and `T046-trunk-ui` FAILED; a later quick run exists.
  Find the real cause of the crash from the run log and the LibertyFramework log before changing code.
- Lane D's HUD also draws text on the shared canvas: keep it compatible.

WORKFLOW
1. Do RULES.md section 2 (confirm the previous agent stopped; review everything; write Lane-B-live.md).
2. Fix the wheel crash first. Then the trunk failures. Then items 1-4.
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
