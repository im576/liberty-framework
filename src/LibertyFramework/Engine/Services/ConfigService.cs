using System;
using System.Collections.Generic;
using System.IO;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IConfig: config\<module id>\<name>.json. Missing files are written from the module's defaults so players can
    // find and edit them; invalid files are logged and replaced in memory by the defaults (never overwritten on disk).
    // Watched files are re-read on the engine thread when their timestamp changes (checked once per second off the engine thread).
    public sealed class ConfigService : IConfig
    {
        private sealed class Watcher
        {
            internal LibertyModule Owner;
            internal string Path;
            internal DateTime Stamp;
            internal Action Reload;
        }

        private readonly List<Watcher> watches = new List<Watcher>();
        // The timestamps are read by a background timer, never on the engine thread: a file query can block for seconds
        // on a busy disk (measured: an 11 s stall in a config poll ended a test run in a crash). The engine thread only
        // takes the paths the timer found changed and re-reads those files.
        private readonly object gate = new object();
        private readonly Dictionary<string, DateTime> stamps = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> changed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private string[] watchedPaths = new string[0];
        private System.Threading.Timer timer;
        private int checking;

        public string PathOf(LibertyModule owner, string name)
        {
            return System.IO.Path.Combine(LibertyPaths.ConfigDirectory, System.IO.Path.Combine(StateService.Safe(owner.Id), StateService.Safe(name) + ".json"));
        }

        public T Load<T>(LibertyModule owner, string name, Func<T> defaults, Action<T> validate) where T : class
        {
            string path = PathOf(owner, name);
            if (!File.Exists(path))
            {
                T fresh = defaults();
                try
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                    JsonStore.Save(path, fresh);
                    RuntimeLog.Info("config_created " + owner.Id + "/" + name);
                }
                catch (Exception error) { RuntimeLog.Error("config_create_failed " + owner.Id + "/" + name + " error=" + error.Message); }
                return fresh;
            }
            try
            {
                T value = JsonStore.Load<T>(path);
                if (value == null) { throw new InvalidDataException("empty"); }
                if (validate != null) { validate(value); }
                return value;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("config_rejected " + owner.Id + "/" + name + " using defaults error=" + error.Message);
                return defaults();
            }
        }

        public void Watch<T>(LibertyModule owner, string name, Func<T> defaults, Action<T> validate, Action<T> onChanged) where T : class
        {
            if (owner == null) { throw new ArgumentNullException("owner"); }
            Watcher watch = new Watcher();
            watch.Owner = owner;
            watch.Path = PathOf(owner, name);
            watch.Stamp = Stamp(watch.Path);
            watch.Reload = () => onChanged(Load(owner, name, defaults, validate));
            watches.Add(watch);
            lock (gate)
            {
                if (!stamps.ContainsKey(watch.Path)) { stamps[watch.Path] = watch.Stamp; }
                RefreshWatchedPaths();
                if (timer == null) { timer = new System.Threading.Timer(state => CheckStamps(), null, 1000, 1000); }
            }
        }

        internal void RemoveOwner(LibertyModule owner)
        {
            watches.RemoveAll(w => w.Owner == owner);
            lock (gate) { RefreshWatchedPaths(); }
        }

        // Engine unload: stop the background timer.
        internal void Stop()
        {
            lock (gate) { if (timer != null) { timer.Dispose(); timer = null; } }
        }

        // Caller holds gate.
        private void RefreshWatchedPaths()
        {
            HashSet<string> paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Watcher watch in watches) { paths.Add(watch.Path); }
            watchedPaths = new string[paths.Count];
            paths.CopyTo(watchedPaths);
        }

        // Background timer thread. Skips a tick while the previous one is still blocked on the disk.
        private void CheckStamps()
        {
            if (System.Threading.Interlocked.Exchange(ref checking, 1) == 1) { return; }
            try
            {
                string[] paths;
                lock (gate) { paths = watchedPaths; }
                foreach (string path in paths)
                {
                    DateTime stamp = Stamp(path);
                    lock (gate)
                    {
                        DateTime known;
                        if (stamps.TryGetValue(path, out known) && known == stamp) { continue; }
                        stamps[path] = stamp;
                        changed.Add(path);
                    }
                }
            }
            catch (Exception error) { RuntimeLog.Error("config_check_failed error=" + error.Message); }
            finally { System.Threading.Interlocked.Exchange(ref checking, 0); }
        }

        // runAs runs the reload as its module (LibertyEngine.RunAs): a throwing validate/onChanged stops that module, which
        // removes its watches, so the loop walks a copy of the list.
        internal void Poll(Func<LibertyModule, Action, bool> runAs)
        {
            string[] paths;
            lock (gate)
            {
                if (changed.Count == 0) { return; }
                paths = new string[changed.Count];
                changed.CopyTo(paths);
                changed.Clear();
            }
            foreach (Watcher watch in watches.ToArray())
            {
                if (!watch.Owner.Running || Array.IndexOf(paths, watch.Path) < 0) { continue; }
                DateTime stamp;
                lock (gate) { stamp = stamps[watch.Path]; }
                if (stamp == watch.Stamp) { continue; }
                watch.Stamp = stamp;
                RuntimeLog.Info("config_changed " + watch.Path);
                runAs(watch.Owner, watch.Reload);
            }
        }

        private static DateTime Stamp(string path)
        {
            try { return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue; }
            catch (Exception error) { RuntimeLog.Error("config_stamp_failed path=" + path + " error=" + error.Message); return DateTime.MinValue; }
        }
    }
}
