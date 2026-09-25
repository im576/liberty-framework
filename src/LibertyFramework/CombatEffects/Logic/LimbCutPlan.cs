using System.Collections.Generic;

namespace LibertyFramework.CombatEffects.Logic
{
    // Which bones leave the body when a limb is severed at a hit bone (GTA IV PedBone tags, same values as
    // ScriptHookDotNet's GTA.Bone). A hit on the upper segment cuts at the shoulder/hip; a hit on the lower
    // segment cuts at the elbow/knee. CutTag is the first removed bone: its origin is the joint the rest
    // collapse into. StumpTag is the bone that stays, used to anchor the blood effect.
    internal sealed class LimbCutPlan
    {
        internal string Name;
        internal int CutTag;
        internal int StumpTag;
        internal int[] RemovedTags;

        private static readonly int[] LeftFingers = { 0x35D0, 0x35D1, 0x35D2, 0x35D3, 0x35D4, 0x35D5, 0x35D6, 0x35D7, 0x35D8, 0x35D9, 0x35E0, 0x35E1, 0x35E2, 0x35E3, 0x35E4 };
        private static readonly int[] RightFingers = { 0x35B0, 0x35B1, 0x35B2, 0x35B3, 0x35B4, 0x35B5, 0x35B6, 0x35B7, 0x35B8, 0x35B9, 0x35C0, 0x35C1, 0x35C2, 0x35C3, 0x35C4 };

        internal static LimbCutPlan ForHitBone(int boneTag)
        {
            switch (boneTag)
            {
                // Left arm: clavicle/upper arm/roll -> shoulder; forearm/twists/hand/fingers -> elbow.
                case 0x4C0: case 0x4C1: case 0x38A0: case 0x3DF1:
                    return Make("left_arm_shoulder", 0x4C1, 0x4C0, Join(new[] { 0x4C1, 0x38A0, 0x3DF1, 0x4C2, 0x38A1, 0x38A2, 0x4C3 }, LeftFingers));
                case 0x4C2: case 0x38A1: case 0x38A2: case 0x4C3:
                    return Make("left_arm_elbow", 0x4C2, 0x4C1, Join(new[] { 0x4C2, 0x38A1, 0x38A2, 0x4C3 }, LeftFingers));
                case 0x4C7: case 0x4C8: case 0x39A0: case 0x3E01:
                    return Make("right_arm_shoulder", 0x4C8, 0x4C7, Join(new[] { 0x4C8, 0x39A0, 0x3E01, 0x4C9, 0x39A1, 0x39A2, 0x4D0 }, RightFingers));
                case 0x4C9: case 0x39A1: case 0x39A2: case 0x4D0:
                    return Make("right_arm_elbow", 0x4C9, 0x4C8, Join(new[] { 0x4C9, 0x39A1, 0x39A2, 0x4D0 }, RightFingers));
                // Legs: thigh -> hip; calf/foot/toe -> knee.
                case 0x1A2:
                    return Make("left_leg_hip", 0x1A2, 0x1A1, new[] { 0x1A2, 0x1A3, 0x38B0, 0x1A4, 0x1A5 });
                case 0x1A3: case 0x38B0: case 0x1A4: case 0x1A5:
                    return Make("left_leg_knee", 0x1A3, 0x1A2, new[] { 0x1A3, 0x38B0, 0x1A4, 0x1A5 });
                case 0x1A7:
                    return Make("right_leg_hip", 0x1A7, 0x1A1, new[] { 0x1A7, 0x1A8, 0x39B0, 0x1A9, 0x4B0 });
                case 0x1A8: case 0x39B0: case 0x1A9: case 0x4B0:
                    return Make("right_leg_knee", 0x1A8, 0x1A7, new[] { 0x1A8, 0x39B0, 0x1A9, 0x4B0 });
                default:
                    return null;
            }
        }

        private static LimbCutPlan Make(string name, int cut, int stump, int[] removed)
        {
            LimbCutPlan plan = new LimbCutPlan();
            plan.Name = name;
            plan.CutTag = cut;
            plan.StumpTag = stump;
            plan.RemovedTags = removed;
            return plan;
        }

        private static int[] Join(int[] a, int[] b)
        {
            List<int> all = new List<int>(a);
            all.AddRange(b);
            return all.ToArray();
        }
    }
}
