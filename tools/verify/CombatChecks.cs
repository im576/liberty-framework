using System;
using LibertyFramework.CombatEffects.Logic;

namespace LibertyFramework.Verify
{
    // T-022: limb cut plans (pure logic).
    internal static class CombatChecks
    {
        internal static void Run(Checker check)
        {
            LimbCutPlan knee = LimbCutPlan.ForHitBone(0x1A4);
            check.True("left foot hit cuts at the knee", knee != null && knee.CutTag == 0x1A3 && knee.StumpTag == 0x1A2 &&
                Array.IndexOf(knee.RemovedTags, 0x1A2) < 0 && Array.IndexOf(knee.RemovedTags, 0x1A5) >= 0, knee == null ? "null" : knee.Name);
            LimbCutPlan hip = LimbCutPlan.ForHitBone(0x1A7);
            check.True("right thigh hit cuts at the hip and removes the whole leg", hip != null && hip.CutTag == 0x1A7 && hip.StumpTag == 0x1A1 &&
                Array.IndexOf(hip.RemovedTags, 0x4B0) >= 0, hip == null ? "null" : hip.Name);
            LimbCutPlan elbow = LimbCutPlan.ForHitBone(0x4D0);
            check.True("right hand hit cuts at the elbow, fingers included", elbow != null && elbow.CutTag == 0x4C9 &&
                Array.IndexOf(elbow.RemovedTags, 0x35C4) >= 0 && Array.IndexOf(elbow.RemovedTags, 0x4C8) < 0, elbow == null ? "null" : elbow.Name);
            LimbCutPlan shoulder = LimbCutPlan.ForHitBone(0x4C1);
            check.True("left upper-arm hit cuts at the shoulder, clavicle stays", shoulder != null && shoulder.CutTag == 0x4C1 &&
                shoulder.StumpTag == 0x4C0 && Array.IndexOf(shoulder.RemovedTags, 0x4C0) < 0, shoulder == null ? "null" : shoulder.Name);
            check.True("head and torso hits never sever a limb", LimbCutPlan.ForHitBone(0x4B5) == null && LimbCutPlan.ForHitBone(0x1A1) == null, "");
        }
    }
}
