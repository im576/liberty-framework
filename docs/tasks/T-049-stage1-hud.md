# T-049 — Basic Liberty HUD

Status: **NEEDS-PLAYTEST** · Lane D · Depends on: T-040 · Design: STAGE1 section 7 Slice A "Basic Liberty HUD", 10 Pillar 1;
radar stays vanilla until T-054 (R5); final styling in Slice C

## Goal

Contextual health, armour, ammunition, wanted level and interaction prompts drawn by Liberty.Ui in the Liberty
Vanilla+ language, shown when relevant instead of permanently covering the screen.

## Scope

- **Find out first** (rule 4) which vanilla HUD elements can be hidden individually without hiding the radar:
  existing hud.dat global work (ADR-0004, `docs/game-api/MEMORY.md`), `DISPLAY_HUD`/`DISPLAY_RADAR`-type natives in the
  registry. Hide only what Liberty replaces; if an element cannot be hidden alone, keep the vanilla one and do not
  draw a duplicate. Record the finding in `docs/research/`.
- Liberty elements: health and armour (show on change, damage, low health, combat), ammo clip/reserve (while armed or
  reloading), wanted level (while wanted), interaction prompts (reuse the IV-style help box from the Arsenal).
  Fade in/out timings in config.
- Owner-directed placement (ART-007 r2): current-weapon silhouette and ammo, with compact health and armour indicators
  directly below, are anchored at the **top right**. Keep the vanilla radar at the **bottom left**. Preserve the
  understated GTA IV-era visual language with original artwork; menu panels must not overlap either HUD anchor.
- Coexists with the reticle (T-043) and weapon wheel (T-045); one shared palette/typography definition in config that
  Slice C will finalise.
- Controller and keyboard/mouse prompts show the right glyphs/keys.

## Acceptance (STAGE1 Pillar 1)

- Every element appears when its condition holds and fades when it does not (scenario `stage1-hud` walks through
  damage, low health, armed, reload, wanted, prompt; screenshots).
- UI draw ≤ 0.5 ms average (all Liberty UI combined); smallest text ≥ 14 px at 720p virtual.
- Switching the HUD off in config restores the complete vanilla HUD.
- Screenshots verify the compact weapon/ammo/health/armour group is at the **top right**, the radar remains
  **bottom left**, and the weapon wheel/trunk panel does not cover either group. No lower-corner HUD relocation.

## Progress (2026-10-01, Claude, lane D, cloud session)

Built and verified offline in a cloud session (no Windows, no game): **nothing below has run in the game.** The in-game checks are queued
(`T049-hud-components`, `T049-stage1-hud`, `T049-hud-look`); until `verify-local.ps1` has run them this card stays NEEDS-PLAYTEST with
those checks unproven.

**What was built**
- **Module `hud`** (`src/LibertyFramework/Hud`, `config/hud.json`, documented in CONFIG_SCHEMA): the engine tick gathers state into an immutable
  frame and `OnDraw(ICanvas)` draws it (no game calls while drawing). The group is anchored **top right** (28/24 units from the corner) in the
  order of the ART-007 r2 board: weapon silhouette, `17 / 383` ammo line right-aligned under it, thin sage-green health bar, pale blue-grey armour
  bar, wanted stars. Slots never move while elements fade. The vanilla radar stays where it is (bottom left); nothing is drawn in a lower corner.
- **Contextual** (`HudPresence`, all times in config): the weapon group appears on weapon change, shot, reload and while aiming (left trigger or right
  mouse), holds 4 s and fades; health shows on any change, damage or combat (6 s), and stays up and pulses at or below 30%; armour shows with health when
  there is some; wanted stars show while wanted and fade 1 s after; the help box (prompt) fades in and out. Hidden in cutscenes, pause, fades and when dead.
- **Prompts** (`UiService.CurrentHelp`, `HelpDrawnByHud`): `ShowHelp` texts may carry `{interact}`, `{accept}`, `{cancel}`... and show `X` or `E` according to the device used
  last (`HudInputDevice`: pad buttons/sticks/triggers against keyboard keys, with a switch delay). The Arsenal's trunk and stash prompts now use `{interact}`
  (they said "X / E" before). Without the HUD module the old box draws both names ("X / E").
- **Hiding only what Liberty replaces** (`GameAddresses.ResolveHudTable`, `HudReticle`, `HudPlan`): the hud.dat register routine's call sites give the whole
  component table; each element lists the vanilla components it replaces in `hud.json`; Liberty draws an element only if all of them are in the table and
  were hidden, otherwise the vanilla element stays and **no duplicate is drawn**. Defaults: weapon = `HUD_WEAPON_ICON` + `HUD_AMMO`, wanted = `HUD_WANTED_BACK` + `HUD_WANTED_FRONT`.
  `"enabled": false` (or `hideVanilla: "none"`) restores the complete vanilla HUD. The reticle's four components stay gunplay's.
- **Research recorded** in [HudComponents.md](../research/HudComponents.md); test and probe commands `lf hudctl ...` (CONFIG_SCHEMA).

**Evidence**
- RAN-PASS offline (cloud): `tools/cloud/test-all.sh` all 12 steps PASS, including the offline verifier `RESULT passed=419 failed=0 notrun=8` (was 347; 72 new
  checks in section "Stage 1 HUD": hud.json equals the code defaults, older/partial files, bad values refused (text under 14 px, palette, fade, names, glyph tokens),
  fade-in/hold/fade-out timing at 60 fps, top-right anchoring and bounds at four aspect ratios, no overlap with the weapon wheel, the default list menu, the prompt box,
  the lower corners or a right-centre panel as on the board, the vanilla-hiding plan in eight cases, glyph/ammo/low-clip/pulse text, the input-device rule, the Arsenal's
  prompt tokens exist in config, and the generic hud.dat table resolver against a synthetic executable image: extra components, a flagged layout, a bad name, a duplicate,
  another argument shape and a broken registration).
- NOT RUN (needs Windows and the game): everything in game. Queued: `T049-hud-components` (research: the component table and what each hide removes),
  `T049-stage1-hud` (the walk-through: change, shot, reload, damage, low health, armour, wanted, prompts on both devices, draw cost, plan before/after), `T049-hud-look` (owner).
- NEEDS OWNER: look, size and timing of every element; both input devices.

**Open questions (conservative choice made, recorded for the owner)**
1. **Health and armour bars are not drawn by default.** The vanilla health/armour arcs are part of the radar's drawing and no separable hud.dat component is known
   (reuse audit F9). The card says not to duplicate what cannot be hidden alone, so `health`/`armour` list no component and stay vanilla. `T049-hud-components`
   finds out whether a component carries the arcs; put its name in `vanillaComponents` (hot-reloaded) and the bars appear. If none does, the bars wait for the radar redraw
   (R5 / T-054) or the owner accepts a duplicate (`drawWithoutHidingVanilla: true`). The scenario draws them with `hudctl layout-test on`, so the layout is reviewable either way.
2. The names `HUD_WEAPON_ICON`, `HUD_AMMO`, `HUD_WANTED_BACK`, `HUD_WANTED_FRONT` come from the owner's hud.dat (reuse audit), but whether their registrations have the parsed
   shape and whether the game re-reads their globals every frame (as it does for the reticle) is not proven. If not, those two elements stay vanilla (log `hud_element_plan ... vanilla_kept`) and the card says what to change.
3. `GET_AMMO_IN_CHAR_WEAPON` is assumed to include the clip (reserve = total - clip, `weapon.totalIncludesClip`); `stage1-hud` reads `17 / 383` for 400 rounds to settle it.
4. Weapon icons are drawn at 2:1 (96 x 48) like the wheel's; the PNGs' real aspect is not recorded.
5. `DISPLAY_HUD(false)` alone was not tried as a coarser fallback: it risks removing mission and help text (HudComponents question 5).
6. No art was needed: the star is drawn in code, everything else is rectangles, text and the existing weapon icons. No art request filed (ART-007 r2 is the reference).

## Human test steps

## Local continuation (2026-09-30, Codex)

Continuation: `codex/T-049-hud-continuation`, isolated checkout `C:/Users/IM576/GTAIV-Reborn-lane-d`, based on `origin/stage1/T-049` at `e2f8181`.
Current main, including `db08832` and `45561b4`, is already an ancestor. No B/C/research worktree was edited.
The first run `20260930-194804-e2f8181` passed Windows build/native tests and content tests, then was stopped **before installation** because the recovered
config still enabled population thinning. The continuation disables the atmosphere density governor per the owner's decision; T-040 evidence is preserved.
The component probe now suspends replacement HUD drawing to capture a real vanilla baseline, reapplies probe hides every tick, and restores them between groups.
The HUD scenario gates combined `draw.ui <=0.5 ms`, tests actual JSON config-off through normal polling, restores original bytes, and uses the existing
T-044 pistol cap (150 total: expected `17 / 133`) rather than requesting 400 rounds that the loadout caps. Layout-test remains diagnostic only.
Offline `-NoGame`: `passed=453 failed=0 notrun=7`; tooling tests: `passed=212 failed=0`; art lifecycle 18/18 and 12 requests valid. Local gameplay results pending.

## Human test steps (continued)

1. Install the build from this run (`tools/verify-local.ps1`, or `tools/install-phase2.ps1`), load a free-roam save and go to the test range (DevTools > TELEPORT).
2. Unarmed and idle: the top-right corner shows nothing of Liberty's HUD. The radar is at the bottom left.
3. Give yourself a Glock 17 (DevTools > WEAPONS, Stage 1 entry). Its silhouette and `17 / 383` (clip / reserve) appear at the top right, stay about four seconds and fade. The vanilla weapon icon and ammo
   must be gone (if they are still there, the log says `hud_element_plan element=weapon mode=vanilla_kept` and why). Aim (LT / right mouse), shoot, reload: the group returns each time; with the clip at 4 or fewer the
   clip number turns orange.
4. Let a pedestrian hit you or drop from a low height: a thin green health bar appears under the ammo line and fades. This needs the radar-arc question (open question 1) to be answered in `config/hud.json`;
   until then the vanilla arcs around the radar are what you see. To see the Liberty bars anyway: console `lf hudctl layout-test on` (then `off`).
5. Get a wanted level: six stars (filled for the level) appear at the top right and fade a second after it ends; the vanilla stars are gone.
6. Walk behind a parked car: the help box (top left) says `Press X to use the trunk.` on a controller and `Press E ...` on keyboard/mouse, following whichever you touched last.
7. Open the weapon wheel and the trunk: nothing may cover the top-right group.
8. Set `"enabled": false` in `scripts\LibertyFramework\config\hud.json`: within a second the complete vanilla HUD is back and nothing of Liberty's is drawn. Set it back to `true`.
9. Tell me per element whether it is too big, too small, too faint, too busy or too slow/fast to fade; the numbers are in `hud.json` (`layout`, `palette`, each element's `holdSeconds` and fade times) and hot-reload.
