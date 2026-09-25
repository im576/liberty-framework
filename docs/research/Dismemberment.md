# Dismemberment: approaches, references, and why ours is built this way

2026-09-25, Claude. This covers references, the approach compared with alternatives, the playtest bugs and their causes, and what is left.

## How others do it

| Where | Hiding the part | Severed piece | Wound surface |
|---|---|---|---|
| **GTA V, DismembermentASI** (CamxxCore; no license, reference only) | Hooks the fragCache draw-skeleton path and draws the chosen bone range at scale 0 | None (parts just vanish) | None |
| **GTA V, Dismemberment Ultimate** (current leading mod) | Low-level "hide-hook" with a **collapse target** for hidden bones (its credits cite memory help and MinHook) | A **cloned ped** matching the victim's look; head and limbs stay on the ground | **Dedicated 3D stump caps** (head, neck, upper arm, forearm, thigh, calf) from its own prop archive |
| **Left 4 Dead / Dying Light / Dead Rising / TLOU** (industry) | Shader clipping of the mesh below the cut | Pre-cut limb meshes or generic gibs | Authored gore caps covering the hole |
| **Unreal/Unity tutorials** | Bone scale 0 or mesh section hide | Separate skeletal/static limb mesh | Cap mesh at the joint |

## What we do, and whether it's the best approach here

1. **Hide:** our dismemberment collapses the bone subtree to the cut joint right after the engine computes the pose. This is the same technique as both GTA V mods, done natively after every `crSkeleton::Update` (ADR-0005). Without authored per-ped cut meshes, the engine offers no better option. **Keep.**
2. **Severed piece:** a cloned ped with everything except the limb collapsed. Dismemberment Ultimate uses the same clone. The alternatives are:
   - a limb prop model, which can't match every ped's clothes;
   - spawning nothing, which is worse.

   **Keep, but make it robust** (below).
3. **Wound surface:** caps are the one missing piece. Our cut closes skin into the joint ("pinched"). Violent Liberty draws wound textures, not caps. Real caps need an original, UV-mapped mesh per joint and a validated attachment path in GTA IV. **Next asset task** (see the last section).
4. **Hit data:** we poll health and the last-damaged bone. Violent Liberty hooks the engine's blood-impact/ped-update path and so knows the exact impact position. **Future:** a damage-event hook with the same native stub technique, which would also remove the scan.

## Playtest bugs and their causes

| Symptom | Cause | Fix (this build) |
|---|---|---|
| Leg "falls up" and floats | The clone spawned at the corpse, so its invisible collapsed body lay on the corpse and held the visible limb up; there was also an upward push | Spawn 0.7 m beside the corpse along the shot, at ground height. After 1.5 s, if the limb's joint (world position through `CPed::CopyBoneMatrix`) is more than 0.45 m above ground, the limb is removed with a small blood effect |
| Arm shows as the whole NPC briefly, then vanishes | The clone was made visible on a timer. When its ragdoll started or ended, the engine moved its skeleton, and our 0.5 s upkeep missed the new matrices, so the full body drew until the next refresh | Every tick, check the skeleton pointer with the engine's own `CPed::BoneMatrix` (no memory-check cost). On any move, republish and **hide** the clone. It is shown only after the engine confirms the collapse on its current skeleton for 3 ticks (`HitsFor`) |
| Four limbs blown off at once | Pellets and follow-up hits each queued a cut | One cut per body (`maximumCutsPerPed`, previous pass) |
| A knee or elbow sometimes doesn't cut | The cut bone's matrix didn't match uniquely on that model | Fall back up the limb (knee → hip, elbow → shoulder) |

## Next for dismemberment

- **Stump caps:**
  - Original low-poly caps for neck, shoulder, elbow, hip and knee, with an original texture, attached to the stump bone.
  - First validate a GTA IV attach path for a custom object, as the finishes pipeline did for weapons.
  - Keep them switchable so a failed attachment can't break the working cut.
- **Damage-event hook:** gives the exact impact position and removes the damage scan.
- **Bone-tag table calibration:** read crSkeletonData's bone tags (0xE0-byte records) at runtime, instead of matrix matching. This removes lookup failures entirely.

## References

- [DismembermentASI (GTA V)](https://github.com/CamxxCore/DismembermentASI): fragCache DrawSkeleton hook, bones drawn at scale 0 (reference only; no license).
- [Dismemberment Ultimate (GTA V)](https://www.gta5-mods.com/scripts/dismemberment-ultimate-singleplayer-for-enhanced-and-nve): hide-hook, collapse target, cloned ped, stump caps.
- [Unreal forum: gore mesh and dismemberment](https://forums.unrealengine.com/t/tutorial-preview-gore-mesh-dismemberment-tutorial/48172): caps and pre-cut meshes.
- [Violent Liberty](https://www.nexusmods.com/gta4/mods/1420): GTA IV impact/decal hooks (see [ViolentLiberty.md](ViolentLiberty.md)).
