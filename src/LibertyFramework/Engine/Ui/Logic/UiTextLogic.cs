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

        // Match the raster's actual pixel width; two rectangles in one eight-pixel bucket can truncate differently.
        internal static string CacheKey(int style, float pixelSize, float rectWidth, bool bold, string text)
        {
            return style + "|" + pixelSize.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "|" +
                (int)System.Math.Max(0, System.Math.Floor(rectWidth)) + "|" + (bold ? "b" : "r") + "|" + text;
        }

        // Measure the candidate including its ellipsis. Average character widths cannot bound proportional text.
        // Text-element boundaries preserve surrogate pairs and combining accents when a label is shortened.
        internal static string FitText(string text, float width, System.Func<string, float> measure)
        {
            if (string.IsNullOrEmpty(text) || width <= 0) { return ""; }
            if (measure(text) <= width) { return text; }
            const string ellipsis = "...";
            if (measure(ellipsis) > width) { return ""; }
            int[] starts = System.Globalization.StringInfo.ParseCombiningCharacters(text);
            int low = 0, high = starts.Length - 1;
            string result = ellipsis;
            while (low <= high)
            {
                int count = low + (high - low) / 2;
                string candidate = text.Substring(0, starts[count]) + ellipsis;
                if (measure(candidate) <= width) { result = candidate; low = count + 1; }
                else { high = count - 1; }
            }
            return result;
        }
    }
}
