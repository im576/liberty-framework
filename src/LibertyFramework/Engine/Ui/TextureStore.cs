using System;
using System.Collections.Generic;
using System.IO;
using Liberty.Sdk;
using LibertyFramework.Core.Config;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Ui
{
    // Textures for UI drawing. Loading on the engine tick only reads the PNG bytes and assigns a handle; the
    // ScriptHookDotNet texture (a D3D object) is created on first use in the draw pass, where the Arsenal wheel
    // proved texture creation works. Cached by path or key for the session.
    internal sealed class TextureStore
    {
        private sealed class Entry
        {
            internal byte[] Png;
            internal GTA.Texture Texture;
            internal bool Failed;
        }

        private readonly object gate = new object();
        private readonly Dictionary<int, Entry> entries = new Dictionary<int, Entry>();
        private readonly Dictionary<string, int> byKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private int nextHandle = 1;

        internal TextureRef Load(string path)
        {
            string full = Path.IsPathRooted(path) ? path : Path.Combine(LibertyPaths.Root, path);
            lock (gate)
            {
                int known;
                if (byKey.TryGetValue(full, out known)) { return new TextureRef(known); }
            }
            if (!File.Exists(full)) { RuntimeLog.Error("ui_texture_missing path=" + full); return TextureRef.None; }
            return Add(File.ReadAllBytes(full), full);
        }

        internal TextureRef Add(byte[] png, string key)
        {
            if (png == null || png.Length == 0) { return TextureRef.None; }
            lock (gate)
            {
                int known;
                if (key != null && byKey.TryGetValue(key, out known)) { return new TextureRef(known); }
                int handle = nextHandle++;
                entries[handle] = new Entry { Png = png };
                if (key != null) { byKey[key] = handle; }
                return new TextureRef(handle);
            }
        }

        internal TextureRef Find(string key)
        {
            lock (gate) { int handle; return byKey.TryGetValue(key, out handle) ? new TextureRef(handle) : TextureRef.None; }
        }

        // Draw pass only.
        internal GTA.Texture Get(TextureRef texture)
        {
            if (texture.IsNone) { return null; }
            Entry entry;
            lock (gate) { if (!entries.TryGetValue(texture.Handle, out entry)) { return null; } }
            if (entry.Texture != null || entry.Failed) { return entry.Texture; }
            try { entry.Texture = new GTA.Texture(entry.Png); entry.Png = null; }
            catch (Exception error) { entry.Failed = true; RuntimeLog.Error("ui_texture_create_failed handle=" + texture.Handle + " error=" + error.Message); }
            return entry.Texture;
        }
    }
}
