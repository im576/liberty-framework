using System;
using System.Collections.Generic;
using Liberty.Sdk;

namespace LibertyFramework.Engine.Events
{
    // SDK IEventBus: typed, synchronous publish/subscribe on the engine thread. A throwing handler fails only its own
    // module. Events are structs, so publishing allocates nothing per event.
    public sealed class EventBus : IEventBus
    {
        private interface IChannel { void RemoveOwner(LibertyModule owner); }

        private sealed class Handler<T> where T : struct
        {
            internal LibertyModule Owner;
            internal Action<T> Action;
            internal bool Removed;
        }

        // Removal during a publish (a handler unsubscribes, or fails and its module's handlers go) only marks entries: a
        // RemoveAll there would shift the list under the publishing loop and skip the next module's handler. The list
        // is compacted when no publish of this type is running.
        private sealed class Channel<T> : IChannel where T : struct
        {
            internal readonly List<Handler<T>> Handlers = new List<Handler<T>>();
            internal int Publishing;
            internal bool Dirty;

            public void RemoveOwner(LibertyModule owner) { Remove(h => h.Owner == owner); }

            internal void Remove(Predicate<Handler<T>> match)
            {
                foreach (Handler<T> h in Handlers) { if (!h.Removed && match(h)) { h.Removed = true; Dirty = true; } }
                Compact();
            }

            internal void Compact()
            {
                if (Publishing > 0 || !Dirty) { return; }
                Handlers.RemoveAll(h => h.Removed);
                Dirty = false;
            }
        }

        private readonly Dictionary<Type, IChannel> channels = new Dictionary<Type, IChannel>();
        private readonly Action<LibertyModule, Exception> onHandlerFailed;
        // Sets the engine's current-module context around each handler (capability checks, implicit ownership).
        internal Action<LibertyModule> Enter = m => { };
        internal Func<LibertyModule> Current = () => null;

        internal EventBus(Action<LibertyModule, Exception> onHandlerFailed) { this.onHandlerFailed = onHandlerFailed; }

        public void Subscribe<T>(LibertyModule owner, Action<T> handler) where T : struct
        {
            if (owner == null || handler == null) { throw new ArgumentNullException(owner == null ? "owner" : "handler"); }
            Get<T>(true).Handlers.Add(new Handler<T> { Owner = owner, Action = handler });
        }

        public void Unsubscribe<T>(LibertyModule owner, Action<T> handler) where T : struct
        {
            Channel<T> channel = Get<T>(false);
            if (channel != null) { channel.Remove(h => h.Owner == owner && h.Action == handler); }
        }

        // Each handler registered when the publish starts sees the event once, in subscription order; a handler added
        // during the publish (by another handler) first sees the next event.
        public void Publish<T>(T value) where T : struct
        {
            Channel<T> channel = Get<T>(false);
            if (channel == null) { return; }
            int count = channel.Handlers.Count;
            channel.Publishing++;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Handler<T> h = channel.Handlers[i];
                    if (h.Removed || !h.Owner.Running) { continue; }
                    LibertyModule previous = Current();
                    Enter(h.Owner);
                    try { h.Action(value); }
                    catch (Exception error) { onHandlerFailed(h.Owner, error); }
                    finally { Enter(previous); }
                }
            }
            finally
            {
                channel.Publishing--;
                channel.Compact();
            }
        }

        internal bool HasSubscribers<T>() where T : struct
        {
            Channel<T> c = Get<T>(false);
            if (c == null) { return false; }
            foreach (Handler<T> h in c.Handlers) { if (!h.Removed) { return true; } }
            return false;
        }

        internal void RemoveOwner(LibertyModule owner) { foreach (IChannel channel in channels.Values) { channel.RemoveOwner(owner); } }

        private Channel<T> Get<T>(bool create) where T : struct
        {
            IChannel channel;
            if (!channels.TryGetValue(typeof(T), out channel) && create)
            {
                channel = new Channel<T>();
                channels[typeof(T)] = channel;
            }
            return (Channel<T>)channel;
        }
    }
}
