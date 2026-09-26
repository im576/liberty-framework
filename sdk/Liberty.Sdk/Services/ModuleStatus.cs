namespace Liberty.Sdk
{
    public struct ModuleStatus
    {
        public ModuleManifest Manifest;
        public bool Running;
        public string FailureReason;
        public float AverageMs;
        public float MaxMs;
        public int Interval;
        public int Throttles;
    }
}