using Liberty.Sdk;

namespace LibertyFramework.Engine.Ui.Logic
{
    // Layout rules of the sprite text renderer (pure, offline-tested): where a rendered string of a given pixel width sits in
    // the rectangle the caller asked for, and the cache key that makes equal requests share one texture.
    internal static class UiTextLogic
    {
        // Left edge of the sprite in pixels. Text wider than its rectangle starts at the rectangle's edge (it is ellipsised at
        // render time, so it never reaches past the rectangle).
        internal static float AlignedX(float rectX, float rectWidth, float textWidth, TextAlign align)
        {
            if (textWidth >= rectWidth) { return rectX; }
            if (align == TextAlign.Center) { return rectX + (rectWidth - textWidth) / 2f; }
            if (align == TextAlign.Right) { return rectX + rectWidth - textWidth; }
            return rectX;
        }

        // One texture per style, pixel size, width bucket and string: the rectangle width only matters when the text would not fit.
        internal static string CacheKey(int style, float pixelSize, float rectWidth, bool bold, string text)
        {
            return style + "|" + (int)(pixelSize * 10f) + "|" + (int)System.Math.Ceiling(rectWidth / 8f) + "|" + (bold ? "b" : "r") + "|" + text;
        }

        // How many characters of `text` fit when each is `averageCharacterWidth` wide, keeping room for the ellipsis (never < 1).
        internal static int FittingCharacters(int length, float averageCharacterWidth, float rectWidth)
        {
            if (length <= 0 || averageCharacterWidth <= 0) { return length; }
            int fit = (int)(rectWidth / averageCharacterWidth);
            return fit >= length ? length : System.Math.Max(1, fit - 1);
        }
    }
}
