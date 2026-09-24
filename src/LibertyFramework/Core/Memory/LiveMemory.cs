using System;
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
        }

        public uint ModuleBase { get { return moduleBase; } }

        public bool IsReadable(uint address, int length)
        {
            return Check(address, length, false);
        }

        internal bool IsWritable(uint address, int length)
        {
            return Check(address, length, true);
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
