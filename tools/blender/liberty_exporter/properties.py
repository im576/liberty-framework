# Per-scene asset settings: everything asset.json needs. Stored as registered properties (saved in the .blend).

import bpy
from bpy.props import BoolProperty, EnumProperty, FloatProperty, FloatVectorProperty, PointerProperty, StringProperty


class LibertyAssetSettings(bpy.types.PropertyGroup):
    asset_name: StringProperty(
        name="Name", maxlen=23,
        description="Model name in game (1-23 letters, digits or _); also the asset folder name")
    asset_type: EnumProperty(
        name="Type", default='prop',
        items=[('prop', "Prop", "Static prop spawned by scripts (compiler v1)"),
               ('object', "World object", "Static world object placed in the map, with collision. Authored and checked now; "
                                          "LibertyContent builds it once the collision and placement writers exist")])
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
    drawable_writer: EnumProperty(
        name="Drawable writer", default='auto',
        items=[('auto', "Automatic", "The structure writer when a LOD has several materials (or several LODs with Native textures), "
                                     "else Template. Not written to asset.json"),
               ('template', "Template", "Compiler v1: LOD 0, one material, the template's single geometry patched (proven in game)"),
               ('structure', "Structure", "Every LOD and one geometry per material, written into a game drawable of the same or larger "
                                          "structure, with native textures (needs playtest)")])
    structure_template_archive: StringProperty(
        name="Structure template archive", default="*",
        description="Archive holding the structure template; * searches every IMG of the game")
    structure_template_model: StringProperty(
        name="Structure template model", default="auto",
        description="Drawable whose structure (LOD slots, geometries, shaders) the structure writer fills; auto takes the first that fits")
    texture_dictionary: StringProperty(
        name="Texture dictionary", maxlen=23, description="WTD name. Empty: the asset name")
    draw_distance: FloatProperty(
        name="Draw distance (m)", default=120.0, min=1.0, max=1500.0,
        description="IDE draw distance in metres (not affected by the scene unit scale)")
    audio_material: StringProperty(
        name="Audio material", description="Optional IDE amat entry (collision sound)")
    collision_source: EnumProperty(
        name="Collision", default='authored',
        items=[('authored', "Authored", "Collision objects in the scene (the collision writer is not available yet: LCC032)"),
               ('borrow', "Borrow", "Ship a vanilla prop's own collision under this model's name (NEEDS-PLAYTEST, T-032)")])
    collision_borrow_archive: StringProperty(
        name="Borrow from archive", default="*", description="Archive holding the prop whose collision is borrowed; * searches every IMG")
    collision_borrow_model: StringProperty(
        name="Borrow from model", default="auto",
        description="Model whose .wbn collision is shipped; auto takes the first vanilla prop with its own collision (as PROBE-collision lists them)")
    use_lod_distances: BoolProperty(
        name="LOD distances", default=False,
        description="Write lodDistancesMeters: how far each LOD is drawn. Checked now; written into the drawable by the LOD writer")
    lod_distances: FloatVectorProperty(
        name="LOD distances (m)", size=4, default=(30.0, 60.0, 120.0, 240.0), min=0.0, max=1500.0,
        description="Distance up to which LOD 0, 1, 2 and 3 are drawn, in metres (ascending; the last at most the draw distance). "
                    "Only the entries for the LODs the asset has are written")


def register():
    bpy.utils.register_class(LibertyAssetSettings)
    bpy.types.Scene.liberty_asset = PointerProperty(type=LibertyAssetSettings)


def unregister():
    del bpy.types.Scene.liberty_asset
    bpy.utils.unregister_class(LibertyAssetSettings)
