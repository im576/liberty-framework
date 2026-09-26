using System;

namespace Liberty.Sdk
{
    // One slot of a radial menu. Values are re-evaluated every frame.
    public sealed class RadialSegment
    {
        public Func<string> Label;
        public Func<TextureRef> Icon;
        // Small text under the icon (e.g. "+2").
        public Func<string> Badge;
    }
}