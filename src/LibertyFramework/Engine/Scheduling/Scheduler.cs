using System;
using System.Collections;
using System.Collections.Generic;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Scheduling
{
    // SDK IScheduler: runs coroutines once per engine frame, before module updates.
    public sealed class Scheduler : IScheduler
    {
        private sealed class Coroutine : ICoroutine
        {
            internal Coroutine(LibertyModule owner, string name, IEnumerator routine) { Owner = owner; Name = name; Routine = routine; }
            internal readonly LibertyModule Owner;
            internal readonly IEnumerator Routine;
            internal Wait Current;
            public string Name { get; private set; }
            public bool Finished { get; internal set; }
            public bool LastTimedOut { get; internal set; }
            public void Cancel() { Finished = true; }
        }

        private readonly List<Coroutine> running = new List<Coroutine>();
        private readonly Action<LibertyModule, Exception> onFailed;

        internal Scheduler(Action<LibertyModule, Exception> onFailed) { this.onFailed = onFailed; }

        public ICoroutine Start(LibertyModule owner, string name, IEnumerator routine)
        {
            // A null owner would throw inside Run every frame for every module (Run reads Owner.Running); refuse it here,
            // where the exception fails only the caller.
            if (owner == null) { throw new ArgumentNullException("owner"); }
            if (routine == null) { throw new ArgumentNullException("routine"); }
            Coroutine c = new Coroutine(owner, name, routine);
            running.Add(c);
            return c;
        }

        public int Running { get { return running.Count; } }

        // enter(owner) before a coroutine runs and enter(null) after: the engine's current-module context.
        internal void Run(Action<LibertyModule> enter)
        {
            int now = Environment.TickCount;
            for (int i = 0; i < running.Count; i++)
            {
                Coroutine c = running[i];
                if (c.Finished || !c.Owner.Running) { c.Finished = true; continue; }
                enter(c.Owner);
                try
                {
                    // Step runs module code (a Wait.Until condition), so it belongs inside the try: a throwing condition
                    // fails its module instead of escaping Run and aborting every engine frame after this point.
                    if (c.Current != null)
                    {
                        if (!c.Current.Step(now)) { continue; }
                        c.LastTimedOut = c.Current.HasTimedOut;
                    }
                    if (!c.Routine.MoveNext()) { c.Finished = true; continue; }
                    c.Current = c.Routine.Current as Wait ?? Wait.NextFrame();
                }
                catch (Exception error)
                {
                    c.Finished = true;
                    onFailed(c.Owner, new InvalidOperationException("coroutine " + c.Name + " failed", error));
                }
                finally { enter(null); }
            }
            running.RemoveAll(c => c.Finished);
        }

        internal void StopOwnedBy(LibertyModule owner) { foreach (Coroutine c in running) { if (c.Owner == owner) { c.Finished = true; } } }
    }
}