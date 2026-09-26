import bpy
from bpy.props import FloatProperty, StringProperty


class LibertyPreferences(bpy.types.AddonPreferences):
    bl_idname = __package__

    content_root: StringProperty(
        name="Content folder", subtype='DIR_PATH',
        description="The repository's content folder; assets are written to <folder>/props/<name>")
    compiler_path: StringProperty(
        name="LibertyContent.exe", subtype='FILE_PATH',
        description="Empty: <content folder>/../tools/content/bin/LibertyContent.exe (build it with tools/build-content.ps1)")
    game_directory: StringProperty(
        name="GTA IV folder", subtype='DIR_PATH',
        description="Folder with GTAIV.exe; builds read their template drawable from the game archives")
    build_directory: StringProperty(
        name="Build output", subtype='DIR_PATH',
        description="Where Build writes .wdr/.wtd, previews and report.json. Empty: liberty_build in the system temp folder")
    compiler_timeout: FloatProperty(
        name="Compiler timeout (s)", default=300.0, min=10.0, max=3600.0,
        description="Seconds LibertyContent may run before the add-on gives up")

    def draw(self, context):
        layout = self.layout
        layout.use_property_split = True
        layout.prop(self, "content_root")
        layout.prop(self, "compiler_path")
        layout.prop(self, "game_directory")
        layout.prop(self, "build_directory")
        layout.prop(self, "compiler_timeout")


def get(context):
    addon = context.preferences.addons.get(__package__)
    return addon.preferences if addon is not None else None


def register():
    bpy.utils.register_class(LibertyPreferences)


def unregister():
    bpy.utils.unregister_class(LibertyPreferences)
