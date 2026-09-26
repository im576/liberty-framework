namespace LibertyFramework.Content
{
    // The key a texture dictionary sorts and looks up its textures by: Jenkins one-at-a-time over the lower-case texture
    // name (no "pack:/" prefix, no ".dds"). Established offline: it reproduces the stored hash of every texture in the
    // game's own dictionaries that `LibertyContent wtdcheck` reads (docs/research/ModelFormat.md, texture dictionary).
    internal static class TextureNameHash
    {
        internal static uint Compute(string name)
        {
            unchecked
            {
                uint hash = 0;
                foreach (char c in name.ToLowerInvariant())
                {
                    hash += (byte)c;
                    hash += hash << 10;
                    hash ^= hash >> 6;
                }
                hash += hash << 3;
                hash ^= hash >> 11;
                hash += hash << 15;
                return hash;
            }
        }
    }
}
