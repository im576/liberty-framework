#!/usr/bin/env python3
"""Inspect a local mod ZIP without extracting or executing it; emit source/provenance metadata."""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import zipfile


def inspect(path):
    with path.open("rb") as stream:
        digest = hashlib.file_digest(stream, "sha256").hexdigest()
    with zipfile.ZipFile(path) as archive:
        entries = [entry for entry in archive.infolist() if not entry.is_dir()]
        sources, binaries, licenses, unsafe = [], [], [], []
        for entry in entries:
            name = entry.filename.replace("\\", "/")
            parsed = PurePosixPath(name)
            if parsed.is_absolute() or ".." in parsed.parts or ":" in name:
                unsafe.append(entry.filename)
            extension = parsed.suffix.lower()
            if extension in (".cs", ".cpp", ".c", ".h", ".hpp"):
                sources.append({"path": name, "bytes": entry.file_size})
            if extension in (".asi", ".dll", ".exe"):
                binaries.append({"path": name, "bytes": entry.file_size})
            if parsed.name.lower().startswith(("license", "licence", "copying")):
                # Keep notices as evidence, but do not infer blanket rights for every bundled asset.
                notice = archive.read(entry).decode("utf-8", errors="replace") if entry.file_size <= 131072 else "[notice too large]"
                licenses.append({"path": name, "text": notice})
        return {"archive": path.name, "sha256": digest, "bytes": path.stat().st_size,
                "files": len(entries), "uncompressedBytes": sum(e.file_size for e in entries),
                "zipComment": archive.comment.decode("utf-8", errors="replace"),
                "sourceFiles": sources, "binaries": binaries, "licenseNotices": licenses,
                "unsafeEntryPaths": unsafe,
                "scope": "Local archive inventory only; no execution, extraction, binary/source equivalence or runtime compatibility established."}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("archive", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = inspect(args.archive)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(f"mod: {result['archive']} source={len(result['sourceFiles'])} binaries={len(result['binaries'])} "
          f"notices={len(result['licenseNotices'])} unsafePaths={len(result['unsafeEntryPaths'])} sha256={result['sha256']}")
    return 1 if result["unsafeEntryPaths"] else 0


if __name__ == "__main__":
    raise SystemExit(main())
