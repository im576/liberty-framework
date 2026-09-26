using System;
using System.Collections.Generic;
using System.IO;
using GTA;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Engine.Ui;

namespace LibertyFramework.Engine.Services
{
    // SDK IUi: GTA IV-style help box (top left), notifications (above the radar), subtitles (bottom centre), list and
    // radial menus, textures and the per-module canvas. Menus take input capture while open (released on close or
    // when the owner stops). Everything a module opened is closed when it stops (resource ledger).
    // Threading: modules post on the engine tick; the draw pass reads snapshots only.
    public sealed class UiService : IUi
    {
        private sealed class TimedText { internal string Text; internal int UntilMs; internal LibertyModule Owner; }

        private readonly LibertyEngine engine;
        private readonly TextureStore textures = new TextureStore();
        private readonly Canvas canvas;
        private readonly List<IMenu> menus = new List<IMenu>();
        private readonly object gate = new object();
        private readonly List<TimedText> notices = new List<TimedText>();
        private TimedText help, subtitle;
        private IMenu[] drawMenus = new IMenu[0];

        internal UiService(LibertyEngine engine)
        {
            this.engine = engine;
            canvas = new Canvas(textures);
        }

        internal Canvas Canvas { get { return canvas; } }
        internal TextureStore Textures { get { return textures; } }

        public TextureRef LoadTexture(string path) { return textures.Load(path); }

        public TextureRef LoadTexture(byte[] png, string cacheKey) { return textures.Add(png, cacheKey); }

        private readonly Dictionary<int, TextureRef> weaponIcons = new Dictionary<int, TextureRef>();

        // Cached per weapon (including misses): menus ask every frame, and the file check must not run per frame.
        public TextureRef WeaponIcon(int weapon)
        {
            TextureRef icon;
            if (weaponIcons.TryGetValue(weapon, out icon)) { return icon; }
            string path = Path.Combine(LibertyPaths.Root, Path.Combine("ui", Path.Combine("icons", weapon + ".png")));
            icon = File.Exists(path) ? textures.Load(path) : TextureRef.None;
            weaponIcons[weapon] = icon;
            return icon;
        }

        public void ShowHelp(LibertyModule owner, string text, int durationMs)
        {
            engine.RequireOwner(owner);
            lock (gate) { help = new TimedText { Text = text, Owner = owner, UntilMs = durationMs > 0 ? Environment.TickCount + durationMs : int.MaxValue }; }
        }

        public void ClearHelp(LibertyModule owner)
        {
            lock (gate) { if (help != null && help.Owner == owner) { help = null; } }
        }

        public void Notify(string text, int durationMs)
        {
            lock (gate)
            {
                notices.Add(new TimedText { Text = text, UntilMs = Environment.TickCount + durationMs });
                if (notices.Count > 4) { notices.RemoveAt(0); }
            }
        }

        public void Subtitle(string text, int durationMs)
        {
            lock (gate) { subtitle = new TimedText { Text = text, UntilMs = Environment.TickCount + durationMs }; }
        }

        public IMenu OpenList(LibertyModule owner, ListMenu menu)
        {
            ListMenuView view = new ListMenuView(owner, menu, Closed);
            return Open(owner, view, menu.LockPlayerControl);
        }

        public IMenu OpenRadial(LibertyModule owner, RadialMenu menu)
        {
            RadialMenuView view = new RadialMenuView(owner, menu, textures, Closed);
            return Open(owner, view, menu.LockPlayerControl);
        }

        private IMenu Open(LibertyModule owner, IMenu view, bool lockControl)
        {
            engine.RequireOwner(owner);
            menus.Add(view);
            engine.Input.CaptureForUi(owner, view, lockControl);
            engine.Ledger.Add(owner, "menu", view.GetHashCode(), () => view.Close());
            Publish();
            return view;
        }

        private void Closed(IMenu view)
        {
            menus.Remove(view);
            LibertyModule owner = view is ListMenuView ? ((ListMenuView)view).Owner : ((RadialMenuView)view).Owner;
            engine.Ledger.Forget(owner, "menu", view.GetHashCode());
            engine.Input.ReleaseForUi(owner, view);
            Publish();
        }

        public bool AnyMenuOpen { get { return menus.Count > 0; } }

        private readonly HashSet<LibertyModule> hudHiders = new HashSet<LibertyModule>();

        // DISPLAY_HUD / DISPLAY_RADAR persist until changed; the HUD returns when the last hiding module stops.
        public void SetHudVisible(LibertyModule owner, bool visible)
        {
            engine.RequireOwner(owner);
            if (!visible)
            {
                if (!hudHiders.Add(owner)) { return; }
                if (hudHiders.Count == 1) { GTA.Native.Function.Call("DISPLAY_HUD", false); GTA.Native.Function.Call("DISPLAY_RADAR", false); }
                engine.Ledger.Add(owner, "hud", 0, () =>
                {
                    if (hudHiders.Remove(owner) && hudHiders.Count == 0) { GTA.Native.Function.Call("DISPLAY_HUD", true); GTA.Native.Function.Call("DISPLAY_RADAR", true); }
                });
            }
            else { engine.Ledger.Release(owner, "hud", 0); }
        }

        private void Publish() { drawMenus = menus.ToArray(); }

        // Engine tick: the top menu gets this frame's input.
        internal void Update()
        {
            if (menus.Count == 0) { return; }
            IMenu top = menus[menus.Count - 1];
            MenuInput input = MenuInput.Read(engine.Input);
            // Menu input edges are logged (rare, one line each): the autopilot and playtest reports can see what a menu received.
            if (input.Up || input.Down || input.Left || input.Right || input.Accept || input.Back || input.X || input.Y || input.PreviousTab || input.NextTab)
            {
                LibertyFramework.Core.Logging.RuntimeLog.Info("ui_input menu=" + (top is RadialMenuView ? "radial" : "list") + (input.Up ? " up" : "") + (input.Down ? " down" : "") +
                    (input.Left ? " left" : "") + (input.Right ? " right" : "") + (input.Accept ? " accept" : "") + (input.Back ? " back" : "") + (input.X ? " x" : "") +
                    (input.Y ? " y" : "") + (input.PreviousTab ? " lb" : "") + (input.NextTab ? " rb" : ""));
            }
            // Menu callbacks (items, labels, select, adjust, close) are the owner's code: they run as that module, and one
            // that throws stops the module. Its menus then close through the ledger, so a broken menu can never stay open
            // with the player's controls locked.
            ListMenuView list = top as ListMenuView;
            if (list != null) { engine.RunAs(list.Owner, () => list.Update(input)); return; }
            RadialMenuView radial = top as RadialMenuView;
            if (radial != null) { engine.RunAs(radial.Owner, () => radial.Update(input)); }
        }

        // Draw pass: modules' canvases first, then menus, help, notifications and subtitles on top.
        internal void Draw(GraphicsEventArgs args, Action<ICanvas> drawModules)
        {
            if (!canvas.Begin(args.Graphics, ScreenInfo.Size)) { return; }
            drawModules(canvas);
            canvas.Opacity = 1f;
            foreach (IMenu menu in drawMenus)
            {
                ListMenuView list = menu as ListMenuView;
                if (list != null) { list.Draw(canvas); continue; }
                RadialMenuView radial = menu as RadialMenuView;
                if (radial != null) { radial.Draw(canvas); }
            }
            int now = Environment.TickCount;
            lock (gate)
            {
                if (help != null && (help.UntilMs == int.MaxValue || unchecked(now - help.UntilMs) < 0) && (help.Owner == null || help.Owner.Running))
                {
                    canvas.Rect(34, 30, 360, 44, new Rgba(0, 0, 0, 200));
                    canvas.Text(help.Text, 46, 40, 336, 30, TextStyle.Body, TextAlign.Left, Rgba.White);
                }
                notices.RemoveAll(n => unchecked(now - n.UntilMs) >= 0);
                float y = 720 - 220;
                for (int i = notices.Count - 1; i >= 0; i--)
                {
                    canvas.Rect(34, y, 360, 32, new Rgba(0, 0, 0, 180));
                    canvas.Text(notices[i].Text, 44, y + 7, 340, 24, TextStyle.Body, TextAlign.Left, Rgba.White);
                    y -= 36;
                }
                if (subtitle != null && unchecked(now - subtitle.UntilMs) < 0)
                {
                    canvas.Text(subtitle.Text, canvas.Width / 2 - 400, 720 - 110, 800, 30, TextStyle.Emphasis, TextAlign.Center, Rgba.White);
                }
            }
        }
    }
}
