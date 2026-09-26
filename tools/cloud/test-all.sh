#!/bin/bash
# Every offline check the cloud container can run, in one command, with a summary table:
#   tools/cloud/test-all.sh [--keep-going]      (logs in $LIBERTY_TEST_LOGS, default /tmp/liberty-test-all)
#
# Status per step: PASS, FAIL, or NOT-RUN with the reason (a missing toolchain, or a check that needs the game's files
# or Windows). A NOT-RUN step is never counted as passing; the local verification plan covers those on the PC.
# Exit code: 0 when no step failed, 1 otherwise. Steps whose inputs failed to build are NOT-RUN, not PASS.

set -uo pipefail
REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
LOGS="${LIBERTY_TEST_LOGS:-/tmp/liberty-test-all}"
TOOLCHAINS="${LIBERTY_TOOLCHAINS:-/opt/liberty-toolchains}"
RECORD="$REPO/tools/toolchains.local.json"
mkdir -p "$LOGS"
cd "$REPO" || exit 1

declare -a NAMES STATUSES DETAILS
record() { NAMES+=("$1"); STATUSES+=("$2"); DETAILS+=("$3"); printf '%-8s %s  %s\n' "$2" "$1" "$3"; }
toolchain() { python3 -c "import json,sys; print(json.load(open('$RECORD')).get('$1',''))" 2>/dev/null; }

# run <name> <log> <summary-regex> <command...>: PASS when the command exits 0. The summary line (the last line matching
# the regex) is shown as the detail; a NOT-RUN count in it is kept visible.
run() {
    local name="$1" logfile="$LOGS/$2" pattern="$3"; shift 3
    "$@" >"$logfile" 2>&1
    local code=$?
    local summary
    summary="$(grep -E "$pattern" "$logfile" | tail -1 | tr -s ' ' | cut -c1-120)"
    if [ $code -eq 0 ]; then record "$name" PASS "$summary"; else record "$name" FAIL "exit $code; ${summary:-see $logfile}"; fi
    return $code
}

missing=""
for tool in mono pwsh; do command -v "$tool" >/dev/null || missing="$missing $tool"; done
[ -f "$RECORD" ] || missing="$missing toolchains.local.json"
if [ -n "$missing" ]; then
    echo "Toolchains missing:$missing. Run tools/cloud/setup.sh (the SessionStart hook does)."
fi
have() { [ -z "$missing" ]; }

SHDN="$(toolchain scriptHookDotNet)"
BLENDER_PY="$(toolchain blenderPython)"

# 1. C#: SDK, engine, SDK mods (warnings are errors).
if have && [ -f "$SHDN" ]; then
    run "C# build (SDK, engine, mods)" build.log '^Built|failed' pwsh -NoProfile -File tools/build.ps1 -ScriptHookDotNetReference "$SHDN"
    csharp=$?
else
    record "C# build (SDK, engine, mods)" NOT-RUN "toolchain missing (mono/pwsh/roslyn/ScriptHookDotNet reference)"; csharp=1
fi

# 2. Native core for the game's target, plus its unit tests (host -m32 build off Windows).
if have && [ -n "$(toolchain clang)" ]; then
    run "Native core + unit tests" core.log 'passed|failed|error' pwsh -NoProfile -File tools/build-core.ps1
else
    record "Native core + unit tests" NOT-RUN "llvm-mingw not recorded"
fi

# 3. Content compiler and its self-test (no game).
if have; then
    if run "Content compiler build" content-build.log '^Built|failed' pwsh -NoProfile -File tools/build-content.ps1; then
        run "Content compiler self-test" content-selftest.log '^selftest:' mono tools/content/bin/LibertyContent.exe selftest
        content=$?
    else
        record "Content compiler self-test" NOT-RUN "the content compiler did not build"; content=1
    fi
else
    record "Content compiler build" NOT-RUN "toolchain missing"; record "Content compiler self-test" NOT-RUN "toolchain missing"; content=1
fi

# 4. Offline verifier, repository-only sections (the game sections report NOT-RUN inside it).
if have; then
    run "Offline verifier (-NoGame)" verify.log '^RESULT' pwsh -NoProfile -File tools/verify.ps1 -NoGame
else
    record "Offline verifier (-NoGame)" NOT-RUN "toolchain missing"
fi

# 5. Blender add-on: manifest validation and the headless tests (builds need the game; those expectations are NOT-RUN).
if [ -x "$BLENDER_PY" ]; then
    EXT="$(find "$(dirname "$(dirname "$BLENDER_PY")")" -path '*bl_pkg/cli/blender_ext.py' 2>/dev/null | head -1)"
    if [ -n "$EXT" ]; then
        run "Blender extension manifest" blender-validate.log 'Success|Error' "$BLENDER_PY" "$EXT" validate tools/blender/liberty_exporter
    else
        record "Blender extension manifest" NOT-RUN "blender_ext.py not found in the bpy module"
    fi
    if [ "$content" -eq 0 ]; then
        run "Blender add-on tests (--no-game)" blender-tests.log '^RESULT' "$BLENDER_PY" tools/blender/tests/run_tests.py -- "$REPO" --no-game "$LOGS/blender-output"
    else
        record "Blender add-on tests (--no-game)" NOT-RUN "the content compiler did not build"
    fi
else
    record "Blender extension manifest" NOT-RUN "bpy not installed"; record "Blender add-on tests (--no-game)" NOT-RUN "bpy not installed"
fi

# 6. PowerShell unit tests (tools/tests/*.Tests.ps1): autopilot result logic, verify-local orchestration, check queue.
if command -v pwsh >/dev/null && [ -f tools/tests/Run-Tests.ps1 ]; then
    run "PowerShell unit tests" pwsh-tests.log '^RESULT' pwsh -NoProfile -File tools/tests/Run-Tests.ps1
else
    record "PowerShell unit tests" NOT-RUN "pwsh or tools/tests/Run-Tests.ps1 missing"
fi

# 7. Local check queue (tests/local/checks.json) is valid and the generated plan is current.
if [ -f tools/checks/checks.py ]; then
    run "Check queue + plan in sync" checks.log '^checks:' python3 tools/checks/checks.py validate
else
    record "Check queue + plan in sync" NOT-RUN "tools/checks/checks.py missing"
fi

echo
echo "== Summary (logs: $LOGS)"
failed=0
for i in "${!NAMES[@]}"; do
    printf '%-8s %-36s %s\n' "${STATUSES[$i]}" "${NAMES[$i]}" "${DETAILS[$i]}"
    [ "${STATUSES[$i]}" = FAIL ] && failed=1
done
echo
echo "Needs the PC (never proven here): game-file sections of the verifier, builds against the game's archives,"
echo "everything in game. See docs/testing/LOCAL_VERIFICATION_PLAN.md."
exit $failed
