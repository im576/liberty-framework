using System;
using System.Collections.Generic;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Ui
{
    // A GTA IV-style list menu: title bar, rows, a status line. D-pad up/down moves, left/right adjusts, A selects
    // (twice for Confirm rows), B closes. Labels are evaluated on the engine tick into an immutable snapshot the draw
    // pass reads, so module callbacks never run on the render thread.
    internal sealed class ListMenuView : IMenu
    {
        private sealed class Snapshot
        {
            internal string Title;
            internal string[] Labels;
            internal bool[] Enabled;
            internal int Selected, First;
            internal string Message;
            internal bool Pending;
        }

        private const float MenuWidth = 430f, RowHeight = 30f, TitleHeight = 44f;
        private readonly ListMenu menu;
        private readonly Action<ListMenuView> onClosed;
        private IList<ListItem> items = new List<ListItem>();
        private ListItem pending;
        private string message;
        private int messageUntil;
        private volatile Snapshot snapshot;

        internal ListMenuView(LibertyModule owner, ListMenu menu, Action<ListMenuView> onClosed)
        {
            Owner = owner; this.menu = menu; this.onClosed = onClosed;
            IsOpen = true;
        }

        internal LibertyModule Owner { get; private set; }
        private bool labelFailureLogged;
        public bool IsOpen { get; private set; }
        public int Selected { get; set; }

        public void Message(string text) { message = text; messageUntil = Environment.TickCount + 2600; }

        public void Close()
        {
            if (!IsOpen) { return; }
            IsOpen = false;
            onClosed(this);
            if (menu.OnClosed != null) { menu.OnClosed(); }
        }

        internal void Update(MenuInput input)
        {
            items = menu.Items != null ? (menu.Items() ?? new List<ListItem>()) : new List<ListItem>();
            int count = items.Count;
            if (count > 0)
            {
                Selected = Math.Max(0, Math.Min(Selected, count - 1));
                if (input.Up) { Selected = (Selected + count - 1) % count; pending = null; }
                if (input.Down) { Selected = (Selected + 1) % count; pending = null; }
                ListItem item = items[Selected];
                bool enabled = item.Enabled == null || item.Enabled();
                if (enabled && item.OnAdjust != null && (input.Left || input.Right)) { Say(item.OnAdjust(input.Left ? -1 : 1)); }
                if (enabled && input.Accept && item.OnSelect != null)
                {
                    if (item.Confirm && pending != item) { pending = item; Message("Press A again to confirm"); }
                    else { pending = null; Say(item.OnSelect()); }
                }
            }
            if (input.Back) { Close(); return; }
            Snapshot s = new Snapshot();
            s.Title = menu.Title;
            s.Labels = new string[count];
            s.Enabled = new bool[count];
            for (int i = 0; i < count; i++)
            {
                try { s.Labels[i] = items[i].Label != null ? items[i].Label() : ""; }
                catch (Exception error)
                {
                    // Tolerated (the row shows "?"), and logged once per menu: this runs every frame.
                    s.Labels[i] = "?";
                    if (!labelFailureLogged) { labelFailureLogged = true; RuntimeLog.Error("[" + Owner.Id + "] ui_label_failed row=" + i + " error=" + error); }
                }
                s.Enabled[i] = items[i].Enabled == null || items[i].Enabled();
            }
            s.Selected = Selected;
            int visible = Math.Max(1, menu.VisibleRows);
            s.First = Math.Max(0, Math.Min(Selected - visible / 2, count - visible));
            s.Message = message != null && unchecked(Environment.TickCount - messageUntil) < 0 ? message : null;
            s.Pending = pending != null;
            snapshot = s;
        }

        private void Say(string text) { if (!string.IsNullOrEmpty(text)) { Message(text); } }

        internal void Draw(ICanvas canvas)
        {
            Snapshot s = snapshot;
            if (s == null) { return; }
            float x = menu.X, y = menu.Y;
            int visible = Math.Max(1, menu.VisibleRows);
            int rows = Math.Min(visible, s.Labels.Length - s.First);
            canvas.Rect(x, y, MenuWidth, TitleHeight, new Rgba(0, 0, 0, 220));
            canvas.Text(s.Title ?? "", x + 16, y + 10, MenuWidth - 32, TitleHeight - 12, TextStyle.Title, TextAlign.Left, Rgba.White);
            y += TitleHeight + 2;
            canvas.Rect(x, y, MenuWidth, Math.Max(1, rows) * RowHeight + 8, Rgba.Glass);
            y += 4;
            for (int i = s.First; i < s.First + rows; i++)
            {
                bool selected = i == s.Selected;
                if (selected) { canvas.Rect(x + 4, y, MenuWidth - 8, RowHeight, Rgba.Highlight); }
                Rgba colour = !s.Enabled[i] ? Rgba.Muted.WithAlpha(110) : selected ? Rgba.White : Rgba.Muted;
                canvas.Text(s.Labels[i], x + 16, y + 6, MenuWidth - 32, RowHeight - 6, selected ? TextStyle.Emphasis : TextStyle.Body, TextAlign.Left, colour);
                y += RowHeight;
            }
            y += 10;
            if (s.Labels.Length > visible)
            {
                canvas.Text((s.Selected + 1) + " / " + s.Labels.Length, x, y, MenuWidth - 12, 20, TextStyle.Small, TextAlign.Right, Rgba.Muted);
            }
            if (s.Message != null)
            {
                canvas.Rect(x, y + 22, MenuWidth, 30, new Rgba(0, 0, 0, 200));
                canvas.Text(s.Message, x + 12, y + 28, MenuWidth - 24, 22, TextStyle.Body, TextAlign.Left, Rgba.White);
            }
        }
    }
}
