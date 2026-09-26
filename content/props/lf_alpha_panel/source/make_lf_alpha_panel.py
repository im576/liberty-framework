# Writes lf_alpha_panel.gltf/.bin/_basecolor.png one folder up: the native-texture-mode DXT5 test asset.
# A standing panel, 0.8 m wide, 0.8 m tall and 0.04 m thick, resting on Z=0 (GTA IV space), with a 128x128 RGBA texture:
# an opaque dark frame, a checker of alpha 64 / 192 cells and an opaque diagonal bar, so translucency, alpha steps and
# the colour channels are each visible in a screenshot. alphaMode BLEND makes the compiler write DXT5.
# Run: python3 make_lf_alpha_panel.py (standard library only).

import json
import math
import os
import struct
import zlib

NAME = "lf_alpha_panel"
SIZE = 128
WIDTH, HEIGHT, DEPTH = 0.8, 0.8, 0.04
FOLDER = os.path.join(os.path.dirname(os.path.abspath(__file__)), os.pardir)


def texel(x, y):
    frame = 8
    if x < frame or y < frame or x >= SIZE - frame or y >= SIZE - frame:
        return (40, 40, 48, 255)
    if abs(x - y) < 6:
        return (230, 60, 40, 255)
    cell = ((x // 16) + (y // 16)) % 2
    return (70, 150, 230, 64) if cell == 0 else (240, 240, 240, 192)


def png(path):
    rows = b"".join(b"\x00" + bytes(c for x in range(SIZE) for c in texel(x, y)) for y in range(SIZE))

    def chunk(kind, data):
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", zlib.crc32(kind + data) & 0xFFFFFFFF)

    header = struct.pack(">IIBBBBB", SIZE, SIZE, 8, 6, 0, 0, 0)
    with open(path, "wb") as stream:
        stream.write(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", header) + chunk(b"IDAT", zlib.compress(rows, 9)) + chunk(b"IEND", b""))


def box():
    """Faces of the panel in glTF space (Y up; GTA IV (x, y, z) = glTF (x, -z, y)): positions, normals, uvs, indices."""
    x0, x1, y0, y1, z0, z1 = -WIDTH / 2, WIDTH / 2, 0.0, HEIGHT, -DEPTH / 2, DEPTH / 2
    faces = [
        ((0, 0, 1), [(x0, y0, z1), (x1, y0, z1), (x1, y1, z1), (x0, y1, z1)]),
        ((0, 0, -1), [(x1, y0, z0), (x0, y0, z0), (x0, y1, z0), (x1, y1, z0)]),
        ((1, 0, 0), [(x1, y0, z1), (x1, y0, z0), (x1, y1, z0), (x1, y1, z1)]),
        ((-1, 0, 0), [(x0, y0, z0), (x0, y0, z1), (x0, y1, z1), (x0, y1, z0)]),
        ((0, 1, 0), [(x0, y1, z1), (x1, y1, z1), (x1, y1, z0), (x0, y1, z0)]),
        ((0, -1, 0), [(x0, y0, z0), (x1, y0, z0), (x1, y0, z1), (x0, y0, z1)]),
    ]
    positions, normals, uvs, indices = [], [], [], []
    for normal, corners in faces:
        base = len(positions)
        positions.extend(corners)
        normals.extend([normal] * 4)
        # glTF UV origin is the top-left of the image: v = 1 at the bottom edge of each face.
        uvs.extend([(0, 1), (1, 1), (1, 0), (0, 0)])
        indices.extend([base, base + 1, base + 2, base, base + 2, base + 3])
    return positions, normals, uvs, indices


def main():
    positions, normals, uvs, indices = box()
    blob = b"".join(struct.pack("<3f", *p) for p in positions)
    blob += b"".join(struct.pack("<3f", *n) for n in normals)
    blob += b"".join(struct.pack("<2f", *uv) for uv in uvs)
    blob += b"".join(struct.pack("<H", i) for i in indices)
    blob += b"\x00" * (-len(blob) % 4)
    count = len(positions)
    views = [(0, count * 12), (count * 12, count * 12), (count * 24, count * 8), (count * 32, len(indices) * 2)]
    document = {
        "asset": {"version": "2.0", "generator": "Liberty Framework content/props/lf_alpha_panel/source/make_lf_alpha_panel.py"},
        "scene": 0,
        "scenes": [{"name": "Scene", "nodes": [0], "extras": {"liberty_asset": NAME}}],
        "nodes": [{"name": NAME + "_lod0", "mesh": 0}],
        "meshes": [{"name": NAME, "primitives": [{"attributes": {"POSITION": 0, "NORMAL": 1, "TEXCOORD_0": 2}, "indices": 3, "material": 0}]}],
        "materials": [{"name": "alpha_panel", "alphaMode": "BLEND",
                       "pbrMetallicRoughness": {"baseColorTexture": {"index": 0}, "metallicFactor": 0, "roughnessFactor": 0.5},
                       "extras": {"liberty_shader": "gta_default"}}],
        "textures": [{"source": 0}],
        "images": [{"uri": NAME + "_basecolor.png", "name": NAME + "_basecolor"}],
        "accessors": [
            {"bufferView": 0, "componentType": 5126, "count": count, "type": "VEC3",
             "min": [min(p[i] for p in positions) for i in range(3)], "max": [max(p[i] for p in positions) for i in range(3)]},
            {"bufferView": 1, "componentType": 5126, "count": count, "type": "VEC3"},
            {"bufferView": 2, "componentType": 5126, "count": count, "type": "VEC2"},
            {"bufferView": 3, "componentType": 5123, "count": len(indices), "type": "SCALAR"},
        ],
        "bufferViews": [{"buffer": 0, "byteOffset": offset, "byteLength": length} for offset, length in views],
        "buffers": [{"uri": NAME + ".bin", "byteLength": len(blob)}],
    }
    with open(os.path.join(FOLDER, NAME + ".bin"), "wb") as stream:
        stream.write(blob)
    with open(os.path.join(FOLDER, NAME + ".gltf"), "w", encoding="utf-8", newline="\n") as stream:
        json.dump(document, stream, indent=1)
        stream.write("\n")
    png(os.path.join(FOLDER, NAME + "_basecolor.png"))
    assert math.isclose(max(p[1] for p in positions), HEIGHT)


if __name__ == "__main__":
    main()
