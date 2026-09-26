using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA.Native;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // Journal of every entity a module created (peds, vehicles, props). Deletion on module stop goes through the
    // resource ledger; this journal covers the case the ledger cannot: a ScriptHookDotNet reload in the same game
    // process, after which the new engine deletes the previous domain's leftovers (same handle and model only).
    public sealed class EntityService
    {
        internal enum Kind { Ped, Vehicle, Prop }

        private sealed class Owned
        {
            internal LibertyModule Owner;
            internal Kind Kind;
            internal int Handle;
            internal int Model;
        }

        private readonly List<Owned> owned = new List<Owned>();
        private bool dirty;
        private int lastSaveMs;

        public int Count { get { return owned.Count; } }

        internal int CountOf(LibertyModule owner)
        {
            int n = 0;
            foreach (Owned o in owned) { if (o.Owner == owner) { n++; } }
            return n;
        }

        internal void Track(LibertyModule owner, Kind kind, int handle, int model)
        {
            Owned entry = new Owned();
            entry.Owner = owner; entry.Kind = kind; entry.Handle = handle; entry.Model = model;
            owned.Add(entry);
            dirty = true;
        }

        internal void Untrack(Kind kind, int handle)
        {
            if (owned.RemoveAll(o => o.Kind == kind && o.Handle == handle) > 0) { dirty = true; }
        }

        // Written at most twice a second (spawning a crowd must not write the file per ped) and at unload.
        internal void Flush(bool force)
        {
            int now = Environment.TickCount;
            if (!dirty || (!force && unchecked(now - lastSaveMs) < 500)) { return; }
            lastSaveMs = now;
            dirty = false;
            SaveJournal();
        }

        internal void SaveJournal()
        {
            try
            {
                if (owned.Count == 0) { if (File.Exists(LibertyPaths.EntityJournal)) { File.Delete(LibertyPaths.EntityJournal); } return; }
                using (Process process = Process.GetCurrentProcess())
                {
                    List<string> lines = new List<string> { process.Id + " " + process.StartTime.ToUniversalTime().Ticks };
                    foreach (Owned entry in owned) { lines.Add(Word(entry.Kind) + " " + entry.Handle + " " + entry.Model); }
                    Directory.CreateDirectory(LibertyPaths.StateDirectory);
                    File.WriteAllLines(LibertyPaths.EntityJournal, lines.ToArray());
                }
            }
            catch (Exception error) { RuntimeLog.Error("entity_journal_save_failed error=" + error.Message); }
        }

        private static string Word(Kind kind) { return kind == Kind.Ped ? "ped" : kind == Kind.Vehicle ? "veh" : "obj"; }

        // Same game process only: after a script reload, handles in the journal that still name the same model are ours.
        internal void RecoverOrphans()
        {
            if (!File.Exists(LibertyPaths.EntityJournal)) { return; }
            try
            {
                string[] lines = File.ReadAllLines(LibertyPaths.EntityJournal);
                File.Delete(LibertyPaths.EntityJournal);
                using (Process process = Process.GetCurrentProcess())
                {
                    if (lines.Length == 0 || lines[0] != process.Id + " " + process.StartTime.ToUniversalTime().Ticks) { return; }
                }
                int removed = 0;
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(' ');
                    if (parts.Length != 3) { continue; }
                    int handle = int.Parse(parts[1]), model = int.Parse(parts[2]);
                    if (Remove(parts[0], handle, model)) { removed++; }
                }
                RuntimeLog.Info("entity_orphans_removed count=" + removed);
            }
            catch (Exception error) { RuntimeLog.Error("entity_orphan_recovery_failed error=" + error.Message); }
        }

        // Literal native names so the offline verifier sees every one (tools/verify NativeChecks).
        private static bool Remove(string kind, int handle, int model)
        {
            Pointer pointer = handle;
            switch (kind)
            {
                case "ped":
                    if (!Function.Call<bool>("DOES_CHAR_EXIST", handle) || NativeCall.OutInt("GET_CHAR_MODEL", handle) != model) { return false; }
                    Function.Call("DELETE_CHAR", pointer);
                    return true;
                case "veh":
                    if (!Function.Call<bool>("DOES_VEHICLE_EXIST", handle) || NativeCall.OutInt("GET_CAR_MODEL", handle) != model) { return false; }
                    Function.Call("DELETE_CAR", pointer);
                    return true;
                case "obj":
                    if (!Function.Call<bool>("DOES_OBJECT_EXIST", handle) || NativeCall.OutInt("GET_OBJECT_MODEL", handle) != model) { return false; }
                    Function.Call("DELETE_OBJECT", pointer);
                    return true;
                default:
                    return false;
            }
        }
    }
}
