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
                TrustPermanentRanges();
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

        // Ranges allocated once for the whole session: the ped pool (objects and flags) and the CWeaponInfo array
        // (FusionFix ExtendedLimits allocates it at startup). Trusting them after one full check skips VirtualQuery
        // on every per-frame read (measured 2026-09-25: gunplay spread 2.8 ms/frame, mostly these checks).
        private void TrustPermanentRanges()
        {
            try
            {
                if (Addresses.PedPoolGlobal != 0)
                {
                    uint pool = Live.TryReadPointer(Addresses.PedPoolGlobal);
                    if (pool != 0 && Live.IsReadable(pool, 16))
                    {
                        uint objects = Live.ReadUInt32(pool), flags = Live.ReadUInt32(pool + 4);
                        int size = Live.ReadInt32(pool + 8), itemSize = Live.ReadInt32(pool + 12);
                        bool ok = size > 0 && size <= 4096 && itemSize >= 0x100 && itemSize <= 0x4000 &&
                            Live.Trust(objects, size * itemSize, true) && Live.Trust(flags, size, false);
                        RuntimeLog.Info("engine_memory_trust ped_pool=" + ok + " size=" + size + " item=0x" + itemSize.ToString("X"));
                    }
                }
                if (Addresses.WeaponInfoResolved)
                {
                    int entries = Addresses.WeaponInfoCount > 0 ? Addresses.WeaponInfoCount : 128;
                    bool ok = Live.Trust(Addresses.WeaponInfoArray, entries * Addresses.WeaponInfoStride, true);
                    RuntimeLog.Info("engine_memory_trust weapon_info=" + ok + " entries=" + entries);
                }
            }
            catch (Exception error) { RuntimeLog.Error("engine_memory_trust_failed error=" + error.Message); }
        }
    }
}