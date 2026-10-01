using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using GTA;
using LibertyFramework.Core.Config;

namespace LibertyFramework.Core.Logging
{
    // The project log (scripts\LibertyFramework\logs\LibertyFramework.log). Callers on the game and draw threads only queue
    // the line: a background thread appends the queue to one file it keeps open, every 100 ms or at once for an error.
    // Why: opening, measuring and closing the file for every line on the game thread (several lines per frame from the shot
    // audit) turned slow disk moments (busy disk, antivirus scan, paging) into multi-second game stalls; one ended a run in a
    // crash. Readers (the autopilot) open the file with FileShare.ReadWrite, so the open writer does not block them.
    internal static class RuntimeLog
    {
        private const long MaximumLogBytes = 1048576;
        // Bounded so a logging storm can never eat memory; overflow is counted and reported in the log.
        private const int MaximumPendingLines = 20000;
        private const int FlushIntervalMilliseconds = 100;

        private static readonly object Sync = new object();
        private static readonly Queue<string> Pending = new Queue<string>();
        private static readonly AutoResetEvent Wake = new AutoResetEvent(false);
        private static Thread writer;
        private static int droppedLines;
        private static volatile string writeFailure;
        private static bool failureReported;
        // Writer thread only.
        private static StreamWriter stream;
        private static string logPath;
        private static string backupPath;

        internal static void Info(string message)
        {
            Write("INFO", message);
        }

        internal static void Error(string message)
        {
            Write("ERROR", message);
        }

        private static void Write(string level, string message)
        {
            string line = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + " [" + level + "] " +
                (message ?? string.Empty).Replace("\r", "\\r").Replace("\n", "\\n");
            lock (Sync)
            {
                if (Pending.Count >= MaximumPendingLines) { droppedLines++; }
                else { Pending.Enqueue(line); }
                if (writer == null)
                {
                    writer = new Thread(WriterLoop);
                    writer.IsBackground = true;
                    writer.Name = "LibertyFramework log writer";
                    writer.Start();
                }
            }
            if (level == "ERROR") { Wake.Set(); }
            // The writer thread never touches the game: a write failure is reported from a caller's thread, once.
            string failure = writeFailure;
            if (failure != null && !failureReported)
            {
                failureReported = true;
                try { Game.Console.Print("[LibertyFramework] Log file write failed: " + failure); }
                catch (Exception) { /* the console itself is unavailable; nothing else can report it */ }
            }
        }

        // Writes everything queued so far, on the calling thread. Engine unload calls it so the last lines are not lost.
        internal static void Flush()
        {
            lock (Sync) { Drain(); }
        }

        private static void WriterLoop()
        {
            while (true)
            {
                Wake.WaitOne(FlushIntervalMilliseconds);
                lock (Sync) { Drain(); }
            }
        }

        // Caller holds Sync (the writer thread or Flush).
        private static void Drain()
        {
            if (Pending.Count == 0 && droppedLines == 0) { return; }
            try
            {
                Open();
                if (droppedLines > 0)
                {
                    stream.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + " [ERROR] log_lines_dropped count=" + droppedLines + " (queue full)");
                    droppedLines = 0;
                }
                while (Pending.Count > 0) { stream.WriteLine(Pending.Dequeue()); }
                stream.Flush();
                if (stream.BaseStream.Length >= MaximumLogBytes) { Rotate(); }
                writeFailure = null;
                failureReported = false;
            }
            catch (Exception error)
            {
                // Keep the lines for the next attempt; close so the next attempt reopens cleanly.
                writeFailure = error.Message;
                CloseStream();
            }
        }

        private static void Open()
        {
            if (stream != null) { return; }
            if (logPath == null)
            {
                string directory = Path.Combine(LibertyPaths.Root, "logs");
                Directory.CreateDirectory(directory);
                logPath = Path.Combine(directory, "LibertyFramework.log");
                backupPath = Path.Combine(directory, "LibertyFramework.1.log");
            }
            FileStream file = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            stream = new StreamWriter(file, new UTF8Encoding(false));
        }

        private static void Rotate()
        {
            CloseStream();
            if (File.Exists(backupPath)) { File.Delete(backupPath); }
            File.Move(logPath, backupPath);
        }

        private static void CloseStream()
        {
            if (stream == null) { return; }
            try { stream.Dispose(); }
            catch (Exception) { /* closing a failed stream; the next Open reports any real problem */ }
            stream = null;
        }
    }
}
