using System;

namespace Liberty.Sdk
{
    // Declares a Liberty module and its manifest. The engine discovers every LibertyModule subclass carrying this
    // attribute (in the engine assembly and in scripts\LibertyFramework\mods\*.dll), checks SdkVersion, dependencies
    // and capabilities, and starts modules in dependency order, then Order.
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string id) { Id = id; Version = "1.0.0"; SdkVersion = Liberty.Sdk.SdkVersion.Text; }

        public string Id { get; private set; }
        public string Version { get; set; }
        public int Order { get; set; }
        // Module ids that must be running before this one starts; if one fails, this one is stopped too.
        public string[] Requires { get; set; }
        // Privileges beyond the normal SDK (see Capabilities); undeclared privileged calls are refused.
        public string[] Capabilities { get; set; }
        public string SdkVersion { get; set; }
        public string Description { get; set; }
        // Average milliseconds per update the module is expected to stay under (0 = engine default).
        public float BudgetMs { get; set; }
    }
}