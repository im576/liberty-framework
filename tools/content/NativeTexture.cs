using System.Collections.Generic;

namespace LibertyFramework.Content
{
    // One texture ready for TextureDictionaryWriter: its name (without "pack:/" and ".dds"), DXT format and the encoded
    // bytes of every mip level, largest first.
    internal sealed class NativeTexture
    {
        internal string Name;
        internal string Format;
        internal int Width;
        internal int Height;
        internal readonly List<byte[]> Levels = new List<byte[]>();

        internal int TotalBytes
        {
            get { int total = 0; foreach (byte[] level in Levels) { total += level.Length; } return total; }
        }
    }
}
