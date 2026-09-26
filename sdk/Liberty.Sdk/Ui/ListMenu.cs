using System;
using System.Collections.Generic;

namespace Liberty.Sdk
{
    // A GTA IV-style list menu (shops, garages, debug pages). Items is re-read every frame, so it may change.
    public sealed class ListMenu
    {
        public string Title;
        public Func<IList<ListItem>> Items;
        public Action OnClosed;
        // Left edge in virtual units; the menu is 430 wide.
        public float X = 60;
        public float Y = 90;
        public int VisibleRows = 10;
        // Lock the player's controls while open (default). Turn off when the module manages control itself.
        public bool LockPlayerControl = true;
    }
}