# T-050 â€” R2 â€” Hit surface material

Status: **NEEDS-PLAYTEST** Â· Research lane (parallel) Â· Design: STAGE1 section 8

## Question

Can the bullet impact or the engine line test (ADR-0008, `lc_raycast`, `docs/research/Raycast.md`) report the material of the surface hit (concrete, wood, glass, metal, flesh, water)?

## Unlocks

Material-specific impacts and impact audio (T-048).

## Method

Read the line-test result structure already traced in Raycast.md; find where the physics hit carries a material index (bounds polygon material, per `docs/research/Collision.md`) and how it maps to material names (materials.dat or equivalent in the game data). Prove it with a spike command that prints the material for rays at known surfaces, verified by screenshots.

## Deliverable

A research note in `docs/research/` with evidence labels (VERIFIED IN GAME / VERIFIED OFFLINE / PLAUSIBLE / UNKNOWN), and one answer: **works**, **works with limits** or **Phase 3**. If it works, the smallest documented engine or SDK capability (natives in `NATIVES.md`, addresses by pattern in `MEMORY.md`, an ADR for hooks), with a queued check. A failed spike moves its feature later; it never removes it from the design.

## Human test steps

Fill in only if the owner must look at something.

## Research progress (2026-09-29)

**VERIFIED OFFLINE:** The result+0x48 packed field and low-byte material lookup were traced in the CE executable; [HitMaterial.md](../research/HitMaterial.md) and `material-probe.py audit` record the byte evidence. The existing ray ABI now exposes numeric `RayHit.SurfaceMaterialId` only when `HasSurfaceMaterial` is true. The scenario `research-material` and check `T050-material-probe` collect raw dumps, a miss control and four visible subjects; **VERIFIED IN GAME (2026-09-30, run 20260930-225503-991cd66):** `material_id` = 0-based row of `materials.dat`; 7 of 7 hits agree with their screenshots (BRICK_COBBLE, TARMAC, CONCRETE, SAND, PAVING_SLABS, CAR_METAL, PED). Answer: **works**. Limits (prop material not observed, player-origin ray) are in HitMaterial.md. The 2026-09-29 run's ped and prop rays had cleared; the scenario now uses typed `ray` queries and four extra world points. No material-name mapping is promoted from entity kind.

## Human test steps

1. Run the committed research branch through `tools/verify-local.ps1 -GameDirectory "C:\Games\Grand Theft Auto IV\GTAIV" -Branch research/stage1 -AnyBranch -NoPush -Restore -NoManual -Only LOOP-package-install,T050-material-probe` in PowerShell.
2. Open the four material screenshots in the resulting scenario folder; confirm the ray subjects and hit positions in the log.
3. Read the decoded report in HitMaterial.md; report any collision surface that disagrees with the visible target. No owner action is needed to execute the automated run.
