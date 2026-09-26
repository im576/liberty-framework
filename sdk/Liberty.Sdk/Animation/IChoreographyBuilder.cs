using System;

namespace Liberty.Sdk
{
    // Fluent description of a sequence. Steps run in order; each step ends by its own rule and a maximum time, so a
    // missing clip never traps the sequence. Cancel() or the owner stopping runs the OnCancel cleanup.
    public interface IChoreographyBuilder
    {
        // Turn the ped toward a point (ends after durationMs).
        IChoreographyBuilder TurnTo(PedRef ped, Vec3 target, int durationMs);
        // Play a clip; the step ends when the clip stops (after at least minMs) or at maxMs.
        IChoreographyBuilder Play(PedRef ped, AnimClip clip, AnimOptions options, int minMs, int maxMs);
        // Start a clip without waiting for it (e.g. a looping idle); the next step starts immediately.
        IChoreographyBuilder Start(PedRef ped, AnimClip clip, AnimOptions options);
        IChoreographyBuilder Wait(int milliseconds);
        // Wait until the condition holds (or timeoutMs).
        IChoreographyBuilder WaitUntil(Func<bool> condition, int timeoutMs);
        // Run an action once (open a door, give a weapon, publish an event...).
        IChoreographyBuilder Do(Action action);
        // Run an action atMs into the NEXT step (timed callbacks inside a clip, e.g. open the lid at 520 ms).
        IChoreographyBuilder At(int atMs, Action action);
        // Repeat the steps added by body until the condition is true (checked between repetitions).
        IChoreographyBuilder LoopUntil(Func<bool> done, Action<IChoreographyBuilder> body);
        IChoreographyBuilder OnComplete(Action action);
        // Runs when the sequence is cancelled or its owner stops (restore doors, clear tasks...).
        IChoreographyBuilder OnCancel(Action action);
        IChoreography Begin();
    }
}