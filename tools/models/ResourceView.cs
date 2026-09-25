using System;
using System.IO;
using System.Text;
using LibertyFramework.Finishes;

namespace LibertyFramework.Models
{
    // Address view over a decompressed RSC05 body. RAGE stores pointers as segment addresses:
    // 0x5xxxxxxx = system (virtual) segment, 0x6xxxxxxx = graphics (physical) segment; the body holds the
    // system segment first, then the graphics segment. Every read is bounds-checked so a malformed file
    // fails loudly instead of producing garbage geometry.
    internal sealed class ResourceView
    {
        internal const uint SystemBase = 0x50000000;
        internal const uint GraphicsBase = 0x60000000;

        internal readonly byte[] Body;
        internal readonly int SystemSize;
        internal readonly int GraphicsSize;

        internal ResourceView(RscResource resource)
        {
            Body = resource.Body;
            SystemSize = resource.SystemSize;
            GraphicsSize = resource.GraphicsSize;
        }

        internal static bool IsSystem(uint pointer) { return (pointer & 0xF0000000) == SystemBase; }
        internal static bool IsGraphics(uint pointer) { return (pointer & 0xF0000000) == GraphicsBase; }

        internal int Offset(uint pointer, int length)
        {
            int offset;
            if (IsSystem(pointer))
            {
                offset = (int)(pointer - SystemBase);
                if (offset < 0 || offset + length > SystemSize) { throw Bad("system pointer 0x" + pointer.ToString("X8") + " +" + length); }
            }
            else if (IsGraphics(pointer))
            {
                int local = (int)(pointer - GraphicsBase);
                if (local < 0 || local + length > GraphicsSize) { throw Bad("graphics pointer 0x" + pointer.ToString("X8") + " +" + length); }
                offset = SystemSize + local;
            }
            else { throw Bad("not a resource pointer: 0x" + pointer.ToString("X8")); }
            return offset;
        }

        internal uint U32(uint pointer) { return BitConverter.ToUInt32(Body, Offset(pointer, 4)); }
        internal ushort U16(uint pointer) { return BitConverter.ToUInt16(Body, Offset(pointer, 2)); }
        internal byte U8(uint pointer) { return Body[Offset(pointer, 1)]; }
        internal float F32(uint pointer) { return BitConverter.ToSingle(Body, Offset(pointer, 4)); }
        internal ulong U64(uint pointer) { return BitConverter.ToUInt64(Body, Offset(pointer, 8)); }

        internal string CString(uint pointer)
        {
            int start = Offset(pointer, 1);
            int end = start;
            while (end < SystemSize && Body[end] != 0) { end++; }
            if (end >= SystemSize) { throw Bad("unterminated string at 0x" + pointer.ToString("X8")); }
            return Encoding.ASCII.GetString(Body, start, end - start);
        }

        // Length of the string slot: the terminator plus RAGE's 0xCD allocation fill that follows it.
        internal int StringCapacity(uint pointer)
        {
            int start = Offset(pointer, 1);
            int end = start;
            while (Body[end] != 0) { end++; }
            end++;
            while (end < SystemSize && Body[end] == 0xCD && (end & 0xF) != 0) { end++; }
            return end - start;
        }

        internal static InvalidDataException Bad(string message) { return new InvalidDataException("drawable: " + message); }
    }
}
