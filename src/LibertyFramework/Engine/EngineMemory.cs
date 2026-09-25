using System;
using System.Diagnostics;
using LibertyFramework.Core.Logging;
using LibertyFramework.Core.Memory;
using LibertyFramework.GameApi;

namespace LibertyFramework.Engine
{
    // The one scan of GTAIV.exe per session: verified addresses (GameAddresses, MEMORY.md) and the native table,
    // shared by the core and every module. Resolved on the engine's first frame.
    public sealed class EngineMemory
    {
        internal LiveMemory Live { get; private set; }
        internal CodeScanner Scanner { get; private set; }
        internal GameAddresses Addresses { get; private set; }
        public bool Resolved { get; private set; }

        internal bool Resolve()
        {
            if (Resolved) { return true; }
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                Live = new LiveMemory();
                Scanner = new CodeScanner(Live);
                Addresses = GameAddresses.Resolve(Scanner);
                try { DirectNatives.Initialize(Scanner); }
                catch (Exception error) { RuntimeLog.Error("direct_natives_unavailable error=" + error.Message); }
                Resolved = true;
                RuntimeLog.Info("engine_resolve module_base=0x" + Live.ModuleBase.ToString("X8") + " natives=" + Scanner.NativeCount +
                    " elapsed_ms=" + timer.ElapsedMilliseconds);
                foreach (string line in Addresses.Report) { RuntimeLog.Info("engine_resolve " + line); }
            }
            catch (Exception error)
            {
                RuntimeLog.Error("engine_resolve_failed engine features that need memory are disabled error=" + error);
            }
            return Resolved;
        }
    }
}