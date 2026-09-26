# Builds content/props/lf_blender_barrel from code, the way an artist would in Blender, then exports it with the
# Liberty Exporter add-on: a 55-gallon steel drum (0.88 m tall, 0.58 m wide) with rolling hoops, an original
# procedurally painted texture, LOD 0 and LOD 1. The .blend and texture are saved under source/ so the asset can be
# edited by hand later. Run by tools/blender/make-examples.ps1:
#   blender -b --factory-startup --python-exit-code 1 --python make_lf_blender_barrel.py -- <repo>

import math
import os
import sys

import addon_utils
import bmesh
import bpy
import numpy as np

REPO = sys.argv[sys.argv.index("--") + 1]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))
NAME = "lf_blender_barrel"
FOLDER = os.path.join(REPO, "content", "props", NAME)
SOURCE = os.path.join(FOLDER, "source")
HEIGHT = 0.88
RADIUS = 0.29
TEXTURE_SIZE = 256

# Side profile (height m, radius m): chimes at both ends, two rolling hoops at 1/3 and 2/3.
PROFILE_LOD0 = [(0.0, 0.283), (0.012, 0.293), (0.03, RADIUS), (0.278, RADIUS), (0.293, 0.301), (0.308, RADIUS),
                (0.572, RADIUS), (0.587, 0.301), (0.602, RADIUS), (0.85, RADIUS), (0.868, 0.293), (HEIGHT, 0.283)]
PROFILE_LOD1 = [(0.0, 0.286), (HEIGHT, 0.286)]


def drum_mesh(name, profile, segments):
    """Side rows share one UV strip (bottom half of the texture, u around, v up); lids are discs in the top half."""
    bm = bmesh.new()
    uv = bm.loops.layers.uv.new("UVMap")
    rings = []
    for z, r in profile:
        rings.append([bm.verts.new((r * math.cos(2 * math.pi * j / segments), r * math.sin(2 * math.pi * j / segments), z)) for j in range(segments)])
    for row in range(len(rings) - 1):
        v0, v1 = profile[row][0] / HEIGHT * 0.5, profile[row + 1][0] / HEIGHT * 0.5
        for j in range(segments):
            k = (j + 1) % segments
            face = bm.faces.new((rings[row][j], rings[row][k], rings[row + 1][k], rings[row + 1][j]))
            face.smooth = True
            u0, u1 = j / segments, (j + 1) / segments
            for loop, coordinate in zip(face.loops, ((u0, v0), (u1, v0), (u1, v1), (u0, v1))):
                loop[uv].uv = coordinate
    for ring, centre_uv, top in ((rings[-1], (0.25, 0.75), True), (rings[0], (0.75, 0.75), False)):
        z = profile[-1][0] if top else profile[0][0]
        centre = bm.verts.new((0.0, 0.0, z))
        for j in range(segments):
            k = (j + 1) % segments
            corners = (centre, ring[j], ring[k]) if top else (centre, ring[k], ring[j])
            face = bm.faces.new(corners)
            face.smooth = False
            for loop in face.loops:
                x, y = loop.vert.co.x / RADIUS, loop.vert.co.y / RADIUS
                loop[uv].uv = (centre_uv[0] + 0.23 * x, centre_uv[1] + 0.23 * y)
    mesh = bpy.data.meshes.new(name)
    bm.normal_update()
    bm.to_mesh(mesh)
    bm.free()
    return mesh


def paint_texture(path):
    """Original texture: red enamel side with dark hoops, wear and a stencil band (bottom half); two steel lids (top)."""
    size = TEXTURE_SIZE
    rng = np.random.default_rng(1604)
    v, u = np.mgrid[0:size, 0:size].astype(np.float64) / size  # Blender pixel rows start at the bottom (v = 0)
    pixels = np.ones((size, size, 4))
    side = v < 0.5
    height = v / 0.5
    paint = np.stack([np.full_like(u, 0.52), np.full_like(u, 0.09), np.full_like(u, 0.06)], axis=-1)
    grain = rng.normal(0.0, 0.025, (size, size))
    streaks = 0.06 * np.sin(u * 2 * math.pi * 23 + np.sin(u * 2 * math.pi * 5) * 2) * np.clip(1 - height, 0, 1)
    colour = paint * (1 + grain + streaks)[..., None]
    for hoop in (0.293 / HEIGHT, 0.587 / HEIGHT):
        colour[np.abs(height - hoop) < 0.02] *= 0.55
    colour[(height < 0.035) | (height > 0.965)] *= 0.5
    band = (np.abs(height - 0.45) < 0.05) & (np.abs(((u * 2) % 1) - 0.5) < 0.3)
    colour[band] = np.array([0.85, 0.72, 0.18]) * (1 + grain[band] * 2)[..., None]
    wear = rng.random((size, size)) < 0.004 + 0.03 * np.clip((0.1 - height) * 10, 0, 1)
    colour[wear] = np.array([0.33, 0.2, 0.12])
    pixels[side, :3] = np.clip(colour[side], 0, 1)
    for centre in ((0.25, 0.75), (0.75, 0.75)):
        dx, dy = (u - centre[0]) / 0.23, (v - centre[1]) / 0.23
        r = np.hypot(dx, dy)
        lid = (~side) & (r <= 1.05)
        steel = 0.42 + 0.05 * np.sin(r * 60) + rng.normal(0.0, 0.02, (size, size))
        steel = np.where(np.abs(r - 0.82) < 0.04, steel * 0.6, steel)
        bung = np.hypot(dx - 0.45, dy - 0.2) < 0.12
        steel = np.where(bung, 0.2, steel)
        pixels[lid, 0] = np.clip(steel[lid] * 1.02, 0, 1)
        pixels[lid, 1] = np.clip(steel[lid] * 1.0, 0, 1)
        pixels[lid, 2] = np.clip(steel[lid] * 0.96, 0, 1)
    pixels[(~side) & (np.minimum(np.hypot((u - 0.25) / 0.23, (v - 0.75) / 0.23), np.hypot((u - 0.75) / 0.23, (v - 0.75) / 0.23)) > 1.05), :3] = 0.3
    image = bpy.data.images.new(NAME + "_basecolor", size, size, alpha=False)
    image.pixels.foreach_set(pixels.astype(np.float32).ravel())
    image.filepath_raw = path
    image.file_format = 'PNG'
    image.save()
    return image


def main():
    os.makedirs(SOURCE, exist_ok=True)
    for collection in (bpy.data.objects, bpy.data.meshes, bpy.data.materials, bpy.data.images):
        for block in list(collection):
            collection.remove(block)
    addon_utils.enable("liberty_exporter", default_set=True)

    image = paint_texture(os.path.join(SOURCE, NAME + "_basecolor.png"))
    material = bpy.data.materials.new(NAME)
    if material.node_tree is None:
        material.use_nodes = True
    bsdf = next(n for n in material.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')
    bsdf.inputs["Metallic"].default_value = 0.3
    bsdf.inputs["Roughness"].default_value = 0.55
    texture = material.node_tree.nodes.new("ShaderNodeTexImage")
    texture.image = image
    texture.location = (-400, 200)
    material.node_tree.links.new(texture.outputs["Color"], bsdf.inputs["Base Color"])

    scene = bpy.context.scene
    collection = bpy.data.collections.new(NAME)
    scene.collection.children.link(collection)
    for object_name, profile, segments in ((NAME, PROFILE_LOD0, 24), (NAME + "_lod1", PROFILE_LOD1, 10)):
        mesh = drum_mesh(object_name, profile, segments)
        mesh.materials.append(material)
        obj = bpy.data.objects.new(object_name, mesh)
        collection.objects.link(obj)
        if object_name.endswith("_lod1"):
            obj.hide_set(True)  # the artist sees LOD 0; the exporter still writes LOD 1 (collection export)

    settings = scene.liberty_asset
    settings.asset_name = NAME
    settings.collection = collection
    settings.template_archive = "pc/models/cdimages/weapons.img"
    settings.template_model = "amb_nailgun"
    settings.draw_distance = 150.0
    settings.audio_material = ""

    prefs = bpy.context.preferences.addons["liberty_exporter"].preferences
    prefs.content_root = os.path.join(REPO, "content")
    prefs.compiler_path = os.path.join(REPO, "tools", "content", "bin", "LibertyContent.exe")

    bpy.context.preferences.filepaths.save_version = 0  # no .blend1 backups in the repository
    blend = os.path.join(SOURCE, NAME + ".blend")
    bpy.ops.wm.save_as_mainfile(filepath=blend, compress=True, relative_remap=True)
    bpy.ops.file.make_paths_relative()
    bpy.ops.wm.save_mainfile(compress=True)
    result = bpy.ops.liberty.export()
    print("EXAMPLE %s %s" % (NAME, result))
    if result != {'FINISHED'}:
        sys.exit(1)


main()
