namespace LibertyFramework.CombatEffects
{
    internal static class HitClassifier
    {
        // GTA IV PedBone values from Sanny Builder's IV enum. The native's returned
        // values need an in-game check before physical limb effects are enabled.
        internal static HitRegion Classify(int bone)
        {
            switch (bone)
            {
                case 1204: case 1205: return HitRegion.Head;
                case 417: case 1202: case 1203: return HitRegion.Torso;
                case 1216: case 1217: case 1218: case 1219: return HitRegion.LeftArm;
                case 1223: case 1224: case 1225: case 1232: return HitRegion.RightArm;
                case 418: case 419: case 420: case 421: return HitRegion.LeftLeg;
                case 423: case 424: case 425: case 1200: return HitRegion.RightLeg;
                default: return HitRegion.Unknown;
            }
        }
    }
}
