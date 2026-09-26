namespace Liberty.Sdk
{
    // Colour with alpha (0-255 each).
    public struct Rgba
    {
        public byte R, G, B, A;
        public Rgba(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }
        public Rgba(byte r, byte g, byte b) : this(r, g, b, 255) { }
        public Rgba WithAlpha(byte a) { return new Rgba(R, G, B, a); }
        public static readonly Rgba White = new Rgba(240, 240, 242);
        public static readonly Rgba Black = new Rgba(0, 0, 0);
        public static readonly Rgba Glass = new Rgba(8, 10, 14, 200);
        public static readonly Rgba Highlight = new Rgba(236, 238, 242, 62);
        public static readonly Rgba Muted = new Rgba(205, 208, 214, 170);
    }
}