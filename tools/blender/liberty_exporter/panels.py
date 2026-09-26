import bpy

from . import checks, lcc, preferences, state

ICONS = {"error": 'CANCEL', "warning": 'ERROR', "info": 'INFO'}
MAX_ISSUES_SHOWN = 25


class _LibertyPanel:
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "Liberty"


class LIBERTY_PT_asset(_LibertyPanel, bpy.types.Panel):
    bl_label = "Liberty Asset"

    def draw(self, context):
        layout = self.layout
        layout.use_property_split = True
        layout.use_property_decorate = False
        settings = context.scene.liberty_asset
        prefs = preferences.get(context)
        if prefs is None or not prefs.content_root:
            layout.label(text="Set the content folder in the add-on preferences", icon='ERROR')
        row = layout.row(align=True)
        row.prop(settings, "asset_name")
        row.operator("liberty.use_active_name", text="", icon='EYEDROPPER')
        layout.prop(settings, "asset_type")
        layout.prop(settings, "collection")
        if settings.collection is None:
            layout.label(text="Exports the selected objects", icon='RESTRICT_SELECT_OFF')
        column = layout.column(align=True)
        column.prop(settings, "template_archive", text="Template")
        column.prop(settings, "template_model", text="Model")
        layout.prop(settings, "texture_dictionary")
        layout.prop(settings, "texture_mode")
        layout.prop(settings, "draw_distance")
        layout.prop(settings, "audio_material")


class LIBERTY_PT_object(_LibertyPanel, bpy.types.Panel):
    bl_label = "Object"
    bl_parent_id = "LIBERTY_PT_asset"

    def draw(self, context):
        layout = self.layout
        obj = context.active_object
        if obj is None:
            layout.label(text="No active object")
            return
        layout.label(text="%s: LOD %d (%s)" % (obj.name, checks.lod_of(obj), checks.lod_source(obj)))
        row = layout.row(align=True)
        for lod in range(checks.MAX_LOD + 1):
            row.operator("liberty.set_lod", text="LOD %d" % lod).lod = lod
        row.operator("liberty.clear_lod", text="", icon='X')
        material = obj.active_material
        if material is None:
            return
        layout.label(text="Material %s, shader %s" % (material.name, material.get(checks.SHADER_PROPERTY, "gta_default")))
        image, problem = checks.base_color_image(material)
        if image is not None:
            layout.label(text="Texture %s %dx%d" % (image.name, image.size[0], image.size[1]), icon='TEXTURE')
        elif problem:
            layout.label(text=problem, icon='ERROR')
        else:
            layout.label(text="No texture: the base colour fills it", icon='INFO')


class LIBERTY_PT_build(_LibertyPanel, bpy.types.Panel):
    bl_label = "Check and Build"
    bl_parent_id = "LIBERTY_PT_asset"

    def draw(self, context):
        layout = self.layout
        row = layout.row(align=True)
        row.operator("liberty.check", icon='CHECKMARK')
        row.operator("liberty.export", icon='EXPORT')
        row.operator("liberty.build", icon='MOD_BUILD')
        prefs = preferences.get(context)
        if prefs is not None and prefs.content_root and not lcc.compiler_path(prefs):
            layout.label(text="LibertyContent.exe not configured", icon='ERROR')

        result = state.get(context.scene)
        if not result.status:
            return
        layout.label(text="Status: %s%s" % (result.status, (" - " + result.message) if result.message else ""))
        row = layout.row(align=True)
        for label, path, icon in (("Folder", result.folder, 'FILE_FOLDER'), ("Preview", result.preview, 'IMAGE_DATA'),
                                  ("Texture", result.texture, 'TEXTURE'), ("Report", result.report, 'TEXT')):
            if path:
                row.operator("liberty.open_path", text=label, icon=icon).path = path
        if result.issues:
            box = layout.box()
            for issue in result.issues[:MAX_ISSUES_SHOWN]:
                box.label(text="%s: %s" % (issue.code, issue.message), icon=ICONS.get(issue.severity, 'DOT'))
            if len(result.issues) > MAX_ISSUES_SHOWN:
                box.label(text="... %d more (see the system console)" % (len(result.issues) - MAX_ISSUES_SHOWN))


_classes = (LIBERTY_PT_asset, LIBERTY_PT_object, LIBERTY_PT_build)


def register():
    for cls in _classes:
        bpy.utils.register_class(cls)


def unregister():
    for cls in reversed(_classes):
        bpy.utils.unregister_class(cls)
