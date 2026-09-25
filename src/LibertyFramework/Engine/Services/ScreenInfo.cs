using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LibertyFramework.Engine.Services
{
    // The game's back-buffer size from the game window's client rectangle (Win32 only). Never use GTA.Game.Resolution:
    // it goes through the renderer; read on the draw thread it stalled frames to ~3 FPS near cars, and read on the
    // script tick it deadlocked the game on the first frame (2026-09-25, reproduced by the autopilot).
    public static class ScreenInfo
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect { public int Left, Top, Right, Bottom; }

        [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr window, out Rect rect);

        private static IntPtr window;
        private static Size size;
        private static int lastRefreshMs;

        // Cached; refreshed at most once per second. Empty until the game window exists.
        public static Size Size
        {
            get
            {
                int now = Environment.TickCount;
                if (size.Height == 0 || unchecked(now - lastRefreshMs) >= 1000)
                {
                    lastRefreshMs = now;
                    if (window == IntPtr.Zero)
                    {
                        using (Process process = Process.GetCurrentProcess()) { window = process.MainWindowHandle; }
                    }
                    Rect rect;
                    if (window != IntPtr.Zero && GetClientRect(window, out rect) && rect.Right > 0 && rect.Bottom > 0)
                    {
                        size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                    }
                }
                return size;
            }
        }
    }
}
