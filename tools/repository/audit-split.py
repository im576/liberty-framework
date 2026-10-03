#!/usr/bin/env python3
"""Validate preserved mod tuning/artwork and the maintained source ownership boundary."""
import argparse
import hashlib
import json
import pathlib
import subprocess

parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--framework',type=pathlib.Path,required=True)
parser.add_argument('--mod',type=pathlib.Path,required=True)
args=parser.parse_args()
framework=args.framework.resolve(); mod=args.mod.resolve()
manifest=json.loads((mod/'docs/migration/extraction.json').read_text())
dedup={x['removed']:x['canonical'] for x in json.loads((mod/'docs/migration/art-deduplication.json').read_text())}
passed=0

for entry in manifest['files']:
    original=entry['original']
    if original.startswith('config/'):
        before=subprocess.check_output(['git','-C',str(framework),'show',manifest['sourceCommit']+':'+original])
        after=(mod/entry['destination']).read_bytes()
        if json.loads(before)!=json.loads(after):
            raise SystemExit('Changed gameplay tuning: '+original)
        passed+=1
    elif original.endswith('.png'):
        path=mod/dedup.get(entry['destination'],entry['destination'])
        if hashlib.sha256(path.read_bytes()).hexdigest()!=entry['sha256']:
            raise SystemExit('Changed or missing artwork: '+original)
        passed+=1

before=subprocess.check_output(['git','-C',str(framework),'show',manifest['sourceCommit']+':config/engine.json'])
if json.loads(before)!=json.loads((mod/'config/runtime/engine.json').read_bytes()):
    raise SystemExit('Changed preview runtime profile')
passed+=1
for directory in ['Arsenal','Atmosphere','CombatEffects','Gunplay','Weapons','Hud','WeaponProbe']:
    if list((framework/'src/LibertyFramework'/directory).glob('**/*.cs')):
        raise SystemExit('Gameplay source leaked into framework: '+directory)
    passed+=1
if (mod/'sdk').exists() or (mod/'native').exists() or (mod/'src/LibertyFramework/Engine').exists():
    raise SystemExit('Framework implementation vendored into mod')
passed+=1
print('split-audit: passed='+str(passed)+' failed=0 (tuning/artwork/profile/source ownership)')
