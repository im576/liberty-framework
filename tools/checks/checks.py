#!/usr/bin/env python3
"""The local check queue: tests/local/checks.json (single source of truth) and the plan generated from it,
docs/testing/LOCAL_VERIFICATION_PLAN.md. Cloud-side only (the PC runs tools/verify-local.ps1, which needs no Python).

    python3 tools/checks/checks.py validate   schema, unique ids, known tools/scenarios, plan up to date
    python3 tools/checks/checks.py plan       regenerate the plan from checks.json
    python3 tools/checks/checks.py list       one line per check

Schema: docs/workflow/CLOUD_LOCAL_LOOP.md#the-check-queue.
"""

import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
QUEUE = os.path.join(ROOT, "tests", "local", "checks.json")
PLAN = os.path.join(ROOT, "docs", "testing", "LOCAL_VERIFICATION_PLAN.md")
SCENARIOS = os.path.join(ROOT, "tools", "autopilot", "scenarios")

KINDS = ("pc-offline", "probe", "scenario", "manual")
STATUSES = ("QUEUED", "PASS", "FAIL", "ERROR", "CRASH", "NOT-RUN", "NEEDS-REVIEW", "RETIRED")
# Named steps tools/verify-local.ps1 knows how to run (Invoke-LocalTool). Nothing else can be queued: a check never
# carries an arbitrary shell command.
TOOLS = {
    "build": "tools/build.ps1 + tools/build-core.ps1 (engine, SDK, mods, native core and its unit tests)",
    "verify": "tools/verify.ps1 -GameDirectory <game> (every section, including GTAIV.exe and game archives)",
    "content-selftest": "tools/build-content.ps1 + LibertyContent selftest",
    "wtdcheck": "LibertyContent wtdcheck --game <game> <archives...>",
    "blender-tests": "tools/blender/run-tests.ps1 -GameDirectory <game> -Blender <blender.exe>",
    "package-install": "tools/package-phase2.ps1, then tools/install-phase2.ps1 (backup kept for rollback)",
    "content-report": "reads staging/phase2-reports/content/<asset>/report.json and checks fields",
    "probe": "LibertyContent probe <name> --game <game> --out <results>/<id>.json (structure only, never asset data)",
}
REQUIRED = ("id", "task", "title", "kind", "run", "pass", "proves", "status")
ID = re.compile(r"^[A-Z0-9]+(-[A-Za-z0-9]+)+$")


def load():
    with open(QUEUE, "r", encoding="utf-8") as stream:
        return json.load(stream)


def errors_in(queue):
    problems = []
    if queue.get("schemaVersion") != 1:
        problems.append("schemaVersion must be 1")
    seen = set()
    scenarios = {name[:-4] for name in os.listdir(SCENARIOS) if name.endswith(".txt")}
    for index, check in enumerate(queue.get("checks", [])):
        where = check.get("id", "#%d" % index)
        for field in REQUIRED:
            if field not in check or check[field] in ("", None, [], {}):
                problems.append("%s: missing %s" % (where, field))
        if not ID.match(check.get("id", "")):
            problems.append("%s: id must look like T027-raycast-scenario" % where)
        if check.get("id") in seen:
            problems.append("%s: duplicate id" % where)
        seen.add(check.get("id"))
        kind = check.get("kind")
        if kind not in KINDS:
            problems.append("%s: kind %r not in %s" % (where, kind, KINDS))
        if check.get("status") not in STATUSES:
            problems.append("%s: status %r not in %s" % (where, check.get("status"), STATUSES))
        run = check.get("run", {})
        if kind in ("pc-offline", "probe"):
            if run.get("tool") not in TOOLS:
                problems.append("%s: run.tool %r is not a known verify-local step" % (where, run.get("tool")))
            if kind == "probe" and run.get("tool") != "probe":
                problems.append("%s: a probe check must use run.tool 'probe'" % where)
        if kind == "scenario":
            if run.get("scenario") not in scenarios:
                problems.append("%s: scenario %r has no tools/autopilot/scenarios/*.txt" % (where, run.get("scenario")))
            for shot in (check.get("review") or {}).get("screenshots", {}):
                text = open(os.path.join(SCENARIOS, run.get("scenario", "") + ".txt"), encoding="utf-8").read() if run.get("scenario") in scenarios else ""
                if not re.search(r"(?m)^shot\s+%s\s*$" % re.escape(shot), text):
                    problems.append("%s: review screenshot %r is not taken by the scenario" % (where, shot))
        if kind == "manual":
            if not run.get("steps"):
                problems.append("%s: a manual check needs run.steps" % where)
            if not isinstance(check.get("minutes"), int) or check["minutes"] <= 0:
                problems.append("%s: a manual check needs minutes (> 0)" % where)
            if not check.get("session"):
                problems.append("%s: a manual check needs a session (groups the owner's play time)" % where)
        reference = check.get("reference")
        if reference:
            path = reference.split("#")[0]
            if not os.path.isfile(os.path.join(ROOT, path)):
                problems.append("%s: reference %s does not exist" % (where, path))
    sessions = {s["id"] for s in queue.get("sessions", [])}
    for check in queue.get("checks", []):
        if check.get("kind") == "manual" and check.get("session") not in sessions:
            problems.append("%s: session %r not in sessions" % (check.get("id"), check.get("session")))
    return problems


def active(queue):
    return [c for c in queue["checks"] if c["status"] != "RETIRED"]


def plan_text(queue):
    checks = active(queue)
    pending = [c for c in checks if c["status"] != "PASS"]
    by_kind = {k: [c for c in pending if c["kind"] == k] for k in KINDS}
    manual_minutes = sum(c["minutes"] for c in by_kind["manual"])
    out = []
    add = out.append
    add("# Local verification plan")
    add("")
    add("<!-- Generated by tools/checks/checks.py plan from tests/local/checks.json. Do not edit by hand. -->")
    add("")
    add("## When you are back at the PC")
    add("")
    add("1. Close GTA IV. In PowerShell at the repository root: `git checkout develop` and `git pull`.")
    add("2. First time only: `./tools/verify-local.ps1 -Smoke -GameDirectory '<GTAIV folder>'` (about 10 minutes; proves the script itself works, then restores your install).")
    add("3. Then the full run: `./tools/verify-local.ps1` (it remembers the game folder). It builds, installs, runs every automated check, and asks you about each manual check. You can skip any.")
    add("4. It pushes the results to the `verification-results` branch. Start a cloud session with: *Process the newest verification results.*")
    add("5. Nothing else to do: the cloud session fixes failures and queues new checks.")
    add("")
    add("## What is queued")
    add("")
    add("| Kind | Pending | Runs |")
    add("|---|---|---|")
    add("| pc-offline | %d | automatically (builds and tests that need Windows or the game's files) |" % len(by_kind["pc-offline"]))
    add("| probe | %d | automatically (read-only questions about the game's files; structure only) |" % len(by_kind["probe"]))
    add("| scenario | %d | automatically (autopilot drives the game; about 2-4 minutes each) |" % len(by_kind["scenario"]))
    add("| manual | %d | you play and judge; about %d minutes in total, grouped below |" % (len(by_kind["manual"]), manual_minutes))
    add("")
    add("Statuses: QUEUED (never run on the current code), PASS, FAIL, ERROR, CRASH, NOT-RUN, NEEDS-REVIEW (a person or "
        "the review session must judge screenshots or log errors).")
    add("")
    add("## Manual checks, grouped into play sessions")
    add("")
    add("Do the sessions in order; stop whenever you like. Each check links to its full steps. Answer p (pass), "
        "f (fail) or s (skip) when the script asks, and add a note for anything odd.")
    for session in queue["sessions"]:
        items = [c for c in by_kind["manual"] if c["session"] == session["id"]]
        if not items:
            continue
        add("")
        add("### %s (about %d minutes)" % (session["title"], sum(c["minutes"] for c in items)))
        add("")
        add(session["setup"])
        for check in items:
            add("")
            add("**%s** `%s` (%s, %d min)" % (check["title"], check["id"], check["task"], check["minutes"]))
            add("")
            for number, step in enumerate(check["run"]["steps"], 1):
                add("%d. %s" % (number, step))
            add("")
            add("- Pass when: %s" % check["pass"])
            if check.get("reference"):
                add("- Full steps: [%s](../../%s)" % (check["reference"], check["reference"]))
    add("")
    add("## Automated checks")
    add("")
    add("| Id | Task | Kind | What passes | Status |")
    add("|---|---|---|---|---|")
    for check in checks:
        if check["kind"] == "manual":
            continue
        add("| `%s` | %s | %s | %s | %s |" % (check["id"], check["task"], check["kind"], check["pass"].replace("|", "\\|"), check["status"]))
    add("")
    add("## What each check proves, and what stays unproven")
    add("")
    for check in checks:
        add("- `%s`: %s%s" % (check["id"], check["proves"], (" **Still unproven:** " + check["unproven"]) if check.get("unproven") else ""))
    add("")
    return "\n".join(out)


def main(argv):
    command = argv[1] if len(argv) > 1 else "validate"
    queue = load()
    if command == "plan":
        problems = errors_in(queue)
        if problems:
            print("\n".join(problems))
            return 1
        with open(PLAN, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(plan_text(queue))
        print("plan written: %s" % os.path.relpath(PLAN, ROOT))
        return 0
    if command == "list":
        for check in queue["checks"]:
            print("%-40s %-10s %-12s %s" % (check["id"], check["kind"], check["status"], check["title"]))
        return 0
    problems = errors_in(queue)
    if not os.path.isfile(PLAN):
        problems.append("plan missing: run python3 tools/checks/checks.py plan")
    else:
        with open(PLAN, "r", encoding="utf-8") as stream:
            if stream.read() != plan_text(queue):
                problems.append("plan out of date: run python3 tools/checks/checks.py plan")
    for problem in problems:
        print("ERROR " + problem)
    counts = {k: len([c for c in active(queue) if c["kind"] == k]) for k in KINDS}
    print("checks: %s checks=%d %s" % ("ok" if not problems else "FAILED", len(active(queue)), " ".join("%s=%d" % p for p in counts.items())))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
