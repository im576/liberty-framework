# T-042 — Stage 1 gunplay tuning and shoulder swap

Status: **READY** · Lane A · Depends on: T-041 · Design: STAGE1 section 7 Slice A "Gunplay tuning", 10 Pillar 3

## Goal

Heavier, grounded combat for every Stage 1 weapon through the existing Gunplay systems
(`src/LibertyFramework/Gunplay`: `Recoil/`, `Spread/`, `Aim/ShoulderSwap.cs`, `config/gunplay.json`). Tuning is data;
new code only where the model lacks a needed input.

## Scope

- Per-weapon recoil, first-shot accuracy, burst control and recovery, sustained-fire climb (automatic weapons must not
  be laser beams; single shots and controlled bursts are useful), stance and movement influence, caliber differences.
- **Shoulder swap:** finish the existing implementation (T-015 card, `Aim/ShoulderSwap.cs`); do not recreate it. Must
  work on foot, in cover and near walls with no camera clipping; camera/FOV safety preserved.
- **Measurement scenario** `stage1-gunplay-range`: at the test range, per weapon class, log bullet events for a first
  aimed shot at 25 m, a 3-round burst plus pause, and a 30-round sustained burst; compute cone, recovery and climb.
- A shoulder-swap camera scenario at fixed wall spots.

## Acceptance (STAGE1 Pillar 3, proposals until owner confirms)

- First aimed shot within the configured cone per class (proposal ≤ 0.5° pistols/SMGs); burst recovery within the
  configured time; AK 30-round climb within its configured range (proposal 6-12°), spread growing every shot.
- Shoulder swap: all states, 0 wall clips at the test spots.
- Budget: gunplay + arsenal + holsters ≤ 1.5 ms average combined; no spike > 5 ms outside menus.
- Owner feel sign-off per weapon class (manual check in the queue).

## Human test steps

Fill in when done: which weapons to try, where, and what to feel for.
