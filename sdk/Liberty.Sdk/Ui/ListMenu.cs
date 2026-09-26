using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // A GTA IV-style list menu (shops, garages, debug pages). Items may be rebuilt with Refresh.
    public sealed class ListMenu
    {
        public string Title;
        public Func<IList<ListItem>> Items;
        public Action OnClosed;
        // Left edge in virtual units; the menu is 430 wide.
        public float X = 60;
        public float Y = 90;
        public int VisibleRows = 10;
    }
}