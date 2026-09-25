using System.Collections.Generic;

namespace LibertyFramework.Models
{
    // One grmShaderFx: its effect name (e.g. gta_default), preset (.sps) and the texture names it samples.
    internal sealed class DrawableShader
    {
        internal uint Address;
        internal string Name;
        internal string Preset;
        internal readonly List<string> Textures = new List<string>();
        // Address of each texture reference's name pointer, for renaming.
        internal readonly List<uint> TextureNameSlots = new List<uint>();
    }
}
