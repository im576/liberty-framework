using System;
using System.IO;
using System.IO.Compression;

namespace LibertyFramework.Finishes
{
    // RAGE resource (RSC05): 12-byte header (magic, type, flags) + zlib stream holding the
    // system segment followed by the graphics segment. Flags encode both segment sizes.
    internal sealed class RscResource
    {
        internal uint Type;
        internal uint Flags;
        internal byte[] Body;

        internal int SystemSize { get { return (int)((Flags & 0x7FF) << (int)(((Flags >> 11) & 0xF) + 8)); } }
        internal int GraphicsSize { get { return (int)(((Flags >> 15) & 0x7FF) << (int)(((Flags >> 26) & 0xF) + 8)); } }

        internal static RscResource Parse(byte[] data)
        {
            if (BitConverter.ToUInt32(data, 0) != 0x05435352) { throw new InvalidDataException("Not an RSC05 resource"); }
            RscResource resource = new RscResource();
            resource.Type = BitConverter.ToUInt32(data, 4);
            resource.Flags = BitConverter.ToUInt32(data, 8);
            // Skip the 2-byte zlib header; DeflateStream reads raw deflate.
            using (MemoryStream input = new MemoryStream(data, 14, data.Length - 14))
            using (DeflateStream inflater = new DeflateStream(input, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                inflater.CopyTo(output);
                resource.Body = output.ToArray();
            }
            if (resource.Body.Length != resource.SystemSize + resource.GraphicsSize)
            {
                throw new InvalidDataException("RSC body " + resource.Body.Length + " != " + (resource.SystemSize + resource.GraphicsSize));
            }
            return resource;
        }

        internal byte[] Serialize()
        {
            using (MemoryStream output = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(output);
                writer.Write(0x05435352u);
                writer.Write(Type);
                writer.Write(Flags);
                writer.Write((byte)0x78);
                writer.Write((byte)0xDA);
                using (DeflateStream deflater = new DeflateStream(output, CompressionMode.Compress, true))
                {
                    deflater.Write(Body, 0, Body.Length);
                }
                uint adler = Adler32(Body);
                writer.Write((byte)(adler >> 24));
                writer.Write((byte)(adler >> 16));
                writer.Write((byte)(adler >> 8));
                writer.Write((byte)adler);
                writer.Flush();
                return output.ToArray();
            }
        }

        private static uint Adler32(byte[] data)
        {
            uint a = 1;
            uint b = 0;
            foreach (byte value in data)
            {
                a = (a + value) % 65521;
                b = (b + a) % 65521;
            }
            return (b << 16) | a;
        }
    }
}
