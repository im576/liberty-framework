using System.Globalization;
using System.Linq;

namespace LibertyFramework.Content
{
    // What the current writers can produce. The validator checks assets against these limits, so the IR, the validator and
    // the Blender add-on can describe multi-material, multi-LOD, collision-carrying world objects while the compiler still
    // refuses, with a clear error, what it cannot write yet. A writer that gains a feature turns it on here, and nothing
    // else has to change. `LibertyContent capabilities` prints ToJson(); the Blender add-on's tests compare their copy of
    // these limits with it (tools/blender/liberty_exporter/checks.py CAPABILITIES).
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
        // Shaders the drawable writer can emit (a material's liberty_shader).
        internal readonly string[] Shaders;
        // asset.json types the compiler builds and packages ("prop": IDE weap entry; "object": static world object).
        internal readonly string[] AssetTypes;
        // Collision shapes the collision writer emits; empty = no collision writer.
        internal readonly string[] CollisionShapes;
        // Whether asset.json lodDistancesMeters is written into the drawable.
        internal readonly bool WritesLodDistances;

        internal CompilerCapabilities(string version, int maxMaterialsPerLod, int compiledLodLevels)
            : this(version, maxMaterialsPerLod, compiledLodLevels, new[] { "gta_default" }, new[] { AssetManifest.TypeProp }, new string[0], false)
        {
        }

        internal CompilerCapabilities(string version, int maxMaterialsPerLod, int compiledLodLevels, string[] shaders, string[] assetTypes, string[] collisionShapes, bool writesLodDistances)
        {
            Version = version;
            MaxMaterialsPerLod = maxMaterialsPerLod;
            CompiledLodLevels = compiledLodLevels;
            Shaders = shaders;
            AssetTypes = assetTypes;
            CollisionShapes = collisionShapes;
            WritesLodDistances = writesLodDistances;
        }

        internal static readonly CompilerCapabilities Current = new CompilerCapabilities("v1", V1MaxMaterialsPerLod, V1CompiledLodLevels);

        // drawableWriter "structure" (NEEDS-PLAYTEST, T-031): every LOD slot, one geometry per material, LOD distances.
        // How many materials a LOD can have is decided by the structure template; this bound only keeps the search sane.
        internal const int StructureMaxMaterialsPerLod = 16;
        internal static readonly CompilerCapabilities Structure = new CompilerCapabilities("v2-structure", StructureMaxMaterialsPerLod, DrawableLodSlots,
            new[] { "gta_default" }, new[] { AssetManifest.TypeProp }, new string[0], true);

        // The capabilities that apply to a manifest: the default writer's, unless it opts into the structure writer.
        internal static CompilerCapabilities For(AssetManifest manifest)
        {
            return manifest != null && manifest.DrawableWriter == AssetManifest.WriterStructure ? Structure : Current;
        }

        internal string ToJson()
        {
            return "{ \"version\": " + Quote(Version) + ", \"maxMaterialsPerLod\": " + MaxMaterialsPerLod.ToString(CultureInfo.InvariantCulture) +
                ", \"compiledLodLevels\": " + CompiledLodLevels.ToString(CultureInfo.InvariantCulture) + ", \"lodSlots\": " + DrawableLodSlots.ToString(CultureInfo.InvariantCulture) +
                ", \"maxVerticesPerGeometry\": " + MaxVerticesPerGeometry.ToString(CultureInfo.InvariantCulture) + ", \"shaders\": " + List(Shaders) +
                ", \"assetTypes\": " + List(AssetTypes) + ", \"collisionShapes\": " + List(CollisionShapes) + ", \"lodDistances\": " + (WritesLodDistances ? "true" : "false") + " }";
        }

        private static string List(string[] values) { return "[" + string.Join(", ", values.Select(Quote).ToArray()) + "]"; }
        private static string Quote(string text) { return "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""; }
    }
}
