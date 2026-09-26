# Writes the authoring fixtures to tests/content/fixtures/<name>/: the glTF and asset.json the Liberty Exporter add-on
# exports, and expect.json from the fixture's builder (tools/blender/tests/fixtures.py). `LibertyContent fixtures
# tests/content/fixtures` then checks them without Blender. Run by tools/blender/make-examples.ps1, or with bpy as a
# Python module (the cloud container):
#   blender -b --factory-startup --python-exit-code 1 --python make_fixtures.py -- <repo>
#   python make_fixtures.py -- <repo>
# The fixtures stay outside content/: v1 refuses most of them on purpose, and package-phase2.ps1 builds all of content/.

import json
import os
import shutil
import sys
import tempfile

import bpy  # first: as the bpy Python module, importing bpy is what puts addon_utils on the path
import addon_utils

REPO = sys.argv[sys.argv.index("--") + 1]
sys.path.insert(0, os.path.join(REPO, "tools", "blender"))
sys.path.insert(0, os.path.join(REPO, "tools", "blender", "tests"))
import fixtures  # noqa: E402

TARGET = os.path.join(REPO, "tests", "content", "fixtures")


def main():
    addon_utils.enable("liberty_exporter", default_set=True, handle_error=None)
    from liberty_exporter import state  # noqa: E402
    work = tempfile.mkdtemp(prefix="liberty_fixtures_")
    prefs = bpy.context.preferences.addons["liberty_exporter"].preferences
    prefs.content_root = work
    prefs.compiler_path = os.path.join(REPO, "tools", "content", "bin", "LibertyContent.exe")
    prefs.build_directory = os.path.join(work, "build")
    os.makedirs(TARGET, exist_ok=True)
    failed = 0
    try:
        for build in fixtures.FIXTURES:
            fixtures.clear_scene()
            settings = bpy.context.scene.liberty_asset
            expect = build(settings)
            try:
                bpy.ops.liberty.export()
            except RuntimeError as error:  # a refused (invalid) fixture cancels after writing its files
                print("export reported: %s" % error)
            result = state.get(bpy.context.scene)
            source = os.path.join(work, fixtures.KINDS[settings.asset_type], settings.asset_name)
            # The add-on's own errors (LBX) stop an export; LibertyContent's refusals (LCC) are what the fixture expects.
            blocking = [tuple(i) for i in result.issues if i.severity == "error" and i.code.startswith("LBX")]
            if blocking or not os.path.isfile(os.path.join(source, "asset.json")):
                failed += 1
                print("FIXTURE %s not exported: %s" % (settings.asset_name, blocking))
                continue
            with open(os.path.join(source, settings.asset_name + ".gltf"), "r", encoding="utf-8") as stream:
                fixtures.ordered(expect, json.load(stream))
            destination = os.path.join(TARGET, settings.asset_name)
            shutil.rmtree(destination, ignore_errors=True)
            shutil.copytree(source, destination)
            with open(os.path.join(destination, "expect.json"), "w", encoding="utf-8", newline="\n") as stream:
                json.dump(expect, stream, indent=2)
                stream.write("\n")
            print("FIXTURE %s written (%s, LibertyContent: %s)" % (settings.asset_name, destination, result.status))
    finally:
        shutil.rmtree(work, ignore_errors=True)
    if failed:
        sys.exit(1)


main()
