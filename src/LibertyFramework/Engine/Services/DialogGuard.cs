using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // Modal message boxes raised inside the game process block the game thread and, in borderless windowed mode, sit
    // hidden behind the game: the game looks frozen. Observed 2026-09-25: FusionFix builds its ambient-occlusion effect
    // on the first rendered frame and, when that fails, shows "Error building shader!" (a compiler warning text) with a
    // single OK button; pressing OK lets the game continue. This background thread presses OK on known harmless boxes
    // and logs every other box it sees (without touching it). It never calls game natives.
    internal static class DialogGuard
    {
        private delegate bool EnumProc(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, IntPtr parameter);
        [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr parent, EnumProc callback, IntPtr parameter);
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr window, StringBuilder text, int size);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr window, StringBuilder text, int size);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);
        [DllImport("user32.dll")] private static extern IntPtr SendMessageTimeout(IntPtr window, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeoutMs, out IntPtr result);

        // BM_CLICK on the box's button (a posted WM_COMMAND IDOK did not close FusionFix's box; verified 2026-09-25).
        private const uint BmClick = 0x00F5;
        private const uint SmtoAbortIfHung = 0x0002;
        private const int PollMs = 500;

        // Titles of boxes that are safe to acknowledge automatically (OK is their only action).
        private static readonly string[] Acknowledge = { "Error building shader!" };

        private static Thread thread;
        private static readonly HashSet<IntPtr> reported = new HashSet<IntPtr>();

        internal static void Start()
        {
            if (thread != null) { return; }
            thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Name = "LibertyDialogGuard";
            thread.Start();
        }

        private static void Run()
        {
            uint self;
            using (Process process = Process.GetCurrentProcess()) { self = (uint)process.Id; }
            while (true)
            {
                try { Scan(self); }
                catch (Exception error) { RuntimeLog.Error("dialog_guard_failed error=" + error.Message); Thread.Sleep(5000); }
                Thread.Sleep(PollMs);
            }
        }

        private static void Scan(uint self)
        {
            EnumWindows((window, parameter) =>
            {
                uint owner;
                GetWindowThreadProcessId(window, out owner);
                if (owner != self || !IsWindowVisible(window) || ClassOf(window) != "#32770") { return true; }
                string title = TextOf(window);
                string body = DialogText(window);
                if (Array.IndexOf(Acknowledge, title) >= 0)
                {
                    IntPtr button = FirstButton(window);
                    IntPtr ignored;
                    if (button != IntPtr.Zero) { SendMessageTimeout(button, BmClick, IntPtr.Zero, IntPtr.Zero, SmtoAbortIfHung, 2000, out ignored); }
                    if (reported.Add(window)) { RuntimeLog.Error("dialog_acknowledged title=\"" + title + "\" text=\"" + body + "\""); }
                }
                else if (reported.Add(window))
                {
                    RuntimeLog.Error("dialog_blocking_game title=\"" + title + "\" text=\"" + body + "\" (left open)");
                }
                return true;
            }, IntPtr.Zero);
        }

        private static IntPtr FirstButton(IntPtr dialog)
        {
            IntPtr found = IntPtr.Zero;
            EnumChildWindows(dialog, (child, parameter) =>
            {
                if (ClassOf(child) == "Button") { found = child; return false; }
                return true;
            }, IntPtr.Zero);
            return found;
        }

        private static string DialogText(IntPtr dialog)
        {
            StringBuilder all = new StringBuilder();
            EnumChildWindows(dialog, (child, parameter) =>
            {
                if (ClassOf(child) == "Static") { string text = TextOf(child); if (text.Length > 0) { all.Append(text.Replace("\r", " ").Replace("\n", " ")).Append(' '); } }
                return true;
            }, IntPtr.Zero);
            return all.ToString().Trim();
        }

        private static string TextOf(IntPtr window) { StringBuilder text = new StringBuilder(1024); GetWindowText(window, text, text.Capacity); return text.ToString(); }
        private static string ClassOf(IntPtr window) { StringBuilder text = new StringBuilder(64); GetClassName(window, text, text.Capacity); return text.ToString(); }
    }
}
