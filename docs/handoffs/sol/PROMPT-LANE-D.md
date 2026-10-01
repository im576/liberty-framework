# Sol prompt — Lane D (T-049 basic Liberty HUD)

ROLE
You are the Lane D engineer for Liberty Vanilla+ Stage 1, continuing work from a Claude Sonnet agent that ran in the
cloud. You now work locally on the owner's Windows PC and may run the game only through the shared verifier.

WORKTREE
`C:\Users\IM576\GTAIV-Reborn-lane-d`, branch `codex/T-049-hud-continuation`. The cloud agent pushed its work to
`origin/codex/T-049-hud-continuation`: first `git fetch origin` and fast-forward (`git merge --ff-only origin/codex/T-049-hud-continuation`).
If it does not fast-forward, stop and report; do not reset or force anything.

READ FIRST, IN ORDER
1. `C:\Users\IM576\GTAIV-Reborn\docs\handoffs\sol\RULES.md` (binding rules)
2. `AGENTS.md`, `docs/design/STAGE1.md` (HUD section, Pillar 1), `tools/README.md`, `docs/research/HudComponents.md`
3. `docs/tasks/T-049-stage1-hud.md`, `docs/reports/2026-09-30-lane-d-hud-local.md`
4. `docs/handoffs/Codex-Lane-D-2026-09-30.md`, `docs/handoffs/Lane-D-next-run.md` and `docs/handoffs/Lane-D-live.md` (if present)
5. The LIVE STATE block the owner pasted below this prompt.

OBJECTIVE
A HUD that never duplicates vanilla elements: weapon/ammo/health group top right, radar bottom left, vanilla kept for
anything Liberty cannot cleanly replace, full-run evidence, card NEEDS-PLAYTEST.

KNOWN STATE (verify against the live state)
- Per-component hiding through hud.dat globals did NOT work in game: wanted stars and radar stayed visible, weapon/ammo
  appeared twice. A guard now keeps vanilla by default; it has not been game-tested.
- Draw cost was fine (draw.hud 0.306 ms). Ammo sampling once read a stale total (16 / 134 instead of 17 / 133).
- The experiment nobody ran yet: `DISPLAY_HUD(false)` + `DISPLAY_RADAR(true)` held every frame while the Liberty HUD is on
  (both natives are already registered in `docs/game-api/NATIVES.md` and used by the autopilot's `hud off`). Capture what
  disappears and what stays: weapon icon, ammo, wanted, cash, health/armour arcs, radar, help box, subtitles, mission text,
  area/vehicle names, in free roam and in a help/subtitle moment. If it keeps radar plus help/subtitles/mission text, that
  is the clean policy: Liberty draws weapon/ammo/wanted at the top right and config-off releases DISPLAY_HUD. If it
  removes text the player needs, it fails: keep the guard (vanilla HUD) and record why.
- Lane B moved shared-canvas text to cached sprites because SHDN DrawText stalls; keep HUD text compatible.

WORKFLOW
1. Do RULES.md section 2 (fetch; confirm the cloud agent stopped pushing; review everything; write Lane-D-live.md).
2. Run the guarded build's checks; fix ammo sampling; build and run the DISPLAY_HUD experiment as a scenario plus a check.
3. Offline checks after each change: `./tools/build.ps1 -ScriptHookDotNetReference 'C:\Games\Grand Theft Auto IV\GTAIV\ScriptHookDotNet.asi'`,
   `./tools/verify.ps1 -NoGame`, `./tools/tests/Run-Tests.ps1`.
4. Game: `./tools/verify-local.ps1 -GameDirectory 'C:\Games\Grand Theft Auto IV\GTAIV' -AnyBranch -NoPush -Restore -NoManual -Quick -StopOnFailure -Only LOOP-package-install,T049-hud-components,T049-stage1-hud,<new ids>`;
   final acceptance without `-Quick -StopOnFailure`.
5. Commit on the branch and push the branch (`git push origin codex/T-049-hud-continuation`); never push or merge main.
   Update the card, `docs/PROJECT_STATE.md` (one line), Lane-D-live.md.

DEFINITION OF DONE
Full-run evidence that no HUD element is duplicated in the shipped default, the chosen hiding policy is proven by
screenshots (or vanilla is kept with the reason recorded), config-off restores the full vanilla HUD, budgets met,
card NEEDS-PLAYTEST.

REPORT
Short and plain: what passes, what fails and why, what is next, what the owner must decide.
