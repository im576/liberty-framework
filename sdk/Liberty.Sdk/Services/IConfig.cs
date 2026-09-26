using System;

namespace Liberty.Sdk
{
    // Per-module configuration: config\<module>\<name>.json (DataContract types). Tuning numbers belong here, not in code.
    public interface IConfig
    {
        // Loads the file (writing defaults() when absent) and validates it; returns defaults() when invalid (logged).
        T Load<T>(LibertyModule owner, string name, Func<T> defaults, Action<T> validate) where T : class;
        // Calls onChanged on the engine thread when the file changes on disk (checked once per second).
        void Watch<T>(LibertyModule owner, string name, Func<T> defaults, Action<T> validate, Action<T> onChanged) where T : class;
        string PathOf(LibertyModule owner, string name);
    }
}