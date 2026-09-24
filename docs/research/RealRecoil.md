# Recoil mods

## Real Recoil Enhanced (CE)

- Source URL: https://www.nexusmods.com/gta4/mods/1220 (page blocks automated fetch — human should read Permissions tab)
- Game version: CE 1.2.0.59 — "fixed all the offsets" for CE [SECONDARY]
- Dependencies: ScriptHookDotNet (ships `WeaponRecoil.net.dll` + `WeaponRecoil.ini` into `scripts\`) [SECONDARY]
- Source available?: unknown. It's a .NET assembly (decompilable for study — only if permissions allow).
- License: **unknown** → treat as reference only until the human checks the Nexus permissions tab.
- Last updated: 2026-07-26 [SECONDARY]
- Rewrite of Real Recoil 1.0.1 (2014) by InfamousSabre.

### How it works (from mod description) [SECONDARY]
- Kick strength comes from each weapon's **`force` value in `weaponinfo.xml`**; shotguns get a separate multiplier.
- **Fire-rate normalization:** force is a *per-shot impulse*. A weapon firing every 66 ms climbs 5× faster than
  one firing every 333 ms for the same value → full-auto stays controllable, single shots kick hard.
- Slight camera shake on every shot.
- Accuracy adjustment for blind fire and drive-bys.
- "Fixed offsets" ⇒ it **does touch memory** (probably camera heading/pitch or weapon state). [HYPOTHESIS]

### Decision
- **Don't replace it for vanilla weapons.** It's a good, current, CE-native option for everything that isn't gold.
- **Conflict risk:** if the tester runs it together with ours, both will kick the gold guns. Our system must either
  (a) be tested with Real Recoil removed, or (b) detect it (file exists in `scripts\`) and log a warning.
- **Most important unknown:** *how* it moves the aim camera on CE (answers Q4). Options for T-009, in order:
  1. Human checks Nexus permissions / contacts the author and asks.
  2. If permitted, open `WeaponRecoil.net.dll` in ILSpy **to learn the API/offset used** — do not copy code.
  3. Otherwise find it ourselves (SHDN camera API, natives, then memory spike).

## Real Recoil 1.0.1 (InfamousSabre, 2014)
- Original; classic exes. ScriptHookDotNet script. [SECONDARY]

## Bullet Spread / Recoil Fix (jenksta)
- Old fix for classic exes (https://www.gtagaming.com/bullet-spread-recoil-fix-v1-1-f29635.html). Reference only. [SECONDARY]

## Liberty Tweaks recoil
- Camera shake with accumulator. See [LibertyTweaks.md](LibertyTweaks.md).

## FusionFix
- `RecoilFix` (K/M = controller recoil) and a recoil multiplier patch in `sniper.ixx`. The game has built-in recoil.
  See [FusionFix.md](FusionFix.md).

## Design takeaways for our RecoilSystem

1. **Per-shot impulse + fire-rate normalization** (Real Recoil) is the right core model.
2. **Accumulator with cap + decay** (Liberty Tweaks) models sustained-fire growth.
3. **Real aim displacement** (pitch up, small random yaw) + **recovery toward the pre-burst aim** — modern shooters
   recover partially, not fully, so the player must still pull down on long bursts (handoff: "longer bursts require
   active correction").
4. **Shake is garnish**, applied on top, tiny.
5. Multipliers for crouch / moving / vehicle / blind fire (handoff §13).
