using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine
{
    // Development hot reload (ROADMAP M5): watches the mods folder and reports a mod assembly once a change has settled.
    // A changed file must keep the same size and write time for one whole poll (a build or copy still writing it is
    // skipped), and its content hash must differ from what is loaded (touching a file reloads nothing). The engine
    // then swaps the modules (LibertyEngine.ReloadAssembly). .NET Framework cannot unload an assembly outside its
    // AppDomain, so every reload leaks the old copy; the engine counts it and refuses reloads past its limit.
    internal sealed class ModuleReloader
    {
        private struct Stamp
        {
            internal long Length;
            internal DateTime WriteUtc;
            internal bool Same(Stamp other) { return Length == other.Length && WriteUtc == other.WriteUtc; }
        }

        private readonly string directory;
        private readonly Dictionary<string, Stamp> seen = new Dictionary<string, Stamp>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Stamp> changing = new Dictionary<string, Stamp>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> loadedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private int lastPollMs;
        private bool polledOnce;

        internal ModuleReloader(string directory) { this.directory = directory; }

        // The bytes the engine loaded for a path (at discovery or reload): the reference for "did the content change".
        internal void Loaded(string path, byte[] bytes)
        {
            loadedHashes[Path.GetFullPath(path)] = Hash(bytes);
            Stamp stamp;
            if (TryStamp(path, out stamp)) { seen[Path.GetFullPath(path)] = stamp; }
        }

        // Paths whose content changed (or new mod files) and settled since the last call; at most one poll per interval.
        internal List<string> Poll(int nowMs, int intervalMs)
        {
            List<string> ready = new List<string>();
            if (polledOnce && unchecked(nowMs - lastPollMs) < intervalMs) { return ready; }
            polledOnce = true;
            lastPollMs = nowMs;
            if (!Directory.Exists(directory)) { return ready; }
            foreach (string file in Directory.GetFiles(directory, "*.dll"))
            {
                string path = Path.GetFullPath(file);
                Stamp stamp;
                if (!TryStamp(path, out stamp)) { continue; }
                Stamp previous;
                if (seen.TryGetValue(path, out previous) && previous.Same(stamp)) { changing.Remove(path); continue; }
                Stamp pending;
                if (!changing.TryGetValue(path, out pending) || !pending.Same(stamp)) { changing[path] = stamp; continue; }
                // Unchanged for a whole poll: settled.
                changing.Remove(path);
                seen[path] = stamp;
                string hash;
                byte[] bytes = TryRead(path);
                if (bytes == null) { changing[path] = stamp; seen.Remove(path); continue; }
                if (loadedHashes.TryGetValue(path, out hash) && hash == Hash(bytes)) { continue; }
                ready.Add(path);
            }
            return ready;
        }

        private static bool TryStamp(string path, out Stamp stamp)
        {
            stamp = default(Stamp);
            try
            {
                FileInfo info = new FileInfo(path);
                if (!info.Exists) { return false; }
                stamp.Length = info.Length;
                stamp.WriteUtc = info.LastWriteTimeUtc;
                return true;
            }
            // Expected while a build replaces the file; the next poll retries. Logged (AGENTS.md rule 8) at info level.
            catch (IOException error) { RuntimeLog.Info("hot_reload_stat_busy " + Path.GetFileName(path) + " error=" + error.Message); return false; }
            catch (UnauthorizedAccessException error) { RuntimeLog.Info("hot_reload_stat_denied " + Path.GetFileName(path) + " error=" + error.Message); return false; }
        }

        // Null while another process still holds the file for writing (the next settled poll retries).
        private static byte[] TryRead(string path)
        {
            try { return File.ReadAllBytes(path); }
            catch (IOException error) { RuntimeLog.Info("hot_reload_read_busy " + Path.GetFileName(path) + " error=" + error.Message); return null; }
            catch (UnauthorizedAccessException error) { RuntimeLog.Info("hot_reload_read_denied " + Path.GetFileName(path) + " error=" + error.Message); return null; }
        }

        internal static string Hash(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create()) { return BitConverter.ToString(sha.ComputeHash(bytes)); }
        }
    }
}
