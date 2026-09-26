using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

#pragma warning disable 0649

namespace LibertyFramework.Content
{
    // content/<kind>/<name>/asset.json: what to build from which source and how the game registers it.
    [DataContract]
    internal sealed class AssetManifest
    {
        [DataContract]
        internal sealed class TemplateRef
        {
            [DataMember(Name = "archive", IsRequired = true)] internal string Archive;
            [DataMember(Name = "model", IsRequired = true)] internal string Model;
        }

        [DataMember(Name = "schemaVersion", IsRequired = true)] internal int SchemaVersion;
        [DataMember(Name = "name", IsRequired = true)] internal string Name;
        [DataMember(Name = "type", IsRequired = true)] internal string Type;
        [DataMember(Name = "source", IsRequired = true)] internal string Source;
        [DataMember(Name = "template", IsRequired = true)] internal TemplateRef Template;
        [DataMember(Name = "textureDictionary", IsRequired = true)] internal string TextureDictionary;
        [DataMember(Name = "drawDistanceMeters", IsRequired = true)] internal float DrawDistanceMeters;
        [DataMember(Name = "audioMaterial", IsRequired = false)] internal string AudioMaterial;

        internal string Directory;

        internal static AssetManifest Load(string path)
        {
            AssetManifest manifest;
            using (FileStream stream = File.OpenRead(path)) { manifest = (AssetManifest)new DataContractJsonSerializer(typeof(AssetManifest)).ReadObject(stream); }
            manifest.Directory = Path.GetDirectoryName(Path.GetFullPath(path));
            if (manifest.SchemaVersion != 1) { throw new InvalidDataException(path + ": schemaVersion must be 1"); }
            if (manifest.Type != "prop") { throw new InvalidDataException(path + ": type '" + manifest.Type + "' is not supported yet (prop)"); }
            if (string.IsNullOrEmpty(manifest.Name) || manifest.Name.Length > 23) { throw new InvalidDataException(path + ": name must be 1-23 characters"); }
            if (string.IsNullOrEmpty(manifest.TextureDictionary) || manifest.TextureDictionary.Length > 23) { throw new InvalidDataException(path + ": textureDictionary must be 1-23 characters"); }
            if (!(manifest.DrawDistanceMeters > 0 && manifest.DrawDistanceMeters <= 1500)) { throw new InvalidDataException(path + ": drawDistanceMeters must be 0-1500"); }
            return manifest;
        }

        internal string SourcePath { get { return Path.Combine(Directory, Source); } }
    }
}
