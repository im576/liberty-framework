# Liberty Tweaks

- Source URL: https://github.com/catsmackaroo/LibertyTweaks
- Game version: 1.0.7.0 / 1.0.8.0 (via IV-SDK .NET). **Not CE.** [SOURCE]
- Dependencies: IV-SDK .NET (1.8 for LT 1.6+), Clonk's Coding Library [SOURCE]
- Source available?: yes, C#
- License: **none** (GitHub API `license: null`) → all rights reserved [SOURCE]
- Latest: 1.7 stable (2024-04), 1.8 pre-release (2024-10); last code commit 2025-05 [SOURCE]
- **Can reuse?: NO.** Study only. Reimplement behavior independently.
- Can reference?: yes — reading to understand techniques is fine.
- Potential value: high as a **feature list + technique map**; low as code (wrong runtime anyway).

Code lives in `LibertyTweaks/Features/<Category>/<Feature>.cs`. Combat folder:
`Recoil.cs, ShoulderSwap.cs, QuickSwitching.cs, DynamicCrosshair.cs, RealisticReloading.cs,
WeaponMagazines.cs, ArmorPenetration.cs, HolsterWeapons.cs, Killcam.cs, ImprovedAIAccuracyAndFirerates.cs,
Sniper Adjustments/…`. Misc: `DynamicFOV.cs`.

## Feature techniques (as read from source on 2026-09-24; summarized, not copied)

### Recoil (`Recoil.cs`) — HIGH priority
- It is **camera shake only**, not an aim-direction kick. [SOURCE]
- Per weapon *group* (small pistol, heavy pistol, SMG, shotgun, rifle/sniper): amplitude + frequency ranges.
- Accumulator: each shot adds a per-group amount (≈0.12–0.55), capped; decays when not shooting.
- Crouch multiplier (default 0.5).
- Each shot fires two shakes: pitch (up/down) and roll (left/right), "constant + fade in/out", 160–250 ms.
- Reads `IS_CHAR_SHOOTING`, `IS_CHAR_DUCKING`, `GET_CURRENT_CHAR_WEAPON`; shakes via IV-SDK camera wrapper.
- No controller vs mouse distinction.
- **Lesson:** shake alone doesn't move where bullets go. We want real aim kick + shake + recovery (see RealRecoil.md).

### Shoulder swap (`ShoulderSwap.cs`) — HIGH priority (but Phase 1 optional)
- Input: controller **LB** (left bumper) while aiming, 500 ms debounce; K/M uses game keys.
- Conditions: aiming, holding a gun, not in pause menu, not in a vehicle.
- Mechanism as read: creates an invisible object (`bm_poolcue` model) attached to the player's root bone
  at a lateral offset, heading synced to the player. It does **not** write camera position directly. [SOURCE]
- Why that works is unclear — likely the aim camera frames around/avoids the attached object. [HYPOTHESIS]
- Zolika's trainer also had shoulder swap (memory-based, 1.0.8.0). A GTAForums thread asks about porting to CE 1.2.0.59
  (couldn't fetch it — 403). → T-015.

### Weapon switching while aiming (`QuickSwitching.cs`)
- Watches Next/Prev weapon inputs while aiming, not in a car, 500 ms debounce.
- Briefly toggles `SET_PLAYER_CONTROL` off/on, then `SET_CURRENT_CHAR_WEAPON`; alt path uses
  `GIVE_DELAYED_WEAPON_TO_CHAR` + `ADD_AMMO_TO_CHAR`. No memory offsets. [SOURCE]
- All natives → should be portable to SHDN on CE. [HYPOTHESIS] → T-016.

### Dynamic crosshair (`DynamicCrosshair.cs`) — reference only
- Toggles the **game menu setting `SETTING_WEAPON_TARGET`** (0/1) via IV-SDK's `IVMenuManager.SetSetting`
  depending on whether the player aims at a ped. [SOURCE]
- **Key insight:** the game has a *targeting mode* menu setting that can be flipped **at runtime**.
  If we can flip it when a gold weapon is equipped, we get per-weapon free aim. On CE/SHDN we need a
  different way to reach that setting (native or memory). → T-008. [HYPOTHESIS]

### Dynamic FOV (`DynamicFOV.cs`) — MEDIUM
- Multiplies final camera FOV each frame; target FOV by context (on-foot, vehicle speed, combat,
  interior, cinematic, killcam); smooth-step lerp, speed ≈0.05 (0.5 for hood cam). Uses IV-SDK `IVCamera.TheFinalCam`
  (memory class — not available in SHDN). [SOURCE]
- On SHDN we'd need a camera FOV API that affects the gameplay camera. Unknown. → later.

## What NOT to take from Liberty Tweaks

Crosshair color-on-target, hitmarkers, kill flashes (handoff says reference only / not wanted),
world events, progression, wardrobe, police — out of scope.
