using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK ILog: one log file for the engine and every module; each line is prefixed with the module id.
    public sealed class LogService : ILog
    {
        public void Info(LibertyModule owner, string message) { RuntimeLog.Info("[" + Id(owner) + "] " + message); }
        public void Warn(LibertyModule owner, string message) { RuntimeLog.Info("[" + Id(owner) + "] WARN " + message); }
        public void Error(LibertyModule owner, string message) { RuntimeLog.Error("[" + Id(owner) + "] " + message); }

        private static string Id(LibertyModule owner) { return owner != null ? owner.Id : "engine"; }
    }
}
