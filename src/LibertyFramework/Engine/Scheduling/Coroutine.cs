using System.Collections;

namespace LibertyFramework.Engine.Scheduling
{
    // A running sequence owned by a module. Stops with its owner.
    public sealed class Coroutine
    {
        internal Coroutine(Module owner, string name, IEnumerator routine) { Owner = owner; Name = name; Routine = routine; }

        public Module Owner { get; private set; }
        public string Name { get; private set; }
        public bool Finished { get; internal set; }
        // True when the last Wait.Until ended by timeout rather than by its condition.
        public bool LastTimedOut { get; internal set; }
        internal IEnumerator Routine { get; private set; }
        internal Wait Current { get; set; }

        public void Stop() { Finished = true; }
    }
}