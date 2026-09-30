"""Independent read-only CE signature/data probe. Writes metadata only to stdout.
Requires locally available pefile and capstone; no network, process access or assets exported.
Signatures are research facts cited to local FusionFix source in Frontend.md, not copied code.
"""
import argparse
import hashlib
import json
import re
import struct
import xml.etree.ElementTree as ET
from pathlib import Path
import capstone
import pefile

PATTERNS = {
    'menu_active': '80 3D ? ? ? ? ? 74 4B E8 ? ? ? ? 84 C0',
    'menu_tab_a': 'A1 ? ? ? ? 83 F8 08 74 17',
    'menu_tab_b': 'A1 ? ? ? ? 83 F8 08 74 0C',
    'frontend_read_a': '51 56 57 64 8B 3D',
    'frontend_read_b': '51 53 56 BE ? ? ? ? 33 DB',
    'frontend_selector_derived': '55 8B EC 83 E4 F8 81 EC 8C 02 00 00 8B 0D ? ? ? ? 53 0F B7 41 04 56',
    'frontend_xml_parser_derived': '51 53 55 56 57 8B 3D ? ? ? ? 6A 01 6A 00 68 ? ? ? ? 51 B9',
    'map_crosshair_entry_derived': '55 8B EC 83 E4 F8 83 EC 58 A1 ? ? ? ? 33 C4 89 44 24 54 56 57',
    'map_crosshair_a': 'F3 0F 10 15 ? ? ? ? F3 0F 10 5C 24 ? 0F B6 C0',
    'map_crosshair_b': 'F3 0F 10 1D ? ? ? ? F3 0F 10 54 24 ? 0F B6 C8',
    'game_process_call': 'E8 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? B9 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? B9',
}

def file_info(p):
    b = p.read_bytes()
    return {'path': str(p), 'bytes': len(b), 'sha256': hashlib.sha256(b).hexdigest()}

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--game', type=Path, required=True)
    a = ap.parse_args()
    p = a.game / 'GTAIV.exe'
    b = p.read_bytes()
    pe = pefile.PE(data=b)
    base = pe.OPTIONAL_HEADER.ImageBase
    md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_32)
    md.detail = True
    out = {'exe': file_info(p), 'image_base': hex(base), 'patterns': {}, 'strings': {}, 'data': []}
    for name, pat in PATTERNS.items():
        rx = b''.join(b'.' if x == '?' else re.escape(bytes([int(x, 16)])) for x in pat.split())
        hits = []
        for sec in pe.sections:
            if not sec.Characteristics & 0x20000000:
                continue
            raw = sec.get_data()
            for m in re.finditer(rx, raw, re.DOTALL):
                rva = sec.VirtualAddress + m.start()
                va = base + rva
                item = {'rva': hex(rva), 'va_for_navigation_only': hex(va),
                        'file_offset': hex(sec.PointerToRawData + m.start()), 'section': sec.Name.rstrip(b'\0').decode(),
                        'instructions': []}
                for ins in md.disasm(raw[m.start():m.start()+96], va):
                    item['instructions'].append({'va': hex(ins.address), 'bytes': ins.bytes.hex(' '),
                                                 'asm': ins.mnemonic + ' ' + ins.op_str})
                    if len(item['instructions']) == 16:
                        break
                if name == 'menu_active':
                    item['absolute_operand_navigation_only'] = hex(struct.unpack_from('<I', raw, m.start()+2)[0])
                if name.startswith('menu_tab'):
                    item['absolute_operand_navigation_only'] = hex(struct.unpack_from('<I', raw, m.start()+1)[0])
                if name == 'game_process_call':
                    item['call_target_navigation_only'] = hex(va+5+struct.unpack_from('<i',raw,m.start()+1)[0])
                hits.append(item)
        out['patterns'][name] = {'pattern': pat, 'count': len(hits), 'hits': hits}
    for s in ['frontend_menus.xml', 'frontend_pc.dat', 'frontend.dat', 'sMenuDisplayValue', 'sMenu',
              'sMenuScreen', 'MENU_MAP', 'PREF_SFXVOLUME', 'MAP_map_scroll_zoom']:
        hits = []
        for m in re.finditer(re.escape(s.encode()+b'\0'), b):
            va = base + pe.get_rva_from_offset(m.start())
            refs = []
            for sec in pe.sections:
                for r in re.finditer(re.escape(struct.pack('<I',va)), sec.get_data()):
                    refs.append(hex(base+sec.VirtualAddress+r.start()))
            hits.append({'va_navigation_only': hex(va), 'raw_pointer_occurrences_not_proven_xrefs': refs})
        out['strings'][s] = hits
    for directory in ['common/data', 'update/common/data']:
        for p in sorted((a.game / directory).glob('frontend*')):
            info = file_info(p)
            if p.suffix == '.xml':
                root = ET.parse(p).getroot()
                info['root'] = root.tag
                info['child_sections'] = [x.tag for x in root]
                info['tag_counts'] = {t: len(root.findall('.//'+t)) for t in sorted({x.tag for x in root.iter()})}
                info['map_elements'] = [{'tag': x.tag, 'attributes': x.attrib} for x in root.iter()
                                        if any('MAP' in v for v in x.attrib.values())][:35]
            else:
                info['map_rows'] = [x for x in p.read_text(errors='replace').splitlines() if x.startswith('MAP_')]
            out['data'].append(info)
    print(json.dumps(out, indent=2))

if __name__ == '__main__':
    main()
