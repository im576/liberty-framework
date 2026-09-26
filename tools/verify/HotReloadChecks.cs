using System;
using System.Collections.Generic;
using System.IO;
using LibertyFramework.Engine;

namespace LibertyFramework.Verify
{
    // ModuleReloader (ROADMAP M5 hot reload) against a temporary mods folder: a change is reported only after it settles,
    // identical content (a touched file) is not, new mod files are, and a file still being written is retried.
    internal static class HotReloadChecks
    {
        internal static void Run(Checker check)
        {
            string folder = Path.Combine(Path.GetTempPath(), "liberty_verify_hotreload_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try { RunIn(folder, check); }
            finally { try { Directory.Delete(folder, true); } catch (IOException) { } }
        }

        private static void RunIn(string folder, Checker check)
        {
            string mod = Path.Combine(folder, "Mod.A.dll");
            byte[] original = { 1, 2, 3, 4 };
            File.WriteAllBytes(mod, original);
            ModuleReloader reloader = new ModuleReloader(folder);
            reloader.Loaded(mod, original);
            int now = 0;
            check.Equal("hot reload: nothing changed, nothing reported", 0, Poll(reloader, ref now).Count);

            File.WriteAllBytes(mod, new byte[] { 1, 2, 3, 4, 5 });
            check.Equal("hot reload: a fresh change waits one poll to settle", 0, Poll(reloader, ref now).Count);
            List<string> ready = Poll(reloader, ref now);
            check.True("hot reload: the settled change is reported", ready.Count == 1 && string.Equals(ready[0], Path.GetFullPath(mod), StringComparison.OrdinalIgnoreCase), string.Join(";", ready.ToArray()));
            reloader.Loaded(mod, new byte[] { 1, 2, 3, 4, 5 });
            check.Equal("hot reload: reported once", 0, Poll(reloader, ref now).Count);

            // Same bytes, new write time: settles, but the hash matches what is loaded.
            File.SetLastWriteTimeUtc(mod, DateTime.UtcNow.AddMinutes(5));
            Poll(reloader, ref now);
            check.Equal("hot reload: a touched but identical file is not reloaded", 0, Poll(reloader, ref now).Count);

            string added = Path.Combine(folder, "Mod.B.dll");
            File.WriteAllBytes(added, new byte[] { 9, 9 });
            Poll(reloader, ref now);
            ready = Poll(reloader, ref now);
            check.True("hot reload: a new mod file is reported", ready.Count == 1 && ready[0].EndsWith("Mod.B.dll", StringComparison.OrdinalIgnoreCase), string.Join(";", ready.ToArray()));
            reloader.Loaded(added, new byte[] { 9, 9 });

            // Held open for writing (a build in progress): not reported until it can be read.
            using (FileStream writer = new FileStream(mod, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                writer.Write(new byte[] { 7, 7, 7 }, 0, 3);
                writer.Flush();
                Poll(reloader, ref now);
                check.Equal("hot reload: a file still open for writing is not reported", 0, Poll(reloader, ref now).Count);
            }
            // Closing may or may not move the write time, so the report can come on either of the next two polls.
            ready = Poll(reloader, ref now);
            ready.AddRange(Poll(reloader, ref now));
            check.True("hot reload: reported once the writer closed it", ready.Count == 1, string.Join(";", ready.ToArray()));
            reloader.Loaded(mod, new byte[] { 7, 7, 7 });

            // Within the poll interval nothing is looked at, even a change that would already count as settled.
            File.WriteAllBytes(added, new byte[] { 8 });
            now += 1000;
            reloader.Poll(now, 1000);
            check.Equal("hot reload: no poll inside the interval", 0, reloader.Poll(now + 100, 1000).Count);
            check.Equal("hot reload: the next poll after the interval reports it", 1, reloader.Poll(now + 1000, 1000).Count);
        }

        private static List<string> Poll(ModuleReloader reloader, ref int now)
        {
            now += 1000;
            return reloader.Poll(now, 1000);
        }
    }
}
