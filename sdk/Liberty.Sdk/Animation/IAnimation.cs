namespace Liberty.Sdk
{
    // Animation clips and choreography. Dictionaries requested here are released when the module stops.
    public interface IAnimation
    {
        bool Play(PedRef ped, AnimClip clip, AnimOptions options);
        bool IsPlaying(PedRef ped, AnimClip clip);
        // Normalized time (0-1) of a playing clip, or -1 when not playing.
        float GetTime(PedRef ped, AnimClip clip);
        void Stop(PedRef ped, AnimClip clip);
        // Ends every scripted clip and task on the ped.
        void StopAll(PedRef ped);
        // Starts building a sequence of steps; call Start() on the result.
        IChoreographyBuilder Choreography(LibertyModule owner, string name);
    }
}