#!/bin/bash
# Cloud build container setup (Claude Code on the web, Ubuntu 24.04, no game files). Run by the SessionStart hook
# (.claude/hooks/session-start.sh) and safe to run by hand. Idempotent: every step checks before it downloads.
#
# Provides, outside the repository (LIBERTY_TOOLCHAINS, default /opt/liberty-toolchains):
#   mono + PowerShell 7 + 32-bit host C++ libraries  (apt: Ubuntu archive, packages.microsoft.com)
#   Roslyn 4.11.0 (C# 7.3)                          (the NuGet package tools/get-toolchains.ps1 pins, same hash)
#   ScriptHookDotNet 1.7.1.8 compile reference       (the release archive third_party/README.md pins, same hash; never committed)
#   llvm-mingw 20260922, Linux host build            (the i686 target compiler tools/build-core.ps1 uses)
#   Blender 5.2.2 as the bpy Python module (3.13)    (PyPI; download.blender.org is not reachable from the container)
# and records the compiler folders in tools/toolchains.local.json (git-ignored) the way get-toolchains.ps1 does.
#
# Never fails the session: a step that cannot finish is reported in $LIBERTY_TOOLCHAINS/setup-status.txt and by
# tools/cloud/test-all.sh as NOT RUN with the reason.

set -uo pipefail

REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
TOOLCHAINS="${LIBERTY_TOOLCHAINS:-/opt/liberty-toolchains}"
DOWNLOADS="$TOOLCHAINS/downloads"
STATUS="$TOOLCHAINS/setup-status.txt"
mkdir -p "$DOWNLOADS"
: > "$STATUS"

ROSLYN_URL="https://api.nuget.org/v3-flatcontainer/microsoft.net.compilers.toolset/4.11.0/microsoft.net.compilers.toolset.4.11.0.nupkg"
ROSLYN_SHA="db165c75f0d0386d078d01448f96c93f8222320633e4fe828f5d1bf8c99b7d6f"
SHDN_URL="https://github.com/Tomasak/gta4_scripthookdotnet/releases/download/release/scripthookdotnet_v1.7.1.8.zip"
SHDN_SHA="5669e4423f93bedfb0ae34579e922213775b46bbee4db6adc953cb53e7ad9058"
MINGW_NAME="llvm-mingw-20260922-ucrt-ubuntu-22.04-x86_64"
MINGW_URL="https://github.com/mstorsjo/llvm-mingw/releases/download/20260922/$MINGW_NAME.tar.xz"
MINGW_SHA="bb7bb7654b33d5aa8712acb837c963b2e0c56352560c76105270a3268c665c21"
BPY_VERSION="5.2.2"

log() { echo "[liberty setup] $*"; }
ok() { echo "ok      $1" >> "$STATUS"; log "ok: $1"; }
fail() { echo "missing $1 ($2)" >> "$STATUS"; log "MISSING: $1 ($2)"; }

# Downloads $1 to $DOWNLOADS/$2 unless present, and checks its SHA-256 against $3. A mismatching file is deleted.
fetch() {
    local url="$1" file="$DOWNLOADS/$2" sha="$3"
    if [ ! -f "$file" ]; then
        curl -fsSL --retry 3 -o "$file.part" "$url" && mv "$file.part" "$file" || { rm -f "$file.part"; return 1; }
    fi
    if [ "$(sha256sum "$file" | cut -d' ' -f1)" != "$sha" ]; then
        log "hash mismatch for $2; deleting it"
        rm -f "$file"
        return 1
    fi
}

# 1. System packages.
host32() { echo 'int main(){}' | clang++ -m32 -x c++ -o /dev/null - >/dev/null 2>&1; }
if ! command -v mono >/dev/null || ! command -v pwsh >/dev/null || ! host32; then
    log "installing mono, PowerShell and 32-bit C++ libraries (apt; a few minutes on a new container)"
    if [ ! -f /etc/apt/sources.list.d/microsoft-prod.list ]; then
        curl -fsSL -o "$DOWNLOADS/packages-microsoft-prod.deb" https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb &&
            dpkg -i "$DOWNLOADS/packages-microsoft-prod.deb" >/dev/null
    fi
    # Unreachable third-party PPAs make apt-get update exit non-zero; the Ubuntu and Microsoft indexes still update.
    apt-get update -qq >/dev/null 2>&1 || true
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq mono-complete powershell g++-multilib clang >"$TOOLCHAINS/apt.log" 2>&1 ||
        log "apt-get install reported an error; see $TOOLCHAINS/apt.log"
fi
command -v mono >/dev/null && ok "mono $(mono --version | head -1 | awk '{print $5}')" || fail mono "apt-get install mono-complete failed"
command -v pwsh >/dev/null && ok "pwsh $(pwsh --version | awk '{print $2}')" || fail pwsh "apt-get install powershell failed"
host32 && ok "clang++ -m32 (host unit tests)" || fail "clang++ -m32" "apt-get install g++-multilib failed"

# 2. Roslyn (the same pinned package as tools/get-toolchains.ps1).
ROSLYN="$TOOLCHAINS/roslyn-4.11.0"
if [ ! -f "$ROSLYN/tasks/net472/csc.exe" ]; then
    fetch "$ROSLYN_URL" roslyn-4.11.0.nupkg "$ROSLYN_SHA" && mkdir -p "$ROSLYN" && unzip -qo "$DOWNLOADS/roslyn-4.11.0.nupkg" -d "$ROSLYN"
fi
[ -f "$ROSLYN/tasks/net472/csc.exe" ] && ok "roslyn 4.11.0" || fail roslyn "download or hash check failed: $ROSLYN_URL"

# 3. ScriptHookDotNet compile reference (third_party/README.md; kept outside the repository, never committed).
SHDN="$TOOLCHAINS/scripthookdotnet-1.7.1.8"
if [ ! -f "$SHDN/ScriptHookDotNet.asi" ]; then
    fetch "$SHDN_URL" scripthookdotnet_v1.7.1.8.zip "$SHDN_SHA" && mkdir -p "$SHDN" && unzip -qo "$DOWNLOADS/scripthookdotnet_v1.7.1.8.zip" -d "$SHDN"
fi
[ -f "$SHDN/ScriptHookDotNet.asi" ] && ok "ScriptHookDotNet 1.7.1.8 reference" || fail scripthookdotnet "download or hash check failed: $SHDN_URL"

# 4. llvm-mingw (Linux host build of the pinned release).
MINGW="$TOOLCHAINS/$MINGW_NAME"
if [ ! -x "$MINGW/bin/i686-w64-mingw32-clang++" ]; then
    fetch "$MINGW_URL" "$MINGW_NAME.tar.xz" "$MINGW_SHA" && tar -xJf "$DOWNLOADS/$MINGW_NAME.tar.xz" -C "$TOOLCHAINS"
fi
[ -x "$MINGW/bin/i686-w64-mingw32-clang++" ] && ok "llvm-mingw 20260922" || fail llvm-mingw "download or hash check failed: $MINGW_URL"

# 5. Blender as a Python module (bpy needs Python 3.13; uv fetches it).
BLENDER_PY="$TOOLCHAINS/blender-py"
if ! "$BLENDER_PY/bin/python" -c "import bpy, sys; sys.exit(bpy.app.version_string.split()[0] != '$BPY_VERSION')" >/dev/null 2>&1; then
    command -v uv >/dev/null || python3 -m pip install -q uv >/dev/null 2>&1 || true
    UV="$(command -v uv || echo "$HOME/.local/bin/uv")"
    log "installing bpy $BPY_VERSION (about 400 MB)"
    "$UV" venv -q --allow-existing --python 3.13 "$BLENDER_PY" >/dev/null 2>&1 &&
        VIRTUAL_ENV="$BLENDER_PY" "$UV" pip install -q "bpy==$BPY_VERSION" >"$TOOLCHAINS/bpy.log" 2>&1
fi
if "$BLENDER_PY/bin/python" -c "import bpy" >/dev/null 2>&1; then ok "blender (bpy) $BPY_VERSION"; else fail blender "uv/pip install of bpy $BPY_VERSION failed; see $TOOLCHAINS/bpy.log"; fi

# 6. Record the toolchains for the build scripts (tools/toolchains.ps1 reads roslyn and clang; the rest is for test-all).
cat > "$REPO/tools/toolchains.local.json" <<EOF
{
  "roslyn": "$ROSLYN/tasks/net472",
  "clang": "$MINGW/bin",
  "scriptHookDotNet": "$SHDN/ScriptHookDotNet.asi",
  "blenderPython": "$BLENDER_PY/bin/python"
}
EOF
if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
    echo "export LIBERTY_TOOLCHAINS=\"$TOOLCHAINS\"" >> "$CLAUDE_ENV_FILE"
fi
log "done; status in $STATUS"
cat "$STATUS"
exit 0
