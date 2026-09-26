# Headless tests for the Liberty Exporter add-on, run by tools/blender/run-tests.ps1:
#   blender -b --factory-startup --python-exit-code 1 --python run_tests.py -- <repo> <game> <output>
# Builds scenes from code, runs the add-on's operators, then checks what Blender exported and what LibertyContent
# (validate and build, against the real game archives) made of it. Prints PASS/FAIL lines and "RESULT passed=N failed=M".

import json
import os
import shutil
import sys

import addon_utils
import bmesh
import bpy
import numpy as np

arguments = sys.argv[sys.argv.index("--") + 1:]
REPO, GAME, OUTPUT = arguments[0], arguments[1], arguments[2]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))

CONTENT = os.path.join(OUTPUT, "content")
BUILD = os.path.join(OUTPUT, "build")
COMPILER = os.path.join(REPO, "tools", "content", "bin", "LibertyContent.exe")
results = {"passed": 0, "failed": 0}


def expect(condition, label, detail=""):
    key = "passed" if condition else "failed"
    results[key] += 1
    print("%s %s%s" % ("PASS" if condition else "FAIL", label, ("  (" + str(detail) + ")") if detail and not condition else ""))


def call(operator, **options):
    """Runs an operator; a CANCELLED operator that reported an error raises in Python, so both come back as a result set."""
    try:
        return operator(**options)
    except RuntimeError as error:
        print("operator refused: %s" % error)
        return {'CANCELLED'}


def codes(issues):
    return [issue.code for issue in issues]


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
    settings.collection = None
    settings.texture_dictionary = ""
    settings.template_archive = "pc/models/cdimages/weapons.img"
    settings.template_model = "amb_nailgun"
    settings.draw_distance = 120.0


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


def material(name, image=None):
    mat = bpy.data.materials.new(name)
    if mat.node_tree is None:
        mat.use_nodes = True
    nodes = mat.node_tree.nodes
    bsdf = next((n for n in nodes if n.type == 'BSDF_PRINCIPLED'), None)
    if bsdf is None:
        bsdf = nodes.new("ShaderNodeBsdfPrincipled")
        output = next((n for n in nodes if n.type == 'OUTPUT_MATERIAL'), None) or nodes.new("ShaderNodeOutputMaterial")
        mat.node_tree.links.new(bsdf.outputs[0], output.inputs["Surface"])
    if image is not None:
        node = nodes.new("ShaderNodeTexImage")
        node.image = image
        mat.node_tree.links.new(node.outputs["Color"], bsdf.inputs["Base Color"])
    return mat


def box(name, low, high, mat=None):
    """Axis-aligned box from low to high (metres, Blender world space), with UVs."""
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bm.loops.layers.uv.new("UVMap")
    bmesh.ops.create_cube(bm, size=1.0, calc_uvs=True)
    low, high = np.array(low, dtype=float), np.array(high, dtype=float)
    for vertex in bm.verts:
        vertex.co = [low[i] + (vertex.co[i] + 0.5) * (high[i] - low[i]) for i in range(3)]
    bm.to_mesh(mesh)
    bm.free()
    return link(name, mesh, mat)


def cylinder(name, segments, radius, height, mat=None):
    mesh = bpy.data.meshes.new(name)
    bm = bmesh.new()
    bm.loops.layers.uv.new("UVMap")
    bmesh.ops.create_cone(bm, cap_ends=True, segments=segments, radius1=radius, radius2=radius, depth=height, calc_uvs=True)
    bmesh.ops.translate(bm, verts=bm.verts, vec=(0.0, 0.0, height / 2))
    bm.to_mesh(mesh)
    bm.free()
    return link(name, mesh, mat)


def link(name, mesh, mat):
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.scene.collection.objects.link(obj)
    if mat is not None:
        mesh.materials.append(mat)
    return obj


def select(*objects):
    bpy.context.view_layer.update()
    for obj in bpy.context.view_layer.objects:
        if obj is not None:
            obj.select_set(False)
    for obj in objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = objects[0] if objects else None


def gltf_json(name):
    with open(os.path.join(CONTENT, "props", name, name + ".gltf"), "r", encoding="utf-8") as stream:
        return json.load(stream)


def report(name):
    with open(os.path.join(BUILD, name, "report.json"), "r", encoding="utf-8") as stream:
        return json.load(stream)


def bounds_issue(issues):
    """LCC014 'bounds (x, y, z) .. (x, y, z)' -> two tuples of floats."""
    for issue in issues:
        if issue.code == "LCC014":
            text = issue.message.replace("bounds", "").replace("(", "").replace(")", "")
            low, high = text.split("..")
            return [float(v) for v in low.split(",")], [float(v) for v in high.split(",")]
    return None


def main():
    if os.path.isdir(OUTPUT):
        shutil.rmtree(OUTPUT)
    os.makedirs(OUTPUT)
    addon_utils.enable("liberty_exporter", default_set=True, handle_error=None)
    from liberty_exporter import checks, state  # noqa: E402
    prefs = bpy.context.preferences.addons["liberty_exporter"].preferences
    prefs.content_root = CONTENT
    prefs.compiler_path = COMPILER
    prefs.game_directory = GAME
    prefs.build_directory = BUILD
    expect(hasattr(bpy.types.Scene, "liberty_asset") and hasattr(bpy.ops.liberty, "build"), "addon registers")
    scene = bpy.context.scene
    settings = scene.liberty_asset

    # 1. Textured barrel with an _lod1 (Blender duplicate-suffixed) and a tagged LOD 2: check, export, validate, build.
    clear_scene()
    mat = material("barrel", texture("barrel_basecolor"))
    lod0 = cylinder("lf_bt_barrel", 24, 0.3, 0.9, mat)
    lod1 = cylinder("lf_bt_barrel_lod1.001", 12, 0.3, 0.9, mat)
    lod2 = cylinder("lf_bt_barrel_far", 6, 0.3, 0.9, mat)
    select(lod2)
    bpy.ops.liberty.set_lod(lod=2)
    select(lod0, lod1, lod2)
    settings.asset_name = "lf_bt_barrel"
    expect(checks.lod_of(lod1) == 1 and checks.lod_of(lod2) == 2 and checks.lod_of(lod0) == 0, "lod tags (suffix with .001, property)")
    call(bpy.ops.liberty.check)
    issues = state.get(scene).issues
    expect(not checks.has_errors(issues), "barrel check has no errors", codes(issues))
    outcome = call(bpy.ops.liberty.build)
    result = state.get(scene)
    expect(outcome == {'FINISHED'} and result.status == "ok", "barrel build ok", "%s %s %s" % (outcome, result.status, result.log[-600:]))
    folder = os.path.join(CONTENT, "props", "lf_bt_barrel")
    manifest = json.load(open(os.path.join(folder, "asset.json"), encoding="utf-8"))
    expect(manifest["name"] == "lf_bt_barrel" and manifest["source"] == "lf_bt_barrel.gltf" and manifest["textureDictionary"] == "lf_bt_barrel" and
           manifest["template"]["model"] == "amb_nailgun", "asset.json written", manifest)
    document = gltf_json("lf_bt_barrel")
    node_extras = {n["name"]: n.get("extras", {}) for n in document["nodes"]}
    expect(node_extras.get("lf_bt_barrel_far", {}).get("liberty_lod") == 2, "liberty_lod exported as node extras", node_extras)
    scene_extras = document["scenes"][0].get("extras", {})
    expect(scene_extras.get("liberty_exporter") == "0.1.0" and "liberty_exporter" not in scene.keys(), "scene tags exported and removed again", scene_extras)
    expect(any(i.get("uri", "").endswith(".png") for i in document.get("images", [])), "texture written as PNG", document.get("images"))
    built = report("lf_bt_barrel")
    expect(built["lods"] == 3 and built["metadata"].get("liberty_exporter") == "0.1.0", "report: 3 LODs, exporter metadata", built)
    expect(os.path.isfile(result.preview) and os.path.isfile(result.texture), "previews written", result.preview)
    expect(not any(i["code"] == "LCC017" for i in built["issues"]), "LOD triangle order accepted")

    # 2. Orientation: an asymmetric box keeps Blender's axes and metres through glTF and the compiler.
    clear_scene()
    low, high = (-0.1, -0.2, 0.0), (0.3, 0.5, 0.6)
    obj = box("lf_bt_axes", low, high, material("axes"))
    select(obj)
    settings.asset_name = "lf_bt_axes"
    call(bpy.ops.liberty.export)
    result = state.get(scene)
    bounds = bounds_issue(result.issues)
    expect(result.status == "exported" and bounds is not None, "axes export validated", result.log[-400:])
    if bounds:
        error = max(abs(a - b) for a, b in zip(bounds[0] + bounds[1], list(low) + list(high)))
        expect(error < 0.002, "compiler bounds match Blender (Z-up metres)", bounds)

    # 3. Modifiers are applied: an Array modifier doubles the triangles the compiler builds.
    settings.asset_name = "lf_bt_array"
    modifier = obj.modifiers.new("array", 'ARRAY')
    modifier.count = 2
    call(bpy.ops.liberty.build)
    result = state.get(scene)
    built = report("lf_bt_array") if os.path.isfile(os.path.join(BUILD, "lf_bt_array", "report.json")) else {}
    expect(result.status == "ok" and built.get("compiled", {}).get("triangles") == 24, "modifiers applied (24 triangles)", built.get("compiled"))
    obj.modifiers.remove(modifier)

    # 4. Two materials in LOD 0: rejected in Blender, nothing written.
    clear_scene()
    a, b = box("lf_bt_two_a", (0, 0, 0), (0.5, 0.5, 0.5), material("red")), box("lf_bt_two_b", (0.5, 0, 0), (1, 0.5, 0.5), material("blue"))
    select(a, b)
    settings.asset_name = "lf_bt_two"
    outcome = call(bpy.ops.liberty.export)
    expect(outcome == {'CANCELLED'} and "LBX009" in codes(state.get(scene).issues), "two materials per LOD rejected (LBX009)")
    expect(not os.path.exists(os.path.join(CONTENT, "props", "lf_bt_two")), "rejected asset writes nothing")

    # 5. Name, unit scale, pivot, missing texture, UVs, shader tag, alpha, nothing selected.
    clear_scene()
    obj = box("lf_bt_misc", (10, 0, 0), (10.5, 0.5, 0.5), material("plain"))
    select(obj)
    settings.asset_name = "bad name!"
    scene.unit_settings.scale_length = 0.01
    call(bpy.ops.liberty.check)
    found = codes(state.get(scene).issues)
    expect("LBX002" in found and "LBX004" in found and "LBX007" in found, "name, unit scale and pivot flagged", found)

    clear_scene()
    missing = bpy.data.images.new("missing", 64, 64)
    missing.source = 'FILE'
    missing.filepath = os.path.join(OUTPUT, "does_not_exist.png")
    obj = box("lf_bt_missing", (0, 0, 0), (0.5, 0.5, 0.5), material("missing", missing))
    select(obj)
    settings.asset_name = "lf_bt_missing"
    call(bpy.ops.liberty.check)
    expect("LBX012" in codes(state.get(scene).issues), "missing image file flagged (LBX012)", codes(state.get(scene).issues))

    clear_scene()
    obj = box("lf_bt_nouv", (0, 0, 0), (0.5, 0.5, 0.5), material("uv", texture("uv_basecolor", 100)))
    obj.data.uv_layers.remove(obj.data.uv_layers[0])
    obj.data.materials[0]["liberty_shader"] = "gta_normal_spec"
    principled = checks.principled_of(obj.data.materials[0])
    principled.inputs["Alpha"].default_value = 0.5
    select(obj)
    settings.asset_name = "lf_bt_nouv"
    call(bpy.ops.liberty.check)
    found = codes(state.get(scene).issues)
    expect(all(c in found for c in ("LBX014", "LBX019", "LBX020", "LBX013")), "no UVs, shader, alpha, non-power-of-two flagged", found)

    clear_scene()
    select()
    settings.asset_name = "lf_bt_empty"
    call(bpy.ops.liberty.check)
    expect(codes(state.get(scene).issues) == ["LBX001"], "nothing selected flagged (LBX001)", codes(state.get(scene).issues))

    # 6. Collection mode exports the collection's objects (nested included) without a selection; LOD inherited from a parent.
    clear_scene()
    collection = bpy.data.collections.new("lf_bt_collection")
    scene.collection.children.link(collection)
    nested = bpy.data.collections.new("nested")
    collection.children.link(nested)
    mat = material("crate", texture("crate_basecolor"))
    near = box("lf_bt_coll", (-0.3, -0.3, 0), (0.3, 0.3, 0.6), mat)
    far_parent = bpy.data.objects.new("lf_bt_coll_lod1", None)
    far = cylinder("far_child", 4, 0.3, 0.6, mat)
    far.parent = far_parent
    for item in (near, far_parent):
        scene.collection.objects.unlink(item) if item.name in scene.collection.objects else None
        collection.objects.link(item)
    scene.collection.objects.unlink(far)
    nested.objects.link(far)
    select()
    settings.asset_name = "lf_bt_coll"
    settings.collection = collection
    expect(checks.lod_of(far) == 1, "LOD inherited from the parent")
    call(bpy.ops.liberty.build)
    result = state.get(scene)
    built = report("lf_bt_coll") if os.path.isfile(os.path.join(BUILD, "lf_bt_coll", "report.json")) else {}
    expect(result.status == "ok" and built.get("lods") == 2, "collection export builds with 2 LODs", "%s %s" % (result.status, built.get("lods")))
    expect(built.get("metadata", {}).get("liberty_exporter") == "0.1.0" and "liberty_exporter" not in collection.keys(), "collection export carries exporter metadata", built.get("metadata"))

    print("RESULT passed=%d failed=%d" % (results["passed"], results["failed"]))
    if results["failed"]:
        sys.exit(1)


main()
