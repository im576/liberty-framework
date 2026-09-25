using System;

namespace LibertyFramework.Core.Logging
{
    // Offline stand-in for the in-game logger (which depends on ScriptHookDotNet paths).
    internal static class RuntimeLog
    {
        internal static void Info(string message) { Console.WriteLine("    log: " + message); }
        internal static void Error(string message) { Console.WriteLine("    log error: " + message); }
    }
}
