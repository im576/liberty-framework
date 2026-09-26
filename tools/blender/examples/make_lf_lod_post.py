# Builds content/props/lf_lod_post from code and exports it with the Liberty Exporter add-on: the in-game test asset for
# the structure writer's LODs (T-031, scenario lod-review). A 3 m post in four LODs of visibly different complexity
# (48, 24, 12 and 6 sides). One material and one texture of four quadrants: each LOD's UVs sample its own quadrant, green,
# yellow, orange and red with 1, 2, 3 and 4 dark bands, so a screenshot shows which LOD the game draws. One material
# keeps the template's needs to four LOD slots with one gta_default geometry each. Run by tools/blender/make-examples.ps1:
#   blender -b --factory-startup --python-exit-code 1 --python make_lf_lod_post.py -- <repo>

import math
import os
import sys

import bpy  # first: as the bpy Python module, importing bpy is what puts addon_utils on the path
import addon_utils
import bmesh
import numpy as np

REPO = sys.argv[sys.argv.index("--") + 1]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))
NAME = "lf_lod_post"
HEIGHT, RADIUS = 3.0, 0.4
SIDES = (48, 24, 12, 6)
# Quadrant colours (LOD 0-3) and their (column, row) in the texture, row 0 at the top.
COLOURS = ((0.15, 0.75, 0.2), (0.95, 0.85, 0.1), (0.98, 0.5, 0.05), (0.85, 0.1, 0.1))
QUADRANTS = ((0, 0), (1, 0), (0, 1), (1, 1))
TEXTURE_SIZE = 256
# Keep each LOD's UVs this far inside its quadrant so mip levels do not bleed the neighbour's colour in.
INSET = 0.04
# How far each LOD is drawn (metres) and the draw distance; lod-review photographs at 6, 18, 38 and 75 m.
LOD_DISTANCES = (12.0, 25.0, 50.0, 100.0)
DRAW_DISTANCE = 150.0


def paint_texture():
    """Four flat quadrants, LOD n's with n + 1 dark horizontal bands (countable without telling colours apart)."""
    half = TEXTURE_SIZE // 2
    pixels = np.ones((TEXTURE_SIZE, TEXTURE_SIZE, 4), dtype=np.float32)
    for lod, ((column, row), colour) in enumerate(zip(QUADRANTS, COLOURS)):
        top = row * half
        block = pixels[top:top + half, column * half:(column + 1) * half]
        block[..., :3] = colour
        bands = lod + 1
        for band in range(bands):
            centre = int((band + 1) * half / (bands + 1))
            block[centre - 4:centre + 4, :, :3] = np.array(colour) * 0.25
    image = bpy.data.images.new(NAME + "_basecolor", TEXTURE_SIZE, TEXTURE_SIZE, alpha=False)
    # Blender stores rows bottom-up; the painting above is top-down (glTF and Direct3D put v = 0 at the top).
    image.pixels.foreach_set(pixels[::-1].ravel())
    image.pack()
    return image


def quadrant_uv(lod, u, v):
    """(u, v) in 0..1 mapped into LOD lod's quadrant, v = 0 at the top of the texture as glTF stores it."""
    column, row = QUADRANTS[lod]
    span = 0.5 - 2 * INSET
    # Blender's UV origin is bottom-left; the glTF exporter flips v, so a quadrant's top row is Blender v = 1 - row / 2.
    return (column * 0.5 + INSET + u * span, 1.0 - (row * 0.5 + INSET + (1.0 - v) * span))


def post_mesh(name, lod):
    sides = SIDES[lod]
    bm = bmesh.new()
    uv = bm.loops.layers.uv.new("UVMap")
    rings = [[bm.verts.new((RADIUS * math.cos(2 * math.pi * i / sides), RADIUS * math.sin(2 * math.pi * i / sides), z)) for i in range(sides)] for z in (0.0, HEIGHT)]
    for i in range(sides):
        j = (i + 1) % sides
        face = bm.faces.new((rings[0][i], rings[0][j], rings[1][j], rings[1][i]))
        face.smooth = True
        for loop, (u, v) in zip(face.loops, ((i / sides, 0.0), ((i + 1) / sides, 0.0), ((i + 1) / sides, 1.0), (i / sides, 1.0))):
            loop[uv].uv = quadrant_uv(lod, u, v)
    for ring, reverse in ((rings[1], False), (rings[0], True)):
        face = bm.faces.new(list(reversed(ring)) if reverse else ring)
        for loop in face.loops:
            loop[uv].uv = quadrant_uv(lod, 0.5, 0.5)
    mesh = bpy.data.meshes.new(name)
    bm.normal_update()
    bm.to_mesh(mesh)
    bm.free()
    return mesh


def main():
    for collection in (bpy.data.objects, bpy.data.meshes, bpy.data.materials, bpy.data.images):
        for block in list(collection):
            collection.remove(block)
    addon_utils.enable("liberty_exporter", default_set=True, handle_error=None)
    material = bpy.data.materials.new(NAME)
    if material.node_tree is None:
        material.use_nodes = True
    bsdf = next(n for n in material.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')
    texture = material.node_tree.nodes.new("ShaderNodeTexImage")
    texture.image = paint_texture()
    material.node_tree.links.new(texture.outputs["Color"], bsdf.inputs["Base Color"])

    scene = bpy.context.scene
    collection = bpy.data.collections.new(NAME)
    scene.collection.children.link(collection)
    for lod in range(len(SIDES)):
        mesh = post_mesh(NAME if lod == 0 else "%s_lod%d" % (NAME, lod), lod)
        mesh.materials.append(material)
        collection.objects.link(bpy.data.objects.new(mesh.name, mesh))

    settings = scene.liberty_asset
    settings.asset_name = NAME
    settings.collection = collection
    settings.template_archive = "pc/models/cdimages/weapons.img"
    settings.template_model = "amb_nailgun"
    settings.texture_mode = 'native'
    settings.drawable_writer = 'structure'
    settings.structure_template_archive = "*"
    settings.structure_template_model = "auto"
    settings.draw_distance = DRAW_DISTANCE
    settings.use_lod_distances = True
    settings.lod_distances = LOD_DISTANCES
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
