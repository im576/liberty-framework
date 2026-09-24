# T-021 — Visible weapons on the body (holsters)

Status: **READY** (Codex Agent B, branch `arsenal/feel`). Build from scratch: no code, files or offsets from Holsterable Weapons, Equip Gun or any other mod (technique only).

## Requirements

1. Every carried weapon that is **not in hand** is shown as a prop of its own model on the player: `SidearmPrimary` (handgun) on the right thigh, `SidearmSecondary` (SMG) on the left hip, `LongGun1`/`LongGun2` slung on the back (two distinct positions, no overlap), `Melee` on the belt/back. Gold variants (`lf_gold_*`) show gold.
2. Source of truth: `ArsenalRegistry.CarriedWeapons` (T-020). While it is null (Arsenal not running), derive the carried set yourself from the player's inventory with the same category→slot rule so the feature works and is testable on its own.
3. Model name per weapon: read from the active `WeaponInfo.xml` (`WeaponInfoXml.ModelFor`) so custom/gold weapons work; request/load models before creating objects; never block the tick waiting for streaming.
4. Hide/cleanup: hide the in-hand weapon; hide all in vehicles except bikes (config), during cutscenes/fades, and when `ArsenalRegistry.WeaponsRemoving` fires; delete every prop on death, bust, script unload and error. No orphaned objects after `ReloadScripts` (track handles; clean up on the next start as well).
5. Placement: bone + position + rotation per `BodySlot` with optional per-category/per-model overrides in `config/holsters.json`. Prefer ScriptHookDotNet wrappers (`World.CreateObject`, `Object.AttachToPed`, `GTA.Bone`; sources in `...\work\shdn-source`) over raw natives. A DevTools page (via `DevToolsPages.Register`) nudges the selected slot's offsets live and saves to `holsters.json` (with `.bak`).
6. Performance: at most one prop per body slot; re-attach only when `Revision`, weapon or visibility changes.

## Acceptance (offline)

Build/verify green; tests in `tools/verify/FeelChecks.cs` for slot mapping, hide rules, and `holsters.json` validation; config documented in `CONFIG_SCHEMA.md`; human test steps (per slot: is it visible, placed well, hidden in hand/vehicle/cutscene, gone after death/bust/reload); report in `docs/agent-reports/agent-b.md`.

## Human test steps

(Agent B fills in.)
