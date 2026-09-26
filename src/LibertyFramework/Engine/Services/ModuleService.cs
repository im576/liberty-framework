using System.Collections.Generic;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Services
{
    // SDK IModules: what is loaded, what runs, and typed access to another module's public API.
    public sealed class ModuleService : IModules
    {
        private readonly LibertyEngine engine;

        internal ModuleService(LibertyEngine engine) { this.engine = engine; }

        public IList<ModuleStatus> List()
        {
            List<ModuleStatus> result = new List<ModuleStatus>();
            foreach (ModuleRuntime m in engine.Runtimes)
            {
                ModuleStatus status = new ModuleStatus();
                status.Manifest = m.Manifest; status.Running = m.Module.Running; status.FailureReason = m.Module.FailureReason;
                status.AverageMs = m.AverageMs; status.MaxMs = m.MaxMs; status.Interval = m.Module.Interval; status.Throttles = m.Throttles;
                result.Add(status);
            }
            return result;
        }

        public bool IsRunning(string id)
        {
            foreach (ModuleRuntime m in engine.Runtimes) { if (m.Id == id) { return m.Module.Running; } }
            return false;
        }

        public T Get<T>() where T : LibertyModule
        {
            foreach (ModuleRuntime m in engine.Runtimes)
            {
                T typed = m.Module as T;
                if (typed != null && typed.Running) { return typed; }
            }
            return null;
        }
    }
}
