// Test-only boundaries: production UiService, RadialMenuView, MenuInput, StorageWheel and ResourceLedger are compiled
// unchanged. SHDN, canvas/texture IO and engine dispatch are spies; this is not engine/game acceptance evidence.
using System;
using System.Collections.Generic;
using Liberty.Sdk;

namespace GTA
{
    public sealed class GraphicsEventArgs { public object Graphics { get { return null; } } }
}
namespace GTA.Native
{
    public static class Function
    {
        public static void Call(string name, bool value) { throw new InvalidOperationException("Unexpected native: " + name); }
    }
}
namespace LibertyFramework.Core.Config
{
    public static class LibertyPaths { public static string Root { get { return "."; } } }
}
namespace LibertyFramework.Core.Logging
{
    public static class RuntimeLog
    {
        public static readonly List<string> Errors = new List<string>();
        public static void Info(string text) { }
        public static void Error(string text) { Errors.Add(text); }
    }
}
namespace LibertyFramework.Core.Performance.Logic
{
    public static class CostMeter { public static void Add(string name, long start) { } }
}
namespace LibertyFramework.Engine
{
    internal sealed class LibertyEngine
    {
        public static LibertyEngine Current;
        public LibertyModule CurrentModule;
        public int Frame = 100;
        public bool OnTick = true;
        public readonly ResourceLedger Ledger = new ResourceLedger();
        public readonly Services.InputService Input = new Services.InputService();
        public readonly Services.UiService Ui;
        public LibertyEngine() { Current = this; Ui = new Services.UiService(this); }
        public void RequireOwner(LibertyModule owner) { if (owner == null) { throw new ArgumentNullException("owner"); } }
        public bool RunAs(LibertyModule owner, Action action)
        {
            if (!OnTick) { throw new InvalidOperationException("Callback outside tick"); }
            LibertyModule previous = CurrentModule;
            CurrentModule = owner;
            try { action(); return true; }
            catch (Exception error)
            {
                Core.Logging.RuntimeLog.Error("engine_module_failed " + owner.Id + " " + error);
                owner.Running = false;
                Ledger.ReleaseAll(owner);
                return false;
            }
            finally { CurrentModule = previous; }
        }
    }
}
namespace LibertyFramework.Engine.Services
{
    internal sealed class InputService
    {
        internal readonly HashSet<IMenu> Captured = new HashSet<IMenu>();
        internal readonly HashSet<IMenu> Locked = new HashSet<IMenu>();
        internal readonly HashSet<PadButton> Pending = new HashSet<PadButton>();
        internal int Reads;
        internal float RightX { get { Reads++; return 0; } }
        internal float RightY { get { Reads++; return 0; } }
        internal bool Pressed(PadButton key) { Reads++; return Pending.Contains(key); }
        internal bool KeyPressed(VirtualKey key) { Reads++; return false; }
        internal void CaptureForUi(LibertyModule owner, IMenu menu, bool locked)
        { Captured.Add(menu); if (locked) { Locked.Add(menu); } }
        internal void ReleaseForUi(LibertyModule owner, IMenu menu) { Captured.Remove(menu); Locked.Remove(menu); }
    }
}
namespace LibertyFramework.Engine.Ui
{
    internal sealed class TextureStore
    {
        internal TextureRef Load(string path) { return TextureRef.None; }
        internal TextureRef Add(byte[] bytes, string key) { return TextureRef.None; }
    }
    internal static class RadialArt
    {
        internal const float IconRadius = 0.7f;
        private static TextureRef Art(int id)
        { if (!LibertyEngine.Current.OnTick) { throw new InvalidOperationException("Art requested during draw"); } return new TextureRef(id); }
        internal static TextureRef Ring(TextureStore store, int count) { return Art(10); }
        internal static TextureRef Highlight(TextureStore store, int count, int selected) { return Art(100 + selected); }
        internal static TextureRef Centre(TextureStore store) { return Art(20); }
    }
    internal static class ScreenInfo { internal static object Size { get { return null; } } }
    internal sealed class Canvas : ICanvas
    {
        internal readonly List<string> Texts = new List<string>();
        internal readonly List<int> Sprites = new List<int>();
        internal Canvas(TextureStore store) { Opacity = 1; }
        internal bool Begin(object graphics, object size) { Texts.Clear(); Sprites.Clear(); return true; }
        internal void DrawProbe() { }
        public float Width { get { return 1280; } }
        public float Height { get { return 720; } }
        public float Opacity { get; set; }
        public void Rect(float x, float y, float width, float height, Rgba colour) { }
        public void Text(string text, float x, float y, float width, float height, TextStyle style, TextAlign align, Rgba colour) { Texts.Add(text); }
        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint) { Sprites.Add(texture.Handle); }
        public void Sprite(TextureRef texture, float x, float y, float width, float height, Rgba tint, float rotation) { Sprites.Add(texture.Handle); }
        public void Line(float x1, float y1, float x2, float y2, float thickness, Rgba colour) { }
    }
    internal sealed class ListMenuView : IMenu
    {
        private readonly Action<ListMenuView> closed;
        internal LibertyModule Owner;
        internal ListMenuView(LibertyModule owner, ListMenu menu, Action<ListMenuView> close) { Owner = owner; closed = close; IsOpen = true; }
        public bool IsOpen { get; private set; }
        public int Selected { get; set; }
        public void Message(string text) { }
        public void Close() { if (!IsOpen) { return; } IsOpen = false; closed(this); }
        internal void Update(MenuInput input) { }
        internal void Draw(ICanvas canvas) { }
    }
}
namespace LibertyFramework.DevTools.Menu
{
    // Storage's gunsmith descriptor boundary is not exercised by this radial-opening regression.
    internal sealed class MenuItem
    {
        internal Func<string> Label { get; set; }
        internal Func<string> Activate { get; set; }
        internal bool RequiresConfirmation { get; set; }
    }
}
