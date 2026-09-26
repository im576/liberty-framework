using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IMemory for privileged modules. Reads need Capabilities.EngineInternal or MemoryPatch; writes need MemoryPatch.
    // Every patch records the original bytes in the resource ledger: restored when the module stops, and on script
    // unload (memory writes need no game functions, so they are safe there). Addresses come from FindPattern /
    // FindNative (signatures), never constants (AGENTS.md rule 6).
    public sealed class MemoryService : IMemory
    {
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualProtect(IntPtr address, UIntPtr size, uint protect, out uint old);
        [DllImport("kernel32.dll")] private static extern bool FlushInstructionCache(IntPtr process, IntPtr address, UIntPtr size);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();

        private const uint PageExecuteReadWrite = 0x40;
        private readonly LibertyEngine engine;
        private string version;

        internal MemoryService(LibertyEngine engine) { this.engine = engine; }

        private void RequireRead()
        {
            LibertyModule caller = engine.CurrentModule;
            if (caller == null) { return; }
            if (!caller.Manifest.Has(Capabilities.EngineInternal) && !caller.Manifest.Has(Capabilities.MemoryPatch))
            {
                throw new UnauthorizedAccessException("module " + caller.Id + " must declare " + Capabilities.MemoryPatch + " to use IMemory");
            }
        }

        private EngineMemory Resolved()
        {
            if (!engine.Memory.Resolved) { throw new InvalidOperationException("game memory is not resolved in this session"); }
            return engine.Memory;
        }

        public uint FindPattern(string pattern)
        {
            RequireRead();
            List<uint> matches = Resolved().Scanner.FindPattern(pattern, true);
            return matches.Count > 0 ? matches[0] : 0;
        }

        public uint FindNative(uint hash) { RequireRead(); return Resolved().Scanner.FindNative(hash); }

        public bool IsReadable(uint address, int length) { RequireRead(); return Resolved().Live.IsReadable(address, length); }

        public byte[] Read(uint address, int length)
        {
            RequireRead();
            if (!Resolved().Live.IsReadable(address, length)) { throw new ArgumentException("unreadable range 0x" + address.ToString("X8")); }
            return engine.Memory.Live.Read(address, length);
        }

        public int ReadInt32(uint address) { return BitConverter.ToInt32(Read(address, 4), 0); }

        public float ReadSingle(uint address) { return BitConverter.ToSingle(Read(address, 4), 0); }

        // Live patches: [address, address + length) per owner. Two modules may not patch overlapping bytes: each would save
        // the other's bytes as "original", and whichever stopped second would write a stale patch back into the game.
        private sealed class PatchRange { internal LibertyModule Owner; internal uint Address; internal int Length; }
        private readonly List<PatchRange> patches = new List<PatchRange>();

        public bool Patch(LibertyModule owner, uint address, byte[] bytes)
        {
            engine.RequireCapability(owner, Capabilities.MemoryPatch);
            if (bytes == null || bytes.Length == 0 || !Resolved().Live.IsReadable(address, bytes.Length)) { return false; }
            PatchRange held = patches.Find(p => p.Owner != owner && Overlaps(p, address, bytes.Length));
            if (held != null)
            {
                RuntimeLog.Error("[" + owner.Id + "] memory_patch_refused address=0x" + address.ToString("X8") + " bytes=" + bytes.Length +
                    " overlaps=" + held.Owner.Id + "@0x" + held.Address.ToString("X8") + "+" + held.Length);
                return false;
            }
            byte[] original = engine.Memory.Live.Read(address, bytes.Length);
            if (!Write(address, bytes)) { return false; }
            PatchRange range = new PatchRange { Owner = owner, Address = address, Length = bytes.Length };
            patches.Add(range);
            engine.Ledger.Add(owner, "patch", address, () => { patches.Remove(range); Write(address, original); });
            RuntimeLog.Info("[" + owner.Id + "] memory_patch address=0x" + address.ToString("X8") + " bytes=" + bytes.Length);
            return true;
        }

        public void RestoreAll(LibertyModule owner) { engine.Ledger.ReleaseKind(owner, "patch"); }

        private static bool Overlaps(PatchRange p, uint address, int length)
        {
            ulong start = address, end = (ulong)address + (ulong)length, heldStart = p.Address, heldEnd = (ulong)p.Address + (ulong)p.Length;
            return start < heldEnd && heldStart < end;
        }

        internal static bool Write(uint address, byte[] bytes)
        {
            IntPtr target = new IntPtr(unchecked((int)address));
            UIntPtr size = new UIntPtr((uint)bytes.Length);
            uint old;
            if (!VirtualProtect(target, size, PageExecuteReadWrite, out old))
            {
                RuntimeLog.Error("memory_protect_failed address=0x" + address.ToString("X8") + " error=" + Marshal.GetLastWin32Error());
                return false;
            }
            Marshal.Copy(bytes, 0, target, bytes.Length);
            uint ignored;
            VirtualProtect(target, size, old, out ignored);
            FlushInstructionCache(GetCurrentProcess(), target, size);
            return true;
        }

        public string GameVersion
        {
            get
            {
                if (version == null)
                {
                    using (Process process = Process.GetCurrentProcess())
                    {
                        FileVersionInfo info = process.MainModule.FileVersionInfo;
                        version = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
                    }
                }
                return version;
            }
        }
    }
}
