using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.WeaponProbe
{
    // T-007 diagnostics. Weapon grants only occur after a menu action or console command.
    public sealed class WeaponSlotProbe : Script
    {
        internal const int PistolId = 58;
        internal const int CarbineId = 59;
        internal const int ShotgunId = 60;
        private static readonly Weapon TestPistol = (Weapon)PistolId;
        private static readonly Weapon TestCarbine = (Weapon)CarbineId;
        private static readonly Weapon TestShotgun = (Weapon)ShotgunId;

        public WeaponSlotProbe()
        {
            BindConsoleCommand("LFWeaponStatus", new ConsoleCommandDelegate(Status),
                "- log the currently selected weapon identifier");
            BindConsoleCommand("LFWeaponGive", new ConsoleCommandDelegate(GiveCandidate),
                "- select the T-007 test pistol (requires its data override)");
            BindConsoleCommand("LFWeaponVanilla", new ConsoleCommandDelegate(GiveVanilla),
                "- select the vanilla pistol");
            RuntimeLog.Info("T-007 weapon probe commands registered; candidate_ids=58,59,60");
        }

        private void Status(ParameterCollection parameters)
        {
            Execute("status", delegate { Game.Console.Print("[LibertyFramework] " + ReportStatus(Player)); });
        }

        private void GiveCandidate(ParameterCollection parameters)
        {
            Execute("give_candidate", delegate
            {
                Game.Console.Print("[LibertyFramework] " + SelectTestPistol(Player));
            });
        }

        private void GiveVanilla(ParameterCollection parameters)
        {
            Execute("give_vanilla", delegate
            {
                Game.Console.Print("[LibertyFramework] " + SelectVanillaPistol(Player));
            });
        }

        internal static string SelectTestPistol(Player player)
        {
            return SelectPair(player, TestPistol, Weapon.Handgun_Glock, true, 51);
        }

        internal static string SelectVanillaPistol(Player player)
        {
            return SelectPair(player, TestPistol, Weapon.Handgun_Glock, false, 51);
        }

        internal static string SelectTestCarbine(Player player)
        {
            return SelectPair(player, TestCarbine, Weapon.Rifle_M4, true, 120);
        }

        internal static string SelectVanillaCarbine(Player player)
        {
            return SelectPair(player, TestCarbine, Weapon.Rifle_M4, false, 120);
        }

        internal static string SelectTestShotgun(Player player)
        {
            return SelectPair(player, TestShotgun, Weapon.Shotgun_Basic, true, 30);
        }

        internal static string SelectVanillaShotgun(Player player)
        {
            return SelectPair(player, TestShotgun, Weapon.Shotgun_Basic, false, 30);
        }

        private static string SelectPair(Player player, Weapon custom, Weapon vanilla, bool useCustom, int initialAmmo)
        {
            if (player == null || player.Character == null) { throw new InvalidOperationException("Player is not ready"); }
            bool customPresent = player.Character.Weapons.FromType(custom).isPresent;
            bool vanillaPresent = player.Character.Weapons.FromType(vanilla).isPresent;
            int ammo = customPresent ? player.Character.Weapons.FromType(custom).Ammo :
                vanillaPresent ? player.Character.Weapons.FromType(vanilla).Ammo : 0;
            Weapon target = useCustom ? custom : vanilla;
            player.Character.Weapons.Select(target);
            player.Character.Weapons.FromType(target).Ammo =
                customPresent || vanillaPresent ? ammo : initialAmmo;
            return ReportStatus(player);
        }

        internal static string ReportStatus(Player player)
        {
            if (player == null || player.Character == null) { throw new InvalidOperationException("Player is not ready"); }
            int current = (int)player.Character.Weapons.CurrentType;
            int ammo = player.Character.Weapons.Current.Ammo;
            bool pistolTest = player.Character.Weapons.FromType(TestPistol).isPresent;
            bool pistolVanilla = player.Character.Weapons.Glock.isPresent;
            bool carbineTest = player.Character.Weapons.FromType(TestCarbine).isPresent;
            bool carbineVanilla = player.Character.Weapons.AssaultRifle_M4.isPresent;
            bool shotgunTest = player.Character.Weapons.FromType(TestShotgun).isPresent;
            bool shotgunVanilla = player.Character.Weapons.BasicShotgun.isPresent;
            RuntimeLog.Info("T-007 weapon status current_id=" + current + " ammo=" + ammo +
                " pistol_test=" + pistolTest + " pistol_vanilla=" + pistolVanilla +
                " carbine_test=" + carbineTest + " carbine_vanilla=" + carbineVanilla +
                " shotgun_test=" + shotgunTest + " shotgun_vanilla=" + shotgunVanilla);
            return "ID=" + current + " " + LabelFor(current) + " ammo=" + ammo;
        }

        private static string LabelFor(int id)
        {
            switch (id)
            {
                case PistolId: return "TEST PISTOL";
                case 7: return "VANILLA PISTOL";
                case CarbineId: return "TEST CARBINE";
                case 15: return "VANILLA CARBINE";
                case ShotgunId: return "TEST SHOTGUN";
                case 10: return "VANILLA SHOTGUN";
                default: return "OTHER";
            }
        }

        private void Execute(string action, Action operation)
        {
            try
            {
                if (Player == null || Player.Character == null)
                {
                    RuntimeLog.Info("T-007 " + action + " skipped: player not ready");
                    return;
                }
                operation();
            }
            catch (Exception error)
            {
                RuntimeLog.Error("T-007 " + action + " failed: " + error);
                Game.Console.Print("[LibertyFramework] T-007 " + action + " failed; see log");
            }
        }
    }
}
