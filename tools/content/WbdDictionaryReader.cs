using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // Read-only CE WBD root/table reader. This decodes the two measured parallel arrays,
    // not the pointed-to bounds shapes or the other root fields.
    internal sealed class WbdDictionaryReader
    {
        internal const uint CeVtable = 0x00695360;
        internal sealed class Entry
        {
            internal uint NameHash;
            internal uint TargetPointer;
            internal uint TargetVtable;
        }

        internal readonly List<Entry> Entries = new List<Entry>();
        internal int HashOffset, TargetOffset, HashCapacity, TargetCapacity;
        internal uint HashPointer, TargetArrayPointer;
        private byte[] body;

        internal static WbdDictionaryReader Parse(RscResource resource)
        {
            return Parse(resource.Body, resource.SystemSize, resource.Type);
        }

        internal static WbdDictionaryReader Parse(byte[] body, int systemSize, uint type)
        {
            if (type != 32 || systemSize < 0x20 || body.Length < systemSize || BitConverter.ToUInt32(body, 0) != CeVtable)
            { throw new InvalidDataException("not CE WBD root class 0x00695360"); }
            WbdDictionaryReader result = new WbdDictionaryReader { body = body };
            result.HashPointer = BitConverter.ToUInt32(body, 0x10);
            result.TargetArrayPointer = BitConverter.ToUInt32(body, 0x18);
            int hashCount = BitConverter.ToUInt16(body, 0x14);
            result.HashCapacity = BitConverter.ToUInt16(body, 0x16);
            int boundsCount = BitConverter.ToUInt16(body, 0x1C);
            result.TargetCapacity = BitConverter.ToUInt16(body, 0x1E);
            if (hashCount != boundsCount || hashCount > result.HashCapacity || boundsCount > result.TargetCapacity)
            { throw new InvalidDataException("WBD parallel table counts/capacities disagree"); }
            if (hashCount > 16384) { throw new InvalidDataException("WBD table count too large"); }
            if (hashCount == 0) { return result; }
            result.HashOffset = SystemOffset(result.HashPointer, systemSize, hashCount * 4);
            result.TargetOffset = SystemOffset(result.TargetArrayPointer, systemSize, boundsCount * 4);
            for (int i = 0; i < hashCount; i++)
            {
                uint pointer = BitConverter.ToUInt32(body, result.TargetOffset + i * 4);
                int target = SystemOffset(pointer, systemSize, 4);
                result.Entries.Add(new Entry {
                    NameHash = BitConverter.ToUInt32(body, result.HashOffset + i * 4),
                    TargetPointer = pointer,
                    TargetVtable = BitConverter.ToUInt32(body, target)
                });
            }
            return result;
        }

        private static int SystemOffset(uint pointer, int size, int bytes)
        {
            if ((pointer & 0xF0000000) != 0x50000000) { throw new InvalidDataException("WBD table/target is not a system pointer"); }
            long offset = pointer - 0x50000000;
            if (offset + bytes > size) { throw new InvalidDataException("WBD table/target pointer out of range"); }
            return (int)offset;
        }

        // Encode only the fields this reader understands into fresh buffers and compare every byte
        // of both arrays and the root's pointer/count records. Unknown resource bytes are excluded.
        internal bool TableRoundTrip()
        {
            byte[] hashes = new byte[Entries.Count * 4], bounds = new byte[Entries.Count * 4];
            for (int i = 0; i < Entries.Count; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Entries[i].NameHash), 0, hashes, i * 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(Entries[i].TargetPointer), 0, bounds, i * 4, 4);
            }
            byte[] root = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(HashPointer), 0, root, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)Entries.Count), 0, root, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)HashCapacity), 0, root, 6, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(TargetArrayPointer), 0, root, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)Entries.Count), 0, root, 12, 2);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)TargetCapacity), 0, root, 14, 2);
            return Equal(body, 0x10, root) && Equal(body, HashOffset, hashes) && Equal(body, TargetOffset, bounds);
        }

        private static bool Equal(byte[] body, int at, byte[] expected)
        {
            return expected.Length <= body.Length - at && !expected.Where((value, i) => value != body[at + i]).Any();
        }
    }
}
