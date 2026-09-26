using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // A GTA IV-style radial menu (weapon wheel, interaction wheel). Segment 0 is at the top, clockwise. The right
    // stick or D-pad left/right selects; LB/RB (or D-pad up/down) cycle within a segment; A/X/Y call the handlers.
    public sealed class RadialMenu
    {
        public string Title;
        public IList<RadialSegment> Segments = new List<RadialSegment>();
        // Lines drawn in the centre for the selected segment (e.g. name, ammo, hints).
        public Func<int, string[]> CenterLines;
        public Func<int, string> OnAccept;
        public Func<int, string> OnX;
        public Func<int, string> OnY;
        // -1 / +1 from LB/RB or D-pad up/down.
        public Action<int, int> OnCycle;
        public Action OnClosed;
        // Fraction of the screen height the wheel spans.
        public float Size = 0.66f;
    }
}