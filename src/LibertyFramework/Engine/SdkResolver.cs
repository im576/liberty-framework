using System;
using System.IO;
using System.Reflection;

namespace LibertyFramework.Engine
{
    // Backstop for binding Liberty.Sdk and this engine assembly. ScriptHookDotNet loads script assemblies from bytes into a
    // domain whose ApplicationBase is the game folder, so Liberty.Sdk.dll lives next to GTAIV.exe and normal probing finds it;
    // this handler covers anything that asks otherwise (e.g. a mod referencing the engine) and always hands out the copy
    // already loaded, because two copies of the SDK would give two incompatible LibertyModule types. Installed before any
    // SDK type is touched, so this class must not reference Liberty.Sdk itself.
    internal static class SdkResolver
    {
        private static bool installed;

        internal static void Install()
        {
            if (installed) { return; }
            installed = true;
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            if (string.Equals(name, "LibertyFramework.net", StringComparison.OrdinalIgnoreCase)) { return typeof(SdkResolver).Assembly; }
            if (!string.Equals(name, "Liberty.Sdk", StringComparison.OrdinalIgnoreCase)) { return null; }
            foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(loaded.GetName().Name, "Liberty.Sdk", StringComparison.OrdinalIgnoreCase)) { return loaded; }
            }
            // Assembly.Location is empty for byte-loaded assemblies: use the game folder.
            string path = Path.Combine(LibertyFramework.Core.Config.LibertyPaths.GameDirectory, "Liberty.Sdk.dll");
            return File.Exists(path) ? Assembly.LoadFrom(path) : null;
        }
    }
}
