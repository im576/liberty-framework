# Authoring fixtures for the Liberty Content Compiler, built in Blender from code (NEXT_SESSIONS session 3):
# multi-material, multi-geometry, LOD 0-3, collision and a static world object. Each builder makes the scene, sets the
# Liberty asset settings and returns what LibertyContent must make of the export (expect.json):
#   status  "valid" or "invalid" (a build reports "ok" for a valid asset)
#   errors  the exact set of LibertyContent error codes: what no writer can write yet is refused (LCC032, LCC033); several
#           materials per LOD go to the structure writer automatically (session 4b)
#   codes   codes that must appear
#   report  a subset of report.json, every number worked out by hand from the scene below
# Used by tests/run_tests.py (checked on every run) and examples/make_fixtures.py (writes tests/content/fixtures, which
# `LibertyContent fixtures` checks without Blender). Also the scene helpers the tests share.

import math

import bmesh
import bpy
import numpy as np

DEFAULT_TEMPLATE_ARCHIVE = "pc/models/cdimages/weapons.img"
DEFAULT_TEMPLATE_MODEL = "amb_nailgun"
# Fixture textures are small: they only have to exist and decode.
FIXTURE_TEXTURE_PIXELS = 64


# ---- scene helpers ----

def clear_scene():
    for collection in (bpy.data.objects, bpy.data.meshes, bpy.data.materials, bpy.data.images, bpy.data.cameras, bpy.data.lights):
        for block in list(collection):
            collection.remove(block)
    for collection in list(bpy.data.collections):
        bpy.data.collections.remove(collection)
    scene = bpy.context.scene
    scene.unit_settings.scale_length = 1.0
    settings = scene.liberty_asset
    settings.asset_name = ""
    settings.asset_type = 'prop'
    settings.collection = None
    settings.texture_dictionary = ""
    settings.texture_mode = 'template'
    settings.template_archive = DEFAULT_TEMPLATE_ARCHIVE
    settings.template_model = DEFAULT_TEMPLATE_MODEL
    settings.draw_distance = 120.0
    settings.audio_material = ""
    settings.use_lod_distances = False
    settings.drawable_writer = 'auto'
    settings.collision_source = 'authored'
    settings.collision_borrow_archive = "*"
    settings.collision_borrow_model = "auto"
    settings.structure_template_archive = "*"
    settings.structure_template_model = "auto"


def texture(name, size=256, packed=True):
    image = bpy.data.images.new(name, size, size, alpha=False)
    v, u = np.mgrid[0:size, 0:size] / float(size)
    pixels = np.ones((size, size, 4), dtype=np.float32)
    pixels[..., 0] = 0.25 + 0.5 * u
    pixels[..., 1] = 0.25 + 0.5 * v
    pixels[..., 2] = 0.3
    image.pixels.foreach_set(pixels.ravel())
    if packed:
        image.pack()
    return image


def material(name, image=None, colour=None):
    mat = bpy.data.materials.new(name)
    if mat.node_tree is None:
        mat.use_nodes = True
    nodes = mat.node_tree.nodes
    bsdf = next((n for n in nodes if n.type == 'BSDF_PRINCIPLED'), None)
    if bsdf is None:
        bsdf = nodes.new("ShaderNodeBsdfPrincipled")
        output = next((n for n in nodes if n.type == 'OUTPUT_MATERIAL'), None) or nodes.new("ShaderNodeOutputMaterial")
        mat.node_tree.links.new(bsdf.outputs[0], output.inputs["Surface"])
    if colour is not None:
        bsdf.inputs["Base Color"].default_value = (colour[0], colour[1], colour[2], 1.0)
    if image is not None:
        node = nodes.new("ShaderNodeTexImage")
        node.image = image
        mat.node_tree.links.new(node.outputs["Color"], bsdf.inputs["Base Color"])
    return mat


def link(name, mesh, mat, collection=None):
    obj = bpy.data.objects.new(name, mesh)
    (collection or bpy.context.scene.collection).objects.link(obj)
    if mat is not None:
        mesh.materials.append(mat)
    return obj


def box(name, low, high, mat=None, collection=None):
    """Axis-aligned box from low to high (metres, Blender world space), with UVs; the object sits at the origin."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bm.loops.layers.uv.new("UVMap")
    bmesh.ops.create_cube(bm, size=1.0, calc_uvs=True)
    low, high = np.array(low, dtype=float), np.array(high, dtype=float)
    for vertex in bm.verts:
        vertex.co = [low[i] + (vertex.co[i] + 0.5) * (high[i] - low[i]) for i in range(3)]
    bm.to_mesh(mesh)
    bm.free()
    return link(name, mesh, mat, collection)


def placed(obj, location=(0, 0, 0), rotation_degrees=(0, 0, 0), scale=(1, 1, 1)):
    obj.location = location
    obj.rotation_euler = [math.radians(a) for a in rotation_degrees]
    obj.scale = scale
    return obj


def unit_box(name, mat=None, collection=None):
    """A 1 m cube centred on its origin: place it with placed()."""
    return box(name, (-0.5, -0.5, -0.5), (0.5, 0.5, 0.5), mat, collection)


def cylinder(name, segments, radius, height, mat=None, centred=False, collection=None):
    """Cylinder along local Z, standing on Z=0 (or centred on the origin)."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bm.loops.layers.uv.new("UVMap")
    bmesh.ops.create_cone(bm, cap_ends=True, segments=segments, radius1=radius, radius2=radius, depth=height, calc_uvs=True)
    if not centred:
        bmesh.ops.translate(bm, verts=bm.verts, vec=(0.0, 0.0, height / 2))
    bm.to_mesh(mesh)
    bm.free()
    return link(name, mesh, mat, collection)


def uv_sphere(name, radius, collection=None):
    """16 x 8 segments: vertices at 0/90/180/270 degrees on the equator and at both poles, so its bounds are exactly
    +-radius on every axis."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bmesh.ops.create_uvsphere(bm, u_segments=16, v_segments=8, radius=radius)
    bm.to_mesh(mesh)
    bm.free()
    return link(name, mesh, None, collection)


def select(*objects):
    bpy.context.view_layer.update()
    for obj in bpy.context.view_layer.objects:
        if obj is not None:
            obj.select_set(False)
    for obj in objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = objects[0] if objects else None


def tag_collision(obj, shape, surface=None):
    obj["liberty_collision"] = shape
    if surface:
        obj["liberty_surface"] = surface
    obj.display_type = 'WIRE'
    return obj


def new_collection(name):
    collection = bpy.data.collections.new(name)
    bpy.context.scene.collection.children.link(collection)
    return collection


# ---- expectations ----

def cylinder_triangles(segments):
    """Sides 2n, plus two n-gon caps of n-2 triangles each."""
    return 2 * segments + 2 * (segments - 2)


def ordered(expect, gltf):
    """Puts expected geometries in material-index order and collision in node-walk order, the orders report.json uses
    (the glTF exporter picks both; the expectations name materials and nodes, not positions)."""
    materials = [m.get("name") for m in gltf.get("materials", [])]
    walk = []

    def visit(index):
        node = gltf["nodes"][index]
        walk.append(node.get("name"))
        for child in node.get("children", []):
            visit(child)

    scene = gltf["scenes"][gltf.get("scene", 0)]
    for root in scene.get("nodes", []):
        visit(root)
    structure = expect.get("report", {}).get("structure", {})
    for lod in structure.get("lods", []):
        if "geometries" in lod:
            lod["geometries"].sort(key=lambda g: materials.index(g["material"]) if g["material"] in materials else -1)
    if "collision" in structure:
        structure["collision"].sort(key=lambda c: walk.index(c["name"]))
    return expect


def match(expected, actual, path, problems, tolerance=1e-3):
    """FixtureCheck.Match in Python: every value in expected must be in actual; numbers within tolerance."""
    if isinstance(expected, dict):
        if not isinstance(actual, dict):
            problems.append("%s: expected an object, got %r" % (path, actual))
            return
        for key, value in expected.items():
            if key not in actual:
                problems.append("%s.%s: missing" % (path, key))
                continue
            match(value, actual[key], "%s.%s" % (path, key), problems, tolerance)
    elif isinstance(expected, list):
        if not isinstance(actual, list) or len(actual) != len(expected):
            problems.append("%s: expected %d items, got %r" % (path, len(expected), actual))
            return
        for index, (a, b) in enumerate(zip(expected, actual)):
            match(a, b, "%s[%d]" % (path, index), problems, tolerance)
    elif isinstance(expected, (int, float)) and not isinstance(expected, bool):
        if not isinstance(actual, (int, float)) or isinstance(actual, bool) or abs(expected - actual) > tolerance:
            problems.append("%s: expected %r, got %r" % (path, expected, actual))
    elif expected != actual:
        problems.append("%s: expected %r, got %r" % (path, expected, actual))


# ---- fixtures ----

def lf_fx_multimat(settings):
    """One crate object with two material slots: a textured body and an untextured red lid (the +Z face). Two
    geometries in LOD 0, so the automatic writer chooses the structure writer (native textures)."""
    body = material("fx_crate", texture("fx_crate_basecolor", FIXTURE_TEXTURE_PIXELS))
    lid = material("fx_lid", colour=(0.8, 0.1, 0.1))
    obj = box("lf_fx_multimat", (-0.3, -0.3, 0.0), (0.3, 0.3, 0.6), body)
    obj.data.materials.append(lid)
    for polygon in obj.data.polygons:
        polygon.material_index = 1 if polygon.normal.z > 0.5 else 0
    select(obj)
    settings.asset_name = "lf_fx_multimat"
    return {
        "status": "valid", "errors": [], "codes": ["LCC026"],
        "report": {"type": "prop", "writer": {"drawable": "structure", "textureMode": "native"}, "structure": {
            "lods": [{"level": 0, "triangles": 12, "geometries": [
                {"material": "fx_crate", "textured": True, "meshes": 1, "triangles": 10},
                {"material": "fx_lid", "textured": False, "meshes": 1, "triangles": 2}]}],
            "lodDistancesMeters": None, "collision": []}},
    }


def lf_fx_multigeo(settings):
    """A table from five objects: a textured top and four legs sharing one metal material. Meshes of one material merge
    into one geometry, so LOD 0 has two geometries (top: 1 mesh, legs: 4 meshes): the structure writer's job."""
    collection = new_collection("lf_fx_multigeo")
    wood = material("fx_top", texture("fx_top_basecolor", FIXTURE_TEXTURE_PIXELS))
    metal = material("fx_legs", colour=(0.2, 0.2, 0.22))
    box("lf_fx_multigeo_top", (-0.6, -0.4, 0.7), (0.6, 0.4, 0.75), wood, collection)
    for index, (x, y) in enumerate(((-0.55, -0.35), (0.55, -0.35), (-0.55, 0.35), (0.55, 0.35))):
        box("lf_fx_multigeo_leg%d" % index, (x - 0.03, y - 0.03, 0.0), (x + 0.03, y + 0.03, 0.7), metal, collection)
    select()
    settings.asset_name = "lf_fx_multigeo"
    settings.collection = collection
    return {
        "status": "valid", "errors": [], "codes": ["LCC026"],
        "report": {"type": "prop", "meshes": 5, "writer": {"drawable": "structure"}, "structure": {
            "lods": [{"level": 0, "triangles": 60, "geometries": [
                {"material": "fx_top", "textured": True, "meshes": 1, "triangles": 12},
                {"material": "fx_legs", "textured": False, "meshes": 4, "triangles": 48}]}],
            "collision": []}},
    }


LOD_SEGMENTS = (48, 24, 12, 6)
LOD_COLOURS = ((0.1, 0.7, 0.2), (0.9, 0.8, 0.1), (0.95, 0.45, 0.05), (0.85, 0.1, 0.1))
LOD_DISTANCES = (25.0, 50.0, 100.0, 200.0)


def lf_fx_lods(settings):
    """A post in four LODs with visibly different complexity (48, 24, 12, 6 sides) and colours (green, yellow, orange,
    red, so a screenshot shows which LOD is drawn), and LOD distances. One material per LOD: valid for v1, which
    compiles LOD 0 and reports the rest (LCC025) and does not write the distances (LCC037)."""
    collection = new_collection("lf_fx_lods")
    lods = []
    for lod, (segments, colour) in enumerate(zip(LOD_SEGMENTS, LOD_COLOURS)):
        name = "lf_fx_lods" if lod == 0 else "lf_fx_lods_lod%d" % lod
        cylinder(name, segments, 0.2, 1.2, material("fx_lod%d" % lod, colour=colour), collection=collection)
        triangles = cylinder_triangles(segments)
        lods.append({"level": lod, "triangles": triangles, "geometries": [{"material": "fx_lod%d" % lod, "meshes": 1, "triangles": triangles}]})
    select()
    settings.asset_name = "lf_fx_lods"
    settings.collection = collection
    settings.draw_distance = 250.0
    settings.use_lod_distances = True
    settings.lod_distances = LOD_DISTANCES
    return {
        "status": "valid", "errors": [], "codes": ["LCC025", "LCC026", "LCC037"],
        "report": {"type": "prop", "lods": 4, "writer": {"drawable": "template"}, "structure": {"lods": lods, "lodDistancesMeters": list(LOD_DISTANCES), "collision": []}},
    }


COS30, SIN30 = math.cos(math.radians(30)), 0.5


def lf_fx_collision(settings):
    """A crate with every collision shape: a hull mesh (by the _col name), a box turned 30 degrees, a sphere and a capsule
    lying along -Y, with surfaces. v1 writes no collision, so LibertyContent refuses it (LCC032)."""
    collection = new_collection("lf_fx_collision")
    box("lf_fx_collision", (-0.4, -0.3, 0.0), (0.4, 0.3, 0.5), material("fx_crate", texture("fx_crate_basecolor", FIXTURE_TEXTURE_PIXELS)), collection)
    hull = box("lf_fx_collision_col", (-0.42, -0.32, 0.0), (0.42, 0.32, 0.52), None, collection)
    hull["liberty_surface"] = "wood"
    tag_collision(placed(unit_box("fx_box", None, collection), (0.0, 0.0, 0.25), (0, 0, 30), (0.8, 0.5, 0.5)), "box", "wood")
    tag_collision(placed(uv_sphere("fx_sphere", 0.25, collection), (0.0, 0.0, 0.25)), "sphere", "metal")
    capsule = cylinder("fx_capsule", 16, 0.1, 0.8, None, centred=True, collection=collection)
    tag_collision(placed(capsule, (0.0, 0.0, 0.25), (90, 0, 0)), "capsule")
    select()
    settings.asset_name = "lf_fx_collision"
    settings.collection = collection
    return {
        "status": "invalid", "errors": ["LCC032"], "codes": ["LCC026", "LCC032"],
        "report": {"type": "prop", "structure": {
            "lods": [{"level": 0, "triangles": 12}],
            "collision": [
                {"name": "lf_fx_collision_col", "shape": "mesh", "surface": "wood", "triangles": 12},
                # Half extents: the unit cube's 0.5 times the scale (0.8, 0.5, 0.5); X turned 30 degrees about Z.
                {"name": "fx_box", "shape": "box", "surface": "wood", "centre": [0, 0, 0.25], "halfExtents": [0.4, 0.25, 0.25],
                 "axes": [[COS30, SIN30, 0], [-SIN30, COS30, 0], [0, 0, 1]]},
                {"name": "fx_sphere", "shape": "sphere", "surface": "metal", "centre": [0, 0, 0.25], "radius": 0.25},
                # Radius 0.1, 0.8 m long: 0.6 m between the hemisphere centres. Turned 90 degrees about X: local Z -> -Y.
                {"name": "fx_capsule", "shape": "capsule", "surface": None, "centre": [0, 0, 0.25], "radius": 0.1, "length": 0.6,
                 "axes": [[1, 0, 0], [0, 0, 1], [0, -1, 0]]}]}},
    }


def lf_fx_world(settings):
    """A static world object: a low stone wall of four blocks and a coping (LOD 0: five meshes, one material, one
    geometry), a single block for LOD 1, a box collision fitted to it (named _col, the box tag wins) and LOD distances.
    World objects build (session 6); authored collision shapes are not written yet (LCC032)."""
    collection = new_collection("lf_fx_world")
    stone = material("fx_stone", texture("fx_stone_basecolor", FIXTURE_TEXTURE_PIXELS))
    for index in range(4):
        box("lf_fx_world_block%d" % index, (-1.0 + 0.5 * index, -0.15, 0.0), (-0.5 + 0.5 * index, 0.15, 1.0), stone, collection)
    box("lf_fx_world_coping", (-1.05, -0.2, 1.0), (1.05, 0.2, 1.1), stone, collection)
    box("lf_fx_world_lod1", (-1.05, -0.2, 0.0), (1.05, 0.2, 1.1), stone, collection)
    tag_collision(placed(unit_box("lf_fx_world_col", None, collection), (0.0, 0.0, 0.55), (0, 0, 0), (2.1, 0.4, 1.1)), "box", "stone")
    select()
    settings.asset_name = "lf_fx_world"
    settings.asset_type = 'object'
    settings.collection = collection
    settings.draw_distance = 300.0
    settings.use_lod_distances = True
    settings.lod_distances = (60.0, 150.0, 0.0, 0.0)
    return {
        "status": "invalid", "errors": ["LCC032"], "codes": ["LCC025", "LCC026", "LCC037"],
        "report": {"type": "object", "lods": 2, "structure": {
            "lods": [{"level": 0, "triangles": 60, "geometries": [{"material": "fx_stone", "meshes": 5, "triangles": 60}]},
                     {"level": 1, "triangles": 12, "geometries": [{"material": "fx_stone", "meshes": 1, "triangles": 12}]}],
            "lodDistancesMeters": [60, 150],
            "collision": [{"name": "lf_fx_world_col", "shape": "box", "surface": "stone", "centre": [0, 0, 0.55],
                           "halfExtents": [1.05, 0.2, 0.55], "axes": [[1, 0, 0], [0, 1, 0], [0, 0, 1]]}]}},
    }


FIXTURES = (lf_fx_multimat, lf_fx_multigeo, lf_fx_lods, lf_fx_collision, lf_fx_world)
KINDS = {"prop": "props", "object": "objects"}
