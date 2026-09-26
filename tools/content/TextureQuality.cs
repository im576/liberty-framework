namespace LibertyFramework.Content
{
    // How closely a compiled texture's top level decodes to its source pixels (native texture mode read-back).
    internal sealed class TextureQuality
    {
        internal double PsnrRgbDb;
        internal int MaxErrorRgb;
        // Alpha is measured for DXT5 only (DXT1 output is opaque); HasAlpha says whether these two are set.
        internal bool HasAlpha;
        internal double PsnrAlphaDb;
        internal int MaxErrorAlpha;
    }
}
