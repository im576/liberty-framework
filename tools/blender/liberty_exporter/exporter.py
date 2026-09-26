# Writes content/<kind>/<name>/: <name>.gltf (+ .bin and textures) through Blender's own glTF exporter, and asset.json
# for LibertyContent. glTF settings are fixed so every export is what the compiler expects: Y-up (the compiler turns it
# back into Blender's Z-up), modifiers applied, extras on (liberty_lod on objects, liberty_shader on materials,
# liberty_* on the scene), no cameras, lights or animation.

import json
import os

import bpy

from . import checks

EXPORTER_VERSION = "0.1.0"
KIND_FOLDERS = {"prop": "props"}


class Exported:
    def __init__(self, folder, manifest, gltf):
        self.folder = folder
        self.manifest = manifest
        self.gltf = gltf


def asset_folder(content_root, settings):
    return os.path.join(content_root, KIND_FOLDERS[settings.asset_type], settings.asset_name)


def manifest_data(settings):
    data = {
        "schemaVersion": 1,
        "name": settings.asset_name,
        "type": settings.asset_type,
        "source": settings.asset_name + ".gltf",
        "template": {"archive": settings.template_archive, "model": settings.template_model},
        "textureDictionary": settings.texture_dictionary or settings.asset_name,
        "drawDistanceMeters": round(float(settings.draw_distance), 3),
    }
    if settings.audio_material:
        data["audioMaterial"] = settings.audio_material
    return data


def export(context, settings, content_root):
    """Exports the asset. The caller runs checks first; this raises on exporter failure."""
    objects = checks.export_objects(context, settings)
    if not objects:
        raise RuntimeError("nothing to export")
    if context.mode != 'OBJECT' and context.view_layer.objects.active is not None:
        bpy.ops.object.mode_set(mode='OBJECT')

    folder = asset_folder(content_root, settings)
    os.makedirs(folder, exist_ok=True)
    gltf = os.path.join(folder, settings.asset_name + ".gltf")
    _remove_previous(gltf)

    options = {
        "filepath": gltf,
        "check_existing": False,
        "export_format": 'GLTF_SEPARATE',
        "export_texture_dir": "",
        "export_image_format": 'AUTO',
        "export_yup": True,
        "export_apply": True,
        "export_extras": True,
        "export_texcoords": True,
        "export_normals": True,
        "export_materials": 'EXPORT',
        "export_cameras": False,
        "export_lights": False,
        "export_animations": False,
        "export_skins": False,
        "export_morph": False,
        "will_save_settings": False,
    }
    if settings.collection is not None:
        options["collection"] = settings.collection.name
    else:
        options["use_selection"] = True
    # Exporter options differ between Blender versions; pass only the ones this version has.
    available = set(bpy.ops.export_scene.gltf.get_rna_type().properties.keys())
    options = {key: value for key, value in options.items() if key in available}

    # A collection export names the glTF scene after the collection and takes its extras from it, not from the scene.
    tagged = settings.collection if settings.collection is not None else context.scene
    added = _tag(tagged)
    try:
        result = bpy.ops.export_scene.gltf(**options)
    finally:
        for key in added:
            del tagged[key]
    if 'FINISHED' not in result or not os.path.isfile(gltf):
        raise RuntimeError("the glTF exporter did not write " + gltf)

    manifest = os.path.join(folder, "asset.json")
    with open(manifest, "w", encoding="utf-8", newline="\n") as stream:
        json.dump(manifest_data(settings), stream, indent=2)
        stream.write("\n")
    return Exported(folder, manifest, gltf)


def _tag(owner):
    """Custom properties the exporter writes as glTF scene extras; LibertyContent copies liberty_* into report.json."""
    tags = {
        "liberty_exporter": EXPORTER_VERSION,
        "liberty_blender": bpy.app.version_string,
        "liberty_source": os.path.basename(bpy.data.filepath) or "unsaved",
    }
    added = []
    for key, value in tags.items():
        if key not in owner.keys():
            owner[key] = value
            added.append(key)
    return added


def _remove_previous(gltf):
    """Deletes the files the previous export of this asset wrote (its .gltf and the buffers/images it references)."""
    if not os.path.isfile(gltf):
        return
    folder = os.path.dirname(gltf)
    try:
        with open(gltf, "r", encoding="utf-8") as stream:
            document = json.load(stream)
    except (OSError, ValueError):
        document = {}
    for entry in document.get("buffers", []) + document.get("images", []):
        uri = entry.get("uri", "")
        if uri and "/" not in uri and "\\" not in uri and not uri.startswith("data:"):
            path = os.path.join(folder, bpy.path.native_pathsep(uri))
            if os.path.isfile(path):
                os.remove(path)
    os.remove(gltf)
