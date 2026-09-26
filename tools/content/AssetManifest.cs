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
        // "template" (default): the template's dictionary with its texture's pixels replaced (proven in game).
        // "native": a dictionary written from scratch by TextureDictionaryWriter (source size, full mip chain, DXT1 or DXT5).
        [DataMember(Name = "textureMode", IsRequired = false)] internal string TextureMode;

        internal const string TextureModeTemplate = "template";
        internal const string TextureModeNative = "native";

        internal string Directory;

        internal static AssetManifest Load(string path)
        {
            AssetManifest manifest;
            using (FileStream stream = File.OpenRead(path)) { manifest = Parse(stream, path); }
            manifest.Directory = Path.GetDirectoryName(Path.GetFullPath(path));
            return manifest;
        }

        // Reads and checks a manifest (path names it in errors). Directory is left unset.
        internal static AssetManifest Parse(Stream stream, string path)
        {
            AssetManifest manifest = (AssetManifest)new DataContractJsonSerializer(typeof(AssetManifest)).ReadObject(stream);
            if (manifest.SchemaVersion != 1) { throw new InvalidDataException(path + ": schemaVersion must be 1"); }
            if (manifest.Type != "prop") { throw new InvalidDataException(path + ": type '" + manifest.Type + "' is not supported yet (prop)"); }
            if (string.IsNullOrEmpty(manifest.Name) || manifest.Name.Length > 23) { throw new InvalidDataException(path + ": name must be 1-23 characters"); }
            if (string.IsNullOrEmpty(manifest.TextureDictionary) || manifest.TextureDictionary.Length > 23) { throw new InvalidDataException(path + ": textureDictionary must be 1-23 characters"); }
            if (!(manifest.DrawDistanceMeters > 0 && manifest.DrawDistanceMeters <= 1500)) { throw new InvalidDataException(path + ": drawDistanceMeters must be 0-1500"); }
            if (string.IsNullOrEmpty(manifest.TextureMode)) { manifest.TextureMode = TextureModeTemplate; }
            if (manifest.TextureMode != TextureModeTemplate && manifest.TextureMode != TextureModeNative)
            {
                throw new InvalidDataException(path + ": textureMode '" + manifest.TextureMode + "' must be '" + TextureModeTemplate + "' or '" + TextureModeNative + "'");
            }
            return manifest;
        }

        internal string SourcePath { get { return Path.Combine(Directory, Source); } }
    }
}
