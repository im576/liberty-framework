using System;
using System.IO;
using LibertyFramework.Core.Memory;

namespace LibertyFramework.Verify
{
    // Maps GTAIV.exe from disk at its preferred image base, exactly as the loader would
    // (without applying relocations, which are unnecessary at the preferred base).
    internal sealed class ImageFileMemory : IMemory
    {
        private readonly byte[] image;
        private readonly uint imageBase;

        internal ImageFileMemory(string path)
        {
            byte[] file = File.ReadAllBytes(path);
            int pe = BitConverter.ToInt32(file, 0x3C);
            int sections = BitConverter.ToUInt16(file, pe + 6);
            int optionalSize = BitConverter.ToUInt16(file, pe + 20);
            int optional = pe + 24;
            imageBase = BitConverter.ToUInt32(file, optional + 28);
            int sizeOfImage = BitConverter.ToInt32(file, optional + 56);
            int sizeOfHeaders = BitConverter.ToInt32(file, optional + 60);
            image = new byte[sizeOfImage];
            Array.Copy(file, 0, image, 0, sizeOfHeaders);
            for (int index = 0; index < sections; index++)
            {
                int entry = optional + optionalSize + index * 40;
                int virtualAddress = BitConverter.ToInt32(file, entry + 12);
                int rawSize = BitConverter.ToInt32(file, entry + 16);
                int rawPointer = BitConverter.ToInt32(file, entry + 20);
                int count = Math.Min(rawSize, image.Length - virtualAddress);
                if (count > 0 && rawPointer + count <= file.Length) { Array.Copy(file, rawPointer, image, virtualAddress, count); }
            }
        }

        public uint ModuleBase { get { return imageBase; } }

        public bool IsReadable(uint address, int length)
        {
            return address >= imageBase && (long)address - imageBase + length <= image.Length;
        }

        // Simulates a runtime patch (e.g. FusionFix NOPing a branch) for resolver robustness checks.
        internal void Patch(uint address, byte[] bytes)
        {
            Array.Copy(bytes, 0, image, (int)(address - imageBase), bytes.Length);
        }

        public byte[] Read(uint address, int length)
        {
            if (!IsReadable(address, length)) { throw new InvalidOperationException("Outside image: 0x" + address.ToString("X8")); }
            byte[] buffer = new byte[length];
            Array.Copy(image, (int)(address - imageBase), buffer, 0, length);
            return buffer;
        }
    }
}
