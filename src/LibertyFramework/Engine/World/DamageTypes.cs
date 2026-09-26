using Liberty.Sdk;

namespace LibertyFramework.Engine.World
{
    // Game weapon type -> SDK DamageType. Ids follow ScriptHookDotNet's GTA.Weapon enum: 0-20 base weapons, 21-44
    // episodic (meaning depends on the episode), 45+ special (49 rammed by car, 50 run over, 51 explosion, 52 drive-by,
    // 53 drowning, 54 fall, 55 unidentified, 56 any melee). The damage routine also reports 58 from vehicle code; ids
    // 58-60 are Liberty's test weapons when a ped is the attacker.
    internal static class DamageTypes
    {
        internal static DamageType Classify(int weapon, int attackerKind, Episode episode)
        {
            if (attackerKind == 2) { return DamageType.Vehicle; }
            switch (weapon)
            {
                case 0: case 1: case 2: case 3: case 56: return DamageType.Melee;
                case 4: case 6: case 18: case 51: return DamageType.Explosion;
                case 5: case 19: return DamageType.Fire;
                case 49: case 50: return DamageType.Vehicle;
                case 53: return DamageType.Drowning;
                case 54: return DamageType.Fall;
                case 52: return DamageType.Bullet;
                case 55: return DamageType.Other;
            }
            if ((weapon >= 7 && weapon <= 17) || weapon == 20) { return DamageType.Bullet; }
            if (weapon >= 58 && weapon <= 60) { return attackerKind == 1 ? DamageType.Bullet : DamageType.Vehicle; }
            if (weapon >= 21 && weapon <= 44) { return Episodic(weapon, episode); }
            return DamageType.Unknown;
        }

        private static DamageType Episodic(int weapon, Episode episode)
        {
            if (episode == Episode.TheBalladOfGayTony)
            {
                if (weapon == 21 || weapon == 36) { return DamageType.Explosion; }   // grenade launcher, sticky bomb
                if (weapon == 41) { return DamageType.Other; }                        // parachute
                return DamageType.Bullet;
            }
            if (episode == Episode.TheLostAndDamned)
            {
                if (weapon == 21 || weapon == 28) { return DamageType.Explosion; }   // grenade launcher, pipe bomb
                if (weapon == 24) { return DamageType.Melee; }                        // pool cue
                return DamageType.Bullet;
            }
            return DamageType.Unknown;
        }
    }
}
