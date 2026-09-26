namespace Liberty.Sdk
{
    // Immediate drawing for LibertyModule.OnDraw and custom widgets. Coordinates are virtual: the screen is always
    // 1280 x 720 units (scaled to the real resolution, aspect kept by Width growing on wide screens).
    public interface ICanvas
    {
        float Width { get; }
        float Height { get; }
        // Multiplies every colour's alpha (transitions); 1 by default.
        float Opacity { get; set; }
        void Rect(float x, float y, float width, float height, Rgba colour);
        void Text(string text, float x, float y, float width, float height, TextStyle style, TextAlign align, Rgba colour);
        void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint);
        void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint, float rotationDegrees);
        void Line(float x1, float y1, float x2, float y2, float thickness, Rgba colour);
    }
}