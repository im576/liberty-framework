#!/usr/bin/env python3
"""Read-only repository/worktree/verification inventory. No game, fetch, install or cleanup.

python tools/review/audit.py --output results-local/offline/repo-audit.json
Defaults to all registered worktrees; --repo selects the main checkout.
"""
import argparse
from collections import Counter, defaultdict
from datetime import datetime
import hashlib
import json
from pathlib import Path
import subprocess


def git(root, *args):
    return subprocess.check_output(["git", "-C", str(root), *args], text=True, encoding="utf-8")


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8-sig"))


def elapsed(run):
    try:
        return round((datetime.fromisoformat(run["finishedUtc"].replace("Z", "+00:00")) -
                      datetime.fromisoformat(run["startedUtc"].replace("Z", "+00:00"))).total_seconds(), 3)
    except (KeyError, ValueError, TypeError):
        return None


def summarize_receipts(paths):
    runs, errors, duplicates = [], [], []
    seen = {}
    identities = defaultdict(list)
    totals, failures, modes = Counter(), Counter(), Counter()
    slow = []
    for path in sorted(paths):
        try:
            data = read_json(path)
            run, checks = data["run"], data["checks"]
            if not isinstance(run, dict) or not isinstance(checks, list):
                raise ValueError("expected run object and checks array")
            identity = (run["id"], run.get("commit"), run.get("mode", "unknown"))
            digest = hashlib.sha256(json.dumps(data, sort_keys=True).encode()).hexdigest()
            if digest in seen:
                duplicates.append({"path": str(path), "sameAs": seen[digest]})
                continue
            # Validate rows before accumulating any counts from this receipt.
            for check in checks:
                if not isinstance(check, dict) or not isinstance(check.get("status"), str) or not check.get("id"):
                    raise ValueError("invalid check row")
                if not isinstance(check.get("seconds", 0), (int, float)) or check.get("seconds", 0) < 0:
                    raise ValueError("invalid check duration")
            seen[digest] = str(path)
            identities[identity].append(str(path))
            seconds = elapsed(run)
            recorded = sum(c.get("seconds", 0) for c in checks)
            item = {"path": str(path), "id": run["id"], "commit": run.get("commit"),
                    "mode": identity[2], "elapsedSeconds": seconds, "recordedCheckSeconds": recorded,
                    "unattributedSeconds": round(max(0, seconds - recorded), 3) if seconds is not None else None,
                    "packageBuildSeconds": run.get("packageBuildSeconds"),
                    "lockWaitSeconds": run.get("gameLockWaitSeconds"), "install": run.get("install"),
                    "finished": bool(run.get("finishedUtc")), "checks": checks}
            runs.append(item)
            modes[identity[2]] += 1
            for check in checks:
                totals[check["status"]] += 1
                row = {"seconds": check.get("seconds", 0), "id": check["id"], "status": check["status"],
                       "mode": identity[2], "run": run["id"], "summary": str(path), "detail": check.get("detail", "")}
                slow.append(row)
                if check["status"] in ("FAIL", "CRASH", "ERROR"):
                    failures[check["id"]] += 1
                    # Saved scenario evidence contains the failed assertion, unlike the one-line parent summary.
                    result = path.parent / check["id"] / "result.json"
                    if result.is_file():
                        try:
                            detail = read_json(result)
                            row["failedSteps"] = detail.get("failedSteps", [])
                            row["runnerError"] = detail.get("runnerError", "")
                        except (OSError, ValueError) as exc:
                            errors.append({"path": str(result), "error": str(exc)})
        except (OSError, ValueError, KeyError, TypeError) as exc:
            errors.append({"path": str(path), "error": str(exc)})
    return {"runs": runs, "counts": dict(totals), "modes": dict(modes),
            "recurringFailures": failures.most_common(),
            "slowChecks": sorted(slow, key=lambda r: r["seconds"], reverse=True)[:30],
            "failedChecks": [r for r in slow if r["status"] in ("FAIL", "CRASH", "ERROR")],
            "duplicates": duplicates, "errors": errors,
            "conflictingReceipts": [paths for paths in identities.values() if len(paths) > 1]}


def inventory(root):
    groups = defaultdict(lambda: {"files": 0, "bytes": 0})
    files, missing = [], []
    for name in git(root, "ls-files", "-z").split("\0"):
        if not name:
            continue
        path = root / name
        if not path.is_file():
            missing.append(name)
            continue
        size = path.stat().st_size
        groups[name.split("/")[0]]["files"] += 1
        groups[name.split("/")[0]]["bytes"] += size
        files.append({"path": name, "bytes": size})
    return {"groups": dict(groups), "files": len(files), "bytes": sum(f["bytes"] for f in files),
            "largest": sorted(files, key=lambda f: f["bytes"], reverse=True)[:20], "missingTracked": missing}


def audit(root):
    worktrees = [Path(line[9:]) for line in git(root, "worktree", "list", "--porcelain").splitlines()
                 if line.startswith("worktree ")]
    states, paths = [], []
    for tree in worktrees:
        if not tree.is_dir():
            states.append({"path": str(tree), "missing": True})
            continue
        states.append({"path": str(tree), "commit": git(tree, "rev-parse", "HEAD").strip(),
                       "branch": git(tree, "branch", "--show-current").strip(),
                       "status": git(tree, "status", "--porcelain").splitlines()})
        # Deliberately omit _runs: those are duplicate staging evidence, not new verification batches.
        paths.extend((tree / "results-local").glob("*/summary.json"))
    return {"schemaVersion": 1, "repo": str(root), "inventory": inventory(root), "worktrees": states,
            "verification": summarize_receipts(paths),
            "limits": ["Historical outcomes across different commits/modes; not current acceptance or a failure rate.",
                       "Elapsed time can include waits and interrupted/resumed sessions; no CPU-time attribution.",
                       "Old summaries omit package build timing; unattributed seconds are not all wasted time.",
                       "Only local results-local/*/summary.json receipts; no remote or chat/session transcripts."]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    result = audit(args.repo.resolve())
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    verification = result["verification"]
    print(f"audit: files={result['inventory']['files']} worktrees={len(result['worktrees'])} "
          f"runs={len(verification['runs'])} errors={len(verification['errors'])}")
    print("Historical check outcomes:", json.dumps(verification["counts"], sort_keys=True))
    print("Recurring failures:", verification["recurringFailures"][:10])
    print("Run modes:", verification["modes"])
    if args.output:
        print("Report:", args.output.resolve())
    for limitation in result["limits"]:
        print("Scope:", limitation)
    return 1 if verification["errors"] or verification["conflictingReceipts"] else 0


if __name__ == "__main__":
    raise SystemExit(main())
