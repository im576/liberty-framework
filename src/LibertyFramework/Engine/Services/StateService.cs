using System;
using System.IO;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IState: per-module save data, state\<module id>\<name>.json (DataContract types). Load returns null when absent
    // or broken (logged); Save writes atomically with one .bak (JsonStore).
    public sealed class StateService : IState
    {
        public T Load<T>(LibertyModule owner, string name) where T : class
        {
            string path = PathFor(owner, name);
            if (!File.Exists(path)) { return null; }
            try { return JsonStore.Load<T>(path); }
            catch (Exception error) { RuntimeLog.Error("state_load_failed " + owner.Id + "/" + name + " error=" + error.Message); return null; }
        }

        public void Save<T>(LibertyModule owner, string name, T value) where T : class
        {
            string path = PathFor(owner, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            JsonStore.Save(path, value);
        }

        internal static string PathFor(LibertyModule owner, string name)
        {
            return Path.Combine(LibertyPaths.StateDirectory, Path.Combine(Safe(owner.Id), Safe(name) + ".json"));
        }

        // Module ids and names become folder and file names; reject anything that could leave the folder.
        internal static string Safe(string part)
        {
            if (string.IsNullOrEmpty(part) || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || part.Contains(".."))
            {
                throw new ArgumentException("invalid name '" + part + "'");
            }
            return part;
        }
    }
}
