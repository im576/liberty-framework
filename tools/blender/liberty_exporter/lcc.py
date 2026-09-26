# Runs LibertyContent.exe (tools/content) and reads its output: issue lines ("ERROR LCC016: ...") and report.json.

import json
import os
import re
import subprocess

import bpy

from .checks import Issue

ISSUE_LINE = re.compile(r"^\s*(ERROR|WARNING|INFO) ([^:\s]+): (.*)$")
READBACK_LINE = re.compile(r"^\s*READBACK (.*)$")


def compiler_path(preferences):
    """The configured LibertyContent.exe, else <content folder>/../tools/content/bin/LibertyContent.exe (repository layout)."""
    if preferences.compiler_path:
        return os.path.normpath(bpy.path.abspath(preferences.compiler_path))
    if preferences.content_root:
        root = bpy.path.abspath(preferences.content_root)
        return os.path.normpath(os.path.join(root, os.pardir, "tools", "content", "bin", "LibertyContent.exe"))
    return ""


def run(compiler, arguments, timeout_seconds):
    flags = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0
    completed = subprocess.run([compiler] + list(arguments), capture_output=True, text=True, timeout=timeout_seconds, creationflags=flags)
    return completed.returncode, (completed.stdout or "") + (completed.stderr or "")


def issues(output):
    found = []
    for line in output.splitlines():
        match = ISSUE_LINE.match(line)
        if match:
            found.append(Issue(match.group(1).lower(), match.group(2), match.group(3)))
            continue
        match = READBACK_LINE.match(line)
        if match:
            found.append(Issue("error", "READBACK", match.group(1)))
    return found


def read_report(path):
    if not os.path.isfile(path):
        return None
    with open(path, "r", encoding="utf-8") as stream:
        return json.load(stream)
