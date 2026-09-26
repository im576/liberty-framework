namespace Liberty.Sdk
{
    // Liberty log (scripts\LibertyFramework\logs\LibertyFramework.log), prefixed with the module id.
    public interface ILog
    {
        void Info(LibertyModule owner, string message);
        void Warn(LibertyModule owner, string message);
        void Error(LibertyModule owner, string message);
    }
}