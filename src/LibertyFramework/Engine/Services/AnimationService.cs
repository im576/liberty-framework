using System;
using System.Collections;
using System.Collections.Generic;
using GTA;
using LibertyFramework.Core.Logging;
using LibertyFramework.Engine.Scheduling;

namespace LibertyFramework.Engine.Services
{
    // Plays the game's animation clips on peds (ScriptHookDotNet task wrappers) with timeouts, so a clip that fails
    // to load never traps a sequence. PlayAndWait is a coroutine: yield return Engine.Animations.PlayAndWait(...).
    public sealed class AnimationService
    {
        private readonly Dictionary<string, AnimationSet> sets = new Dictionary<string, AnimationSet>(StringComparer.OrdinalIgnoreCase);

        public AnimationSet Set(string name)
        {
            AnimationSet set;
            if (!sets.TryGetValue(name, out set)) { set = new AnimationSet(name); sets[name] = set; }
            return set;
        }

        public bool Play(Ped ped, string set, string clip, float blendSpeed)
        {
            try { ped.Task.PlayAnimation(Set(set), clip, blendSpeed); return true; }
            catch (Exception error) { RuntimeLog.Error("animation_play_failed " + set + "/" + clip + " error=" + error.Message); return false; }
        }

        public bool Play(Ped ped, string set, string clip, float blendSpeed, AnimationFlags flags)
        {
            try { ped.Task.PlayAnimation(Set(set), clip, blendSpeed, flags); return true; }
            catch (Exception error) { RuntimeLog.Error("animation_play_failed " + set + "/" + clip + " error=" + error.Message); return false; }
        }

        public bool IsPlaying(Ped ped, string set, string clip)
        {
            try { return ped.Animation.isPlaying(Set(set), clip); }
            catch (Exception) { return false; }
        }

        // Starts the clip, waits until it has started and then stopped playing, or until timeoutMs.
        public IEnumerator PlayAndWait(Ped ped, string set, string clip, float blendSpeed, int minimumMs, int timeoutMs)
        {
            if (!Play(ped, set, clip, blendSpeed)) { yield break; }
            int started = Environment.TickCount;
            yield return Wait.Milliseconds(minimumMs);
            yield return Wait.Until(() => !ped.Exists() || !IsPlaying(ped, set, clip), Math.Max(0, timeoutMs - (Environment.TickCount - started)));
        }
    }
}