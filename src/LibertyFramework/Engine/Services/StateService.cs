using System;
using System.IO;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // Per-module save data: state\<module id>\<name>.json (DataContract types). Load returns null when absent or broken
    // (logged); Save writes atomically with one .bak (JsonStore).
    public sealed class StateService
    {
        public T Load<T>(Module owner, string name) where T : class
        {
            string path = PathFor(owner, name);
            if (!File.Exists(path)) { return null; }
            try { return JsonStore.Load<T>(path); }
            catch (Exception error) { RuntimeLog.Error("state_load_failed " + owner.Id + "/" + name + " error=" + error.Message); return null; }
        }

        public void Save<T>(Module owner, string name, T value) where T : class
        {
            string path = PathFor(owner, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            JsonStore.Save(path, value);
        }

        private static string PathFor(Module owner, string name)
        {
            return Path.Combine(LibertyPaths.StateDirectory, Path.Combine(owner.Id, name + ".json"));
        }
    }
}