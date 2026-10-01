using System;
using System.IO;
using System.Reflection;
using Liberty.Sdk;
using LibertyFramework.Engine.Services;

namespace LibertyFramework.Verify
{
    // Exercise the real watcher against a temporary file. Drive the timer's check deterministically so tests do not
    // depend on sleeps, and use a long timer period while doing so. No engine, game directory or game process is used.
    internal static class ConfigWatchChecks
    {
        private sealed class TestModule : LibertyModule { }

        internal static void Run(Checker check)
        {
            string directory = Path.Combine(Path.GetTempPath(), "liberty-config-watch-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "shared.json");
            ConfigService service = new ConfigService();
            TestModule first = new TestModule { Running = true };
            TestModule second = new TestModule { Running = true };
            int firstReads = 0, secondReads = 0;
            string firstValue = null, secondValue = null;
            Func<LibertyModule, Action, bool> run = (owner, action) => { action(); return true; };
            DateTime stamp = DateTime.UtcNow;
            try
            {
                File.WriteAllText(path, "original");
                File.SetLastWriteTimeUtc(path, stamp);
                service.WatchFile(first, path, () => { firstReads++; firstValue = File.ReadAllText(path); });
                DisableAutomaticChecks(service);
                service.WatchFile(second, path, () => { secondReads++; secondValue = File.ReadAllText(path); });
                CheckStamps(service);
                service.Poll(run);
                check.True("unchanged shared config performs no reload", firstReads == 0 && secondReads == 0, "");

                File.WriteAllText(path, "changed");
                File.SetLastWriteTimeUtc(path, stamp.AddSeconds(10));
                // A changed file is still invisible to the game-thread poll until the background check reports it.
                for (int i = 0; i < 100; i++) { service.Poll(run); }
                check.True("idle engine polling does not inspect or reread a changed config", firstReads == 0 && secondReads == 0, "");
                CheckStamps(service);
                service.Poll(run);
                check.True("one background change reloads both owners of a shared file",
                    firstReads == 1 && secondReads == 1 && firstValue == "changed" && secondValue == "changed", "");
                CheckStamps(service);
                service.Poll(run);
                check.True("consumed change does not reload again", firstReads == 1 && secondReads == 1, "");

                service.RemoveOwner(first);
                File.WriteAllText(path, "second owner");
                File.SetLastWriteTimeUtc(path, stamp.AddSeconds(20));
                CheckStamps(service);
                service.Poll(run);
                check.True("removed owner stays untouched while the remaining owner reloads",
                    firstReads == 1 && secondReads == 2 && secondValue == "second owner", "");
                second.Running = false;
                File.SetLastWriteTimeUtc(path, stamp.AddSeconds(30));
                CheckStamps(service);
                service.Poll(run);
                check.True("stopped module receives no config callback", secondReads == 2, "");
                service.RemoveOwner(second);
                service.Stop();
                check.True("engine unload disposes the config timer",
                    typeof(ConfigService).GetField("timer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service) == null, "");
            }
            finally
            {
                service.Stop();
                Directory.Delete(directory, true);
            }
            CheckPathCasing(check);
        }

        private static void CheckPathCasing(Checker check)
        {
            string directory = Path.Combine(Path.GetTempPath(), "liberty-config-watch-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "shared.json");
            ConfigService service = new ConfigService();
            TestModule first = new TestModule { Running = true };
            TestModule second = new TestModule { Running = true };
            int firstReads = 0, secondReads = 0;
            try
            {
                DateTime stamp = DateTime.UtcNow;
                File.WriteAllText(path, "original");
                File.SetLastWriteTimeUtc(path, stamp);
                service.WatchFile(first, path, () => firstReads++);
                DisableAutomaticChecks(service);
                // ConfigService deliberately treats paths case-insensitively, including on offline test hosts.
                service.WatchFile(second, path.ToUpperInvariant(), () => secondReads++);
                File.WriteAllText(path, "changed");
                File.SetLastWriteTimeUtc(path, stamp.AddSeconds(10));
                CheckStamps(service);
                service.Poll((owner, action) => { action(); return true; });
                check.True("shared path casing does not lose an owner's reload", firstReads == 1 && secondReads == 1, "");
            }
            finally
            {
                service.Stop();
                Directory.Delete(directory, true);
            }
        }

        private static void CheckStamps(ConfigService service)
        {
            typeof(ConfigService).GetMethod("CheckStamps", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(service, null);
        }

        private static void DisableAutomaticChecks(ConfigService service)
        {
            System.Threading.Timer timer = (System.Threading.Timer)typeof(ConfigService)
                .GetField("timer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(service);
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
    }
}
