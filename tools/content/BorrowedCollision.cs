using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibertyFramework.Finishes;

namespace LibertyFramework.Content
{
    // asset.json "collision": { "borrow": { archive, model } } (T-032): the asset takes a vanilla prop's own bounds resource
    // (<model>.wbn, the RSC file unchanged) and ships it under its own model name. The collision writer for authored shapes
    // needs the bounds layout first (PROBE-bounds-layout); borrowing is the smallest experiment that answers the questions
    // before it: does a model in this project's archive get collision from a same-named bounds resource, for script-created
    // objects, and does the engine's raycast hit it (docs/research/Collision.md C4-C6). The bytes are read from the owner's
    // game at build time and never committed. model "auto" takes the first prop candidate by the rule PROBE-collision uses
    // (a drawable shipped with a same-named .wbn in the same archive, sorted by lower-case name), so it is the model the
    // raycast-objects scenario tests on its own.
    internal static class BorrowedCollision
    {
        internal const string Extension = ".wbn";

        internal sealed class Borrowed
        {
            internal string From;        // archive/model
            internal byte[] Resource;    // the RSC file as the game ships it
            internal uint ResourceType;
        }

        // Prop candidates as PROBE-collision lists them: (lower-case model name, archive), sorted by name.
        internal static List<KeyValuePair<string, string>> Candidates(string game, byte[] key, IEnumerable<string> archives, List<string> notes)
        {
            List<KeyValuePair<string, string>> found = new List<KeyValuePair<string, string>>();
            foreach (string relative in archives)
            {
                ImgArchive archive;
                try { archive = ImgArchive.Open(Path.Combine(game, relative), key); }
                catch (Exception error) { notes.Add("collision search skipped " + relative + ": " + error.Message); continue; }
                Dictionary<string, HashSet<string>> byName = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                foreach (ImgArchive.Entry entry in archive.Entries)
                {
                    string baseName = Path.GetFileNameWithoutExtension(entry.Name);
                    HashSet<string> kinds;
                    if (!byName.TryGetValue(baseName, out kinds)) { kinds = new HashSet<string>(); byName[baseName] = kinds; }
                    kinds.Add(Path.GetExtension(entry.Name).ToLowerInvariant());
                }
                foreach (KeyValuePair<string, HashSet<string>> pair in byName)
                {
                    if (pair.Value.Contains(".wdr") && pair.Value.Contains(Extension)) { found.Add(new KeyValuePair<string, string>(pair.Key.ToLowerInvariant(), relative)); }
                }
            }
            return found.OrderBy(c => c.Key, StringComparer.Ordinal).ToList();
        }

        // The borrowed bounds, or null when "auto" finds no candidate (the build then ships no collision and says so, so one
        // asset cannot fail the whole package). A named model that is missing is an error.
        internal static Borrowed Find(string game, AssetManifest.TemplateRef reference, List<string> notes)
        {
            byte[] key = null;
            try { key = ImgArchive.FindKey(Path.Combine(game, "GTAIV.exe")); }
            catch (InvalidDataException error) { notes.Add("no IMG key (" + error.Message + "): encrypted archives cannot be searched for collision"); }
            List<string> archives = reference.Archive == AssetManifest.AnyArchive ? Probe.Archives(game, new List<string>()) : new List<string> { reference.Archive };
            string model = reference.Model, archiveName = null;
            if (string.Equals(model, AssetManifest.AutoTemplate, StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> candidates = Candidates(game, key, archives, notes);
                if (candidates.Count == 0)
                {
                    notes.Add("collision borrow: no drawable with a same-named " + Extension + " in " + reference.Archive + "; built without collision");
                    return null;
                }
                model = candidates[0].Key;
                archiveName = candidates[0].Value;
                notes.Add("collision borrowed from the first of " + candidates.Count + " prop candidates: " + archiveName + "/" + model);
            }
            foreach (string relative in archiveName != null ? new List<string> { archiveName } : archives)
            {
                ImgArchive archive;
                try { archive = ImgArchive.Open(Path.Combine(game, relative), key); }
                catch (Exception error)
                {
                    if (archiveName != null || archives.Count == 1) { throw; }
                    notes.Add("collision search skipped " + relative + ": " + error.Message);
                    continue;
                }
                ImgArchive.Entry entry = archive.Entries.FirstOrDefault(e => string.Equals(e.Name, model + Extension, StringComparison.OrdinalIgnoreCase));
                if (entry == null) { continue; }
                byte[] data = archive.Extract(entry.Name);
                RscResource resource = RscResource.Parse(data, true); // it must at least be a resource
                return new Borrowed { From = relative + "/" + model, Resource = data, ResourceType = resource.Type };
            }
            throw new InvalidDataException("collision borrow: " + model + Extension + " not found in " + reference.Archive);
        }
    }
}
