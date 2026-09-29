using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [DataContract]
        internal sealed class CollisionRef
        {
            [DataMember(Name = "borrow", IsRequired = false)] internal TemplateRef Borrow;
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
        // Optional: the distance up to which each LOD is drawn, one entry per LOD level from LOD 0, ascending, the last at
        // most drawDistanceMeters. Authoring intent for the LOD writer; the validator checks it against the asset's LODs.
        [DataMember(Name = "lodDistancesMeters", IsRequired = false)] internal float[] LodDistancesMeters;
        // "auto" (default, absent): the structure writer when the asset needs it (ResolveWriter), else v1. "template": v1, the
        // template's single geometry patched (proven in game). "structure" (NEEDS-PLAYTEST): DrawableStructureBuilder over
        // structureTemplate, writing every LOD and one geometry per material (CompilerCapabilities.Structure), with native
        // textures unless textureMode says template (LCC038).
        [DataMember(Name = "drawableWriter", IsRequired = false)] internal string DrawableWriter;
        // The structure writer's template: a drawable with enough LOD slots, geometries and gta_default shaders. model
        // "auto" takes the first suitable drawable (by name) in archive, and archive "*" searches every IMG of the game.
        // Absent: "*" / "auto".
        [DataMember(Name = "structureTemplate", IsRequired = false)] internal TemplateRef StructureTemplate;
        // Optional: { "borrow": { archive, model } } ships a vanilla prop's own bounds resource under this model's name
        // (BorrowedCollision; NEEDS-PLAYTEST, T-032). model "auto" takes the first prop candidate, archive "*" searches every IMG.
        [DataMember(Name = "collision", IsRequired = false)] internal CollisionRef Collision;

        internal const string TextureModeTemplate = "template";
        internal const string TextureModeNative = "native";
        // "prop": a model spawned by scripts (IDE weap entry, v1). "object": a static world object placed in the map, with
        // collision (IDE objs + placement: NEXT_SESSIONS sessions 5-6). The compiler's capabilities decide which it builds.
        internal const string TypeProp = "prop";
        internal const string TypeObject = "object";
        internal static readonly string[] Types = { TypeProp, TypeObject };
        internal const float MaxDrawDistanceMeters = 1500;
        internal const string WriterAuto = "auto";
        internal const string WriterTemplate = "template";
        internal const string WriterStructure = "structure";
        internal const string AutoTemplate = "auto";
        internal const string AnyArchive = "*";

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
            if (System.Array.IndexOf(Types, manifest.Type) < 0) { throw new InvalidDataException(path + ": type '" + manifest.Type + "' is unknown (" + string.Join(", ", Types) + ")"); }
            if (string.IsNullOrEmpty(manifest.Name) || manifest.Name.Length > 23) { throw new InvalidDataException(path + ": name must be 1-23 characters"); }
            if (string.IsNullOrEmpty(manifest.TextureDictionary) || manifest.TextureDictionary.Length > 23) { throw new InvalidDataException(path + ": textureDictionary must be 1-23 characters"); }
            if (!(manifest.DrawDistanceMeters > 0 && manifest.DrawDistanceMeters <= MaxDrawDistanceMeters)) { throw new InvalidDataException(path + ": drawDistanceMeters must be 0-" + MaxDrawDistanceMeters); }
            if (manifest.LodDistancesMeters != null)
            {
                if (manifest.LodDistancesMeters.Length == 0 || manifest.LodDistancesMeters.Length > CompilerCapabilities.DrawableLodSlots)
                {
                    throw new InvalidDataException(path + ": lodDistancesMeters must have 1-" + CompilerCapabilities.DrawableLodSlots + " entries (one per LOD)");
                }
                foreach (float distance in manifest.LodDistancesMeters)
                {
                    if (!(distance > 0 && distance <= MaxDrawDistanceMeters)) { throw new InvalidDataException(path + ": lodDistancesMeters entries must be 0-" + MaxDrawDistanceMeters); }
                }
            }
            manifest.TextureModeExplicit = !string.IsNullOrEmpty(manifest.TextureMode);
            if (string.IsNullOrEmpty(manifest.TextureMode)) { manifest.TextureMode = TextureModeTemplate; }
            if (manifest.TextureMode != TextureModeTemplate && manifest.TextureMode != TextureModeNative)
            {
                throw new InvalidDataException(path + ": textureMode '" + manifest.TextureMode + "' must be '" + TextureModeTemplate + "' or '" + TextureModeNative + "'");
            }
            if (string.IsNullOrEmpty(manifest.DrawableWriter)) { manifest.DrawableWriter = WriterAuto; }
            if (manifest.DrawableWriter != WriterAuto && manifest.DrawableWriter != WriterTemplate && manifest.DrawableWriter != WriterStructure)
            {
                throw new InvalidDataException(path + ": drawableWriter '" + manifest.DrawableWriter + "' must be '" + WriterAuto + "', '" + WriterTemplate + "' or '" + WriterStructure + "'");
            }
            if (manifest.Collision != null && (manifest.Collision.Borrow == null || string.IsNullOrEmpty(manifest.Collision.Borrow.Archive) || string.IsNullOrEmpty(manifest.Collision.Borrow.Model)))
            {
                throw new InvalidDataException(path + ": collision needs borrow { archive, model } (the only collision a build can ship until the bounds layout is known)");
            }
            if (manifest.StructureTemplate != null && (string.IsNullOrEmpty(manifest.StructureTemplate.Archive) || string.IsNullOrEmpty(manifest.StructureTemplate.Model)))
            {
                throw new InvalidDataException(path + ": structureTemplate needs archive and model");
            }
            return manifest;
        }

        // Whether asset.json set textureMode (absent means template, but the structure writer then uses native).
        internal bool TextureModeExplicit;
        // Why ResolveWriter chose the writer, for report.json.
        internal string WriterReason;

        // Settles "auto": the structure writer when a LOD has several materials (v1 writes one), or when the asset has several
        // LODs and asks for native textures (v1 would drop the extra LODs); otherwise v1, which every shipped single-geometry
        // asset was proven with in game. The structure writer uses native textures unless textureMode says template.
        internal void ResolveWriter(ContentAsset asset)
        {
            if (DrawableWriter == WriterAuto)
            {
                List<ContentLod> lods = asset.BuildLods().Where(l => l.TriangleCount > 0).ToList();
                ContentLod crowded = lods.FirstOrDefault(l => l.Groups.Count(g => g.TriangleCount > 0) > 1);
                if (crowded != null)
                {
                    DrawableWriter = WriterStructure;
                    WriterReason = "auto: LOD " + crowded.Level + " has " + crowded.Groups.Count(g => g.TriangleCount > 0) + " materials";
                }
                else if (lods.Count > 1 && TextureModeExplicit && TextureMode == TextureModeNative)
                {
                    DrawableWriter = WriterStructure;
                    WriterReason = "auto: " + lods.Count + " LODs with native textures";
                }
                else
                {
                    DrawableWriter = WriterTemplate;
                    WriterReason = "auto: one material per LOD" + (lods.Count > 1 && !(TextureModeExplicit && TextureMode == TextureModeNative) ? " (extra LODs are written only with textureMode native)" : "");
                }
            }
            else if (WriterReason == null) { WriterReason = "asset.json"; }
            if (DrawableWriter == WriterStructure && !TextureModeExplicit) { TextureMode = TextureModeNative; }
        }

        // The structure writer's template reference: structureTemplate, else a search of every archive.
        internal TemplateRef StructureTemplateOrDefault { get { return StructureTemplate ?? new TemplateRef { Archive = AnyArchive, Model = AutoTemplate }; } }

        internal bool BorrowsCollision { get { return Collision != null && Collision.Borrow != null; } }

        internal string SourcePath { get { return Path.Combine(Directory, Source); } }
    }
}
