#!/bin/bash
# SessionStart hook: prepares the cloud build container (Claude Code on the web) so every session can build and run
# the offline tests immediately. Local sessions are left alone. See docs/workflow/CLOUD_LOCAL_LOOP.md.
set -uo pipefail
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
    exit 0
fi
"$CLAUDE_PROJECT_DIR/tools/cloud/setup.sh" >&2
# Stdout becomes session context: the toolchain status in a few lines.
echo "Liberty cloud toolchains (tools/cloud/setup.sh):"
cat "${LIBERTY_TOOLCHAINS:-/opt/liberty-toolchains}/setup-status.txt" 2>/dev/null
echo "Run tools/cloud/test-all.sh for every offline check. Workflow: docs/workflow/CLOUD_LOCAL_LOOP.md"
exit 0
