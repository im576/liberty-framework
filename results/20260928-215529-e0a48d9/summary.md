# Verification run 20260928-215529-e0a48d9

- Commit: e0a48d9 (HEAD)  Mode: smoke
- Game: 1.2.0.59  exe SHA-256 08759A5516F9837920EA504436236BBAB89D0826A8E4D04FF106345177B5345D
- Started: 2026-09-29T04:55:29.8412277Z  Finished: 2026-09-29T05:04:51.7611851Z
- Install: restored from <game>\scripts\LibertyFramework\backups\phase2-20260928-220124
- Counts: PASS=2 NEEDS-REVIEW=0 FAIL=1 CRASH=0 ERROR=0 NOT-RUN=0

| Check | Kind | Status | Detail | Evidence |
|---|---|---|---|---|
| `LOOP-build` | pc-offline | **PASS** | exit code 0 | LOOP-build.log |
| `LOOP-package-install` | pc-offline | **PASS** | installed; backup <game>\scripts\LibertyFramework\backups\phase2-20260928-220124 | LOOP-package-install.log, LOOP-package-install-install.log |
| `SDK-selftest` | scenario | **FAIL** | FAIL; steps=13 failed=1 logErrors=1 gameAlive=True | SDK-selftest.log, SDK-selftest |
