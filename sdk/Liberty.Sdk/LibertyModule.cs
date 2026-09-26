namespace Liberty.Sdk
{
    // Base class of every Liberty mod/module. Override the On* methods and use Liberty (the engine's services).
    // All calls happen on the engine thread; an exception from any of them disables only this module, and the
    // engine then releases everything the module owns (entities, cameras, FX, sounds, streaming, UI, input capture,
    // memory patches, player-control locks).
    public abstract class LibertyModule
    {
        public string Id { get { return Manifest != null ? Manifest.Id : GetType().Name; } }
        public ModuleManifest Manifest { get; internal set; }
        public bool Running { get; internal set; }
        public string FailureReason { get; internal set; }

        // Milliseconds between OnUpdate calls; 0 = every frame. The performance governor may lengthen it.
        public int Interval { get; set; }

        // The engine's services, available from OnStart on.
        protected ILiberty Liberty { get { return LibertyHost.Current; } }

        protected internal virtual void OnStart() { }
        protected internal virtual void OnUpdate() { }
        // Draw pass: only draw from state gathered in OnUpdate; never call game functions here.
        protected internal virtual void OnDraw(ICanvas canvas) { }
        protected internal virtual void OnStop() { }
        // The script domain is unloading (reload or exit): game functions are unavailable; only release managed state.
        protected internal virtual void OnUnload() { }
    }
}