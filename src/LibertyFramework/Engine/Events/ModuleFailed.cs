namespace LibertyFramework.Engine.Events
{
    // A module threw and was disabled by the engine.
    public struct ModuleFailed
    {
        public string ModuleId;
        public string Error;
    }
}