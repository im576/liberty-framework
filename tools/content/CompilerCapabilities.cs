namespace LibertyFramework.Content
{
    // What the current writers can produce. The validator checks assets against these limits, so the IR and validator can
    // describe multi-material, multi-LOD assets while the compiler still rejects what it cannot write yet.
    internal sealed class CompilerCapabilities
    {
        // Version 1 (template patching): one material per LOD, LOD 0 compiled; other LODs are validated and reported only.
        internal const int V1MaxMaterialsPerLod = 1;
        internal const int V1CompiledLodLevels = 1;
        // A drawable has four LOD model-collection pointers (docs/research/ModelFormat.md, drawable +0x40), so LODs 0-3.
        internal const int DrawableLodSlots = 4;
        // 16-bit index buffers (ModelFormat.md: index buffer data is u16 indices).
        internal const int MaxVerticesPerGeometry = 65535;

        internal readonly string Version;
        internal readonly int MaxMaterialsPerLod;
        internal readonly int CompiledLodLevels;

        internal CompilerCapabilities(string version, int maxMaterialsPerLod, int compiledLodLevels)
        {
            Version = version;
            MaxMaterialsPerLod = maxMaterialsPerLod;
            CompiledLodLevels = compiledLodLevels;
        }

        internal static readonly CompilerCapabilities Current = new CompilerCapabilities("v1", V1MaxMaterialsPerLod, V1CompiledLodLevels);
    }
}
