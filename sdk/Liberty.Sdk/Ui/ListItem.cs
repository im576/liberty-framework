using System;

namespace Liberty.Sdk
{
    // One row of a list menu. Label is re-evaluated every frame (live values).
    public sealed class ListItem
    {
        public Func<string> Label;
        // A: returns a status line to show (or null).
        public Func<string> OnSelect;
        // Left/right: -1 or +1; returns a status line (or null).
        public Func<int, string> OnAdjust;
        // A must be pressed twice (purchases, destructive actions).
        public bool Confirm;
        public Func<bool> Enabled;

        public static ListItem Action(string label, Func<string> onSelect) { ListItem i = new ListItem(); i.Label = () => label; i.OnSelect = onSelect; return i; }
        public static ListItem Info(Func<string> label) { ListItem i = new ListItem(); i.Label = label; return i; }
        public static ListItem Adjust(Func<string> label, Func<int, string> onAdjust) { ListItem i = new ListItem(); i.Label = label; i.OnAdjust = onAdjust; return i; }
    }
}