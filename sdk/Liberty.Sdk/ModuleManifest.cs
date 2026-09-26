namespace Liberty.Sdk
{
    // The resolved manifest of a loaded module (from its ModuleAttribute).
    public sealed class ModuleManifest
    {
        public string Id { get; internal set; }
        public string Version { get; internal set; }
        public int Order { get; internal set; }
        // Copies: the manifest is readable by every module (IModules.List), and the engine's capability checks read these,
        // so a caller must not be able to rewrite the arrays in place.
        public string[] Requires { get { return (string[])requires.Clone(); } internal set { requires = Copy(value); } }
        public string[] Capabilities { get { return (string[])capabilities.Clone(); } internal set { capabilities = Copy(value); } }
        public string SdkVersion { get; internal set; }
        public string Description { get; internal set; }
        public float BudgetMs { get; internal set; }
        public string Assembly { get; internal set; }

        private string[] requires = new string[0];
        private string[] capabilities = new string[0];

        public bool Has(string capability)
        {
            foreach (string c in capabilities) { if (c == capability) { return true; } }
            return false;
        }

        private static string[] Copy(string[] value) { return value != null ? (string[])value.Clone() : new string[0]; }

        public static ModuleManifest From(ModuleAttribute attribute, string assembly)
        {
            ModuleManifest m = new ModuleManifest();
            m.Id = attribute.Id; m.Version = attribute.Version; m.Order = attribute.Order;
            m.Requires = attribute.Requires ?? new string[0]; m.Capabilities = attribute.Capabilities ?? new string[0];
            m.SdkVersion = attribute.SdkVersion; m.Description = attribute.Description; m.BudgetMs = attribute.BudgetMs;
            m.Assembly = assembly;
            return m;
        }
    }
}