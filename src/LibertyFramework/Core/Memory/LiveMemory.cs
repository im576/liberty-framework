using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibertyFramework.Core.Memory
{
    // In-process access to GTAIV.exe memory. Every read/write is checked with VirtualQuery
    // first because an access violation cannot be caught by managed code on .NET 4.
    internal sealed class LiveMemory : IMemory
    {
        private const uint MemCommit = 0x1000;
        private const uint PageNoAccess = 0x01;
        private const uint PageGuard = 0x100;
        private const uint WritableMask = 0x04 | 0x08 | 0x40 | 0x80;
        private readonly uint moduleBase;
        // [start, end) of GTAIV.exe's non-executable sections (.rdata, .data, ...): mapped for the process lifetime.
        // Code sections are excluded: parts of them are encrypted on disk and may be protected differently at runtime.
        private readonly List<uint[]> imageData = new List<uint[]>();
        // T-026: VirtualQuery takes the process address-space lock and measured ~1-5 ms per call in-game while DXVK
        // and the streamer allocate. Two fast paths skip it: reads inside GTAIV.exe's own image (always mapped), and
        // ranges registered with Trust() after one full check - the engine's rage pools, allocated once at startup
        // and never freed. Everything else (heap objects such as skeletons) is still checked on every access.
        private readonly List<uint[]> trusted = new List<uint[]>();
        private readonly object trustedGate = new object();

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryBasicInformation
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr VirtualQuery(IntPtr address, out MemoryBasicInformation buffer, IntPtr length);

        internal LiveMemory()
        {
            moduleBase = (uint)Process.GetCurrentProcess().MainModule.BaseAddress.ToInt32();
            try
            {
                uint header = moduleBase + (uint)Marshal.ReadInt32(new IntPtr((int)moduleBase + 0x3C));
                int sections = Marshal.ReadInt16(new IntPtr((int)header + 6));
                int optional = Marshal.ReadInt16(new IntPtr((int)header + 20));
                for (int index = 0; index < sections; index++)
                {
                    uint entry = header + 24 + (uint)optional + (uint)(index * 40);
                    uint size = (uint)Marshal.ReadInt32(new IntPtr((int)entry + 8));
                    uint start = moduleBase + (uint)Marshal.ReadInt32(new IntPtr((int)entry + 12));
                    uint flags = (uint)Marshal.ReadInt32(new IntPtr((int)entry + 36));
                    // Readable, not executable, not discardable; verified mapped once.
                    if ((flags & 0x20000000) == 0 && (flags & 0x40000000) != 0 && (flags & 0x02000000) == 0 && size > 0 && Check(start, (int)size, false))
                    {
                        imageData.Add(new uint[] { start, start + size });
                    }
                }
            }
            catch (Exception) { imageData.Clear(); } // no fast path; every access is checked
        }

        public uint ModuleBase { get { return moduleBase; } }

        public bool IsReadable(uint address, int length)
        {
            if (length > 0)
            {
                uint end = unchecked(address + (uint)length);
                foreach (uint[] range in imageData) { if (address >= range[0] && end <= range[1] && end > address) { return true; } }
            }
            return InTrusted(address, length, false) || Check(address, length, false);
        }

        internal bool IsWritable(uint address, int length)
        {
            return InTrusted(address, length, true) || Check(address, length, true);
        }

        // Registers a permanent engine range (a rage pool's object or flag array) after one full check.
        internal bool Trust(uint address, int length, bool writable)
        {
            if (!Check(address, length, writable)) { return false; }
            lock (trustedGate)
            {
                foreach (uint[] range in trusted) { if (range[0] == address && range[1] == unchecked(address + (uint)length)) { return true; } }
                trusted.Add(new uint[] { address, unchecked(address + (uint)length), writable ? 1u : 0u });
            }
            return true;
        }

        private bool InTrusted(uint address, int length, bool write)
        {
            if (length <= 0) { return false; }
            uint end = unchecked(address + (uint)length);
            if (end < address) { return false; }
            lock (trustedGate)
            {
                foreach (uint[] range in trusted)
                {
                    if (address >= range[0] && end <= range[1] && (!write || range[2] != 0)) { return true; }
                }
            }
            return false;
        }

        private static bool Check(uint address, int length, bool write)
        {
            if (address < 0x10000 || length <= 0) { return false; }
            uint cursor = address;
            uint end = unchecked(address + (uint)length);
            // A range that wraps past 4 GB would skip the loop below and wrongly report success.
            if (end < address) { return false; }
            while (cursor < end)
            {
                MemoryBasicInformation info;
                if (VirtualQuery(new IntPtr((int)cursor), out info,
                    new IntPtr(Marshal.SizeOf(typeof(MemoryBasicInformation)))) == IntPtr.Zero)
                {
                    return false;
                }
                if (info.State != MemCommit) { return false; }
                if ((info.Protect & PageNoAccess) != 0 || (info.Protect & PageGuard) != 0) { return false; }
                if (write && (info.Protect & WritableMask) == 0) { return false; }
                uint regionEnd = (uint)info.BaseAddress.ToInt32() + (uint)info.RegionSize.ToInt32();
                if (regionEnd <= cursor) { return false; }
                cursor = regionEnd;
            }
            return true;
        }

        public byte[] Read(uint address, int length)
        {
            if (!IsReadable(address, length))
            {
                throw new InvalidOperationException("Unreadable memory at 0x" + address.ToString("X8"));
            }
            byte[] buffer = new byte[length];
            Marshal.Copy(new IntPtr((int)address), buffer, 0, length);
            return buffer;
        }

        internal void WriteSingle(uint address, float value)
        {
            Write(address, BitConverter.GetBytes(value));
        }

        internal void WriteUInt32(uint address, uint value)
        {
            Write(address, BitConverter.GetBytes(value));
        }

        internal void WriteByte(uint address, byte value)
        {
            Write(address, new byte[] { value });
        }

        private void Write(uint address, byte[] data)
        {
            if (!IsWritable(address, data.Length))
            {
                throw new InvalidOperationException("Unwritable memory at 0x" + address.ToString("X8"));
            }
            Marshal.Copy(data, 0, new IntPtr((int)address), data.Length);
        }
    }
}
