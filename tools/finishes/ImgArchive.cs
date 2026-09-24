using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LibertyFramework.Finishes
{
    // GTA IV IMG version 3 archive: read (AES-encrypted table) and write (unencrypted, which the
    // game and FusionFix's IMG loader both accept). Entry layout matches FusionFix imgloader.ixx.
    internal sealed class ImgArchive
    {
        private const uint Magic = 0xA94E2A52;
        private const int BlockSize = 2048;

        internal sealed class Entry
        {
            internal string Name;
            internal uint SizeOrFlags;
            internal uint ResourceType;
            internal uint OffsetBlocks;
            internal ushort UsedBlocks;
            internal ushort Flags;
        }

        internal readonly List<Entry> Entries = new List<Entry>();
        private readonly string path;

        private ImgArchive(string path)
        {
            this.path = path;
        }

        internal static ImgArchive Open(string path, byte[] aesKey)
        {
            ImgArchive archive = new ImgArchive(path);
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] header = new byte[32];
                stream.Read(header, 0, 32);
                bool encrypted = BitConverter.ToUInt32(header, 0) != Magic;
                if (encrypted) { header = Decrypt(header, aesKey); }
                if (BitConverter.ToUInt32(header, 0) != Magic) { throw new InvalidDataException("Not an IMG v3 archive: " + path); }
                int count = BitConverter.ToInt32(header, 8);
                int tableSize = BitConverter.ToInt32(header, 12);
                byte[] table = new byte[tableSize];
                stream.Position = 20;
                stream.Read(table, 0, tableSize);
                if (encrypted) { table = Decrypt(table, aesKey); }
                int nameOffset = count * 16;
                for (int index = 0; index < count; index++)
                {
                    Entry entry = new Entry();
                    entry.SizeOrFlags = BitConverter.ToUInt32(table, index * 16);
                    entry.ResourceType = BitConverter.ToUInt32(table, index * 16 + 4);
                    entry.OffsetBlocks = BitConverter.ToUInt32(table, index * 16 + 8);
                    entry.UsedBlocks = BitConverter.ToUInt16(table, index * 16 + 12);
                    entry.Flags = BitConverter.ToUInt16(table, index * 16 + 14);
                    int end = Array.IndexOf(table, (byte)0, nameOffset);
                    entry.Name = Encoding.ASCII.GetString(table, nameOffset, end - nameOffset);
                    nameOffset = end + 1;
                    archive.Entries.Add(entry);
                }
            }
            return archive;
        }

        internal byte[] Extract(string name)
        {
            foreach (Entry entry in Entries)
            {
                if (!string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase)) { continue; }
                int length = entry.UsedBlocks * BlockSize - ((entry.Flags & 0x2000) != 0 ? (entry.Flags & 0x7FF) : 0);
                if ((entry.Flags & 0x2000) == 0) { length = (int)entry.SizeOrFlags; }
                byte[] data = new byte[length];
                using (FileStream stream = File.OpenRead(path))
                {
                    stream.Position = (long)entry.OffsetBlocks * BlockSize;
                    int read = 0;
                    while (read < length)
                    {
                        int count = stream.Read(data, read, length - read);
                        if (count == 0) { throw new EndOfStreamException(name); }
                        read += count;
                    }
                }
                return data;
            }
            throw new FileNotFoundException(name + " not found in " + path);
        }

        internal static void Write(string outputPath, IList<KeyValuePair<string, byte[]>> files)
        {
            int nameBytes = 0;
            foreach (KeyValuePair<string, byte[]> file in files) { nameBytes += file.Key.Length + 1; }
            int tocSize = 20 + files.Count * 16 + nameBytes + 32;
            int firstBlock = (tocSize + BlockSize - 1) / BlockSize;
            using (MemoryStream output = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(Magic);
                writer.Write(3);
                writer.Write(files.Count);
                writer.Write(files.Count * 16 + nameBytes);
                writer.Write((ushort)16);
                writer.Write((ushort)0x00E9);
                int block = firstBlock;
                foreach (KeyValuePair<string, byte[]> file in files)
                {
                    byte[] data = file.Value;
                    int blocks = (data.Length + BlockSize - 1) / BlockSize;
                    bool resource = data.Length >= 12 && BitConverter.ToUInt32(data, 0) == 0x05435352;
                    writer.Write(resource ? BitConverter.ToUInt32(data, 8) : (uint)data.Length);
                    writer.Write(resource ? BitConverter.ToUInt32(data, 4) : 0u);
                    writer.Write(block);
                    writer.Write((ushort)blocks);
                    writer.Write(resource ? (ushort)(((blocks * BlockSize - data.Length) & 0x7FF) | 0x2000) : (ushort)0);
                    block += blocks;
                }
                foreach (KeyValuePair<string, byte[]> file in files)
                {
                    writer.Write(Encoding.ASCII.GetBytes(file.Key));
                    writer.Write((byte)0);
                }
                output.SetLength((long)firstBlock * BlockSize);
                output.Position = output.Length;
                foreach (KeyValuePair<string, byte[]> file in files)
                {
                    writer.Write(file.Value);
                    long padded = (output.Length + BlockSize - 1) / BlockSize * BlockSize;
                    output.SetLength(padded);
                    output.Position = padded;
                }
                File.WriteAllBytes(outputPath, output.ToArray());
            }
        }

        // GTA IV encrypts IMG tables with AES-256-ECB applied 16 times; the key lives in GTAIV.exe.
        internal static byte[] Decrypt(byte[] data, byte[] key)
        {
            byte[] result = (byte[])data.Clone();
            int length = data.Length / 16 * 16;
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                aes.KeySize = 256;
                aes.Key = key;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    for (int round = 0; round < 16; round++)
                    {
                        decryptor.TransformBlock(result, 0, length, result, 0);
                    }
                }
            }
            return result;
        }

        // Finds the IMG key inside the user's own GTAIV.exe by its well-known SHA-1 fingerprint.
        internal static byte[] FindKey(string exePath)
        {
            byte[] exe = File.ReadAllBytes(exePath);
            byte[] expected = HexToBytes("DEA375EF1E6EF2223A1221C2C575C47BF17EFA5E");
            using (SHA1 sha = SHA1.Create())
            {
                for (int offset = 0; offset + 32 <= exe.Length; offset += 4)
                {
                    byte[] hash = sha.ComputeHash(exe, offset, 32);
                    bool match = true;
                    for (int index = 0; index < hash.Length && match; index++) { match = hash[index] == expected[index]; }
                    if (match)
                    {
                        byte[] key = new byte[32];
                        Array.Copy(exe, offset, key, 0, 32);
                        return key;
                    }
                }
            }
            throw new InvalidDataException("IMG key not found in " + exePath);
        }

        private static byte[] HexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int index = 0; index < bytes.Length; index++) { bytes[index] = Convert.ToByte(hex.Substring(index * 2, 2), 16); }
            return bytes;
        }
    }
}
