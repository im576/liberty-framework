using System;
using System.Collections.Generic;

namespace LibertyFramework.Engine.Events
{
    // Typed, synchronous publish/subscribe (ADR-0006). Handlers run on the engine thread in subscription order; a
    // throwing handler fails only its own module. Events are structs, so publishing allocates nothing per event.
    public sealed class EventBus
    {
        private interface IChannel { void RemoveOwner(Module owner); }

        private sealed class Channel<T> : IChannel where T : struct
        {
            internal readonly List<KeyValuePair<Module, Action<T>>> Handlers = new List<KeyValuePair<Module, Action<T>>>();
            public void RemoveOwner(Module owner) { Handlers.RemoveAll(pair => pair.Key == owner); }
        }

        private readonly Dictionary<Type, IChannel> channels = new Dictionary<Type, IChannel>();
        private readonly Action<Module, Exception> onHandlerFailed;

        internal EventBus(Action<Module, Exception> onHandlerFailed) { this.onHandlerFailed = onHandlerFailed; }

        public void Subscribe<T>(Module owner, Action<T> handler) where T : struct
        {
            if (owner == null || handler == null) { throw new ArgumentNullException(owner == null ? "owner" : "handler"); }
            ((Channel<T>)Find(typeof(T), true)).Handlers.Add(new KeyValuePair<Module, Action<T>>(owner, handler));
        }

        public void Publish<T>(T value) where T : struct
        {
            Channel<T> channel = (Channel<T>)Find(typeof(T), false);
            if (channel == null) { return; }
            // Index loop: a handler may subscribe or its module may fail (and be removed) during dispatch.
            for (int i = 0; i < channel.Handlers.Count; i++)
            {
                KeyValuePair<Module, Action<T>> pair = channel.Handlers[i];
                if (!pair.Key.Running) { continue; }
                try { pair.Value(value); }
                catch (Exception error) { onHandlerFailed(pair.Key, error); }
            }
        }

        internal void RemoveOwner(Module owner) { foreach (IChannel channel in channels.Values) { channel.RemoveOwner(owner); } }


        private IChannel Find(Type type, bool create)
        {
            IChannel channel;
            if (!channels.TryGetValue(type, out channel) && create)
            {
                channel = (IChannel)Activator.CreateInstance(typeof(Channel<>).MakeGenericType(type));
                channels[type] = channel;
            }
            return channel;
        }
    }
}