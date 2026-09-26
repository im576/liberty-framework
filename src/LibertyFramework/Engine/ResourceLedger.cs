using System;
using System.Collections.Generic;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine
{
    // Everything a module owns in the game (entities, cameras, FX, sounds, blips, streaming references, UI, input
    // capture, player-control locks, weather/time/density overrides, memory patches). Each entry carries the action
    // that undoes it; when a module stops or fails, its entries are released newest first.
    public sealed class ResourceLedger
    {
        private sealed class Entry
        {
            internal LibertyModule Owner;
            internal string Kind;
            internal long Key;
            internal Action Release;
        }

        private readonly List<Entry> entries = new List<Entry>();

        public int Count { get { return entries.Count; } }

        public void Add(LibertyModule owner, string kind, long key, Action release)
        {
            if (owner == null) { return; }
            Entry e = new Entry();
            e.Owner = owner; e.Kind = kind; e.Key = key; e.Release = release;
            entries.Add(e);
        }

        public bool Has(LibertyModule owner, string kind, long key)
        {
            foreach (Entry e in entries) { if (e.Owner == owner && e.Kind == kind && e.Key == key) { return true; } }
            return false;
        }

        public bool Has(string kind, long key)
        {
            foreach (Entry e in entries) { if (e.Kind == kind && e.Key == key) { return true; } }
            return false;
        }

        // Drops entries without running their release action (the resource is already gone or handed back).
        public void Forget(string kind, long key) { entries.RemoveAll(e => e.Kind == kind && e.Key == key); }
        public void Forget(LibertyModule owner, string kind, long key) { entries.RemoveAll(e => e.Owner == owner && e.Kind == kind && e.Key == key); }

        // Runs and drops the release actions of one resource (any owner).
        public void Release(string kind, long key)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (i >= entries.Count || entries[i].Kind != kind || entries[i].Key != key) { continue; }
                Entry e = entries[i];
                entries.RemoveAt(i);
                Run(e);
            }
        }

        public void Release(LibertyModule owner, string kind, long key)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (i >= entries.Count || entries[i].Owner != owner || entries[i].Kind != kind || entries[i].Key != key) { continue; }
                Entry e = entries[i];
                entries.RemoveAt(i);
                Run(e);
            }
        }

        // Runs every entry of one kind for one owner (e.g. all memory patches).
        public void ReleaseKind(LibertyModule owner, string kind)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (i >= entries.Count || entries[i].Owner != owner || entries[i].Kind != kind) { continue; }
                Entry e = entries[i];
                entries.RemoveAt(i);
                Run(e);
            }
        }

        // Every owner's entries of one kind (script unload: memory patches are the only resources safe to undo there).
        public void ReleaseKind(string kind)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (i >= entries.Count || entries[i].Kind != kind) { continue; }
                Entry e = entries[i];
                entries.RemoveAt(i);
                Run(e);
            }
        }

        public int ReleaseAll(LibertyModule owner)
        {
            int released = 0;
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                // A release action may drop other entries (a menu closing forgets its own entry).
                if (i >= entries.Count || entries[i].Owner != owner) { continue; }
                Entry e = entries[i];
                entries.RemoveAt(i);
                Run(e);
                released++;
            }
            return released;
        }

        public int CountOf(LibertyModule owner)
        {
            int n = 0;
            foreach (Entry e in entries) { if (e.Owner == owner) { n++; } }
            return n;
        }

        // Kind -> count for one module (diagnostics).
        public Dictionary<string, int> Summary(LibertyModule owner)
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (Entry e in entries)
            {
                if (e.Owner != owner) { continue; }
                int n;
                counts.TryGetValue(e.Kind, out n);
                counts[e.Kind] = n + 1;
            }
            return counts;
        }

        private static void Run(Entry e)
        {
            try { if (e.Release != null) { e.Release(); } }
            catch (Exception error) { RuntimeLog.Error("resource_release_failed module=" + e.Owner.Id + " kind=" + e.Kind + " key=" + e.Key + " error=" + error.Message); }
        }
    }
}