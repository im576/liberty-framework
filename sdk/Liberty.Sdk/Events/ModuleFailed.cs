namespace Liberty.Sdk.Events
{
    // A module threw and was stopped by the engine.
    public struct ModuleFailed
    {
        public string ModuleId;
        public string Error;
    }
}