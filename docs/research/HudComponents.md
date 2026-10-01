# Vanilla HUD components: what can be hidden on its own (T-049, rule 4)

Question (T-049 scope): which vanilla HUD elements can be hidden individually without hiding the radar, so that Liberty draws only
what it replaces and never a duplicate? Status: **mechanism built, answer pending the PC run of `T049-hud-components`**. This note
records what is known, what is not, and exactly how the open part gets answered. Confidence tags as in [README](README.md).

## What is known

- **[VERIFIED offline + owner report 2026-09-24] (T-010; [HudAndCrosshair.md](HudAndCrosshair.md), [MEMORY.md](../game-api/MEMORY.md))** The game registers
  hud.dat components through one routine, `push [alpha]; push [colour]; push imm8; push &size; push &position; push imm8 type;
  push "<NAME>"; call register`. For the four reticle components (`HUD_WEAPON_CROSSHAIR`, `HUD_WEAPON_HEALTH_TARGET`,
  `HUD_WEAPON_ARMOUR_TARGET`, `HUD_WEAPON_DOT`) the HUD code copies those globals into the live component every frame, so writing alpha 0 and a
  near-zero size hides the component and restoring the saved values brings it back. Size is at position + 8 and alpha at size + 8.
- **[SOURCE] (reuse audit F9, `common/data/hud.dat` of the owner's install)** hud.dat also names `HUD_AMMO`, `HUD_WEAPON_ICON`,
  `HUD_WANTED_BACK`, `HUD_WANTED_FRONT`, `HUD_CASH`, `HUD_RADAR`, `HUD_AREA_NAME`, `HUD_STREET_NAME`, `HUD_VEHICLE_NAME`, `HUD_HELP_MESSAGE` and more. The
  player's health and armour are **not** in that list: the vanilla health and armour arcs are drawn around the radar, by code not yet located.
- **[SOURCE] (natives)** `DISPLAY_HUD` and `DISPLAY_RADAR` are separate natives, but the project has only used them together
  (`IUi.SetHudVisible`); what `DISPLAY_HUD(false)` alone removes (and whether it takes mission text and help messages with it) is not recorded.

## What the engine does about it (built)

1. **Generic table** (`GameAddresses.ResolveHudTable`, check: `tools/verify` section "Stage 1 HUD"). Every call to the register routine is a component; the
   call sites are found with `CodeScanner.FindCallsTo`, the argument shape proven for the reticle is parsed, and the name is read from the image.
   A component whose globals do not have the proven layout (size = position + 8, alpha = size + 8) is listed but never written to. A call of another shape is **not
   guessed at**: it is kept in `GameAddresses.HudUnparsed` with the 45 bytes before it, and logged by `hudctl table` (`hud_unparsed ...`), so the next
   resolver revision is written from evidence. Verified offline on a synthetic image (reticle components, extras, a bad name, a duplicate, another shape, a broken
   registration). Not yet run against the real GTAIV.exe: `AddressChecks` prints every component it finds when `tools/verify.ps1 -GameDirectory` runs on the PC.
2. **Hiding** reuses `HudReticle.Hide/Restore` (alpha 0 and a tiny size each frame, saved values restored on stop, config change, `"enabled": false` or an error).
3. **Policy** (`HudPlan`): Liberty draws an element only when every vanilla component listed for it in `config/hud.json` is in the table and was hidden. Otherwise
   the vanilla element stays and nothing is drawn for it. `drawWithoutHidingVanilla` overrides this per element (a duplicate; off by default).
   Shipped defaults: weapon = `HUD_WEAPON_ICON` + `HUD_AMMO`, wanted = `HUD_WANTED_BACK` + `HUD_WANTED_FRONT` (names from the owner's hud.dat), health and armour
   = **no component** (their vanilla counterpart is the radar's arcs), so the compact health and armour bars are **not drawn by default** until the component is known.

## What is not known (to be answered by the PC run)

| # | Question | How it is answered | Answer |
|---|---|---|---|
| 1 | Do `HUD_WEAPON_ICON`, `HUD_AMMO`, `HUD_WANTED_BACK`, `HUD_WANTED_FRONT` parse with the proven shape and disappear when hidden? | scenario `hud-components`: `hud_component` lines, screenshots `hud_hidden_weapon_ammo`, `hud_hidden_wanted`; `hudctl check` in `stage1-hud` | pending |
| 2 | Is there a component for the radar's health and armour arcs, and does it hide without the radar disc? | screenshots `hud_hidden_radar`, `hud_hidden_health`, `hud_hidden_armour` | pending |
| 3 | Does the game re-read the globals every frame for these components (as it does for the reticle)? | the hidden element must stay hidden for several seconds in the screenshots; `hud_probe_hide ... hidden=True` is logged | pending |
| 4 | Which hud.dat registrations have another argument shape (`hud_unparsed`)? | `hudctl table` on the PC | pending |
| 5 | What does `DISPLAY_HUD(false)` with `DISPLAY_RADAR(true)` remove (an alternative to question 2 if no arcs component exists)? | not built: it would also remove help and mission text and needs its own spike | not asked yet |

Reading the result: put the confirmed names into `vanillaComponents` of `health` and `armour` in `config/hud.json` (hot-reloaded), set the answers above, and
mark the tags [VERIFIED]. If question 2 has no answer, the bars wait for the radar redraw (R5, [T-054](../tasks/T-054-r5-custom-radar.md)); the weapon, ammo and
wanted elements do not depend on it.

## Decisions taken without the answer

- No auto-hiding by name pattern: a component is hidden only when it is listed in the config, so a wrong guess can never remove an element nobody asked to replace.
- Mission and tutorial text (`HUD_HELP_MESSAGE` and similar) is never in the default lists; the prompt restyle draws only the help box that Liberty modules open themselves (`IUi.ShowHelp`).
