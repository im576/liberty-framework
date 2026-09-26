using System;
using LibertyFramework.Models;

namespace LibertyFramework.Content
{
    // RGBA image -> NativeTexture (DXT1 or DXT5) with a box-filtered mip chain.
    internal static class TextureEncoder
    {
        internal const int MinSizePixels = 4;
        internal const int MaxSizePixels = 2048;
        // Every mipmapped texture in the game dictionaries surveyed stops at the level whose smaller side is 4 pixels (one DXT
        // block): 256x256 has 7 levels, 128x128 has 6, 64x64 has 5 (amb_nailgun.wtd, coronas.wtd). "Full chain" means that.
        internal const int SmallestMipSidePixels = 4;

        internal static int FullChainLevels(int width, int height)
        {
            int levels = 1, side = Math.Min(width, height);
            while (side > SmallestMipSidePixels) { side /= 2; levels++; }
            return levels;
        }

        internal static bool IsPowerOfTwo(int value) { return value > 0 && (value & (value - 1)) == 0; }

        internal static void CheckSize(int width, int height)
        {
            if (!IsPowerOfTwo(width) || !IsPowerOfTwo(height) || width < MinSizePixels || height < MinSizePixels || width > MaxSizePixels || height > MaxSizePixels)
            {
                throw new ArgumentException("texture size " + width + "x" + height + " must be a power of two from " + MinSizePixels + " to " + MaxSizePixels);
            }
        }

        // levels: 1 (no mips) up to FullChainLevels; 0 means the full chain.
        internal static NativeTexture Encode(string name, RgbaImage image, string format, int levels)
        {
            CheckSize(image.Width, image.Height);
            if (format != "DXT1" && format != "DXT5") { throw new ArgumentException("texture " + name + ": format " + format + " is not written (DXT1, DXT5)"); }
            int full = FullChainLevels(image.Width, image.Height);
            if (levels == 0) { levels = full; }
            if (levels < 1 || levels > full) { throw new ArgumentException("texture " + name + ": " + levels + " mip levels requested, 1-" + full + " possible for " + image.Width + "x" + image.Height); }
            NativeTexture texture = new NativeTexture { Name = name, Format = format, Width = image.Width, Height = image.Height };
            RgbaImage level = image;
            for (int i = 0; i < levels; i++)
            {
                texture.Levels.Add(format == "DXT1" ? Dxt1Encoder.Encode(level.ToRgb(), level.Width, level.Height) : Dxt5Encoder.Encode(level.Pixels, level.Width, level.Height));
                if (i + 1 < levels) { level = level.Half(); }
            }
            return texture;
        }
    }
}
