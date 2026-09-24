using System;
using System.IO;
using System.Text;
using GTA;
using LibertyFramework.Core.Config;

namespace LibertyFramework.Core.Logging
{
    internal static class RuntimeLog
    {
        private static readonly object Sync = new object();
        private const long MaximumLogBytes = 1048576;
        // Resolved once: the shot audit can log several lines per frame, and looking up the process
        // main module for every line costs far more than the append itself.
        private static string logDirectory;

        internal static void Info(string message)
        {
            Write("INFO", message);
        }

        internal static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            string line = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + " [" + level + "] " +
                message.Replace("\r", "\\r").Replace("\n", "\\n");
            try
            {
                lock (Sync)
                {
                    if (logDirectory == null)
                    {
                        string directory = Path.Combine(LibertyPaths.Root, "logs");
                        Directory.CreateDirectory(directory);
                        logDirectory = directory;
                    }
                    string logPath = Path.Combine(logDirectory, "LibertyFramework.log");
                    string backupPath = Path.Combine(logDirectory, "LibertyFramework.1.log");
                    if (File.Exists(logPath) && new FileInfo(logPath).Length >= MaximumLogBytes)
                    {
                        if (File.Exists(backupPath)) { File.Delete(backupPath); }
                        File.Move(logPath, backupPath);
                    }
                    File.AppendAllText(logPath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception error)
            {
                logDirectory = null;
                Game.Console.Print("[LibertyFramework] Log file write failed: " + error);
            }
        }
    }
}
