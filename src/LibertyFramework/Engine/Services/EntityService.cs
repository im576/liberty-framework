using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GTA;
using GTA.Native;
using HandleObject = GTA.@base.HandleObject;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // Spawned props and peds with an owning module. The engine deletes a module's entities when it stops or fails,
    // and journals every live handle so a ScriptHookDotNet reload in the same game process removes the leftovers.
    public sealed class EntityService
    {
        private sealed class Owned
        {
            internal Module Owner;
            internal HandleObject Entity;
            internal int Handle;
            internal int ModelHash;
            internal bool IsPed;
        }

        private readonly List<Owned> owned = new List<Owned>();

        public int Count { get { return owned.Count; } }

        // Non-blocking: requests the model and reports whether it is loaded now. Call again on later frames.
        public bool RequestModel(Model model)
        {
            if (!model.isValid) { return false; }
            Function.Call("REQUEST_MODEL", model.Hash);
            return Function.Call<bool>("HAS_MODEL_LOADED", model.Hash);
        }

        // Returns null until the model is loaded.
        public GTA.Object CreateObject(Module owner, Model model, Vector3 position)
        {
            if (!RequestModel(model)) { return null; }
            GTA.Object prop = GTA.World.CreateObject(model, position);
            if (prop != null) { Track(owner, prop, model.Hash, false); }
            return prop;
        }

        public Ped CreatePed(Module owner, Model model, Vector3 position)
        {
            if (!RequestModel(model)) { return null; }
            Ped ped = GTA.World.CreatePed(model, position);
            if (ped != null) { Track(owner, ped, model.Hash, true); }
            return ped;
        }

        public void Track(Module owner, HandleObject entity, int modelHash, bool isPed)
        {
            Owned entry = new Owned();
            entry.Owner = owner; entry.Entity = entity; entry.Handle = entity.GetHashCode(); entry.ModelHash = modelHash; entry.IsPed = isPed;
            owned.Add(entry);
            SaveJournal();
        }

        public void Release(Module owner, HandleObject entity)
        {
            for (int i = owned.Count - 1; i >= 0; i--)
            {
                if (owned[i].Owner == owner && owned[i].Entity == entity) { Delete(owned[i]); owned.RemoveAt(i); }
            }
            SaveJournal();
        }

        public void ReleaseAll(Module owner)
        {
            bool changed = false;
            for (int i = owned.Count - 1; i >= 0; i--)
            {
                if (owned[i].Owner == owner) { Delete(owned[i]); owned.RemoveAt(i); changed = true; }
            }
            if (changed) { SaveJournal(); }
        }

        private static void Delete(Owned entry)
        {
            try
            {
                Ped ped = entry.Entity as Ped;
                GTA.Object prop = entry.Entity as GTA.Object;
                Vehicle vehicle = entry.Entity as Vehicle;
                if (ped != null && ped.Exists()) { ped.Delete(); }
                else if (prop != null && prop.Exists()) { prop.Delete(); }
                else if (vehicle != null && vehicle.Exists()) { vehicle.Delete(); }
            }
            catch (Exception error) { RuntimeLog.Error("entity_delete_failed handle=" + entry.Handle + " error=" + error.Message); }
        }

        internal void SaveJournal()
        {
            try
            {
                if (owned.Count == 0) { if (File.Exists(LibertyPaths.EntityJournal)) { File.Delete(LibertyPaths.EntityJournal); } return; }
                using (Process process = Process.GetCurrentProcess())
                {
                    List<string> lines = new List<string> { process.Id + " " + process.StartTime.ToUniversalTime().Ticks };
                    foreach (Owned entry in owned) { lines.Add((entry.IsPed ? "ped " : "obj ") + entry.Handle + " " + entry.ModelHash); }
                    Directory.CreateDirectory(LibertyPaths.StateDirectory);
                    File.WriteAllLines(LibertyPaths.EntityJournal, lines.ToArray());
                }
            }
            catch (Exception error) { RuntimeLog.Error("entity_journal_save_failed error=" + error.Message); }
        }

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
                    int handle = int.Parse(parts[1]), model = int.Parse(parts[2]);
                    if (parts[0] == "ped")
                    {
                        if (!Function.Call<bool>("DOES_CHAR_EXIST", handle)) { continue; }
                        Pointer pedModel = typeof(int);
                        Function.Call("GET_CHAR_MODEL", handle, pedModel);
                        if ((int)pedModel != model) { continue; }
                        Pointer pointer = handle;
                        Function.Call("DELETE_CHAR", pointer);
                    }
                    else
                    {
                        if (!Function.Call<bool>("DOES_OBJECT_EXIST", handle)) { continue; }
                        Pointer objectModel = typeof(int);
                        Function.Call("GET_OBJECT_MODEL", handle, objectModel);
                        if ((int)objectModel != model) { continue; }
                        Pointer pointer = handle;
                        Function.Call("DELETE_OBJECT", pointer);
                    }
                    removed++;
                }
                RuntimeLog.Info("entity_orphans_removed count=" + removed);
            }
            catch (Exception error) { RuntimeLog.Error("entity_orphan_recovery_failed error=" + error.Message); }
        }
    }
}