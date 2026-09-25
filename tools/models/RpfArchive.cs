using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // Read-only RPF2 archive (e.g. pc/models/cdimages/playerped.rpf). Header: magic "RPF2", TOC size, entry count,
    // unused, encrypted flag. The TOC sits at 0x800 (AES-encrypted with the same key as the IMG archives): 16-byte
    // entries, then the name table. Resource entries store "offset | resource type" and the RSC flags; their data
    // is a complete RSC05 file. Directories are flattened; only file names are kept.
    internal sealed class RpfArchive
    {
        private readonly string path;
        private readonly Dictionary<string, KeyValuePair<long, int>> files = new Dictionary<string, KeyValuePair<long, int>>(StringComparer.OrdinalIgnoreCase);

        internal IEnumerable<string> Names { get { return files.Keys; } }

        private RpfArchive(string path) { this.path = path; }

        internal static RpfArchive Open(string path, byte[] aesKey)
        {
            RpfArchive archive = new RpfArchive(path);
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] header = new byte[0x14];
                if (stream.Read(header, 0, header.Length) != header.Length || BitConverter.ToUInt32(header, 0) != 0x32465052)
                    { throw new InvalidDataException(path + " is not an RPF2 archive"); }
                int tocSize = BitConverter.ToInt32(header, 4), count = BitConverter.ToInt32(header, 8);
                byte[] toc = new byte[tocSize];
                stream.Position = 0x800;
                if (stream.Read(toc, 0, tocSize) != tocSize) { throw new InvalidDataException("truncated RPF TOC"); }
                if (BitConverter.ToUInt32(header, 0x10) != 0) { toc = ImgArchive.Decrypt(toc, aesKey); }
                int names = count * 16;
                for (int i = 0; i < count; i++)
                {
                    uint nameOffset = BitConverter.ToUInt32(toc, i * 16);
                    uint second = BitConverter.ToUInt32(toc, i * 16 + 4);
                    uint third = BitConverter.ToUInt32(toc, i * 16 + 8);
                    // Directories carry 0x80000000 in the third word (flags | first child).
                    if ((third & 0x80000000) != 0) { continue; }
                    int start = names + (int)(nameOffset & 0x7FFFFFFF);
                    int end = Array.IndexOf(toc, (byte)0, start);
                    string name = Encoding.ASCII.GetString(toc, start, end - start);
                    archive.files[name] = new KeyValuePair<long, int>(third & 0xFFFFFF00, (int)second);
                }
            }
            return archive;
        }

        internal byte[] Extract(string name)
        {
            KeyValuePair<long, int> entry;
            if (!files.TryGetValue(name, out entry)) { throw new FileNotFoundException(name + " not in " + path); }
            using (FileStream stream = File.OpenRead(path))
            {
                byte[] data = new byte[entry.Value];
                stream.Position = entry.Key;
                if (stream.Read(data, 0, data.Length) != data.Length) { throw new InvalidDataException("truncated entry " + name); }
                return data;
            }
        }
    }
}
