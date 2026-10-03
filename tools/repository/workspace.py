#!/usr/bin/env python3
"""Assemble an isolated two-repository workspace for legacy integration tools.

Maintained source stays in its owning repository. Old-path source mirrors exist
only in this generated directory, for historical verifier fixtures. The engine
build excludes those mirrors; Liberty+ builds into a separate discovered DLL.
"""
import argparse
import hashlib
import json
import pathlib
import shutil
import subprocess
import uuid


def git(root, *args):
    return subprocess.check_output(['git', '-C', str(root), *args], text=True).strip()


def inventory(root):
    # Include current edits and non-ignored additions; never ignored game files/caches.
    files = sorted(set(git(root, 'ls-files', '--cached', '--others', '--exclude-standard').splitlines()))
    result = {}
    for name in files:
        path = root / name
        if not path.exists():
            continue  # tracked deletion
        if path.is_symlink() or not path.resolve().is_relative_to(root):
            raise ValueError('Unsafe source path: ' + str(path))
        result[name] = hashlib.sha256(path.read_bytes()).hexdigest()
    return result


def create(framework, mod):
    framework, mod = framework.resolve(strict=True), mod.resolve(strict=True)
    if framework == mod:
        raise ValueError('Framework and mod must be separate repositories')
    files = {'framework': inventory(framework), 'mod': inventory(mod)}
    metadata = {'schemaVersion': 1, 'repositories': {}}
    for role, root in [('framework', framework), ('mod', mod)]:
        metadata['repositories'][role] = {'repo': str(root), 'commit': git(root, 'rev-parse', 'HEAD'),
            'dirty': bool(git(root, 'status', '--porcelain')), 'files': files[role]}
    # UUID avoids modifying a workspace currently used by another check or package build.
    work = mod / 'results-local' / 'integration' / uuid.uuid4().hex[:12]
    work.mkdir(parents=True)
    owners = {}

    def copy(root, name, destination, override=False):
        target = work / destination
        if not target.resolve().is_relative_to(work.resolve()):
            raise ValueError('Unsafe destination: ' + destination)
        if destination in owners and not override:
            raise ValueError('Ownership collision: ' + destination)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(root / name, target)
        owners[destination] = {'repo': str(root), 'path': name}

    for name in files['framework']:
        copy(framework, name, name)
    for name in files['mod']:
        if name in ['README.md', 'AGENTS.md', 'CLAUDE.md', '.gitignore', '.gitattributes', 'docs/PROJECT_STATE.md', 'docs/tasks/README.md']:
            continue
        if name.startswith('tools/integration/legacy/'):
            copy(mod, name, name.replace('tools/integration/legacy/', 'tools/', 1))
        elif name == 'third_party/README.md':
            copy(mod, name, 'third_party/LIBERTY_PLUS.md')
        elif name == 'tools/build.ps1':
            copy(mod, name, 'tools/build-libertyplus.ps1')
        elif name.startswith('src/LibertyPlus/'):
            copy(mod, name, name)
            copy(mod, name, name.replace('src/LibertyPlus/', 'src/LibertyFramework/', 1))
        else:
            copy(mod, name, name)

    # Explicit integration overrides; all other duplicate ownership is an error.
    copy(framework, 'tools/build.ps1', 'tools/build-framework.ps1')
    copy(mod, 'tools/integration/build.ps1', 'tools/build.ps1', override=True)
    copy(mod, 'tools/integration/verify.ps1', 'tools/verify.ps1', override=True)
    copy(mod, 'tools/integration/VerifyProgram.cs', 'tools/verify/Program.cs', override=True)
    copy(mod, 'config/runtime/engine.json', 'config/engine.json', override=True)
    # Machine toolchain locations are local and never part of the source inventory.
    local = framework / 'tools/toolchains.local.json'
    if local.is_file():
        shutil.copyfile(local, work / 'tools/toolchains.local.json')
    metadata['workspace'] = str(work)
    metadata['ownership'] = owners
    (work / 'workspace.json').write_text(json.dumps(metadata, indent=2)+'\n', encoding='utf-8')
    # A disposable identity prevents legacy verification reading the parent mod's
    # HEAD. No remote; source maintenance still happens only in the two real repos.
    subprocess.run(['git','init','-q','-b','integration',str(work)],check=True)
    subprocess.run(['git','-C',str(work),'config','user.name','Liberty integration snapshot'],check=True)
    subprocess.run(['git','-C',str(work),'config','user.email','snapshot@example.invalid'],check=True)
    subprocess.run(['git','-C',str(work),'add','.'],check=True,stdout=subprocess.DEVNULL,stderr=subprocess.PIPE)
    subprocess.run(['git','-C',str(work),'-c','user.name=Liberty integration snapshot',
                    '-c','user.email=snapshot@example.invalid','commit','-qm','Integration input snapshot'],check=True)
    return work


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--framework', type=pathlib.Path, required=True)
    parser.add_argument('--mod', type=pathlib.Path, required=True)
    args = parser.parse_args()
    print(create(args.framework, args.mod))


if __name__ == '__main__':
    main()
