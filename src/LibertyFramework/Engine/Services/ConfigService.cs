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
    // Watched files are re-read on the engine thread when their timestamp changes (checked once per second).
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
        private int lastPollMs;

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
            Watcher watch = new Watcher();
            watch.Owner = owner;
            watch.Path = PathOf(owner, name);
            watch.Stamp = Stamp(watch.Path);
            watch.Reload = () => onChanged(Load(owner, name, defaults, validate));
            watches.Add(watch);
        }

        internal void RemoveOwner(LibertyModule owner) { watches.RemoveAll(w => w.Owner == owner); }

        internal void Poll(Action<LibertyModule, Exception> fail)
        {
            int now = Environment.TickCount;
            if (watches.Count == 0 || unchecked(now - lastPollMs) < 1000) { return; }
            lastPollMs = now;
            for (int i = 0; i < watches.Count; i++)
            {
                Watcher watch = watches[i];
                if (!watch.Owner.Running) { continue; }
                DateTime stamp = Stamp(watch.Path);
                if (stamp == watch.Stamp) { continue; }
                watch.Stamp = stamp;
                RuntimeLog.Info("config_changed " + watch.Path);
                try { watch.Reload(); }
                catch (Exception error) { fail(watch.Owner, error); }
            }
        }

        private static DateTime Stamp(string path)
        {
            try { return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue; }
            catch (Exception error) { RuntimeLog.Error("config_stamp_failed path=" + path + " error=" + error.Message); return DateTime.MinValue; }
        }
    }
}
