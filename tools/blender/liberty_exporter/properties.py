# Per-scene asset settings: everything asset.json needs. Stored as registered properties (saved in the .blend).

import bpy
from bpy.props import EnumProperty, FloatProperty, PointerProperty, StringProperty


class LibertyAssetSettings(bpy.types.PropertyGroup):
    asset_name: StringProperty(
        name="Name", maxlen=23,
        description="Model name in game (1-23 letters, digits or _); also the asset folder name")
    asset_type: EnumProperty(
        name="Type", items=[('prop', "Prop", "Static prop spawned by scripts (compiler v1)")], default='prop')
    collection: PointerProperty(
        name="Collection", type=bpy.types.Collection,
        description="Objects to export (with nested collections). Empty: the selected objects")
    template_archive: StringProperty(
        name="Template archive", default="pc/models/cdimages/weapons.img",
        description="Game archive holding the template drawable (list candidates with LibertyContent templates)")
    template_model: StringProperty(
        name="Template model", default="amb_nailgun",
        description="Single-geometry gta_default drawable whose structure the compiler reuses; in Template texture mode its texture size is the output size")
    texture_mode: EnumProperty(
        name="Texture mode", default='template',
        items=[('template', "Template", "The template's texture dictionary with its pixels replaced: template size, DXT1, opaque (proven in game)"),
               ('native', "Native", "Dictionary written from scratch: source size (power of two, 4-2048), full mip chain, "
                                    "DXT5 when the material has alpha, else DXT1 (needs playtest)")])
    texture_dictionary: StringProperty(
        name="Texture dictionary", maxlen=23, description="WTD name. Empty: the asset name")
    draw_distance: FloatProperty(
        name="Draw distance (m)", default=120.0, min=1.0, max=1500.0,
        description="IDE draw distance in metres (not affected by the scene unit scale)")
    audio_material: StringProperty(
        name="Audio material", description="Optional IDE amat entry (collision sound)")


def register():
    bpy.utils.register_class(LibertyAssetSettings)
    bpy.types.Scene.liberty_asset = PointerProperty(type=LibertyAssetSettings)


def unregister():
    del bpy.types.Scene.liberty_asset
    bpy.utils.unregister_class(LibertyAssetSettings)
