using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.WeaponProbe
{
    // T-007 diagnostic only. It never grants a weapon until the console command is used.
    public sealed class WeaponSlotProbe : Script
    {
        internal const int CandidateId = 58;
        private static readonly Weapon Candidate = (Weapon)CandidateId;

        public WeaponSlotProbe()
        {
            BindConsoleCommand("LFWeaponStatus", new ConsoleCommandDelegate(Status),
                "- log the currently selected weapon identifier");
            BindConsoleCommand("LFWeaponGive", new ConsoleCommandDelegate(GiveCandidate),
                "- grant and select the T-007 custom pistol candidate (requires its data override)");
            BindConsoleCommand("LFWeaponVanilla", new ConsoleCommandDelegate(GiveVanilla),
                "- grant and select the vanilla pistol");
            RuntimeLog.Info("T-007 weapon probe commands registered; candidate_id=" + CandidateId);
        }

        private void Status(ParameterCollection parameters)
        {
            Execute("status", delegate { Game.Console.Print("[LibertyFramework] " + ReportStatus(Player)); });
        }

        private void GiveCandidate(ParameterCollection parameters)
        {
            Execute("give_candidate", delegate
            {
                Game.Console.Print("[LibertyFramework] " + SelectCandidate(Player));
            });
        }

        private void GiveVanilla(ParameterCollection parameters)
        {
            Execute("give_vanilla", delegate
            {
                Game.Console.Print("[LibertyFramework] " + SelectVanilla(Player));
            });
        }

        internal static string SelectCandidate(Player player)
        {
            int ammo;
            bool hadHandgun = ReadCurrentHandgunAmmo(player, out ammo);
            player.Character.Weapons.Select(Candidate);
            if (hadHandgun) { player.Character.Weapons.FromType(Candidate).Ammo = ammo; }
            return ReportStatus(player);
        }

        internal static string SelectVanilla(Player player)
        {
            int ammo;
            bool hadHandgun = ReadCurrentHandgunAmmo(player, out ammo);
            player.Character.Weapons.Select(Weapon.Handgun_Glock);
            if (hadHandgun) { player.Character.Weapons.Glock.Ammo = ammo; }
            return ReportStatus(player);
        }

        private static bool ReadCurrentHandgunAmmo(Player player, out int ammo)
        {
            ammo = 0;
            if (player == null || player.Character == null) { throw new InvalidOperationException("Player is not ready"); }
            if (player.Character.Weapons.FromType(Candidate).isPresent)
            {
                ammo = player.Character.Weapons.FromType(Candidate).Ammo;
                return true;
            }
            if (player.Character.Weapons.Glock.isPresent)
            {
                ammo = player.Character.Weapons.Glock.Ammo;
                return true;
            }
            return false;
        }

        internal static string ReportStatus(Player player)
        {
            if (player == null || player.Character == null) { throw new InvalidOperationException("Player is not ready"); }
            int current = (int)player.Character.Weapons.CurrentType;
            bool candidatePresent = player.Character.Weapons.FromType(Candidate).isPresent;
            bool vanillaPresent = player.Character.Weapons.Glock.isPresent;
            int handgunAmmo = candidatePresent ? player.Character.Weapons.FromType(Candidate).Ammo :
                vanillaPresent ? player.Character.Weapons.Glock.Ammo : 0;
            string message = "T-007 weapon status current_id=" + current +
                " candidate_present=" + candidatePresent + " vanilla_present=" + vanillaPresent +
                " handgun_ammo=" + handgunAmmo;
            RuntimeLog.Info(message);
            return message;
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
