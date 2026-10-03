#!/usr/bin/env python3
"""Hash the current framework native/managed/SDK contract independently of paths."""
import argparse
import hashlib
import json
import pathlib
import subprocess


def fingerprint(root):
    root=pathlib.Path(root).resolve(strict=True)
    receipt=root/'workspace.json'
    if receipt.is_file():
        names=json.loads(receipt.read_text())['repositories']['framework']['files']
    else:
        names=subprocess.check_output(['git','-C',str(root),'ls-files','--cached','--others','--exclude-standard'],text=True).splitlines()
    selected=sorted(set(name for name in names if name.startswith(('src/LibertyFramework/','sdk/','native/'))
                        and pathlib.PurePosixPath(name).suffix in ['.cs','.h','.hpp','.cpp'] and (root/name).is_file()))
    lines=[]
    for name in selected:
        path=root/name
        if not path.resolve().is_relative_to(root):
            raise ValueError('Unsafe framework input')
        # Source contract is portable across Git CRLF/LF working-tree conversion;
        # package provenance still records exact raw file hashes separately.
        lines.append(name+'|'+hashlib.sha256(path.read_bytes().replace(b'\r\n',b'\n')).hexdigest())
    if not selected:
        raise ValueError('Empty framework contract')
    return {'sourceSha256':hashlib.sha256(('\n'.join(lines)+'\n').encode()).hexdigest(),'sourceFiles':len(selected)}


if __name__=='__main__':
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root',required=True)
    print(json.dumps(fingerprint(parser.parse_args().root)))
