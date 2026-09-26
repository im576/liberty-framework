namespace Liberty.Sdk
{
    // Performance measurement and the governor's view of the machine.
    public interface IPerf
    {
        // Named cost sample: var t = Perf.Begin(); ...; Perf.End(this, "raycast", t). No allocation.
        long Begin();
        void End(LibertyModule owner, string name, long token);
        float FrameMs { get; }
        float FrameP95Ms { get; }
        // 0 = keep everything, 1 = shed as much optional work as possible (set by the governor from frame time,
        // address space and memory headroom). Modules scale optional effects by (1 - Pressure).
        float Pressure { get; }
        long ProcessPrivateBytes { get; }
        long AddressSpaceFreeBytes { get; }
        long ManagedHeapBytes { get; }
    }
}