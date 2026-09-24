using System.Collections.Generic;
using System.Runtime.Serialization;

#pragma warning disable 0649

namespace LibertyFramework.Finishes
{
    // assets/finishes/finishes.json: finish definitions (colour ramps) and the model variants that use them.
    [DataContract]
    internal sealed class FinishCatalogFile
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "finishes", IsRequired = true)] internal List<FinishDefinition> Finishes;
        [DataMember(Name = "variants", IsRequired = true)] internal List<ModelVariant> Variants;
    }

    [DataContract]
    internal sealed class FinishDefinition
    {
        [DataMember(Name = "id", IsRequired = true)] internal string Id;
        [DataMember(Name = "label", IsRequired = true)] internal string Label;
        [DataMember(Name = "diffuse", IsRequired = true)] internal ColourRamp Diffuse;
        [DataMember(Name = "specular", IsRequired = true)] internal ColourRamp Specular;
        [DataMember(Name = "icon", IsRequired = true)] internal ColourRamp Icon;
    }

    [DataContract]
    internal sealed class ColourRamp
    {
        [DataMember(Name = "shadowRgb", IsRequired = true)] internal int[] Shadow;
        [DataMember(Name = "midRgb", IsRequired = true)] internal int[] Mid;
        [DataMember(Name = "highlightRgb", IsRequired = true)] internal int[] Highlight;
        [DataMember(Name = "contrast", IsRequired = true)] internal double Contrast;
        // Luminance remap before the ramp: out = lift + (1 - lift) * in^gamma. Lift raises black plastic into the finish colour.
        [DataMember(Name = "lift", IsRequired = true)] internal double Lift;
        [DataMember(Name = "gamma", IsRequired = true)] internal double Gamma;
    }

    [DataContract]
    internal sealed class ModelVariant
    {
        [DataMember(Name = "variantModel", IsRequired = true)] internal string VariantModel;
        [DataMember(Name = "baseModel", IsRequired = true)] internal string BaseModel;
        [DataMember(Name = "sourceImg", IsRequired = true)] internal string SourceImg;
        [DataMember(Name = "finish", IsRequired = true)] internal string Finish;
        [DataMember(Name = "animGroup", IsRequired = true)] internal string AnimGroup;
        [DataMember(Name = "drawDistance", IsRequired = true)] internal int DrawDistance;
        [DataMember(Name = "audioMaterial", IsRequired = true)] internal string AudioMaterial;
        [DataMember(Name = "weaponInfoType", IsRequired = true)] internal string WeaponInfoType;
        [DataMember(Name = "textureRoles", IsRequired = true)] internal Dictionary<string, string> TextureRoles;
    }
}
