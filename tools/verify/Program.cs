using System;

namespace LibertyFramework.Verify
{
    // Offline verification for Liberty Framework: engine address resolution against the real
    // GTAIV.exe, plus the pure gunplay/config logic. Exit code 0 only when every check passes.
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("usage: OfflineVerify <GTAIV.exe> <repo root>");
                return 2;
            }
            Checker check = new Checker();
            try
            {
                Console.WriteLine("== Engine address resolution");
                AddressChecks.Run(args[0], check);
                Console.WriteLine("== Native names used by the DLL");
                NativeChecks.Run(args[0], args[1], check);
                EngineChecks.Run(args[0], args[1], check);
                Console.WriteLine("== Gunplay logic and configuration");
                LogicChecks.Run(args[1], check);
                Console.WriteLine("== Arsenal core (T-020)");
                ArsenalCoreChecks.Run(args[1], check);
                Console.WriteLine("== Phase 2 ownership and catalog");
                Phase2SystemsChecks.Run(args[1], check);
                Console.WriteLine("== Feel and presentation (T-011, T-013..T-017, T-021)");
                FeelChecks.Run(args[1], check);
                Console.WriteLine("== Vehicle body parts (T-023)");
                VehicleChecks.Run(args[0], check);
                Console.WriteLine("== Dismemberment plans (T-022)");
                CombatChecks.Run(args[0], args[1], check);
                Console.WriteLine("== Dismemberment collapse engine machine code (ADR-0005)");
                CollapseEngineChecks.Run(check);
                Console.WriteLine("== Atmosphere: weather director and density governor (M-2, E-5)");
                AtmosphereChecks.Run(args[1], check);
                Console.WriteLine("== Hot reload file watcher (M5)");
                HotReloadChecks.Run(check);
            }
            catch (Exception error)
            {
                check.True("verifier completed without exception", false, error.ToString());
            }
            Console.WriteLine("RESULT passed=" + check.Passed + " failed=" + check.Failed);
            return check.Failed == 0 ? 0 : 1;
        }
    }
}
