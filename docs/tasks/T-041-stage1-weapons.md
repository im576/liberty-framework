# T-041 — Stage 1 arsenal and availability

Status: **READY** · Lane A · Depends on: T-040 (audit) · Design: STAGE1 sections 4-5 (spec), 7 Slice A, 10 Pillar 3

## Goal

Define the Stage 1 arsenal as data and make every catalog weapon a Liberty weapon: its own identity, class, tier and
availability, driven by config. Extends `config/weapon-catalog.json`, `config/gunplay.json` (per-weapon profiles),
Arsenal (`src/LibertyFramework/Arsenal`) and the installed Realistic Weapon Overhaul models. AGENTS.md rule 2: weapons
outside the catalog stay vanilla; every system can be switched off.

## Scope

- **Arsenal list** with class and tier (STAGE1 Slice A): common (cheap/service pistols, revolver where it fits, basic
  shotgun, Uzi-class SMG), less common (better pistols/shotguns, MP5-type SMG, better criminal-market guns), rare
  (AK-type rifle, tactical weapons by circumstance). Not normal in Stage 1: high-end snipers, LMGs, P90-type, military
  gear. Map each to a GTA IV weapon slot/ID (vanilla IDs, FusionFix ExtendedLimits IDs if needed, ADR-0002).
- **Per-weapon identity data:** fire rate, damage, accuracy fields (WeaponInfo through the existing override path) plus
  a Liberty gunplay profile per weapon (recoil, spread, recovery) as a starting point for T-042. Extend the gunplay gate
  from test weapons 58/59/60 to "any weapon in the Stage 1 catalog".
- **Availability:** document what a script can control in IV (gun shop stock, pickups, NPC loadouts, prices, mission
  rewards) from `docs/game-api` and research notes; implement the controllable parts from config (tier by story
  progress / money / contact), write the rest as open questions. No XP bars.
- A DevTools/autopilot command to list the catalog and give any catalog weapon (for tests).

**Owner decision (2026-09-30):** keep every catalog weapon, including the P90-type, MG36 and snipers. They are excluded from normal Stage 1 availability but must be obtainable at any time from the DevTools/mod menu (a give entry per weapon). SMGs (Uzi, MP5) are long guns.

## Acceptance

- Every Stage 1 weapon has a catalog entry (class, tier, availability rule, profile, model) and the verifier checks it.
- No sniper/LMG/P90/military weapon is in normal Stage 1 availability.
- Scenario `stage1-arsenal`: gives each catalog weapon, fires it, confirms bullet events and the Liberty profile in the
  log. Vanilla weapons outside the catalog behave as vanilla.

## Human test steps

Fill in when done.
