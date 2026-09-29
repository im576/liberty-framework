using System;
using System.IO;

namespace LibertyFramework.Content
{
    internal static class WbdDictionarySelfTest
    {
        internal static void Run(SelfTest.Runner t, string output)
        {
            byte[] body = new byte[0x400];
            Put32(body, 0, WbdDictionaryReader.CeVtable);
            Put32(body, 0x10, 0x50000080); Put16(body, 0x14, 2); Put16(body, 0x16, 2);
            Put32(body, 0x18, 0x50000090); Put16(body, 0x1C, 2); Put16(body, 0x1E, 2);
            Put32(body, 0x80, 0x12345678); Put32(body, 0x84, 0xABCDEF01);
            Put32(body, 0x90, 0x50000100); Put32(body, 0x94, 0x50000200);
            Put32(body, 0x100, 0x0069C19C); Put32(body, 0x200, 0x0069AAF4);
            WbdDictionaryReader reader = WbdDictionaryReader.Parse(body, body.Length, 32);
            t.Check(reader.Entries.Count == 2 && reader.HashOffset == 0x80 && reader.TargetOffset == 0x90,
                "two equal-length arrays read from root pointers and count fields");
            t.Check(reader.Entries[0].NameHash == 0x12345678 && reader.Entries[1].TargetPointer == 0x50000200 &&
                reader.Entries[1].TargetVtable == 0x0069AAF4, "hashes pair by index with valid target structures");
            t.Check(reader.TableRoundTrip(), "decoded root fields and arrays reproduce the source bytes");
            reader.Entries[0].NameHash ^= 1;
            t.Check(!reader.TableRoundTrip(), "a changed hash fails the table roundtrip");
            reader.Entries[0].NameHash ^= 1;
            reader.Entries[1].TargetPointer += 4;
            t.Check(!reader.TableRoundTrip(), "a changed target pointer fails the table roundtrip");
            byte[] badCount = (byte[])body.Clone(); Put16(badCount, 0x1C, 1);
            t.Throws<InvalidDataException>(() => WbdDictionaryReader.Parse(badCount, badCount.Length, 32),
                "unequal root counts are refused");
            byte[] badPointer = (byte[])body.Clone(); Put32(badPointer, 0x94, 0x50001000);
            t.Throws<InvalidDataException>(() => WbdDictionaryReader.Parse(badPointer, badPointer.Length, 32),
                "out-of-range target is refused");
        }

        private static void Put16(byte[] body, int at, ushort value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, body, at, 2); }
        private static void Put32(byte[] body, int at, uint value) { Buffer.BlockCopy(BitConverter.GetBytes(value), 0, body, at, 4); }
    }
}
