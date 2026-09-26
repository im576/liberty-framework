using System;
using System.Collections.Generic;
using GTA.Native;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Services
{
    // SDK IStreaming: model and animation-dictionary requests with per-module holders. A resource is released to the
    // game (MARK_MODEL_AS_NO_LONGER_NEEDED / REMOVE_ANIMS) when its last holder releases it or stops, so modules can
    // request freely without leaking streaming memory (the 32-bit address space is the tight limit on 8 GB machines).
    public sealed class StreamingService : IStreaming
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<int, HashSet<LibertyModule>> models = new Dictionary<int, HashSet<LibertyModule>>();
        private readonly Dictionary<string, HashSet<LibertyModule>> anims = new Dictionary<string, HashSet<LibertyModule>>(StringComparer.OrdinalIgnoreCase);

        internal StreamingService(LibertyEngine engine) { this.engine = engine; }

        public int HeldCount { get { return models.Count + anims.Count; } }

        public bool IsValidModel(ModelRef model) { return !model.IsNone && Function.Call<bool>("IS_MODEL_IN_CDIMAGE", model.Hash); }

        public bool RequestModel(LibertyModule owner, ModelRef model)
        {
            if (!IsValidModel(model)) { return false; }
            HashSet<LibertyModule> holders;
            if (!models.TryGetValue(model.Hash, out holders)) { holders = new HashSet<LibertyModule>(); models[model.Hash] = holders; }
            if (holders.Add(owner))
            {
                int hash = model.Hash;
                engine.Ledger.Add(owner, "model", hash, () => ReleaseModelHold(owner, hash));
            }
            Function.Call("REQUEST_MODEL", model.Hash);
            return Function.Call<bool>("HAS_MODEL_LOADED", model.Hash);
        }

        public void ReleaseModel(LibertyModule owner, ModelRef model) { engine.Ledger.Release(owner, "model", model.Hash); }

        private void ReleaseModelHold(LibertyModule owner, int hash)
        {
            HashSet<LibertyModule> holders;
            if (!models.TryGetValue(hash, out holders)) { return; }
            holders.Remove(owner);
            if (holders.Count > 0) { return; }
            models.Remove(hash);
            Function.Call("MARK_MODEL_AS_NO_LONGER_NEEDED", hash);
        }

        public bool RequestAnimations(LibertyModule owner, string dictionary)
        {
            if (string.IsNullOrEmpty(dictionary)) { return false; }
            HashSet<LibertyModule> holders;
            if (!anims.TryGetValue(dictionary, out holders)) { holders = new HashSet<LibertyModule>(); anims[dictionary] = holders; }
            if (holders.Add(owner))
            {
                engine.Ledger.Add(owner, "anims", KeyOf(dictionary), () => ReleaseAnimHold(owner, dictionary));
            }
            Function.Call("REQUEST_ANIMS", dictionary);
            return Function.Call<bool>("HAVE_ANIMS_LOADED", dictionary);
        }

        public void ReleaseAnimations(LibertyModule owner, string dictionary) { engine.Ledger.Release(owner, "anims", KeyOf(dictionary)); }

        private void ReleaseAnimHold(LibertyModule owner, string dictionary)
        {
            HashSet<LibertyModule> holders;
            if (!anims.TryGetValue(dictionary, out holders)) { return; }
            holders.Remove(owner);
            if (holders.Count > 0) { return; }
            anims.Remove(dictionary);
            try { Function.Call("REMOVE_ANIMS", dictionary); }
            catch (Exception error) { RuntimeLog.Error("streaming_remove_anims_failed " + dictionary + " error=" + error.Message); }
        }

        internal static long KeyOf(string text) { return ModelRef.HashOf(text); }
    }
}
