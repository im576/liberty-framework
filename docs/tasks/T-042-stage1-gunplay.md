# T-042 — Stage 1 gunplay tuning and shoulder swap

Status: **READY** (implemented; in-game checks not yet run, see Blocked) · Lane A · Depends on: T-041 · Design: STAGE1 section 7 Slice A "Gunplay tuning", 10 Pillar 3

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

## Progress (2026-09-30, Claude, lane A)

Implemented and offline-verified; the in-game checks `T042-gunplay-range` and `T042-shoulder-swap` have **not run**: the game could not start (see Blocked). Built on `stage1/T-041` (which is not merged for the same reason).

**What was built**
- **Class targets** (`config/gunplay.json` `classTargets[]`, validated): per class (pistol, shotgun, smg, rifle) the first-shot cone (proposal 0.5 deg), burst shots and the time to recover to the first-shot cone, sustained shots (30 for automatic classes) and the camera climb range (rifle 6-12 deg as proposed, SMG 4-9 deg). All are proposals until the owner confirms them.
- **Model simulation** (`Gunplay/Logic/GunplaySimulation.cs`): the game's own `SpreadModel` and `RecoilSolver` stepped at 60 Hz with the weapon's catalog fire interval; the verifier (`Stage1GunplayChecks`) and the in-game `catalog sim` command both use it. It also proves the checks bite: a rifle whose spread never grows is reported as a laser, three times the kick as climbing too far, a wide pistol as a wide first shot.
- **Per-weapon tuning** of the six Stage 1 profiles (starting points, `config/gunplay.json`); the model's numbers, not measured feel:

| Weapon | First-shot cone | Cone after a 3-round burst | Back to the first-shot cone | 30-round climb | Cone after 30 rounds |
|---|---|---|---|---|---|
| Glock 17 | 0.30 deg | 0.45 | 133 ms | - | - |
| .44 AutoMag | 0.35 | 0.85 | 367 ms | - | - |
| Street Sweeper | 0.45 (+3.4 pellet ring) | 0.66 | 250 ms | - | - |
| Remington 1100 | 0.40 (+3.0 pellet ring) | 0.58 | 267 ms | - | - |
| IMI Uzi | 0.45 | 0.69 | 133 ms | 7.2 deg (target 4-9) | 3.4 (grows every shot) |
| AK-47 | 0.35 | 0.65 | 167 ms | 10.1 deg (target 6-12) | 2.8 (grows every shot) |

- **Range measurement** (`range` command, `RangeRecorder`, `tools/perf/Measure-GunplayRange.ps1`): the player fires single shots, bursts and continuous fire at a point 25 m ahead and the gunplay module logs every bullet's deviation from the line to that point next to the cone the model wrote (`range_shot`, `range_summary`). `aim on|off [crouched] [cover] [speed]` is a test hook that makes the model treat the player as aiming (the autopilot cannot hold the aim button). Scenario `stage1-gunplay-range`.
- **Shoulder swap without the aim button** (`swap left|right|toggle|probe`): forced sides slide the game's aim-camera table to -1/+1 and `swap probe` fails unless the live table equals the originals times the side factor and the slide has finished; `ray left|right` casts the engine ray from the player's own sides for clearance at wall spots. Scenario `stage1-shoulder-swap` (swap mechanism, the test wall at 0.4 to 3 m facing both ways, four city capture points). The shoulder swap code itself (`Aim/ShoulderSwap.cs`) is unchanged apart from the test hook: nothing in it needed finishing that offline evidence could show.
- DevTools > WEAPONS lists the Stage 1 arsenal ("Give ... (Stage 1, tier)").

**Evidence**
- RAN-PASS: build; `tools/verify.ps1` 912 passed 0 failed (new section "Stage 1 gunplay": every profile meets its class targets, and the negative cases fail as they should); `Run-Tests.ps1` 175 passed (the range log parser included); `checks.py`.
- NOT RUN: `T042-gunplay-range`, `T042-shoulder-swap` in game (reason below). The range scenario's key question, whether bullets fired by the game's shoot-at-coordinate task honour the accuracy the spread model writes, is unanswered; if they do not, only `catalog sim` (the model) stays as the first-shot/burst/climb evidence and the feel check decides.
- NEEDS OWNER: feel per weapon class and shoulder swap near walls with the real aim camera (`T042-feel-per-class`).

## Blocked

Same cause as T-041: the game cannot start (fatal error "GTA IV requires a sound card in order to run", no active audio output). Needed: reconnect an audio output, then
`./tools/verify-local.ps1 -GameDirectory "C:\Games\Grand Theft Auto IV\GTAIV" -Branch stage1/T-042 -AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T041-stage1-arsenal,T042-gunplay-range,T042-shoulder-swap`.

## Open questions

1. The class targets are the design's proposals (0.5 deg, 6-12 deg AK climb) plus my own for burst recovery time (800 ms pistol/SMG/rifle, 1500 ms shotgun) and SMG climb (4-9 deg). Confirm or change them in `classTargets`.
2. The 30-round climb is set by `maxAccumulatedDegrees` (the cap) more than by the per-shot kick: a longer burst than 30 rounds climbs no further. Intended (a cap keeps a magazine dump on screen), tell me if you want it to keep climbing.
3. Does the shoulder swap clip walls with the real aim camera? The autopilot cannot enter the aim camera, so this is only checkable by hand (manual check `T042-feel-per-class`) plus the lateral ray clearances the scenario logs.

## Human test steps

1. Give yourself a Glock 17, IMI Uzi, Remington 1100 and AK-47 (DevTools > WEAPONS, entries marked Stage 1) and go to the test range (DevTools > TELEPORT).
2. Pistol: aim (LT / right mouse) and fire single shots, then a quick three-round burst, then wait. The shots should be precise; the crosshair opens with each shot and is closed again within a second.
3. Uzi: hold the trigger for a whole magazine. The crosshair keeps growing, the camera climbs steadily; pull down to stay on a target 25 m away.
4. AK-47: hold the trigger for 30 rounds at 25 m. It must not be a laser: the shots spread out and the view climbs about ten degrees (a third of the screen) if you do not pull down.
5. Remington 1100: single shots; each kicks hard and the pellets spread in a ring.
6. Shoulder swap: aim and press LB (or Z). The camera slides to the other shoulder in about 0.2 s. Try it on foot, in cover and standing with a wall close to either shoulder; report any place where the camera passes through the wall.
7. Tell me for each class: too weak, right or too strong; the numbers to change are in `config\gunplay.json` `weapons[]` (`recoil`, `spread`) and `classTargets`; DevTools > LIVE TUNING changes the live values.