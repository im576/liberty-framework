# Builds content/objects/lf_world_wall from code and exports it with the Liberty Exporter add-on: the test asset for static
# world objects (T-033). A low stone wall (2.1 x 0.4 x 1.1 m) of four blocks and a coping as LOD 0, one block as LOD 1,
# one textured material (so the automatic writer keeps the v1 build proven in game), type "object", and collision
# borrowed from a vanilla prop (T-032) until authored collision can be written. The world mod places it in the map from
# config/world/objects.json ("test_wall", 8 m north of the east_park teleport); scenario world-objects. Run by
# tools/blender/make-examples.ps1:
#   blender -b --factory-startup --python-exit-code 1 --python make_lf_world_wall.py -- <repo>

import os
import sys

import bpy  # first: as the bpy Python module, importing bpy is what puts addon_utils on the path
import addon_utils

REPO = sys.argv[sys.argv.index("--") + 1]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))
sys.path.insert(0, os.path.join(REPO, "tools", "blender", "tests"))
from fixtures import box, clear_scene, material, new_collection, select, texture  # noqa: E402

NAME = "lf_world_wall"
# Small on purpose: the texture only has to read as stone-coloured in a screenshot.
TEXTURE_PIXELS = 128


def main():
    addon_utils.enable("liberty_exporter", default_set=True, handle_error=None)
    clear_scene()
    collection = new_collection(NAME)
    stone = material(NAME, texture(NAME + "_basecolor", TEXTURE_PIXELS))
    for index in range(4):
        box("%s_block%d" % (NAME, index), (-1.0 + 0.5 * index, -0.15, 0.0), (-0.52 + 0.5 * index, 0.15, 1.0), stone, collection)
    box(NAME + "_coping", (-1.05, -0.2, 1.0), (1.05, 0.2, 1.1), stone, collection)
    box(NAME + "_lod1", (-1.05, -0.2, 0.0), (1.05, 0.2, 1.1), stone, collection)
    select()
    settings = bpy.context.scene.liberty_asset
    settings.asset_name = NAME
    settings.asset_type = 'object'
    settings.collection = collection
    settings.draw_distance = 300.0
    settings.collision_source = 'borrow'
    prefs = bpy.context.preferences.addons["liberty_exporter"].preferences
    prefs.content_root = os.path.join(REPO, "content")
    prefs.compiler_path = os.path.join(REPO, "tools", "content", "bin", "LibertyContent.exe")
    try:
        result = bpy.ops.liberty.export()
    except RuntimeError as error:
        print("EXAMPLE %s refused: %s" % (NAME, error))
        sys.exit(1)
    print("EXAMPLE %s %s" % (NAME, result))
    if result != {'FINISHED'}:
        sys.exit(1)


main()
