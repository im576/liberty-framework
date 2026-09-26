# Checks an asset in Blender before export. The rules mirror LibertyContent's validator (tools/content/AssetValidator.cs,
# codes LCC001-LCC037). They add what only Blender can see: unit scale, modifiers, armatures, missing image files and
# material node setups the glTF exporter cannot translate. LibertyContent runs its own validation after export and stays
# the authority. Codes (LBX001-LBX033) are stable so reports and tests can refer to them.
#
# Errors are authoring mistakes and stop the export. What the current compiler cannot write yet (more materials per LOD,
# other shaders, collision, world objects: CAPABILITIES) is a warning: the asset is exported as authored, and
# LibertyContent refuses to build it with its own error until a compiler that writes it exists.

import math
import os
import re
from collections import namedtuple

import bpy
import numpy as np

Issue = namedtuple("Issue", "severity code message")

NAME_PATTERN = re.compile(r"^[A-Za-z0-9_]{1,23}$")
SURFACE_PATTERN = re.compile(r"^[A-Za-z0-9_]{1,31}$")
# Same rules as the compiler's GltfImporter.LodSuffix and CollisionSuffix, including Blender's duplicate suffix (".001").
LOD_SUFFIX = re.compile(r"_lod(\d+)(\.\d+)?$", re.IGNORECASE)
COLLISION_SUFFIX = re.compile(r"_col\d*(\.\d+)?$", re.IGNORECASE)
LOD_PROPERTY = "liberty_lod"
SHADER_PROPERTY = "liberty_shader"
COLLISION_PROPERTY = "liberty_collision"
SURFACE_PROPERTY = "liberty_surface"
COLLISION_NONE = "none"
COLLISION_SHAPES = ("mesh", "box", "sphere", "capsule")

# What LibertyContent's current writers emit: a copy of `LibertyContent capabilities` (CompilerCapabilities.cs). The
# add-on's tests compare the two, so a compiler that gains a feature fails the tests until this copy follows.
CAPABILITIES = {
    "version": "v1",
    "maxMaterialsPerLod": 1,
    "compiledLodLevels": 1,
    "lodSlots": 4,
    "maxVerticesPerGeometry": 65535,
    "shaders": ["gta_default"],
    "assetTypes": ["prop"],
    "collisionShapes": [],
    "lodDistances": False,
}
# The opt-in structure writer's limits (drawableWriter "structure", NEEDS-PLAYTEST): `LibertyContent capabilities --writer
# structure`. The structure template decides how many materials really fit; the compiler says why one does not.
STRUCTURE_CAPABILITIES = {
    "version": "v2-structure",
    "maxMaterialsPerLod": 16,
    "compiledLodLevels": 4,
    "lodSlots": 4,
    "maxVerticesPerGeometry": 65535,
    "shaders": ["gta_default"],
    "assetTypes": ["prop"],
    "collisionShapes": [],
    "lodDistances": True,
}


def capabilities_for(settings):
    """The limits of the drawable writer the asset settings choose."""
    return STRUCTURE_CAPABILITIES if getattr(settings, "drawable_writer", 'template') == 'structure' else CAPABILITIES

# Format limits (AssetValidator.cs), not tuning values.
MAX_VERTICES_PER_GEOMETRY = CAPABILITIES["maxVerticesPerGeometry"]
MIN_EXTENT_METERS, MAX_EXTENT_METERS = 0.02, 200.0
MAX_TEXTURE_SIZE = 2048
MAX_LOD = CAPABILITIES["lodSlots"] - 1
MAX_DRAW_DISTANCE_METERS = 1500.0
# Same tolerances as GltfImporter: |cos| between local axes that counts as sheared, sphere/capsule roundness, and how far
# collision may sit outside the model (AssetValidator.CollisionOverlapMarginMeters).
SHEAR_TOLERANCE = 1e-3
NON_UNIFORM_TOLERANCE = 0.01
CAPSULE_LENGTH_TOLERANCE_METERS = 1e-4
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


def collision_of(obj):
    """The collision shape an object declares (lower case), or None for a render object: the liberty_collision custom
    property ("none" or empty: render), else an _col name suffix (a mesh shape). Not inherited from parents."""
    if COLLISION_PROPERTY in obj.keys():
        shape = str(obj[COLLISION_PROPERTY]).strip().lower()
        return None if shape in ("", COLLISION_NONE) else shape
    return "mesh" if COLLISION_SUFFIX.search(obj.name) else None


def export_objects(context, settings):
    """Mesh objects the export will write: the asset collection (with nested collections) or the selection."""
    if settings.collection is not None:
        return [o for o in settings.collection.all_objects if o.type == 'MESH']
    return [o for o in context.selected_objects if o.type == 'MESH']


def lod_count(objects):
    """Number of LOD levels the render objects span (highest LOD + 1), as asset.json lodDistancesMeters needs."""
    levels = [lod_of(o) for o in objects if collision_of(o) is None]
    return max(levels) + 1 if levels else 0


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


def _world_vertices(obj, mesh):
    count = len(mesh.vertices)
    coordinates = np.empty(count * 3, dtype=np.float64)
    mesh.vertices.foreach_get("co", coordinates)
    local = coordinates.reshape(-1, 3)
    matrix = np.array(obj.matrix_world, dtype=np.float64)
    return local, local @ matrix[:3, :3].T + matrix[:3, 3]


def fit_primitive(obj, local, shape):
    """(problem, radius, non_uniform_ratio) for a box, sphere or capsule fitted as GltfImporter.FitPrimitive does: the
    object's local bounds scaled by its world transform, a capsule along local Z."""
    matrix = np.array(obj.matrix_world, dtype=np.float64)[:3, :3]
    scales = np.linalg.norm(matrix, axis=0)
    for axis in range(3):
        if scales[axis] < 1e-9:
            return "the object's scale is zero on its local %s axis" % "XYZ"[axis], 0.0, 1.0
    axes = matrix / scales
    for a in range(3):
        for b in range(a + 1, 3):
            if abs(float(np.dot(axes[:, a], axes[:, b]))) > SHEAR_TOLERANCE:
                return "the object's transform is sheared (local axes not perpendicular); apply the transform", 0.0, 1.0
    extent = (local.max(axis=0) - local.min(axis=0)) / 2 * scales
    if shape == "sphere":
        smallest = float(extent.min())
        return None, float(extent.max()), (float(extent.max()) / smallest if smallest > 1e-9 else math.inf)
    if shape == "capsule":
        radius = float(max(extent[0], extent[1]))
        smallest = float(min(extent[0], extent[1]))
        if 2 * extent[2] - 2 * radius < -CAPSULE_LENGTH_TOLERANCE_METERS:
            return ("the capsule is %.3f m long on its local Z axis, shorter than its diameter %.3f m (a capsule runs along the object's local Z)" %
                    (2 * extent[2], 2 * radius)), radius, 1.0
        return None, radius, (radius / smallest if smallest > 1e-9 else math.inf)
    if float(extent.min()) <= 0:
        return "the box has no size (flat object)", 0.0, 1.0
    return None, 0.0, 1.0


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
    render = [o for o in objects if collision_of(o) is None]
    collision = [o for o in objects if collision_of(o) is not None]
    if not render:
        add("error", "LBX001", "nothing to export: select mesh objects or set the asset collection" +
            (" (every object is tagged as collision)" if collision else ""))
        return _sorted(issues)

    unit = context.scene.unit_settings.scale_length
    if abs(unit - 1.0) > 1e-6:
        add("warning", "LBX004", "scene unit scale is %g; glTF writes Blender units as metres (set Unit Scale to 1)" % unit)

    caps = capabilities_for(settings)
    if settings.drawable_writer == 'structure' and settings.texture_mode != 'native':
        add("error", "LBX033", "the structure writer writes one texture per material into a new dictionary: set Texture mode to Native (LCC038)")

    depsgraph = context.evaluated_depsgraph_get()
    low = np.full(3, np.inf)
    high = np.full(3, -np.inf)
    triangles_per_lod = {}
    materials_per_lod = {}
    materials = set()
    for obj in render:
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
            _, world = _world_vertices(obj, mesh)
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
        if shader not in caps["shaders"]:
            add("warning", "LBX019", "material %s asks for shader '%s'; compiler %s writes %s, so LibertyContent will refuse to build it (LCC019)" %
                (material.name, shader, caps["version"], ", ".join(caps["shaders"])))
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

    limit = caps["maxMaterialsPerLod"]
    for lod, used in sorted(materials_per_lod.items()):
        if len(used) > limit:
            add("warning", "LBX009", "LOD %d uses %d materials (%d geometries); compiler %s writes %d per LOD, so LibertyContent will refuse to build it "
                                     "(LCC016). Merge materials or bake an atlas to build now" % (lod, len(used), len(used), caps["version"], limit))
    if triangles_per_lod and 0 not in triangles_per_lod:
        add("error", "LBX017", "no LOD 0 mesh")
    for lod in range(1, MAX_LOD + 1):
        if lod in triangles_per_lod and lod - 1 in triangles_per_lod and triangles_per_lod[lod] >= triangles_per_lod[lod - 1]:
            add("warning", "LBX018", "LOD %d has %d triangles, not fewer than LOD %d (%d)" % (lod, triangles_per_lod[lod], lod - 1, triangles_per_lod[lod - 1]))
    levels = sorted(triangles_per_lod)
    if levels and levels != list(range(len(levels))):
        add("warning", "LBX031", "LOD levels %s have a gap (use 0, 1, 2... in order)" % levels)

    shapes = _collision_checks(context, depsgraph, collision, low, high, add, caps)
    _asset_checks(settings, levels, shapes, add, caps)

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
        add("info", "LBX021", "%d object(s), triangles per LOD %s, %d collision shape(s), bounds (%.3f, %.3f, %.3f) .. (%.3f, %.3f, %.3f) m" %
            (len(render), dict(sorted(triangles_per_lod.items())), len(collision), low[0], low[1], low[2], high[0], high[1], high[2]))
    return _sorted(issues)


def _collision_checks(context, depsgraph, objects, low, high, add, caps):
    """LBX023-LBX028 for collision objects; returns the shapes found (valid or not)."""
    shapes = []
    for obj in objects:
        shape = collision_of(obj)
        shapes.append(shape)
        if shape not in COLLISION_SHAPES:
            add("error", "LBX023", "%s: collision shape '%s' is unknown (%s)" % (obj.name, shape, ", ".join(COLLISION_SHAPES)))
            continue
        if SURFACE_PROPERTY in obj.keys() and not SURFACE_PATTERN.match(str(obj[SURFACE_PROPERTY])):
            add("error", "LBX023", "%s: collision surface '%s' must be 1-31 letters, digits or _" % (obj.name, obj[SURFACE_PROPERTY]))
        if LOD_PROPERTY in obj.keys() or LOD_SUFFIX.search(obj.name):
            add("warning", "LBX026", "%s is collision and carries a LOD tag; collision has no LODs and the tag is ignored" % obj.name)
        evaluated = obj.evaluated_get(depsgraph)
        mesh = evaluated.to_mesh()
        try:
            if len(mesh.vertices) == 0 or (shape == "mesh" and len(mesh.polygons) == 0):
                add("error", "LBX024", "%s: collision %s has no %s" % (obj.name, shape, "faces" if shape == "mesh" else "vertices"))
                continue
            local, world = _world_vertices(obj, mesh)
            if shape != "mesh":
                problem, radius, ratio = fit_primitive(obj, local, shape)
                if problem:
                    add("error", "LBX024", "%s: collision %s: %s" % (obj.name, shape, problem))
                    continue
                if shape in ("sphere", "capsule") and ratio > 1 + NON_UNIFORM_TOLERANCE:
                    add("warning", "LBX025", "%s: collision %s is fitted to an object that is not round (extents differ %.2fx); the radius is the largest, %.3f m" %
                        (obj.name, shape, ratio, radius))
            if np.all(np.isfinite(low)) and (np.any(world.min(axis=0) > high + 0.01) or np.any(world.max(axis=0) < low - 0.01)):
                add("warning", "LBX027", "%s: collision lies outside the model's bounds" % obj.name)
        finally:
            evaluated.to_mesh_clear()
    unsupported = sorted(set(s for s in shapes if s in COLLISION_SHAPES and s not in caps["collisionShapes"]))
    if unsupported:
        add("warning", "LBX028", "%d collision shape(s) (%s): compiler %s writes %s, so LibertyContent will refuse to build the asset (LCC032)" %
            (len(shapes), ", ".join(unsupported), caps["version"],
             ("only " + ", ".join(caps["collisionShapes"])) if caps["collisionShapes"] else "no collision yet"))
    return shapes


def _asset_checks(settings, levels, shapes, add, caps):
    """Asset type (LBX029), world objects without collision (LBX032) and LOD distances (LBX030)."""
    if settings.asset_type not in caps["assetTypes"]:
        add("warning", "LBX029", "type '%s': compiler %s builds %s, so LibertyContent will refuse to build it (LCC033)" %
            (settings.asset_type, caps["version"], ", ".join(caps["assetTypes"])))
    if settings.asset_type == 'object' and not shapes:
        add("warning", "LBX032", "world object without collision: the player and vehicles would pass through it (tag collision objects)")
    if not settings.use_lod_distances or not levels:
        return
    distances = [float(d) for d in settings.lod_distances[:max(levels) + 1]]
    for lod, distance in enumerate(distances):
        if not 0 < distance <= MAX_DRAW_DISTANCE_METERS:
            add("error", "LBX030", "LOD %d distance %.1f m must be above 0 and at most %g m" % (lod, distance, MAX_DRAW_DISTANCE_METERS))
    for lod in range(1, len(distances)):
        if distances[lod] <= distances[lod - 1]:
            add("error", "LBX030", "LOD distances must ascend: LOD %d (%.1f m) is not farther than LOD %d (%.1f m)" % (lod, distances[lod], lod - 1, distances[lod - 1]))
    if distances[-1] > settings.draw_distance + 1e-6:
        add("error", "LBX030", "the last LOD distance (%.1f m) is beyond the draw distance (%.1f m)" % (distances[-1], settings.draw_distance))


def _sorted(issues):
    return sorted(issues, key=lambda issue: SEVERITY_ORDER.get(issue.severity, 3))


def has_errors(issues):
    return any(issue.severity == "error" for issue in issues)


def summary(issues):
    counts = {"error": 0, "warning": 0, "info": 0}
    for issue in issues:
        counts[issue.severity] = counts.get(issue.severity, 0) + 1
    return "%d error(s), %d warning(s)" % (counts["error"], counts["warning"])
