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

        private sealed class Channel<T> : IChannel where T : struct
        {
            internal readonly List<KeyValuePair<LibertyModule, Action<T>>> Handlers = new List<KeyValuePair<LibertyModule, Action<T>>>();
            public void RemoveOwner(LibertyModule owner) { Handlers.RemoveAll(pair => pair.Key == owner); }
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
            Get<T>(true).Handlers.Add(new KeyValuePair<LibertyModule, Action<T>>(owner, handler));
        }

        public void Unsubscribe<T>(LibertyModule owner, Action<T> handler) where T : struct
        {
            Channel<T> channel = Get<T>(false);
            if (channel != null) { channel.Handlers.RemoveAll(pair => pair.Key == owner && pair.Value == handler); }
        }

        public void Publish<T>(T value) where T : struct
        {
            Channel<T> channel = Get<T>(false);
            if (channel == null) { return; }
            for (int i = 0; i < channel.Handlers.Count; i++)
            {
                KeyValuePair<LibertyModule, Action<T>> pair = channel.Handlers[i];
                if (!pair.Key.Running) { continue; }
                LibertyModule previous = Current();
                Enter(pair.Key);
                try { pair.Value(value); }
                catch (Exception error) { onHandlerFailed(pair.Key, error); }
                finally { Enter(previous); }
            }
        }

        internal bool HasSubscribers<T>() where T : struct { Channel<T> c = Get<T>(false); return c != null && c.Handlers.Count > 0; }

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