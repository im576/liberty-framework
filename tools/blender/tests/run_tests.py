# Headless tests for the Liberty Exporter add-on, run by tools/blender/run-tests.ps1:
#   blender -b --factory-startup --python-exit-code 1 --python run_tests.py -- <repo> <game> <output>
# Builds scenes from code, runs the add-on's operators, then checks what Blender exported and what LibertyContent
# (validate and build, against the real game archives) made of it. Prints PASS/FAIL lines and "RESULT passed=N failed=M".
# <game> may be --no-game (the cloud container, bpy as a Python module): builds need the game's template archives, so
# the operators export and validate instead and every expectation about a build is printed as NOT-RUN, never PASS.

import json
import os
import shutil
import sys

import bpy  # first: as the bpy Python module (cloud container), importing bpy is what puts addon_utils on the path
import addon_utils

arguments = sys.argv[sys.argv.index("--") + 1:]
REPO, GAME, OUTPUT = arguments[0], arguments[1], arguments[2]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))
sys.path.insert(0, os.path.join(REPO, "tools", "blender", "tests"))
import fixtures  # noqa: E402
from fixtures import box, clear_scene, cylinder, material, placed, select, tag_collision, texture, unit_box, uv_sphere  # noqa: E402

CONTENT = os.path.join(OUTPUT, "content")
BUILD = os.path.join(OUTPUT, "build")
COMPILER = os.path.join(REPO, "tools", "content", "bin", "LibertyContent.exe")
NO_GAME = GAME == "--no-game"
results = {"passed": 0, "failed": 0, "notrun": 0}


def expect(condition, label, detail=""):
    key = "passed" if condition else "failed"
    results[key] += 1
    print("%s %s%s" % ("PASS" if condition else "FAIL", label, ("  (" + str(detail) + ")") if detail and not condition else ""))


def expect_built(condition, label, detail=lambda: ""):
    """An expectation about a compiled build: condition and detail are callables so nothing reads a build that a
    --no-game run never made."""
    if NO_GAME:
        results["notrun"] += 1
        print("NOT-RUN %s (needs the game's archives to build)" % label)
        return
    expect(condition(), label, detail())


def build():
    """The Build operator; without the game, Export (glTF, asset.json and the compiler's validation) instead."""
    return call(bpy.ops.liberty.export if NO_GAME else bpy.ops.liberty.build)


def call(operator, **options):
    """Runs an operator; a CANCELLED operator that reported an error raises in Python, so both come back as a result set."""
    try:
        return operator(**options)
    except RuntimeError as error:
        print("operator refused: %s" % error)
        return {'CANCELLED'}


def codes(issues):
    return [issue.code for issue in issues]


def gltf_json(name, kind="props"):
    with open(os.path.join(CONTENT, kind, name, name + ".gltf"), "r", encoding="utf-8") as stream:
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
    from liberty_exporter import checks, exporter, lcc, state  # noqa: E402
    prefs = bpy.context.preferences.addons["liberty_exporter"].preferences
    prefs.content_root = CONTENT
    prefs.compiler_path = COMPILER
    prefs.game_directory = "" if NO_GAME else GAME
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
    outcome = build()
    result = state.get(scene)
    if NO_GAME:
        expect(outcome == {'FINISHED'} and result.status == "exported", "barrel export validated", "%s %s %s" % (outcome, result.status, result.log[-600:]))
    expect_built(lambda: outcome == {'FINISHED'} and result.status == "ok", "barrel build ok", lambda: "%s %s %s" % (outcome, result.status, result.log[-600:]))
    folder = os.path.join(CONTENT, "props", "lf_bt_barrel")
    manifest = json.load(open(os.path.join(folder, "asset.json"), encoding="utf-8"))
    expect(manifest["name"] == "lf_bt_barrel" and manifest["source"] == "lf_bt_barrel.gltf" and manifest["textureDictionary"] == "lf_bt_barrel" and
           manifest["template"]["model"] == "amb_nailgun", "asset.json written", manifest)
    document = gltf_json("lf_bt_barrel")
    node_extras = {n["name"]: n.get("extras", {}) for n in document["nodes"]}
    expect(node_extras.get("lf_bt_barrel_far", {}).get("liberty_lod") == 2, "liberty_lod exported as node extras", node_extras)
    scene_extras = document["scenes"][0].get("extras", {})
    expect(scene_extras.get("liberty_exporter") == exporter.EXPORTER_VERSION and "liberty_exporter" not in scene.keys(), "scene tags exported and removed again", scene_extras)
    expect(any(i.get("uri", "").endswith(".png") for i in document.get("images", [])), "texture written as PNG", document.get("images"))
    built = {} if NO_GAME else report("lf_bt_barrel")
    expect_built(lambda: built["lods"] == 3 and built["metadata"].get("liberty_exporter") == exporter.EXPORTER_VERSION, "report: 3 LODs, exporter metadata", lambda: built)
    expect_built(lambda: os.path.isfile(result.preview) and os.path.isfile(result.texture), "previews written", lambda: result.preview)
    expect_built(lambda: not any(i["code"] == "LCC017" for i in built["issues"]), "LOD triangle order accepted")

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
    build()
    result = state.get(scene)
    built = report("lf_bt_array") if os.path.isfile(os.path.join(BUILD, "lf_bt_array", "report.json")) else {}
    expect_built(lambda: result.status == "ok" and built.get("compiled", {}).get("triangles") == 24, "modifiers applied (24 triangles)", lambda: built.get("compiled"))
    obj.modifiers.remove(modifier)

    # 3b. Native texture mode: asset.json carries it; the dictionary is written from scratch at the source size (128x128,
    # full mip chain to 4 px = 6 levels, opaque -> DXT1) and read back byte for byte with a PSNR measurement.
    clear_scene()
    obj = box("lf_bt_native", (-0.25, -0.25, 0.0), (0.25, 0.25, 0.5), material("native", texture("native_basecolor", 128)))
    select(obj)
    settings.asset_name = "lf_bt_native"
    settings.texture_mode = 'native'
    build()
    settings.texture_mode = 'template'
    result = state.get(scene)
    manifest = json.load(open(os.path.join(CONTENT, "props", "lf_bt_native", "asset.json"), encoding="utf-8"))
    compiled = report("lf_bt_native").get("compiled", {}) if os.path.isfile(os.path.join(BUILD, "lf_bt_native", "report.json")) else {}
    if NO_GAME:
        expect(result.status == "exported" and manifest.get("textureMode") == "native", "native texture mode written to asset.json", manifest)
    expect_built(lambda: result.status == "ok" and manifest.get("textureMode") == "native", "native texture mode builds", lambda: "%s %s" % (result.status, result.log[-400:]))
    expect_built(lambda: compiled.get("textureMode") == "native" and compiled.get("textureFormat") == "DXT1" and compiled.get("textureSize") == [128, 128] and
           compiled.get("textureLevels") == 6 and compiled.get("textureQuality", {}).get("psnrRgbDb", 0) >= 30, "native texture: 128x128 DXT1, 6 levels, PSNR >= 30 dB", lambda: compiled)

    # 4. Two materials in LOD 0: the automatic writer takes the structure writer (valid, two geometries, native textures);
    #    with Drawable writer Template it is authored as it is (a warning in Blender, exported) and refused (LCC016).
    #    An authoring error (a bad name) still writes nothing.
    clear_scene()
    a, b = box("lf_bt_two_a", (0, 0, 0), (0.5, 0.5, 0.5), material("red")), box("lf_bt_two_b", (0.5, 0, 0), (1, 0.5, 0.5), material("blue"))
    select(a, b)
    settings.asset_name = "lf_bt_two"
    outcome = call(bpy.ops.liberty.export)
    result = state.get(scene)
    validation = report("lf_bt_two") if os.path.isfile(os.path.join(BUILD, "lf_bt_two", "report.json")) else {}
    expect(outcome == {'FINISHED'} and result.status == "exported" and "LBX009" not in codes(result.issues) and "LBX033" in codes(result.issues) and
           validation.get("writer", {}).get("drawable") == "structure" and validation.get("writer", {}).get("textureMode") == "native",
           "automatic writer: two materials build with the structure writer and native textures", "%s %s %s" % (outcome, result.status, validation.get("writer")))
    settings.drawable_writer = 'template'
    outcome = call(bpy.ops.liberty.export)
    result = state.get(scene)
    lbx009 = [i for i in result.issues if i.code == "LBX009"]
    expect(lbx009 and lbx009[0].severity == "warning" and "LCC016" in lbx009[0].message, "two materials per LOD: LBX009 warning naming LCC016", lbx009)
    expect(outcome == {'CANCELLED'} and result.status == "invalid" and "LCC016" in codes(result.issues), "exported, then refused by LibertyContent (LCC016)",
           "%s %s %s" % (outcome, result.status, codes(result.issues)))
    expect(os.path.isfile(os.path.join(CONTENT, "props", "lf_bt_two", "asset.json")), "a capability refusal still writes the export")
    validation = report("lf_bt_two") if os.path.isfile(os.path.join(BUILD, "lf_bt_two", "report.json")) else {}
    expect(validation.get("status") == "invalid" and len(validation.get("structure", {}).get("lods", [{}])[0].get("geometries", [])) == 2 and
           result.report.endswith("report.json"), "validation report: invalid, LOD 0 with two geometries", validation.get("structure"))
    settings.drawable_writer = 'auto'
    settings.asset_name = "lf bt two"
    outcome = call(bpy.ops.liberty.export)
    expect(outcome == {'CANCELLED'} and "LBX002" in codes(state.get(scene).issues) and not os.path.exists(os.path.join(CONTENT, "props", "lf bt two")),
           "an authoring error (LBX002) writes nothing")

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
    build()
    result = state.get(scene)
    built = report("lf_bt_coll") if os.path.isfile(os.path.join(BUILD, "lf_bt_coll", "report.json")) else {}
    if NO_GAME:
        expect(result.status == "exported", "collection export validated", result.log[-400:])
    expect_built(lambda: result.status == "ok" and built.get("lods") == 2, "collection export builds with 2 LODs", lambda: "%s %s" % (result.status, built.get("lods")))
    expect_built(lambda: built.get("metadata", {}).get("liberty_exporter") == exporter.EXPORTER_VERSION and "liberty_exporter" not in collection.keys(), "collection export carries exporter metadata", lambda: built.get("metadata"))

    # 7. The authoring fixtures (tests/fixtures.py; committed under tests/content/fixtures): export each, then check what
    #    LibertyContent made of it against the fixture's expectations. v1 refuses all but lf_fx_lods on purpose.
    for build_fixture in fixtures.FIXTURES:
        clear_scene()
        wanted = build_fixture(settings)
        name, kind = settings.asset_name, fixtures.KINDS[settings.asset_type]
        blender_errors = [i for i in checks.run(bpy.context, settings) if i.severity == "error"]
        expect(not blender_errors, "%s: no add-on errors (capability limits are warnings)" % name, blender_errors)
        build()
        result = state.get(scene)
        exported = os.path.isfile(os.path.join(CONTENT, kind, name, "asset.json"))
        expect(exported, "%s: exported to %s/" % (name, kind))
        if not exported:
            continue
        fixtures.ordered(wanted, gltf_json(name, kind))
        built = report(name) if os.path.isfile(os.path.join(BUILD, name, "report.json")) else {}
        status = built.get("status")
        # A build of a valid asset reports "ok" (it needs the game); validation reports "valid".
        want_status = ("ok" if not NO_GAME else "valid") if wanted["status"] == "valid" else "invalid"
        expect(status == want_status, "%s: LibertyContent status %s" % (name, want_status), "%s (%s)" % (status, result.log[-400:]))
        errors = sorted(set(i["code"] for i in built.get("issues", []) if i["severity"] == "error"))
        expect(errors == sorted(wanted["errors"]), "%s: errors %s" % (name, wanted["errors"]), errors)
        found = set(i["code"] for i in built.get("issues", []))
        expect(all(code in found for code in wanted["codes"]), "%s: codes %s reported" % (name, wanted["codes"]), sorted(found))
        problems = []
        fixtures.match(wanted["report"], built, "report", problems)
        expect(not problems, "%s: report structure as authored" % name, problems)
    document = gltf_json("lf_fx_collision")
    extras = {n["name"]: n.get("extras", {}) for n in document["nodes"]}
    expect(extras.get("fx_box", {}).get("liberty_collision") == "box" and extras.get("fx_box", {}).get("liberty_surface") == "wood" and
           extras.get("lf_fx_collision_col", {}).get("liberty_surface") == "wood", "collision tags exported as node extras", extras)
    manifest = json.load(open(os.path.join(CONTENT, "objects", "lf_fx_world", "asset.json"), encoding="utf-8"))
    expect(manifest.get("type") == "object" and manifest.get("lodDistancesMeters") == [60.0, 150.0], "world object asset.json: type and one distance per LOD", manifest)
    manifest = json.load(open(os.path.join(CONTENT, "props", "lf_fx_multimat", "asset.json"), encoding="utf-8"))
    expect("lodDistancesMeters" not in manifest, "LOD distances written only when enabled", manifest)

    # 8. Collision authoring mistakes: caught in Blender (errors stop the export, warnings do not).
    clear_scene()
    crate = box("lf_bt_hull", (-0.4, -0.3, 0), (0.4, 0.3, 0.5), material("crate"))
    cone = tag_collision(placed(unit_box("bt_cone"), (0, 0, 0.25), scale=(0.2, 0.2, 0.2)), "cone")
    select(crate, cone)
    settings.asset_name = "lf_bt_hull"
    found = codes(checks.run(bpy.context, settings))
    expect("LBX023" in found and checks.has_errors(checks.run(bpy.context, settings)), "unknown collision shape: LBX023 error", found)
    cone["liberty_collision"] = "box"
    cone["liberty_surface"] = "not a name"
    expect("LBX023" in codes(checks.run(bpy.context, settings)), "bad surface name: LBX023")
    del cone["liberty_surface"]
    stub = tag_collision(placed(cylinder("bt_stub", 16, 0.3, 0.2, centred=True), (0, 0, 0.25)), "capsule")
    egg = tag_collision(placed(uv_sphere("bt_egg", 0.2), (0, 0, 0.25), scale=(1, 1, 0.5)), "sphere")
    tagged = tag_collision(placed(unit_box("bt_tagged_lod1"), (0, 0, 0.25), scale=(0.3, 0.3, 0.3)), "box")
    far = tag_collision(placed(unit_box("bt_far"), (30, 0, 0.25), scale=(0.3, 0.3, 0.3)), "box")
    select(crate, cone, stub, egg, tagged, far)
    issues = checks.run(bpy.context, settings)
    by_code = {}
    for issue in issues:
        by_code.setdefault(issue.code, []).append(issue)
    expect(any("bt_stub" in i.message and "shorter than its diameter" in i.message for i in by_code.get("LBX024", [])), "capsule shorter than its diameter: LBX024", issues)
    expect(any("bt_egg" in i.message for i in by_code.get("LBX025", [])), "non-round sphere: LBX025 warning", issues)
    expect(any("bt_tagged_lod1" in i.message for i in by_code.get("LBX026", [])), "LOD tag on collision: LBX026 warning", issues)
    expect(any("bt_far" in i.message for i in by_code.get("LBX027", [])), "collision outside the model: LBX027 warning", issues)
    expect([i.severity for i in by_code.get("LBX028", [])] == ["warning"], "collision with compiler v1: one LBX028 warning", by_code.get("LBX028"))
    expect("LBX001" not in by_code and "LBX021" in by_code and "5 collision shape(s)" in by_code["LBX021"][0].message, "collision objects are not render objects", by_code.get("LBX021"))
    select(stub)
    expect("LBX001" in codes(checks.run(bpy.context, settings)), "collision alone is nothing to export (LBX001)")

    # 9. LOD distances and world objects in Blender.
    clear_scene()
    near = cylinder("lf_bt_dist", 12, 0.3, 0.9, material("dist"))
    lod1 = cylinder("lf_bt_dist_lod1", 6, 0.3, 0.9, material("dist"))
    select(near, lod1)
    settings.asset_name = "lf_bt_dist"
    settings.use_lod_distances = True
    settings.lod_distances = (80.0, 40.0, 0.0, 0.0)
    expect("LBX030" in codes(checks.run(bpy.context, settings)), "descending LOD distances: LBX030")
    settings.lod_distances = (40.0, 200.0, 0.0, 0.0)
    expect("LBX030" in codes(checks.run(bpy.context, settings)), "LOD distance beyond the draw distance: LBX030")
    settings.lod_distances = (40.0, 100.0, 0.0, 0.0)
    expect("LBX030" not in codes(checks.run(bpy.context, settings)), "only the distances of existing LODs are checked (LOD 2-3 zero)")
    settings.asset_type = 'object'
    found = codes(checks.run(bpy.context, settings))
    expect("LBX029" in found and "LBX032" in found, "world object with v1 (LBX029) and without collision (LBX032)", found)
    lod3 = cylinder("lf_bt_dist_lod3", 4, 0.3, 0.9, material("dist"))
    select(near, lod1, lod3)
    expect("LBX031" in codes(checks.run(bpy.context, settings)), "LOD 3 without LOD 2: LBX031")

    # 10. Operators: collision, surface and shader tags.
    clear_scene()
    obj = box("lf_bt_ops", (0, 0, 0), (0.5, 0.5, 0.5), material("ops"))
    select(obj)
    call(bpy.ops.liberty.set_collision, shape='CAPSULE')
    expect(obj.get("liberty_collision") == "capsule" and obj.display_type == 'WIRE' and checks.collision_of(obj) == "capsule", "set_collision tags and shows wireframe")
    call(bpy.ops.liberty.set_surface, surface="metal")
    expect(obj.get("liberty_surface") == "metal", "set_surface")
    call(bpy.ops.liberty.set_surface, surface="")
    expect("liberty_surface" not in obj.keys(), "an empty surface removes the tag")
    call(bpy.ops.liberty.set_collision, shape='NONE')
    expect("liberty_collision" not in obj.keys() and obj.display_type == 'TEXTURED' and checks.collision_of(obj) is None, "collision None removes the tag")
    named = box("lf_bt_ops_col", (0, 0, 0), (0.5, 0.5, 0.5))
    expect(checks.collision_of(named) == "mesh", "an _col name is mesh collision")
    select(named)
    call(bpy.ops.liberty.set_collision, shape='NONE')
    expect(named.get("liberty_collision") == "none" and checks.collision_of(named) is None, "collision None on an _col name writes 'none'")
    select(obj)
    call(bpy.ops.liberty.set_shader, shader="gta_normal_spec")
    expect(obj.active_material.get("liberty_shader") == "gta_normal_spec", "set_shader tags the active material")
    found = [i for i in checks.run(bpy.context, settings) if i.code == "LBX019"]
    expect(found and found[0].severity == "warning" and "LCC019" in found[0].message, "shader outside the capabilities: LBX019 warning naming LCC019", found)

    # 11. The add-on's copies of the compiler's capabilities are current (default writer and structure writer).
    reported = lcc.capabilities(COMPILER, 120)
    expect(reported == checks.CAPABILITIES, "checks.CAPABILITIES equals LibertyContent capabilities", "%s != %s" % (reported, checks.CAPABILITIES))
    code, text = lcc.run(COMPILER, ["capabilities", "--writer", "structure"], 120)
    reported = json.loads(text) if code == 0 else None
    expect(reported == checks.STRUCTURE_CAPABILITIES, "checks.STRUCTURE_CAPABILITIES equals capabilities --writer structure", "%s != %s" % (reported, checks.STRUCTURE_CAPABILITIES))

    # 12. The structure writer opt-in: two materials in LOD 0 are within its limits, asset.json carries the writer and
    #     its template, and LibertyContent validates the export with the structure capabilities (building needs the game).
    clear_scene()
    two = [box("lf_bt_struct_a", (0, 0, 0), (0.5, 0.5, 0.5), material("s_red")), box("lf_bt_struct_b", (0.5, 0, 0), (1, 0.5, 0.5), material("s_blue")),
           cylinder("lf_bt_struct_lod1", 6, 0.4, 0.5, material("s_red"))]
    select(*two)
    settings.asset_name = "lf_bt_struct"
    settings.drawable_writer = 'structure'
    found = checks.run(bpy.context, settings)
    expect([i.severity for i in found if i.code == "LBX033"] == ["info"], "structure writer with Template texture mode: LBX033 says native textures are used", codes(found))
    settings.texture_mode = 'native'
    found = checks.run(bpy.context, settings)
    expect(not checks.has_errors(found) and "LBX009" not in codes(found) and "LBX033" not in codes(found), "structure writer: two materials in LOD 0 are no warning", codes(found))
    outcome = call(bpy.ops.liberty.export)
    result = state.get(scene)
    manifest = json.load(open(os.path.join(CONTENT, "props", "lf_bt_struct", "asset.json"), encoding="utf-8"))
    expect(manifest.get("drawableWriter") == "structure" and "structureTemplate" not in manifest and manifest.get("textureMode") == "native",
           "asset.json: drawableWriter and textureMode; the default template search is not written", manifest)
    validation = report("lf_bt_struct") if os.path.isfile(os.path.join(BUILD, "lf_bt_struct", "report.json")) else {}
    expect(outcome == {'FINISHED'} and result.status == "exported" and validation.get("capabilities", {}).get("version") == "v2-structure",
           "LibertyContent validates it with the structure capabilities", "%s %s %s" % (outcome, result.status, validation.get("capabilities")))
    settings.drawable_writer = 'auto'
    settings.texture_mode = 'template'

    print("RESULT passed=%d failed=%d%s" % (results["passed"], results["failed"], (" notrun=%d" % results["notrun"]) if results["notrun"] else ""))
    if results["failed"]:
        sys.exit(1)


main()
