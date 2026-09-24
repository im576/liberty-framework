using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GTA;

namespace LibertyFramework.Core.Logging
{
    internal static class RuntimeLog
    {
        private static readonly object Sync = new object();

        internal static void Info(string message)
        {
            string line = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + " [INFO] " + message;
            string gameDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string logDirectory = Path.Combine(gameDirectory, "scripts", "LibertyFramework", "logs");

            try
            {
                lock (Sync)
                {
                    Directory.CreateDirectory(logDirectory);
                    File.AppendAllText(Path.Combine(logDirectory, "LibertyFramework.log"), line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception error)
            {
                Game.Console.Print("[LibertyFramework] Log file write failed: " + error);
            }
        }
    }
}
