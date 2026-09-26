# Checks an asset in Blender before export. The rules mirror LibertyContent's validator (tools/content/AssetValidator.cs,
# codes LCC001-LCC023). They add what only Blender can see: unit scale, modifiers, armatures, missing image files and
# material node setups the glTF exporter cannot translate. LibertyContent runs its own validation after export and stays
# the authority. Codes (LBX001-LBX021) are stable so reports and tests can refer to them.

import math
import os
import re
from collections import namedtuple

import bpy
import numpy as np

Issue = namedtuple("Issue", "severity code message")

NAME_PATTERN = re.compile(r"^[A-Za-z0-9_]{1,23}$")
# Same rule as the compiler's GltfImporter.LodSuffix, including Blender's duplicate suffix (".001").
LOD_SUFFIX = re.compile(r"_lod(\d+)(\.\d+)?$", re.IGNORECASE)
LOD_PROPERTY = "liberty_lod"
SHADER_PROPERTY = "liberty_shader"

# Limits of the compiler version this add-on targets (AssetValidator.cs); format limits, not tuning values.
SUPPORTED_SHADERS = ("gta_default",)
MAX_VERTICES_PER_GEOMETRY = 65535
MIN_EXTENT_METERS, MAX_EXTENT_METERS = 0.02, 200.0
MAX_TEXTURE_SIZE = 2048
MAX_MATERIALS_PER_LOD = 1
MAX_LOD = 3
SEVERITY_ORDER = {"error": 0, "warning": 1, "info": 2}


def lod_of(obj):
    """LOD as the compiler reads it: the liberty_lod custom property, else an _lod<N> name suffix, else the parent's."""
    while obj is not None:
        if LOD_PROPERTY in obj.keys():
            return int(obj[LOD_PROPERTY])
        match = LOD_SUFFIX.search(obj.name)
        if match:
            return int(match.group(1))
        obj = obj.parent
    return 0


def lod_source(obj):
    if LOD_PROPERTY in obj.keys():
        return "custom property"
    if LOD_SUFFIX.search(obj.name):
        return "name"
    return "parent" if obj.parent is not None and lod_of(obj.parent) != 0 else "default"


def export_objects(context, settings):
    """Mesh objects the export will write: the asset collection (with nested collections) or the selection."""
    if settings.collection is not None:
        return [o for o in settings.collection.all_objects if o.type == 'MESH']
    return [o for o in context.selected_objects if o.type == 'MESH']


def principled_of(material):
    if material is None or material.node_tree is None:
        return None
    return next((n for n in material.node_tree.nodes if n.type == 'BSDF_PRINCIPLED'), None)


def base_color_image(material):
    """(image, problem): the Image Texture feeding Principled BSDF's Base Color, as the glTF exporter reads it."""
    if material is None or material.node_tree is None:
        return None, None
    bsdf = principled_of(material)
    if bsdf is None:
        return None, "no Principled BSDF (the glTF exporter only translates Principled BSDF materials)"
    socket = bsdf.inputs.get("Base Color")
    if socket is None or not socket.is_linked:
        return None, None
    node = socket.links[0].from_node
    while node.type == 'REROUTE' and node.inputs[0].is_linked:
        node = node.inputs[0].links[0].from_node
    if node.type == 'TEX_IMAGE':
        return node.image, None if node.image is not None else "Image Texture node without an image"
    return None, "Base Color comes from a %s node; only a direct Image Texture is exported" % node.bl_idname


def _image_issues(image, add):
    if image.source not in ('FILE', 'GENERATED'):
        add("error", "LBX012", "image %s: source %s is not supported (one still image)" % (image.name, image.source))
        return
    if image.source == 'FILE' and image.packed_file is None:
        path = bpy.path.abspath(image.filepath, library=image.library)
        if not os.path.isfile(path):
            add("error", "LBX012", "image %s: file %s not found (fix the path or pack the image)" % (image.name, path))
            return
    width, height = image.size[0], image.size[1]
    if width == 0 or height == 0:
        add("error", "LBX012", "image %s has no pixel data" % image.name)
        return
    if not (_power_of_two(width) and _power_of_two(height)):
        add("warning", "LBX013", "image %s is %dx%d (not a power of two; it will be resampled)" % (image.name, width, height))
    if width > MAX_TEXTURE_SIZE or height > MAX_TEXTURE_SIZE:
        add("warning", "LBX013", "image %s is larger than %d (it will be resampled)" % (image.name, MAX_TEXTURE_SIZE))


def _power_of_two(value):
    return value > 0 and (value & (value - 1)) == 0


def _is_skinned(obj):
    return any(m.type == 'ARMATURE' for m in obj.modifiers) or (obj.parent is not None and obj.parent.type == 'ARMATURE')


def run(context, settings):
    """All checks for the scene's asset settings; returns Issue tuples sorted errors first."""
    issues = []

    def add(severity, code, message):
        issues.append(Issue(severity, code, message))

    name = settings.asset_name
    if not NAME_PATTERN.match(name):
        add("error", "LBX002", "asset name '%s' must be 1-23 letters, digits or _" % name)
    dictionary = settings.texture_dictionary or name
    if not NAME_PATTERN.match(dictionary):
        add("error", "LBX003", "texture dictionary '%s' must be 1-23 letters, digits or _" % dictionary)

    objects = export_objects(context, settings)
    if not objects:
        add("error", "LBX001", "nothing to export: select mesh objects or set the asset collection")
        return _sorted(issues)

    unit = context.scene.unit_settings.scale_length
    if abs(unit - 1.0) > 1e-6:
        add("warning", "LBX004", "scene unit scale is %g; glTF writes Blender units as metres (set Unit Scale to 1)" % unit)

    depsgraph = context.evaluated_depsgraph_get()
    low = np.full(3, np.inf)
    high = np.full(3, -np.inf)
    triangles_per_lod = {}
    materials_per_lod = {}
    materials = set()
    for obj in objects:
        lod = lod_of(obj)
        if lod < 0 or lod > MAX_LOD:
            add("error", "LBX017", "%s: LOD %d is outside 0-%d" % (obj.name, lod, MAX_LOD))
            continue
        if _is_skinned(obj):
            add("warning", "LBX015", "%s is skinned: this compiler version writes static props (exported in its current pose)" % obj.name)
        evaluated = obj.evaluated_get(depsgraph)
        mesh = evaluated.to_mesh()
        try:
            if len(mesh.polygons) == 0:
                add("warning", "LBX022", "%s has no faces (ignored)" % obj.name)
                continue
            mesh.calc_loop_triangles()
            count = len(mesh.vertices)
            coordinates = np.empty(count * 3, dtype=np.float64)
            mesh.vertices.foreach_get("co", coordinates)
            matrix = np.array(obj.matrix_world, dtype=np.float64)
            world = coordinates.reshape(-1, 3) @ matrix[:3, :3].T + matrix[:3, 3]
            if not np.all(np.isfinite(world)):
                add("error", "LBX008", "%s has NaN/infinite vertex positions" % obj.name)
                continue
            low = np.minimum(low, world.min(axis=0))
            high = np.maximum(high, world.max(axis=0))
            if len(mesh.loops) > MAX_VERTICES_PER_GEOMETRY:
                add("warning", "LBX016", "%s has %d face corners; after splitting seams and hard edges it may exceed the %d vertices one geometry allows" %
                    (obj.name, len(mesh.loops), MAX_VERTICES_PER_GEOMETRY))
            triangles_per_lod[lod] = triangles_per_lod.get(lod, 0) + len(mesh.loop_triangles)
            indices = np.empty(len(mesh.polygons), dtype=np.int32)
            mesh.polygons.foreach_get("material_index", indices)
            slots = [slot.material for slot in obj.material_slots]
            used = set(slots[i] if 0 <= i < len(slots) else None for i in np.unique(indices).tolist())
            if None in used:
                add("warning", "LBX010", "%s has faces without a material (the default white material is used)" % obj.name)
            materials_per_lod.setdefault(lod, set()).update(used)
            used.discard(None)
            materials.update(used)
            if any(base_color_image(m)[0] is not None for m in used) and len(mesh.uv_layers) == 0:
                add("error", "LBX014", "%s is textured but has no UV map" % obj.name)
        finally:
            evaluated.to_mesh_clear()

    for material in sorted(materials, key=lambda m: m.name):
        shader = str(material.get(SHADER_PROPERTY, "gta_default"))
        if shader not in SUPPORTED_SHADERS:
            add("error", "LBX019", "material %s asks for shader '%s' (supported: %s)" % (material.name, shader, ", ".join(SUPPORTED_SHADERS)))
        image, problem = base_color_image(material)
        if problem:
            add("warning", "LBX011", "material %s: %s; the texture is filled with the base colour" % (material.name, problem))
        if image is not None:
            _image_issues(image, add)
        bsdf = principled_of(material)
        alpha = bsdf.inputs.get("Alpha") if bsdf is not None else None
        if alpha is not None and (alpha.is_linked or alpha.default_value < 0.999):
            if settings.texture_mode == 'native':
                add("warning", "LBX020", "material %s uses alpha; native mode writes DXT5, but whether gta_default draws it translucent "
                                         "is unverified in game" % material.name)
            else:
                add("warning", "LBX020", "material %s uses alpha; template mode writes opaque DXT1 (native mode keeps alpha)" % material.name)

    for lod, used in sorted(materials_per_lod.items()):
        if len(used) > MAX_MATERIALS_PER_LOD:
            add("error", "LBX009", "LOD %d uses %d materials; this compiler version writes %d per LOD (merge materials or bake an atlas)" %
                (lod, len(used), MAX_MATERIALS_PER_LOD))
    if triangles_per_lod and 0 not in triangles_per_lod:
        add("error", "LBX017", "no LOD 0 mesh")
    for lod in range(1, MAX_LOD + 1):
        if lod in triangles_per_lod and lod - 1 in triangles_per_lod and triangles_per_lod[lod] >= triangles_per_lod[lod - 1]:
            add("warning", "LBX018", "LOD %d has %d triangles, not fewer than LOD %d (%d)" % (lod, triangles_per_lod[lod], lod - 1, triangles_per_lod[lod - 1]))

    if np.all(np.isfinite(low)):
        size = high - low
        extent = float(size.max())
        if extent < MIN_EXTENT_METERS or extent > MAX_EXTENT_METERS:
            add("warning", "LBX006", "largest extent is %.3f m (expected %g-%g m): check the unit scale and object scale" %
                (extent, MIN_EXTENT_METERS, MAX_EXTENT_METERS))
        centre = (low + high) / 2
        offset = math.hypot(centre[0], centre[1])
        if offset > max(0.5, extent):
            add("warning", "LBX007", "the model's centre is %.2f m from the world origin in X/Y; the world origin is the spawn point" % offset)
        if abs(low[2]) > max(0.05, 0.1 * extent):
            add("info", "LBX008", "the lowest point is at Z=%.3f m; props usually rest on Z=0" % low[2])
        add("info", "LBX021", "%d object(s), triangles per LOD %s, bounds (%.3f, %.3f, %.3f) .. (%.3f, %.3f, %.3f) m" %
            (len(objects), dict(sorted(triangles_per_lod.items())), low[0], low[1], low[2], high[0], high[1], high[2]))
    return _sorted(issues)


def _sorted(issues):
    return sorted(issues, key=lambda issue: SEVERITY_ORDER.get(issue.severity, 3))


def has_errors(issues):
    return any(issue.severity == "error" for issue in issues)


def summary(issues):
    counts = {"error": 0, "warning": 0, "info": 0}
    for issue in issues:
        counts[issue.severity] = counts.get(issue.severity, 0) + 1
    return "%d error(s), %d warning(s)" % (counts["error"], counts["warning"])
