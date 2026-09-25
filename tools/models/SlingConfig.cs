using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

// Fields are assigned by the JSON serializer.
#pragma warning disable 0649
namespace LibertyFramework.Models
{
    // config/models/sling.json: which body meshes the straps are fitted to, and each strap's shoulder/hip anchors.
    [DataContract]
    internal sealed class SlingConfig
    {
        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "bodyArchive", IsRequired = true)] internal string BodyArchive;
        [DataMember(Name = "bodyPrefixes", IsRequired = true)] internal List<string> BodyPrefixes;
        [DataMember(Name = "skeletonModel", IsRequired = true)] internal string SkeletonModel;
        [DataMember(Name = "attachBone", IsRequired = true)] internal string AttachBone;
        [DataMember(Name = "templateArchive", IsRequired = true)] internal string TemplateArchive;
        [DataMember(Name = "templateModel", IsRequired = true)] internal string TemplateModel;
        [DataMember(Name = "clearanceMeters", IsRequired = true)] internal double ClearanceMeters;
        [DataMember(Name = "hipMarginMeters", IsRequired = true)] internal double HipMarginMeters;
        [DataMember(Name = "widthMeters", IsRequired = true)] internal double WidthMeters;
        [DataMember(Name = "thicknessMeters", IsRequired = true)] internal double ThicknessMeters;
        [DataMember(Name = "textureRepeatMeters", IsRequired = true)] internal double TextureRepeatMeters;
        [DataMember(Name = "segments", IsRequired = true)] internal int Segments;
        [DataMember(Name = "straps", IsRequired = true)] internal List<Strap> Straps;
        [DataMember(Name = "textureDictionary", IsRequired = true)] internal string TextureDictionary;
        [DataMember(Name = "drawDistanceMeters", IsRequired = true)] internal int DrawDistanceMeters;
        [DataMember(Name = "audioMaterial", IsRequired = true)] internal string AudioMaterial;
        [DataMember(Name = "leather", IsRequired = true)] internal Leather LeatherLook;

        [DataContract]
        internal sealed class Leather
        {
            [DataMember(Name = "baseColour", IsRequired = true)] internal int[] BaseColour;
            [DataMember(Name = "edgeColour", IsRequired = true)] internal int[] EdgeColour;
            [DataMember(Name = "stitchColour", IsRequired = true)] internal int[] StitchColour;
            [DataMember(Name = "grainStrength", IsRequired = true)] internal double GrainStrength;
            [DataMember(Name = "scuffStrength", IsRequired = true)] internal double ScuffStrength;
            [DataMember(Name = "stitchInsetFraction", IsRequired = true)] internal double StitchInsetFraction;
            [DataMember(Name = "stitchPeriodPixels", IsRequired = true)] internal int StitchPeriodPixels;
        }

        [DataContract]
        internal sealed class Strap
        {
            [DataMember(Name = "model", IsRequired = true)] internal string Model;
            [DataMember(Name = "shoulderBone", IsRequired = true)] internal string ShoulderBone;
            [DataMember(Name = "shoulderTowards", IsRequired = true)] internal string ShoulderTowards;
            [DataMember(Name = "shoulderFraction", IsRequired = true)] internal double ShoulderFraction;
            [DataMember(Name = "hipBone", IsRequired = true)] internal string HipBone;
            [DataMember(Name = "hipOffsetMeters", IsRequired = true)] internal double[] HipOffsetMeters;
        }

        internal static SlingConfig Load(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                SlingConfig config = (SlingConfig)new DataContractJsonSerializer(typeof(SlingConfig)).ReadObject(stream);
                if (config.SchemaVersion != 1 || config.Straps == null || config.Straps.Count == 0 || config.Segments < 8 ||
                    config.WidthMeters <= 0 || config.ThicknessMeters <= 0 || config.TextureRepeatMeters <= 0 || config.ClearanceMeters < 0)
                    { throw new InvalidDataException("invalid sling config " + path); }
                return config;
            }
        }
    }
}
