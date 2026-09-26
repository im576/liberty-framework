# SPDX-License-Identifier: GPL-3.0-or-later
# (Blender add-ons run against bpy, so Blender requires a GPL-compatible license for this folder only.)
#
# Liberty Exporter: the Blender side of the Liberty Content Compiler (docs/content/README.md, docs/content/BLENDER.md).
# Blender stays the modelling tool. This add-on tags Liberty metadata, checks the scene for what the compiler would
# reject, exports glTF + asset.json, and runs LibertyContent.exe (validate/build) so problems show up in Blender
# instead of in game. It does not compile anything itself: LibertyContent stays the single authority.

bl_info = {
    "name": "Liberty Exporter",
    "author": "Liberty Framework",
    "version": (0, 1, 0),
    "blender": (4, 2, 0),
    "location": "View3D > Sidebar > Liberty; File > Export > Liberty Asset",
    "description": "Export GTA IV props through the Liberty Content Compiler",
    "category": "Import-Export",
}

if "bpy" in locals():
    import importlib
    for _module in (state, checks, lcc, exporter, preferences, properties, operators, panels):  # noqa: F821
        importlib.reload(_module)

import bpy  # noqa: E402,F401

from . import state, checks, lcc, exporter, preferences, properties, operators, panels  # noqa: E402

_modules = (preferences, properties, operators, panels)


def register():
    for module in _modules:
        module.register()


def unregister():
    for module in reversed(_modules):
        module.unregister()
    state.clear()
