using System.Collections.Generic;

namespace Liberty.Sdk
{
    // Loaded modules and cross-module access.
    public interface IModules
    {
        IList<ModuleStatus> List();
        bool IsRunning(string id);
        // Another module's public object (for mods that expose an API to each other); null when absent or stopped.
        T Get<T>() where T : LibertyModule;
    }
}