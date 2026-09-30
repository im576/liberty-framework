# Weapon availability: what a script can control (T-041)

Sources: [NATIVES.md](../game-api/NATIVES.md), the Arsenal and Gunplay code, and the T-040 [reuse audit](../reports/2026-09-29-stage1-reuse-audit.md). Nothing below is inferred from memory of the game's own scripts; "no evidence" means the repository has no native, note or test for it.

| Lever | Status | What exists / what is missing |
|---|---|---|
| Give and remove a weapon on any ped | **Controllable** | `GIVE_WEAPON_TO_CHAR`, `REMOVE_WEAPON_FROM_CHAR`, `SET_CHAR_AMMO` (NATIVES.md); SDK `IWeapons.Give/Remove/Select`. Used by Arsenal, the autopilot and `catalog give`. |
| Weapons of script-created peds (test subjects, future scripted encounters) | **Controllable** | `IPeds.SpawnRandom` then `IWeapons.Give`. |
| Price and unlock rules of a Liberty-run purchase (gunsmith, contacts) | **Controllable (data done, UI not)** | `weapon-catalog.json` `availability` (price, contact, story progress, sources) and `WeaponAvailability.Evaluate`; the Arsenal gunsmith already charges money for finishes and attachments (`gunsmithGoldFinishPrice`, attachment prices). A weapon purchase screen is not built: T-041 delivers the rules and the query, not a shop. |
| What the player carries, and keeping restricted weapons out of the normal loadout | **Controllable** | Arsenal observes the inventory, tracks owned vs mission gains and can move a weapon to storage. Not enforced for restricted tiers: a mission may hand over a sniper, and mission compatibility (STAGE1 Pillar 3) wins; the restricted weapons simply stay vanilla. |
| Player money | **Read/charge available** | ScriptHookDotNet `Player.Money`, used by the Arsenal. |
| Story progress (fraction of missions done) | **No evidence** | No native or memory location is verified. `GET_MISSION_FLAG` is used only as a "mission running" flag. The rules keep `minimumStoryProgress` as data; unknown progress fails a rule that needs progress. |
| Contacts (phone contacts, Little Jacob) | **No evidence** | No note about unlocking or reading contacts. `availability.contact` is data only. |
| Ammu-Nation stock and prices | **No evidence** | The shop runs in the game's own scripts; no native in NATIVES.md reads or writes it. |
| Weapon pickups in the world | **No evidence** | No pickup native is listed in NATIVES.md and nothing in `src/` creates one. |
| Ambient NPC loadouts (what pedestrians and gangs carry) | **No evidence** | Set by the game's ped/population data, not by a script call in this repository. A script can replace a ped's weapon after it spawns, but no such module exists. |
| Which weapon a mission hands over | **No control** | Mission scripts are the game's. Arsenal already tracks them as mission weapons. |

## Open questions (carried on the T-041 card)

1. Where is story progress? Candidates to test with a spike: a game stat, the mission-passed counter, a script global.
2. Can a contact be unlocked or read (phone contacts list)?
3. Can Ammu-Nation stock be limited (native, data file, or hook)? Without it the game's own shops still sell every vanilla weapon, so Stage 1 tiers rule only Liberty's own offers.
4. Can `CREATE_PICKUP` be used safely for tiered world pickups (spike needed; native not listed)?
5. Ped loadouts: does the population data (`pedgrp`/`popcycle` files under FusionFix's overload folder) give per-area weapons, or does a script have to re-arm peds after spawn?