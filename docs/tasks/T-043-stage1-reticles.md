# T-043 — Weapon-specific reticles

Status: **READY** · Lane A · Depends on: T-041 (classes); run after T-042 if both are in flight (shared Gunplay files)
Design: STAGE1 section 7 Slice A "Weapon-specific reticles", 10 Pillar 3 "Reticle truthfulness"; final styling in Slice C

## Goal

Replace the single generic crosshair with reticles that match each weapon's type and handling, and that show the
**real** gunplay state, not a cosmetic animation.

## Starting point (read first)

- `src/LibertyFramework/Gunplay/Crosshair/CrosshairRenderer.cs`: four segments whose gap is the live spread cone
  (`coneDegrees` → pixels via FOV); `Profiles/CrosshairSettings.cs` and `config/gunplay.json` `crosshair`.
- `Gunplay/Spread/SpreadModel.cs`, `ShooterState.cs`: the cone and its inputs (movement, stance, bloom, recovery).
- `Gunplay/GunplayController.cs`: where the crosshair is drawn; draw-thread rules (never call natives or read
  `Game.Resolution` while drawing; gather state on the tick).
- ADR-0004 / `docs/game-api/MEMORY.md`: how the vanilla reticle is hidden (hud.dat globals).

## Scope

- **Reticle styles by class** (config-selectable, per-weapon overrides):
  - pistol: small, precise;
  - SMG: wider, communicates spread and burst control;
  - assault rifle: tighter, structured, clear recoil/spread feedback;
  - shotgun: wide circular / pellet-pattern ring sized to the pellet spread;
  - sniper/precision: minimal or none from the hip; when aimed, a scope-specific UI (keep the game's scope if a
    restyle needs art or research; document the choice);
  - heavy/special: unique where appropriate.
- **Truthful dynamics:** size and spread follow movement, stance, recoil, bloom, sustained fire and recovery from the
  spread model every frame; opening immediately on a shot, easing closed on recovery (existing smoothing rule).
- **Visibility rules** in config: hide or simplify where realism or gameplay calls for it (for example no reticle for
  precision weapons from the hip, reduced reticle when not aiming, hidden in cutscenes/menus/vehicles as configured).
- **Style:** restrained, gritty, readable, not futuristic (Liberty Vanilla+ / GTA IV; STAGE1 section 3). Colours,
  thickness, outline and opacity from config. Readable on bright and dark scenes (outline).
- **Input:** works with controller and keyboard/mouse, free aim and vanilla aim profiles.
- **Config:** `reticles` section: class definitions (style, sizes in pixels at 720p virtual, colours, outline, dot,
  pellet count/ring, visibility rules, smoothing) and per-weapon overrides; validated like the rest of `gunplay.json`;
  documented in `docs/architecture/CONFIG_SCHEMA.md`; hot-reloaded.
- **Debug:** an option to log reticle opening vs spread cone per frame for the truthfulness check.
- Art: if a style needs sprites rather than lines/circles, file art requests (AGENTS.md 5b); line-drawn reticles are
  the default. Concept reference: ART-001.

## Acceptance (STAGE1 Pillar 3)

- Each Stage 1 weapon class shows its configured reticle; per-weapon override works.
- Scenario `stage1-reticles` at the test range: stand, crouch, move, sustained fire, recovery for one weapon per
  class; logged opening within ±5% of the live cone at every sample; screenshots per class for review.
- Reticle drawing ≤ 0.1 ms average.
- All values in config; the old `crosshair` section migrates cleanly (existing installs keep working).

## Human test steps

Fill in when done: per class, what the reticle should look like and how it should move while firing.
