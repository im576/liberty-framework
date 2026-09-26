using System;

namespace Liberty.Sdk
{
    // Typed publish/subscribe on the engine thread. Game events live in Liberty.Sdk.Events; modules may publish their
    // own structs for other modules. Subscriptions end automatically when the subscribing module stops.
    public interface IEventBus
    {
        void Subscribe<T>(LibertyModule owner, Action<T> handler) where T : struct;
        void Unsubscribe<T>(LibertyModule owner, Action<T> handler) where T : struct;
        void Publish<T>(T value) where T : struct;
    }
}