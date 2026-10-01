#!/usr/bin/env python3
"""T-050 independent, read-only PE audit and existing raydebug log decoder.

No process access, game launch, network, asset extraction, or runtime pointers.
Forensic VAs below pin evidence in one disk image; they are not runtime resolvers.
"""
import argparse
import hashlib
import json
import re
import struct
import subprocess
from pathlib import Path

EXE_SHA256 = "08759a5516f9837920ea504436236bbab89d0826a8e4d04ff106345177b5345d"
# Independent transcription of the owner's disk-image instructions, not mod code.
PINS = {
    "line_result_argument": (0xA53757, "ff75148d44241c50e81c51ceff"),
    "query_result_pointer": (0x76786F, "8b44241033d285c0"),
    "query_result_store": (0x767881, "894620"),
    "result_tail_init": (0x766FE3, "c7404000000000c7404400000000c7404cffff0000c64050006689785289784883c660"),
    "consumer_result_pointer": (0xD5FAF2, "8d8424ac00000050"),
    "consumer_line_call": (0xD5FB38, "e8733bcfff83c41c85c00f85bb000000"),
    "consumer_result_material": (0xD5FC17, "8b9424ec000000"),
    "consumer_material_lookup": (0xD5FC7E, "8b3568898b01528b068bce8b4014ffd0668378201e"),
    "lookup_low_byte": (0xCF2BC0, "8b110fb644240489442404ff6234"),
    "lookup_record": (0xCF2BF0, "0fb74118568b7424083bf08b4114730a8bd6c1e2042bd68d04905ec20400"),
    "append_record": (0xCF29F2, "0fb753188b7424788bc28bcac1e1042bc88b4314578d3c888d42016a6466894318"),
    "loader_append_call": (0xCF308B, "8b0383c40c8d8c2434020000518bcbff5030"),
    "loader_parse_call": (0xCF30AF, "8bc8e80ac50600"),
    "parser_fx_store": (0xD5F68E, "66894620"),
    "material_name_store": (0x6935C0, "8b442404894108c20400"),
}
RANGES = [(0xA536B0, 0xA5376E), (0x738880, 0x738937),
          (0x767840, 0x7678CA), (0x766F60, 0x767013),
          (0xD5FA98, 0xD5FD1D), (0xCF2940, 0xCF2961),
          (0xCF29E0, 0xCF2A56), (0xCF2BC0, 0xCF2C0E),
          (0xCF2F60, 0xCF30DC), (0xD5F5C0, 0xD5F75B)]


class PE:
    def __init__(self, path):
        self.data = Path(path).read_bytes()
        pe = struct.unpack_from("<I", self.data, 60)[0]
        if self.data[:2] != b"MZ" or self.data[pe:pe+4] != b"PE\0\0":
            raise ValueError("not a PE image")
        machine, count = struct.unpack_from("<HH", self.data, pe+4)
        optional_size = struct.unpack_from("<H", self.data, pe+20)[0]
        if machine != 0x14C or struct.unpack_from("<H", self.data, pe+24)[0] != 0x10B:
            raise ValueError("requires x86 PE32")
        self.base = struct.unpack_from("<I", self.data, pe+52)[0]
        self.sections = []
        for i in range(count):
            off = pe+24+optional_size+40*i
            name = self.data[off:off+8].split(b"\0")[0].decode("ascii")
            _, rva, size, raw = struct.unpack_from("<IIII", self.data, off+8)
            self.sections.append((name, self.base+rva, size, raw))

    def offset(self, va, size=1):
        for _, start, length, raw in self.sections:
            if start <= va and va+size <= start+length:
                return raw+va-start
        raise ValueError("VA outside file-backed section: " + hex(va))

    def read(self, va, size):
        off = self.offset(va, size)
        return self.data[off:off+size]


def audit(args):
    pe = PE(args.exe)
    digest = hashlib.sha256(pe.data).hexdigest()
    if digest != EXE_SHA256:
        raise ValueError("unsupported executable SHA-256 " + digest)
    print(json.dumps({"label": "VERIFIED OFFLINE", "sha256": digest,
                      "image_base": hex(pe.base), "sections": pe.sections}))
    for name, (va, expected) in PINS.items():
        value = bytes.fromhex(expected)
        if pe.read(va, len(value)) != value:
            raise ValueError("instruction evidence mismatch: " + name)
        # Whole-file exact matches are forensic pattern candidates; not runtime signatures.
        matches = []
        for section, start, size, raw in pe.sections:
            if section != ".text":
                continue
            for m in re.finditer(re.escape(value), pe.data[raw:raw+size]):
                matches.append(hex(start+m.start()))
        print(json.dumps({"label": "VERIFIED OFFLINE", "evidence": name,
                          "va": hex(va), "bytes": value.hex(" "),
                          "text_match_count": len(matches), "text_matches_first8": matches[:8]}))
    table = struct.unpack("<14I", pe.read(0xEDE9C4, 56))
    for slot, expected in [(0x14, 0xCF2BC0), (0x30, 0xCF29E0), (0x34, 0xCF2BF0)]:
        if table[slot//4] != expected:
            raise ValueError("material manager vtable mismatch")
    print(json.dumps({"label": "VERIFIED OFFLINE", "vtable": "0xEDE9C4",
                      "slots": {hex(i*4): hex(v) for i, v in enumerate(table)}}))
    if args.objdump:
        for start, end in RANGES:
            subprocess.run([str(args.objdump), "-d", "--x86-asm-syntax=intel",
                            "--start-address="+hex(start), "--stop-address="+hex(end),
                            str(args.exe)], check=True)


def materials(path):
    rows = []
    version = None
    for line_number, line in enumerate(path.read_text(encoding="utf-8-sig").splitlines(), 1):
        cells = line.split("#", 1)[0].split()
        if not cells:
            continue
        if version is None:
            version = cells[0]
            if cells != ["2.00"]:
                raise ValueError("requires materials.dat 2.00")
            continue
        # The trailing editorial name is ignored by the game's sscanf format;
        # e.g. WINDSCREEN_WEAK repeats WINDSCREEN there. Only column 0 is the key.
        if len(cells) != 17:
            raise ValueError("unexpected material row at line " + str(line_number))
        # Validate the columns that distinguish a data row from headings/comments.
        for cell in cells[3:13]:
            float(cell)
        for cell in cells[13:16]:
            if cell not in ("0", "1"):
                raise ValueError("unexpected flag at line " + str(line_number))
        rows.append({"index": len(rows), "line": line_number,
                     "name": cells[0], "fx_group": cells[1]})
    if not rows or len(rows) > 256:
        raise ValueError("empty or incompatible material table")
    return rows


def decode(args):
    rows = materials(args.materials)
    print(json.dumps({"label": "VERIFIED OFFLINE", "materials": str(args.materials),
                      "sha256": hashlib.sha256(args.materials.read_bytes()).hexdigest(),
                      "rows": len(rows), "mapping": "zero-based data row; comments/version excluded",
                      "runtime_table_equivalence": "UNKNOWN until effective loaded file is confirmed"}))
    if not args.log:
        for row in rows:
            print(json.dumps(row))
        return
    found = 0
    for line_number, line in enumerate(args.log.read_text(encoding="utf-8-sig", errors="replace").splitlines(), 1):
        if "raydebug " not in line or " raw=" not in line:
            continue
        tokens = line.split(" raw=", 1)[1].split()
        if len(tokens) != 24 or any(not re.fullmatch(r"[0-9a-fA-F]{8}", t) for t in tokens):
            raise ValueError("malformed 24-word raydebug at log line " + str(line_number))
        found += 1
        words = [int(t, 16) for t in tokens]
        status_match = re.search(r"\bstatus=(\w+)", line)
        status = status_match[1] if status_match else "UNKNOWN"
        packed = words[18]
        index = packed & 255
        hit = status == "Hit" and words[0] != 0
        row = rows[index] if hit and index < len(rows) else None
        print(json.dumps({"log_line": line_number, "status": status,
                          "result_field": "+0x48", "packed": f"0x{packed:08X}",
                          "index": index if hit else None, "material": row,
                          "classification": "PLAUSIBLE file mapping" if row else "UNKNOWN",
                          "tail_words": {f"+0x{i*4:02X}": f"0x{words[i]:08X}" for i in range(12, 24)},
                          "capture": line.split(" raw=", 1)[0]}))
    if not found:
        raise ValueError("no raydebug 24-word records found")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    p = commands.add_parser("audit", help="verify disk-image instruction pins; optional disassembly")
    p.add_argument("--exe", type=Path, required=True)
    p.add_argument("--objdump", type=Path)
    p = commands.add_parser("decode", help="list file rows, or annotate existing raydebug records")
    p.add_argument("--materials", type=Path, required=True)
    p.add_argument("--log", type=Path)
    args = parser.parse_args()
    try:
        (audit if args.command == "audit" else decode)(args)
    except (ValueError, OSError, struct.error, subprocess.CalledProcessError) as error:
        parser.exit(1, "ERROR: " + str(error) + "\n")


if __name__ == "__main__":
    main()
