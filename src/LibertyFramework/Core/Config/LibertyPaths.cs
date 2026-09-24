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
        internal static string PresetDirectory { get { return Path.Combine(ConfigDirectory, "presets"); } }
        internal static string StateDirectory { get { return Path.Combine(Root, "state"); } }
        internal static string GunplayConfig { get { return Path.Combine(ConfigDirectory, "gunplay.json"); } }
        internal static string Locations { get { return Path.Combine(ConfigDirectory, Path.Combine("devtools", "locations.json")); } }
        internal static string Finishes { get { return Path.Combine(ConfigDirectory, "finishes.json"); } }
        internal static string FreeAimRestoreState { get { return Path.Combine(StateDirectory, "freeaim_restore.json"); } }
    }
}
