# Verification run 20260928-215323-e0a48d9

- Commit: e0a48d9 (HEAD)  Mode: smoke
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T04:53:23.5030581Z  Finished: 2026-09-29T04:54:37.9878905Z
- Install: not installed
- Counts: PASS=1 NEEDS-REVIEW=0 FAIL=1 CRASH=0 ERROR=0 NOT-RUN=1

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-build` | pc-offline | **PASS** | exit code 0 | LOOP-build.log |
| `LOOP-package-install` | pc-offline | **FAIL** | package-phase2.ps1 failed (exit 1) | LOOP-package-install.log |
| `SDK-selftest` | scenario | **NOT-RUN** | LOOP-package-install did not pass in this run |  |
