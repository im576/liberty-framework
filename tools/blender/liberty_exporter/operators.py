import os
import re
import subprocess
import tempfile

import bpy
from bpy.props import EnumProperty, IntProperty, StringProperty

from . import checks, exporter, lcc, preferences, state


def _log(result, text):
    result.log = text
    print(text)


def _print_issues(issues):
    for issue in issues:
        print("  %s %s: %s" % (issue.severity.upper(), issue.code, issue.message))


def _export_and_compile(operator, context, build):
    """Check, export, then let LibertyContent validate (Export) or build (Build). Returns an operator result set."""
    settings = context.scene.liberty_asset
    prefs = preferences.get(context)
    result = state.reset(context.scene)
    if prefs is None or not prefs.content_root:
        result.status = "failed"
        operator.report({'ERROR'}, "Set the content folder in the Liberty Exporter preferences")
        return {'CANCELLED'}

    result.issues = checks.run(context, settings)
    if checks.has_errors(result.issues):
        result.status = "failed"
        result.message = checks.summary(result.issues)
        _print_issues(result.issues)
        operator.report({'ERROR'}, "Not exported: %s (see the Liberty panel)" % result.message)
        return {'CANCELLED'}

    try:
        exported = exporter.export(context, settings, bpy.path.abspath(prefs.content_root))
    except Exception as error:  # the exporter's own errors are reported, never swallowed
        result.status = "failed"
        result.message = "glTF export failed: %s" % error
        operator.report({'ERROR'}, result.message)
        return {'CANCELLED'}
    result.folder = exported.folder

    compiler = lcc.compiler_path(prefs)
    if not os.path.isfile(compiler):
        result.status = "exported"
        result.message = "LibertyContent.exe not found (%s); compiler checks skipped" % (compiler or "no path")
        operator.report({'WARNING'}, "Exported to %s; %s" % (exported.folder, result.message))
        return {'FINISHED'}

    output = bpy.path.abspath(prefs.build_directory) if prefs.build_directory else os.path.join(tempfile.gettempdir(), "liberty_build")
    folder = os.path.join(output, settings.asset_name)
    # Build and the validation report share <output>/<name>/report.json: a stale one must never be read as this run's.
    report = os.path.join(folder, "report.json")
    try:
        if os.path.isfile(report):
            os.remove(report)
        if build:
            game = bpy.path.abspath(prefs.game_directory)
            if not os.path.isfile(os.path.join(game, "GTAIV.exe")):
                result.status = "failed"
                result.message = "Set the GTA IV folder (with GTAIV.exe) in the preferences to build"
                operator.report({'ERROR'}, result.message)
                return {'CANCELLED'}
            code, text = lcc.run(compiler, ["build", game, exported.manifest, output], prefs.compiler_timeout)
            result.report = report
            data = lcc.read_report(report)
            result.status = data.get("status", "failed") if data else "failed"
            for path, attribute in ((settings.asset_name + "_preview.png", "preview"), (settings.asset_name + "_texture.png", "texture")):
                if os.path.isfile(os.path.join(folder, path)):
                    setattr(result, attribute, os.path.join(folder, path))
        else:
            # The validation report: status valid/invalid, the asset's structure and every issue.
            code, text = lcc.run(compiler, ["validate", exported.manifest, "--report", report], prefs.compiler_timeout)
            result.status = "exported" if code == 0 else "invalid"
            if os.path.isfile(report):
                result.report = report
    except (OSError, subprocess.SubprocessError) as error:
        result.status = "failed"
        result.message = "LibertyContent could not run: %s" % error
        operator.report({'ERROR'}, result.message)
        return {'CANCELLED'}

    _log(result, text)
    result.issues = result.issues + lcc.issues(text)
    result.message = checks.summary(result.issues)
    ok = result.status in ("exported", "ok")
    operator.report({'INFO'} if ok else {'ERROR'}, "%s %s: %s, %s" % ("Build" if build else "Export", settings.asset_name, result.status, result.message))
    return {'FINISHED'} if ok else {'CANCELLED'}


class LIBERTY_OT_check(bpy.types.Operator):
    bl_idname = "liberty.check"
    bl_label = "Check"
    bl_description = "Check the asset for what the Liberty Content Compiler would warn about or reject"

    def execute(self, context):
        result = state.reset(context.scene)
        result.issues = checks.run(context, context.scene.liberty_asset)
        result.status = "checked"
        result.message = checks.summary(result.issues)
        _print_issues(result.issues)
        self.report({'WARNING'} if checks.has_errors(result.issues) else {'INFO'}, "Liberty check: " + result.message)
        return {'FINISHED'}


class LIBERTY_OT_export(bpy.types.Operator):
    bl_idname = "liberty.export"
    bl_label = "Export"
    bl_description = "Check, export glTF + asset.json to the content folder, and validate with LibertyContent"

    def execute(self, context):
        return _export_and_compile(self, context, build=False)


class LIBERTY_OT_build(bpy.types.Operator):
    bl_idname = "liberty.build"
    bl_label = "Build"
    bl_description = "Export, then compile with LibertyContent (WDR/WTD, read-back verification, previews, report.json)"

    def execute(self, context):
        return _export_and_compile(self, context, build=True)


class LIBERTY_OT_set_lod(bpy.types.Operator):
    bl_idname = "liberty.set_lod"
    bl_label = "Set LOD"
    bl_description = "Tag the selected objects with this LOD (liberty_lod custom property, exported as glTF extras)"
    bl_options = {'REGISTER', 'UNDO'}

    lod: IntProperty(name="LOD", min=0, max=checks.MAX_LOD)

    @classmethod
    def poll(cls, context):
        return bool(context.selected_objects)

    def execute(self, context):
        for obj in context.selected_objects:
            obj[checks.LOD_PROPERTY] = self.lod
        return {'FINISHED'}


class LIBERTY_OT_clear_lod(bpy.types.Operator):
    bl_idname = "liberty.clear_lod"
    bl_label = "Clear LOD"
    bl_description = "Remove the LOD tag from the selected objects (the name suffix or parent decides again)"
    bl_options = {'REGISTER', 'UNDO'}

    @classmethod
    def poll(cls, context):
        return bool(context.selected_objects)

    def execute(self, context):
        for obj in context.selected_objects:
            if checks.LOD_PROPERTY in obj.keys():
                del obj[checks.LOD_PROPERTY]
        return {'FINISHED'}


COLLISION_ITEMS = [('NONE', "None", "A render object (removes the collision tag)"),
                   ('MESH', "Mesh", "Collision from the object's triangles"),
                   ('BOX', "Box", "A box fitted to the object's local bounds and transform"),
                   ('SPHERE', "Sphere", "A sphere fitted to the object's local bounds (radius: the largest half extent)"),
                   ('CAPSULE', "Capsule", "A capsule along the object's local Z axis, fitted to its local bounds")]


class LIBERTY_OT_set_collision(bpy.types.Operator):
    bl_idname = "liberty.set_collision"
    bl_label = "Set Collision"
    bl_description = "Tag the selected objects as collision of this shape (liberty_collision custom property, exported as glTF extras); collision is never drawn"
    bl_options = {'REGISTER', 'UNDO'}

    shape: EnumProperty(name="Shape", items=COLLISION_ITEMS, default='MESH')

    @classmethod
    def poll(cls, context):
        return any(o.type == 'MESH' for o in context.selected_objects)

    def execute(self, context):
        for obj in context.selected_objects:
            if obj.type != 'MESH':
                continue
            if self.shape == 'NONE':
                # An _col name makes an object collision on its own: "none" overrides it, else the tag is removed.
                if checks.COLLISION_SUFFIX.search(obj.name):
                    obj[checks.COLLISION_PROPERTY] = checks.COLLISION_NONE
                elif checks.COLLISION_PROPERTY in obj.keys():
                    del obj[checks.COLLISION_PROPERTY]
                obj.display_type = 'TEXTURED'
            else:
                obj[checks.COLLISION_PROPERTY] = self.shape.lower()
                obj.display_type = 'WIRE'
        return {'FINISHED'}


class LIBERTY_OT_set_surface(bpy.types.Operator):
    bl_idname = "liberty.set_surface"
    bl_label = "Set Collision Surface"
    bl_description = "Name the surface of the selected collision objects (liberty_surface; empty removes it). How the game uses surfaces is decided by the collision writer"
    bl_options = {'REGISTER', 'UNDO'}

    surface: StringProperty(name="Surface", maxlen=31, description="1-31 letters, digits or _")

    @classmethod
    def poll(cls, context):
        return any(checks.collision_of(o) is not None for o in context.selected_objects)

    def invoke(self, context, event):
        active = context.active_object
        if active is not None and checks.SURFACE_PROPERTY in active.keys():
            self.surface = str(active[checks.SURFACE_PROPERTY])
        return context.window_manager.invoke_props_dialog(self)

    def execute(self, context):
        for obj in context.selected_objects:
            if checks.collision_of(obj) is None:
                continue
            if self.surface:
                obj[checks.SURFACE_PROPERTY] = self.surface
            elif checks.SURFACE_PROPERTY in obj.keys():
                del obj[checks.SURFACE_PROPERTY]
        return {'FINISHED'}


class LIBERTY_OT_set_shader(bpy.types.Operator):
    bl_idname = "liberty.set_shader"
    bl_label = "Set Shader"
    bl_description = "Set the active material's GTA IV shader (liberty_shader custom property, exported as glTF extras)"
    bl_options = {'REGISTER', 'UNDO'}

    shader: StringProperty(name="Shader", default="gta_default",
                           description="GTA IV shader name; the compiler refuses shaders its writer does not emit (LCC019)")

    @classmethod
    def poll(cls, context):
        return context.active_object is not None and context.active_object.active_material is not None

    def invoke(self, context, event):
        self.shader = str(context.active_object.active_material.get(checks.SHADER_PROPERTY, "gta_default"))
        return context.window_manager.invoke_props_dialog(self)

    def execute(self, context):
        context.active_object.active_material[checks.SHADER_PROPERTY] = self.shader.strip() or "gta_default"
        return {'FINISHED'}


class LIBERTY_OT_use_active_name(bpy.types.Operator):
    bl_idname = "liberty.use_active_name"
    bl_label = "Name from Active Object"
    bl_description = "Use the active object's name (without an _lod suffix) as the asset name"
    bl_options = {'REGISTER', 'UNDO'}

    @classmethod
    def poll(cls, context):
        return context.active_object is not None

    def execute(self, context):
        name = checks.COLLISION_SUFFIX.sub("", checks.LOD_SUFFIX.sub("", context.active_object.name))
        name = re.sub(r"\.\d+$", "", name)
        name = re.sub(r"[^A-Za-z0-9_]", "_", name).lower()[:23]
        context.scene.liberty_asset.asset_name = name
        return {'FINISHED'}


class LIBERTY_OT_open_path(bpy.types.Operator):
    bl_idname = "liberty.open_path"
    bl_label = "Open"
    bl_description = "Open this file or folder"

    path: StringProperty()

    def execute(self, context):
        if not os.path.exists(self.path):
            self.report({'ERROR'}, "Not found: " + self.path)
            return {'CANCELLED'}
        bpy.ops.wm.path_open(filepath=self.path)
        return {'FINISHED'}


_classes = (LIBERTY_OT_check, LIBERTY_OT_export, LIBERTY_OT_build, LIBERTY_OT_set_lod, LIBERTY_OT_clear_lod,
            LIBERTY_OT_set_collision, LIBERTY_OT_set_surface, LIBERTY_OT_set_shader, LIBERTY_OT_use_active_name, LIBERTY_OT_open_path)


def _menu_export(self, context):
    self.layout.operator(LIBERTY_OT_export.bl_idname, text="Liberty Asset (glTF + asset.json)")


def register():
    for cls in _classes:
        bpy.utils.register_class(cls)
    bpy.types.TOPBAR_MT_file_export.append(_menu_export)


def unregister():
    bpy.types.TOPBAR_MT_file_export.remove(_menu_export)
    for cls in reversed(_classes):
        bpy.utils.unregister_class(cls)
