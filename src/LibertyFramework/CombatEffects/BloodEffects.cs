using System;
using System.Collections.Generic;
using GTA;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.CombatEffects
{
    // Every blood particle goes through here. GTA IV has two kinds of stock blood effect:
    // - one-shot (blood_gun_entry/exit, blood_death, blood_ped_mouth ...): TRIGGER_PTFX_ON_PED_BONE plays them once;
    // - looping (blood_artery*, blood_drips, blood_gun_mist, *_chunks ...): TRIGGER refuses them (playtest 2 log,
    //   and the engine check at 0xAA0D6E), so they are STARTed on the bone and STOPped after a duration.
    // Which kind an effect is gets learned from the first refusal. The spray from streams and chunks lands on the
    // ground and walls as the game's own blood decals.
    internal sealed class BloodEffects
    {
        private sealed class Loop
        {
            internal int Handle;
            internal long StopAt;
        }

        private readonly HashSet<string> looping = new HashSet<string>();
        private readonly Dictionary<string, int> logged = new Dictionary<string, int>();
        private readonly List<BloodEmitter> pulses = new List<BloodEmitter>();
        private readonly List<Loop> loops = new List<Loop>();
        private bool callFailureLogged;

        internal int ActiveLoops { get { return loops.Count; } }

        // Plays 'effect' on the bone. A looping effect runs for durationMilliseconds; a one-shot effect plays once, or
        // repeats every intervalMilliseconds for durationMilliseconds when both are set. Returns whether it spawned.
        internal bool Play(CombatEffectsConfig config, string effect, Ped ped, int bone, float scale, long now, int durationMilliseconds, int intervalMilliseconds)
        {
            if (string.IsNullOrEmpty(effect) || ped == null || !ped.Exists() || scale <= 0) { return false; }
            if (!looping.Contains(effect))
            {
                bool ok = Call(() => CombatEffectsNatives.Burst(effect, ped, bone, scale), effect);
                Log(effect, bone, scale, ok ? "trigger" : "refused");
                if (ok)
                {
                    if (durationMilliseconds > 0 && intervalMilliseconds > 0) { AddPulse(config, effect, ped, bone, scale, now, durationMilliseconds, intervalMilliseconds); }
                    return true;
                }
                looping.Add(effect);
            }
            int duration = durationMilliseconds > 0 ? durationMilliseconds : config.BurstLoopMilliseconds;
            if (loops.Count >= config.MaximumLoopedEffects) { StopLoop(0); }
            int handle = 0;
            Call(() => { handle = CombatEffectsNatives.Start(effect, ped, bone, scale); return handle != 0; }, effect);
            Log(effect, bone, scale, handle != 0 ? "loop " + duration + "ms" : "loop_failed");
            if (handle == 0) { return false; }
            Loop loop = new Loop();
            loop.Handle = handle;
            loop.StopAt = now + duration;
            loops.Add(loop);
            return true;
        }

        private void AddPulse(CombatEffectsConfig config, string effect, Ped ped, int bone, float scale, long now, int duration, int interval)
        {
            if (pulses.Count >= config.MaximumEmitters) { pulses.RemoveAt(0); }
            BloodEmitter pulse = new BloodEmitter();
            pulse.Ped = ped; pulse.Bone = bone; pulse.Effect = effect; pulse.Scale = scale;
            pulse.IntervalMilliseconds = interval; pulse.NextMilliseconds = now + interval; pulse.UntilMilliseconds = now + duration;
            pulses.Add(pulse);
        }

        internal void Update(long now)
        {
            for (int i = loops.Count - 1; i >= 0; i--) { if (now >= loops[i].StopAt) { StopLoop(i); } }
            for (int i = pulses.Count - 1; i >= 0; i--)
            {
                BloodEmitter pulse = pulses[i];
                if (pulse.Ped == null || !pulse.Ped.Exists() || now > pulse.UntilMilliseconds) { pulses.RemoveAt(i); continue; }
                if (now < pulse.NextMilliseconds) { continue; }
                pulse.NextMilliseconds = now + pulse.IntervalMilliseconds;
                Call(() => CombatEffectsNatives.Burst(pulse.Effect, pulse.Ped, pulse.Bone, pulse.Scale), pulse.Effect);
            }
        }

        private void StopLoop(int index)
        {
            int handle = loops[index].Handle;
            loops.RemoveAt(index);
            Call(() => { CombatEffectsNatives.Stop(handle); return true; }, "stop");
        }

        // Natives only: call from a script tick, never from unload.
        internal void StopAll()
        {
            while (loops.Count > 0) { StopLoop(loops.Count - 1); }
            pulses.Clear();
        }

        private bool Call(Func<bool> action, string effect)
        {
            try { return action(); }
            catch (Exception error)
            {
                if (!callFailureLogged) { callFailureLogged = true; RuntimeLog.Error("ptfx_call_failed effect=" + effect + " error=" + error.Message); }
                return false;
            }
        }

        private void Log(string effect, int bone, float scale, string result)
        {
            int count;
            logged.TryGetValue(effect, out count);
            if (count >= 2) { return; }
            logged[effect] = count + 1;
            RuntimeLog.Info("ptfx effect=" + effect + " bone=0x" + bone.ToString("X") + " scale=" + scale.ToString("0.0") + " " + result);
        }
    }
}
