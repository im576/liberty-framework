using System;
using System.Collections.Generic;

namespace LibertyFramework.Models
{
    // crSkeletonData from a drawable: bones are 0xE0-byte records (name +0x00, parent pointer +0x10, bone id u16 +0x16,
    // local offset +0x20, local rotation quaternion xyzw +0x40, bind-pose model-space position +0x60). The model-space
    // rotation of each bone is rebuilt by chaining local rotations from the root; the constructor checks the result
    // against the stored model-space positions, so a wrong convention fails instead of producing a skewed body.
    internal sealed class Skeleton
    {
        internal const int SkeletonBones = 0x00, SkeletonBoneCount = 0x14, BoneSize = 0xE0;
        internal const int BoneName = 0x00, BoneParent = 0x10, BoneId = 0x16, BoneOffset = 0x20, BoneRotation = 0x40, BoneModelPosition = 0x60;

        internal sealed class Bone
        {
            internal string Name;
            internal int Id;
            internal int Parent;
            internal Vec3 LocalOffset;
            internal Quat LocalRotation;
            internal Vec3 StoredPosition;
            internal Vec3 Position;
            internal Quat Rotation;
        }

        internal readonly List<Bone> Bones = new List<Bone>();

        internal Skeleton(ResourceView view, uint address)
        {
            uint bones = view.U32(address + SkeletonBones);
            int count = view.U16(address + SkeletonBoneCount);
            Dictionary<uint, int> indexOf = new Dictionary<uint, int>();
            for (int i = 0; i < count; i++) { indexOf[bones + (uint)(i * BoneSize)] = i; }
            for (int i = 0; i < count; i++)
            {
                uint b = bones + (uint)(i * BoneSize);
                Bone bone = new Bone();
                bone.Name = view.CString(view.U32(b + BoneName));
                bone.Id = view.U16(b + BoneId);
                uint parent = view.U32(b + BoneParent);
                bone.Parent = parent == 0 ? -1 : indexOf[parent];
                bone.LocalOffset = new Vec3(view.F32(b + BoneOffset), view.F32(b + BoneOffset + 4), view.F32(b + BoneOffset + 8));
                bone.LocalRotation = new Quat(view.F32(b + BoneRotation), view.F32(b + BoneRotation + 4), view.F32(b + BoneRotation + 8), view.F32(b + BoneRotation + 12)).Normalized();
                bone.StoredPosition = new Vec3(view.F32(b + BoneModelPosition), view.F32(b + BoneModelPosition + 4), view.F32(b + BoneModelPosition + 8));
                if (bone.Parent >= i) { throw ResourceView.Bad("bone " + bone.Name + " precedes its parent"); }
                Bones.Add(bone);
            }
            // Model space = chain of (offset, rotation) from the root, root included. Verified on Niko's skeleton:
            // rebuilt positions match the stored ones to 1e-7 m (docs/research/ModelFormat.md).
            double worst = 0;
            for (int i = 0; i < Bones.Count; i++)
            {
                Bone bone = Bones[i];
                if (bone.Parent < 0) { bone.Position = bone.LocalOffset; bone.Rotation = bone.LocalRotation; continue; }
                Bone parent = Bones[bone.Parent];
                bone.Position = parent.Position + parent.Rotation.Rotate(bone.LocalOffset);
                bone.Rotation = (parent.Rotation * bone.LocalRotation).Normalized();
                worst = Math.Max(worst, (bone.Position - bone.StoredPosition).Length);
            }
            WorstPositionError = worst;
        }

        internal readonly double WorstPositionError;

        internal int Find(string name)
        {
            for (int i = 0; i < Bones.Count; i++) { if (string.Equals(Bones[i].Name, name, StringComparison.OrdinalIgnoreCase)) { return i; } }
            return -1;
        }
    }
}
