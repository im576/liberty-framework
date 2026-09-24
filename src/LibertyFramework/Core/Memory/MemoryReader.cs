using System;

namespace LibertyFramework.Core.Memory
{
    internal static class MemoryReader
    {
        internal static uint ReadUInt32(this IMemory memory, uint address)
        {
            return BitConverter.ToUInt32(memory.Read(address, 4), 0);
        }

        internal static int ReadInt32(this IMemory memory, uint address)
        {
            return BitConverter.ToInt32(memory.Read(address, 4), 0);
        }

        internal static float ReadSingle(this IMemory memory, uint address)
        {
            return BitConverter.ToSingle(memory.Read(address, 4), 0);
        }

        internal static byte ReadByte(this IMemory memory, uint address)
        {
            return memory.Read(address, 1)[0];
        }

        // Returns 0 instead of throwing when the pointer target is not mapped/readable.
        internal static uint TryReadPointer(this IMemory memory, uint address)
        {
            if (address == 0 || !memory.IsReadable(address, 4)) { return 0; }
            return memory.ReadUInt32(address);
        }

        // Destination of an E8/E9 rel32 instruction located at 'address'.
        internal static uint RelativeTarget(this IMemory memory, uint address)
        {
            int displacement = memory.ReadInt32(address + 1);
            return (uint)(address + 5 + displacement);
        }
    }
}
