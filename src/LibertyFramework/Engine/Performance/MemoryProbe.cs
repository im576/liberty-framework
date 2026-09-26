using System;
using System.Runtime.InteropServices;

namespace LibertyFramework.Engine.Performance
{
    // Process memory figures that matter for a 32-bit game on an 8 GB machine: private bytes, free address space and
    // the largest free address-space block (fragmentation decides whether big streaming allocations still fit).
    // Sampled on the watchdog thread; VirtualQuery and the counters are safe off the game thread.
    internal static class MemoryProbe
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryStatusEx
        {
            internal uint Length;
            internal uint MemoryLoad;
            internal ulong TotalPhys, AvailPhys, TotalPageFile, AvailPageFile, TotalVirtual, AvailVirtual, AvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ProcessMemoryCounters
        {
            internal uint Size;
            internal uint PageFaultCount;
            internal UIntPtr PeakWorkingSetSize, WorkingSetSize, QuotaPeakPagedPoolUsage, QuotaPagedPoolUsage,
                QuotaPeakNonPagedPoolUsage, QuotaNonPagedPoolUsage, PagefileUsage, PeakPagefileUsage, PrivateUsage;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryBasicInformation
        {
            internal IntPtr BaseAddress, AllocationBase;
            internal uint AllocationProtect;
            internal IntPtr RegionSize;
            internal uint State, Protect, Type;
        }

        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx status);
        [DllImport("kernel32.dll", EntryPoint = "K32GetProcessMemoryInfo")] private static extern bool GetProcessMemoryInfo(IntPtr process, ref ProcessMemoryCounters counters, uint size);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")] private static extern UIntPtr VirtualQuery(IntPtr address, out MemoryBasicInformation info, UIntPtr length);

        private const uint MemFree = 0x10000;

        internal struct Sample
        {
            internal long PrivateBytes;
            internal long WorkingSetBytes;
            internal long AddressSpaceFreeBytes;
            internal long LargestFreeBlockBytes;
            internal int PhysicalLoadPercent;
        }

        internal static Sample Take(bool walkAddressSpace, long previousLargest)
        {
            Sample sample = new Sample();
            MemoryStatusEx status = new MemoryStatusEx();
            status.Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
            if (GlobalMemoryStatusEx(ref status))
            {
                sample.AddressSpaceFreeBytes = (long)status.AvailVirtual;
                sample.PhysicalLoadPercent = (int)status.MemoryLoad;
            }
            ProcessMemoryCounters counters = new ProcessMemoryCounters();
            counters.Size = (uint)Marshal.SizeOf(typeof(ProcessMemoryCounters));
            if (GetProcessMemoryInfo(GetCurrentProcess(), ref counters, counters.Size))
            {
                sample.PrivateBytes = (long)counters.PrivateUsage.ToUInt64();
                sample.WorkingSetBytes = (long)counters.WorkingSetSize.ToUInt64();
            }
            sample.LargestFreeBlockBytes = walkAddressSpace ? LargestFreeBlock() : previousLargest;
            return sample;
        }

        // Walks the user address space (a few thousand regions; about a millisecond).
        private static long LargestFreeBlock()
        {
            long largest = 0;
            long address = 0x10000;
            const long limit = 0xFFFF0000L;
            MemoryBasicInformation info;
            UIntPtr size = new UIntPtr((uint)Marshal.SizeOf(typeof(MemoryBasicInformation)));
            while (address < limit)
            {
                if (VirtualQuery(new IntPtr(unchecked((int)(uint)address)), out info, size) == UIntPtr.Zero) { break; }
                long region = (long)(uint)info.RegionSize.ToInt32();
                if (region <= 0) { break; }
                if (info.State == MemFree && region > largest) { largest = region; }
                address = (long)(uint)info.BaseAddress.ToInt32() + region;
            }
            return largest;
        }
    }
}
