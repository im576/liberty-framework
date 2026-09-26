namespace Liberty.Sdk
{
    // Where modules find the engine. Installed by the engine before any module starts.
    public static class LibertyHost
    {
        public static ILiberty Current { get; internal set; }
    }
}