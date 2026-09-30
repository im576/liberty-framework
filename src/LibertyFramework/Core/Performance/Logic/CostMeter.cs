using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace LibertyFramework.Core.Performance.Logic
{
    // Who reads (and resets) the counters. Every sample is added to each reader's window, so one reader's report never
    // shortens another's: the periodic log line (gunplay, every 30 s) and the `costs` command (measurement scenarios).
    internal enum CostReader { Log = 0, Command = 1 }

    // T-026: wall-clock cost of named sections across every Liberty Framework script (script ticks, native calls).
    // Scripts may tick on different threads, so the table is locked; each Add is a few hundred nanoseconds.
    // The first observation of a section records its thread, which tells whether SHDN runs scripts in parallel.
    internal static class CostMeter
    {
        private const int Readers = 2;

        private sealed class Entry
        {
            internal readonly long[] Total = new long[Readers];
            internal readonly long[] Maximum = new long[Readers];
            internal readonly int[] Count = new int[Readers];
            internal int Thread;
        }

        private static readonly Dictionary<string, Entry> Entries = new Dictionary<string, Entry>();
        private static readonly object Gate = new object();

        internal static void Add(string name, long startTimestamp)
        {
            long elapsed = Stopwatch.GetTimestamp() - startTimestamp;
            if (elapsed < 0) { return; }
            lock (Gate)
            {
                Entry entry;
                if (!Entries.TryGetValue(name, out entry))
                {
                    entry = new Entry();
                    entry.Thread = Thread.CurrentThread.ManagedThreadId;
                    Entries.Add(name, entry);
                }
                for (int reader = 0; reader < Readers; reader++)
                {
                    entry.Total[reader] += elapsed;
                    entry.Count[reader]++;
                    if (elapsed > entry.Maximum[reader]) { entry.Maximum[reader] = elapsed; }
                }
            }
        }

        // "name=avg/max/count@thread" for every section, most expensive total first, then resets this reader's counters.
        internal static string ReportAndReset(CostReader reader)
        {
            int index = (int)reader;
            List<KeyValuePair<string, Entry>> rows;
            lock (Gate)
            {
                rows = new List<KeyValuePair<string, Entry>>();
                foreach (KeyValuePair<string, Entry> pair in Entries)
                {
                    if (pair.Value.Count[index] == 0) { continue; }
                    Entry copy = new Entry();
                    copy.Total[index] = pair.Value.Total[index]; copy.Maximum[index] = pair.Value.Maximum[index];
                    copy.Count[index] = pair.Value.Count[index]; copy.Thread = pair.Value.Thread;
                    rows.Add(new KeyValuePair<string, Entry>(pair.Key, copy));
                    pair.Value.Total[index] = 0; pair.Value.Maximum[index] = 0; pair.Value.Count[index] = 0;
                }
            }
            rows.Sort((a, b) => b.Value.Total[index].CompareTo(a.Value.Total[index]));
            StringBuilder text = new StringBuilder("costs_ms(avg/max/count@thread)");
            foreach (KeyValuePair<string, Entry> row in rows)
            {
                double average = row.Value.Total[index] * 1000.0 / Stopwatch.Frequency / row.Value.Count[index];
                double maximum = row.Value.Maximum[index] * 1000.0 / Stopwatch.Frequency;
                double total = row.Value.Total[index] * 1000.0 / Stopwatch.Frequency;
                text.Append(' ').Append(row.Key).Append('=').Append(average.ToString("0.000")).Append('/')
                    .Append(maximum.ToString("0.0")).Append('/').Append(row.Value.Count[index]).Append('@').Append(row.Value.Thread)
                    .Append(" total=").Append(total.ToString("0"));
            }
            return text.ToString();
        }
    }
}
