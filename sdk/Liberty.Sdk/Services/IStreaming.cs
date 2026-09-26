namespace Liberty.Sdk
{
    // Model/animation/effect streaming with per-module reference counts: a resource stays requested while any module
    // holds it and is released when the last holder stops or releases it (keeps memory low on 8 GB machines).
    public interface IStreaming
    {
        bool IsValidModel(ModelRef model);
        // Requests and reports whether it is loaded now; call again on later frames.
        bool RequestModel(LibertyModule owner, ModelRef model);
        void ReleaseModel(LibertyModule owner, ModelRef model);
        bool RequestAnimations(LibertyModule owner, string dictionary);
        void ReleaseAnimations(LibertyModule owner, string dictionary);
        int HeldCount { get; }
    }
}