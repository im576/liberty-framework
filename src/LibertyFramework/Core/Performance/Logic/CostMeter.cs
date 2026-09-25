using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace LibertyFramework.Core.Performance.Logic
{
    // T-026: wall-clock cost of named sections across every Liberty Framework script (script ticks, native calls).
    // Scripts may tick on different threads, so the table is locked; each Add is a few hundred nanoseconds.
    // The first observation of a section records its thread, which tells whether SHDN runs scripts in parallel.
    internal static class CostMeter
    {
        private sealed class Entry
        {
            internal long Total;
            internal long Maximum;
            internal int Count;
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
                entry.Total += elapsed;
                entry.Count++;
                if (elapsed > entry.Maximum) { entry.Maximum = elapsed; }
            }
        }

        // "name=avg/max/count@thread" for every section, most expensive total first, then resets the counters.
        internal static string ReportAndReset()
        {
            List<KeyValuePair<string, Entry>> rows;
            lock (Gate)
            {
                rows = new List<KeyValuePair<string, Entry>>();
                foreach (KeyValuePair<string, Entry> pair in Entries)
                {
                    if (pair.Value.Count == 0) { continue; }
                    Entry copy = new Entry();
                    copy.Total = pair.Value.Total; copy.Maximum = pair.Value.Maximum; copy.Count = pair.Value.Count; copy.Thread = pair.Value.Thread;
                    rows.Add(new KeyValuePair<string, Entry>(pair.Key, copy));
                    pair.Value.Total = 0; pair.Value.Maximum = 0; pair.Value.Count = 0;
                }
            }
            rows.Sort((a, b) => b.Value.Total.CompareTo(a.Value.Total));
            StringBuilder text = new StringBuilder("costs_ms(avg/max/count@thread)");
            foreach (KeyValuePair<string, Entry> row in rows)
            {
                double average = row.Value.Total * 1000.0 / Stopwatch.Frequency / row.Value.Count;
                double maximum = row.Value.Maximum * 1000.0 / Stopwatch.Frequency;
                double total = row.Value.Total * 1000.0 / Stopwatch.Frequency;
                text.Append(' ').Append(row.Key).Append('=').Append(average.ToString("0.000")).Append('/')
                    .Append(maximum.ToString("0.0")).Append('/').Append(row.Value.Count).Append('@').Append(row.Value.Thread)
                    .Append(" total=").Append(total.ToString("0"));
            }
            return text.ToString();
        }
    }
}
