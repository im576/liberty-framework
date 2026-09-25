using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LibertyFramework.Core.Memory
{
    // Byte-pattern and native-table scanning over the loaded GTAIV.exe image.
    // No absolute address is hardcoded: everything is derived from code bytes at runtime,
    // which the loader has already relocated for the current process base.
    internal sealed class CodeScanner
    {
        private const uint SectionExecutable = 0x20000000;
        private readonly IMemory memory;
        private readonly List<Section> sections = new List<Section>();
        private Dictionary<uint, uint> nativeFunctions;

        private sealed class Section
        {
            internal string Name;
            internal uint Start;
            internal byte[] Bytes;
            internal bool Executable;
        }

        internal CodeScanner(IMemory memory)
        {
            this.memory = memory;
            uint moduleBase = memory.ModuleBase;
            uint peHeader = moduleBase + memory.ReadUInt32(moduleBase + 0x3C);
            if (memory.ReadUInt32(peHeader) != 0x00004550) { throw new InvalidOperationException("PE signature not found."); }
            int sectionCount = BitConverter.ToUInt16(memory.Read(peHeader + 6, 2), 0);
            int optionalSize = BitConverter.ToUInt16(memory.Read(peHeader + 20, 2), 0);
            uint table = peHeader + 24 + (uint)optionalSize;
            for (int index = 0; index < sectionCount; index++)
            {
                uint entry = table + (uint)(index * 40);
                string name = Encoding.ASCII.GetString(memory.Read(entry, 8)).TrimEnd('\0');
                uint virtualSize = memory.ReadUInt32(entry + 8);
                uint virtualAddress = memory.ReadUInt32(entry + 12);
                uint characteristics = memory.ReadUInt32(entry + 36);
                // Only scan the code and read-only data sections that the resolvers need.
                bool executable = (characteristics & SectionExecutable) != 0;
                if (!executable && name != ".rdata") { continue; }
                uint start = moduleBase + virtualAddress;
                if (!memory.IsReadable(start, (int)virtualSize)) { continue; }
                Section section = new Section();
                section.Name = name;
                section.Start = start;
                section.Bytes = memory.Read(start, (int)virtualSize);
                section.Executable = executable;
                sections.Add(section);
            }
        }

        internal IMemory Memory { get { return memory; } }

        internal List<uint> FindPattern(string pattern, bool executableOnly)
        {
            int[] expected = ParsePattern(pattern);
            List<uint> hits = new List<uint>();
            foreach (Section section in sections)
            {
                if (executableOnly && !section.Executable) { continue; }
                byte[] bytes = section.Bytes;
                int last = bytes.Length - expected.Length;
                int first = expected[0];
                for (int offset = 0; offset <= last; offset++)
                {
                    if (first >= 0 && bytes[offset] != first) { continue; }
                    if (Matches(bytes, offset, expected)) { hits.Add(section.Start + (uint)offset); }
                }
            }
            return hits;
        }

        // Checks that the code at 'address' has the expected instruction shape.
        internal bool ShapeAt(uint address, string pattern)
        {
            int[] expected = ParsePattern(pattern);
            if (!memory.IsReadable(address, expected.Length)) { return false; }
            return Matches(memory.Read(address, expected.Length), 0, expected);
        }

        internal uint FindAsciiString(string text)
        {
            byte[] needle = Encoding.ASCII.GetBytes(text + "\0");
            foreach (Section section in sections)
            {
                if (section.Executable) { continue; }
                int index = IndexOf(section.Bytes, needle, 0);
                if (index >= 0) { return section.Start + (uint)index; }
            }
            return 0;
        }

        // GTA IV registers every script native with "push handler; push hash; call register".
        internal uint FindNative(uint hash)
        {
            if (nativeFunctions == null) { BuildNativeTable(); }
            uint function;
            return nativeFunctions.TryGetValue(hash, out function) ? function : 0;
        }

        internal int NativeCount
        {
            get
            {
                if (nativeFunctions == null) { BuildNativeTable(); }
                return nativeFunctions.Count;
            }
        }

        private void BuildNativeTable()
        {
            nativeFunctions = new Dictionary<uint, uint>();
            foreach (Section section in sections)
            {
                if (!section.Executable) { continue; }
                byte[] bytes = section.Bytes;
                for (int offset = 0; offset + 15 <= bytes.Length; offset++)
                {
                    if (bytes[offset] != 0x68 || bytes[offset + 5] != 0x68 || bytes[offset + 10] != 0xE8) { continue; }
                    uint handler = BitConverter.ToUInt32(bytes, offset + 1);
                    uint hash = BitConverter.ToUInt32(bytes, offset + 6);
                    if (!nativeFunctions.ContainsKey(hash) && InExecutable(handler)) { nativeFunctions.Add(hash, handler); }
                }
            }
        }

        // Every "call rel32" (E8) in executable sections whose destination is 'target'.
        internal List<uint> FindCallsTo(uint target)
        {
            List<uint> sites = new List<uint>();
            foreach (Section section in sections)
            {
                if (!section.Executable) { continue; }
                byte[] bytes = section.Bytes;
                for (int offset = 0; offset + 5 <= bytes.Length; offset++)
                {
                    if (bytes[offset] != 0xE8) { continue; }
                    uint site = section.Start + (uint)offset;
                    if (site + 5 + (uint)BitConverter.ToInt32(bytes, offset + 1) == target) { sites.Add(site); }
                }
            }
            return sites;
        }

        internal bool InExecutable(uint address)
        {
            foreach (Section section in sections)
            {
                if (section.Executable && address >= section.Start && address < section.Start + section.Bytes.Length)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool Matches(byte[] bytes, int offset, int[] expected)
        {
            for (int index = 0; index < expected.Length; index++)
            {
                if (expected[index] >= 0 && bytes[offset + index] != expected[index]) { return false; }
            }
            return true;
        }

        private static int IndexOf(byte[] haystack, byte[] needle, int start)
        {
            int last = haystack.Length - needle.Length;
            for (int offset = start; offset <= last; offset++)
            {
                if (haystack[offset] != needle[0]) { continue; }
                bool match = true;
                for (int index = 1; index < needle.Length; index++)
                {
                    if (haystack[offset + index] != needle[index]) { match = false; break; }
                }
                if (match) { return offset; }
            }
            return -1;
        }

        private static int[] ParsePattern(string pattern)
        {
            string[] tokens = pattern.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int[] result = new int[tokens.Length];
            for (int index = 0; index < tokens.Length; index++)
            {
                result[index] = tokens[index].StartsWith("?") ? -1 :
                    int.Parse(tokens[index], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}
