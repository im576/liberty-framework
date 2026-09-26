using System;
using System.Collections;
using System.Collections.Generic;
using Liberty.Sdk;
using LibertyFramework.Core.Logging;

namespace LibertyFramework.Engine.Animation
{
    // Choreography engine (generalised from the Arsenal trunk sequence): a list of steps run as one coroutine. Every
    // step ends by its own rule or a maximum time, so a clip that never loads cannot trap the sequence. Peds taking
    // part are checked every frame; if one disappears or dies the sequence cancels. OnCancel always runs when the
    // sequence does not complete, including when the owning module stops (resource ledger entry).
    internal sealed class Choreography : IChoreographyBuilder, IChoreography
    {
        private enum Kind { TurnTo, Play, Start, Wait, WaitUntil, Do, Loop }

        private sealed class Step
        {
            internal Kind Kind;
            internal PedRef Ped;
            internal AnimClip Clip;
            internal AnimOptions Options;
            internal Vec3 Target;
            internal int MinMs, MaxMs;
            internal Func<bool> Condition;
            internal Action Action;
            internal List<Step> Body;
            internal readonly List<KeyValuePair<int, Action>> Timed = new List<KeyValuePair<int, Action>>();
        }

        // Clip loading may take a few frames; a step that cannot start within this time is skipped (logged).
        private const int StartTimeoutMs = 2500;
        private static long nextId;

        private readonly LibertyEngine engine;
        private readonly LibertyModule owner;
        private readonly List<Step> steps = new List<Step>();
        private readonly HashSet<PedRef> cast = new HashSet<PedRef>();
        private readonly List<KeyValuePair<int, Action>> pendingTimed = new List<KeyValuePair<int, Action>>();
        private List<Step> target;
        private Action onComplete, onCancel;
        private ICoroutine coroutine;
        private long id;
        private bool cancelled, finished;

        internal Choreography(LibertyEngine engine, LibertyModule owner, string name)
        {
            this.engine = engine; this.owner = owner; Name = name;
            target = steps;
        }

        public string Name { get; private set; }
        public bool IsRunning { get { return coroutine != null && !finished; } }
        public bool Completed { get; private set; }
        public int StepIndex { get; private set; }

        private IChoreographyBuilder Add(Step step)
        {
            step.Timed.AddRange(pendingTimed);
            pendingTimed.Clear();
            if (!step.Ped.IsNone) { cast.Add(step.Ped); }
            target.Add(step);
            return this;
        }

        public IChoreographyBuilder TurnTo(PedRef ped, Vec3 at, int durationMs) { return Add(new Step { Kind = Kind.TurnTo, Ped = ped, Target = at, MaxMs = durationMs }); }

        public IChoreographyBuilder Play(PedRef ped, AnimClip clip, AnimOptions options, int minMs, int maxMs)
        {
            return Add(new Step { Kind = Kind.Play, Ped = ped, Clip = clip, Options = options, MinMs = minMs, MaxMs = maxMs });
        }

        public IChoreographyBuilder Start(PedRef ped, AnimClip clip, AnimOptions options) { return Add(new Step { Kind = Kind.Start, Ped = ped, Clip = clip, Options = options }); }

        public IChoreographyBuilder Wait(int milliseconds) { return Add(new Step { Kind = Kind.Wait, MaxMs = milliseconds }); }

        public IChoreographyBuilder WaitUntil(Func<bool> condition, int timeoutMs) { return Add(new Step { Kind = Kind.WaitUntil, Condition = condition, MaxMs = timeoutMs }); }

        public IChoreographyBuilder Do(Action action) { return Add(new Step { Kind = Kind.Do, Action = action }); }

        public IChoreographyBuilder At(int atMs, Action action)
        {
            pendingTimed.Add(new KeyValuePair<int, Action>(atMs, action));
            return this;
        }

        public IChoreographyBuilder LoopUntil(Func<bool> done, Action<IChoreographyBuilder> body)
        {
            Step loop = new Step { Kind = Kind.Loop, Condition = done, Body = new List<Step>() };
            Add(loop);
            List<Step> outer = target;
            target = loop.Body;
            try { body(this); }
            finally { target = outer; }
            return this;
        }

        public IChoreographyBuilder OnComplete(Action action) { onComplete = action; return this; }

        public IChoreographyBuilder OnCancel(Action action) { onCancel = action; return this; }

        public IChoreography Begin()
        {
            if (coroutine != null) { return this; }
            id = ++nextId;
            engine.Ledger.Add(owner, "choreography", id, () => { if (!Completed) { RunCancel("owner stopped"); } });
            coroutine = engine.Scheduler.Start(owner, "choreography:" + Name, Run());
            RuntimeLog.Info("[" + owner.Id + "] choreography_begin " + Name + " steps=" + steps.Count);
            return this;
        }

        public void Cancel()
        {
            if (finished) { return; }
            cancelled = true;
        }

        private IEnumerator Run()
        {
            IEnumerator body = RunSteps(steps, true);
            while (true)
            {
                if (cancelled || !CastAlive()) { Stop(cancelled ? "cancelled" : "a ped is gone"); yield break; }
                bool more;
                try { more = body.MoveNext(); }
                catch (Exception error)
                {
                    RuntimeLog.Error("[" + owner.Id + "] choreography_failed " + Name + " step=" + StepIndex + " error=" + error.Message);
                    Stop("error");
                    yield break;
                }
                if (!more) { break; }
                yield return body.Current;
            }
            finished = true;
            Completed = true;
            engine.Ledger.Forget(owner, "choreography", id);
            RuntimeLog.Info("[" + owner.Id + "] choreography_complete " + Name);
            if (onComplete != null) { onComplete(); }
        }

        private void Stop(string reason)
        {
            finished = true;
            engine.Ledger.Forget(owner, "choreography", id);
            RunCancel(reason);
        }

        private void RunCancel(string reason)
        {
            finished = true;
            RuntimeLog.Info("[" + owner.Id + "] choreography_cancel " + Name + " step=" + StepIndex + " reason=" + reason);
            if (onCancel == null) { return; }
            try { onCancel(); }
            catch (Exception error) { RuntimeLog.Error("[" + owner.Id + "] choreography_cancel_failed " + Name + " error=" + error.Message); }
        }

        private bool CastAlive()
        {
            foreach (PedRef ped in cast)
            {
                if (!engine.Peds.Exists(ped) || engine.Peds.IsDead(ped)) { return false; }
            }
            return true;
        }

        private IEnumerator RunSteps(List<Step> list, bool top)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (top) { StepIndex = i; }
                Step step = list[i];
                if (step.Kind == Kind.Loop)
                {
                    while (!step.Condition())
                    {
                        IEnumerator inner = RunSteps(step.Body, false);
                        while (inner.MoveNext()) { yield return inner.Current; }
                        yield return null;
                    }
                    continue;
                }
                IEnumerator run = RunStep(step);
                while (run.MoveNext()) { yield return run.Current; }
            }
        }

        private IEnumerator RunStep(Step step)
        {
            int start = Environment.TickCount;
            List<KeyValuePair<int, Action>> timed = step.Timed.Count > 0 ? new List<KeyValuePair<int, Action>>(step.Timed) : null;
            switch (step.Kind)
            {
                case Kind.Do:
                    step.Action();
                    yield break;
                case Kind.TurnTo:
                    engine.Tasks.TurnTo(step.Ped, step.Target);
                    break;
                case Kind.Play:
                case Kind.Start:
                    while (!engine.Animation.Play(step.Ped, step.Clip, step.Options))
                    {
                        if (Elapsed(start) >= StartTimeoutMs)
                        {
                            RuntimeLog.Error("[" + owner.Id + "] choreography_clip_unavailable " + Name + " clip=" + step.Clip);
                            yield break;
                        }
                        yield return null;
                    }
                    start = Environment.TickCount;
                    if (step.Kind == Kind.Start) { yield break; }
                    break;
            }
            while (true)
            {
                int elapsed = Elapsed(start);
                if (timed != null)
                {
                    for (int t = timed.Count - 1; t >= 0; t--)
                    {
                        if (elapsed < timed[t].Key) { continue; }
                        Action action = timed[t].Value;
                        timed.RemoveAt(t);
                        action();
                    }
                }
                if (Done(step, elapsed)) { break; }
                yield return null;
            }
            // Timed actions scheduled past the end of the step still run (a door must not stay open).
            if (timed != null) { foreach (KeyValuePair<int, Action> pair in timed) { pair.Value(); } }
        }

        private bool Done(Step step, int elapsed)
        {
            switch (step.Kind)
            {
                case Kind.TurnTo:
                case Kind.Wait: return elapsed >= step.MaxMs;
                case Kind.WaitUntil: return step.Condition() || elapsed >= step.MaxMs;
                case Kind.Play: return elapsed >= step.MaxMs || (elapsed >= step.MinMs && !engine.Animation.IsPlaying(step.Ped, step.Clip));
                default: return true;
            }
        }

        private static int Elapsed(int start) { return unchecked(Environment.TickCount - start); }
    }
}
