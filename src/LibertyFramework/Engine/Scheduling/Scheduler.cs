using System;
using System.Collections;
using System.Collections.Generic;

namespace LibertyFramework.Engine.Scheduling
{
    // Runs coroutines once per engine frame, before module updates. A coroutine yields Wait objects (or null for the
    // next frame). Choreography (trunk use), scenarios and staged effects are written as straight-line sequences.
    public sealed class Scheduler
    {
        private readonly List<Coroutine> running = new List<Coroutine>();
        private readonly Action<Module, Exception> onFailed;

        internal Scheduler(Action<Module, Exception> onFailed) { this.onFailed = onFailed; }

        public Coroutine Start(Module owner, string name, IEnumerator routine)
        {
            Coroutine coroutine = new Coroutine(owner, name, routine);
            running.Add(coroutine);
            return coroutine;
        }

        public int Count { get { return running.Count; } }

        internal void Run()
        {
            int now = Environment.TickCount;
            for (int i = 0; i < running.Count; i++)
            {
                Coroutine c = running[i];
                if (c.Finished || !c.Owner.Running) { c.Finished = true; continue; }
                if (c.Current != null && !c.Current.Ready(now)) { continue; }
                if (c.Current != null) { c.LastTimedOut = c.Current.TimedOut; }
                try
                {
                    if (!c.Routine.MoveNext()) { c.Finished = true; continue; }
                    c.Current = c.Routine.Current as Wait ?? Wait.NextFrame();
                }
                catch (Exception error)
                {
                    c.Finished = true;
                    onFailed(c.Owner, new InvalidOperationException("coroutine " + c.Name + " failed", error));
                }
            }
            running.RemoveAll(c => c.Finished);
        }

        internal void StopOwnedBy(Module owner)
        {
            foreach (Coroutine c in running) { if (c.Owner == owner) { c.Finished = true; } }
        }
    }
}