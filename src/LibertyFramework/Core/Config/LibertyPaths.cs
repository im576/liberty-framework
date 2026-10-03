using System.Diagnostics;
using System.IO;

namespace LibertyFramework.Core.Config
{
    internal static class LibertyPaths
    {
        private static string gameDirectory;

        internal static string GameDirectory
        {
            get
            {
                if (gameDirectory == null) { gameDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); }
                return gameDirectory;
            }
        }

        internal static string Root { get { return Path.Combine(GameDirectory, Path.Combine("scripts", "LibertyFramework")); } }
        internal static string ConfigDirectory { get { return Path.Combine(Root, "config"); } }
        internal static string StateDirectory { get { return Path.Combine(Root, "state"); } }
        // Engine (ADR-0006): engine settings, optional mod assemblies, and the entity journal for reload cleanup.
        internal static string EngineConfig { get { return Path.Combine(ConfigDirectory, "engine.json"); } }
        internal static string ModsDirectory { get { return Path.Combine(Root, "mods"); } }
        internal static string EntityJournal { get { return Path.Combine(StateDirectory, "engine_entities.json"); } }
    }
}
