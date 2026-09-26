namespace Liberty.Sdk
{
    // The resolved manifest of a loaded module (from its ModuleAttribute).
    public sealed class ModuleManifest
    {
        public string Id { get; internal set; }
        public string Version { get; internal set; }
        public int Order { get; internal set; }
        public string[] Requires { get; internal set; }
        public string[] Capabilities { get; internal set; }
        public string SdkVersion { get; internal set; }
        public string Description { get; internal set; }
        public float BudgetMs { get; internal set; }
        public string Assembly { get; internal set; }

        public bool Has(string capability)
        {
            if (Capabilities == null) { return false; }
            foreach (string c in Capabilities) { if (c == capability) { return true; } }
            return false;
        }

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