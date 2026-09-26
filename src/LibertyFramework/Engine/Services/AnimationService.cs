using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;
using LibertyFramework.Engine.Animation;

namespace LibertyFramework.Engine.Services
{
    // SDK IAnimation. Default clips go through ScriptHookDotNet's Task.PlayAnimation (the path the trunk sequence proved
    // in game). Clips with options use TASK_PLAY_ANIM / _UPPER_BODY / _SECONDARY(ped, clip, dictionary, blend, loop,
    // lockX, lockY, keepLastFrame, time) from Sanny Builder's GTA IV library, after the dictionary is streamed; the
    // dictionary is held for the module that is running (released when it stops).
    public sealed class AnimationService : IAnimation
    {
        private readonly LibertyEngine engine;
        private readonly Dictionary<string, AnimationSet> sets = new Dictionary<string, AnimationSet>(StringComparer.OrdinalIgnoreCase);

        internal AnimationService(LibertyEngine engine) { this.engine = engine; }

        private AnimationSet Set(string dictionary)
        {
            AnimationSet set;
            if (!sets.TryGetValue(dictionary, out set)) { set = new AnimationSet(dictionary); sets[dictionary] = set; }
            return set;
        }

        public bool Play(PedRef ped, AnimClip clip, AnimOptions options)
        {
            try
            {
                if (!options.Loop && !options.UpperBody && !options.HoldLastFrame && !options.Secondary)
                {
                    Ped wrapper = Handles.Ped(ped);
                    if (wrapper == null) { return false; }
                    wrapper.Task.PlayAnimation(Set(clip.Dictionary), clip.Name, options.BlendIn > 0 ? options.BlendIn : 4f);
                    return true;
                }
                LibertyModule owner = engine.CurrentModule;
                bool loaded = owner != null ? engine.Streaming.RequestAnimations(owner, clip.Dictionary) : RequestUnowned(clip.Dictionary);
                if (!loaded) { return false; }
                string native = options.Secondary ? "TASK_PLAY_ANIM_SECONDARY" : options.UpperBody ? "TASK_PLAY_ANIM_UPPER_BODY" : "TASK_PLAY_ANIM";
                Function.Call(native, ped.Handle, clip.Name, clip.Dictionary, options.BlendIn > 0 ? options.BlendIn : 4f,
                    options.Loop, false, false, options.HoldLastFrame, -1);
                return true;
            }
            catch (Exception error)
            {
                RuntimeLog.Error("animation_play_failed " + clip + " error=" + error.Message);
                return false;
            }
        }

        private static bool RequestUnowned(string dictionary)
        {
            Function.Call("REQUEST_ANIMS", dictionary);
            return Function.Call<bool>("HAVE_ANIMS_LOADED", dictionary);
        }

        public bool IsPlaying(PedRef ped, AnimClip clip)
        {
            try
            {
                Ped wrapper = Handles.Ped(ped);
                if (wrapper != null) { return wrapper.Animation.isPlaying(Set(clip.Dictionary), clip.Name); }
                return Function.Call<bool>("IS_CHAR_PLAYING_ANIM", ped.Handle, clip.Dictionary, clip.Name);
            }
            catch (Exception error)
            {
                RuntimeLog.Error("animation_query_failed " + clip + " error=" + error.Message);
                return false;
            }
        }

        public float GetTime(PedRef ped, AnimClip clip)
        {
            if (!IsPlaying(ped, clip)) { return -1f; }
            Pointer time = typeof(float);
            Function.Call("GET_CHAR_ANIM_CURRENT_TIME", ped.Handle, clip.Dictionary, clip.Name, time);
            return (float)time;
        }

        // GTA IV has no per-clip stop native; ending the ped's task ends its scripted clip.
        public void Stop(PedRef ped, AnimClip clip) { if (IsPlaying(ped, clip)) { Function.Call("CLEAR_CHAR_TASKS", ped.Handle); } }

        public void StopAll(PedRef ped) { Function.Call("CLEAR_CHAR_TASKS", ped.Handle); }

        public IChoreographyBuilder Choreography(LibertyModule owner, string name) { return new Choreography(engine, owner, name); }
    }
}
