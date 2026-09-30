"""Independent read-only audio inventory and empirical bank-layout probe.
No decryption keys, copied tool source, audio export, writer or network access.
Field names deliberately describe observations, not an asserted IVAUD specification.
"""
import argparse
import collections
import hashlib
import json
import re
import struct
import xml.etree.ElementTree as ET
import zlib
from pathlib import Path

def info(p):
    b = p.read_bytes()
    return {'path': str(p), 'bytes': len(b), 'sha256': hashlib.sha256(b).hexdigest()}

def inspect(b):
    first, header_end, count, flags, data_start = struct.unpack_from('<QQIII', b)
    assert first == 28 and header_end == 28+48*count
    assert header_end <= data_start <= len(b)
    table = [struct.unpack_from('<QII', b, 28+16*i) for i in range(count)]
    meta_start = 28+16*count
    rows = [struct.unpack_from('<QIIIIHBBI', b, meta_start+32*i) for i in range(count)]
    assert sorted(x[0] for x in table) == list(range(0,32*count,32))
    assert all(x[2] == 32 for x in table)
    assert all(data_start+r[0]+r[2] <= len(b) for r in rows)
    hashes = {off//32: h for off,h,length in table}
    facts = {'first_u64': first, 'header_end_u64': header_end, 'count_u32': count,
             'flags_u32': flags, 'data_start_u32': data_start, 'metadata_start': meta_start,
             'descriptor_metadata_offsets_are_permutation': True, 'all_descriptor_lengths_32': True,
             'all_payload_ranges_in_bounds': True, 'distinct_descriptor_hashes': len(set(x[1] for x in table)),
             'record_word_8_equals_descriptor_hash_count': sum(r[1] == hashes[i] for i,r in enumerate(rows)),
             'rate_like_u16_distribution': dict(collections.Counter(r[5] for r in rows)),
             'final_u32_distribution': dict(collections.Counter(r[8] for r in rows)),
             'bytes_equal_twice_sample_like_count': sum(r[2] == 2*r[3] for r in rows),
             'metadata_word_8_crc32_matches': sum(zlib.crc32(b[data_start+r[0]:data_start+r[0]+r[2]]) == r[1] for r in rows),
             'last_payload_end': max(data_start+r[0]+r[2] for r in rows),
             'size_exceptions': [{'record_index': i, 'descriptor_hash': f'{hashes[i]:08x}', 'raw_fields': list(r)}
                                 for i,r in enumerate(rows) if r[2] != 2*r[3]]}
    return facts, rows, hashes

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--game', required=True, type=Path)
    ap.add_argument('--sources', required=True, type=Path)
    a = ap.parse_args()
    audio = a.game/'pc/audio'
    out = {'scope': 'VERIFIED OFFLINE observations; no writer or runtime test',
           'sfx_inventory': [{'name': p.name, 'bytes': p.stat().st_size} for p in sorted((audio/'sfx').glob('*.rpf'))],
           'config_files': [info(p) for p in sorted((audio/'config').iterdir()) if p.is_file()]}
    root = ET.parse(audio/'config/waveslots.xml').getroot()
    out['selected_wave_slots'] = [{x.tag: (x.text.strip() if x.text else x.attrib.get('value')) for x in e}
                                  for e in root if e.findtext('Name') in ['WEAPONS','FRONTEND_MENU','FRONTEND_GAME']]
    out['rpf_registration'] = [{x.tag:x.text for x in e} for e in ET.parse(audio/'config/rpf.xml').getroot()
                               if e.findtext('Name') == 'resident']
    p = audio/'sfx/resident.rpf'
    resident = p.read_bytes()
    out['resident_rpf'] = info(p)
    out['resident_header_u32'] = list(struct.unpack_from('<5I',resident))
    override = a.game/'update/pc/audio/sfx/resident.rpf/RESIDENT/WEAPONS'
    b = override.read_bytes()
    out['installed_override'] = info(override)
    facts, rows, hashes = inspect(b)
    out['override_observed_layout'] = facts
    candidates = []
    for at in range(4096,len(resident)-28,2048):
        first,end,count,flags,start = struct.unpack_from('<QQIII',resident,at)
        if first == 28 and 0<count<4000 and end == 28+48*count and end <= start < 300000:
            candidates.append({'resident_offset': hex(at),'count':count,'header_end':end,'flags':flags,'data_start':start})
    out['uniform_32byte_bank_header_candidates'] = candidates
    matches = [int(x['resident_offset'],16) for x in candidates
               if resident[int(x['resident_offset'],16):int(x['resident_offset'],16)+facts['metadata_start']] == b[:facts['metadata_start']]]
    out['unique_matching_descriptor_table_offsets'] = [hex(x) for x in matches]
    assert len(matches) == 1
    at = matches[0]
    next_at = min(int(x['resident_offset'],16) for x in candidates if int(x['resident_offset'],16)>at)
    original = resident[at:next_at]
    old_facts, old_rows, old_hashes = inspect(original)
    out['base_candidate_observed_layout'] = old_facts
    out['base_candidate_extent_bytes'] = len(original)
    out['base_candidate_extent_sha256'] = hashlib.sha256(original).hexdigest()
    out['comparison'] = {'identical_header_and_descriptors': original[:facts['metadata_start']] == b[:facts['metadata_start']],
                          'changed_records': [], 'metadata_word_8_changed_count': sum(x[1]!=y[1] for x,y in zip(rows,old_rows)),
                          'size_unchanged_count': sum(x[2]==y[2] for x,y in zip(rows,old_rows))}
    start = facts['data_start_u32']
    for i,(new,old) in enumerate(zip(rows,old_rows)):
        if b[start+new[0]:start+new[0]+new[2]] != original[start+old[0]:start+old[0]+old[2]]:
            out['comparison']['changed_records'].append({'index':i,'descriptor_hash':f'{hashes[i]:08x}',
                 'base_bytes':old[2],'override_bytes':new[2],'base_rate_like':old[5],'override_rate_like':new[5]})
    sounds = (audio/'config/sounds.dat15').read_bytes()
    out['sounds_observed_identifiers'] = []
    for identifier in [b'RESIDENT\\WEAPONS', b'AUD_EVENT_HANDGUN_RELOAD_INSERT_CLIP',
                       b'AUD_EVENT_AK47_RELOAD_COCK_PULL', b'AUD_EVENT_DEAGLE_RELOAD_INSERT_CLIP',
                       b'DISTANT_GUNSHOTS_GUN_PISTOL']:
        out['sounds_observed_identifiers'].append({'identifier': identifier.decode(),
                      'file_offsets': [hex(m.start()) for m in re.finditer(re.escape(identifier), sounds)]})
    for rel in ['source/rpfloader.ixx','source/settings.ixx','source/fixes.ixx','source/comvars.ixx','source/dllmain.cpp','LICENSE']:
        out.setdefault('reference_source_files',[]).append(info(a.sources/rel))
    ini = a.game/'plugins/GTAIV.EFLC.FusionFix.ini'
    out['installed_fusionfix_ini'] = info(ini)
    out['LoadRPF_lines'] = [line for line in ini.read_text(errors='replace').splitlines() if 'LoadRPF' in line]
    print(json.dumps(out,indent=2))

if __name__ == '__main__':
    main()
