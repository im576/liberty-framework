using System;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.WeaponProbe
{
    // T-007 diagnostic only. It never grants a weapon until the console command is used.
    public sealed class WeaponSlotProbe : Script
    {
        private const int CandidateId = 58;
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
            Execute("status", delegate { ReportStatus(); });
        }

        private void GiveCandidate(ParameterCollection parameters)
        {
            Execute("give_candidate", delegate
            {
                Player.Character.Weapons.Select(Candidate);
                ReportStatus();
            });
        }

        private void GiveVanilla(ParameterCollection parameters)
        {
            Execute("give_vanilla", delegate
            {
                Player.Character.Weapons.Select(Weapon.Handgun_Glock);
                ReportStatus();
            });
        }

        private void ReportStatus()
        {
            int current = (int)Player.Character.Weapons.CurrentType;
            bool candidatePresent = Player.Character.Weapons.FromType(Candidate).isPresent;
            bool vanillaPresent = Player.Character.Weapons.Glock.isPresent;
            string message = "T-007 weapon status current_id=" + current +
                " candidate_present=" + candidatePresent + " vanilla_present=" + vanillaPresent;
            RuntimeLog.Info(message);
            Game.Console.Print("[LibertyFramework] " + message);
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
